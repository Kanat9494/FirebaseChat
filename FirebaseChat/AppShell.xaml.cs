namespace FirebaseChat;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(ChatPage), typeof(ChatPage));
    }
}
