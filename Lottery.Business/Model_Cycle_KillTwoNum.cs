using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;

namespace Lottery.Business
{
    public class Model_Cycle_KillTwoNum : IModel
    {
        public Model_Cycle_KillTwoNum(List<DB_PredictRecord> history) : base(history)
        {

        }

        public override List<DB_PredictRecord> GenerateAmountResult(double balance, double firstBet, int chaseNum, double benifitRate, BetModel model, double secondBenifitRate)
        {
            this.PayMoneys[0] = 5;
            this.PayMoneys[1] = 12;
            this.PayMoneys[2] = 27;
            List<DB_PredictRecord> betList = new List<DB_PredictRecord>();
            //3期为一个周期
            int count = 1;

            bool isStop = false;
            //前30期用来判断，下棋号码，不进行结果预测
            for (var i = 29; i < this.History.Count; i++) {
               
                DB_PredictRecord item = new DB_PredictRecord();
                item.GoodNo = History[i].GoodNo;
                item.RewardDate = History[i].RewardDate;
                item.IssueId = History[i].IssueId;

                //判断前三期的对子数，如果有两个对子，则不下单，且再跳2期
                //3期有对子计数器
                //int pairCount = 0;
                //for (var j = 1; j <= 3; j++) {
                //    if (this.IsHasPair(History[i - j].GoodNo.Substring(3)))
                //    {
                //        pairCount = pairCount + 1;
                //    }
                //}

                //if (pairCount >= 1) {
                //    item.Status = "未下单";
                //    betList.Add(item);
                //    i = i + 1;
                //    if (i < History.Count) {
                //        History[i].Status = "未下单";
                //        betList.Add(History[i]);
                //    }

                //    i = i + 1;
                //    if (i < History.Count) {
                //        History[i].Status = "未下单";
                //        betList.Add(History[i]);
                //    }

                //    continue;
                //}

                //出现对子停3期
                //if (this.IsHasPair(History[i-1].GoodNo.Substring(3))) {
                //    item.Status = "未下单";
                //    betList.Add(item);
                //    i = i + 1;
                //    if (i < History.Count)
                //    {
                //        History[i].Status = "未下单";
                //        betList.Add(History[i]);
                //    }

                //    i = i + 1;
                //    if (i < History.Count)
                //    {
                //        History[i].Status = "未下单";
                //        betList.Add(History[i]);
                //    }

                //    continue;
                //}

                item.PayAmount = this.PayMoneys[count - 1];
                balance = balance - item.PayAmount;
                item.CirCleNum = count;
                item.Balance = balance;
                if (this.IsWinByMode(this.History[i - 1], BetModel.OnlyPair) || !isStop)
                {
                    if (IsWinByMode(item, BetModel.OnlyPair))
                    {
                        item.Earn = item.PayAmount * benifitRate;
                        balance = balance + item.Earn;
                        item.Balance = balance;
                        item.Status = "中奖";
                        isStop = false;
                        item.CircleTotal = item.CircleTotal + 1;
                        count = 1;
                    }
                    else
                    {
                        count = count + 1;
                        item.Status = "未中奖";
                        if (count == 4)
                        {
                            isStop = true;
                            count = 1;
                            item.CircleTotal = item.CircleTotal + 1;
                        }
                        else {
                            isStop = false;
                        }


                    }
                }
                else {
                    item.Status = "未下单";
                }
               
                betList.Add(item);
            }
            return betList;


        }

        public bool IsWinByKillTwoNum(int payIndex,List<DB_PredictRecord> records,ref DB_PredictRecord item) {
            bool flag = false;
            Dictionary<int, int> ne = new Dictionary<int, int>();
            ne[0] = 0;
            ne[1] = 0;
            ne[2] = 0;
            ne[3] = 0;
            ne[4] = 0;
            ne[5] = 0;
            ne[6] = 0;
            ne[7] = 0;
            ne[8] = 0;
            ne[9] = 0;
            List<string> rlt = new List<string>();
            //先选出前两期出现了两次的号
           
            foreach (var key in ne.Keys) {
                if (records[payIndex - 1].GoodNo.Substring(3).Contains(key.ToString()) && records[payIndex - 2].GoodNo.Substring(3).Contains(key.ToString())) {
                    rlt.Add(key.ToString());
                }
            }

            if (rlt.Count > 1) {
                item.Kill3No = rlt[0];
                item.Kill5No = rlt[1];
                return CheckWin(rlt[0], rlt[1], records[payIndex].GoodNo,true);
            }
            var firstNum = "";
            var secondNum = "";
            //能取到一个号
            if (rlt.Count == 1)
            {
                firstNum = rlt[0];
                secondNum = SelectColdNo(payIndex, records,null,true);
                item.Kill3No = firstNum;
                item.Kill5No = secondNum;
              
                return CheckWin(firstNum, secondNum, records[payIndex].GoodNo,true);
            }
            //一个号都不能取到，选择冷号集合(两个冷号)
            else {
                firstNum = SelectColdNo(payIndex, records,null,true);
                secondNum = SelectColdNo(payIndex, records,firstNum,true);
                item.Kill3No = firstNum;
                item.Kill5No = secondNum;
              
                return CheckWin(firstNum, secondNum, records[payIndex].GoodNo,true);
            }
          


            

         
        }

        public bool CheckWin(string num1, string num2, string goodNo,bool onlyOne = false)
        {
            if (onlyOne == true) {
                num1 = "11111";
            }
            var afterThree = goodNo.Substring(3);
            if (!afterThree.Contains(num1) && !afterThree.Contains(num2)) {
                //后三不出对子

               // var strArr = afterThree.Split(new char[] { ',' });

                if (this.IsHasPair(afterThree) && onlyOne == false) {
                    return false;
                }

                return true;
            }
            return false;
        }


        /// <summary>
        /// 判断后三是否出现对子
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private bool IsHasPair(string afterThree) {
            string[] strArr = afterThree.Split(new char[] { ',' });
            bool rlt = false;
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

            foreach (var key in occurCountDic.Keys)
            {
                if (occurCountDic[key] == 2)
                {
                    return true;
                }
            }

            return rlt;

        }






    }
}
