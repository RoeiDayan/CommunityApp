using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class ProfilePageView : ContentPage
{
	public ProfilePageView(ProfilePageViewModel vm)
	{
		this.BindingContext = vm;
        InitializeComponent();
	}
}