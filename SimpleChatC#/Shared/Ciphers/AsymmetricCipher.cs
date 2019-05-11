using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Ciphers
{
    public class AsymmetricCipher
    {
        private RSACryptoServiceProvider cipher;

        public AsymmetricCipher()
        {
            cipher = new RSACryptoServiceProvider();
        }

        public string Encrypt(string message)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(message);
            string encryptedMessage = Convert.ToBase64String(cipher.Encrypt(bytes,true));
            return encryptedMessage;
        }

        public string Decrypt(string message)
        {
            byte[] bytes = Convert.FromBase64String(message);
            string decryptedMessage = Encoding.Unicode.GetString(cipher.Decrypt(bytes, true));
            return decryptedMessage;
        }

        public string PublicKey()
        {
            return cipher.ToXmlString(false);
        }

        public void LoadPublicKey(string publicKey)
        {
            cipher.FromXmlString(publicKey);
        }
    }
}