using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;

namespace CommunityApp
{
    public partial class App : Application
    {
        public Community? CurCom { get; set; }
        public Member? CurMem { get; set; }
        public Account? LoggedInUser { get; set; }
        private CommunityWebAPIProxy proxy;
        public App(IServiceProvider serviceProvider, CommunityWebAPIProxy proxy)
        {
            this.proxy = proxy;
            InitializeComponent();
            LoggedInUser = null;
            CurCom = null;
            //Start with the Login View
            MainPage = new NavigationPage(serviceProvider.GetService<LoginView>());
        }
    }
}
