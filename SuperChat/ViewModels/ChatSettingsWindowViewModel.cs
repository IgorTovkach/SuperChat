using System.Collections.Generic;
using SuperChat.Models.Chats.Base;
using SuperChat.MvvmCore;

namespace SuperChat.ViewModels
{
    public class ChatSettingsWindowViewModel : ViewModelBase
    {
        private BaseChat baseChat;
        private string chatName;
        public IEnumerable<EndPointModel> EndPoints => baseChat?.EndPoints;

        public BaseChat BaseChat
        {
            get { return baseChat; }
            set
            {
                baseChat = value;
                OnPropertyChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(EndPoints));
            }
        }

        public string ChatName
        {
            get { return chatName; }
            set
            {
                chatName = value;
                OnPropertyChanged();
            }
        }
    }
}