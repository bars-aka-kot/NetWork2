using System.Net;


namespace ChatClient
{
    public interface IMessageSource
    {
        Task SendAsync(NetMessage message, IPEndPoint ep);

        NetMessage Receive(ref IPEndPoint ep);
    }
}
