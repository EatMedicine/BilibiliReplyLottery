using BilibiliReplyLottery.Class;
using BilibiliReplyLottery.Filter;
using BilibiliReplyLottery.Models;
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
        public ActionResult Hello()
        {
            ViewBag.ReadyData = DBHelper.GetLotteryReady(0);
            ViewBag.ResultData = DBHelper.GetLotteryReady(1);
            ViewBag.Title = "首页 -- Bilibili评论抽奖";
            return View();
        }

        public ActionResult Lottery()
        {
            ViewBag.Title = "即时抽奖 -- Bilibili评论抽奖";
            ViewBag.MaxPageNum = Tools.MaxPageNum;
            return View();
        }
        [LoginFilter]
        public ActionResult TimeLottery()
        {
            ViewBag.Title = "定时抽奖 -- Bilibili评论抽奖";
            ViewBag.MaxPageNum = Tools.MaxPageNum;
            return View();
        }
        [LoginFilter]
        [HttpPost]
        public ActionResult TimeLottery(int avNum,int lotteryNum,int lotteryDay,string lotteryCheck)
        {
            ViewBag.Title = "定时抽奖 -- Bilibili评论抽奖";
            if (Tools.GetPageNum(avNum.ToString()) == 0)
            {
                Tools.AlertAndRedirect("无法找到该视频", Url.Action("TimeLottery"));
                return null;
            }
            int check;
            if (lotteryCheck == null)
                check = 0;
            else
                check = 1;
            LotteryReady task = new LotteryReady
            {
                AvNum = avNum.ToString(),
                LotteryNum = lotteryNum,
                LotteryTime = DateTime.Now.AddDays(lotteryDay),
                IsFilter = check,
                IsExecuted = 0
            };
            DBHelper.AddReady(task);
            HttpCookie loginNameCookie = HttpContext.Request.Cookies.Get("LoginName");
            DBHelper.AddAccountCount(loginNameCookie.Value,1);
            Tools.count++;
            return RedirectToAction("Hello");
        }


        [HttpPost]
        public ActionResult Result(int avNum,int lotteryNum)
        {
            if (Tools.GetPageNum(avNum.ToString()) == 0)
            {
                Tools.AlertAndRedirect("无法找到该视频", Url.Action("Lottery"));
                return null;
            }
            if (Tools.GetPageNum(avNum.ToString()) > Tools.MaxPageNum)
            {
                Tools.AlertAndRedirect("视频评论过多\n目前只能抽评论页数低于"+Tools.MaxPageNum+"页", Url.Action("Lottery", "Home"));
                return null;
            }
            List<ReplyInfo> list = Tools.GetAllReplyInfoFromBilibili(avNum.ToString());
            List<ReplyInfo> luckyList = Tools.Lottery(list, lotteryNum);
            ViewBag.TotalNum = list.Count;
            ViewBag.AvNum = avNum;
            ViewBag.LotteryNum = lotteryNum;
            ViewBag.LuckyList = luckyList;
            ViewBag.Title = "抽奖结果 -- Bilibili评论抽奖";
            Tools.count++;
            return View();
        }

        public ActionResult TimeResult(int lotteryNum)
        {
            ViewBag.Title = "定时抽奖结果 -- Bilibili评论抽奖";
            LotteryReady ready = DBHelper.GetLotteryReadyById(lotteryNum);
            List<LotteryResult> result = DBHelper.GetLotteryResult(lotteryNum);
            if (ready == null || ready.IsExecuted == 0 || result.Count == 0)
                return Content("未查询到开奖结果");
            ViewBag.LotteryData = ready;
            ViewBag.ResultData = result;

            return View();
        }
        public JsonResult Count()
        {
            return Json(Tools.count, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string LotteryName,string LotteryPsw)
        {
            if (DBHelper.CheckAccount(LotteryName, LotteryPsw) == false)
            {
                Tools.AlertAndRedirect("账号或密码错误", Url.Action("Login", "Home"));
                return Content("账号或密码错误");
            }
            else
            {
                //Add Name Cookie
                HttpCookie cookieName = new HttpCookie("LoginName");
                cookieName.Value = LotteryName;
                cookieName.Expires = DateTime.Now.AddHours(1);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookieName);
                //Add Certification Cookie
                string certification = Tools.makeCertification(LotteryName);
                HttpCookie cookieCer = new HttpCookie("Certification");
                cookieCer.Value = certification;
                cookieCer.Expires = DateTime.Now.AddHours(1);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookieCer);

                return RedirectToAction("Hello");
            }
        }

        public ActionResult Jump()
        {
            return View();
        }
    }
}