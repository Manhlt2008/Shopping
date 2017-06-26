using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace WebApplication.Lib.Bll.InlamiaHttpRequest
{
    public class InlamiaHttpRequest
    {
        public static string Post(Dictionary<string, string> postDataDictionary, string url)
        {
            return Post(JsonConvert.SerializeObject(postDataDictionary), url);
        }

        public static string Post(string json, string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var data = Encoding.GetEncoding("UTF-8").GetBytes(json);

            request.Method = "POST";
            request.ContentType = HttpRequestContentType.ApplicationJson;
            request.Accept = HttpRequestContentType.ApplicationJson;
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }

        public static string RecieveHttpGet(string urlget)
        {
            using (var client = new WebClient())
            {
                var responseString = client.DownloadString(urlget);
                return responseString;
            }
        }
    }
}