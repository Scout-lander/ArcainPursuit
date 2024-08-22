using System;
using System.Security.Cryptography;

public class KeyIVGenerator
{
    public static void GenerateKeyAndIV()
    {
        using (Aes aes = Aes.Create())
        {
            aes.GenerateKey();
            aes.GenerateIV();

            string key = Convert.ToBase64String(aes.Key);
            string iv = Convert.ToBase64String(aes.IV);

            Console.WriteLine("Key: " + key);
            Console.WriteLine("IV: " + iv);
        }
    }

    static void Main(string[] args)
    {
        GenerateKeyAndIV();
    }
}
