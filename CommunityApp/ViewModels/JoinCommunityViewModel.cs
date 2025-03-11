using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityApp.ViewModels
{
    public class JoinCommunityViewModel:ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        public JoinCommunityViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            JoinCommand = new Command(OnJoinCommunity);
        }
        #region Properties
        private string communityCode;
        public string CommunityCode
        {
            get => communityCode;
            set
            {
                if (communityCode != value)
                {
                    communityCode = value;
                    OnPropertyChanged(nameof(CommunityCode));
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
                    unitNum = value; OnPropertyChanged(nameof(UnitNum));
                }
            }
        }

        private bool isLiable;
        public bool IsLiable
        {
            get => isLiable;
            set
            {
                isLiable = value; OnPropertyChanged(nameof(IsLiable));
            }
        }

        private bool isResident;
        public bool IsResident
        {
            get => isResident;
            set
            {
                isResident = value; OnPropertyChanged(nameof (IsResident));
            }
        }

        private bool isManager;
        public bool IsManager
        {
            get => isManager;
            set
            {
                isManager = value; OnPropertyChanged(nameof(IsManager));
            }
        }

        private bool isProvider;
        public bool IsProvider
        {
            get => isProvider;
            set
            {
                isProvider = value; OnPropertyChanged(nameof(IsProvider));
            }
        }

        #endregion

        #region Commands
        public Command JoinCommand { get;}
        public Command LoginCommand { get;}
        #endregion

        #region Methods
        private async void OnJoinCommunity()
        {

        }
        #endregion
    }
}
