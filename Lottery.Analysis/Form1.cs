using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lottery;
using Lottery.Business;

namespace Lottery.Analysis
{
    public partial class Form1 : Form
    {
       

        private LotteryBusiness lb = new LotteryBusiness();

        private int currentPage = 0;

        private List<Model.DB_PredictRecord> TestList = new List<Model.DB_PredictRecord>();
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Analysis() {

            var list = TestList;
            list = list.OrderBy(r => r.IssueId).ToList();
            //var betModel = BetModel.OnlyPair;

            //if (this.comboBox2.SelectedIndex == 0)
            //{
            //    betModel = BetModel.OnlyPair;

            //}
            //else if (this.comboBox2.SelectedIndex == 1)
            //{
            //    betModel = BetModel.PairKill;

            //}
            //else if (this.comboBox2.SelectedIndex == 2)
            //{
            //    betModel = BetModel.PairKlllCV1;
            //}
            //else if (this.comboBox2.SelectedIndex == 3) {

            //    betModel = BetModel.PairKillCV2;
            //} else if (this.comboBox2.SelectedIndex == 4) {
            //    betModel = BetModel.OnlyPairPreWin;
            //}

            var betModel = (BetModel)(this.comboBox2.SelectedIndex + 1);

            AnalysisBusiness ab = new AnalysisBusiness(betModel, list);
            ab.SetKillNo(this.txtkill1.Text, this.txtkill2.Text);
            //list = ab.GenerateBetResult(BetModel.OnlyPair, list);
            double balance = double.Parse(this.textBox1.Text);
            double rate = double.Parse(this.textBox2.Text);
            double rate2 = double.Parse(this.textBox3.Text);
            var fistBet = double.Parse(this.txtFirstBet.Text);
            var chaseNum = int.Parse(this.comboBox1.SelectedItem.ToString());

            list = ab.GenerateAmountResult(balance, fistBet, chaseNum - 1, rate, betModel,rate2);
            ///分析模型
            var anModel = ab.GenerateAnalysisModel(list);

            if (betModel == BetModel.KillTowNo) {
                this.lblCycleTotal.Text = anModel.CycleTotalNum.ToString();
                this.lblCycleWin.Text = anModel.CycleWinNum.ToString();
                this.lblCycleFailedTotal.Text = anModel.CycleFailedNum.ToString();
                this.lblCycleRate.Text = ((double)anModel.CycleWinNum / (double)anModel.CycleTotalNum).ToString("P0");
            }
            this.label2.Text = anModel.MaxPay.ToString();
            this.lblPayTotal.Text = anModel.TotalPay.ToString();
            this.txtBalance.Text = anModel.Balance.ToString();
            this.txtChaseFailed.Text = anModel.ChaseFailedCount.ToString();
            this.lblWinTime.Text = anModel.WinTimes.ToString();
            this.lblLossTime.Text = anModel.LossTimes.ToString();
            this.lblWInRate.Text = (anModel.WinTimes / (anModel.WinTimes + anModel.LossTimes)).ToString("P");
            this.txtMaxFailed.Text = anModel.MaxFailedCount.ToString();

            this.lblContinuFailed.Text = anModel.MaxContinueFailedCount.ToString();

            //列表数据
            this.dataGridView1.DataSource = list;
           
            this.dataGridView1.Columns[0].HeaderCell.Value = "期号";
            this.dataGridView1.Columns[1].HeaderCell.Value = "五星杀号";
            this.dataGridView1.Columns[2].HeaderCell.Value = "后三杀号";
            this.dataGridView1.Columns[3].HeaderCell.Value = "中奖号码";
            this.dataGridView1.Columns[4].HeaderCell.Value = "状态";

            this.dataGridView1.Columns[5].HeaderCell.Value = "是否下单";
            this.dataGridView1.Columns[5].Visible = false;

            this.dataGridView1.Columns[6].HeaderCell.Value = "下单金额";
            this.dataGridView1.Columns[7].HeaderCell.Value = "奖金";
            this.dataGridView1.Columns[8].HeaderCell.Value = "余额";

          
            


            for (var i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                for (var j = 0; j < this.dataGridView1.Rows[i].Cells.Count; j++) {
                    var cell = this.dataGridView1.Rows[i].Cells[j];
                    if (cell.Value != null) {

                        if (cell.Value.ToString() == "中奖")
                        {
                            cell.Style.BackColor = Color.Green;
                        }
                        else if (cell.Value.ToString() == "未中奖")
                        {
                            cell.Style.BackColor = Color.Red;
                        }
                        else if (cell.Value.ToString() == "未下单") {
                            cell.Style.BackColor = Color.Yellow;
                        }
                        else if(j == 1)
                        {
                            cell.Style.BackColor = Color.Yellow;
                            
                        }
                    }

                    
                  
                }
               

                //if (this.dataGridView1.Rows[i].Cells[5].Value.ToString() == "未下单")
                //{
                //    this.dataGridView1.Rows[i].Cells[5].Style.BackColor = Color.Yellow;
                //}





            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.radioButton1.Checked = true;
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.ShowUpDown = true;

            this.dateTimePicker2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.ShowUpDown = true;


            //this.Analysis();
            // TestList = this.lb.GetLoRecordsByLast(int.Parse(txtlast.Text.ToString()));
            TestList = this.lb.GetRecordsByPage();
            lblfrom.Text = (currentPage * 1000).ToString();
            lblto.Text = ((currentPage + 1) * 1000).ToString();
            this.dataGridView1.DataSource = TestList;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked == true)
            {
                TestList = this.lb.GetLoRecordsByLast(int.Parse(txtlast.Text.ToString()));
            }
            else if (this.radioButton2.Checked == true) {
                TestList = this.lb.GetLoRecordsByDate(dateTimePicker1.Value,dateTimePicker2.Value);
            }
          
           
            this.dataGridView1.DataSource = TestList;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Analysis();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// next 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            currentPage = currentPage + 1;
            TestList = this.lb.GetRecordsByPage(currentPage);
            this.dataGridView1.DataSource = TestList;
            lblfrom.Text = (currentPage * 1000).ToString();
            lblto.Text = ((currentPage+1) * 1000).ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (currentPage != 0) {
                currentPage = currentPage - 1;
                TestList = this.lb.GetRecordsByPage(currentPage);
                this.dataGridView1.DataSource = TestList;
                lblfrom.Text = (currentPage * 1000).ToString();
                lblto.Text = ((currentPage + 1) * 1000).ToString();
            }
            
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            var betModel = (BetModel)(this.comboBox2.SelectedIndex + 1);
            var list = TestList;
            list = list.OrderBy(r => r.IssueId).ToList();
            AnalysisBusiness ab = new AnalysisBusiness(betModel, list);
            ab.SetKillNo(this.txtkill1.Text, this.txtkill2.Text);
            this.dataGridView2.DataSource = ab.FindPayPoints(int.Parse(txtling.Text));


        //     public string PayIssueId { get; set; }

        //public string StartDate { get; set; }

        //public string PayDate { get; set; }

        //public int RestPayCount { get; set; }


        //public string StartIssueId { get; set; }

        //public int PassCount { get; set; }

        //public int WinCount { get; set; }

            this.dataGridView2.Columns[0].HeaderCell.Value = "下单期";
            this.dataGridView2.Columns[1].HeaderCell.Value = "观察开始日期";
            this.dataGridView2.Columns[2].HeaderCell.Value = "下单时间";
            this.dataGridView2.Columns[3].HeaderCell.Value = "剩余下单量";
            this.dataGridView2.Columns[4].HeaderCell.Value = "观察开始期";
            this.dataGridView2.Columns[5].HeaderCell.Value = "已过期数";
            this.dataGridView2.Columns[6].HeaderCell.Value = "胜利期数";
            this.dataGridView2.Columns[7].HeaderCell.Value = "下单终止期";

        }
    }
}
