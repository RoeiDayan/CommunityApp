using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class CreateReportView : ContentPage
{
	public CreateReportView(CreateReportViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}