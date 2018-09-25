using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Model
{
    public class ResultItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lottery { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string issue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string code1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string code2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long openTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int clearStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long clearTime { get; set; }
    }

    public class Lottery
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 腾讯分分彩
        /// </summary>
        public string showName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string shortName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string frequency { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int times { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int stopDelay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int downCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fenDownCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int liDownCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int floatBonus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double maxBonus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 开奖时间：凌晨0点至24点，开奖频率：1分钟一期，每日期数：1440期。
        /// </summary>
        public string description { get; set; }
    }

    public class QQminData
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ResultItem> result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Lottery lottery { get; set; }
    }

    public class Res_QQminResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 请求成功
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public QQminData data { get; set; }
    }
}
