using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class HomePageView : ContentPage
{
	public HomePageView(HomePageViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
    private async void OnRefreshButtonClicked(object sender, EventArgs e)
    {
        // Scale the button up
        await (sender as Button).ScaleTo(1.2, 150, Easing.CubicOut);
        // Scale it back to original size
        await (sender as Button).ScaleTo(1, 150, Easing.CubicIn);
    }
}