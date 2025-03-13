using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        public LoginViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            LoginCommand = new Command(OnLogin);
            RegisterCommand = new Command(OnRegister);
            email = "";
            password = "";
            InServerCall = false;
            errorMsg = "";
            ((App)Application.Current).LoggedInUser = null;
            ((App)Application.Current).CurCom = null;
            ((App)Application.Current).CurMem = null;
        }

        private string email;
        private string password;

        public string Email
        {
            get => email;
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private string errorMsg;
        public string ErrorMsg
        {
            get => errorMsg;
            set
            {
                if (errorMsg != value)
                {
                    errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }


        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }



        private async void OnLogin()
        {
            //Choose the way you want to block the page while indicating a server call
            InServerCall = true;
            ErrorMsg = "";
            //Call the server to login
            LoginInfo loginInfo = new LoginInfo { Email = Email, Password = Password };
            Account? acc = await this.proxy.LoginAsync(loginInfo);

            InServerCall = false;

            //Set the application logged in user to be whatever user returned (null or real user)
            ((App)Application.Current).LoggedInUser = acc;
            if (acc == null)
            {
                ErrorMsg = "Invalid email or password";
            }
            else
            {
                ErrorMsg = "";
                
                SelectCommunityView v = serviceProvider.GetService<SelectCommunityView>();
                ((App)Application.Current).MainPage = v;

                //Shell.Current.FlyoutIsPresented = false; //close the flyout
            }
        }

        private void OnRegister()
        {
            ErrorMsg = "";
            Email = "";
            Password = "";
            // Navigate to the Register View page
            ((App)Application.Current).MainPage.Navigation.PushAsync(serviceProvider.GetService<RegisterView>());
        }


    }
}
