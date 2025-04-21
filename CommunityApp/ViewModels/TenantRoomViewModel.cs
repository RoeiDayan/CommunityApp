using CommunityApp.Models;
using CommunityApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityApp.ViewModels
{
    public class TenantRoomViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        private TenantRoom tenantRoom;
        private List<RoomRequest> approvedRequests;
        private string errorMessage;
        private bool hasErrorMessage;
        private MemberAccount keyHolder;
        private ObservableCollection<RequestWithUser> requestsWithUsers;

        public TenantRoomViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            this.approvedRequests = new List<RoomRequest>();
            this.requestsWithUsers = new ObservableCollection<RequestWithUser>();

            RefreshCommand = new Command(async () => await LoadData());
            InServerCall = false;

            // Load data immediately when the ViewModel is created
            Task.Run(async () => await LoadData());
        }

        // Class to combine request with user information
        public class RequestWithUser
        {
            public RoomRequest Request { get; set; }
            public MemberAccount User { get; set; }
            public string DisplayName => User?.Account?.Name ?? "Unknown";
            public string ContactInfo => User?.Account?.PhoneNumber ?? "No contact info";
            public string DateRange => $"{Request?.StartTime.ToString("MMM dd")} - {Request?.EndTime.ToString("MMM dd")}";
        }

        #region Properties
        public TenantRoom TenantRoom
        {
            get => tenantRoom;
            set
            {
                tenantRoom = value;
                OnPropertyChanged(nameof(TenantRoom));
                OnPropertyChanged(nameof(RoomStatus));
                OnPropertyChanged(nameof(IsRoomAvailable));
                OnPropertyChanged(nameof(CurrentSessionInfo));
                OnPropertyChanged(nameof(HasActiveSession));
                OnPropertyChanged(nameof(KeyHolderInfo));
            }
        }

        public MemberAccount KeyHolder
        {
            get => keyHolder;
            set
            {
                keyHolder = value;
                OnPropertyChanged(nameof(KeyHolder));
                OnPropertyChanged(nameof(KeyHolderName));
                OnPropertyChanged(nameof(KeyHolderContact));
                OnPropertyChanged(nameof(HasKeyHolder));
            }
        }

        public string KeyHolderName => KeyHolder?.Account?.Name ?? "No key holder";
        public string KeyHolderContact => KeyHolder?.Account?.PhoneNumber ?? "";
        public bool HasKeyHolder => KeyHolder != null;

        public ObservableCollection<RequestWithUser> RequestsWithUsers
        {
            get => requestsWithUsers;
            set
            {
                requestsWithUsers = value;
                OnPropertyChanged(nameof(RequestsWithUsers));
            }
        }

        public List<RoomRequest> ApprovedRequests
        {
            get => approvedRequests;
            set
            {
                approvedRequests = value;
                OnPropertyChanged(nameof(ApprovedRequests));
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                HasErrorMessage = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasErrorMessage
        {
            get => hasErrorMessage;
            set
            {
                hasErrorMessage = value;
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }

        // Computed properties for UI
        public string RoomStatus => TenantRoom?.Status ?? "Unknown";

        public bool IsRoomAvailable =>
            TenantRoom != null &&
            string.Equals(TenantRoom.Status, "Available", StringComparison.OrdinalIgnoreCase);

        public string CurrentSessionInfo
        {
            get
            {
                if (TenantRoom?.SessionStart == null || TenantRoom?.SessionEnd == null)
                    return "No active session";

                return $"{TenantRoom.SessionStart?.ToString("dd/MM/yyyy HH:mm")} - {TenantRoom.SessionEnd?.ToString("dd/MM/yyyy HH:mm")}";
            }
        }

        public bool HasActiveSession => TenantRoom?.SessionStart != null && TenantRoom?.SessionEnd != null;

        public string KeyHolderInfo => TenantRoom?.KeyHolderId > 0 ? $"Key Holder ID: {TenantRoom?.KeyHolderId}" : "No key holder assigned";
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

                // Get current community ID from the app
                int comId = ((App)Application.Current).CurCom?.ComId ?? 0;

                if (comId == 0)
                {
                    ErrorMessage = "No community selected";
                    return;
                }

                var tenantRoomTask = proxy.GetCommunityTenantRoomAsync(comId);
                var requestsTask = proxy.GetApprovedRoomRequestsAsync(comId);

                await Task.WhenAll(tenantRoomTask, requestsTask);

                TenantRoom = await tenantRoomTask;
                ApprovedRequests = await requestsTask ?? new List<RoomRequest>();

                // Load key holder information if available
                if (TenantRoom?.KeyHolderId > 0)
                {
                    KeyHolder = await proxy.GetMemberAccountAsync(TenantRoom.KeyHolderId, comId);
                }
                else
                {
                    KeyHolder = null;
                }

                // Load user information for each approved request
                RequestsWithUsers.Clear();
                foreach (var request in ApprovedRequests)
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

        // Method for the DateToAvailabilityColorConverter
        public string GetRoomStatusForDate(DateTime date)
        {
            // Check if the room is currently occupied for this date
            if (TenantRoom?.SessionStart != null && TenantRoom?.SessionEnd != null)
            {
                if (date.Date >= TenantRoom.SessionStart.Value.Date && date.Date <= TenantRoom.SessionEnd.Value.Date)
                {
                    return "Occupied";
                }
            }

            // Check if the date is within any approved request
            foreach (var request in ApprovedRequests)
            {
                if (date.Date >= request.StartTime.Date && date.Date <= request.EndTime.Date)
                {
                    return "Requested";
                }
            }

            return "Available";
        }
        #endregion
    }
}