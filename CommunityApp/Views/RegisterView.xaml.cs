using CommunityApp.ViewModels;
namespace CommunityApp.Views;

public partial class RegisterView : ContentPage
{
	public RegisterView(RegisterViewModel vm)
	{
        this.BindingContext = vm;
        InitializeComponent();
	}
}