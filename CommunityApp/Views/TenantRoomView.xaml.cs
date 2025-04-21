using CommunityApp.ViewModels;

namespace CommunityApp.Views
{
    public partial class TenantRoomView : ContentPage
    {
        private TenantRoomViewModel viewModel;

        public TenantRoomView(TenantRoomViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }
    }
}