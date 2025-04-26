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
    public class ManageMembersViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;

        public ManageMembersViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;

            // Initialize commands
            FetchMembersCommand = new Command(async () => await FetchMembers());
            CopyToClipboardCommand = new Command<string>(async (textToCopy) => await CopyTextToClipboard(textToCopy));
            UpdateMemberCommand = new Command<MemberAccount>(async (memberAccount) => await UpdateMember(memberAccount));

            // Initial data load
            _ = FetchMembers();
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
        public ICommand FetchMembersCommand { get; }
        public ICommand CopyToClipboardCommand { get; }
        public ICommand UpdateMemberCommand { get; }
        #endregion

        #region Methods
        private async Task FetchMembers()
        {
            try
            {
                IsRefreshing = true;

                int currentCommunityId = ((App)Application.Current).CurCom.ComId;
                List<MemberAccount> MembersFromServer = await proxy.GetSelectCommunityMemberAccountsAsync(currentCommunityId, true);

                Members.Clear();
                foreach (var member in MembersFromServer)
                {
                    Members.Add(member);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to retrieve members. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error fetching  members: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task UpdateMember(MemberAccount memberAccount)
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
                    

                    // Show success toast
                    var toast = Toast.Make($"{memberAccount.Account.Name} updated successfully");
                    await toast.Show();
                    await FetchMembers();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to update member. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "An error occurred while processing your request.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error updating member: {ex.Message}");
            }
        }


        private void UpdateMemberRole(Member member)
        {
            string role = "";

            if (member.IsResident == true)
                role += "Resident, ";
            if (member.IsLiable == true)
                role += "Liable, ";
            if (member.IsManager == true)
                role += "Manager, ";
            if (member.IsProvider == true)
                role += "Provider, ";


            if (role.Length > 0)
                role = role.Substring(0, role.Length - 2);
            else
                role = "Member";

            member.Role = role;
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
