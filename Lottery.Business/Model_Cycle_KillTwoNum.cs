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

       
      

        public override List<CanBePayPoint> FindPayPoints(int splitNum = 455)
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
                    var x = (splitNum - wincount) * 0.16464;
                    var y = 0.056 * (1440 - passcount);
                    if (x - y > 0)
                    {

                        CanBePayPoint p = new CanBePayPoint();
                        p.StartIssueId = an[i].IssueId;
                        p.StartDate = an[i].RewardDate;
                        p.RestPayCount = 1440 - passcount;
                        p.PayIssueId = an[j].IssueId;
                        p.PayDate = an[j].RewardDate;
                        p.PassCount = passcount;
                        p.WinCount = wincount;
                        if (p.RestPayCount > 0) {
                            payPoints.Add(p);
                        }
                     
                    }

                }
            }
            payPoints = payPoints.OrderByDescending(r => r.StartIssueId).ToList();
            return payPoints;

        }

        public override CanBePayPoint FindPayPoint() {
            List<CanBePayPoint> payPoints = new List<CanBePayPoint>();
            var an = this.GenerateAmountResult(1, 1, 1, 1, BetModel.KillTowNo, 1);
          
            for (var i = 0; i <= an.Count - 1; i++) {
                int wincount = 0;
                int failedcount = 0;
                int passcount = 0;
                for (var j = i; j <= an.Count - 1; j++) {
                    passcount++;
                    if (an[j].Status == "中奖")
                    {
                        wincount++;
                    }
                    else {
                        failedcount++;
                    }
                    var x = (455 - wincount) * 0.16464;
                    var y = 0.056 * (1440 - passcount);
                    if (x - y > 0) {
                       
                        CanBePayPoint p = new CanBePayPoint();
                        p.StartIssueId = an[i].IssueId;
                        p.RestPayCount = 1440 - passcount;
                        p.PayIssueId = an[j].IssueId;
                        p.PassCount = passcount;
                        p.WinCount = wincount;
                        if (an[j].IssueId == an[an.Count - 1].IssueId) {
                            payPoints.Add(p);
                        }
                        
                        break;
                    }
                   
                }
            }
            payPoints = payPoints.OrderByDescending(r => r.StartIssueId).ToList();
            if (payPoints.Count > 0) {
                return payPoints[0];
            }
            return null;
            
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



                if (IsWinByKillTwoNum(i, this.History, ref item)) {
                    item.Status = "中奖";
                } else {
                    item.Status = "未中奖";
                }
               
                betList.Add(item);
            }
            return betList;


        }

        public bool IsWinByKillTwoNum(int payIndex,List<DB_PredictRecord> records,ref DB_PredictRecord item) {
            if (!string.IsNullOrEmpty(this.KillNo1) && !string.IsNullOrEmpty(this.KillNo2)) {
                item.Kill3No = this.KillNo1;
                item.Kill5No = this.KillNo2;
                return CheckWin(this.KillNo1, this.KillNo2, records[payIndex].GoodNo);
            }
            

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
                if (GetThreeNo(records[payIndex - 1].GoodNo).Contains(key.ToString()) && GetThreeNo(records[payIndex - 2].GoodNo).Contains(key.ToString())) {
                    rlt.Add(key.ToString());
                }
            }

            if (rlt.Count > 1) {
                item.Kill3No = rlt[0];
                item.Kill5No = rlt[1];
                return CheckWin(rlt[0], rlt[1], records[payIndex].GoodNo);
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
              
                return CheckWin(firstNum, secondNum, records[payIndex].GoodNo);
            }
            //一个号都不能取到，选择冷号集合(两个冷号)
            else {
                firstNum = SelectColdNo(payIndex, records,null,true);
                secondNum = SelectColdNo(payIndex, records,firstNum,true);
                item.Kill3No = firstNum;
                item.Kill5No = secondNum;
              
                return CheckWin(firstNum, secondNum, records[payIndex].GoodNo);
            }
         
        }

        public List<string> GetTwoKillNo(int payIndex, List<DB_PredictRecord> records, ref DB_PredictRecord item) {
            List<string> nums = new List<string>();
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

            foreach (var key in ne.Keys)
            {
                if (GetThreeNo(records[payIndex - 1].GoodNo).Contains(key.ToString()) && GetThreeNo(records[payIndex - 2].GoodNo).Contains(key.ToString()))
                {
                    rlt.Add(key.ToString());
                }
            }

            if (rlt.Count > 1)
            {
                item.Kill3No = rlt[0];
                item.Kill5No = rlt[1];
                nums.Add(item.Kill3No);
                nums.Add(item.Kill5No);
                //return CheckWin(rlt[0], rlt[1], records[payIndex].GoodNo, true);
            }
            var firstNum = "";
            var secondNum = "";
            //能取到一个号
            if (rlt.Count == 1)
            {
                firstNum = rlt[0];
                secondNum = SelectColdNo(payIndex, records, null, true);
                item.Kill3No = firstNum;
                item.Kill5No = secondNum;
                nums.Add(item.Kill3No);
                nums.Add(item.Kill5No);
                //return CheckWin(firstNum, secondNum, records[payIndex].GoodNo, true);
            }
            //一个号都不能取到，选择冷号集合(两个冷号)
            else
            {
                firstNum = SelectColdNo(payIndex, records, null, true);
                secondNum = SelectColdNo(payIndex, records, firstNum, true);
                item.Kill3No = firstNum;
                item.Kill5No = secondNum;
                nums.Add(item.Kill3No);
                nums.Add(item.Kill5No);
                //return CheckWin(firstNum, secondNum, records[payIndex].GoodNo, true);
            }

            return nums;
        }

        public bool CheckWin(string num1, string num2, string goodNo,bool onlyOne = false)
        {
            if (onlyOne == true) {
                num1 = "11111";
            }
            var afterThree = GetThreeNo(goodNo);
            if (!afterThree.Contains(num1) && !afterThree.Contains(num2)) {
                //后三不出对子

               // var strArr = afterThree.Split(new char[] { ',' });

                if (this.IsHasPairAndThree(afterThree) && onlyOne == false) {
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
        private bool IsHasPairAndThree(string afterThree) {
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
                if (occurCountDic[key] == 2 || occurCountDic[key] == 3)
                {
                    return true;
                }
            }

            return rlt;

        }






    }
}
