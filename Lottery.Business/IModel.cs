using Lottery.Model;
using Lottery.PlatForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Business
{

    public enum BetModel
    {
        OnlyPair = 1,
        PairKill = 2,
        PairKlllCV1 = 3, // 复杂
        PairKillCV2 = 4,  // 杀冷号
        OnlyPairPreWin = 5, // 前三期迈对子，前三期不中，如果出现对子，再追，失败后，继续重复
        KillTowNo = 6,
        KillTowNoMiddle = 7, //中三杀2
        KillTowNoFront = 8 //前三杀2

    }
    public abstract class IModel
    {
        public string KillNo1 { get; set; }

        public string KillNo2 { get; set; }

        public Dictionary<int, double> PayMoneys = new Dictionary<int, double>();
        public IModel(List<DB_PredictRecord> history) {

            PayMoneys.Add(0, 5.04);
            PayMoneys.Add(1, 11.76);
            PayMoneys.Add(2, 23.52);
            PayMoneys.Add(3, 20.16);
            PayMoneys.Add(4, 26.28);
            PayMoneys.Add(5, 41.328);
            PayMoneys.Add(6, 59.472);
            PayMoneys.Add(7, 86.688);
            PayMoneys.Add(8, 124.992);
            PayMoneys.Add(9, 181.440);
            PayMoneys.Add(10, 262.080);
            PayMoneys.Add(11, 380.016);
            PayMoneys.Add(12, 550.368);
            PayMoneys.Add(13, 796.320);

            PayMoneys.Add(14, 1154.16);
            PayMoneys.Add(15, 1671.264);
            PayMoneys.Add(16, 2419.200);
            PayMoneys.Add(17, 3503.80);
            PayMoneys.Add(18, 5074.272);
            PayMoneys.Add(19, 7349.328);

            this.History = history;
        }
        public virtual List<CanBePayPoint> FindPayPoints(int splitNum = 455)
        {
            List<CanBePayPoint> payPoints = new List<CanBePayPoint>();
          
            return payPoints;

        }

        public virtual List<CanBePayPoint> FindPayPoints(int lastNumber = 240, double payRate = 0.83)
        {
            List<CanBePayPoint> payPoints = new List<CanBePayPoint>();

            return payPoints;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pf">用来下单的平台</param>
        /// <param name="last30Records">预测杀号，有些模型不需要，比如杀固定值</param>
        /// <returns></returns>
        public virtual bool AddOrder(IPlatForm pf,List<DB_PredictRecord> last30Records,string model = "li", int mul = 1)
        {
            return false;
        }

        public virtual CanBePayPoint FindPayPoint()
        {
            List<CanBePayPoint> payPoints = new List<CanBePayPoint>();
            var an = this.GenerateAmountResult(1, 1, 1, 1, BetModel.KillTowNo, 1);

            for (var i = 0; i <= an.Count - 1; i++)
            {
                int wincount = 0;
                int failedcount = 0;
                int passcount = 0;
                for (var j = i; j <= an.Count - 1; j++)
                {
                    passcount++;
                    if (an[j].Status == "中奖")
                    {
                        wincount++;
                    }
                    else
                    {
                        failedcount++;
                    }
                    var x = (455 - wincount) * 0.16464;
                    var y = 0.056 * (1440 - passcount);
                    if (x - y > 0)
                    {

                        CanBePayPoint p = new CanBePayPoint();
                        p.StartIssueId = an[i].IssueId;
                        p.RestPayCount = 1440 - passcount;
                        p.PayIssueId = an[j].IssueId;
                        p.PassCount = passcount;
                        p.WinCount = wincount;
                        if (an[j].IssueId == an[an.Count - 1].IssueId)
                        {
                            payPoints.Add(p);
                        }

                        break;
                    }

                }
            }
            payPoints = payPoints.OrderByDescending(r => r.StartIssueId).ToList();
            if (payPoints.Count > 0)
            {
                return payPoints[0];
            }
            return null;

        }


        /// <summary>
        /// 后三
        /// </summary>
        /// <param name="goodNo"></param>
        /// <returns></returns>
        public virtual string GetThreeNo(string goodNo)
        {
            return goodNo.Substring(3);
        }

        /// <summary>
        /// 根据模式判断
        /// 判断逻辑，有且只有一个对子，不能有3个的，4 个的，5个的。就是中奖
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool IsWinByMode(DB_PredictRecord record, BetModel model)
        {
            bool win = false;

            if (OnlyPairModelBet(record.GoodNo))
            {
                win = true;
            }


            return win;
        }


        /// <summary>
        /// 获取最近十期，出现的次数
        /// </summary>
        /// <param name="payIndex"></param>
        /// <param name=""></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public int GetOccurTimesLast10(int payIndex, List<DB_PredictRecord> records, string num)
        {
            var count = 0;
            for (var i = 1; i <= 9; i++)
            {
                if (records[payIndex - i].GoodNo.Contains(num))
                {
                    count = count + 1;
                }
            }

            return count;
        }

        /// <summary>
        /// 选择第二个杀号（冷号）
        /// </summary>
        /// <param name="payIndex"></param>
        /// <param name="records"></param>
        /// <param name="exceptNum">排除一个号</param>
        /// <returns></returns>
        public string SelectColdNo(int payIndex, List<DB_PredictRecord> records, string exceptNum = null,bool isAfterThree = false)
        {
            //计算出所有号 最近出现的次数
            Dictionary<int, int> ne = new Dictionary<int, int>();
            for (var i = 0; i <= 9; i++)
            {
                var count = 0;
                for (var j = payIndex - 1; j >= 0; j--)
                {
                    var goodNo = records[j].GoodNo.ToString();
                    if (isAfterThree) {
                        goodNo = GetThreeNo(goodNo);
                    }
                    if (goodNo.Contains(i.ToString()))
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

            //如果有排除的号，排除
            if (!string.IsNullOrEmpty(exceptNum)) {
                ne.Remove(int.Parse(exceptNum));
            }

            Dictionary<int, int> afterXie = new Dictionary<int, int>();
            //过滤掉不斜连号
            foreach (var key in ne.Keys)
            {
                var preGoodNo = records[payIndex - 1].GoodNo;
                var prepreGoodNo = records[payIndex - 2].GoodNo;
                if (isAfterThree)
                {
                    preGoodNo = GetThreeNo(preGoodNo);
                    prepreGoodNo = GetThreeNo(prepreGoodNo);
                }
                if (!IsObliqueConnection(key.ToString(), preGoodNo, prepreGoodNo))
                {
                    afterXie.Add(key, ne[key]);
                }
                //Console.WriteLine(key + ne[key]);
            }

            ///如果没有满足不斜连的号,就用之前的集合来判定
            if (afterXie.Count == 0) {
                afterXie = ne;
            }

            //找出没有斜连的号的近期最大未出现次数
            int max = 0;
            foreach (var key in afterXie.Keys)
            {
                if (max < afterXie[key])
                {
                    max = afterXie[key];
                }
            }

            //抽出满足不是斜连而且近期未出现次数最大 的号 集合
            Dictionary<int, int> maxNos = new Dictionary<int, int>();
            foreach (var key in afterXie.Keys)
            {
                if (afterXie[key] == max)
                {
                    maxNos.Add(key, max);
                }
            }
            if (maxNos.Keys.Count == 1)
            {
                return maxNos.Keys.ToList()[0].ToString();
            }

            //如果有相同的未出现次数以及都满足未斜连，比较最新10期出现次数,选最少的
            Dictionary<int, int> occurLessNums = new Dictionary<int, int>();

            int minCount = 100000;
            foreach (var key in maxNos.Keys)
            {
                var co = this.GetOccurTimesLast10(payIndex, records, key.ToString());
                if (minCount > co)
                {
                    minCount = co;
                }
                occurLessNums.Add(key, co);
            }

            //取出现最少的那个，如果是集合，取第一个
            foreach (var key in occurLessNums.Keys)
            {
                if (occurLessNums[key] == minCount)
                {
                    return key.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// 判断斜连
        /// </summary>
        /// <param name="killNo">所选杀号</param>
        /// <param name="preGoodNo">上期中奖号码</param>
        /// <param name="pre2GoodNo">上上期中奖号码</param>
        /// <returns></returns>
        public bool IsObliqueConnection(string killNo, string preGoodNo, string pre2GoodNo)
        {
            var flag = false;


            int ko = int.Parse(killNo);

            //ko 作为 等差数列末尾 等差为1
            if (preGoodNo.Contains((ko - 1).ToString()) && pre2GoodNo.Contains((ko - 2).ToString()))
            {
                flag = true;
            }
            //ko 作为等差数列开始 等差为1
            if (preGoodNo.Contains((ko + 1).ToString()) && pre2GoodNo.Contains((ko + 2).ToString()))
            {
                flag = true;
            }

            //ko 作为 等差数列末尾 等差为2
            if (preGoodNo.Contains((ko - 2).ToString()) && pre2GoodNo.Contains((ko - 4).ToString()))
            {
                flag = true;
            }
            //ko 作为等差数列开始 等差为2
            if (preGoodNo.Contains((ko + 2).ToString()) && pre2GoodNo.Contains((ko + 4).ToString()))
            {
                flag = true;
            }

            return flag;
        }




        /// <summary>
        /// 判断逻辑，有且只有一个对子，不能有3个的，4 个的，5个的。就是中奖
        /// </summary>
        /// <param name="lotteryResult"></param>
        /// <returns></returns>
        public bool OnlyPairModelBet(string lotteryResult)
        {
            bool flag = false;

            var strArr = lotteryResult.Split(new char[] { ',' });

            Dictionary<int, int> occurCountDic = new Dictionary<int, int>();
            occurCountDic.Add(0, 0);
            occurCountDic.Add(1, 0);
            occurCountDic.Add(2, 0);
            occurCountDic.Add(3, 0);
            occurCountDic.Add(4, 0);
            occurCountDic.Add(5, 0);
            occurCountDic.Add(6, 0);
            occurCountDic.Add(7, 0);
            occurCountDic.Add(8, 0);
            occurCountDic.Add(9, 0);


            int pairCount = 0;
            for (var i = 0; i < strArr.Length; i++)
            {
                if (!string.IsNullOrEmpty(strArr[i].ToString()))
                {
                    occurCountDic[int.Parse(strArr[i].ToString())] = occurCountDic[int.Parse(strArr[i].ToString())] + 1;
                }
            }

            ///判断有3个,4个,5个 存在一样的
            for (var i = 0; i <= 9; i++)
            {
                if (occurCountDic[i] > 2)
                {
                    return false;
                }
                else if (occurCountDic[i] == 2)
                {
                    pairCount = pairCount + 1;
                }
            }

            ///判断只有一个对子
            if (pairCount == 1)
            {
                flag = true;
            }
            return flag;
        }

        public List<DB_PredictRecord> History { get; set; }

        /// <summary>
        /// 根据下单模式生成对应历史中奖记录
        /// 
        /// <param name="balance">余额</param>
        /// <param name="firstBet">第一次下注金额</param>
        /// <param name="chaseNum">最高跟追次数</param>
        /// <param name="benifitRate">彩票赔率</param>
        /// <param name="model">判断中奖的模式</param>
        /// <param name="records">开奖历史记录</param>
        public abstract List<DB_PredictRecord> GenerateAmountResult(double balance, double firstBet, int chaseNum, double benifitRate, BetModel model, double secondBenifitRate);
           
        

        
            
             
    }
}
