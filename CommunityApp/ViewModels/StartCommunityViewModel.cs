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
            Role = "Manager";
            Balance = 0;
            IsResident = false;
        }

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
                }
            }
        }

        private string picture;
        public string Picture
        {
            get => picture;
            set
            {
                if (picture != value)
                {
                    picture = value;
                    OnPropertyChanged(nameof(Picture));
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
            com.Picture = Picture;
            com.GatePhoneNum = GatePhoneNum;
        }
        public void ComposeMember()
        {
            mem.UserId = userId;
            mem.UnitNum = UnitNum;
            mem.Balance = Balance;
            mem.Role = Role;
            mem.IsLiable = true;
            mem.IsResident = IsResident;
            mem.IsManager = true;
            mem.IsProvider = false;
            mem.IsApproved = true;
        }
        public async void CreateCommunity()
        {
            MemberCommunity Submission = new MemberCommunity {Member = this.mem, Community = this.com };
            MemberCommunity result = await this.proxy.CreateCommunityAsync(Submission);
            ((App)Application.Current).CurCom = result.Community;
            ((App)Application.Current).CurMem = result.Member;

            AppShell v = serviceProvider.GetService<AppShell>();
            ((App)Application.Current).MainPage = v;
        }
        #endregion
    }
}
