using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace Hineini.Encryption {
    public static class Encryptor
    {
        public static string Password { 
            get {
                string appString = Assembly.GetExecutingAssembly().GetName().FullName;
                return UniqueDeviceId.GetDeviceID(appString);
            }
        }

        public static byte[] EncryptedData { get; set; }

        public static byte[] Encrypt(string message, string password)
        {
            if (Utility.Helpers.StringHasValue(password))
            {
                // RijndaelManages is the prefered symmetric algorithm, since it has the strongest encryption and it is completely managed code:
                SymmetricAlgorithm sa = new RijndaelManaged();

                // apply key and IV to the algorithm, depending on password:
                PasswordToKey(password, sa);

                byte[] binary = Encoding.Unicode.GetBytes(message);
                MemoryStream ms = new MemoryStream();
                CryptoStream stream = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write);
                stream.Write(binary, 0, binary.Length);
                stream.FlushFinalBlock();
                stream.Close();
                byte[] encrypted = ms.ToArray();
                EncryptedData = encrypted;
                return encrypted;
            }
            else
            {
                return Encoding.Default.GetBytes(message);
            }

        }

        /// <summary>
        /// Decrypts a byte array to a string, using symmetric encryption.
        /// </summary>
        /// <param name="encrypted">The encrypted data as byte array.</param>
        /// <param name="password">The password for decryption.</param>
        /// <returns>A string with the decrypted message.</returns>
        public static string Decrypt(byte[] encrypted, string password)
        {
            if (Utility.Helpers.StringHasValue(password))
            {
                // RijndaelManages is the prefered symmetric algorithm, since it has the strongest encryption and it is completely managed code:
                SymmetricAlgorithm sa = new RijndaelManaged();

                // apply key and IV to the algorithm, depending on password:
                PasswordToKey(password, sa);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream stream = new CryptoStream(ms, sa.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        stream.Write(encrypted, 0, encrypted.Length);
                        stream.Flush();
                        stream.Close();
                    }
                    byte[] decrypted = ms.ToArray();
                    string s = Encoding.Unicode.GetString(decrypted, 0, decrypted.Length);
                    return s;
                }
            }
            else
            {
                string s = Encoding.Default.GetString(encrypted, 0, encrypted.Length);
                int idx = s.IndexOf("<?xml");
                if (idx > 0) s = s.Substring(idx);
                return s;
            }
        }

        /// <summary>
        /// Generates key and IV from the specified password, using an MD5 hash.
        /// </summary>
        /// <remarks>
        /// Usually you would prefer to use Rfc2898DeriveBytes for this purpose but since 
        /// .net compact framework (for windows mobile) does not have any implementation
        /// for Rcf2898DeriveBytes or even PasswordDeriveBytes, this is a simple workaround
        /// for it.
        /// </remarks>
        /// <param name="password">The password to generate a key/ITfrom it.</param>
        /// <param name="sa">The SymmetricAlgorithm class for which to create a key/IV pair.</param>
        private static void PasswordToKey(string password, SymmetricAlgorithm sa)
        {
            HashAlgorithm hashAlgo = new MD5CryptoServiceProvider();
            byte[] hash = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(password));
            sa.BlockSize = hash.Length * 8;
            sa.Key = hash;
            sa.IV = hash;
        }


    }
}