
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Wesley.Crawler.StrongCrawler.Events;
using Wesley.Crawler.StrongCrawler.Models;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.PhantomJS;
using Newtonsoft.Json;

namespace Wesley.Crawler.StrongCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            GetHotel();
            //    GetTrain();

        }




        public static void GetTrain()
        {
            var url = "http://mobile.12306.cn/weixin/leftTicket/query?leftTicketDTO.train_date=2017-10-26&leftTicketDTO.from_station=SZH&leftTicketDTO.to_station=SHH&purpose_codes=ADULT";
            var result = HttpHelper.GetDataByUrl<Train>(url);
            Console.WriteLine(result.data[0].from_station_name);
            Console.ReadKey();
        }


        public static void GetHotel()
        {
            //进入查询首页
            var hotelUrl = "http://mobile.12306.cn/weixin/wxcore/init";
            var hotelCrawler = new StrongCrawler();
            hotelCrawler.OnStart += (s, e) =>
            {
                Console.WriteLine("爬虫开始抓取地址：" + e.Uri.ToString());
            };
            hotelCrawler.OnError += (s, e) =>
            {
                Console.WriteLine("爬虫抓取出现错误：" + e.Uri.ToString() + "，异常消息：" + e.Exception.ToString());
            };
            hotelCrawler.OnCompleted += (s, e) =>
            {
                HotelCrawler(e);
            };
            var operation = new Operation
            {
                Action = (x) =>
                {
                    //通过Selenium驱动点击页面的“酒店评论”
                    x.FindElement(By.XPath("//*[@id='J_depart_name']")).Click();
                },
                Condition = (x) =>
                {
                    //判断Ajax评论内容是否已经加载成功
                    return x.FindElement(By.XPath("//*[@id='his_citybox']")).Displayed;
                },
                Timeout = 5000
            };

            hotelCrawler.Start(new Uri(hotelUrl), null, operation);//不操作JS先将参数设置为NULL

            Console.ReadKey();
        }


        private static void HotelCrawler(OnCompletedEventArgs e)
        {



            var StationsInfos = e.WebDriver.FindElement(By.XPath("//*[@id='all_citybox']"));
            var stationList = StationsInfos.FindElements(By.XPath("li[@class='station-item']"));
            //var totalPage = Convert.ToInt32(comments.FindElement(By.XPath("div[@class='c_page_box']/div[@class='c_page']/div[contains(@class,'c_page_list')]/a[last()]")).Text);
            TrainStation temp;
            List<TrainStation> list = new List<TrainStation>();
            foreach (var item in stationList)
            {
                temp = new TrainStation();
                temp.text =   item.GetAttribute("data-text");
                temp.code =  item.GetAttribute("data-code");
                list.Add(temp);
            }

            Console.WriteLine(JsonConvert.SerializeObject(list));            
            Console.ReadKey();




        }
    }
}
