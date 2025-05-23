using CommunityApp.ViewModels;

namespace CommunityApp.Views;

public partial class PaymentHistoryView : ContentPage
{
	public PaymentHistoryView(PaymentHistoryViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
	}
}