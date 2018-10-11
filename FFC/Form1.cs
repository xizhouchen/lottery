using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

using RestSharp;
using CommonFunction;
using Lottery.Model;

namespace Lottery.FFC
{
    public partial class Form1 : Form
    {
        private string _cookie;

        private string _loginCookie;

        private Lottery.Business.LotteryBusiness _lb = new Business.LotteryBusiness();

        public static readonly CookieContainer CookieContainer = new CookieContainer();

        private string GetAccountBalanceURL = "https://www.blgj02.com/controller/user/get/get_user_balance/964896";

        private string LotteryURl = "https://www.blgj02.com/controller/lottery/chart";

        private string PayUrl = "https://www.blgj02.com/controller/lottery/964896";

        private string predictNum = "11111111"; //下一期彩票的编号 如201809070980

        private List<DB_PredictRecord> records = new List<DB_PredictRecord>();

        private List<DB_PredictRecord> failedRecords = new List<DB_PredictRecord>();

        private List<string> failedNums = new List<string>();

        private List<string> winNums = new List<string>();

        private LISTItem lastRecord = new LISTItem(); //最新的一期

        private bool IsAutoPay = false;

        private Dictionary<int, double> PayMoneys = new Dictionary<int, double>();

        private Dictionary<int, double> QQMoneys = new Dictionary<int, double>();

        private int BatchId = 0;

        private int lottery_Id = 10014;

        private int failedCount = 0;

        private int chaseCount = 1;

        private Business.Model_OnlyPair OnlyPairModel = new Business.Model_OnlyPair(null);

        private string minjueQQMinURL = "http://mj.gud5100.com/api/game-lottery/query-trend";

       // private string minjueCookie = "SESSION=590458bc-4298-4b48-835c-8f15d2dbfbfd";

        private string minjuePost = "name=qqmin&query=latest-30";

        private bool _isLogin = false;

        private Business.Model_Cycle_KillTwoNum OnlyKill2Model = new Business.Model_Cycle_KillTwoNum(null);

        private double StopWin = 1;

        private double StopFailed = 1;

        private double OriginBalance;

        private double BasePayPoint = 0.3344;

        private double PayPoint = 0;

        private double FailedTimes = 0;

        private double WinTimes = 0;

        private string QQPayUrl = "http://mj.gud5100.com/api/game-lottery/add-order";

        private double PayCount = 0;

        private DateTime autoStartTime = DateTime.Now;

        private DateTime autoEndTime = DateTime.Now;

        private CanBePayPoint _goodPoint;

        public Form1()
        {
            InitializeComponent();
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "qihao", HeaderText = "期号" });
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "goodNo", HeaderText = "中奖号码" });
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "status", HeaderText = "状态" });
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ispay", HeaderText = "是否下单" });
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "pay", HeaderText = "支付" });
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "earn", HeaderText = "奖金" });
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "balance", HeaderText = "余额" });

            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "killno1", HeaderText = "杀号1" });
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn() { Name = "killno2", HeaderText = "杀号2" });
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.timer1.Start();
            this.timer1.Interval = 3000;

            this.timer2.Start();
            this.timer2.Interval = 2000;

            PayMoneys[1] = 1.680;
            PayMoneys[2] = 3.360;
            PayMoneys[3] = 6.720;
            PayMoneys[4] = 15.120;
            PayMoneys[5] = 30.240;
            PayMoneys[6] = 63.840;
            PayMoneys[7] = 131.040;
            PayMoneys[8] = 273.840;
            PayMoneys[9] = 566.160;
            PayMoneys[10] = 1176.000;
            PayMoneys[11] = 2441.040;
            PayMoneys[12] = 5065.200;
            //this.IsAutoPay = true;

            QQMoneys[1] = 0.056;

         
        
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.radioDown.Checked = true;
                this.comboBox1.SelectedIndex = 0;
                //1. 获取登录页面
                Random dom = new Random();
                var dd = (double)dom.Next() / (double)100;

                string loginGetUrl = "http://mj.gud5100.com/api/utils/login-security-code?" + dd;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(loginGetUrl);
                req.Method = "GET";// POST OR GET， 如果是GET, 则没有第二步传参，直接第三步，获取服务端返回的数据
                req.AllowAutoRedirect = false;//服务端重定向。一般设置false
                req.ContentType = "application/x-www-form-urlencoded";//数据一般设置这个值，除非是文件上传
                req.Accept = "image/webp,image/apng,image/*,*/*;q=0.8";
                //Stream postDataStream = req.GetRequestStream();
                //postDataStream.Write(postBytes, 0, postBytes.Length);
                //postDataStream.Close();
                req.Timeout = 5000;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string cookie = resp.Headers.Get("Set-Cookie");//获取登录后的cookie值。
                var resStream = resp.GetResponseStream();

                System.Drawing.Image result = System.Drawing.Image.FromStream(resStream);

                Bitmap bit = new Bitmap(result);
                //2. 获取验证码
                this.pictureBox1.Image = bit;
                _cookie = cookie;
            }
            catch (Exception ex) {

                LogHelper.ErrorLog(ex.Message, ex);

            }
           

        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            

        }

        private List<DB_PredictRecord> Convert(List<ResultItem> rltList) {
            List<DB_PredictRecord> records = new List<DB_PredictRecord>();
            foreach (var item in rltList) {
                records.Add(new DB_PredictRecord()
                {
                    GoodNo = item.code,
                    IssueId = item.issue
                });
            }
            records = records.Select(r => r).OrderBy(r => r.IssueId).ToList();
            return records;
        }

        private void AddAutoRecord() {

            double payTotal = 0;
            int payCount = 0;
            int payWinTime = 0;
            int payFailedTime = 0;
            double lowestBalance = 1000000;

            foreach (var item in this.records) {
               
                if (item.IsPay == "已下单")
                {
                    if (lowestBalance >= item.Balance)
                    {
                        lowestBalance = item.Balance;
                    }
                    payCount++;
                    payTotal = payTotal + item.PayAmount;
                    if (item.Status == "已中奖")
                    {
                        payWinTime++;
                    }
                    else {
                        payFailedTime++;
                    }
                }
            }
            int isUp = 0;
            if (radioUp.Checked == true) {
                isUp = 1;
            }
            Lottery.Business.LotteryBusiness lb = new Business.LotteryBusiness();
            lb.InsertAutoRecord(payCount, payTotal, payWinTime, payFailedTime, this.BasePayPoint,
                this.autoStartTime, this.autoEndTime, lowestBalance, isUp, txtUserName.Text,this.OriginBalance,this.GetBalance());


        }

        private void timer1_Tick(object sender, EventArgs e)
        {


            if (!_isLogin) {
                return;
            }

            timer1.Stop();

            //刷新计算下单点
            var his = _lb.GetLastRecordsByPage();
            his = his.OrderBy(r => r.IssueId).ToList();
            OnlyKill2Model.History = his;
            if (_goodPoint == null)
            {
                _goodPoint = OnlyKill2Model.FindPayPoint();
                if (_goodPoint != null)
                {
                    lblstopissue.Text = _goodPoint.StopIssueId;
                    lblRestPayCount.Text = _goodPoint.RestPayCount.ToString();
                    this.Switch();
                }
            }


            var lottery_id = 10014;
            try
            {
                var res = Util.getURLResponseStr(minjueQQMinURL, _cookie, minjuePost);
                List<LISTItem> lotteryRlts = new List<LISTItem>();
                var qqrlt = JsonConvert.DeserializeObject<Res_QQminResult>(res);
                foreach (var qqItem in qqrlt.data.result) {
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                    DateTime dt = startTime.AddMilliseconds(qqItem.openTime);
                    lotteryRlts.Add(new LISTItem
                    {
                        CP_ID = "腾讯分分彩",
                        CP_QS = qqItem.issue,
                        ZJHM = qqItem.code,
                        SJKJSJ = dt.ToString("yyyyMMdd hh:mm:ss"),
                        KJSJ = dt.ToString("yyyyMMdd hh:mm:ss")

                    });
                }

               
                this.dataGridView1.DataSource = lotteryRlts;
                this.dataGridView1.Columns[0].HeaderCell.Value = "开奖号码";
                this.dataGridView1.Columns[1].HeaderCell.Value = "期号";
                this.dataGridView1.Columns[2].HeaderCell.Value = "开奖时间";
                this.dataGridView1.Columns[3].HeaderCell.Value = "实际开奖时间";
                this.dataGridView1.Columns[4].HeaderCell.Value = "彩票类型ID";


                DB_PredictRecord dr = new DB_PredictRecord();
                var nums = OnlyKill2Model.GetTwoKillNo(qqrlt.data.result.Count, this.Convert(qqrlt.data.result),ref dr);//Util.PredictResult(lotteryRlts);

                foreach (var item in lotteryRlts)
                {
                    if (item.CP_QS == predictNum)
                    {
                        lastRecord = item;
                    }
                }
                var afterspli = lotteryRlts[0].CP_QS.Split(new char[] { '-' });
                var seq = afterspli[0] + afterspli[1];
                predictNum = (Int64.Parse(seq) + 1).ToString();

                if (predictNum.Contains("1440")) {
                    DateTime date = DateTime.Parse(lotteryRlts[0].CP_QS.Substring(0, 4) + "-" + lotteryRlts[0].CP_QS.Substring(4, 2) + "-" + lotteryRlts[0].CP_QS.Substring(6, 2));

                    predictNum = date.AddDays(1).ToString("yyyyMMdd") + "0001";
                }
                predictNum = predictNum.Substring(0, 8) + "-" + predictNum.Substring(8, 4);
                label2.Text = nums[0];
                label3.Text = nums[1];
                var selectNum = "";
                for (var i = 0; i <= 9; i++)
                {
                    if (i.ToString() != label2.Text && i.ToString() != label3.Text)
                    {
                        if (selectNum.Length != 14)
                        {
                            selectNum += i.ToString() + ",";
                        }
                        else {
                            selectNum += i.ToString();
                        }
                       
                    }
                }
                label6.Text = selectNum;

                this.label1.Visible = true;
                this.label2.Visible = true;
                this.label3.Visible = true;
                this.label4.Visible = true;
                this.label5.Visible = true;
                this.label6.Visible = true;
                this.button2.Visible = true;
               
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog(ex.Message, ex);

            }
            finally {
                timer1.Start();
            }
            
          
        }
        private void refreshStatus() {
            if (IsAutoPay == false)
            {
                chaseCount = 1;
                failedCount = 0;

                this.txtLossStop.Enabled = true;
                this.txtStopWin.Enabled = true;
                this.txtPayPoint.Enabled = true;
                this.comboBox1.Enabled = true;

                this.radioDown.Enabled = true;
                this.radioUp.Enabled = true;
                //开启时还原到0



                this.autoEndTime = DateTime.Now;


                AddAutoRecord();

                //PayCount = 0;
                //lblpaytotal.Text = "0";
                //lblflow.Text = "0";
                this.button2.BackColor = System.Drawing.SystemColors.ActiveCaption;
                
                
                //  this.button2.bak == Color.Act
                //lblWinRate.Text = "0%";
                //WinTimes = 0;
                //FailedTimes = 0;
            }
            else
            {
                this.txtLossStop.Enabled = false;
                this.txtStopWin.Enabled = false;
                this.txtPayPoint.Enabled = false;
                this.comboBox1.Enabled = false;
                this.radioDown.Enabled = false;
                this.radioUp.Enabled = false;
                //关闭后刷新原始金额
                this.OriginBalance = this.GetBalance();
                this.lblOrBalance.Text = this.OriginBalance.ToString();
                this.lblBalance.Text = this.OriginBalance.ToString();
                this.button2.BackColor = Color.Red;

                this.autoStartTime = DateTime.Now;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblLoginStatus.Text == "登录成功")
                {
                    if (_goodPoint == null && IsAutoPay == false) {
                        MessageBox.Show("当前期不是下单点，如果为下单点会自动开启下单");
                        return;
                    }
                    
                    IsAutoPay = !IsAutoPay;
                    this.button2.Text = IsAutoPay != true ? "开启下单" : "关闭下单";

                    refreshStatus();
                }
                else
                {
                    MessageBox.Show("请先登录！");
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + ": 输入有错误");
                IsAutoPay = false;
            }
           

           

        }


        private bool PayQQBill(string model = "li",int multiple = 1,string content = "0,1,2,3,4,5,6,7") {
            bool isSuccess = false;
            var data = System.Web.HttpUtility.UrlEncode("[{\"lottery\":\"qqmin\",\"issue\":\"\",\"method\":\"sxzuxzlh\",\"content\":\""+content+"\",\"model\":\""+model+"\",\"multiple\":"+multiple+",\"code\":1976,\"compress\":false}]");
            var postParam = @"text=%5B%7B%22lottery%22%3A%22qqmin%22%2C%22issue%22%3A%22%22%2C%22method%22%3A%22sxzuxzlh%22%2C%22content%22%3A%220%2C1%2C2%2C3%2C4%2C5%2C6%2C7%22%2C%22model%22%3A%22li%22%2C%22multiple%22%3A1%2C%22code%22%3A1976%2C%22compress%22%3Afalse%7D%5D";
            postParam = "text=" + data;
            LogHelper.InfoLog(DateTime.Now.ToString() + "  -- " + postParam);

            var res = Util.getURLResponseStr(QQPayUrl, _loginCookie, postParam);
            if (res.Contains("请求成功")) {
                isSuccess = true;
            }


            LogHelper.InfoLog(DateTime.Now.ToString() + "  -- " + res);
            return isSuccess;
        } 

        //private Res_Pay PayBill(double money) {
        //    var payCount = 0;
        //    if (money == 1.68)
        //    {
        //        payCount = 1;
        //    }
        //    else if (money == 3.36) {

        //        payCount = 2;
        //    }
        //    else if (money == 6.72)
        //    {

        //        payCount = 4;
        //    }
        //    else if (money == 15.12)
        //    {

        //        payCount = 9;
        //    }
        //    else if (money == 30.24)
        //    {

        //        payCount = 18;
        //    }
        //    else if (money == 63.84)
        //    {

        //        payCount = 38;
        //    }
        //    else if (money == 131.04)
        //    {

        //        payCount = 78;
        //    }
        //    else if (money == 273.84)
        //    {

        //        payCount = 163;
        //    }
        //    else if (money == 566.16)
        //    {

        //        payCount = 337;
        //    }
        //    else if (money == 1176)
        //    {

        //        payCount = 700;
        //    }
        //    else if (money == 2441.04)
        //    {

        //        payCount = 1453;
        //    }
        //    else if (money == 5065.20)
        //    {

        //        payCount = 3015;
        //    }

        //    //var cookie = this.textBox1.Text;
        //    var postParam = @"command=lottery_logon_request_transmit_v2&param=%7B%22command_id%22%3A521%2C%22lottery_id%22%3A%2210014%22%2C%22issue%22%3A%22"+predictNum+"%22%2C%22count%22%3A1%2C%22bet_info%22%3A%5B%7B%22method_id%22%3A%22150042%22%2C%22number%22%3A%220123456789%2C0123456789%22%2C%22rebate_count%22%3A75%2C%22multiple%22%3A%22"+payCount+"%22%2C%22mode%22%3A3%2C%22bet_money%22%3A%22"+ money + "%22%2C%22calc_type%22%3A%220%22%7D%5D%7D";

        //    LogHelper.InfoLog(DateTime.Now.ToString() + "  -- "   + postParam);
           
        //    var res = Util.getURLResponseStr(PayUrl, _loginCookie, postParam);

        //    LogHelper.InfoLog(DateTime.Now.ToString() + "  -- "  + res);

        //    return JsonConvert.DeserializeObject<Res_Pay>(res);

        //}


        private void UpdatePayMoneyAndChase(string goodNum,string ispay) {
            if (!failedNums.Contains(goodNum))
            {
                failedNums.Add(goodNum);
                failedCount++;
                if(ispay == "已下单")
                    chaseCount++;
            }
            else {

            }
        }

        private void UpdateFailed(string goodNum) {
            if (!failedNums.Contains(goodNum))
            {
                failedNums.Add(goodNum);
                FailedTimes++;
            }
        }

        private void UpdateWin(string goodNum) {
            if (!winNums.Contains(goodNum))
            {
                winNums.Add(goodNum);
                WinTimes++;
            }
        }

        private void Switch() {
            IsAutoPay = !IsAutoPay;
            this.button2.Text = IsAutoPay != true ? "开启下单" : "关闭下单";
            refreshStatus();
            timer2.Start();
            return;
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!_isLogin)
            {
                return;
            }
            timer2.Stop();

            //追12手追不到自动停
            //if (chaseCount > 12) {
            //    IsAutoPay = !IsAutoPay;
            //    this.button2.Text = IsAutoPay != true ? "开启自动下单" : "关闭自动下单";
            //    return;
            //}
            var bal = this.GetBalance();
            if (bal == 0) {
                timer2.Start();
                return;
            }

            try
            {
                this.StopWin = double.Parse(txtStopWin.Text);
                this.StopFailed = double.Parse(txtLossStop.Text);
            }
            catch (Exception ex) {
                LogHelper.ErrorLog("解析止损赢失败：" + ex.Message, ex);
            }

            //止损
            if ((bal - this.OriginBalance) >= this.StopWin) {
                Switch();
            }

            //止盈
            if (( this.OriginBalance - bal) >= this.StopFailed)
            {
                Switch();
            }

            if (_goodPoint != null && _goodPoint.RestPayCount <= 0)
            {
                Switch();
                _goodPoint = OnlyKill2Model.FindPayPoint();
                if (_goodPoint != null)
                {
                    lblstopissue.Text = _goodPoint.StopIssueId;
                }
                else
                {
                    //lblstopissue.Text = "-";
                }

            }
           

          



            if (label1.Visible == true)
            {


                if (lastRecord != null)
                {
                    var subRecords = records.Select(r => r).Where(r => r.IssueId == lastRecord.CP_QS).ToList();
                    if (subRecords.Count > 0)
                    {
                        subRecords[0].GoodNo = lastRecord.ZJHM;
                        subRecords[0].RewardDate = lastRecord.SJKJSJ;

                        var sub = lastRecord.ZJHM;
                        if (!OnlyKill2Model.CheckWin(subRecords[0].Kill3No,subRecords[0].Kill5No,sub))
                        {
                            subRecords[0].Status = "未中奖";

                            UpdateFailed(subRecords[0].GoodNo);

                            // UpdatePayMoneyAndChase(lastRecord.CP_QS, subRecords[0].IsPay);
                        }
                        else
                        {
                            subRecords[0].Status = "中奖";


                          
                            subRecords[0].Earn = subRecords[0].PayAmount * 2.94;
                            UpdateWin(subRecords[0].GoodNo);


                          
                        }

                     

                    }

                }

                var subRecords2 = records.Select(r => r).Where(r => r.IssueId == predictNum).ToList();
                if (subRecords2.Count == 0)
                {
                    var payItem = new DB_PredictRecord();
                    payItem.GoodNo = "";
                    payItem.IssueId = predictNum;
                    payItem.Status = "未开奖";
                    payItem.IsPay = "未下单";
                    payItem.Kill3No = label2.Text;
                    payItem.Kill5No = label3.Text;

                    try
                    {
                       
                        
                        if (IsAutoPay && _goodPoint!= null && _goodPoint.RestPayCount > 0) {
                            int mul = int.Parse(this.comboBox1.SelectedItem.ToString());
                            var bill = PayQQBill("li", mul, this.label6.Text);
                            payItem.PayAmount = QQMoneys[1]*mul;
                            payItem.IsPay = "已下单";
                            payItem.Balance = this.GetBalance();
                            lblBalance.Text = payItem.Balance.ToString();
                            PayCount = PayCount + 1;
                            lblpaytotal.Text = PayCount.ToString();
                            lblflow.Text = (PayCount * payItem.PayAmount).ToString();
                            _goodPoint.RestPayCount = _goodPoint.RestPayCount - 1;
                            lblRestPayCount.Text = _goodPoint.RestPayCount.ToString();
                        }

                        records.Add(payItem);
                    }
                    catch (Exception ex)
                    {
                        chaseCount = 1;
                        failedCount = 0;
                        payItem.IsPay = "网站出问题，下单失败";
                        LogHelper.ErrorLog(DateTime.Now.ToString() + " -- 下单失败  -- "  + ex.Message,ex);
                    }
                }

                List<DB_PredictRecord> newRecords = new List<DB_PredictRecord>();
                for (var j = records.Count - 1; j >= 0; j--)
                {
                    newRecords.Add(records[j]);
                }

                for (var j = 0; j < newRecords.Count; j++)
                {
                    bool exsit = false;
                    for (var i = 0; i < this.dataGridView2.Rows.Count; i++)
                    {
                          
                            if (this.dataGridView2.Rows[i].Cells[0].Value.ToString() == newRecords[j].IssueId.ToString())
                            {
                                exsit = true;
                                this.dataGridView2.Rows[i].Cells["goodNo"].Value = newRecords[j].GoodNo;
                                this.dataGridView2.Rows[i].Cells["status"].Value = newRecords[j].Status;
                                this.dataGridView2.Rows[i].Cells["ispay"].Value = newRecords[j].IsPay;
                                this.dataGridView2.Rows[i].Cells["pay"].Value = newRecords[j].PayAmount;
                                this.dataGridView2.Rows[i].Cells["earn"].Value = newRecords[j].Earn;
                                this.dataGridView2.Rows[i].Cells["balance"].Value = newRecords[j].Balance;
                        }
                     }
                    if (!exsit)
                    {

                        int index = this.dataGridView2.Rows.Add();
                        this.dataGridView2.Rows[index].Cells[0].Value = newRecords[0].IssueId;
                        this.dataGridView2.Rows[index].Cells[1].Value = newRecords[0].GoodNo;
                        this.dataGridView2.Rows[index].Cells[2].Value = newRecords[0].Status;
                        this.dataGridView2.Rows[index].Cells[3].Value = newRecords[0].IsPay;
                        this.dataGridView2.Rows[index].Cells["killno1"].Value = newRecords[0].Kill3No;
                        this.dataGridView2.Rows[index].Cells["killno2"].Value = newRecords[0].Kill5No;

                    }

                }


               

                for (var i = 0; i < this.dataGridView2.Rows.Count; i++)
                {
                    if (this.dataGridView2.Rows[i].Cells["status"].Value.ToString() == "中奖")
                    {
                        this.dataGridView2.Rows[i].Cells["status"].Style.BackColor = Color.Green;
                    }
                    else
                    {
                        this.dataGridView2.Rows[i].Cells["status"].Style.BackColor = Color.Red;
                    }

                    if (this.dataGridView2.Rows[i].Cells["ispay"].Value.ToString() == "未下单")
                    {
                        this.dataGridView2.Rows[i].Cells["ispay"].Style.BackColor = Color.Yellow;
                    }

                }
            }

            timer2.Start();
        }

        int seed = 1;
        /// <summary>
        /// 模拟发单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            seed++;
            predictNum = (11111111 + seed).ToString();
        }
        
        /// <summary>
        /// 模拟开奖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            lastRecord = new LISTItem();
            lastRecord.CP_QS = predictNum;
            lastRecord.SJKJSJ = "2222222";
            lastRecord.ZJHM = textBox2.Text;

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //3. post 登录数据

            // var postLoginUrl = "https://www.blgj02.com/login";
            var postLoginUrl = "http://mj.gud5100.com/api/web-login";

            HttpWebRequest request = HttpWebRequest.CreateHttp(postLoginUrl);
            // request.Headers["accept"] = "*/*";
            request.Accept = "*/*";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            // request.Headers["connection"] = "Keep-Alive";
            //  request.Connection = "Keep-Alive";
            request.KeepAlive = true;
            request.Method = "POST";
            //request.Headers["user-agent"] = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1;SV1)";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1;SV1)";
            //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
            if (!string.IsNullOrEmpty(_cookie))
            {
                request.Headers.Add("Cookie", _cookie);
            }
            //resp = (HttpWebResponse)request.GetResponse();
            //resp.GetResponseStream();
            var validCode = this.txtvalidCode.Text;
            //var postStr = "user_name=" + txtUserName.Text + "&password=" + txtPassword.Text + "&valid_code=" + validCode + "&g_code=&wechat_web=web&wechat_app=app&u_g=0&command=login";
            var postStr = "username="+txtUserName.Text+"&password="+txtPassword.Text+"&securityCode="+txtvalidCode.Text+"";
            if (postStr != null)
            {
                using (var reqc = request.GetRequestStream())
                {
                    var bs = Encoding.UTF8.GetBytes(postStr);
                    reqc.Write(bs, 0, bs.Length);
                }
            }

            var rsp = (HttpWebResponse)request.GetResponse();

            var stream = rsp.GetResponseStream();
            _loginCookie = rsp.Headers.Get("Set-Cookie");//获取登录后的cookie值。
            if (string.IsNullOrEmpty(_loginCookie)) {
                _loginCookie = _cookie;
            }
            List<byte> bytes = new List<byte>();
            int temp = stream.ReadByte();
            while (temp != -1)
            {
                bytes.Add((byte)temp);
                temp = stream.ReadByte();
            }
            var byteFile = bytes.ToArray();



            //stream.Write(bytes, 0, (int)stream.Length);
            string rspStr = Encoding.GetEncoding("UTF-8").GetString(byteFile);
            //Res_LoginResult rlt = new Res_LoginResult();
            //try
            //{
            //    rlt = JsonConvert.DeserializeObject<Res_LoginResult>(rspStr);
            //}
            //catch (Exception ex)
            //{

            //    MessageBox.Show("登录失败： " + rspStr + " 异常：" + ex.Message);
            //}

            if (rspStr.Contains("请求成功"))
            {
                lblLoginStatus.Text = "登录成功";
                lblLoginStatus.ForeColor = Color.Green;
             //   lblBalance.Text = rlt.data.money.ToString();
                lblBalance.ForeColor = Color.Orange;
                lblOrBalance.ForeColor = Color.Orange;
                //  lblUserId.Text = rlt.data.user_id;
                OriginBalance = this.GetBalance();
                lblBalance.Text = OriginBalance.ToString();
                lblOrBalance.Text = OriginBalance.ToString();
                _isLogin = true;
            }
            else
            {
                lblLoginStatus.Text = "登录失败";
                lblLoginStatus.ForeColor = Color.Red;
            }
            //return rspStr;
        }

        private double GetBalance() {
            double balance = 0;
            try
            {
                var baUrl = "http://mj.gud5100.com/api/web-ajax/loop-page";
                var baRes = Util.getURLResponseStr(baUrl, _cookie);
                var baRlt = JsonConvert.DeserializeObject<Res_Minjue_BalanceResult>(baRes);
                balance = baRlt.data.lotteryBalance;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("获取余额失败：" + ex.Message, ex);
                return balance;
            }
            return balance;

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.PayQQBill();
        }
    }
}
