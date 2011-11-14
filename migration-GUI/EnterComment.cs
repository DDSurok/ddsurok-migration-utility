using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace migration
{
    public partial class EnterComment : Form
    {
        private ListRevision _Parent;
        public EnterComment(ListRevision parent)
        {
            this._Parent = parent;
            InitializeComponent();
        }

        private void EnterComment_Shown(object sender, EventArgs e)
        {
            this.comment.Text = "";
        }

        private void EnterComment_FormClosed(object sender, FormClosedEventArgs e)
        {
            this._Parent.ActiveComment = this.comment.Text;
        }
    }
}
