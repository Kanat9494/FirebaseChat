using System.Text;

namespace FirebaseChat.ViewModels;

internal class ChatViewModel : ViewModelBase
{
    public ChatViewModel()
    {
        Messages = new ObservableCollection<Message2>();
        userName = "user";
        receiverName = "doctor";

        SendMessage = new Command(async () =>
        {
            await OnSendMessage();
        });

        AbortCommand = new Command(Abort);

        client = new TcpClient();

        ConnectTcp();
    }

    void ConnectTcp()
    {
        try
        {
            client.Connect(host, port);
            stream = client.GetStream();

            var message = new Message2()
            {
                SenderName = userName,
                ReceiverName = receiverName,
                Content = SendingMessage
            };

            var jsonMessage = JsonConvert.SerializeObject(message); 
            byte[] data = Encoding.UTF8.GetBytes(jsonMessage);  
            stream.Write(data, 0, data.Length);

            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
            //Task.Run(async () =>
            //{
            //    await ReceiveMessage();
            //});
            //new Thread(new ThreadStart(() =>
            //{
            //    ReceiveMessage();
            //})).Start();
            //SendTcpMessage(message);
        }
        catch (Exception ex)
        {
            Disconnect();
        }
    }

    string userName;
    string receiverName;
    private const string host = "192.168.2.33";
    private const int port = 8888;
    TcpClient client;
    NetworkStream stream;

    public Command SendMessage { get; }
    public Command AbortCommand { get; }
    public ObservableCollection<Message2> Messages { get; set; }

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
            var message = new Message2()
            {
                SenderName = userName,
                ReceiverName = receiverName,
                Content = SendingMessage,
                //ImageUrl = "https://www.google.com/images/logos/ps_logo2.png"

            };

            SendLocalMessage(message);
            SendTcpMessage(message);
        }
        catch (Exception ex)
        {

        }
    }

    async void Abort()
    {

    }

    private void SendLocalMessage(Message2 message)
    {
        if (string.IsNullOrEmpty(message.Content))
            return;


        Messages.Add(message);

        SendingMessage = string.Empty;
    }




    #region Tcp messages
    void SendTcpMessage(Message2 message)
    {
        var jsonMessage = JsonConvert.SerializeObject(message);
        byte[] data = Encoding.Unicode.GetBytes(jsonMessage);
        stream.Write(data, 0, data.Length);
    }

    void ReceiveMessage()
    {
        
        while (true)
        {
            try
            {
                byte[] data = new byte[64];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                if (stream.CanRead)
                {
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        int count = 0;

                        if (builder.Length > 0)
                        {
                            try
                            {
                                App.Current.Dispatcher.Dispatch(() =>
                                {
                                    SendLocalMessage(new Message2
                                    {
                                        SenderName = receiverName,
                                        ReceiverName = userName,
                                        Content = builder.ToString()
                                    });
                                });
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    while (stream.DataAvailable && bytes > 0);
                }

                //App.Current.Dispatcher.Dispatch(() =>
                //{
                //    SendLocalMessage(new Message2 { Content = builder.ToString() });
                //});
                    
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }
    }

    void Disconnect()
    {
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();

        Environment.Exit(0);
    }
    #endregion
}
