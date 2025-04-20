using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class CreateNoticeView : ContentPage
{
	public CreateNoticeView(CreateNoticeViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}