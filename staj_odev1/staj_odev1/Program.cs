using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.AccessControl;

namespace staj_odev1
{
    class Program
    {
        static void Main()
        {
            string dosyaadi = "Albil_staj.txt";
            string yazi = "Bu benim ilk staj odevim.";

            File.WriteAllText(dosyaadi, yazi);

            string hash = SHA256Hash(dosyaadi);

            string dosyaadi2 = "Albil_staj_hash.txt";
            File.WriteAllText(dosyaadi2, hash);

            EncryptFile(dosyaadi2);

            string yaz_deg = "Bu da degistirilmis yazi.";
            File.WriteAllText(dosyaadi, yaz_deg);

            string hash2 = SHA256Hash(dosyaadi);

            string dosyaadi3 = "Albil_staj_hash_2.txt";
            File.WriteAllText(dosyaadi3, hash2);

            DecryptFile(dosyaadi2);

            string veri1 = File.ReadAllText(dosyaadi2);
            string veri2 = File.ReadAllText(dosyaadi3); 

        if(string.Equals(veri1, veri2))
            {
                Console.WriteLine("Dosya değişmemiş");
            }
            else {
                Console.WriteLine("Dosyanız değiştirilmiştir.");
         }    


        }

        public static string SHA256Hash(string dosya_adi)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] dosyaBytes = File.ReadAllBytes(dosya_adi);
                byte[] hashBytes = sha256.ComputeHash(dosyaBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
        static void EncryptFile(string dosya_adi)
        {
            byte[] key;
            byte[] iv;

            using (Aes aes = Aes.Create())
            {
                key = aes.Key;
                iv = aes.IV;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] inputData = File.ReadAllBytes(dosya_adi);
                    byte[] encryptedData = encryptor.TransformFinalBlock(inputData, 0, inputData.Length);

                    using (FileStream encryptedFile = File.Create(dosya_adi))
                    {
                        encryptedFile.Write(encryptedData, 0, encryptedData.Length);
                    }
                }
            }
            string keyFilePath = Path.ChangeExtension(dosya_adi, ".key");
            File.WriteAllBytes(keyFilePath, key);

            string ivFilePath = Path.ChangeExtension(dosya_adi, ".iv");
            File.WriteAllBytes(ivFilePath, iv);
        }
        static void DecryptFile(string dosya_adi)
        {
            string keyFilePath = Path.ChangeExtension(dosya_adi, ".key");
            string ivFilePath = Path.ChangeExtension(dosya_adi, ".iv");

            byte[] key = File.ReadAllBytes(keyFilePath);
            byte[] iv = File.ReadAllBytes(ivFilePath);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] encryptedData = File.ReadAllBytes(dosya_adi);
                    byte[] decryptedData = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                    string decryptedText = Encoding.UTF8.GetString(decryptedData);

                    File.WriteAllText(dosya_adi, decryptedText);
                }
            }
        }

    }
}
