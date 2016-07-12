using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using SuperChat.Models.ExceptionManagement.Exceptions;
using SuperChat.Properties;

namespace SuperChat.Models.ExceptionManagement
{
    public static class ExceptionManager
    {
        private static readonly Dictionary<Type, ResolvingFunction> ResolvesDictionary = new Dictionary
            <Type, ResolvingFunction>
        {
            [typeof (ChatException)] = ResolveChatException,
            [typeof (ChatCreateException)] = ResolveChatCreateException
        };

        private static string ResolveSocketError(SocketException exception)
        {
            switch (exception.SocketErrorCode)
            {
                case SocketError.AddressAlreadyInUse:
                    return Resources.SocketAlreadyInUseMessage;
                default:
                    return Resources.SocketExceptionMessage;
            }
        }

        private static string ResolveChatCreateException(Exception exception)
        {
            var chatCreateException = exception as ChatCreateException;
            if (chatCreateException == null)
                return UnresolvedException(exception);
            var innerException = chatCreateException.InnerException as SocketException;
            if (innerException != null)
            {
                return $"{chatCreateException.Message} {chatCreateException.ExceptionalChat.GetType()}\n" +
                       ResolveSocketError(innerException);
            }
            if (chatCreateException.InnerException is ArgumentOutOfRangeException)
            {
                return $"{chatCreateException.Message} {chatCreateException.ExceptionalChat.GetType()}\n" +
                       Resources.WrongPortExceptionMessage;
            }
            return UnresolvedException(chatCreateException.InnerException);
        }

        private static string ResolveChatException(Exception exception)
        {
            var chatException = exception as ChatException;
            if (chatException == null)
                return UnresolvedException(exception);
            if (chatException.InnerException is ThreadAbortException)
                return string.Empty;
            return chatException.ExceptionalChat.Disposed ? string.Empty : UnresolvedException(exception.InnerException);
        }

        private static string UnresolvedException(Exception exception)
        {
            return $"{Resources.UnresolvedExceptionPreamble}\n{exception.Message}";
        }

        public static void Resolve(Exception exception)
        {
            ResolvingFunction resolvingFunction;
            var resolvingMessage = ResolvesDictionary.TryGetValue(exception.GetType(), out resolvingFunction)
                ? resolvingFunction(exception)
                : UnresolvedException(exception);
            if (!string.IsNullOrWhiteSpace(resolvingMessage))
                MessageBox.Show(resolvingMessage, "OMG");
        }

        private delegate string ResolvingFunction(Exception exception);
    }
}