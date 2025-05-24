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
    public class ManagePaymentsViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;

        public ManagePaymentsViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;

            // Initialize commands
            FetchPaymentsCommand = new AsyncRelayCommand(FetchPayments);
            MarkAsPaidCommand = new AsyncRelayCommand<PaymentMemberAccount>(MarkAsPaid);
            DeletePaymentCommand = new AsyncRelayCommand<PaymentMemberAccount>(DeletePayment);
            ToggleFilterCommand = new Command<string>(FilterPayments);

            // Initial data load
            _ = FetchPayments();
        }

        #region Properties
        private ObservableCollection<PaymentMemberAccount> allPayments = new ObservableCollection<PaymentMemberAccount>();
        private ObservableCollection<PaymentMemberAccount> filteredPayments = new ObservableCollection<PaymentMemberAccount>();

        public ObservableCollection<PaymentMemberAccount> Payments
        {
            get => filteredPayments;
            set
            {
                filteredPayments = value ?? new ObservableCollection<PaymentMemberAccount>();
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
        #endregion

        #region Commands
        public IAsyncRelayCommand FetchPaymentsCommand { get; }
        public IAsyncRelayCommand<PaymentMemberAccount> MarkAsPaidCommand { get; }
        public IAsyncRelayCommand<PaymentMemberAccount> DeletePaymentCommand { get; }
        public ICommand ToggleFilterCommand { get; }
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
                List<PaymentMemberAccount> payments = await proxy.GetCommunityPaymentsAsync(currentCommunityId) ?? new List<PaymentMemberAccount>();

                // Don't clear if this is a refresh since we already did above
                if (!_hasLoaded)
                {
                    allPayments.Clear();
                }

                // Add all payments to the collection
                foreach (var payment in payments)
                {
                    allPayments.Add(payment);
                }

                // Mark as loaded
                _hasLoaded = true;

                // Apply current filter
                ApplyFilter(FilterMode);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to load payments. Please try again.", "OK");
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
                    Payments = new ObservableCollection<PaymentMemberAccount>(
                        allPayments.Where(p => p.Payment.WasPayed==true));
                    break;
                case "unpaid":
                    Payments = new ObservableCollection<PaymentMemberAccount>(
                        allPayments.Where(p => !p.Payment.WasPayed == true));
                    break;
                case "all":
                default:
                    Payments = new ObservableCollection<PaymentMemberAccount>(allPayments);
                    break;
            }
        }

        private async Task MarkAsPaid(PaymentMemberAccount paymentMemberAccount)
        {
            if (paymentMemberAccount?.Payment == null)
                return;

            try
            {
                // Update the payment status locally first
                paymentMemberAccount.Payment.WasPayed = true;

                bool success = await proxy.UpdatePaymentAsync(paymentMemberAccount.Payment);


                if (success)
                {
                    // Update the local collection
                    var localPayment = allPayments.FirstOrDefault(p => p.Payment.PaymentId == paymentMemberAccount.Payment.PaymentId);
                    if (localPayment != null)
                    {
                        localPayment.Payment.WasPayed = true;
                    }

                    // Reapply the filter
                    ApplyFilter(FilterMode);

                    // Show success toast
                    var toast = Toast.Make("Payment marked as paid successfully");
                    await toast.Show();
                }
                else
                {
                    // Revert the change if server update failed
                    paymentMemberAccount.Payment.WasPayed = false;

                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to update payment status. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Revert the change if an error occurred
                paymentMemberAccount.Payment.WasPayed = false;

                await Application.Current.MainPage.DisplayAlert("Error",
                    "An error occurred while updating the payment.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error marking payment as paid: {ex.Message}");
            }
        }

        private async Task DeletePayment(PaymentMemberAccount paymentMemberAccount)
        {
            if (paymentMemberAccount?.Payment == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete this payment of ${paymentMemberAccount.Payment.Amount:F2}?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                // Delete the payment from the server
                bool success = await proxy.DeleteMemberPaymentAsync(paymentMemberAccount.Payment.PaymentId);

                if (success)
                {
                    // Remove from local collection
                    var paymentToRemove = allPayments.FirstOrDefault(p => p.Payment.PaymentId == paymentMemberAccount.Payment.PaymentId);
                    if (paymentToRemove != null)
                    {
                        allPayments.Remove(paymentToRemove);
                    }

                    // Reapply the filter
                    ApplyFilter(FilterMode);

                    // Show success toast
                    var toast = Toast.Make("Payment deleted successfully");
                    await toast.Show();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to delete payment. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to delete payment. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error deleting payment: {ex.Message}");
            }
        }


        #endregion
    }
}