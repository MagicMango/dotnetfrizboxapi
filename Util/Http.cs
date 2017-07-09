using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace FritzBoxAPI.Util
{
    public static class Http
    {
        public static String Post(string uri, NameValueCollection pairs)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, pairs);
            }
            return Encoding.UTF8.GetString(response);
        }

        public static string ReadSite(string url, string paramstring)
        {
            Uri uri = new Uri(url + paramstring);
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string str = reader.ReadToEnd();
            return str;
        }
    }
}
