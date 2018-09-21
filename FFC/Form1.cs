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

        public static readonly CookieContainer CookieContainer = new CookieContainer();

        private string GetAccountBalanceURL = "https://www.blgj02.com/controller/user/get/get_user_balance/964896";

        private string LotteryURl = "https://www.blgj02.com/controller/lottery/chart";

        private string PayUrl = "https://www.blgj02.com/controller/lottery/964896";

        private string predictNum = "11111111"; //下一期彩票的编号 如201809070980

        private List<DB_PredictRecord> records = new List<DB_PredictRecord>();

        private List<DB_PredictRecord> failedRecords = new List<DB_PredictRecord>();

        private List<string> failedNums = new List<string>();

        private LISTItem lastRecord = new LISTItem(); //最新的一期

        private bool IsAutoPay = false;

        private Dictionary<int, double> PayMoneys = new Dictionary<int, double>();

        private int BatchId = 0;

        private int lottery_Id = 10014;

        private int failedCount = 0;

        private int chaseCount = 1;

        private Business.Model_OnlyPair OnlyPairModel = new Business.Model_OnlyPair(null);

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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.radioButton1.Text = "分分彩";
            //this.radioButton2.Text = "1分半彩";

            //获取最大批次号
            //var sql = "select max(BatchId) from [Lottery].[dbo].[PredictRecord]";
            //var dataTable = SQLHelper.GetDataTable(sql, null);
            //if (dataTable.Rows.Count > 0) {
            //    var bat = dataTable.Rows[0][0].ToString();
            //    this.BatchId = int.Parse(bat);
            //    this.BatchId = this.BatchId + 1;
            //}

            //1. 获取登录页面
            Random dom = new Random();
            var dd = (double)dom.Next() / (double)100;

            string loginGetUrl = "https://www.blgj02.com/valid_code?t=" + dd + "&user_name=";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(loginGetUrl);
            req.Method = "GET";// POST OR GET， 如果是GET, 则没有第二步传参，直接第三步，获取服务端返回的数据
            req.AllowAutoRedirect = false;//服务端重定向。一般设置false
            req.ContentType = "application/x-www-form-urlencoded";//数据一般设置这个值，除非是文件上传
            req.Accept = "image/webp,image/apng,image/*,*/*;q=0.8";
            //Stream postDataStream = req.GetRequestStream();
            //postDataStream.Write(postBytes, 0, postBytes.Length);
            //postDataStream.Close();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            string cookie = resp.Headers.Get("Set-Cookie");//获取登录后的cookie值。
            var resStream = resp.GetResponseStream();

            System.Drawing.Image result = System.Drawing.Image.FromStream(resStream);

            Bitmap bit = new Bitmap(result);
            //2. 获取验证码
            this.pictureBox1.Image = bit;
            _cookie = cookie;


            //this.dataGridView2.Columns[0].HeaderCell.Value = "期号";
            //this.dataGridView2.Columns[1].HeaderCell.Value = "五星杀号";
            //this.dataGridView2.Columns[2].HeaderCell.Value = "后三杀号";
            //this.dataGridView2.Columns[3].HeaderCell.Value = "中奖号码";
            //this.dataGridView2.Columns[4].HeaderCell.Value = "状态";
            //this.dataGridView2.Columns[5].HeaderCell.Value = "是否下单";

        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            var lottery_id = 10014;
            try
            {
                var res = Util.getURLResponseStr(LotteryURl, null, "content=%7B%22command_id%22%3A23%2C%22lottery_id%22%3A%22" + lottery_id + "%22%2C%22issue_status%22%3A%221%22%2C%22count%22%3A%2230%22%7D&command=lottery_request_transmit_v2");
                var rlt = JsonConvert.DeserializeObject<Res_LotteryResult>(res);
              
                this.dataGridView1.DataSource = rlt.data.detail.LIST;
                this.dataGridView1.Columns[0].HeaderCell.Value = "开奖号码";
                this.dataGridView1.Columns[1].HeaderCell.Value = "期号";
                this.dataGridView1.Columns[2].HeaderCell.Value = "开奖时间";
                this.dataGridView1.Columns[3].HeaderCell.Value = "实际开奖时间";
                this.dataGridView1.Columns[4].HeaderCell.Value = "彩票类型ID";

                var nums = Util.PredictResult(rlt.data.detail.LIST);

                foreach (var item in rlt.data.detail.LIST)
                {
                    if (item.CP_QS == predictNum)
                    {
                        lastRecord = item;
                    }
                }

                predictNum = (long.Parse(rlt.data.detail.LIST[0].CP_QS) + 1).ToString();

                if (predictNum.Contains("1920")) {
                    DateTime date = DateTime.Parse(rlt.data.detail.LIST[0].CP_QS.Substring(0, 4) + "-" + rlt.data.detail.LIST[0].CP_QS.Substring(4, 2) + "-" + rlt.data.detail.LIST[0].CP_QS.Substring(6, 2));

                    predictNum = date.AddDays(1).ToString("yyyyMMdd") + "0001";
                }
                label2.Text = nums[0];
                label3.Text = nums[1];
                var selectNum = "";
                for (var i = 0; i <= 9; i++)
                {
                    if (i.ToString() != label2.Text && i.ToString() != label3.Text)
                    {
                        selectNum += i.ToString();
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

        private void button2_Click(object sender, EventArgs e)
        {
            
            if (lblLoginStatus.Text == "登录成功")
            {
                IsAutoPay = !IsAutoPay;
                this.button2.Text = IsAutoPay != true ? "开启自动下单" : "关闭自动下单";


                if (IsAutoPay == false)
                {
                    chaseCount = 1;
                    failedCount = 0;
                }
            }
            else {
                MessageBox.Show("请先登录！");
            }

           

        }

        private Res_Pay PayBill(double money) {
            var payCount = 0;
            if (money == 1.68)
            {
                payCount = 1;
            }
            else if (money == 3.36) {

                payCount = 2;
            }
            else if (money == 6.72)
            {

                payCount = 4;
            }
            else if (money == 15.12)
            {

                payCount = 9;
            }
            else if (money == 30.24)
            {

                payCount = 18;
            }
            else if (money == 63.84)
            {

                payCount = 38;
            }
            else if (money == 131.04)
            {

                payCount = 78;
            }
            else if (money == 273.84)
            {

                payCount = 163;
            }
            else if (money == 566.16)
            {

                payCount = 337;
            }
            else if (money == 1176)
            {

                payCount = 700;
            }
            else if (money == 2441.04)
            {

                payCount = 1453;
            }
            else if (money == 5065.20)
            {

                payCount = 3015;
            }

            //var cookie = this.textBox1.Text;
            var postParam = @"command=lottery_logon_request_transmit_v2&param=%7B%22command_id%22%3A521%2C%22lottery_id%22%3A%2210014%22%2C%22issue%22%3A%22"+predictNum+"%22%2C%22count%22%3A1%2C%22bet_info%22%3A%5B%7B%22method_id%22%3A%22150042%22%2C%22number%22%3A%220123456789%2C0123456789%22%2C%22rebate_count%22%3A75%2C%22multiple%22%3A%22"+payCount+"%22%2C%22mode%22%3A3%2C%22bet_money%22%3A%22"+ money + "%22%2C%22calc_type%22%3A%220%22%7D%5D%7D";

            LogHelper.InfoLog(DateTime.Now.ToString() + "  -- "   + postParam);
           
            var res = Util.getURLResponseStr(PayUrl, _loginCookie, postParam);

            LogHelper.InfoLog(DateTime.Now.ToString() + "  -- "  + res);

            return JsonConvert.DeserializeObject<Res_Pay>(res);

        }


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
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            
            if (chaseCount > 12) {
                IsAutoPay = !IsAutoPay;
                this.button2.Text = IsAutoPay != true ? "开启自动下单" : "关闭自动下单";
                return;
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
                        if (!OnlyPairModel.OnlyPairModelBet(sub))
                        {
                            subRecords[0].Status = "未中奖";
                            UpdatePayMoneyAndChase(lastRecord.CP_QS, subRecords[0].IsPay);
                        }
                        else
                        {
                            subRecords[0].Status = "中奖";
                            subRecords[0].Earn = subRecords[0].PayAmount * 1.93;
                            failedCount = 0;
                            if (subRecords[0].IsPay == "已下单")
                            {
                                chaseCount = 1;
                            }
                        }

                        //var sql = "select * from [Lottery].[dbo].[PredictRecord] where issueId = '"+subRecords[0].IssueId+"'";
                        //var dataTable = SQLHelper.GetDataTable(sql, null);
                        //if (dataTable.Rows.Count == 0) {
                        //    sql = "insert into[Lottery].[dbo].[PredictRecord] values('"+ subRecords[0].IssueId + "','"+ 
                        //        subRecords[0].Kill5No+ "','"+subRecords[0].Kill3No+"','"+subRecords[0].GoodNo+"','"+subRecords[0].Status+"','"+ subRecords[0].RewardDate+ "',getdate(),"+this.BatchId+","+this.lottery_Id+")";
                        //    var rlt = SQLHelper.ExecNonQuery(sql, null);
                        //}

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

                    try
                    {
                        if (IsAutoPay && failedCount < 3)
                        {

                            //发送下单数据


                            var bill = PayBill(PayMoneys[chaseCount]);

                            payItem.IsPay = "已下单";
                            payItem.PayAmount = PayMoneys[chaseCount];
                            payItem.Balance = double.Parse(bill.data.balance);
                            lblBalance.Text = payItem.Balance.ToString();
                        }
                        //如果失败次数大等于3次，判断前一期是否正常，如果正常就可以下单
                        else if (IsAutoPay && failedCount >= 3 && OnlyPairModel.OnlyPairModelBet(records[records.Count - 1].GoodNo))
                        {
                            //发送下单数据


                            var bill = PayBill(PayMoneys[chaseCount]);

                            payItem.IsPay = "已下单";
                            payItem.PayAmount = PayMoneys[chaseCount];
                            payItem.Balance = double.Parse(bill.data.balance);
                            
                            lblBalance.Text = payItem.Balance.ToString();
                        }

                        records.Add(payItem);
                    }
                    catch (Exception ex)
                    {
                        chaseCount = 1;
                        failedCount = 0;
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

            var postLoginUrl = "https://www.blgj02.com/login";
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
            var postStr = "user_name=" + txtUserName.Text + "&password=" + txtPassword.Text + "&valid_code=" + validCode + "&g_code=&wechat_web=web&wechat_app=app&u_g=0&command=login";
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
            Res_LoginResult rlt = new Res_LoginResult();
            try
            {
                rlt = JsonConvert.DeserializeObject<Res_LoginResult>(rspStr);
            }
            catch (Exception ex)
            {

                MessageBox.Show("登录失败： " + rspStr + " 异常：" + ex.Message);
            }

            if (rlt.success == 1)
            {
                lblLoginStatus.Text = "登录成功";
                lblLoginStatus.ForeColor = Color.Green;
                lblBalance.Text = rlt.data.money.ToString();
                lblBalance.ForeColor = Color.Orange;
                lblUserId.Text = rlt.data.user_id;
            }
            else
            {
                lblLoginStatus.Text = "登录失败";
                lblLoginStatus.ForeColor = Color.Red;
            }
            //return rspStr;
        }
    }
}
