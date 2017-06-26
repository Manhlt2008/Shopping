using System;
using System.Security.Cryptography;

namespace WebApplication.Lib.Util.Security
{
    public class HashingUtils
    {
        public static string CreateSha256Token(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            var keyByte = encoding.GetBytes(secret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                var hashmessage = hmacsha256.ComputeHash(messageBytes);
                var str = Convert.ToBase64String(hashmessage);
                var bytes = Convert.FromBase64String(str);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower(); ;
            }
        }

        public static string CreateSha1Token(string message)
        {
            var encoding = new System.Text.ASCIIEncoding();
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA1())
            {
                var hashmessage = hmacsha256.ComputeHash(messageBytes);
                var str = Convert.ToBase64String(hashmessage);
                var bytes = Convert.FromBase64String(str);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower(); ;
            }
        }
    }
}