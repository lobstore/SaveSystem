using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionTool
{
    private static readonly byte[] encryptionKey = Encoding.UTF8.GetBytes("uFsxJjZv7YscM2VX8gPbTg=="); // Должно быть 16, 24 или 32 байта
    private static readonly byte[] iv = new byte[16]; // Должно быть 16 байт для AES

    public static string DecryptData(string encryptedData)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

        using (var aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.IV = iv;

            using (var memoryStream = new MemoryStream(encryptedBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
            using (var streamReader = new StreamReader(cryptoStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
    public static string EncryptData(string dataToStore)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = encryptionKey;
            aes.IV = iv;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(dataToStore);
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }

}