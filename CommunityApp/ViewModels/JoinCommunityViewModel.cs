using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.ViewModels
{
    public class JoinCommunityViewModel:ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        public JoinCommunityViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            JoinCommand = new Command(OnJoinCommunity);
            LoginCommand = new Command(OnLogin);
            OnSelectCommunityCommand = new Command(OnSelectCommunity);
            InServerCall = false;
            ComCodeErrorMsg = "Invalid community code";
            showInvalidCodeMsg = false;
            ShowLogin = (((App)Application.Current).CurMem == null);
        }
        #region Properties
        private string communityCode;
        public string CommunityCode
        {
            get => communityCode;
            set
            {
                if (communityCode != value)
                {
                    communityCode = value;
                    OnPropertyChanged(nameof(CommunityCode));
                }
            }
        }

        private int unitNum;
        public int UnitNum
        {
            get => unitNum;
            set
            {
                if (unitNum != value)
                {
                    unitNum = value; OnPropertyChanged(nameof(UnitNum));
                }
            }
        }

        private bool isLiable;
        public bool IsLiable
        {
            get => isLiable;
            set
            {
                isLiable = value; OnPropertyChanged(nameof(IsLiable));
            }
        }

        private bool isResident;
        public bool IsResident
        {
            get => isResident;
            set
            {
                isResident = value; OnPropertyChanged(nameof (IsResident));
            }
        }

        private bool isManager;
        public bool IsManager
        {
            get => isManager;
            set
            {
                isManager = value; OnPropertyChanged(nameof(IsManager));
            }
        }

        private bool isProvider;
        public bool IsProvider
        {
            get => isProvider;
            set
            {
                isProvider = value; OnPropertyChanged(nameof(IsProvider));
            }
        }
        private string comCodeErrorMsg;
        public string ComCodeErrorMsg
        {
            get => comCodeErrorMsg;
            set
            {
                comCodeErrorMsg = value; OnPropertyChanged(ComCodeErrorMsg);
            }
        }

        
        #region CodeValid
        private bool isCodeValid;
        public bool IsCodeValid
        {
            get => isCodeValid;
            set
            {
                isCodeValid = value;
                OnPropertyChanged(nameof(IsCodeNotValid));
                OnPropertyChanged(nameof(IsCodeValid));
            }
        }
        public bool IsCodeNotValid
        {
            get
            {
                return !IsCodeValid;
            }
        }
        #endregion

        private bool showInvalidCodeMsg;
        public bool ShowInvalidCodeMsg
        {
            get => showInvalidCodeMsg;
            set
            {
                showInvalidCodeMsg = value; OnPropertyChanged(nameof(ShowInvalidCodeMsg));
            }
        }

        private bool showLogin;
        public bool ShowLogin
        {
            get => showLogin;
            set
            {
                showLogin = value; OnPropertyChanged(nameof(ShowLogin));
            }
        }
        #endregion

        #region Commands
        public Command JoinCommand { get;}
        public Command LoginCommand { get;}

        public Command OnSelectCommunityCommand { get;}
        #endregion

        #region Methods
        private async void OnJoinCommunity()
        {
            InServerCall = true;
            int comID = await GetCommunityId();
            if (IsCodeValid)
            {
                Member m = new Member
                {
                    ComId = comID,
                    UserId = ((App)Application.Current).LoggedInUser.Id,
                    UnitNum = this.UnitNum,
                    IsLiable = this.IsLiable,
                    IsResident = this.IsResident,
                    IsManager = this.isManager,
                    IsProvider = this.IsProvider,
                    IsApproved = false
                };
                bool joinSuccess = await proxy.JoinCommunityAsync(m);
                InServerCall = false;
                if (joinSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert("Success!", "Your member submition is pending approval", "ok");
                    SelectCommunityView v = serviceProvider.GetService<SelectCommunityView>();
                    ((App)Application.Current).MainPage = v;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Failed", "Your member submition failed. Remember: You can't join a community your'e already in", "ok");
                }
            }
            else
            {
                InServerCall = false;
                await showInvalid();
            }

        }
        private async Task<int> GetCommunityId()
        {
            int result = await this.proxy.GetCommunityIdAsync(this.CommunityCode);
            if (result >= 0)
            {
                IsCodeValid = true;
            }
            else
            {
                IsCodeValid = false;
            }
            return result;
        }

        private async Task showInvalid()
        {
            ShowInvalidCodeMsg = true;
            await Task.Delay(3500);
            ShowInvalidCodeMsg = false;
        }
        private async void OnLogin()
        {
            var loginPage = serviceProvider.GetService<LoginView>();
            ((App)Application.Current).MainPage = new NavigationPage(loginPage);
            await Task.CompletedTask;

            
        }

        private async void OnSelectCommunity()
        {
            SelectCommunityView v = serviceProvider.GetService<SelectCommunityView>();
            ((App)Application.Current).MainPage = v;
        }
        #endregion
    }
}
