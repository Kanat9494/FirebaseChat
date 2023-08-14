namespace FirebaseChat.Views;

public partial class TestPage : ContentPage
{
	public TestPage()
	{
		InitializeComponent();

		BindingContext = new TestViewModel();
	}
}