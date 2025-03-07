using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityApp.ViewModels
{
    public class AccountPageViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;

        public AccountPageViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            CurrentAcc = ((App)Application.Current).LoggedInUser;
            LogoutCommand = new Command(LogoutOfAccount);

        }
        public ICommand LogoutCommand { get; }
        #region Properties
        private Account currentAcc;
        public Account CurrentAcc
        {
            get => currentAcc;
            set
            {
                currentAcc = value;
                OnPropertyChanged(nameof(CurrentAcc));
            }
        }
        #endregion
        public async void LogoutOfAccount()
        {
            ((App)Application.Current).LoggedInUser = null;
            ((App)Application.Current).CurCom = null;
            ((App)Application.Current).CurMem = null;

            var loginPage = serviceProvider.GetService<LoginView>();
            ((App)Application.Current).MainPage = new NavigationPage(loginPage);
            await Task.CompletedTask;

        }
    }
}
