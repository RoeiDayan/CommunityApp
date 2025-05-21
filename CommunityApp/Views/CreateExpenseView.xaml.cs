using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class CreateExpenseView : ContentPage
{
	public CreateExpenseView(CreateExpenseViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}