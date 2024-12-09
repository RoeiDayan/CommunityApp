using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}