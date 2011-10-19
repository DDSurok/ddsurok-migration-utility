using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ConfigureMigrationTool
{
    public partial class PleaseWait : Form
    {
        private MainForm _parent;

        delegate void Proc0();

        public PleaseWait(MainForm parent)
        {
            this._parent = parent;
            InitializeComponent();
        }

        private void PleaseWait_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sender != this)
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        private void PleaseWait_Shown(object sender, EventArgs e)
        {
            Thread t1 = new Thread(this.Start);
            t1.Start();
            while (t1.IsAlive)
            { }
            this.Close();
        }
        private void Start()
        {
            Proc0 d = new Proc0(this._parent.ReloadServerList);
            this._parent.Invoke(d);
        }
    }
}
