using CommunityApp.Models;
using CommunityApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CommunityApp.ViewModels
{
    public class TenantRoomViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;

        public TenantRoomViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.proxy = proxy;
            this._approvedRequests = new List<RoomRequest>();
            this._requestsWithUsers = new ObservableCollection<RequestWithMemberAccount>();
            RefreshCommand = new Command(async () => await LoadData());
            InServerCall = false;
            RoomStatus = "Loading...";
            CurrentSessionInfo = "Loading session info...";
            this.serviceProvider = serviceProvider;
        }

        private List<RoomRequest> _approvedRequests;
        private string _errorMessage;
        private bool _hasErrorMessage;
        private string _roomStatus;
        private bool _isRoomAvailable;
        private string _currentSessionInfo;
        private bool _hasKeyHolder; 
        private string _keyHolderName;
        private string _keyHolderContact;
        public class RequestWithMemberAccount
        {
            public RoomRequest Request { get; set; }
            public MemberAccount MemberAccount { get; set; }
            public string DisplayName => MemberAccount?.Account?.Name ?? "Unknown";
            public string ContactInfo => MemberAccount?.Account?.PhoneNumber ?? "No contact info";
            public string DateRange => $"{Request?.StartTime:dd/MM/yyyy HH:mm} - {Request?.EndTime:dd/MM/yyyy HH:mm}";
        }

        #region Properties

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

        

        public bool HasKeyHolder  
        {
            get => _hasKeyHolder;
            set
            {
                _hasKeyHolder = value;
                OnPropertyChanged(nameof(HasKeyHolder));
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
        public List<RoomRequest> ApprovedRequests
        {
            get => _approvedRequests;
            set
            {
                _approvedRequests = value;
                OnPropertyChanged(nameof(ApprovedRequests));
            }
        }

        private ObservableCollection<RequestWithMemberAccount> _requestsWithUsers;
        public ObservableCollection<RequestWithMemberAccount> RequestsWithUsers
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

                List<RoomRequest> allApprovedRequests = await proxy.GetSelectRoomRequestsAsync(comId, true) ?? new List<RoomRequest>();
                ApprovedRequests = allApprovedRequests;

                // Await the async method
                await CalculateCurrentRoomStatus(allApprovedRequests);

                RequestsWithUsers.Clear();
                DateTime now = DateTime.Now;

                List<RoomRequest> upcomingRequests = allApprovedRequests
                    .Where(r => r.StartTime > now)
                    .OrderBy(r => r.StartTime)
                    .ToList();

                foreach (var request in upcomingRequests)
                {
                    try
                    {
                        var memberAccount = await proxy.GetMemberAccountAsync(request.UserId, comId);
                        RequestsWithUsers.Add(new RequestWithMemberAccount
                        {
                            Request = request,
                            MemberAccount = memberAccount
                        });
                    }
                    catch (Exception)
                    {
                        continue;
                    }
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

        private async Task CalculateCurrentRoomStatus(List<RoomRequest> approvedRequests)
        {
            DateTime now = DateTime.Now;

            RoomRequest activeRequest = approvedRequests
                .FirstOrDefault(r => r.StartTime <= now && r.EndTime >= now);

            if (activeRequest != null)
            {
                RoomStatus = "Occupied";
                IsRoomAvailable = false;
                CurrentSessionInfo = $"{activeRequest.StartTime:dd/MM/yyyy HH:mm} - {activeRequest.EndTime:dd/MM/yyyy HH:mm}";
                HasKeyHolder = true;

                try
                {
                    int comId = ((App)Application.Current).CurCom?.ComId ?? 0;
                    var memberAccount = await proxy.GetMemberAccountAsync(activeRequest.UserId, comId);
                    KeyHolderName = memberAccount?.Account?.Name ?? "Unknown";
                    KeyHolderContact = memberAccount?.Account?.PhoneNumber ?? "No contact info";
                }
                catch (Exception)
                {
                    KeyHolderName = "Unknown";
                    KeyHolderContact = "No contact info";
                }
            }
            else
            {
                RoomStatus = "Available";
                IsRoomAvailable = true;
                CurrentSessionInfo = "No active session";
                HasKeyHolder = false;
                KeyHolderName = null;
                KeyHolderContact = null;
            }
        }

        public string GetRoomStatusForDate(DateTime date)
        {
            var requestOnDate = ApprovedRequests
                .FirstOrDefault(r => date.Date >= r.StartTime.Date && date.Date <= r.EndTime.Date);

            if (requestOnDate != null)
            {
                DateTime now = DateTime.Now;
                if (requestOnDate.StartTime <= now && requestOnDate.EndTime >= now)
                {
                    return "Occupied";
                }
                else
                {
                    return "Requested";
                }
            }

            return "Available";
        }

        #endregion
    }
}
