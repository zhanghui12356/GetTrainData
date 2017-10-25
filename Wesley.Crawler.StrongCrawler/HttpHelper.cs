
using System.IO;
using Newtonsoft.Json;
using System.Net;



namespace Wesley.Crawler.StrongCrawler
{
    public class HttpHelper
    {

        public static T GetDataByUrl<T>(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (System.Exception)
            {

                return default(T);
            }
           

        }
    }
}
