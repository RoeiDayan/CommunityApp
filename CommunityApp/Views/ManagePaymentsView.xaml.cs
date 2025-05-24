using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class ManagePaymentsView : ContentPage
{
	public ManagePaymentsView(ManagePaymentsViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}