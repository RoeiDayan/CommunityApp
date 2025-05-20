using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class RoomRequestView : ContentPage
{
	public RoomRequestView(RoomRequestViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}