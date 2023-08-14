namespace FirebaseChat.ViewModels;

internal class SecondViewModel : ViewModelBase
{
    public SecondViewModel()
    {
        ChatCommand = new Command(OnChat);
        ToTestPageCommand = new Command(async () => await OnToTestPage());
    }

    public Command ChatCommand { get; }
    public Command ToTestPageCommand { get; }

    private async void OnChat()
    {
        await Shell.Current.GoToAsync(nameof(ChatPage));
    }

    private async Task OnToTestPage()
    {
        await Shell.Current.GoToAsync($"//TestPage?{nameof(TestViewModel.ProductPrice)}=35455");
    }
}
