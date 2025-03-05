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
            MemberCommunities = new ObservableCollection<MemberCommunity>();
            SignInCommand = new Command<int>(OnSignIn);
            LoadCommunities();
            this.serviceProvider = serviceProvider;
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

        #region SelectedCommunity
        private MemberCommunity selectedCommunity;
        public MemberCommunity SelectedCommunity
        {
            get => selectedCommunity;
            set
            {
                selectedCommunity = value;
                OnPropertyChanged(nameof(SelectedCommunity));
            }
        }
        #endregion

        #region Commands
        public Command<int> SignInCommand { get; }

        private async void OnSignIn(int comId)
        {
            
            InServerCall = true;

            bool success = await proxy.SignInToCommunityAsync(comId);
            InServerCall = false;

            if (success)
            {
                ((App)Application.Current).CurCom = SelectedCommunity.Community;
                ((App)Application.Current).CurMem = SelectedCommunity.Member;

                
                //Navigate to the main page
                Shell v = serviceProvider.GetService<Shell>();
                ((App)Application.Current).MainPage = v;

                Shell.Current.FlyoutIsPresented = false; //close the flyout

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
        #endregion
    }
}
