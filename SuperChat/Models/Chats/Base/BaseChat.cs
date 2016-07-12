using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SuperChat.Models.ConfigurationManagement;
using SuperChat.Models.Encryption;
using SuperChat.Models.ExceptionManagement;
using SuperChat.Models.ExceptionManagement.Exceptions;

namespace SuperChat.Models.Chats.Base
{
    public abstract class BaseChat : IDisposable
    {
        private readonly UdpClient client = new UdpClient();
        private readonly NetworkPacketFactory networkPacketFactory;
        private IPEndPoint broadcastEndpoint;
        private Thread listenerThread = new Thread(StartListen);
        private UdpClient server;

        protected BaseChat()
        {
            // ReSharper disable once VirtualMemberCallInContructor
            ChatConfigurationManager = new ChatConfigurationManager(ChatConfigurationFile);
            networkPacketFactory = new NetworkPacketFactory(ChatConfigurationManager);
            ChatConfigurationManager.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Port")
                    InitializeServer();
            };
            client.EnableBroadcast = true;
            listenerThread.IsBackground = true;
            InitializeServer();
        }

        public ChatConfigurationManager ChatConfigurationManager { get; }

        public abstract string ChatConfigurationFile { get; }

        public bool Active { get; set; } = true;
        public bool Disposed { get; private set; }

        public HashSet<EndPointModel> EndPoints { get; set; } =
            new HashSet<EndPointModel>(new EndPointModelEqualityComparer());

        public virtual void Dispose()
        {
            Disposed = true;
            Active = false;
            server.Close();
            client.Close();
            listenerThread.Abort();
            listenerThread.Join();
        }

        private void InitializeServer()
        {
            try
            {
                if (listenerThread.IsAlive)
                {
                    server.Close();
                    listenerThread.Abort();
                    listenerThread.Join();
                    listenerThread = new Thread(StartListen);
                }
                server = new UdpClient(new IPEndPoint(IPAddress.Any, ChatConfigurationManager.Port));
                broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, ChatConfigurationManager.Port);
                listenerThread.Start(this);
            }
            catch (Exception ex)
            {
                ExceptionManager.Resolve(new ChatCreateException(this, ex));
            }
        }

        private static void StartListen(object arg)
        {
            var chatClient = arg as BaseChat;
            try
            {
                if (chatClient == null)
                {
                    throw new ArgumentNullException(nameof(chatClient));
                }
                while (chatClient.Active)
                {
                    var th = Thread.CurrentThread;
                    var senderEndpoint = new IPEndPoint(IPAddress.Any, chatClient.ChatConfigurationManager.Port);
                    var receivedBytes = chatClient.server.Receive(ref senderEndpoint);
                    chatClient.EndPoints.Add(new EndPointModel {EndPoint = senderEndpoint, IsIgnored = false});
                    chatClient.OnNewMessage(senderEndpoint, receivedBytes);
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Resolve(new ChatException(chatClient, ex));
            }
        }

        private void OnNewMessage(IPEndPoint endpoint, byte[] message)
        {
            if (EndPoints.Any(endPoint => endPoint.EndPoint.Address.Equals(endpoint.Address) && endPoint.IsIgnored))
                return;
            var encodedBytes = networkPacketFactory.ParsePacket(message);
            if (encodedBytes != null)
                NewMessage?.Invoke(endpoint, encodedBytes);
        }

        protected void Send(byte[] message)
        {
            try
            {
                var newMessage = networkPacketFactory.CreatePacket(message);
                client.Send(newMessage, newMessage.Length, broadcastEndpoint);
            }
            catch (Exception ex)
            {
                ExceptionManager.Resolve(new ChatException(this, ex));
            }
        }

        protected event NewMessageHandler NewMessage;

        protected delegate void NewMessageHandler(IPEndPoint endPoint, byte[] message);
    }
}