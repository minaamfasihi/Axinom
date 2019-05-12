using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Axinom.ControlPanel.Services.Encryption {
    public class AESEncryptor : IEncryptor {
        private readonly string _key;
        private readonly string _iv;
        public AESEncryptor(string key, string iv) { 
            _key = key;
            _iv = iv;
        }
        public byte[] Encrypt(string str) {
            byte[] encrypted;
            
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(_key);
                aesAlg.IV = Convert.FromBase64String(_iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(str);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }
    }
}