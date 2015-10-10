using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace News.MySql
{
    public partial class FrmDataBase : Form
    {
        public FrmDataBase()
        {
            InitializeComponent();

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //验证控件内容
            if (!CheckInput())
            {
                lblMsg.ForeColor = Color.Red;
                lblMsg.Text = "بوش قالمىسۇن";
                return;
            }
            lblMsg.ForeColor = Color.Blue;
            lblMsg.Text = "ساندانغا ئۇلىنىۋاتىدۇ...";
            //string conStr = string.Format("user id={0};password={1};host={2};Database={3};persist security info=True", txtUserName.Text.Trim(), txtPassWord.Text, txtHost.Text.Trim(), txtDataBase.Text.Trim());
            MySqlConnectionStringBuilder buil = new MySqlConnectionStringBuilder();
            buil.CharacterSet = "utf8";
            buil.Server = txtHost.Text.Trim();
            buil.UserID = txtUserName.Text.Trim();
            buil.Password = txtPassWord.Text;
            buil.Database = txtDataBase.Text.Trim();
            using (MySqlConnection con = new MySqlConnection(buil.ConnectionString))
            {
                try
                {
                    con.Open();
                }
                catch (Exception)
                {
                    lblMsg.ForeColor = Color.Red;
                    lblMsg.Text = "ساندانغا ئۇلىنالمىدى";
                    return;
                }
            }
            MessageBox.Show("ساندانغا مۇۋاپىقىيەتلىك ئۇلاندى!~");
            Helper.conStr = buil.ConnectionString;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        /// <summary>
        /// 验证控件
        /// </summary>
        /// <returns></returns>
        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(txtHost.Text.Trim()) || string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtDataBase.Text.Trim()))
            {
                lblMsg.Text = "بوش قالمىسۇن";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 验证是否
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            lblMsg.Text = "";
        }


    }
}
