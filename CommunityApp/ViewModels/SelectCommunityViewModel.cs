using CommunityApp.Models;
using CommunityApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityApp.ViewModels
{
    public class SelectCommunityViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;

        public SelectCommunityViewModel(CommunityWebAPIProxy proxy)
        {
            this.proxy = proxy;
            Communities = new ObservableCollection<MemberCommunityDTO>();
            SignInCommand = new Command<int>(OnSignIn);
            LoadCommunities();
        }

        #region Communities
        private ObservableCollection<MemberCommunityDTO> communities;
        public ObservableCollection<MemberCommunityDTO> Communities
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
        private MemberCommunityDTO selectedCommunity;
        public MemberCommunityDTO SelectedCommunity
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
            if (InServerCall) return; // Prevent multiple simultaneous calls
            InServerCall = true;

            bool success = await proxy.SignInToCommunityAsync(comId);
            InServerCall = false;

            if (success)
            {
                // Navigate to the community-specific page
                await Application.Current.MainPage.DisplayAlert("Success", "Signed into community successfully!", "OK");

                // Replace with actual navigation logic
                await Application.Current.MainPage.Navigation.PushAsync(new CommunityHomePage(comId));
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
                int userId = ((App)Application.Current).LoggedInUser.Id; // Replace with actual user ID logic
                var userCommunities = await proxy.GetUserCommunitiesAsync(userId);
                InServerCall = false;

                if (userCommunities != null)
                {
                    Communities = new ObservableCollection<MemberCommunityDTO>(userCommunities);
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
