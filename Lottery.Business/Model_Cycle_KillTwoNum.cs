using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lottery.Model;
using Lottery.PlatForm;

namespace Lottery.Business
{
  
    public class Model_Cycle_KillTwoNum : IModel
    {
        private const int KillUsedIssueCount = 100;
       
        public int Critical { get; set; } // 
        public int Range { get; set; } //1440
       
        public Model_Cycle_KillTwoNum(List<DB_PredictRecord> history,int ct = 455,int rg = 1440) : base(history)
        {
            Critical = ct;
            Range = rg;
        }

        public Model_Cycle_KillTwoNum(List<DB_PredictRecord> history,string killno1,string killno2, int ct = 455, int rg = 1440) : base(history)
        {
            this.KillNo1 = killno1;
            this.KillNo2 = killno2;
            Critical = ct;
            Range = rg;
        }

        public override bool AddOrder(IPlatForm pf, List<DB_PredictRecord> last30Records,
            string model = "li",int mul = 1)
        {
            //return base.AddOrder(pf, last30Records);

            var selectNum = "";
            var killno1 = this.KillNo1;
            var killno2 = this.KillNo2;
            if (string.IsNullOrEmpty(killno1) && string.IsNullOrEmpty(killno2)) {
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
            var flag = pf.MakeOrder(model, mul, selectNum, pf.GetBackKillTwoMethodName());
            return flag;
        }

        public override List<CanBePayPoint> FindPayPoints(int lastNumber = 240, double payRate = 0.83)
        {
            //return base.FindPayPoints(lastNumber, payRate);
            List<CanBePayPoint> payPoints = new List<CanBePayPoint>();
            var an = this.GenerateAmountResult(1, 1, 1, 1, BetModel.KillTowNo, 1);

            for (var i = 0; i <= an.Count - (lastNumber + 1); i++) {
                double cycleWinNum = 0;
                double cycleTotalNum = 0;
                double cycleFailedNum = 0;
                for (var j = i; j < lastNumber + i; j++) {
                    var item = an[j];
                    if (item.Status == "中奖")
                    {
                        cycleWinNum++;
                        cycleTotalNum++;
                    }
                    else if (item.Status == "未中奖" && item.CirCleNum == 3)
                    {
                        cycleFailedNum++;
                        cycleTotalNum++;
                    }
                }
                if ((cycleWinNum / cycleTotalNum) <= payRate) {
                    //一天只取一个
                    bool isExist = false;
                    foreach (var existItem in payPoints) {
                        if (an[lastNumber + i].CreatedOn.Month == existItem.StartDateTime.Month &&
                            an[lastNumber + i].CreatedOn.Date == existItem.StartDateTime.Date) {
                            isExist = true;
                        }
                    }
                    if (!isExist) {
                        CanBePayPoint cpp = new CanBePayPoint();
                        cpp.PayIssueId = an[lastNumber + i].IssueId;
                        cpp.WinRate = (cycleWinNum / cycleTotalNum);
                        cpp.WinCount = (int)cycleWinNum;
                        cpp.PassCount = (int)cycleTotalNum;
                        cpp.StartDateTime = an[lastNumber + i].CreatedOn;
                        payPoints.Add(cpp);
                    }
                  
                }
            }

            //根据接下来的一天下单情况，算出是否盈利
            foreach (var item in payPoints) {
                LotteryBusiness lb = new LotteryBusiness();
                double balance = 1000;
                double maxLowBalance = 10000000;
                this.History = lb.GetLoRecordsByDate(item.StartDateTime.AddMinutes(-KillUsedIssueCount), item.StartDateTime.AddDays(1));
                var an2 = this.GenerateAmountResult(1, 1, 1, 1, BetModel.KillTowNo, 1);
                for (var j = 0; j < an2.Count; j++)
                {
                    var item2 = an2[j];
                    if (item2.Status == "中奖")
                    {
                        balance = balance + 4.85;
                    }
                    else if (item2.Status == "未中奖" && item2.CirCleNum == 3)
                    {
                        balance = balance - 38.4;
                    }
                    if (maxLowBalance > balance) {
                        maxLowBalance = balance;
                    }
                    if (balance <= 0) {
                        item.IsWin = "亏";
                        item.EndDateTime = item2.CreatedOn;
                       
                        break;
                    }
                    if (balance >= 1150) {
                        item.IsWin = "盈";
                        item.EndDateTime = item2.CreatedOn;
                        break;
                    }
                }
                item.Balance = balance;
                item.MaxBalance = maxLowBalance;
            }

            return payPoints;
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
                        if (p.RestPayCount >= 240) {
                            payPoints.Add(p);
                            break;
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
                    var x = (this.Critical - wincount) * 0.16464;
                    var y = 0.056 * (this.Range - passcount);
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
            for (var i = KillUsedIssueCount; i < this.History.Count; i++) {
               
                DB_PredictRecord item = new DB_PredictRecord();
                item.GoodNo = History[i].GoodNo;
                item.RewardDate = History[i].RewardDate;
                item.IssueId = History[i].IssueId;
                item.CreatedOn = History[i].CreatedOn;


                //if (IsWinByKillTwoNum(i, this.History, ref item)) {
                //    item.Status = "中奖";
                //} else {
                //    item.Status = "未中奖";
                //}

                if (IsWinByKillTwoNum_Individual(i, this.History, ref item))
                {
                    item.Status = "中奖";
                    count = 1;
                }
                else
                {
                    item.Status = "未中奖";
                    item.CirCleNum = count;
                    count++;
                }


                betList.Add(item);
            }
            return betList;


        }


        public string SelectCode(Dictionary<int,int> dic_1, Dictionary<int, int> dic_2, Dictionary<int, int> dic_3,int exceptNum = -1) {
            if (dic_1.ContainsKey(exceptNum)) {
                dic_1.Remove(exceptNum);
            }
            if (dic_2.ContainsKey(exceptNum)) {
                dic_2.Remove(exceptNum);
            }
            if (dic_3.ContainsKey(exceptNum)) {
                dic_3.Remove(exceptNum);
            }

            var max1 = -1;
            var maxCount = 0;
            foreach (var item in dic_1.Keys) {
                if (dic_1[item] > maxCount) {
                    max1 = item;
                    maxCount = dic_1[item];
                }
            }

            var max2 = -1;
            maxCount = 0;
            foreach (var item in dic_2.Keys)
            {
                if (dic_2[item] > maxCount)
                {
                    max2 = item;
                    maxCount = dic_2[item];
                }
            }

            var max3 = -1;
            maxCount = 0;
            foreach (var item in dic_3.Keys)
            {
                if (dic_3[item] > maxCount)
                {
                    max3 = item;
                    maxCount = dic_3[item];
                }
            }

            var max = 0;
            var maxKey = -1;
            if (max <= dic_1[max1]) {
                max = dic_1[max1];
                maxKey = max1;
            }

            if (max <= dic_2[max2]) {
                max = dic_2[max2];
                maxKey = max2;
            }

            if (max <= dic_3[max3]) {
                max = dic_3[max3];
                maxKey = max3;
            }

            return maxKey.ToString();
        }



        public List<string> GetTwoKillNo_Individual(int payIndex, List<DB_PredictRecord> records) {
            List<string> nums = new List<string>();
            Dictionary<int, int> Dic_1 = new Dictionary<int, int>();//从左开始数，百位
            Dictionary<int, int> Dic_2 = new Dictionary<int, int>();//从左开始数，十位
            Dictionary<int, int> Dic_3 = new Dictionary<int, int>();//从左开始数，个位
            for (var i = 0; i <= 9; i++)
            {
                var count_1 = 0;
                var count_2 = 0;
                var count_3 = 0;
                for (var j = payIndex - 1; j >= 0; j--)
                {
                   
                    var goodNo = records[j].GoodNo.ToString();
                    goodNo = GetThreeNo(goodNo);
                    var baiwei = goodNo.Substring(1, 1);
                    var shiwei = goodNo.Substring(3, 1);
                    var gewei = goodNo.Substring(5, 1);
                    if (baiwei.Contains(i.ToString()))
                    {
                        Dic_1.Add(i, count_1);
                        break;
                    }
                    else
                    {
                        count_1++;

                    }
                }
            }

            for (var i = 0; i <= 9; i++)
            {
                var count_1 = 0;
                var count_2 = 0;
                var count_3 = 0;
                for (var j = payIndex - 1; j >= 0; j--)
                {
                    var goodNo = records[j].GoodNo.ToString();
                    goodNo = GetThreeNo(goodNo);
                    var baiwei = goodNo.Substring(1, 1);
                    var shiwei = goodNo.Substring(3, 1);
                    var gewei = goodNo.Substring(5, 1);
                    if (shiwei.Contains(i.ToString()))
                    {
                        Dic_2.Add(i, count_2);
                        break;
                    }
                    else
                    {
                        count_2++;

                    }
                }
            }

            for (var i = 0; i <= 9; i++)
            {
                var count_1 = 0;
                var count_2 = 0;
                var count_3 = 0;
                for (var j = payIndex - 1; j >= 0; j--)
                {
                    var goodNo = records[j].GoodNo.ToString();
                    goodNo = GetThreeNo(goodNo);
                    var baiwei = goodNo.Substring(1, 1);
                    var shiwei = goodNo.Substring(3, 1);
                    var gewei = goodNo.Substring(5, 1);
                    if (gewei.Contains(i.ToString()))
                    {
                        Dic_3.Add(i, count_3);
                        break;
                    }
                    else
                    {
                        count_3++;

                    }
                }
            }


            var num1 = SelectCode(Dic_1, Dic_2, Dic_3);
            var num2 = SelectCode(Dic_1, Dic_2, Dic_3, int.Parse(num1));

            nums.Add(num1);
            nums.Add(num2);
            return nums;
        }
        /// <summary>
        /// 后三位独立运算
        /// </summary>
        /// <param name="payIndex"></param>
        /// <param name="records"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool IsWinByKillTwoNum_Individual(int payIndex, List<DB_PredictRecord> records, ref DB_PredictRecord item) {
            var flag = true;

            var mums = this.GetTwoKillNo_Individual(payIndex, records);
            item.Kill3No = mums[0];
            item.Kill5No = mums[1];
            return CheckWin(mums[0], mums[1], records[payIndex].GoodNo);
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

                //if (this.IsHasPairAndThree(afterThree) && onlyOne == false) {
                //    return false;
                //}

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
