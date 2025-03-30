using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class MemberListView : ContentPage
{
	public MemberListView(MemberListViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}