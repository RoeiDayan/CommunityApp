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

    #endregion

    #region Methods
    private async Task FetchRoomRequests()
        {
            try
            {
                IsRefreshing = true;

                int currentCommunityId = ((App)Application.Current).CurCom.ComId;

                // Get all room requests (both approved and pending)
                List<RoomRequest> approvedRequests = await proxy.GetSelectRoomRequestsAsync(currentCommunityId, true) ?? new List<RoomRequest>();
                List<RoomRequest> pendingRequests = await proxy.GetSelectRoomRequestsAsync(currentCommunityId, false) ?? new List<RoomRequest>();

                // Combine the lists
                List<RoomRequest> allRequests = new List<RoomRequest>();
                allRequests.AddRange(approvedRequests);
                allRequests.AddRange(pendingRequests);

                // Clear current lists
                allRoomRequests.Clear();

                // Load member details for each request
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

                // Apply current filter
                ApplyFilter(FilterMode);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Failed to retrieve room requests. Please try again.", "OK");
                System.Diagnostics.Debug.WriteLine($"Error fetching room requests: {ex.Message}");
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
    "Are you sure you want to reject this room request?",
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
        #endregion
    }

    // Helper class to bind room requests with member accounts
    public class RoomRequestWithMember
    {
        public RoomRequest Request { get; set; }
        public MemberAccount MemberAccount { get; set; }
    }
}