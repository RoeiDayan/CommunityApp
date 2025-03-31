using CommunityApp.Models;
using CommunityApp.Services;
using System;
using CommunityToolkit.Maui;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace CommunityApp.ViewModels
{
    public class MemberListViewModel:ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;

        public MemberListViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            _=FetchCommunityUsers();
            FetchMembersCommand = new Command(async () => await FetchCommunityUsers());
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
        public Command FetchMembersCommand { get; }

        #endregion

        #region Methods
        private async Task FetchCommunityUsers()
        {
            try
            {
                List<MemberAccount> memberAccountsFromServer = await proxy.GetCommunityMemberAccountsAsync(((App)Application.Current).CurCom.ComId);
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
        #endregion
    }
}
