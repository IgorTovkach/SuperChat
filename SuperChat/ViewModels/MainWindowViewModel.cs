using System;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using SuperChat.Models.Chats.Text;
using SuperChat.Models.Chats.Voice;
using SuperChat.Models.ExceptionManagement;
using SuperChat.MvvmCore;
using SuperChat.Views;

namespace SuperChat.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly StringBuilder chat = new StringBuilder();
        private DelegateCommand closeCommand;
        private DelegateCommand loadedCommand;
        private string login;
        private string message;
        private DelegateCommand sendCommand;
        private DelegateCommand showAudioChatClientsCommand;
        private DelegateCommand showTextChatClientsCommand;

        private TextChat textChat;
        private VoiceChat voiceChat;

        public string Login
        {
            get { return login; }
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }

        public string Chat
        {
            get { return chat.ToString(); }
            set
            {
                chat.Append(value);
                OnPropertyChanged();
            }
        }

        public ICommand SendCommand => sendCommand ?? (sendCommand = new DelegateCommand(Send, CanSend));
        public ICommand CloseCommand => closeCommand ?? (closeCommand = new DelegateCommand(Close));

        public ICommand ShowTextChatClientsCommand
            => showTextChatClientsCommand ?? (showTextChatClientsCommand = new DelegateCommand(ShowTextChatClients));

        public ICommand ShowAudioChatClientsCommand
            => showAudioChatClientsCommand ?? (showAudioChatClientsCommand = new DelegateCommand(ShowAudioChatClients));

        public ICommand LoadedCommand => loadedCommand ?? (loadedCommand = new DelegateCommand(Loaded));

        private void Loaded()
        {
            try
            {
                textChat = new TextChat();
                textChat.NewTextMessage += OnNewMessage;
                voiceChat = new VoiceChat();
            }
            catch (Exception ex)
            {
                ExceptionManager.Resolve(ex);
            }
        }

        private void OnNewMessage(IPEndPoint endPoint, string newMessage)
        {
            Chat = newMessage;
            if (newMessage == $"{Login} :: {Message}\n")
                Message = string.Empty;
        }

        private void ShowAudioChatClients()
        {
            var window = new ChatSettingsWindow();
            var viewModel = window.DataContext as ChatSettingsWindowViewModel;
            if (viewModel == null)
            {
                return;
            }
            viewModel.ChatName = "Audio";
            viewModel.BaseChat = voiceChat;
            window.Show();
        }

        private void ShowTextChatClients()
        {
            var window = new ChatSettingsWindow();
            var viewModel = window.DataContext as ChatSettingsWindowViewModel;
            if (viewModel == null)
            {
                return;
            }
            viewModel.ChatName = "Text";
            viewModel.BaseChat = textChat;
            window.Show();
        }

        private void Close()
        {
            try
            {
                textChat.Dispose();
                voiceChat.Dispose();
                Application.Current.Shutdown(0);
            }
            catch (Exception ex)
            {
                ExceptionManager.Resolve(ex);
            }
        }

        private bool CanSend()
        {
            return !string.IsNullOrWhiteSpace(Message) && !string.IsNullOrWhiteSpace(Login);
        }

        private void Send()
        {
            try
            {
                textChat.SendMessage($"{Login} :: {Message}\n");
            }
            catch (Exception ex)
            {
                ExceptionManager.Resolve(ex);
            }
        }
    }
}