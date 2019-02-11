using BilibiliReplyLottery.Class;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BilibiliReplyLottery.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Lottery()
        {
            ViewBag.Title = "抽奖首页 -- Bilibili评论抽奖";
            return View();
        }

        [HttpPost]
        public ActionResult Result(int avNum,int lotteryNum)
        {
            if (Tools.GetPageNum(avNum.ToString()) > 50)
            {
                Tools.AlertAndRedirect("视频评论过多", Url.Action("Lottery", "Home"));
                return null;
            }
            List<ReplyInfo> list = Tools.GetAllReplyInfoFromBilibili(avNum.ToString());
            List<ReplyInfo> luckyList = Tools.Lottery(list, lotteryNum);
            ViewBag.TotalNum = list.Count;
            ViewBag.AvNum = avNum;
            ViewBag.LotteryNum = lotteryNum;
            ViewBag.LuckyList = luckyList;
            ViewBag.Title = "抽奖结果 -- Bilibili评论抽奖";
            return View();
        }
    }
}