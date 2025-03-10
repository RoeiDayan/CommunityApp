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
        private string title;
        public string Title
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
                reportDesc = value;
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

        #endregion

        #region Methods
        public async void CreateReport()
        {
            InServerCall = true;
            int userId = ((App)Application.Current).LoggedInUser.Id;
            int comId = ((App)Application.Current).CurCom.ComId;

            Report r = new Report { UserId = userId, ComId = comId, Title = Title, ReportDesc = ReportDesc, IsAnon = IsAnon, Priority = null, Status = null, CreatedAt = null };
            bool answer = false;
            answer = await this.proxy.CreateReportAsync(r);
            InServerCall = false;
            if (answer == false)
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Failed to create a report", "ok");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Success!", "Report Created", "ok");

                HomePageView v = serviceProvider.GetService<HomePageView>();
                ((App)Application.Current).MainPage = v;
            }

        }
        #endregion
    }
}
