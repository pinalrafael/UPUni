using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UPUni.Cryptor
{
    /// <summary>
    /// Encrypt and decript files.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Encrypt file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="key">Encrypt key. Max length 8 characters.</param>
        public static void Encrypt(string filePath, string key)
        {
            byte[] plainContent = System.IO.File.ReadAllBytes(filePath);
            using (var DES = new DESCryptoServiceProvider())
            {
                DES.IV = Encoding.ASCII.GetBytes(key);
                DES.Key = Encoding.ASCII.GetBytes(key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;


                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateEncryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(plainContent, 0, plainContent.Length);
                    cryptoStream.FlushFinalBlock();
                    System.IO.File.WriteAllBytes(filePath, memStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypt file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="key">Decrypt key. Max length 8 characters.</param>
        public static void Decrypt(string filePath, string key)
        {
            byte[] encrypted = System.IO.File.ReadAllBytes(filePath);
            using (var DES = new DESCryptoServiceProvider())
            {
                DES.IV = Encoding.ASCII.GetBytes(key);
                DES.Key = Encoding.ASCII.GetBytes(key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;


                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateDecryptor(),
                        CryptoStreamMode.Write);

                    cryptoStream.Write(encrypted, 0, encrypted.Length);
                    cryptoStream.FlushFinalBlock();
                    System.IO.File.WriteAllBytes(filePath, memStream.ToArray());
                }
            }
        }
    }
}
