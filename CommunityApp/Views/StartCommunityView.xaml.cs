using CommunityApp.ViewModels;
namespace CommunityApp.Views;

public partial class StartCommunityView : ContentPage
{
	public StartCommunityView(StartCommunityViewModel vm)
	{
        this.BindingContext = vm;
        InitializeComponent();
	}
}
