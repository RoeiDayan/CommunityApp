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
        private ObservableCollection<Member> members = new ObservableCollection<Member>();
        public ObservableCollection<Member> Members
        {
            get => members;
            set
            {
                members = value ?? new ObservableCollection<Member>();
                OnPropertyChanged(nameof(Members));
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
                List<Member> membersFromServer = await proxy.GetCommunityMembersAsync(((App)Application.Current).CurCom.ComId);
                Members.Clear();
                foreach (var member in membersFromServer)
                {
                    Members.Add(member);
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
