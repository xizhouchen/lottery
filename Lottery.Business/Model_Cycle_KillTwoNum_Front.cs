using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;
using Lottery.PlatForm;

namespace Lottery.Business
{
    public class Model_Cycle_KillTwoNum_Front : Model_Cycle_KillTwoNum
    {
        public Model_Cycle_KillTwoNum_Front(List<DB_PredictRecord> history, int ct = 455, int rg = 1440) : base(history)
        {
            Critical = ct;
            Range = rg;
        }

        public Model_Cycle_KillTwoNum_Front(List<DB_PredictRecord> history, string killno1, string killno2, int ct = 455, int rg = 1440) : base(history)
        {
            this.KillNo1 = killno1;
            this.KillNo2 = killno2;
            Critical = ct;
            Range = rg;
        }

        public override string GetThreeNo(string goodNo)
        {
            var no = "," + goodNo.Substring(0, 5);
            return no;
        }


        public override bool AddOrder(IPlatForm pf, List<DB_PredictRecord> last30Records,
           string model = "li", int mul = 1)
        {
            //return base.AddOrder(pf, last30Records);

            var selectNum = "";
            var killno1 = this.KillNo1;
            var killno2 = this.KillNo2;
            if (string.IsNullOrEmpty(killno1) && string.IsNullOrEmpty(killno2))
            {
                DB_PredictRecord dr = new DB_PredictRecord();
                var nums = this.GetTwoKillNo(last30Records.Count, last30Records, ref dr);
                killno1 = nums[0];
                killno2 = nums[1];
            }
            for (var i = 0; i <= 9; i++)
            {
                if (i.ToString() != killno1 && i.ToString() != killno2)
                {
                    if (selectNum.Length != 14)
                    {
                        selectNum += i.ToString() + ",";
                    }
                    else
                    {
                        selectNum += i.ToString();
                    }
                }
            }
            var flag = pf.MakeOrder(model, mul, selectNum, pf.GetFrontKillTwoMethodName());
            return flag;
        }
    }
}
