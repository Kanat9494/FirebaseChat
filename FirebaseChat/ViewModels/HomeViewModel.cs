using Firebase.Database.Query;
using Microsoft.Maui.Controls;
using System.Reactive.Linq;

namespace FirebaseChat.ViewModels;

internal class HomeViewModel : ViewModelBase
{
    public HomeViewModel()
    {
        Messages = new ObservableCollection<Message>();
        firebaseClient = new FirebaseClient("https://mauichat-e3ebb-default-rtdb.europe-west1.firebasedatabase.app/");
        senderName = "996701555268";
        receiverName = "996505505505";
        SendMessage = new Command(async () =>
        {
            await OnSendMessage();
        });

        var collection = firebaseClient
        .Child("Messages")
        .OrderByPriority()
        .LimitToLast(1)
        .AsObservable<Message>()
        //.Where(m => m.Object.ReceiverName == senderName && m.Object.SenderName == receiverName)
        .Subscribe((item) =>
        {
            if (item.Object != null)
            {
                Messages.Add(item.Object);

                string lastMessageId = item.Key;

                // Удаляем последнее сообщение из базы данных
                //Task.Run(async () =>
                //{
                //    await firebaseClient.Child("Messages").Child(lastMessageId).DeleteAsync();
                //});
            }
        });

        //var c = firebaseClient
        //    .Child("Messages")
        //    .OrderByPriority()
        //    .LimitToLast(1)
        //    .AsObservable<Message>()



        //AbortCommand = new Command(() => collection.Dispose());
        AbortCommand = new Command(() => firebaseClient.Dispose());

        //Messages.CollectionChanged += Messages_CollectionChanged;
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
            SendingMessage = string.Empty;
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

    private void OnItemsUpdated()
    {

    }
    private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (Messages.Count > 0)
        {
            var lastMessage = Messages.Count - 1;
            //int count = lastMessage.Coun
            //var lastMessage = Messages.Count(Messages.Count() - 1);

            try
            {
                App.Current.Dispatcher.Dispatch(() =>
                {
                    Application.Current.MainPage.FindByName<CollectionView>("ChatCollectionView").ScrollTo(lastMessage, position: ScrollToPosition.End);
                });
            }
            catch
            {

            }
        }
    }
}
