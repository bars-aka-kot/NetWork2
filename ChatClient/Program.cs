using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ChatClient;
public class Program
{
    static void Main(string[] args)
    {
       Client client;
    }


}

public class Client
{
    private readonly string _name;

    private readonly IMessageSource _messageSouce;
    private IPEndPoint remoteEndPoint;
    public Client(string name, string address, int port)
    {
        this._name = name;

        _messageSouce = new UdpMessageSouce();
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
    }

    UdpClient udpClientClient = new UdpClient();
    async Task ClientListener()
    {
        while (true)
        {
            try
            {
                var messageReceived = _messageSouce.Receive(ref remoteEndPoint);

                Console.WriteLine($"Получено сообщение от {messageReceived.NickNameFrom}:");
                Console.WriteLine(messageReceived.Text);

                await Confirm(messageReceived, remoteEndPoint);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении сообщения: " + ex.Message);
            }
        }
    }

    async Task Confirm(NetMessage message, IPEndPoint remoteEndPoint)
    {
        message.Command = Command.Confirmation;
        await _messageSouce.SendAsync(message, remoteEndPoint);
    }


    void Register(IPEndPoint remoteEndPoint)
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        var message = new NetMessage() { NickNameFrom = _name, NickNameTo = null, Text = null, Command = Command.Register, EndPoint = ep };
        _messageSouce.SendAsync(message, remoteEndPoint);
    }

    async Task ClientSender()
    {


        Register(remoteEndPoint);

        while (true)
        {
            try
            {
                Console.Write("Введите  имя получателя: ");
                var nameTo = Console.ReadLine();

                Console.Write("Введите сообщение и нажмите Enter: ");
                var messageText = Console.ReadLine();

                var message = new NetMessage() { Command = Command.Message, NickNameFrom = _name, NickNameTo = nameTo, Text = messageText };

                await _messageSouce.SendAsync(message, remoteEndPoint);

                Console.WriteLine("Сообщение отправлено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
            }
        }
    }

    public async Task Start()
    {
        //udpClientClient = new UdpClient(port);

        await ClientListener();

        await ClientSender();

    }
}

internal class UdpMessageSouce : IMessageSource
{
    private readonly UdpClient _udpClient;
    public UdpMessageSouce()
    {
        _udpClient = new UdpClient(12345);
    }
    public NetMessage Receive(ref IPEndPoint ep)
    {
        byte[] data = _udpClient.Receive(ref ep);
        string str = Encoding.UTF8.GetString(data);
        return NetMessage.DeserializeMessgeFromJSON(str) ?? new NetMessage();
    }

    public async Task SendAsync(NetMessage message, IPEndPoint ep)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message.SerialazeMessageToJSON());

        await _udpClient.SendAsync(buffer, buffer.Length, ep);
    }
}