using CommunityApp.ViewModels;
namespace CommunityApp.Views;

public partial class CommunityExpensesView : ContentPage
{
	public CommunityExpensesView(CommunityExpensesViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}