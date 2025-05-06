using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WASender
{
    public static class EmailMiner
    {
        public static string GetEmail(string Url, string[] ContactPageUrls)
        {
            string result = "";
            int num = 5000;
            bool cancel = false;
            Timer timer = new Timer();
            timer.Interval = num;
            timer.Elapsed += delegate
            {
                cancel = true;
            };
            timer.Start();
            if (Url == null || Url == "")
            {
                return result;
            }
            Url = Url.Replace("https:", "http:");
            string hTML = HTTPScraper.ClearString(HTTPScraper.GetPage(Url, null));
            List<string[]> list = HTTPScraper.ParseHTML(hTML, "(mailto\\:|)([\\w\\.\\-]+)@((([\\-\\w]+\\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\\.){3}[0-9]{1,3}))");
            if (list.Count > 0)
            {
                return FindCorrectEmail(list).Replace("mailto:", "");
            }
            string text = "";
            list = HTTPScraper.ParseHTML(hTML, "href=(\"|'|)(.*?)(\"|'|)[>|\\s]");
            foreach (string[] item in list)
            {
                foreach (string value in ContactPageUrls)
                {
                    if (cancel)
                    {
                        break;
                    }
                    if (item[2].IndexOf(value, StringComparison.InvariantCultureIgnoreCase) < 0)
                    {
                        continue;
                    }
                    text = item[2];
                    if (text.IndexOf("http") == -1)
                    {
                        text = ((text[0] != '/') ? ("http://" + Url.TrimEnd('/').Replace("http://", "") + "/" + text) : ("http://" + Url.TrimEnd('/') + text));
                    }
                    else
                    {
                        string text2 = Url.Replace("http://", "").Replace("https://", "").Replace("www.", "");
                        text2 = text2.Split('/')[0];
                        if (text.IndexOf(text2) == -1)
                        {
                            text = "";
                        }
                    }
                    if (text != "")
                    {
                        hTML = HTTPScraper.ClearString(HTTPScraper.GetPage(text, null));
                        list = HTTPScraper.ParseHTML(hTML, "(mailto\\:|)([\\w\\.\\-]+)@((([\\-\\w]+\\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\\.){3}[0-9]{1,3}))");
                        if (list.Count > 0)
                        {
                            result = FindCorrectEmail(list).Replace("mailto:", "");
                        }
                    }
                }
            }
            return result;
        }

        private static string FindCorrectEmail(List<string[]> Items)
        {
            foreach (string[] Item in Items)
            {
                if (Item[0].IndexOf("@mail.com", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf("@email.com", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf("example", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf(".jpg", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf(".gif", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf("esempio", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf("javascript", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf(".png", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf(".io", StringComparison.InvariantCultureIgnoreCase) == -1 && Item[0].IndexOf(".tri", StringComparison.InvariantCultureIgnoreCase) == -1)
                {
                    return Item[0].Replace("mailto:", "");
                }
            }
            return "";
        }


    }

}
