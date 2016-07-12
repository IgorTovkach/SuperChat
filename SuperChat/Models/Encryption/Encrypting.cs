using System.Security.Cryptography;

namespace SuperChat.Models.Encryption
{
    public static class Encrypting
    {
        public static void XorEncrypt(ref byte[] message, byte[] key)
        {
            for (var i = 0; i < message.Length; i++)
            {
                message[i] = (byte) (message[i] ^ key[i%key.Length]);
            }
        }

        public static byte[] ComputeMd5Hash(byte[] array)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(array);
        }
    }
}