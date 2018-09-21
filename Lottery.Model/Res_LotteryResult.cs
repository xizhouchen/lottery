using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Model
{
   
    public class LISTItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string ZJHM { get; set; } // 中奖号码
        /// <summary>
        /// 
        /// </summary>
        //public string MEMO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CP_QS { get; set; } //期号
        /// <summary>
        /// 
        /// </summary>
        public string KJSJ { get; set; } // 开奖时间
        /// <summary>
        /// 
        /// </summary>
        public string SJKJSJ { get; set; } //实际开奖时间
        /// <summary>
        /// 
        /// </summary>
        public string CP_ID { get; set; } // 彩票类型ID
    }

    public class Detail
    {
        /// <summary>
        /// 
        /// </summary>
        public string LIST_PAGE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<LISTItem> LIST { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LIST_MAX_PAGE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LIST_COUNT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LIST_ROW_COUNT { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public string count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Detail detail { get; set; }
    }

    public class Res_LotteryResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string x_hit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
    }
}
