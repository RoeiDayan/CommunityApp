using CommunityApp.Models;
using CommunityApp.Views;
using CommunityApp.ViewModels;
using System;
using Microsoft.Maui.Controls;

namespace CommunityApp.ViewModels
{
    public class AppShellViewModel : ViewModelBase
    {
        private Account? currentUser;
        private IServiceProvider serviceProvider;
        private bool _isManaging;
        public bool IsManaging
        {
            get => _isManaging;
            private set
            {
                _isManaging = value;
                OnPropertyChanged(nameof(IsManaging));
            }
        }

        private bool _isProviding;
        public bool IsProviding
        {
            get => _isProviding;
            private set
            {
                _isProviding = value;
                OnPropertyChanged(nameof(IsProviding));
            }
        }

        private bool _isManagingOrProviding;
        public bool IsManagingOrProviding
        {
            get => _isManagingOrProviding;
            private set
            {
                _isManagingOrProviding = value;
                OnPropertyChanged(nameof(IsManagingOrProviding));
            }
        }

        
        

        public AppShellViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.currentUser = ((App)Application.Current).LoggedInUser;
            CheckUserPermissions();
        }

        private void CheckUserPermissions()
        {
            IsManaging = ((App)Application.Current).CurMem?.IsManager == true;
            IsProviding = ((App)Application.Current).CurMem?.IsProvider == true;
            IsManagingOrProviding = IsManaging || IsProviding;
        }

        public Command LogoutCommand
        {
            get
            {
                return new Command(OnLogout);
            }
        }
        public void OnLogout()
        {
            ((App)Application.Current).LoggedInUser = null;
            ((App)Application.Current).MainPage = new NavigationPage(serviceProvider.GetService<LoginView>());
        }
    }
}