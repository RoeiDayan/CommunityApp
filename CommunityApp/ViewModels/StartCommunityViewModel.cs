using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityApp.ViewModels
{
    public class StartCommunityViewModel:ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        private Member mem = new Member();
        private Community com = new Community();
        


        public StartCommunityViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.proxy = proxy;
            this.serviceProvider = serviceProvider;
            this.comId = 0;
            this.userId = ((App)Application.Current).LoggedInUser.Id;
            CreateCommunityCommand = new Command(OnCreateCommunity);
            OnSelectCommunityCommand = new Command(OnSelectCommunity);

            InServerCall = false;
        }
        #region Commands
        public Command CreateCommunityCommand { get;}
        public Command OnSelectCommunityCommand { get; }

        #endregion
        #region UserProperties
        private int comId;

        private int userId;

        private string role;
        public string Role
        {
            get => role;
            set
            {
                if (role != value)
                {
                    role = value;
                    OnPropertyChanged(nameof(Role));
                }
            }
        }

        private int balance;
        public int Balance
        {
            get => balance;
            set
            {
                if (balance != value)
                {
                    balance = value;
                    OnPropertyChanged(nameof(Balance));
                }
            }
        }

        private int unitNum;
        public int UnitNum
        {
            get => unitNum;
            set
            {
                if (unitNum != value)
                {
                    unitNum = value;
                    OnPropertyChanged(nameof(UnitNum));
                }
            }
        }

        private bool isLiable;
        public bool IsLiable
        {
            get => isLiable;
            set
            {
                if (isLiable != value)
                {
                    isLiable = value;
                    OnPropertyChanged(nameof(IsLiable));
                }
            }
        }

        private bool isResident;
        public bool IsResident
        {
            get => isResident;
            set
            {
                if (isResident != value)
                {
                    isResident = value;
                    OnPropertyChanged(nameof(IsResident));
                }
            }
        }

        private bool isManager;
        public bool IsManager
        {
            get => isManager;
            set
            {
                if (isManager != value)
                {
                    isManager = value;
                    OnPropertyChanged(nameof(IsManager));
                }
            }
        }

        private bool isProvider;
        public bool IsProvider
        {
            get => isProvider;
            set
            {
                if (isProvider != value)
                {
                    isProvider = value;
                    OnPropertyChanged(nameof(IsProvider));
                }
            }
        }

        private bool isApproved;
        public bool IsApproved
        {
            get => isApproved;
            set
            {
                if (isApproved != value)
                {
                    isApproved = value;
                    OnPropertyChanged(nameof(IsApproved));
                }
            }
        }

        #endregion
        #region ComProperties
        private string comName;
        public string ComName
        {
            get => comName;
            set
            {
                if (comName != value)
                {
                    comName = value;
                    OnPropertyChanged(nameof(ComName));
                    if(comName == null|| comName=="")
                    {
                        ComNameInvalid = true;
                    }
                    else
                    {
                        ComNameInvalid = false;
                    }
                }
            }
        }
        private bool comNameInvalid;
        public bool ComNameInvalid
        {
            get => comNameInvalid;
            set
            {
                if (comNameInvalid != value)
                {
                    comNameInvalid = value;
                    OnPropertyChanged(nameof(ComNameInvalid));
                }
            }
        }

        private string comDesc;
        public string ComDesc
        {
            get => comDesc;
            set
            {
                if (comDesc != value)
                {
                    comDesc = value;
                    OnPropertyChanged(nameof(ComDesc));
                }
            }
        }

        private string comCode;
        public string ComCode
        {
            get => comCode;
            set
            {
                if (comCode != value)
                {
                    comCode = value;
                    OnPropertyChanged(nameof(ComCode));
                    if (comCode == null|| comCode == "")
                    {
                        ComCodeInvalid = true;
                    }
                    else
                    {
                        ComCodeInvalid = false;
                    }
                }
            }
        }
        private bool comCodeInvalid;
        public bool ComCodeInvalid
        {
            get => comCodeInvalid;
            set
            {
                if (comCodeInvalid != value)
                {
                    comCodeInvalid = value;
                    OnPropertyChanged(nameof(ComCodeInvalid));
                }
            }
        }

        

        private string gatePhoneNum;
        public string GatePhoneNum
        {
            get => gatePhoneNum;
            set
            {
                if (gatePhoneNum != value)
                {
                    gatePhoneNum = value;
                    OnPropertyChanged(nameof(GatePhoneNum));
                }
            }
        }

        #endregion

        #region Methods
        public void ComposeCommunity()
        {
            com.ComName = ComName;
            com.ComDesc = ComDesc;
            com.ComCode = ComCode;
            com.GatePhoneNum = GatePhoneNum;
        }
        public void ComposeMember()
        {
            mem.UserId = userId;
            mem.UnitNum = UnitNum;
            mem.Balance = 0;
            mem.Role = "Manager";
            mem.IsLiable = true;
            mem.IsResident = true;
            mem.IsManager = true;
            mem.IsProvider = false;
            mem.IsApproved = true;
        }
        public async void OnCreateCommunity()
        {
            InServerCall = true;
            if (ComCodeInvalid)
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Must enter community code", "ok");
                InServerCall = false;
                return;
            }
            if (ComNameInvalid)
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Must enter community name", "ok");
                InServerCall = false;
                return;
            }
            //Check if community code is unique
            try
            {
                int existingCommunityId = await this.proxy.GetCommunityIdAsync(ComCode);

                if (existingCommunityId >= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Failed","A community with this code already exists. Please choose a different code.", "ok");
                    InServerCall = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Network error while validating community code. Please check your connection and try again.", "ok");
                InServerCall = false;
                return;
            }

            ComposeCommunity();
            ComposeMember();
            MemberCommunity Submission = new MemberCommunity {Member = this.mem, Community = this.com };
            MemberCommunity result = await this.proxy.CreateCommunityAsync(Submission);
            
            InServerCall = false;   
            if(result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Failed to create a community", "ok");
            }
            else { 
                ((App)Application.Current).CurCom = result.Community;
                ((App)Application.Current).CurMem = result.Member;
                await Application.Current.MainPage.DisplayAlert("Success!", "Community created!", "ok");

                AppShell v = serviceProvider.GetService<AppShell>();
                ((App)Application.Current).MainPage = v;
            }
        }
        private async void OnSelectCommunity()
        {
            SelectCommunityView v = serviceProvider.GetService<SelectCommunityView>();
            ((App)Application.Current).MainPage = v;
        }
        #endregion
    }
}
