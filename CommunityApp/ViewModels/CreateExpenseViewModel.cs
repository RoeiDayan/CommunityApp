using CommunityApp.Models;
using CommunityApp.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CommunityApp.ViewModels
{
    public class CreateExpenseViewModel : ViewModelBase
    {
        private readonly CommunityWebAPIProxy proxy;
        private readonly IServiceProvider serviceProvider;

        public CreateExpenseViewModel(CommunityWebAPIProxy proxy, IServiceProvider serviceProvider)
        {
            this.proxy = proxy;
            this.serviceProvider = serviceProvider;
            CreateExpenseCommand = new Command(async () => await CreateExpense());
            ExpenseDate = DateTime.Today;
            CreatedAt = DateTime.Now;
        }

        // Properties bound to the UI
        private string title;
        public string Title
        {
            get => title;
            set { title = value; OnPropertyChanged(); }
        }

        private string description;
        public string Description
        {
            get => description;
            set { description = value; OnPropertyChanged(); }
        }

        private decimal amount;
        public decimal Amount
        {
            get => amount;
            set { amount = value; OnPropertyChanged(); }
        }

        private DateTime expenseDate;
        public DateTime ExpenseDate
        {
            get => expenseDate;
            set { expenseDate = value; OnPropertyChanged(); }
        }

        private DateTime createdAt;
        public DateTime CreatedAt
        {
            get => createdAt;
            set { createdAt = value; OnPropertyChanged(); }
        }

        private bool inServerCall;
        public bool InServerCall
        {
            get => inServerCall;
            set { inServerCall = value; OnPropertyChanged(); }
        }

        public ICommand CreateExpenseCommand { get; }

        private async Task CreateExpense()
        {
            InServerCall = true;

            // Validate input
            if (string.IsNullOrWhiteSpace(Title) || Amount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Please enter a valid title and amount.", "OK");
                InServerCall = false;
                return;
            }

            int userId = ((App)Application.Current).LoggedInUser.Id;
            int comId = ((App)Application.Current).CurCom.ComId;

            var expense = new Expense
            {
                ComId = comId,
                Title = Title,
                Description = Description,
                Amount = Amount,
                ExpenseDate = DateOnly.FromDateTime(ExpenseDate),
                CreatedAt = DateTime.Now
            };

            bool success = await proxy.CreateExpenseAsync(expense);
            InServerCall = false;

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Expense created successfully.", "OK");
                AppShell v = serviceProvider.GetService<AppShell>();
                ((App)Application.Current).MainPage = v;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to create expense.", "OK");
            }
        }
    }
}
