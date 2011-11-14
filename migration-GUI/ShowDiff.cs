using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace migration
{
    public partial class ShowDiff : Form
    {
        SqlConnection connection;
        DataSet ds1, ds2;
        SqlDataAdapter da1, da2;
        public ShowDiff()
        {
            InitializeComponent();
            connection = new SqlConnection("Data Source=" + ConfigFile.serverName + ";Integrated Security=True");
            connection.Open();
            connection.ChangeDatabase(ConfigFile.databaseName);
            ds1 = new DataSet();
            da1 = new SqlDataAdapter("SELECT * FROM [dds].[up]", connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(da1);
            da1.UpdateCommand = builder.GetUpdateCommand();
            da1.Fill(ds1, "up");
            this.diffUp.AutoGenerateColumns = true;
            this.diffUp.DataSource = ds1;
            this.diffUp.DataMember = "up";
            ds2 = new DataSet();
            da2 = new SqlDataAdapter("SELECT * FROM [dds].[down]", connection);
            builder = new SqlCommandBuilder(da2);
            da2.UpdateCommand = builder.GetUpdateCommand();
            da2.Fill(ds2, "down");
            this.diffDown.AutoGenerateColumns = true;
            this.diffDown.DataSource = ds2;
            this.diffDown.DataMember = "down";
        }

        private void ShowDiff_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.da1.Update(ds1, "up");
            this.da2.Update(ds2, "down");
        }
    }
}
