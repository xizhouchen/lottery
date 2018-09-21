using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Model
{
    public class AnalysisModel
    {

        public int MaxContinueFailedCount { get; set; }
        /// <summary>
        /// 最大连续未出现对子
        /// </summary>
        public int MaxFailedCount { get; set; }

        /// <summary>
        /// 最高一把追的钱
        /// </summary>
        public double MaxPay { get; set; }

        /// <summary>
        /// 追失败的次数
        /// </summary>
        public double ChaseFailedCount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public double Balance { get; set; }


        public List<DB_PredictRecord> explodeRecord { get; set; }


        public double TotalPay { get; set; }


        public double WinTimes { get; set; }

        public double LossTimes { get; set; }



        public int CycleTotalNum { get; set; }

        public int CycleWinNum { get; set; }

        public int CycleFailedNum { get; set; }


    }
}
