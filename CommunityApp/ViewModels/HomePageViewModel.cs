﻿using CommunityApp.Models;
using CommunityApp.Services;
using System;
using CommunityToolkit.Maui;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

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
            CurrentComName = CurrentCom.ComName;
            CurrentComPhone = CurrentCom.GatePhoneNum;
            CopyPhoneCommand = new Command(CopyPhoneToClipboard);
            Reports = new ObservableCollection<Report>();
            Notices = new ObservableCollection<Notice>();
            // Command to fetch and refresh both notices and reports
            FetchBothCommand = new Command(async () => await FetchBothData());

            // Automatically refresh when the ViewModel is created
            _ = FetchBothData();
        }

        #region Properties

        private Community? currentCom;
        public Community? CurrentCom
        {
            get => currentCom;
            set
            {
                currentCom = value;
                OnPropertyChanged(nameof(CurrentCom));
            }
        }

        private string currentComName;
        public string CurrentComName
        {
            get => currentComName;
            set
            {
                currentComName = value;
                OnPropertyChanged(nameof(CurrentComName));
            }
        }

        private string currentComPhone;
        public string CurrentComPhone
        {
            get => currentComPhone;
            set
            {
                currentComPhone = value;
                OnPropertyChanged(nameof(CurrentComPhone));
            }
        }

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

        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        #endregion

        #region Commands
        public ICommand FetchBothCommand { get; }
        public ICommand CopyPhoneCommand { get; }
        #endregion

        #region Methods

        // Refresh method that fetches both notices and reports
        private async Task FetchBothData()
        {
            if (CurrentCom == null)
                return;

            try
            {
                ErrorMessage = ""; // Clear any previous errors

                // Run both methods concurrently (parallel requests)
                var noticesTask = GetNoticesFromServer();
                var reportsTask = GetReportsFromServer();

                // Wait for both tasks to complete
                await Task.WhenAll(noticesTask, reportsTask);
            }
            catch (Exception)
            {
                ErrorMessage = "Failed to load notices and reports. Please check your connection.";
            }
        }

        // Fetch notices from the server
        private async Task GetNoticesFromServer()
        {
            if (CurrentCom != null)
            {
                List<Notice> noticesFromServer = await proxy.GetCommunityNoticesAsync(CurrentCom.ComId);
                Notices.Clear();
                foreach (var notice in noticesFromServer)
                {
                    Notices.Add(notice);
                }
            }
        }

        // Fetch reports from the server
        private async Task GetReportsFromServer()
        {
            if (CurrentCom != null)
            {
                List<Report> reportsFromServer = await proxy.GetCommunityReportsAsync(CurrentCom.ComId);
                Reports.Clear();
                foreach (var report in reportsFromServer)
                {
                    Reports.Add(report);
                }
            }
        }

        private async void CopyPhoneToClipboard()
        {
            if (!string.IsNullOrWhiteSpace(CurrentComPhone))
            {
                await Clipboard.SetTextAsync(CurrentComPhone);
                // Show a toast notification
                var toast = Toast.Make("Phone number copied!", ToastDuration.Short, 14);
                await toast.Show();
            }
        }
        #endregion
    }
}
