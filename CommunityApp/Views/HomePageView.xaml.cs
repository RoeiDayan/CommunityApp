using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class HomePageView : ContentPage
{
	public HomePageView(HomePageViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}