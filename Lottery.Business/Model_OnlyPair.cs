using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;

namespace Lottery.Business
{
    public class Model_OnlyPair : IModel
    {
        public Model_OnlyPair(List<DB_PredictRecord> history) : base(history)
        {

        }
        public override List<DB_PredictRecord> GenerateAmountResult(double balance, double firstBet, int chaseNum, double benifitRate, BetModel model,double benifitRate2)
        {
            //throw new NotImplementedException();
            double count = 0;
            double rateCount = 0;
            foreach (var item in History)
            {

                if (count < 3)
                {
                    item.PayAmount = firstBet * Math.Pow(2, rateCount);
                    balance = balance - firstBet * Math.Pow(2, rateCount);
                    item.Balance = balance;
                    if (IsWinByMode(item, model))
                    {
                        item.Status = "中奖";
                        balance = balance + (firstBet * Math.Pow(2, rateCount) * benifitRate);
                        item.Balance = balance;
                        item.Earn = firstBet * Math.Pow(2, rateCount) * benifitRate;
                        count = 0;
                        rateCount = 0;

                    }
                    else
                    {
                        item.Status = "未中奖";
                        //追6期，停止追
                        if (rateCount == chaseNum)
                        {
                            rateCount = 0;
                            count++;
                        }
                        else
                        {
                            rateCount++;
                            count++;
                        }

                    }
                    item.IsPay = "已下单";
                }
                else
                {
                    item.IsPay = "未下单";
                    item.Balance = balance;
                    if (IsWinByMode(item, model))
                    {
                        count = 0;
                    }
                    item.PayAmount = 0;
                    //item.Status = "未下单";

                }

            }

            return History;
        }
    }
}
