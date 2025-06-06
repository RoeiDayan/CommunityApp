using CommunityApp.Models;
using CommunityApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace CommunityApp.ViewModels
{
    public class ManageNoticesViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;

        public ManageNoticesViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;

            // Initialize commands
            FetchNoticesCommand = new AsyncRelayCommand(FetchNotices);
            DeleteNoticeCommand = new AsyncRelayCommand<Notice>(DeleteNotice);

            // Initial data load
            _ = FetchNotices();
        }

        #region Properties
        private ObservableCollection<Notice> notices = new ObservableCollection<Notice>();

        public ObservableCollection<Notice> Notices
        {
            get => notices;
            set
            {
                notices = value ?? new ObservableCollection<Notice>();
                OnPropertyChanged(nameof(Notices));
            }
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        #endregion

        #region Commands
        public IAsyncRelayCommand FetchNoticesCommand { get; }
        public IAsyncRelayCommand<Notice> DeleteNoticeCommand { get; }
        #endregion

        #region Methods

        private async Task FetchNotices()
        {
            if (IsRefreshing) return;

            try
            {
                IsRefreshing = true;

                notices.Clear();

                int currentCommunityId = ((App)Application.Current).CurCom.ComId;
                List<Notice> noticesList = await proxy.GetCommunityNoticesAsync(currentCommunityId) ?? new List<Notice>();

                foreach (var notice in noticesList)
                {
                    notices.Add(notice);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to load notices. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error fetching notices: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }


        private async Task DeleteNotice(Notice notice)
        {
            if (notice == null)
                return;
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",$"Are you sure you want to delete the notice '{notice.Title}'?","Yes", "No");

            if (!confirm)
                return;

            try
            {
                // Delete the notice from the server
                bool success = await proxy.DeleteNoticeAsync(notice.NoticeId);

                if (success)
                {
                    // Remove from local collection
                    var noticeToRemove = notices.FirstOrDefault(n => n.NoticeId == notice.NoticeId);
                    if (noticeToRemove != null)
                    {
                        notices.Remove(noticeToRemove);
                    }

                    // Show success toast
                    var toast = Toast.Make("Notice deleted successfully");
                    await toast.Show();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error","Failed to delete notice. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete notice. Please try again.", "OK");
            }
        }
        #endregion
    }
}