using CommunityApp.ViewModels;
namespace CommunityApp.Views;

public partial class JoinCommunityView : ContentPage
{
	public JoinCommunityView(JoinCommunityViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}