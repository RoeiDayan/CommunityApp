using CommunityApp.ViewModels;
namespace CommunityApp.Views;

public partial class IssuePaymentsView : ContentPage
{
	public IssuePaymentsView(IssuePaymentsViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}