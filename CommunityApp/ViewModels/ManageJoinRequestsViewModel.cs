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

namespace CommunityApp.ViewModels
{
    public class ManageJoinRequestsViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;

        public ManageJoinRequestsViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;

            // Initialize commands
            FetchPendingMembersCommand = new Command(async () => await FetchPendingMembers());
            CopyToClipboardCommand = new Command<string>(async (textToCopy) => await CopyTextToClipboard(textToCopy));
            ApproveMemberCommand = new Command<MemberAccount>(async (memberAccount) => await ApproveMember(memberAccount));
            RejectMemberCommand = new Command<MemberAccount>(async (memberAccount) => await RejectMember(memberAccount));

            // Initial data load
            _ = FetchPendingMembers();
        }

        #region Properties
        private ObservableCollection<MemberAccount> pendingMembers = new ObservableCollection<MemberAccount>();
        public ObservableCollection<MemberAccount> PendingMembers
        {
            get => pendingMembers;
            set
            {
                pendingMembers = value ?? new ObservableCollection<MemberAccount>();
                OnPropertyChanged(nameof(PendingMembers));
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
        #endregion

        #region Commands
        public ICommand FetchPendingMembersCommand { get; }
        public ICommand CopyToClipboardCommand { get; }
        public ICommand ApproveMemberCommand { get; }
        public ICommand RejectMemberCommand { get; }
        #endregion

        #region Methods
        private async Task FetchPendingMembers()
        {
            try
            {
                IsRefreshing = true;

                int currentCommunityId = ((App)Application.Current).CurCom.ComId;
                List<MemberAccount> pendingMembersFromServer = await proxy.GetUnapprovedCommunityMemberAccountsAsync(currentCommunityId);

                PendingMembers.Clear();
                foreach (var member in pendingMembersFromServer)
                {
                    PendingMembers.Add(member);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to retrieve pending requests. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error fetching pending members: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task ApproveMember(MemberAccount memberAccount)
        {
            if (memberAccount == null)
                return;

            try
            {
                // Update the approval status
                memberAccount.Member.IsApproved = true;

                // If no role is selected, default to resident
                if (!(memberAccount.Member.IsResident == true) &&
                    !(memberAccount.Member.IsLiable == true) &&
                    !(memberAccount.Member.IsManager == true) &&
                    !(memberAccount.Member.IsProvider == true))
                {
                    memberAccount.Member.IsResident = true;
                }

                // Set role text based on selected checkboxes
                UpdateMemberRole(memberAccount.Member);

                // Send update to server
                bool success = await proxy.UpdateMemberAsync(memberAccount.Member);

                if (success)
                {
                    // Remove from local collection
                    PendingMembers.Remove(memberAccount);

                    // Show success toast
                    var toast = Toast.Make($"{memberAccount.Account.Name} approved successfully");
                    await toast.Show();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to approve member. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "An error occurred while processing your request.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error approving member: {ex.Message}");
            }
        }

        private async Task RejectMember(MemberAccount memberAccount)
        {
            if (memberAccount == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Rejection",
                $"Are you sure you want to reject {memberAccount.Account.Name}'s request?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                // Here we would typically delete the member record
                // For now, let's just remove from local collection as an example
                PendingMembers.Remove(memberAccount);

                // In a real implementation, you might want to:
                // 1. Call an API to delete the member
                // 2. Or have an API to reject (which might set a different status)

                var toast = Toast.Make($"{memberAccount.Account.Name}'s request rejected");
                await toast.Show();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to reject member request. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error rejecting member: {ex.Message}");
            }
        }

        private void UpdateMemberRole(Member member)
        {
            List<string> roles = new List<string>();

            if (member.IsResident == true) roles.Add("Resident");
            if (member.IsLiable == true) roles.Add("Liable");
            if (member.IsManager == true) roles.Add("Manager");
            if (member.IsProvider == true) roles.Add("Provider");

            member.Role = roles.Count > 0 ? string.Join(", ", roles) : "Member";
        }

        private async Task CopyTextToClipboard(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    await Clipboard.SetTextAsync(text);
                    string message = text.Contains('@') ?
                        "Email copied to clipboard" :
                        (text.All(char.IsDigit) ? "Phone number copied to clipboard" : "Text copied to clipboard");

                    var toast = Toast.Make(message);
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