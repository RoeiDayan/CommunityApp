using CommunityApp.ViewModels;
namespace CommunityApp.Views;

public partial class ManageMembersView : ContentPage
{
	public ManageMembersView(ManageMembersViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}