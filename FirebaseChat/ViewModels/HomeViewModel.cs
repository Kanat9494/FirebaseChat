using Firebase.Database.Query;
using System.Reactive.Linq;

namespace FirebaseChat.ViewModels;

internal class HomeViewModel : ViewModelBase
{
    public HomeViewModel()
    {
        Messages = new ObservableCollection<Message>();
        firebaseClient = new FirebaseClient("https://mauichat-e3ebb-default-rtdb.europe-west1.firebasedatabase.app/");
        senderName = "996505505505";
        receiverName = "996701555268";
        SendMessage = new Command(async () =>
        {
            await OnSendMessage();
        });

        var collection = firebaseClient
        .Child("Messages")
        .AsObservable<Message>()
        .Where(m => m.Object.SenderName == receiverName && m.Object.ReceiverName == senderName)
        .Subscribe((item) =>
        {
            if (item.Object != null)
            {
                Messages.Add(item.Object);
            }
        });

        AbortCommand = new Command(() => collection.Dispose());
    }

    public Command SendMessage { get; }
    public Command AbortCommand { get; }
    FirebaseClient firebaseClient;
    private string senderName;
    private string receiverName;

    public ObservableCollection<Message> Messages { get; set; }

    private string _sendingMessage;
    public string SendingMessage
    {
        get => _sendingMessage;
        set => SetProperty(ref _sendingMessage, value);
    }

    async Task OnSendMessage()
    {
        try
        {
            var message = new Message()
            {
                SenderName = senderName,
                ReceiverName = receiverName,
                Content = SendingMessage,
                //ImageUrl = "https://www.google.com/images/logos/ps_logo2.png"

            };

            var serializedMessage = JsonConvert.SerializeObject(message);
            await firebaseClient.Child("Messages").PostAsync(serializedMessage);
            //это лишнее убрал, чтобы не отправлять сообщение 2 раза
            //SendLocalMessage(message);
        }
        catch (Exception ex)
        {

        }
    }

    private void SendLocalMessage(Message message)
    {
        if (string.IsNullOrEmpty(message.Content))
            return;


        #region сохранение фото в локальном хранилище
        //if (message.ImageUrl != null)
        //{
        //    Task.Run(async () =>
        //    {
        //        var imabeBytes = await FileHelper.DownloadImageBytesAsync(message.ImageUrl);
        //        if (imabeBytes != null)
        //        {
        //            var c = await FileHelper.SaveFileAsync(imabeBytes);
        //            message.ImageUrl = c;
        //        }
        //    }).Wait();
        //}
        #endregion

        Messages.Add(message);

        SendingMessage = string.Empty;
    }
}
