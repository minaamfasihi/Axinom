using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Axinom.DataManagement.Services.Decryption {
    public class AESDecryptor : IDecryptor {
        private readonly string _key;
        private readonly string _iv;
        public AESDecryptor(string key, string iv) { 
            _key = key;
            _iv = iv;
        }
        public string Decrypt(byte[] buffer) {
            string plaintext = null;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Convert.FromBase64String(_key);
                aesAlg.IV = Convert.FromBase64String(_iv);
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // var buffer = Convert.FromBase64String(str);
                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
    
}
