using System.Net;
using System.Text;
using SuperChat.Models.Chats.Base;
using SuperChat.Models.Notifications;

namespace SuperChat.Models.Chats.Text
{
    public sealed class TextChat : BaseChat
    {
        public delegate void NewTextMessageHandler(IPEndPoint clientEndPoint, string newMessage);

        public TextChat()
        {
            NewMessage += (point, message) => OnNewTextMessage(point, Encoding.Unicode.GetString(message));
            NewMessage += AudioNotification.OnNewMessage;
        }

        public override string ChatConfigurationFile => "TextChat.xml";

        public void SendMessage(string message)
        {
            Send(Encoding.Unicode.GetBytes(message));
        }

        public event NewTextMessageHandler NewTextMessage;

        private void OnNewTextMessage(IPEndPoint clientEndPoint, string newMessage)
        {
            NewTextMessage?.Invoke(clientEndPoint, newMessage);
        }
    }
}