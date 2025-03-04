using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityApp.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;

        public HomePageViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            CurrentCom = ((App)Application.Current).CurCom;
            Reports = new ObservableCollection<Report>();
            Notices = new ObservableCollection<Notice>();
            InitDataFromserver();
        }

        private async void InitDataFromServer()
        {
            
        }
        #region Declaring 
        private ObservableCollection<Report> reports;
        public ObservableCollection<Report> Reports
        {
            get => reports;
            set
            {
                reports = value;
                OnPropertyChanged(nameof(Reports));
            }
        }

        private ObservableCollection<Notice> notices;
        public ObservableCollection<Notice> Notices
        {
            get => notices;
            set
            {
                notices = value;
                OnPropertyChanged(nameof(Notices));
            }
        }
        #endregion
        private Community? currentCom;
        public Community? CurrentCom
        {
            get => currentCom;
            set
            {
                currentCom = value;
                OnPropertyChanged("CurrentCom");
            }
        }
    }
}
