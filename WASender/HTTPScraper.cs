using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WASender
{

    public static class HTTPScraper
    {
        public static List<string[]> ParseHTML(string HTML, string Template)
        {
            List<string[]> strArrays = new List<string[]>();
            Match i = Regex.Match(HTML, Template);
            while (i.Success)
            {
                string[] value = new string[i.Groups.Count];
                for (int j = 0; j < i.Groups.Count; j++)
                {
                    value[j] = i.Groups[j].Value;
                }
                strArrays.Add(value);
                i = i.NextMatch();
            }
            return strArrays;
        }

        public static string ClearString(string Source)
        {
            Source = Source.Replace("   ", " ");
            char[] charArray = Source.ToCharArray();
            char[] chrArray = new char[3] { '\n', '\r', '\t' };
            for (int i = 0; i < Source.Length - 1; i++)
            {
                if (Source[i] == ' ' && Source[i + 1] == ' ')
                {
                    charArray[i] = '*';
                    charArray[i + 1] = '*';
                }
                for (int j = 0; j < chrArray.Length; j++)
                {
                    if (charArray[i] == chrArray[j])
                    {
                        charArray[i] = '*';
                    }
                }
            }
            return new string(charArray).Replace("*", "");
        }


        public static string GetPage(string Url, string PostData)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 7000;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
                httpWebRequest.MaximumAutomaticRedirections = 15;
                httpWebRequest.AllowAutoRedirect = true;
               
                httpWebRequest.Timeout = 7000;
                httpWebRequest.Method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(PostData);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = bytes.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse obj = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = obj.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string result = streamReader.ReadToEnd();
                streamReader.Close();
                streamReader.Dispose();
                responseStream.Close();
                responseStream.Dispose();
                obj.Close();
                return result;
            }
            catch
            {
                return "";
            }
        }

        public static string GetPage(string Url)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 7000;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";
                httpWebRequest.KeepAlive = false;
                httpWebRequest.MaximumAutomaticRedirections = 15;
                httpWebRequest.AllowAutoRedirect = true;

                httpWebRequest.Timeout = 7000;
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string end = streamReader.ReadToEnd();
                streamReader.Close();
                streamReader.Dispose();
                responseStream.Close();
                responseStream.Dispose();
                response.Close();
                return end;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }
    }
}
