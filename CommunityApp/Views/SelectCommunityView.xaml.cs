using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class SelectCommunityView : ContentPage
{
	public SelectCommunityView(SelectCommunityViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}