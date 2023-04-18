namespace FirebaseChat.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SecondPage : ContentPage
{
	public SecondPage()
	{
		InitializeComponent();

		BindingContext = new SecondViewModel();
	}
}