using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class ManageReportsView : ContentPage
{
	public ManageReportsView(ManageReportsViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}