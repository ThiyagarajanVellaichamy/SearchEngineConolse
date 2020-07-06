using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text.RegularExpressions;

namespace SearchEngineConolse
{

    public struct LinkItem
    {
        public string Href;
        public string Text;

        public override string ToString()
        {
            return Href + "\n\t" + Text;
        }
    }

    static class Extension
    {
        public static void Find(this string file, Action<LinkItem> data)
        {

            MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
                RegexOptions.Singleline);

            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                LinkItem item = new LinkItem();

                Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                    RegexOptions.Singleline);
                if (m2.Success)
                {
                    item.Href = m2.Groups[1].Value;

                    if (!item.Href.StartsWith("http") && item.Href.Contains("http"))
                    {
                        item.Href = item.Href.Remove(0, item.Href.IndexOf("http") - 1);
                    }
                }

                item.Text = Regex.Replace(value, @"\s*<.*?>\s*", "", RegexOptions.Singleline);

                if (item.Href.Contains("http"))
                {
                    data(item);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Start:
            Console.WriteLine("Please choose option");
            Console.WriteLine("1. Google Search");
            Console.WriteLine("2. Bing Search");
            string input = Console.ReadLine();

            
            string uriString = string.Empty;
            switch (input)
            {
                case "1":
                    uriString = "https://www.google.com/search";
                    break;
                case "2":
                    uriString = "https://www.bing.com/search";
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    goto Start;
            }

            Console.WriteLine("Please enter your search");
            string keywordString = Console.ReadLine();

            WebClient webClient = new WebClient();
            NameValueCollection nameValueCollection = new NameValueCollection();
            nameValueCollection.Add("q", keywordString);

            webClient.QueryString.Add(nameValueCollection);
            string s = webClient.DownloadString(uriString);
            webClient.Dispose();


            s.Find(i =>
            {
                Console.WriteLine($"Title: {i.Text}, Link:{i.Href}");
            });

            
            Console.ReadKey();
        }
        

    }
 }
