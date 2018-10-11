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

namespace Lottery.ModelUserControl
{
    public partial class ModelAutoPay : UserControl
    {
        private IPlatForm PlatForm;

        private IModel Model;
        public ModelAutoPay()
        {
            InitializeComponent();
        }

        public void InitControl(IPlatForm platForm, IModel model) {
            this.PlatForm = platForm;
            this.Model = model;
        }
    }
}
