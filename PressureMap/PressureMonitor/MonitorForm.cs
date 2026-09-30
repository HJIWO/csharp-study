using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PressureMonitor
{
    public partial class MonitorForm : Form
    {
        public MonitorForm()
        {
            InitializeComponent();
            this.Text = "Monitor - 포트" + PressureMap.Common.Config.Port;
        }
    }
}
