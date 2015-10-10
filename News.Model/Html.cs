using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace News.Model
{
    /// <summary>
    /// Html بىر تەرەپ قىلغۇچ
    /// </summary>
    public class Html
    {
        private static readonly string _setRoot = "setting/rootfolder.bulaq.net";
        /// <summary>
        /// Html
        /// نىڭ ئەسلى كودىغا ئىرىشىش
        /// </summary>
        /// <param name="uri">ئادرىس</param>
        /// <returns>Html code</returns>
        public static string GetHtml(string uri, string refer)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("Referer", refer);
                try
                {
                    byte[] buffer = wc.DownloadData(uri);
                    return System.Text.Encoding.UTF8.GetString(buffer);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// رەسىم ساقلاش مەشغۇلاتىنى تاماملايدۇ
        /// </summary>
        /// <param name="uri">رەسىم ئادرىسى</param>
        /// <param name="fileName">يەرلىككە ساقلايدىغان ئادرىسى</param>
        /// <returns>رەسىمنى ساقلىيالىغانلىقىنى قايتۇرىدۇ</returns>
        public static bool SaveImg(string uri, string fileName)
        {
            fileName = RootFolder().TrimEnd('\\') + '\\' + fileName;
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            WebClient wb = new WebClient();
            try
            {
                wb.DownloadFile(uri, fileName);
                return true;
            }
            catch (Exception) { return false; }
        }

        #region 检查网络状态

        //检测网络状态
        [DllImport("wininet.dll")]
        extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        /// <summary>
        /// تور ئۇلىنىشنى تەكشۈرىدۇ
        /// </summary>
        /// <returns>تورغا ئۇلانغان ياكى ئۇلانمىغانلىقىنى قايتۇرىدۇ</returns>
        public bool isConnected()
        {
            int I = 0;
            bool state = InternetGetConnectedState(out I, 0);
            return state;
        }

        #endregion

        /// <summary>
        /// 递归遍历内容中的图片
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static void EachImages(HtmlNode node, string baseUri = "")
        {
            //判断是否有子标签
            if (node.HasChildNodes)
                foreach (HtmlNode nn in node.ChildNodes)
                    EachImages(nn, baseUri);
            else if (node.Name == "img")
            {
                //图片
                string url = node.GetAttributeValue("src", "");
                if (url == "") return;
                //获取图片信息,生成新的路径
                //getimages/{Year}/{Week}/{FileName}
                string exe = Path.GetExtension(url).TrimStart(new char[] { '.' });
                string fileName = Guid.NewGuid().ToString() + "." + exe;
                int day = DateTime.Now.Day;
                //相对路径
                string fullName = string.Format("autoimages\\{0}\\{3}\\{1}\\{2}." + exe, DateTime.Now.Year, day <= 10 ? 1 : (day <= 20 ? 2 : 3), Guid.NewGuid().ToString(), DateTime.Now.Month);
                //网站 
                string urlNew = "/" + fullName.Replace("\\", "/");
                node.SetAttributeValue("src", urlNew);
                //保存到本地
                Uri uri = baseUri == "" ? new Uri(url) : new Uri(new Uri(baseUri), url);
                SaveImg(uri.AbsoluteUri, fullName);
            }
        }

        /// <summary>
        /// 获取第一个图片
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetFirstImg(HtmlNode node)
        {
            if (node.Name == "img")
            {
                //图片
                return node.GetAttributeValue("src", string.Empty);
            }
            else if (node.HasChildNodes)
            {
                foreach (HtmlNode nn in node.ChildNodes)
                {
                    string str = GetFirstImg(nn);
                    if (!string.IsNullOrEmpty(str)) return str;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取根目录
        /// </summary>
        /// <returns></returns>
        private static string RootFolder()
        {
            string root = string.Empty;
            if (!File.Exists(_setRoot))
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.RootFolder = Environment.SpecialFolder.Desktop;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    root = fbd.SelectedPath;
                    File.WriteAllText(_setRoot, root, Encoding.UTF8);
                }
            }
            else
            {
                root = File.ReadAllText(_setRoot, Encoding.UTF8);
            }
            return root;

        }
    }
}
