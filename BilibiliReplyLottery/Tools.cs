using BilibiliReplyLottery.Class;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BilibiliReplyLottery
{

    public static class Tools
    {
        public static int count = 0;
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        public static int GetPageNum(string avNum)
        {
            int pageNum = 0;
            HttpWebRequest req = WebRequest.Create("https://api.bilibili.com/x/v2/reply?jsonp=jsonp&pn=" +
    0 + "&type=1&oid=" + avNum + "&sort=0") as HttpWebRequest;
            req.Method = "GET";
            req.UserAgent = DefaultUserAgent;
            Stream stream = req.GetResponse().GetResponseStream();
            string json;
            using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
            {
                json = reader.ReadToEnd();
            }

            var jObject = JObject.Parse(json);
            int tmp;
            try
            {
                tmp = int.Parse(jObject["data"]["page"]["count"].ToString());
            }
            catch
            {
                return 0;
            }

            pageNum = tmp / 20;
            if (pageNum % 20 != 0)
            {
                pageNum++;
            }
            return pageNum;
        }

        public static List<ReplyInfo> GetReplyInfosFromJson(JToken json)
        {
            List<ReplyInfo> list = new List<ReplyInfo>();
            if (json == null)
            {
                return list;
            }
            try
            {
                foreach (JToken jToken in json)
                {
                    ReplyInfo tmp = new ReplyInfo(jToken["mid"].ToString(),
                        int.Parse(jToken["floor"].ToString()),
                        jToken["member"]["uname"].ToString(),
                        jToken["member"]["sex"].ToString(),
                        jToken["member"]["sign"].ToString(),
                        jToken["content"]["message"].ToString());
                    list.Add(tmp);
                }
            }
            catch
            {
                return list;
            }

            return list;
        }

        public static List<ReplyInfo> GetAllReplyInfoFromBilibili(string avNum)
        {
            List<ReplyInfo> list = new List<ReplyInfo>();
            int maxPage = 1;
            for (int count = 1; count <= maxPage; count++)
            {
                HttpWebRequest req = WebRequest.Create("https://api.bilibili.com/x/v2/reply?jsonp=jsonp&pn=" +
    count + "&type=1&oid=" + avNum + "&sort=0") as HttpWebRequest;
                req.Method = "GET";
                req.UserAgent = DefaultUserAgent;
                Stream stream = req.GetResponse().GetResponseStream();
                string json;
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                {
                    json = reader.ReadToEnd();
                }

                var jObject = JObject.Parse(json);
                int tmp;
                try
                {
                    if (jObject["data"]["replies"] == null)
                    {
                        break;
                    }
                    list.AddRange(Tools.GetReplyInfosFromJson(jObject["data"]["replies"]));
                    tmp = int.Parse(jObject["data"]["page"]["count"].ToString());
                }
                catch
                {
                    return list;
                }
                
                maxPage = tmp / 20;
                if (maxPage % 20 != 0)
                {
                    maxPage++;
                }
            }
            return list;
        }

        public static List<ReplyInfo> Lottery(List<ReplyInfo> list,int lotteryNum)
        {
            Random rd = new Random();
            List<ReplyInfo> lList = new List<ReplyInfo>();
            if (lotteryNum > list.Count)
            {
                lotteryNum = list.Count;
            }
            int maxNum = list.Count - 1;
            for(int count = 0; count < lotteryNum; count++)
            {
                int luckyNum = rd.Next(0, maxNum);
                lList.Add(list[luckyNum]);
                list.Remove(list[luckyNum]);
                maxNum = list.Count -1;
            }
            return lList;
        }

        /// <summary>
        /// 弹窗并跳到指定Url
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="Url"></param>
        public static void AlertAndRedirect(string msg, string Url)
        {
            string js = "<script language=javascript>alert('{0}');window.location.replace('{1}')</script>";
            HttpContext.Current.Response.Write(string.Format(js, msg, Url));
            HttpContext.Current.Response.End();
        }
    }
}