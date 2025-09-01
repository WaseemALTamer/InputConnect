using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using InputConnect.Structures;

namespace InputConnect.Network
{
    public static class Encryptor{
        private const int KeySize = 32;
        private const int IvSize = 16;
        private const int SaltSize = 16;
        private const int HmacSize = 32;
        private const int Iterations = 100000;



        public static string Encrypt(string plainText, PasswordKey? pk)
        {
            if (pk == null || pk.Token == null) return plainText;

            // Only generate salt/key once
            if (pk.Salt == null || pk.Key == null)
            {
                pk.Salt = pk.Salt ?? RandomNumberGenerator.GetBytes(SaltSize);
                pk.Key = pk.Key ?? DeriveKey(pk.Token, pk.Salt);
            }

            byte[] iv = RandomNumberGenerator.GetBytes(IvSize);
            byte[] cipherBytes;

            using (var aes = Aes.Create())
            {
                aes.Key = pk.Key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor();
                using var ms = new MemoryStream();
                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cryptoStream, Encoding.UTF8))
                {
                    sw.Write(plainText);
                }
                cipherBytes = ms.ToArray();
            }

            byte[] hmac = ComputeHmac(pk.Key, pk.Salt, iv, cipherBytes);

            using var finalStream = new MemoryStream();
            finalStream.Write(pk.Salt);
            finalStream.Write(iv);
            finalStream.Write(cipherBytes);
            finalStream.Write(hmac);

            return Convert.ToBase64String(finalStream.ToArray());
        }

        public static string? Decrypt(string encryptedBase64, PasswordKey? passwordKey)
        {
            if (passwordKey == null || passwordKey.Token == null) return null;

            try
            {
                byte[] encryptedData = Convert.FromBase64String(encryptedBase64);

                byte[] salt = encryptedData[..SaltSize];
                byte[] iv = encryptedData[SaltSize..(SaltSize + IvSize)];
                int cipherLength = encryptedData.Length - SaltSize - IvSize - HmacSize;
                byte[] cipherBytes = encryptedData[(SaltSize + IvSize)..(SaltSize + IvSize + cipherLength)];
                byte[] expectedHmac = encryptedData[^HmacSize..];

                byte[] key;
                if (passwordKey.Key != null && passwordKey.Salt != null && salt.SequenceEqual(passwordKey.Salt) ){
                    key = passwordKey.Key;
                }
                else{
                    key = DeriveKey(passwordKey.Token, salt);
                    passwordKey.Salt = salt;
                    passwordKey.Key = key;
                }

                // Verify HMAC
                byte[] actualHmac = ComputeHmac(key, salt, iv, cipherBytes);
                if (!CryptographicOperations.FixedTimeEquals(expectedHmac, actualHmac))
                    return null;

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(cipherBytes);
                using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cryptoStream, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }

        private static byte[] DeriveKey(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(KeySize);
        }

        private static byte[] ComputeHmac(byte[] key, byte[] salt, byte[] iv, byte[] cipherText)
        {
            using var hmac = new HMACSHA256(key);
            using var ms = new MemoryStream();
            ms.Write(salt);
            ms.Write(iv);
            ms.Write(cipherText);
            return hmac.ComputeHash(ms.ToArray());
        }
    }
}
