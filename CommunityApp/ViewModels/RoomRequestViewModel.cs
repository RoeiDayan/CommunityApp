using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityApp.Models;
using CommunityApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace CommunityApp.ViewModels
{
        public class RoomRequestViewModel : ViewModelBase
        {
            private CommunityWebAPIProxy proxy;
            private IServiceProvider serviceProvider;

        public RoomRequestViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.proxy = proxy;
            this.serviceProvider = serviceProvider;

            StartTime = DateTime.Now.TimeOfDay;
            EndTime = DateTime.Now.AddHours(1).TimeOfDay;
            SubmitRequestCommand = new Command(SubmitRequest);
            InServerCall = false;
        }


        #region Properties

        // Date part
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime EndDate { get; set; } = DateTime.Now.Date;

        // Time part
        public TimeSpan StartTime { get; set; } = DateTime.Now.TimeOfDay;
        public TimeSpan EndTime { get; set; } = DateTime.Now.TimeOfDay;

        // Combined timestamp to send
        public DateTime StartDateTime => StartDate.Date + StartTime;
        public DateTime EndDateTime => EndDate.Date + EndTime;

        // Optional description
        public string? RequestText { get; set; }

        // Minimum selectable date
        public DateTime MinDate => DateTime.Now.Date;

        #endregion

            public Command SubmitRequestCommand { get; }

            public async void SubmitRequest()
            {
                if (!IsTimeRangeValid())
                {
                    await Application.Current.MainPage.DisplayAlert("Invalid Time", "End time must be after start time.", "OK");
                    return;
                }
                InServerCall = true;

                int userId = ((App)Application.Current).LoggedInUser.Id;
                int comId = ((App)Application.Current).CurCom.ComId;

            RoomRequest req = new RoomRequest
            {
                UserId = ((App)Application.Current).LoggedInUser.Id,
                ComId = ((App)Application.Current).CurCom.ComId,
                StartTime = StartDateTime,
                EndTime = EndDateTime,
                Text = RequestText,
                IsApproved = null,
                CreatedAt = DateTime.Now
            };


            bool result = await proxy.CreateRoomRequestAsync(req);

                InServerCall = false;

                if (!result)
                {
                    await Application.Current.MainPage.DisplayAlert("Failed", "Failed to submit room request", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Success!", "Room request submitted", "OK");
                    AppShell shell = serviceProvider.GetService<AppShell>();
                    ((App)Application.Current).MainPage = shell;
                }
            }
        private bool IsTimeRangeValid()
        {
            return EndDateTime > StartDateTime;
        }

    }
}
