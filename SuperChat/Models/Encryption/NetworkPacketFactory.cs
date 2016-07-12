using System;
using System.Text;
using SuperChat.Models.ConfigurationManagement;

namespace SuperChat.Models.Encryption
{
    public class NetworkPacketFactory
    {
        private readonly ChatConfigurationManager configurationManager;

        public NetworkPacketFactory(ChatConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public byte[] CreatePacket(byte[] array)
        {
            byte[] resultArray;
            if (!configurationManager.Encryption)
            {
                //For non-encrypted packets first byte must be 0
                //Next packet data
                resultArray = new byte[array.Length + 1];
                resultArray[0] = 0;
                Array.Copy(array, 0, resultArray, 1, array.Length);
                return resultArray;
            }
            //For encoded packet first byte must be 1
            //Next 4 bytes it is length of data
            //Next encoded data
            //Next non-encoded data hash sum
            var hashSum = Encrypting.ComputeMd5Hash(array);
            byte[] encodingByte = {1};
            Encrypting.XorEncrypt(ref array, Encoding.Unicode.GetBytes(configurationManager.EncryptionKey));
            resultArray = new byte[encodingByte.Length + hashSum.Length + array.Length + 4];
            var copyBytes = 0;
            Array.Copy(encodingByte, 0, resultArray, 0, encodingByte.Length);
            copyBytes += encodingByte.Length;
            Array.Copy(BitConverter.GetBytes(array.Length), 0, resultArray, copyBytes, 4);
            copyBytes += 4;
            Array.Copy(array, 0, resultArray, copyBytes, array.Length);
            copyBytes += array.Length;
            Array.Copy(hashSum, 0, resultArray, copyBytes, hashSum.Length);
            return resultArray;
        }

        public byte[] ParsePacket(byte[] array)
        {
            var passedBytes = 1;
            if (array[0] == 0 && !configurationManager.Encryption)
            {
                var resultArray = new byte[array.Length - 1];
                Array.Copy(array, 1, resultArray, 0, resultArray.Length);
                return resultArray;
            }
            if (array[0] != 1 || !configurationManager.Encryption) return null;
            var dataLength = BitConverter.ToInt32(array, passedBytes);
            passedBytes += 4;
            var data = new byte[dataLength];
            Array.Copy(array, passedBytes, data, 0, dataLength);
            Encrypting.XorEncrypt(ref data, Encoding.Unicode.GetBytes(configurationManager.EncryptionKey));
            passedBytes += dataLength;
            var hashSum = Encrypting.ComputeMd5Hash(data);
            for (var i = 0; i < array.Length - passedBytes; i++)
            {
                if (hashSum[i] != array[i + passedBytes])
                    return null;
            }
            return data;
        }
    }
}