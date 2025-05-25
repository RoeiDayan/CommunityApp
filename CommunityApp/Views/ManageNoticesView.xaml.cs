using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class ManageNoticesView : ContentPage
{
	public ManageNoticesView(ManageNoticesViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}