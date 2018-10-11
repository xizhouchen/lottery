using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Model
{

    public class BalanceData
    {
        /// <summary>
        /// LotteryBalance
        /// </summary>
        public double lotteryBalance { get; set; }
        /// <summary>
        /// BaccaratBalance
        /// </summary>
        public double baccaratBalance { get; set; }
        /// <summary>
        /// TotalBaccaratBalance
        /// </summary>
        public double totalBaccaratBalance { get; set; }
        /// <summary>
        /// MsgCount
        /// </summary>
        public int msgCount { get; set; }
    }

    public class Res_Minjue_BalanceResult
    {
        /// <summary>
        /// Error
        /// </summary>
        public int error { get; set; }
        /// <summary>
        /// Code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 请求成功
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        public BalanceData data { get; set; }
    }

}
