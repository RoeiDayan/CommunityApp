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
    public class ManageRoomRequestsViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;
        public ManageRoomRequestsViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;

            // Initialize commands
            FetchRoomRequestsCommand = new AsyncRelayCommand(FetchRoomRequests);
            ApproveRequestCommand = new AsyncRelayCommand<RoomRequest>(ApproveRequest);
            RejectRequestCommand = new AsyncRelayCommand<RoomRequest>(RejectRequest);
            CopyToClipboardCommand = new AsyncRelayCommand<string>(CopyTextToClipboard);
            ToggleFilterCommand = new Command<string>(FilterRequests);
            DeletePastRequestsCommand = new AsyncRelayCommand(DeletePastRequests);
            // Initial data load
            _ = FetchRoomRequests();
        }

        #region Properties
        private ObservableCollection<RoomRequestWithMember> allRoomRequests = new ObservableCollection<RoomRequestWithMember>();
        private ObservableCollection<RoomRequestWithMember> filteredRoomRequests = new ObservableCollection<RoomRequestWithMember>();

        public ObservableCollection<RoomRequestWithMember> RoomRequests
        {
            get => filteredRoomRequests;
            set
            {
                filteredRoomRequests = value ?? new ObservableCollection<RoomRequestWithMember>();
                OnPropertyChanged(nameof(RoomRequests));
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

        private string filterMode = "all";
        public string FilterMode
        {
            get => filterMode;
            set
            {
                filterMode = value;
                OnPropertyChanged(nameof(FilterMode));
                OnPropertyChanged(nameof(IsAllFilterSelected));
                OnPropertyChanged(nameof(IsApprovedFilterSelected));
                OnPropertyChanged(nameof(IsPendingFilterSelected));
            }
        }

        public bool IsAllFilterSelected => FilterMode == "all";
        public bool IsApprovedFilterSelected => FilterMode == "approved";
        public bool IsPendingFilterSelected => FilterMode == "pending";
        #endregion

        #region Commands

    public IAsyncRelayCommand FetchRoomRequestsCommand { get; }
    public IAsyncRelayCommand<RoomRequest> ApproveRequestCommand { get; }
    public IAsyncRelayCommand<RoomRequest> RejectRequestCommand { get; }
    public IAsyncRelayCommand<string> CopyToClipboardCommand { get; }
    public ICommand ToggleFilterCommand { get; }

        public IAsyncRelayCommand DeletePastRequestsCommand { get; }
        #endregion

        #region Methods
        private bool _hasLoaded = false;

        private async Task FetchRoomRequests()
        {
            if (IsRefreshing) return; // Already refreshing

            try
            {
                IsRefreshing = true;

                // If this is a refresh (not initial load), clear data first
                if (_hasLoaded)
                {
                    allRoomRequests.Clear();
                }

                int currentCommunityId = ((App)Application.Current).CurCom.ComId;
                List<RoomRequest> allRequests = await proxy.GetAllRoomRequestsAsync(currentCommunityId) ?? new List<RoomRequest>();

                // Don't clear if this is a refresh since we already did above
                if (!_hasLoaded)
                {
                    allRoomRequests.Clear();
                }

                // Same logic as before
                foreach (var request in allRequests)
                {
                    var memberAccount = await proxy.GetMemberAccountAsync(request.UserId, currentCommunityId);
                    if (memberAccount != null)
                    {
                        allRoomRequests.Add(new RoomRequestWithMember
                        {
                            Request = request,
                            MemberAccount = memberAccount
                        });
                    }
                }

                // Mark as loaded
                _hasLoaded = true;

                // Apply current filter
                ApplyFilter(FilterMode);
            }
            catch (Exception ex)
            {
                // Error handling
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private void FilterRequests(string filterMode)
        {
            FilterMode = filterMode;
            ApplyFilter(filterMode);
        }

        private void ApplyFilter(string filterMode)
        {
            switch (filterMode)
            {
                case "approved":
                    RoomRequests = new ObservableCollection<RoomRequestWithMember>(
                        allRoomRequests.Where(r => r.Request.IsApproved.GetValueOrDefault()));
                    break;
                case "pending":
                    RoomRequests = new ObservableCollection<RoomRequestWithMember>(
                        allRoomRequests.Where(r => !r.Request.IsApproved.GetValueOrDefault()));
                    break;
                case "all":
                default:
                    RoomRequests = new ObservableCollection<RoomRequestWithMember>(allRoomRequests);
                    break;
            }
        }

        private async Task ApproveRequest(RoomRequest request)
        {
            if (request == null)
                return;

            try
            {
                // Update the approval status
                request.IsApproved = true;

                // Send update to server
                bool success = await proxy.UpdateRoomRequestAsync(request);

                if (success)
                {
                    // Update the local collection
                    var requestWithMember = allRoomRequests.FirstOrDefault(r => r.Request.RequestId == request.RequestId);
                    if (requestWithMember != null)
                    {
                        requestWithMember.Request.IsApproved = true;
                    }

                    // Reapply the filter
                    ApplyFilter(FilterMode);

                    // Show success toast
                    var toast = Toast.Make("Room request approved successfully");
                    await toast.Show();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to approve request. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "An error occurred while processing your request.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error approving request: {ex.Message}");
            }
        }

        private async Task RejectRequest(RoomRequest request)
        {
            if (request == null)
                return;

            bool confirm = (bool)await Application.Current.MainPage.DisplayAlert(
                "Confirm Rejection",
                "Are you sure you want to reject this room request? Rejecting an unapproved requests deletes it",
                "Yes", "No");



            if (!confirm)
                return;

            try
            {
                // For pending requests, we'll delete them
                // For approved requests, we'll update the status
                bool success;

                if (!request.IsApproved.GetValueOrDefault())
                {
                    // Delete the request from the server
                    success = await proxy.DeleteRoomRequestAsync(request.RequestId);
                }
                else
                {
                    // Update the status
                    request.IsApproved = false;
                    success = await proxy.UpdateRoomRequestAsync(request);
                }

                if (success)
                {
                    if (!request.IsApproved.GetValueOrDefault())
                    {
                        // Remove from local collection
                        var requestToRemove = allRoomRequests.FirstOrDefault(r => r.Request.RequestId == request.RequestId);
                        if (requestToRemove != null)
                        {
                            allRoomRequests.Remove(requestToRemove);
                        }
                    }
                    else
                    {
                        // Update the local collection
                        var requestWithMember = allRoomRequests.FirstOrDefault(r => r.Request.RequestId == request.RequestId);
                        if (requestWithMember != null)
                        {
                            requestWithMember.Request.IsApproved = false;
                        }
                    }

                    // Reapply the filter
                    ApplyFilter(FilterMode);

                    // Show success toast
                    var toast = Toast.Make("Room request rejected");
                    await toast.Show();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to reject request. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to reject request. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error rejecting request: {ex.Message}");
            }
        }

        private async Task CopyTextToClipboard(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    await Clipboard.SetTextAsync(text);
                    string message = text.Contains('@') ?
                        "Email copied to clipboard" :
                        (text.All(char.IsDigit) ? "Phone number copied to clipboard" : "Text copied to clipboard");

                    var toast = Toast.Make(message);
                    await toast.Show();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error copying text: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", "Could not copy to clipboard", "OK");
                }
            }
        }

        private async Task DeletePastRequests()
        {
            bool confirm = (bool)await Application.Current.MainPage.DisplayAlert(
                "Confirm Deletion",
                "All outdated room requests will be permanently deleted. Would you like to continue?",
                "Yes", "No");



            if (!confirm)
                return;
            try
            {
                int currentCommunityId = ((App)Application.Current).CurCom.ComId;

                bool success = await proxy.DeletePastRoomRequestsAsync(currentCommunityId);

                if (success)
                {
                    var toast = Toast.Make("Past room requests deleted successfully");
                    await toast.Show();

                    // Refresh the list after deletion
                    await FetchRoomRequests();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "No past requests to delete.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete past requests.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error deleting past requests: {ex.Message}");
            }
        }
        #endregion
    }

    public class RoomRequestWithMember
    {
        public RoomRequest Request { get; set; }
        public MemberAccount MemberAccount { get; set; }
    }
}