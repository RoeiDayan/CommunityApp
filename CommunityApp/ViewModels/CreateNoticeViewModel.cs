using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityApp.ViewModels
{
    public class CreateNoticeViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;

        public CreateNoticeViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            CreateNoticeCommand = new Command(CreateNotice);
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

        private string? noticeDesc;
        public string? NoticeDesc
        {
            get => noticeDesc;
            set
            {
                noticeDesc = value;
                OnPropertyChanged(nameof(NoticeDesc));
            }
        }
        private DateTime? startDate;
        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                OnPropertyChanged(nameof(StartDate));
                // עדכון StartDateTime מיד כש-StartDate משתנה
                UpdateStartDateTime();
            }
        }

        private TimeSpan? startTime;
        public TimeSpan? StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                OnPropertyChanged(nameof(StartTime));
                // עדכון StartDateTime מיד כש-StartTime משתנה
                UpdateStartDateTime();
            }
        }


        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                OnPropertyChanged(nameof(EndDate));
                // עדכון EndDateTime מיד כש-EndDate משתנה
                UpdateEndDateTime();
            }
        }

        private TimeSpan? endTime;
        public TimeSpan? EndTime
        {
            get => endTime;
            set
            {
                endTime = value;
                OnPropertyChanged(nameof(EndTime));
                // עדכון EndDateTime מיד כש-EndTime משתנה
                UpdateEndDateTime();
            }
        }


        private DateTime? startDateTime;
        public DateTime? StartDateTime
        {
            get => startDateTime;
            private set
            {
                startDateTime = value;
                OnPropertyChanged(nameof(StartDateTime));
            }
        }
        private DateTime? endDateTime;
        public DateTime? EndDateTime
        {
            get => endDateTime;
            private set
            {
                endDateTime = value;
                OnPropertyChanged(nameof(EndDateTime));
            }
        }

        #endregion

        #region Commands
        public Command CreateNoticeCommand { get; }
        #endregion

        #region Methods
        public async void CreateNotice()
        {
            InServerCall = true;

            // בדיקה האם יש ערכים ל-StartDateTime ו-EndDateTime לפני האימות
            if (StartDate.HasValue && StartTime.HasValue && EndDate.HasValue && EndTime.HasValue)
            {
                // אימות זמנים רק אם כל חלקי הזמן והתאריך הוזנו
                if (!ValidateTimes())
                {
                    InServerCall = false;
                    await Application.Current.MainPage.DisplayAlert("Invalid Time", "Start time must be before end time.", "OK");
                    return;
                }
            }
            else if ((StartDate.HasValue && StartTime.HasValue) != (EndDate.HasValue && EndTime.HasValue))
            {
                // אם רק אחד מזמני ההתחלה/סיום הוזן באופן מלא - הצג שגיאה
                InServerCall = false;
                await Application.Current.MainPage.DisplayAlert("Invalid Time", "Please provide both start and end date/time or leave both empty.", "OK");
                return;
            }

            if (((App)Application.Current).CurMem.IsManager != null && ((App)Application.Current).CurMem.IsProvider != null)
            {
                if (((App)Application.Current).CurMem.IsManager == false && ((App)Application.Current).CurMem.IsProvider == false)
                {
                    InServerCall = false;
                    await Application.Current.MainPage.DisplayAlert("No permission", "You don't have the required role to perform this action", "OK");
                    return;
                }
            }

            int userId = ((App)Application.Current).LoggedInUser.Id;
            int comId = ((App)Application.Current).CurCom.ComId;

            Notice notice = new Notice
            {
                UserId = userId,
                ComId = comId,
                Title = Title,
                NoticeDesc = NoticeDesc,
                StartTime = StartDateTime,
                EndTime = EndDateTime,
                CreatedAt = DateTime.Now
            };

            bool answer = await this.proxy.CreateNoticeAsync(notice);
            InServerCall = false;

            if (!answer)
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Failed to create a notice", "ok");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Success!", "Notice Created", "ok");

                AppShell v = serviceProvider.GetService<AppShell>();
                ((App)Application.Current).MainPage = v;
            }
        }

        private bool ValidateTimes()
        {
            // מכיוון שאנו בודקים כבר ב-CreateNotice אם יש ערכים,
            // כאן אנו מניחים שאם הגענו לכאן, לשניהם יש ערכים.
            if (StartDateTime.HasValue && EndDateTime.HasValue)
            {
                if (StartDateTime.Value.Date == EndDateTime.Value.Date)
                {
                    return StartDateTime.Value.TimeOfDay < EndDateTime.Value.TimeOfDay;
                }
                else
                {
                    return StartDateTime.Value < EndDateTime.Value;
                }
            }
            // אם הגענו לכאן ואין ערכים (למרות הבדיקה ב-CreateNotice) - נחזיר true כי הזמנים אופציונליים
            return true;
        }

        public void UpdateStartDateTime()
        {
            if (StartDate.HasValue && StartTime.HasValue)
            {
                StartDateTime = StartDate.Value.Date + StartTime.Value;
            }
            else
            {
                StartDateTime = null; // איפוס אם אחד מהערכים חסר
            }
        }

        public void UpdateEndDateTime()
        {
            if (EndDate.HasValue && EndTime.HasValue)
            {
                EndDateTime = EndDate.Value.Date + EndTime.Value;
            }
            else
            {
                EndDateTime = null; // איפוס אם אחד מהערכים חסר
            }
        }

        #endregion
    }
}