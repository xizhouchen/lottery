using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;

namespace Lottery.Business
{
    public class Model_PairKillOne : IModel
    {

       
        public Model_PairKillOne(List<DB_PredictRecord> history) : base(history)
        {
            
        }

        public override List<DB_PredictRecord> GenerateAmountResult(double balance, double firstBet, int chaseNum, double benifitRate, BetModel model,double secondBenifitRate)
        {
            List<DB_PredictRecord> betList = new List<DB_PredictRecord>();
            //追3期不中，停一期，计数器
            int preThreeCount = 0;
            //当前追的期，0为第一期
            int rateCount = 0;

            for (var i = 0; i < History.Count; i++) {
                if (model == BetModel.PairKlllCV1 || model == BetModel.PairKillCV2) {
                    if (i < 30) {
                        i++;
                        continue;
                    }
                  
                }
                DB_PredictRecord item = new DB_PredictRecord();
                item.GoodNo = History[i].GoodNo;
                item.RewardDate = History[i].RewardDate;
                item.IssueId = History[i].IssueId;
                //根据chaseNum 设置爆仓
                if (rateCount == chaseNum + 1) {
                    rateCount = 0;
                }
                //判断是否为前三期，前三期下注1.93*PayMoneys[stopCount]
                if (rateCount < 3)
                {

                    //下注前三期
                    item.PayAmount = PayMoneys[rateCount];
                    balance = balance - item.PayAmount;
                    if (IsWinByMode(item, model))
                    {
                        item.Earn = PayMoneys[rateCount] * benifitRate;
                        balance = balance + item.Earn;
                        item.Status = "中奖";
                        rateCount = 0;
                    }
                    else
                    {
                        rateCount = rateCount + 1;
                        item.Status = "未中奖";
                    }
                    //item.Balance = balance;

                }
                //前三次未中奖
                else {

                    //判断上次是否中奖(是否出现对子)，应该4次开始，如果上次中奖就下单
                    if (IsWinByMode(History[i - 1], model))
                    {
                        item.PayAmount = PayMoneys[rateCount];
                        balance = balance - item.PayAmount;
                        item.Kill5No = this.FindKillNumber(i,History); //i为要下注的那一期

                        if (IsWinByModeKillNo(History[i], model, item.Kill5No))
                        {
                            item.Earn = PayMoneys[rateCount] * secondBenifitRate;
                            balance = balance + item.Earn;
                            rateCount = 0;
                        }
                        else {
                            rateCount = rateCount + 1;
                            item.Status = "未中奖";
                        }
                    }
                    else {
                        item.Status = "未下单";
                        
                      
                        
                    }
                    
                }
                item.Balance = balance;
                betList.Add(item);

            }
            

            return betList;
        }


        /// <summary>
        /// 根据模式判断
        /// 判断逻辑，有且只有一个对子，不能有3个的，4 个的，5个的。就是中奖
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool IsWinByModeKillNo(DB_PredictRecord record, BetModel model, string killNo)
        {
            bool win = false;

            if (OnlyPairModelBet(record.GoodNo) && !record.GoodNo.Contains(killNo))
            {
                win = true;
            }

            return win;
        }

        /// <summary>
        /// 获取对子的数
        /// </summary>
        /// <returns></returns>
        public virtual string FindKillNumber(int payIndex, List<DB_PredictRecord> records)
        {
            var lotteryResult = records[payIndex - 1].GoodNo;
            var strArr = lotteryResult.Split(new char[] { ',' });
            var rlt = "";
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

            for (var i = 0; i <= 9; i++)
            {
                if (occurCountDic[i] > 2)
                {

                }
                else if (occurCountDic[i] == 2)
                {
                    rlt = i.ToString();
                }
            }

            return rlt;
        }

        
    }
}
