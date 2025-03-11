using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityApp.ViewModels
{
    public class ProfilePageViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        public ProfilePageViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            CurMember = ((App)Application.Current).CurMem;
            MemberName = ((App)Application.Current).LoggedInUser.Name;
            Balance = CurMember.Balance;
            UnitNum = CurMember.UnitNum;
            IsLiable = CurMember.IsLiable;
            IsResident = CurMember.IsResident;
            IsManager = CurMember.IsManager;
            ChooseRole();
        }

        public void ChooseRole()
        {
            if (IsManager.HasValue && IsManager.Value)
            {
                MemberRole = "Manager";
            }
            else if (IsProvider.HasValue && IsProvider.Value)
            {
                MemberRole = "Provider";
            }
            else
            {
                MemberRole = "Resident";
            }
        }
        private string memberName;
        private Member? curMember;
        public Member? CurMember
        {
            get => curMember;
            set
            {
                curMember = value;
                OnPropertyChanged("CurMember");
             
            }
        }
        public string MemberName
        {
            get { return memberName; }
            set
            {
                memberName = value;
                OnPropertyChanged("MemberName");
            }
        }
        private string? memberRole;

        public int? balance;

        public int? unitNum;

        public bool? isLiable;

        public bool? isResident;

        public bool? isManager;

        public bool? isProvider;

        public string? MemberRole
        {
            get => memberRole;
            set
            {
                memberRole = value;
                OnPropertyChanged("MemberRole");

            }
        }
        public int? Balance
        {
            get => balance;
            set
            {
                balance = value;
                OnPropertyChanged("Balance");

            }
        }
        public int? UnitNum
        {
            get => unitNum;
            set
            {
                unitNum = value;
                OnPropertyChanged("UnitNum");

            }
        }
        public bool? IsLiable
        {
            get => isLiable;
            set
            {
                isLiable = value;
                OnPropertyChanged("IsLiable");

            }
        }
        public bool? IsResident
        {
            get => isResident;
            set
            {
                isResident = value;
                OnPropertyChanged("IsResident");

            }
        }
        public bool? IsManager
        {
            get => isManager;
            set
            {
                isManager = value;
                OnPropertyChanged("IsManager");

            }
        }
        public bool? IsProvider
        {
            get => isProvider;
            set
            {
                isProvider = value;
                OnPropertyChanged("IsProvider");

            }
        }
    }
}
