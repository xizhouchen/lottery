using CommonFunction;
using Lottery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Business
{
    /// <summary>
    /// 分析彩票中奖记录的连续性
    /// </summary>
    public class AnalysisBusiness
    {
        private IModel _calModel;
        public AnalysisBusiness(BetModel betModel,List<DB_PredictRecord> history) {

            if (betModel == BetModel.OnlyPair)
            {
                _calModel = new Model_OnlyPair(history);

            }
            else if (betModel == BetModel.PairKill)
            {
                // calModel = new
                _calModel = new Model_PairKillOne(history);
            }
            else if (betModel == BetModel.PairKlllCV1)
            {
                _calModel = new Model_PairKillOne_CV1(history);

            }
            else if (betModel == BetModel.PairKillCV2)
            {
                _calModel = new Model_PairKillOne_CV2(history);
            }
            else if (betModel == BetModel.OnlyPairPreWin)
            {

                _calModel = new Model_OnlyPairPreWinChase(history);
            }
            else if (betModel == BetModel.KillTowNo)
            {
                _calModel = new Model_Cycle_KillTwoNum(history);

            }
            else if (betModel == BetModel.KillTowNoMiddle) {
                _calModel = new Model_Cycle_KillTwoNum_Middle(history);
            } else if (betModel == BetModel.KillTowNoFront) {
                _calModel = new Model_Cycle_KillTwoNum_Front(history);
            }


        }

        public void SetKillNo(string no1,string no2) {
            _calModel.KillNo1 = no1;
            _calModel.KillNo2 = no2;
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

        public List<CanBePayPoint> FindPayPoints(int linjie) {

            return _calModel.FindPayPoints(linjie);
        }

        public List<CanBePayPoint> FindPayPoints(int linjie,double payRate)
        {

            return _calModel.FindPayPoints(linjie,payRate);
        }

        public AnalysisModel GenerateAnalysisModel(List<DB_PredictRecord> records) {
            AnalysisModel model = new AnalysisModel();
            model.explodeRecord = new List<DB_PredictRecord>();
            //var records = this._calModel.History;
            records = records.OrderBy(r => r.IssueId).ToList();
            var count = 0;
            double maxMoney = 0;
            
            foreach (var item in records) {
                if (!this.OnlyPairModelBet(item.GoodNo))
                {
                    count++;
                    if (model.MaxFailedCount < count)
                    {
                        model.MaxFailedCount = count;
                    }
                }
                else {
                    count = 0;
                }
                if (model.MaxPay < item.PayAmount) {
                    model.MaxPay = item.PayAmount;
                }

                if (maxMoney < item.PayAmount) {
                    maxMoney = item.PayAmount;
                }

                model.TotalPay = model.TotalPay + item.PayAmount;
            }


            int maxContineCount = 0;
            foreach (var item in records) {
                if (item.PayAmount == maxMoney && item.Status == "未中奖") {
                    model.ChaseFailedCount = model.ChaseFailedCount + 1;
                    model.explodeRecord.Add(item);
                }

                //中奖次数和未中奖次数
                if (item.Status == "未中奖")
                {
                    model.LossTimes = model.LossTimes + 1;
                    maxContineCount = maxContineCount + 1;
                    if (model.MaxContinueFailedCount < maxContineCount) {
                        model.MaxContinueFailedCount = maxContineCount;
                    }
                }
                else if (item.Status == "中奖") {
                    model.WinTimes = model.WinTimes + 1;
                    maxContineCount = 0;
                }
            }
            model.Balance = records[records.Count - 1].Balance;

            //周期分析
            foreach (var item in records) {
                if (item.Status == "中奖")
                {
                    model.CycleWinNum = model.CycleWinNum + 1;
                    model.CycleTotalNum = model.CycleTotalNum + 1;
                }
                else if (item.Status == "未中奖" && item.CirCleNum == 3) {
                    model.CycleFailedNum = model.CycleFailedNum + 1;
                    model.CycleTotalNum = model.CycleTotalNum + 1;
                }
            }

            return model;
        }

        /// <summary>
        /// 生成 中奖结果
        /// </summary>
        /// <param name="model">下注模式</param>
        /// <param name="records">历史开奖记录</param>
        /// <returns></returns>
        public List<DB_PredictRecord> GenerateBetResult(BetModel model, List<DB_PredictRecord> records) {
            return records;
            //return null;
            //foreach (var item in records) {
            //    if (IsWinByMode(item, model))
            //    {
            //        item.Status = "中奖";
            //    }
            //    else
            //    {
            //        item.Status = "未中奖";
            //    }
            //}
            //return records;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="balance">余额</param>
        /// <param name="firstBet">第一次下注金额</param>
        /// <param name="chaseNum">最高跟追次数</param>
        /// <param name="benifitRate">彩票赔率</param>
        /// <param name="model">判断中奖的模式</param>
        /// <param name="records">中奖历史记录</param>
        /// <returns></returns>
        public List<DB_PredictRecord> GenerateAmountResult(double balance,double firstBet, int chaseNum,double benifitRate,BetModel model,double rate2)
        {
           
            var records = _calModel.GenerateAmountResult(balance, firstBet, chaseNum, benifitRate, model,rate2);
            return records;
        }

        
       

    }
}
