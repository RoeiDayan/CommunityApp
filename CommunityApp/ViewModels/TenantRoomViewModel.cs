using CommunityApp.Models;
using CommunityApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls; // Make sure this is included for Application.Current

namespace CommunityApp.ViewModels
{
    public class TenantRoomViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private List<RoomRequest> _approvedRequests;
        private string _errorMessage;
        private bool _hasErrorMessage;

        // Properties for UI Binding (Calculated)
        private string _roomStatus;
        private bool _isRoomAvailable;
        private string _currentSessionInfo;
        private bool _hasActiveSession;
        private MemberAccount _currentKeyHolder;
        private string _keyHolderName;
        private string _keyHolderContact;
        private bool _hasKeyHolder;

        // New property to hold the currently active request
        private RoomRequest _currentActiveRequest;

        public TenantRoomViewModel(CommunityWebAPIProxy proxy)
        {
            this.proxy = proxy;
            this._approvedRequests = new List<RoomRequest>();
            this._requestsWithUsers = new ObservableCollection<RequestWithUser>();

            RefreshCommand = new Command(async () => await LoadData());

            InServerCall = false;
            // Initialize with loading states for a better user experience
            RoomStatus = "Loading...";
            CurrentSessionInfo = "Loading session info...";
            KeyHolderName = "Loading...";
            KeyHolderContact = "";
            HasKeyHolder = false; // Initialize HasKeyHolder to false
        }

        // Helper class to combine request and user info for the UI
        public class RequestWithUser
        {
            public RoomRequest Request { get; set; }
            public MemberAccount User { get; set; }
            public string DisplayName => User?.Account?.Name ?? "Unknown";
            public string ContactInfo => User?.Account?.PhoneNumber ?? "No contact info";
            // Updated to display time as well for better precision
            public string DateRange => $"{Request?.StartTime.ToString("dd/MM/yyyy HH:mm")} - {Request?.EndTime.ToString("dd/MM/yyyy HH:mm")}";
        }


        #region Properties for UI Binding (Calculated)

        public string RoomStatus
        {
            get => _roomStatus;
            set
            {
                _roomStatus = value;
                OnPropertyChanged(nameof(RoomStatus));
            }
        }

        public bool IsRoomAvailable
        {
            get => _isRoomAvailable;
            set
            {
                _isRoomAvailable = value;
                OnPropertyChanged(nameof(IsRoomAvailable));
            }
        }

        public string CurrentSessionInfo
        {
            get => _currentSessionInfo;
            set
            {
                _currentSessionInfo = value;
                OnPropertyChanged(nameof(CurrentSessionInfo));
            }
        }

        public bool HasActiveSession
        {
            get => _hasActiveSession;
            set
            {
                _hasActiveSession = value;
                OnPropertyChanged(nameof(HasActiveSession));
            }
        }

        public MemberAccount CurrentKeyHolder
        {
            get => _currentKeyHolder;
            set
            {
                _currentKeyHolder = value;
                OnPropertyChanged(nameof(CurrentKeyHolder));
                // Update dependent properties when CurrentKeyHolder changes
                KeyHolderName = value?.Account?.Name ?? "No key holder";
                KeyHolderContact = value?.Account?.PhoneNumber ?? "";
                HasKeyHolder = value != null;
            }
        }

        public string KeyHolderName
        {
            get => _keyHolderName;
            set
            {
                _keyHolderName = value;
                OnPropertyChanged(nameof(KeyHolderName));
            }
        }

        public string KeyHolderContact
        {
            get => _keyHolderContact;
            set
            {
                _keyHolderContact = value;
                OnPropertyChanged(nameof(KeyHolderContact));
            }
        }

        public bool HasKeyHolder
        {
            get => _hasKeyHolder;
            set
            {
                _hasKeyHolder = value;
                OnPropertyChanged(nameof(HasKeyHolder));
            }
        }

        // List of all approved requests (fetched from API)
        public List<RoomRequest> ApprovedRequests
        {
            get => _approvedRequests;
            set
            {
                _approvedRequests = value;
                OnPropertyChanged(nameof(ApprovedRequests));
            }
        }

        // ObservableCollection for the UI list (CollectionView)
        private ObservableCollection<RequestWithUser> _requestsWithUsers;
        public ObservableCollection<RequestWithUser> RequestsWithUsers
        {
            get => _requestsWithUsers;
            set
            {
                _requestsWithUsers = value;
                OnPropertyChanged(nameof(RequestsWithUsers));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                HasErrorMessage = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasErrorMessage
        {
            get => _hasErrorMessage;
            set
            {
                _hasErrorMessage = value;
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }

        #endregion


        #region Commands

        public Command RefreshCommand { get; }

        #endregion


        #region Methods

        public async Task LoadData()
        {
            try
            {
                InServerCall = true;
                ErrorMessage = string.Empty;

                int comId = ((App)Application.Current).CurCom?.ComId ?? 0;

                if (comId == 0)
                {
                    ErrorMessage = "No community selected";
                    return;
                }

                // 1. Fetch all approved room requests from the API
                List<RoomRequest> allApprovedRequests = await proxy.GetApprovedRoomRequestsAsync(comId) ?? new List<RoomRequest>();
                ApprovedRequests = allApprovedRequests; // Also update the backing field.

                // 2. Calculate the current room status (and active request)
                await CalculateCurrentRoomStatus(comId, allApprovedRequests); // Pass the fetched requests

                // 3. Clear the UI collection
                RequestsWithUsers.Clear();

                DateTime now = DateTime.Now;

                // 4. Filter for *upcoming* requests (StartTime > now)
                List<RoomRequest> upcomingRequests = allApprovedRequests
                    .Where(r => r.StartTime > now)
                    .OrderBy(r => r.StartTime)
                    .ToList();

                // 5. Populate the UI collection with RequestWithUser objects
                foreach (var request in upcomingRequests)
                {
                    var user = await proxy.GetMemberAccountAsync(request.UserId, comId);
                    RequestsWithUsers.Add(new RequestWithUser
                    {
                        Request = request,
                        User = user
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading data: {ex.Message}";
            }
            finally
            {
                InServerCall = false;
            }
        }

        private async Task CalculateCurrentRoomStatus(int comId, List<RoomRequest> approvedRequests)
        {
            DateTime now = DateTime.Now;

            // Normalize all comparisons to local time
            RoomRequest activeRequest = approvedRequests
                .FirstOrDefault(r =>
                    r.StartTime.ToLocalTime() <= now &&
                    r.EndTime.ToLocalTime() >= now
                );

            _currentActiveRequest = activeRequest;

            if (activeRequest != null)
            {
                RoomStatus = "Occupied";
                IsRoomAvailable = false;
                CurrentSessionInfo = $"{activeRequest.StartTime.ToLocalTime():dd/MM/yyyy HH:mm} - {activeRequest.EndTime.ToLocalTime():dd/MM/yyyy HH:mm}";
                HasActiveSession = true;
                CurrentKeyHolder = await proxy.GetMemberAccountAsync(activeRequest.UserId, comId);
            }
            else
            {
                RoomStatus = "Available";
                IsRoomAvailable = true;
                CurrentSessionInfo = "No active session";
                HasActiveSession = false;
                CurrentKeyHolder = null;
                _currentActiveRequest = null;
            }
        }




        // Method used by the DateToAvailabilityColorConverter (or similar logic)
        public string GetRoomStatusForDate(DateTime date)
        {
            // Find if there's any approved request that covers the given date (day-wise)
            var requestOnDate = ApprovedRequests
                .FirstOrDefault(r => date.Date >= r.StartTime.Date && date.Date <= r.EndTime.Date);

            if (requestOnDate != null)
            {
                // If the request for this date is also the currently active session
                if (requestOnDate == _currentActiveRequest)
                {
                    return "Occupied"; // It's the active session right now
                }
                else
                {
                    // If there's an approved request for this date, but it's not the currently active one (i.e., a future request)
                    return "Requested";
                }
            }

            return "Available"; // No approved request found for this date
        }
        #endregion
    }
}