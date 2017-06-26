using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication.Lib.Util
{
    public class StringUtil
    {
        public static string Generate()
        {
            var path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        private static readonly string[] VietnameseSigns =
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }

            return str;

        }

        public static string ReplaceCharSet(string input)
        {
            var charSet = input.ToLower();
            switch (charSet)
            {
                case "a":
                    return "[aàảãáạăằẳẵắặâầẩẫấậ]";
                case "e":
                    return "[eèẻẽéẹêềểễếệ]";
                case "i":
                    return "[iìỉĩíị]";
                case "o":
                    return "[oòỏõóọôồổỗốộơờởỡớợ]";
                case "u":
                    return "[uùủũúụưừửữứự]";
                case "y":
                    return "[yỳỷỹýỵ]";
                case "d":
                    return "[dđ]";
            }
            return charSet;
        }

        public static string RemoveVietnameseTone(string input)
        {
            return input.Aggregate("", (current, c) => current + ReplaceCharSet(c.ToString(CultureInfo.InvariantCulture)));
        }

        public static DateTime ParseVnDate(string date, char p)
        {
            try
            {
                var strs = date.Split(p);
                return new DateTime(Int32.Parse(strs[2]), Int32.Parse(strs[1]), Int32.Parse(strs[0]));
            }
            catch
            {
                try
                {
                    return DateTime.Parse(date);
                }
                catch (Exception)
                {
                    return DateTime.MinValue;
                }
            }
        }

        public static TimeSpan ParseTime(string time, char p)
        {
            try
            {
                var strs = time.Split(p);
                if (strs.Length == 3)
                {
                    return new TimeSpan(Int32.Parse(strs[0]), Int32.Parse(strs[1]), Int32.Parse(strs[2]));
                }
                else
                {
                    return new TimeSpan(Int32.Parse(strs[0]), Int32.Parse(strs[1]), 0);
                }
            }
            catch (Exception)
            {
                return TimeSpan.MinValue;
            }
        }

        public static string CalculateMd5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            foreach (byte t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string ToUrlAddress(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }
            return url;
        }

        public static string GetMd5Password(string password, string salt)
        {
            using (var md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, password + salt);
            }
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}