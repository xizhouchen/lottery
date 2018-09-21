using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Model
{
    public class DB_PredictRecord
    {
        public string IssueId { get; set; } // 期号
        public string Kill5No { get; set; } // 杀号字符串

        public string Kill3No { get; set; }

        public string GoodNo { get; set; } // 中奖号码

        public string Status { get; set; } // 中奖或者未中

        /// <summary>
        /// 是否下单
        /// </summary>
        public string IsPay { get; set; }
        public double PayAmount { get; set; }

        public double Earn { get; set; }
        public double Balance { get; set; }


       

        public string RewardDate { get; set; }

        /// <summary>
        /// 周期内的期号
        /// </summary>
        public int CirCleNum { get; set; }

        /// <summary>
        /// 周期号，3期出奖，为1周期
        /// </summary>
        public int CircleTotal { get; set; }

     

      
    }
}
