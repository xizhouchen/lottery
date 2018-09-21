using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;

namespace Lottery.Business
{
    public class Model_PairKillOne_CV1 : Model_PairKillOne
    {
        public Model_PairKillOne_CV1(List<DB_PredictRecord> history) : base(history)
        {

        }

        public override string FindKillNumber(int payIndex, List<DB_PredictRecord> records)
        {
            var num = "";
           
            var firstKillNo =  base.FindKillNumber(payIndex, records);

            //第一个杀号如果不满足如下3条件中一个，进入第二杀号选择

            //1.上上期是否此号存在
            if(records[payIndex - 2].GoodNo.Contains(firstKillNo)){
                return SelectColdNo(payIndex, records);
            }
            //2. 出现斜连 （等差数列）
            if (IsObliqueConnection(firstKillNo, records[payIndex - 1].GoodNo, records[payIndex - 2].GoodNo)) {
                return SelectColdNo(payIndex, records);
            }

            //3. 最近9期出现大于等于5次
            if (IsHotNo(firstKillNo, payIndex, records))
            {
                return SelectColdNo(payIndex, records);
            }

            return firstKillNo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsHotNo(string killNo,int payIndex,List<DB_PredictRecord> records ) {
            bool hot = false;
            int count = 0;
            for (var i = 1; i <= 9; i++) {
                if (records[payIndex - i].GoodNo.Contains(killNo))
                {
                    count++;
                }
            }
            if (count >= 5) {
                hot = true;
            }

            return hot;
        }
    }
}
