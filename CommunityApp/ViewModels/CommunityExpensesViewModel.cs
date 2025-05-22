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

namespace CommunityApp.ViewModels
{
    public class CommunityExpensesViewModel : ViewModelBase
    {
        private CommunityWebAPIProxy proxy;
        private IServiceProvider serviceProvider;

        public CommunityExpensesViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.proxy = proxy;
            _ = FetchCommunityExpenses();
            FetchExpensesCommand = new Command(async () => await FetchCommunityExpenses());
        }

        #region Properties
        private ObservableCollection<Expense> expenses = new ObservableCollection<Expense>();
        public ObservableCollection<Expense> Expenses
        {
            get => expenses;
            set
            {
                expenses = value ?? new ObservableCollection<Expense>();
                OnPropertyChanged(nameof(Expenses));
                OnPropertyChanged(nameof(TotalExpenses));
            }
        }

        public decimal TotalExpenses => Expenses?.Sum(e => e.Amount) ?? 0;
        #endregion

        #region Commands
        public ICommand FetchExpensesCommand { get; }
        #endregion

        #region Methods
        private async Task FetchCommunityExpenses()
        {
            try
            {
                List<Expense> expensesFromServer = await proxy.GetCommunityExpensesAsync(((App)Application.Current).CurCom.ComId);

                // Sort by expense date (newest first) and then by created date
                var sortedExpenses = expensesFromServer
                    .OrderByDescending(e => e.ExpenseDate)
                    .ThenByDescending(e => e.CreatedAt)
                    .ToList();

                Expenses.Clear();
                foreach (var expense in sortedExpenses)
                {
                    Expenses.Add(expense);
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Failed to retrieve expenses", "Encountered a problem with fetching the community expenses. Please try again", "ok");
            }
        }
        #endregion
    }
}