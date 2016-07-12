using System;
using SuperChat.Models.Chats.Base;
using SuperChat.Properties;

namespace SuperChat.Models.ExceptionManagement.Exceptions
{
    public class ChatException : ApplicationException
    {
        public ChatException(BaseChat exceptionalChat, Exception innerException)
            : base(Resources.ChatExceptionMessage, innerException)
        {
            ExceptionalChat = exceptionalChat;
        }

        public BaseChat ExceptionalChat { get; }
    }
}