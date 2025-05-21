using CommunityApp.ViewModels;

namespace CommunityApp.Views
{
    public partial class ManageRoomRequestsView : ContentPage
    {
        private readonly ManageRoomRequestsViewModel viewModel;

        public ManageRoomRequestsView(ManageRoomRequestsViewModel vm)
        {

            BindingContext = vm;
            InitializeComponent();


        }
    }
}