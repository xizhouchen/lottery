using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Model
{
    public class Bet_infoItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string multiple { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bet_money { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string method_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string calc_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rebate_count { get; set; }
    }

    public class Params
    {
        /// <summary>
        /// 
        /// </summary>
        public string count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Bet_infoItem> bet_info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string issue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lottery_id { get; set; }
    }

    public class DetailItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string order_id { get; set; }
        /// <summary>
        /// 找不到期号
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string business_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uniq_order_id { get; set; }
    }

        public class PayData
        {
            /// <summary>
            /// 
            /// </summary>
            public string count { get; set; }
            /// <summary>
            /// 
            /// </summary>
            //public Params params { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string balance { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string user_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<DetailItem> detail { get; set; }
    }

    public class Res_Pay
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
        public PayData data { get; set; }
    }
}
