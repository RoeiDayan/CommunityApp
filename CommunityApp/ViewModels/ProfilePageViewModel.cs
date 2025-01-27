using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityApp.ViewModels
{
    public class ProfilePageViewModel:ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        public ProfilePageViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            CurMember = ((App)Application.Current).CurMem;
        }
        private Member? curMember;
        public Member? CurMember
        {
            get => curMember;
            set
            {
                curMember = value;
                OnPropertyChanged("CurMember");
             
            }
        }
    }
}
