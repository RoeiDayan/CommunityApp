using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityApp.ViewModels
{
    public class SelectCommunityViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;

        public SelectCommunityViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.proxy = proxy;
            this.serviceProvider = serviceProvider;
            MemberCommunities = new ObservableCollection<MemberCommunity>();
            SignInCommand = new Command<int>(OnSignIn);
            JoinCommand = new Command(JoinCommunity);
            GoToLoginCommand = new Command(GoToLogin);
            LoadCommunities();
        }

        #region Communities
        private ObservableCollection<MemberCommunity> memberCommunities;
        public ObservableCollection<MemberCommunity> MemberCommunities
        {
            get => memberCommunities;
            set
            {
                memberCommunities = value;
                OnPropertyChanged(nameof(MemberCommunities));
            }
        }

        private ObservableCollection<Community> communities;
        public ObservableCollection<Community> Communities
        {
            get => communities;
            set
            {
                communities = value;
                OnPropertyChanged(nameof(Communities));
            }
        }
        #endregion

        #region Commands
        public Command<int> SignInCommand { get; }
        public Command JoinCommand {  get; }
        public Command GoToLoginCommand { get; }
        private async void OnSignIn(int comId)
        {
            bool found = false;
            MemberCommunity ChosenMemCom = new MemberCommunity();
             foreach(MemberCommunity m in MemberCommunities)
             { 
                if(m.Community.ComId == comId)
                {
                    ChosenMemCom.Community = m.Community;
                    ChosenMemCom.Member = m.Member;
                    found = true;
                    break;
                }
             }
            if (found)
            {
                ((App)Application.Current).CurCom = ChosenMemCom.Community;
                ((App)Application.Current).CurMem = ChosenMemCom.Member;

                //Navigate to the main page
                AppShell v = serviceProvider.GetService<AppShell>();
                ((App)Application.Current).MainPage = v;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to sign into the community", "OK");
            }
        }

        #endregion

        #region Methods
        private async void LoadCommunities()
        {
            try
            {
                InServerCall = true;
                int userId = ((App)Application.Current).LoggedInUser.Id;
                var userCommunities = await proxy.GetUserCommunitiesAsync(userId);
                InServerCall = false;

                if (userCommunities != null)
                {
                    //Created MemberCommunity to link a member to a community so on login you know where to go

                    MemberCommunities = new ObservableCollection<MemberCommunity>(userCommunities);
                    Communities = new ObservableCollection<Community>();
                    foreach (MemberCommunity m in MemberCommunities)
                        Communities.Add(m.Community);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to load communities. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                InServerCall = false;
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
        private async void JoinCommunity()
        {
            JoinCommunityView v = serviceProvider.GetService<JoinCommunityView>();
            ((App)Application.Current).MainPage = v;
        }


        private async void GoToLogin()
        {
            LoginView v = serviceProvider.GetService<LoginView>();
            ((App)Application.Current).MainPage = v;
        }


        #endregion
    }
}
