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
    public class ManageReportsViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;

        public ManageReportsViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;

            // Initialize commands
            FetchReportsCommand = new AsyncRelayCommand(FetchReports);
            DeleteReportCommand = new AsyncRelayCommand<Report>(DeleteReport);

            // Initial data load
            _ = FetchReports();
        }

        #region Properties
        private ObservableCollection<Report> reports = new ObservableCollection<Report>();

        public ObservableCollection<Report> Reports
        {
            get => reports;
            set
            {
                reports = value ?? new ObservableCollection<Report>();
                OnPropertyChanged(nameof(Reports));
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
        public IAsyncRelayCommand FetchReportsCommand { get; }
        public IAsyncRelayCommand<Report> DeleteReportCommand { get; }
        #endregion

        #region Methods
        private bool _hasLoaded = false;

        private async Task FetchReports()
        {
            if (IsRefreshing) return; // Already refreshing

            try
            {
                IsRefreshing = true;

                // If this is a refresh (not initial load), clear data first
                if (_hasLoaded)
                {
                    reports.Clear();
                }

                int currentCommunityId = ((App)Application.Current).CurCom.ComId;
                List<Report> reportsList = await proxy.GetCommunityReportsAsync(currentCommunityId) ?? new List<Report>();

                // Don't clear if this is a refresh since we already did above
                if (!_hasLoaded)
                {
                    reports.Clear();
                }

                // Add all reports to the collection
                foreach (var report in reportsList)
                {
                    reports.Add(report);
                }

                // Mark as loaded
                _hasLoaded = true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to load reports. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error fetching reports: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task DeleteReport(Report report)
        {
            if (report == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete the report '{report.Title}'?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                // Delete the report from the server
                bool success = await proxy.DeleteReportAsync(report.ReportId);

                if (success)
                {
                    // Remove from local collection
                    var reportToRemove = reports.FirstOrDefault(r => r.ReportId == report.ReportId);
                    if (reportToRemove != null)
                    {
                        reports.Remove(reportToRemove);
                    }

                    // Show success toast
                    var toast = Toast.Make("Report deleted successfully");
                    await toast.Show();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to delete report. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to delete report. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error deleting report: {ex.Message}");
            }
        }
        #endregion
    }
}