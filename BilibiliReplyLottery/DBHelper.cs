using BilibiliReplyLottery.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BilibiliReplyLottery
{
    public static class DBHelper
    {
        public static bool AddReady(LotteryReady task)
        {
            if (task == null)
                return false;
            using(LotteryDataEntities db = new LotteryDataEntities())
            {
                db.LotteryReady.Add(task);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }
                
            }
            return true;
        }

        public static bool ChangeStateReady(int lotteryNum,bool isExecuted)
        {
            using(LotteryDataEntities db = new LotteryDataEntities())
            {
                LotteryReady ready = db.LotteryReady.Where(r => r.LotteryId == lotteryNum).FirstOrDefault();
                if(ready == null)
                {
                    return false;
                }
                if (isExecuted == true)
                    ready.IsExecuted = 1;
                else
                    ready.IsExecuted = 0;
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static bool AddResult(LotteryResult result)
        {
            if (result == null)
                return false;
            using (LotteryDataEntities db = new LotteryDataEntities())
            {
                db.LotteryResult.Add(result);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }

            }
            return true;
        }

        public static List<LotteryReady> GetLotteryReady(int IsExecuted)
        {
            using(LotteryDataEntities db = new LotteryDataEntities())
            {
                return db.LotteryReady.Where(r => r.IsExecuted == IsExecuted).ToList();
            }
        }
        public static LotteryReady GetLotteryReadyById(int lotteryId)
        {
            using (LotteryDataEntities db = new LotteryDataEntities())
            {
                return db.LotteryReady.Where(r => r.LotteryId == lotteryId).FirstOrDefault();
            }
        }

        public static List<LotteryResult> GetLotteryResult(int lotteryId)
        {
            using (LotteryDataEntities db = new LotteryDataEntities())
            {
                return db.LotteryResult.Where(r => r.LotteryId == lotteryId).ToList();
            }
        }

        public static bool AddAccount(string name,string psw)
        {
            using(LotteryDataEntities db = new LotteryDataEntities())
            {
                LotteryAccount account = new LotteryAccount
                {
                    LotteryName = name,
                    LotteryPsw = psw,
                    LotteryCount = 0,
                };
                db.LotteryAccount.Add(account);
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }

            }
            return true;
        }

        public static bool CheckAccount(string name,string psw)
        {
            using (LotteryDataEntities db = new LotteryDataEntities())
            {
                LotteryAccount account =  db.LotteryAccount.
                    Where(u => u.LotteryName == name && u.LotteryPsw == psw).FirstOrDefault();
                if (account == null)
                    return false;
                else
                    return true;
            }
        }

        public static bool AddAccountCount(string name,int addNum)
        {
            using(LotteryDataEntities db = new LotteryDataEntities())
            {
                LotteryAccount account = db.LotteryAccount.
                    Where(u => u.LotteryName == name).FirstOrDefault();
                if (account == null)
                    return false;
                account.LotteryCount += addNum;
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static LotteryAccount GetAccount(int Id)
        {
            using (LotteryDataEntities db = new LotteryDataEntities())
            {
                LotteryAccount account = db.LotteryAccount.
                    Where(u => u.LID==Id).FirstOrDefault();
                if (account == null)
                    return null;
                else
                    return account;
            }
        }

        public static LotteryAccount GetAccount(string name)
        {
            using (LotteryDataEntities db = new LotteryDataEntities())
            {
                LotteryAccount account = db.LotteryAccount.
                    Where(u => u.LotteryName == name).FirstOrDefault();
                if (account == null)
                    return null;
                else
                    return account;
            }
        }
    }
}