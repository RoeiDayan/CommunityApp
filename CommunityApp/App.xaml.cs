using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;

namespace CommunityApp
{
    public partial class App : Application
    {
        public Account? LoggedInUser { get; set; }
        private CommunityWebAPIProxy proxy;
        public App(IServiceProvider serviceProvider, CommunityWebAPIProxy proxy)
        {
            this.proxy = proxy;
            InitializeComponent();
            LoggedInUser = null;
            //Start with the Login View
            MainPage = new NavigationPage(serviceProvider.GetService<LoginView>());
        }
    }
}
