using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;

namespace Lottery.Business
{
    public class Model_Cycle_KillTwoNum_Middle : Model_Cycle_KillTwoNum
    {
        public Model_Cycle_KillTwoNum_Middle(List<DB_PredictRecord> history) : base(history)
        {
        }

        public override string GetThreeNo(string goodNo)
        {
            // return base.GetThreeNo(goodNo);
            var no = goodNo.Substring(2, 5);
            return no;
        }
    }
}
