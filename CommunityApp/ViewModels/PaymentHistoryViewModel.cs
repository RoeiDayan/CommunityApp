using CommunityApp.Models;
using CommunityApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace CommunityApp.ViewModels
{
    public class PaymentHistoryViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;

        public PaymentHistoryViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;

            // Initialize commands
            FetchPaymentsCommand = new AsyncRelayCommand(FetchPayments);
            ToggleFilterCommand = new Command<string>(FilterPayments);
            CopyToClipboardCommand = new AsyncRelayCommand<string>(CopyTextToClipboard);

            // Initial data load
            _ = FetchPayments();
        }

        #region Properties
        private ObservableCollection<Payment> allPayments = new ObservableCollection<Payment>();
        private ObservableCollection<Payment> filteredPayments = new ObservableCollection<Payment>();

        public ObservableCollection<Payment> Payments
        {
            get => filteredPayments;
            set
            {
                filteredPayments = value ?? new ObservableCollection<Payment>();
                OnPropertyChanged(nameof(Payments));
            }
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        private string filterMode = "all";
        public string FilterMode
        {
            get => filterMode;
            set
            {
                filterMode = value;
                OnPropertyChanged(nameof(FilterMode));
                OnPropertyChanged(nameof(IsAllFilterSelected));
                OnPropertyChanged(nameof(IsPaidFilterSelected));
                OnPropertyChanged(nameof(IsUnpaidFilterSelected));
            }
        }

        public bool IsAllFilterSelected => FilterMode == "all";
        public bool IsPaidFilterSelected => FilterMode == "paid";
        public bool IsUnpaidFilterSelected => FilterMode == "unpaid";

        private decimal totalPaid;
        public decimal TotalPaid
        {
            get => totalPaid;
            set
            {
                totalPaid = value;
                OnPropertyChanged(nameof(TotalPaid));
            }
        }

        private decimal totalUnpaid;
        public decimal TotalUnpaid
        {
            get => totalUnpaid;
            set
            {
                totalUnpaid = value;
                OnPropertyChanged(nameof(TotalUnpaid));
            }
        }
        #endregion

        #region Commands
        public IAsyncRelayCommand FetchPaymentsCommand { get; }
        public ICommand ToggleFilterCommand { get; }
        public IAsyncRelayCommand<string> CopyToClipboardCommand { get; }
        #endregion

        #region Methods
        private bool _hasLoaded = false;

        private async Task FetchPayments()
        {
            if (IsRefreshing) return; // Already refreshing

            try
            {
                IsRefreshing = true;

                // If this is a refresh (not initial load), clear data first
                if (_hasLoaded)
                {
                    allPayments.Clear();
                }

                int currentCommunityId = ((App)Application.Current).CurCom.ComId;
                int currentUserId = ((App)Application.Current).LoggedInUser.Id;

                List<Payment> payments = await proxy.GetMemberPaymentsAsync(currentCommunityId, currentUserId) ?? new List<Payment>();

                // Don't clear if this is a refresh since we already did above
                if (!_hasLoaded)
                {
                    allPayments.Clear();
                }

                foreach (var payment in payments.OrderByDescending(p => p.PayFrom))
                {
                    allPayments.Add(payment);
                }

                // Mark as loaded
                _hasLoaded = true;

                // Calculate totals
                CalculateTotals();

                // Apply current filter
                ApplyFilter(FilterMode);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to load payment history. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error fetching payments: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private void FilterPayments(string filterMode)
        {
            FilterMode = filterMode;
            ApplyFilter(filterMode);
        }

        private void ApplyFilter(string filterMode)
        {
            switch (filterMode)
            {
                case "paid":
                    Payments = new ObservableCollection<Payment>(
                        allPayments.Where(p => p.WasPayed == true));
                    break;
                case "unpaid":
                    Payments = new ObservableCollection<Payment>(
                        allPayments.Where(p => p.WasPayed != true));
                    break;
                case "all":
                default:
                    Payments = new ObservableCollection<Payment>(allPayments);
                    break;
            }
        }

        private void CalculateTotals()
        {
            TotalPaid = allPayments.Where(p => p.WasPayed == true).Sum(p => p.Amount);
            TotalUnpaid = allPayments.Where(p => p.WasPayed != true).Sum(p => p.Amount);
        }

        private async Task CopyTextToClipboard(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    await Clipboard.SetTextAsync(text);
                    var toast = Toast.Make("Payment details copied to clipboard");
                    await toast.Show();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error copying text: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", "Could not copy to clipboard", "OK");
                }
            }
        }
        #endregion
    }
}