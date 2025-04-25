using CommunityApp.ViewModels;
namespace CommunityApp.Views;

public partial class ManageJoinRequestsView : ContentPage
{
	public ManageJoinRequestsView(ManageJoinRequestsViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}