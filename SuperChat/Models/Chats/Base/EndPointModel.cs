using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using SuperChat.Properties;

namespace SuperChat.Models.Chats.Base
{
    public sealed class EndPointModel : INotifyPropertyChanged
    {
        private IPEndPoint endPoint;
        private bool isIgnored;

        public IPEndPoint EndPoint
        {
            get { return endPoint; }
            set
            {
                endPoint = value;
                OnPropertyChanged();
            }
        }

        public bool IsIgnored
        {
            get { return isIgnored; }
            set
            {
                isIgnored = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}