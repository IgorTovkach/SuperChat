using System;
using SuperChat.Models.Chats.Base;
using SuperChat.Properties;

namespace SuperChat.Models.ExceptionManagement.Exceptions
{
    public class ChatCreateException : ApplicationException
    {
        public ChatCreateException(BaseChat exceptionalChat, Exception innerException)
            : base(Resources.ChatCreateExceptionMessage, innerException)
        {
            ExceptionalChat = exceptionalChat;
        }

        public BaseChat ExceptionalChat { get; }
    }
}