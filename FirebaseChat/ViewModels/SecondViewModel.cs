namespace FirebaseChat.ViewModels;

internal class SecondViewModel : ViewModelBase
{
    public SecondViewModel()
    {
        ChatCommand = new Command(OnChat);
    }

    public Command ChatCommand { get; }

    private async void OnChat()
    {
        await Shell.Current.GoToAsync(nameof(ChatPage));
    }
}
