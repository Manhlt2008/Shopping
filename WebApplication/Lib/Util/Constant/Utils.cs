using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using log4net;
using WebApplication.Lib.Dal.DbContext;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace WebApplication.Lib.Util.Common
{
    public class Utils
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            Console.WriteLine(Utils.HashMD5("123456"));
        }

        public static string HashMD5(string InputText)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            if (String.IsNullOrEmpty(InputText.Trim()))
                return "";
            byte[] arrInput = null;
            arrInput = UnicodeEncoding.UTF8.GetBytes(InputText);
            byte[] arrOutput = null;
            arrOutput = MD5.ComputeHash(arrInput);
            return Convert.ToBase64String(arrOutput);
        }

        public static bool IsValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        public static string createToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());

            return token;
        }

        public static string createLinkForgotPassword(string email, string token)
        {
            string path = "Http://" + HttpContext.Current.Request.Url.Authority;
            path = path + "/Authentication/ResetPassword?token=" + HttpContext.Current.Server.UrlEncode(token) + "&email=" + email;

            return path;
        }

        public static string GenerateCode()
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var code = dbContext.Database.SqlQuery<string>(
                       "SELECT UPPER(SUBSTRING(CONVERT(varchar(40), NEWID()),0,9))").FirstOrDefault();

                    return code ?? string.Empty;
                }
            }
            catch (Exception exception)
            {
                Log.Error("GenerateCode()", exception);
            }

            return string.Empty;
        }

        public static string converseDMYToYMD(string date)
        {

            date = Regex.Replace(date, @"[^\u0000-\u007F]", string.Empty);

            string inputFormat = "dd-MM-yyyy HH:mm:ss";
            string outputFormat = "yyyy-MM-dd HH:mm:ss";
            var dateTime = DateTime.ParseExact(date, inputFormat, CultureInfo.InvariantCulture);
            string output = dateTime.ToString(outputFormat);

            return output;
        }
        public static string SendHttpPost(NameValueCollection values, string urlpost)
        {
            using (var client = new WebClient())
            {
                var response = client.UploadValues(urlpost, values);
                var responseString = Encoding.UTF8.GetString(response);
                return responseString;
            }
        }

      

        public static string Hmacsha1Encode(string input, string secretKey = null)
        {
            var myhmacsha1 = new HMACSHA1();
            if (secretKey != null)
            {
                var key = Encoding.ASCII.GetBytes(secretKey);
                myhmacsha1 = new HMACSHA1(key);
            }
            var byteArray = Encoding.ASCII.GetBytes(input);
            var stream = new MemoryStream(byteArray);

            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + "{e:x2}", s => s);
        }

        public static string Sha1(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string GetIpAddress()
        {
            string ipAddressString = HttpContext.Current.Request.UserHostAddress;

            if (ipAddressString == null)
                return null;

            IPAddress ipAddress;
            IPAddress.TryParse(ipAddressString, out ipAddress);

            // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
            // This usually only happens when the browser is on the same machine as the server.
            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                ipAddress = System.Net.Dns.GetHostEntry(ipAddress).AddressList
                    .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }

            return ipAddress.ToString();
        }

        public static string GetAuthority()
        {
            string url = System.Web.HttpContext.Current.Request.Url.Authority;

            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTPS"] == "on")
            {
                url = "https://" + url;
            }
            else
            {
                url = "http://" + url;
            }

            return url;
        }

        public static string GetUrlPath(string actionName, string controlerName)
        {
            var requestContext = HttpContext.Current.Request.RequestContext;
            return GetAuthority()+new UrlHelper(requestContext).Action(actionName, controlerName);
        }

        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        private static byte[] HexDecode(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }

        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static string HashHmacHex(string keyHex, string message)
        {
            byte[] hash = HashHMAC(HexDecode(keyHex), StringEncode(message));
            return HashEncode(hash);
        }
    }
}