using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Forms;

namespace News.MySql
{
    public class Helper
    {
        private static string _setRoot = "setting\\mysql";
        private static string _setPath = "setting\\mysql\\constr.bulaq.net";
        /// <summary>
        /// 链接字符串
        /// </summary>
        public static string conStr = null;
        private static void SetConstr()
        {
            if (File.Exists(_setPath))
            {
                string str = File.ReadAllText(_setPath, Encoding.UTF8);
                using (MySqlConnection con = new MySqlConnection(str))
                {
                    try
                    {
                        con.Open();
                        conStr = str;
                        return;
                    }
                    catch (Exception) { }
                }
            }
            else if (!File.Exists(_setPath))
                if (!Directory.Exists(_setRoot)) 
                    Directory.CreateDirectory(_setRoot);
            FrmDataBase frm = new FrmDataBase();
            while (frm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("ساندان ئۇچۇرلىرىنى توغرا كىرگۈزۈڭ، بولمىسا داۋاملاشتۇرالمايسىز");
            }
            File.WriteAllText(_setPath, conStr, Encoding.UTF8);
        }

        public static int Insert(string sql, params MySqlParameter[] param)
        {
            if (string.IsNullOrEmpty(conStr)) SetConstr();
            using (MySqlConnection con = new MySqlConnection(conStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    if (param != null) cmd.Parameters.AddRange(param);
                    try
                    {
                        con.Open();
                        return cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
        }
    }
}
