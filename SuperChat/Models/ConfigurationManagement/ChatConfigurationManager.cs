using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using SuperChat.Properties;

namespace SuperChat.Models.ConfigurationManagement
{
    [Serializable]
    public class ChatConfigurationManager : INotifyPropertyChanged
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof (ChatConfigurationManager));
        private bool encryption;
        private string encryptionKey;
        private string path;

        private int port;

        public ChatConfigurationManager()
        {
            path = default(string);
            port = default(int);
            encryption = default(bool);
            encryptionKey = default(string);
        }

        public ChatConfigurationManager(string path)
        {
            Path = path;
            if (!File.Exists(path))
            {
                port = default(int);
                encryption = default(bool);
                encryptionKey = "Default";
            }
            else
            {
                using (var stream = File.OpenRead(path))
                {
                    var obj = Serializer.Deserialize(stream) as ChatConfigurationManager;
                    if (obj == null)
                    {
                        throw new XmlException("Configuration file corrupt");
                    }
                    Encryption = obj.Encryption;
                    Port = obj.Port;
                    EncryptionKey = obj.encryptionKey;
                }
            }
            PropertyChanged += (sender, args) => Save();
        }

        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get { return port; }
            set
            {
                if (value > IPEndPoint.MinPort && value < IPEndPoint.MaxPort)
                {
                    port = value;
                    OnPropertyChanged();
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.WrongPortExceptionMessage);
            }
        }

        public bool Encryption
        {
            get { return encryption; }
            set
            {
                encryption = value;
                OnPropertyChanged();
            }
        }

        public string EncryptionKey
        {
            get { return encryptionKey; }
            set
            {
                encryptionKey = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public void Save()
        {
            using (var stream = File.Create(Path))
            {
                Serializer.Serialize(stream, this);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}