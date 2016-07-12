using System.Media;
using System.Net;
using SuperChat.Properties;

namespace SuperChat.Models.Notifications
{
    public static class AudioNotification
    {
        private static readonly SoundPlayer NotificationSound;

        static AudioNotification()
        {
            NotificationSound = new SoundPlayer(Resources.NewMessageSound);
            NotificationSound.Load();
        }

        public static void OnNewMessage(IPEndPoint endPoint, byte[] message)
        {
            NotificationSound.Play();
        }
    }
}