using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace EternalBlue.Ifs
{
    public class Encryptor : IEncryptor
    {
        public string Encrypt(string text)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string Decrypt(string text)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
