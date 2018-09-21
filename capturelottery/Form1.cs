using CommonFunction;
using Lottery.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lottery.Business;

namespace capturelottery
{
    public partial class Form1 : Form
    {
        private bool IsCaptured = false;

        private int BatchId = 0;

        private LotteryBusiness LoBll = new LotteryBusiness();

        private string LotteryURl = "https://www.blgj02.com/controller/lottery/chart";
        public Form1()
        {
            InitializeComponent();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IsCaptured  = !IsCaptured;
            if (IsCaptured == true)
            {
                this.button1.Text = "停止抓取";
                this.timer1.Start();
            }
            else {

                this.button1.Text = "开始抓取";
                this.timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            var currentIssue = "";
            try
            {
                var lottery_id = 10014;
              
                var res = Util.getURLResponseStr(LotteryURl, null, "content=%7B%22command_id%22%3A23%2C%22lottery_id%22%3A%22" + lottery_id + "%22%2C%22issue_status%22%3A%221%22%2C%22count%22%3A%2230%22%7D&command=lottery_request_transmit_v2");
                var rlt = JsonConvert.DeserializeObject<Res_LotteryResult>(res);
                foreach (var item in rlt.data.detail.LIST) {
                    if (!LoBll.IsExsit(lottery_id.ToString(), item.CP_QS))
                    {
                        currentIssue = item.CP_QS.ToString();
                        var exrlt = LoBll.InsertLottery(item.ZJHM, this.BatchId, lottery_id.ToString(), item.CP_QS.ToString(), item.SJKJSJ);
                        if (exrlt > 0) {
                            this.richTextBox1.Text += string.Format("{0}--抓取成功\t 时间：{1}\n", item.CP_QS, DateTime.Now);
                            this.richTextBox1.ForeColor = Color.Green;
                        }
                    }
                    else {
                        break;
                    }
                   
                }
            }
            catch (Exception ex) {
                this.richTextBox1.Text += string.Format("\n抓取失败\t 时间：{0}\t 异常：{1}\n", DateTime.Now,ex.Message);
                this.richTextBox1.ForeColor = Color.Red;
            }
        
           

            this.timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //获取最大批次号
            var sql = "select max(BatchId) from [Lottery].[dbo].[PredictRecord]";
            var dataTable = SQLHelper.GetDataTable(sql, null);
            if (dataTable.Rows.Count > 0)
            {
                var bat = dataTable.Rows[0][0].ToString();
                this.BatchId = int.Parse(bat);
                this.BatchId = this.BatchId + 1;
            }
            this.timer1.Interval = 5000;
        }
    }
}
