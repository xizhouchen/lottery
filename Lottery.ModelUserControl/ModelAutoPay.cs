using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lottery.PlatForm;
using Lottery.Business;
using Lottery.Model;

namespace Lottery.ModelUserControl
{

    public enum AutoStatus{

        Stop = 1,
        Watching = 2,
        Ordering = 3,
        Start = 4,
        OrderOver = 5

    }
    public partial class ModelAutoPay : UserControl
    {
        /// <summary>
        /// 下单平台
        /// </summary>
        private IPlatForm PlatForm;
        /// <summary>
        /// 下单模型，根据模型预测下棋买的号
        /// </summary>
        private IModel Model;
        /// <summary>
        /// 下单模式 离，分，元
        /// </summary>
        private string PayModel;

        /// <summary>
        /// 下单倍数
        /// </summary>
        private string PayMuli;
        /// <summary>
        /// 那一期下单
        /// </summary>
        private string PayIssueId;

        private AutoStatus Status;

        /// <summary>
        /// 0 为 开
        /// </summary>
        private bool SwichOn;
        /// <summary>
        /// 已下单的期
        /// </summary>
        private List<string> PayedIssues;

        public ModelAutoPay()
        {
            InitializeComponent();
        }

        public void InitControl(IPlatForm platForm, IModel model) {
            this.PlatForm = platForm;
            this.Model = model;
        }

        public void AddOrder() {

        }

        public void UpdateStatus(List<DB_PredictRecord> records) {
            if (this.SwichOn) {

                Model.History = records;
                Model.FindPayPoint();
            }
        }

        public void Start() {
            this.SwichOn = true;
            this.Status = AutoStatus.Start;
        }

        public void Stop() {
            this.SwichOn = false;
            this.Status = AutoStatus.Stop;
        }
    }
}
