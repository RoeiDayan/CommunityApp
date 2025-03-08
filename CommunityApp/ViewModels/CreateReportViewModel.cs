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
    public class CreateReportViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        public CreateReportViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            CreateReportCommand = new Command(CreateReport);
            InServerCall = false;
        }
        #region Properties
        private string? title;
        public string? Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string? reportDesc;
        public string? ReportDesc
        {
            get => reportDesc;
            set
            {
                title = value;
                OnPropertyChanged(nameof(ReportDesc));
            }
        }

        private bool? isAnon;
        public bool? IsAnon
        {
            get => isAnon;
            set
            {
                isAnon = value;
                OnPropertyChanged(nameof(IsAnon));
            }
        }
        #endregion

        #region Commands
        public Command CreateReportCommand { get; }

        public async void CreateReport()
        {
            InServerCall = true;
            string? RepTitle = this.Title;
            string? RepDesc = this.ReportDesc;
            bool? isAnon = this.IsAnon;
            int userId = ((App)Application.Current).LoggedInUser.Id;
            int comId = ((App)Application.Current).CurCom.ComId;

            bool answer = false;

        }
        #endregion
    }
}
