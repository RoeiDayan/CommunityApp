﻿using CommunityApp.Models;
using CommunityApp.Services;
using CommunityApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommunityApp.ViewModels
{
    public class HomePageViewModel:ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;
        
        public HomePageViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            CurrentCom = ((App)Application.Current).CurCom;
        }
        private Community? currentCom;
        public Community? CurrentCom
        {
            get => currentCom;
            set
            {
                currentCom = value;
                OnPropertyChanged("CurrentCom");
            }
        }
    }
}
