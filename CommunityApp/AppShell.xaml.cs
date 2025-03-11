using CommunityApp.ViewModels;
using CommunityApp.Views;

namespace CommunityApp
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel vm)
        {
            this.BindingContext = vm;
            InitializeComponent();
            RegisterRoutes();
        }
        private void RegisterRoutes()
        {
            Routing.RegisterRoute("JoinCommunity", typeof(JoinCommunityView));
        }

    }
}
