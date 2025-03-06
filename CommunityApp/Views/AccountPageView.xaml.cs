using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class AccountPageView : ContentPage
{
	public AccountPageView(AccountPageViewModel vm)
	{
        this.BindingContext = vm;
		InitializeComponent();
	}
}