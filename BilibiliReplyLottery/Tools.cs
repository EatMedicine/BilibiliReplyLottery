using BilibiliReplyLottery.Class;
using BilibiliReplyLottery.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace BilibiliReplyLottery
{

    public static class Tools
    {
        public static int count;
        public static int MaxPageNum;

        public static void UpdateMaxPageNum()
        {
            string str = ConfigurationManager.AppSettings["MaxPageNum"];
            int num = 50;
            if(int.TryParse(str,out num) == true)
            {
                MaxPageNum = num;
            }

        }

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
                Thread.Sleep(1);
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

        public static bool isMidInList(List<ReplyInfo> list, string mid)
        {
            foreach (ReplyInfo reply in list)
            {
                if (reply.mid == mid)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<ReplyInfo> SelectUserInReply(List<ReplyInfo> list)
        {
            List<ReplyInfo> newList = new List<ReplyInfo>();
            foreach (ReplyInfo reply in list)
            {
                if (isMidInList(newList, reply.mid) == false)
                {
                    newList.Add(reply);
                }
            }
            return newList;
        }

        public static void UpdateResult(object source, System.Timers.ElapsedEventArgs e)
        {
            using(LotteryDataEntities db = new LotteryDataEntities())
            {
                LotteryReady[] list = db.LotteryReady.Where(r => DateTime.Compare((DateTime)r.LotteryTime, DateTime.Now) < 0 && r.IsExecuted == 0).ToArray();
                foreach(LotteryReady item in list)
                {
                    DBHelper.ChangeStateReady(item.LotteryId, true);
                    List<ReplyInfo> rlist = Tools.GetAllReplyInfoFromBilibili(item.AvNum);
                    if (rlist.Count == 0)
                    {
                        DBHelper.ChangeStateReady(item.LotteryId, true);
                        return;
                    }
                    if(item.IsFilter == 1)
                    {
                        rlist = Tools.SelectUserInReply(rlist);
                    }
                    List<ReplyInfo> lucky = Tools.Lottery(rlist, (int)item.LotteryNum);
                    foreach(ReplyInfo user in lucky)
                    {
                        LotteryResult result = new LotteryResult
                        {
                            LotteryId = item.LotteryId,
                            Name = user.uname,
                            mid = user.mid,
                            LotteryFloor = user.floor,
                            Msg = user.msg,
                            LotteryTime = DateTime.Now
                            
                        };
                        DBHelper.AddResult(result);
                    }
                }
            }
        }

        //创建盐
        public static string salt = "BilibiliLottery";

        /// <summary>
        /// 生成凭证
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string makeCertification(string userId)
        {
            return encrypt(userId + salt);
        }

        /// <summary>
        /// 验证凭证
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="certification"></param>
        /// <returns></returns>
        public static bool verifyCertification(string userId, string certification)
        {
            string encryptCertification = encrypt(userId + salt);
            if (certification == encryptCertification)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断cookie是否为无效 true为有效 false为无效
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns>true为有效 false为无效</returns>
        public static bool IsCookieEmpty(HttpCookie cookie)
        {
            if (cookie == null)
                return false;
            if (cookie.Values.Count == 0)
                return false;
            if (cookie.Value == "")
                return false;
            return true;
        }

        public static string encrypt(string userPwd)
        {
            //MD5加密
            byte[] password = Encoding.UTF8.GetBytes(userPwd);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] pwOutput = md5.ComputeHash(password);
            string pwd = BitConverter.ToString(pwOutput).Replace("-", "");
            return pwd;
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