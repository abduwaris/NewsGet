using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace News
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //ئەگەر تەڭشەك ھۆججەتلىرىنى ساقلايدىغان مۇندەرىجە بولمىسا ئۇنى قۇرۇپ بەرسۇن
            if (!Directory.Exists("setting")) Directory.CreateDirectory("setting");
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmHome());
            
        }
    }
}
