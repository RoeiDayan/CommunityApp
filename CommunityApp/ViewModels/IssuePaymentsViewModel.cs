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
    public class IssuePaymentsViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;

        public IssuePaymentsViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;

            // Initialize commands
            LoadMembersCommand = new AsyncRelayCommand(LoadMembers);
            IssueToAllMembersCommand = new AsyncRelayCommand(IssuePaymentToAllMembers);
            IssueToMemberCommand = new AsyncRelayCommand<MemberAccount>(IssuePaymentToMember);

            // Initialize date pickers with current date
            PayFromDate = DateOnly.FromDateTime(DateTime.Today);
            PayUntilDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));

            // Initial data load
            _ = LoadMembers();
        }

        #region Properties
        private ObservableCollection<MemberAccount> members = new ObservableCollection<MemberAccount>();
        public ObservableCollection<MemberAccount> Members
        {
            get => members;
            set
            {
                members = value ?? new ObservableCollection<MemberAccount>();
                OnPropertyChanged(nameof(Members));
            }
        }

        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                OnPropertyChanged(nameof(Amount));
                OnPropertyChanged(nameof(IsAmountValid));
            }
        }

        private string details = string.Empty;
        public string Details
        {
            get => details;
            set
            {
                details = value ?? string.Empty;
                OnPropertyChanged(nameof(Details));
            }
        }

        private DateOnly payFromDate;
        public DateOnly PayFromDate
        {
            get => payFromDate;
            set
            {
                payFromDate = value;
                OnPropertyChanged(nameof(PayFromDate));
            }
        }

        private DateOnly payUntilDate;
        public DateOnly PayUntilDate
        {
            get => payUntilDate;
            set
            {
                payUntilDate = value;
                OnPropertyChanged(nameof(PayUntilDate));
            }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        private bool isProcessing;
        public bool IsProcessing
        {
            get => isProcessing;
            set
            {
                isProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }

        public bool IsAmountValid => Amount > 0;
        #endregion

        #region Commands
        public IAsyncRelayCommand LoadMembersCommand { get; }
        public IAsyncRelayCommand IssueToAllMembersCommand { get; }
        public IAsyncRelayCommand<MemberAccount> IssueToMemberCommand { get; }
        #endregion

        #region Methods
        private async Task LoadMembers()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Members.Clear();

                int currentCommunityId = ((App)Application.Current).CurCom.ComId;
                List<MemberAccount> allMembers = await proxy.GetSelectCommunityMemberAccountsAsync(currentCommunityId, true) ?? new List<MemberAccount>();

                foreach (var member in allMembers)
                {
                    Members.Add(member);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to load community members. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error loading members: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task IssuePaymentToAllMembers()
        {
            if (!ValidatePaymentInput())
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Payment Issue",
                $"Are you sure you want to issue a payment of {Amount} to all {Members.Count} members?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                IsProcessing = true;
                int currentCommunityId = ((App)Application.Current).CurCom.ComId;

                var payment = new Payment
                {
                    ComId = currentCommunityId,
                    UserId = 0,
                    Amount = Amount,
                    Details = string.IsNullOrWhiteSpace(Details) ? null : Details,
                    WasPayed = false,
                    PayFrom = PayFromDate,
                    PayUntil = PayUntilDate
                };

                bool success = await proxy.IssuePaymentToAllMembersAsync(payment);

                if (success)
                {
                    var toast = Toast.Make($"Payment issued successfully to all {Members.Count} members");
                    await toast.Show();

                    // Clear form after successful submission
                    ClearForm();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to issue payment to all members. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "An error occurred while issuing payment. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error issuing payment to all: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task IssuePaymentToMember(MemberAccount memberAccount)
        {
            if (memberAccount == null || !ValidatePaymentInput())
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Payment Issue",
                $"Issue payment of {Amount} to {memberAccount.Account.Name}?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                IsProcessing = true;
                int currentCommunityId = ((App)Application.Current).CurCom.ComId;

                var payment = new Payment
                {
                    ComId = currentCommunityId,
                    UserId = memberAccount.Account.Id,
                    Amount = Amount,
                    Details = string.IsNullOrWhiteSpace(Details) ? null : Details,
                    WasPayed = false,
                    PayFrom = PayFromDate,
                    PayUntil = PayUntilDate
                };

                bool success = await proxy.IssuePaymentToMemberAsync(payment);

                if (success)
                {
                    var toast = Toast.Make($"Payment issued successfully to {memberAccount.Account.Name}");
                    await toast.Show();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to issue payment. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "An error occurred while issuing payment. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error issuing payment to member: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private bool ValidatePaymentInput()
        {
            if (Amount <= 0)
            {
                Application.Current.MainPage.DisplayAlert("Validation Error", "Amount must be greater than 0.", "OK");
                return false;
            }

            if (PayFromDate > PayUntilDate)
            {
                Application.Current.MainPage.DisplayAlert("Validation Error", "Pay From date cannot be after Pay Until date.", "OK");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            Amount = 0;
            Details = string.Empty;
            PayFromDate = DateOnly.FromDateTime(DateTime.Today);
            PayUntilDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
        }
        #endregion
    }
}