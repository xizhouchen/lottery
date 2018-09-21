using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;

namespace Lottery.Business
{
    public class Model_OnlyPairPreWinChase : IModel
    {
        public Model_OnlyPairPreWinChase(List<DB_PredictRecord> history) : base(history)
        {

        }
        /// <summary>
        /// 3期后，如果上期出现对子，再追。
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="firstBet"></param>
        /// <param name="chaseNum"></param>
        /// <param name="benifitRate"></param>
        /// <param name="model"></param>
        /// <param name="secondBenifitRate"></param>
        /// <returns></returns>
        public override List<DB_PredictRecord> GenerateAmountResult(double balance, double firstBet, int chaseNum, double benifitRate, BetModel model, double secondBenifitRate)
        {
            //throw new NotImplementedException();
            List<DB_PredictRecord> betList = new List<DB_PredictRecord>();
            //追3期不中，停一期，计数器
            int preThreeCount = 0;
            //当前追的期，0为第一期
            int rateCount = 0;

            for (var i = 0; i < History.Count; i++)
            {
                if (model == BetModel.PairKlllCV1 || model == BetModel.PairKillCV2)
                {
                    if (i < 30)
                    {
                        i++;
                        continue;
                    }

                }
                DB_PredictRecord item = new DB_PredictRecord();
                item.GoodNo = History[i].GoodNo;
                item.RewardDate = History[i].RewardDate;
                item.IssueId = History[i].IssueId;
                //根据chaseNum 设置爆仓
                if (rateCount == chaseNum + 1)
                {
                    rateCount = 0;
                }
                //判断是否为前三期，前三期下注1.93*PayMoneys[stopCount]
                //if (rateCount < 3)
                //{

                //    //下注前三期
                //    item.PayAmount = firstBet * Math.Pow(2, rateCount);
                //    balance = balance - item.PayAmount;
                //    if (IsWinByMode(item, model))
                //    {
                //        item.Earn = firstBet * Math.Pow(2, rateCount) * benifitRate;
                //        balance = balance + item.Earn;
                //        item.Status = "中奖";
                //        rateCount = 0;
                //    }
                //    else
                //    {
                //        rateCount = rateCount + 1;
                //        item.Status = "未中奖";
                //    }
                //    //item.Balance = balance;

                //}
                ////前三次未中奖
                //else
                //{

                    if (i == 0) {
                    continue;
                    }
                    //判断上次是否中奖(是否出现对子)，应该4次开始，如果上次中奖就下单
                    if (IsWinByMode(History[i - 1], model))
                    {
                        item.PayAmount = firstBet * Math.Pow(2, rateCount);
                        balance = balance - item.PayAmount;
                      //  item.Kill5No = this.FindKillNumber(i, History); //i为要下注的那一期

                        if (IsWinByMode(History[i], model))
                        {
                            item.Earn = firstBet * Math.Pow(2, rateCount) * benifitRate;
                            balance = balance + item.Earn;
                            rateCount = 0;
                        }
                        else
                        {
                            rateCount = rateCount + 1;
                            item.Status = "未中奖";
                        }
                    }
                    else
                    {
                        item.Status = "未下单";
                    }

                //}
                item.Balance = balance;
                betList.Add(item);

            }


            return betList;
        }
    }
}
