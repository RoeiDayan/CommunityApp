using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityApp.ViewModels
{
    public class StartCommunityViewModel:ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        private Member mem;
        private Community com;



        public StartCommunityViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.proxy = proxy;
            this.serviceProvider = serviceProvider;
        }



        #region Methods
        public void ComposeCommunity()
        {

        }
        public void ComposeMember()
        {

        }
        public async void CreateCommunity()
        {
            MemberCommunity Submission = new MemberCommunity {Member = this.mem, Community = this.com };
            MemberCommunity result = await this.proxy.CreateCommunityAsync(Submission);
            ((App)Application.Current).CurCom = result.Community;
            ((App)Application.Current).CurMem = result.Member;

            AppShell v = serviceProvider.GetService<AppShell>();
            ((App)Application.Current).MainPage = v;
        }
        #endregion
    }
}
