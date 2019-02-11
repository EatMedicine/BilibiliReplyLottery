using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BilibiliReplyLottery.Class
{
    public class ReplyInfo
    {
        //唯一指示用户id
        public string mid;
        //楼层数
        public int floor;
        
        //名字
        public string uname;
        public string sex;
        //个人签名
        public string sign;
        public string msg;

        public ReplyInfo(string mid, int floor, string uname, string sex, string sign, string msg)
        {
            this.mid = mid;
            this.floor = floor;
            this.uname = uname;
            this.sex = sex;
            this.sign = sign;
            this.msg = msg;
        }
    }
}