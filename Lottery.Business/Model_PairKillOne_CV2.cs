using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;

namespace Lottery.Business
{
    public class Model_PairKillOne_CV2 : Model_PairKillOne
    {
        public Model_PairKillOne_CV2(List<DB_PredictRecord> history) : base(history)
        {

        }

        public override string FindKillNumber(int payIndex, List<DB_PredictRecord> records)
        {
            var num = "";

            Dictionary<int, int> ne = new Dictionary<int, int>();
            for (var i = 0; i <= 9; i++)
            {
                var count = 0;
                for (var j = payIndex - 1; j >= 0; j--)
                {
                    if (records[j].GoodNo.ToString().Contains(i.ToString()))
                    {
                        ne.Add(i, count);
                        break;
                    }
                    else
                    {
                        count++;

                    }
                }

            }

            return SelectMax(ne);
        }

        public string SelectMax(Dictionary<int,int> noExists) {
            var num = "";
            int maxValue = 0;
            foreach (var key in noExists.Keys) {
                if (maxValue < noExists[key]) {
                    maxValue = noExists[key];
                }
            }
            Dictionary<int,int> maxCol = new Dictionary<int, int>();
            foreach (var key in noExists.Keys) {
                if (noExists[key] == maxValue) {
                    maxCol.Add(key, maxValue);
                }
            }

            if (maxCol.Keys.Count == 1 || maxValue == 1)
            {
                return maxCol.Keys.ToList()[0].ToString();
            }
            else {
                foreach (var key in maxCol.Keys) {
                    noExists.Remove(key);
                }
                //如果都一样
                if (noExists.Keys.Count == 0) {
                    return maxCol.Keys.ToList()[0].ToString();
                }
                num = SelectMax(noExists);
            }

            return num;
                
        }
    }
}
