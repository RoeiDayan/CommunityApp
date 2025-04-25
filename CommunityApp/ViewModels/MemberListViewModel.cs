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

namespace CommunityApp.ViewModels
{
    public class MemberListViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;

        public MemberListViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            _ = FetchCommunityUsers();
            FetchMembersCommand = new Command(async () => await FetchCommunityUsers());
            CopyToClipboardCommand = new Command<string>(async (textToCopy) => await CopyTextToClipboard(textToCopy));
        }

        #region Properties
        private ObservableCollection<MemberAccount> memAcc = new ObservableCollection<MemberAccount>();
        public ObservableCollection<MemberAccount> MemAcc
        {
            get => memAcc;
            set
            {
                memAcc = value ?? new ObservableCollection<MemberAccount>();
                OnPropertyChanged(nameof(MemAcc));
            }
        }

        #endregion

        #region Commands
        public ICommand FetchMembersCommand { get; }
        public Command<string> CopyToClipboardCommand { get; }

        #endregion

        #region Methods
        private async Task FetchCommunityUsers()
        {
            try
            {
                List<MemberAccount> memberAccountsFromServer = await proxy.GetApprovedCommunityMemberAccountsAsync(((App)Application.Current).CurCom.ComId);
                MemAcc.Clear();
                foreach (var ma in memberAccountsFromServer)
                {
                    MemAcc.Add(ma);
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Failed to retrieve members", "Encountered a problem with fetching the members. Please try again", "ok");
            }
        }

        private async Task CopyTextToClipboard(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    await Clipboard.SetTextAsync(text);
                    string message = text.Contains('@') ? "Email copied to clipboard" : (text.All(char.IsDigit) ? "Phone number copied to clipboard" : "Text copied to clipboard");
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