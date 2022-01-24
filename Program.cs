using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace readLetterFromWebsite
{
    class Program
    {
        //1. Create a variable for the url of web scrapped and the alphabet for count the letter
        static Uri url = new Uri("https://klasika.kompas.id/baca/inspiraksi-kemilau-perayaan-hut-ke-50-kompas/");
        static string collection = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static async Task Main(string[] args)
        {
            //2. Get data from the url with http get Method
            //3. Initialize HtmlDocument object to process the html response 
            //4. Set up pattern recognitions to only get the article and Clean the data that scraped.
            string data = (await getData()).ToUpper();
            int endIndex = 0;
            if ((endIndex = data.IndexOf("TAGS :")) != -1)
            {
                data = data.Substring(0, endIndex);
            }
            int dataLength = data.Length;
            int result;
            //5. Loop the alphabet to count each letter using foreach
            foreach (var item in collection)
            {
                result = 0;
                //6. Finally, the number of letters = the number of data - the number of data that has been modified by replacing the searched letter with an empty string
                result = dataLength - data.Replace(item.ToString(), "").Length;
                Console.WriteLine($"{item.ToString()} = {result}");
            }
            Console.ReadLine();
        }

        static async Task<string> getData()
        {
            string result = string.Empty;
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    using (Stream articleContent = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        HtmlDocument htmlDoc = new HtmlDocument();
                        htmlDoc.Load(articleContent, Encoding.ASCII);
                        var rawData = htmlDoc.DocumentNode.SelectNodes("//div[contains(@itemprop,'articleBody')]");
                        if (rawData != null)
                        {
                            foreach (HtmlNode div in rawData)
                            {
                                result = Regex.Replace(div.InnerText, @"\s+", " ").Replace("&nbsp;", "", StringComparison.OrdinalIgnoreCase);
                            }
                        }

                    }
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
            }
            return result;
        }
    }
}
