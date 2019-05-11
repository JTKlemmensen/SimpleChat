using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Ciphers
{
    public class SymmetricCipher
    {
        private RijndaelManaged cipher;
        public string Key
        {
            get => Convert.ToBase64String(cipher.Key);
            set => cipher.Key = Convert.FromBase64String(value);
        }

        public string IV
        {
            get => Convert.ToBase64String(cipher.IV);
            set => cipher.IV = Convert.FromBase64String(value);
        }

        public SymmetricCipher()
        {
            cipher = new RijndaelManaged();
            cipher.KeySize = 128;
        }

        public string Encrypt(string message)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cipher.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(message);

                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    byte[] cipherBytes = memoryStream.ToArray();

                    string EncryptedMessage = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
                    return EncryptedMessage;
                }
            }
            catch(Exception)
            {
                return null;
            }
        }
    

        public string Decrypt(string message)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    ICryptoTransform rijndaelDecryptor = cipher.CreateDecryptor();

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelDecryptor, CryptoStreamMode.Write))
                    {
                        string decryptedMessage = "";

                        byte[] cipherBytes = Convert.FromBase64String(message);

                        cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] plainBytes = memoryStream.ToArray();

                        decryptedMessage = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
                        return decryptedMessage;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }    
        }
    }
}