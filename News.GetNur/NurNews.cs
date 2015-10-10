using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using News.IGet;
using News.Model;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace News.GetNur
{
    /// <summary>
    /// نۇر خەۋەرلىرى
    /// </summary>
    public class NurNews : INewsBase
    {
        private string _setRoot = "setting\\nur";
        private string _setPath = "setting\\nur\\getted.bulaq.net";
        private string _setDefaultPic = "";
        /// <summary>
        /// نۇر خەۋەرلىرىگە ئىرىشىش
        /// </summary>
        /// <returns></returns>
        public NewsModel GetNews(NewsModel news)
        {
            //http://www.nur.cn/news/2014/10/190521.shtml
            //string uri = string.Format("http://www.nur.cn/news/2014/10/{0}.shtml", news.ID);
            string htm = Html.GetHtml(news.SourceUrl, "http://www.nur.cn/news");
            if (string.IsNullOrEmpty(htm)) return null;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htm);
            HtmlNode node = GetMamun(doc.DocumentNode);
            if (node == null) return null;
            node.RemoveChild(node.Elements("p").LastOrDefault());
            //获取图片
            Html.EachImages(node);
            //保存头铁
            if (news.Pic != null)
            {
                string pic = "autoimages\\nur\\" + DateTime.Now.Month + "\\" + Guid.NewGuid().ToString() + "." + Path.GetExtension(news.Pic).Trim('.');
                if (Html.SaveImg(news.Pic, pic))
                {
                    news.Pic = "/" + pic.Replace('\\', '/');
                }
                else
                {
                    //获取默认图片
                    news.Pic = _setDefaultPic;
                }
            }
            else
            {
                news.Pic = _setDefaultPic;
            }

            //删除 注释
            IEnumerable<HtmlNode> commons = node.Elements("#comment");
        Remove:
            if (commons != null && commons.Count() > 0)
            {
                node.RemoveChild(commons.First());
                goto Remove;
            }
            news.Content = node.InnerHtml.Trim(' ', '\r', '\n');
            news.Time = DateTime.Now;
            string innerText = node.InnerText;
            news.KeyWords = innerText.Length > 240 ? innerText.Substring(0, 240) : innerText;
            return news;
        }

        public void WriteGettedLog(NewsModel news)
        {
            //写入记录
            if (!File.Exists(_setPath))
            {
                if (!Directory.Exists(_setRoot)) Directory.CreateDirectory(_setRoot);
            }
            File.AppendAllText(_setPath, news.ID.ToString() + ":", Encoding.UTF8);
        }

        /// <summary>
        /// خەۋەرنىڭ مەزمۇنىغا ئىرىشىش
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private HtmlNode GetMamun(HtmlNode node)
        {
            try
            {
                HtmlNode html = node.Element("html");
                node = html.Element("body");
                IEnumerable<HtmlNode> divs = node.Elements("div");
                node = divs.Where(c => c.Attributes.Contains("class")).FirstOrDefault(p => p.GetAttributeValue("class", "") == "list_box");
                divs = node.Elements("div");
                node = divs.Where(c => c.Attributes.Contains("class")).FirstOrDefault(p => p.GetAttributeValue("class", "") == "content_box");
                divs = node.Elements("div");
                node = divs.Where(c => c.Attributes.Contains("class")).FirstOrDefault(p => p.GetAttributeValue("class", "") == "content");
                divs = node.Elements("div");
                node = divs.Where(c => c.Attributes.Contains("class")).FirstOrDefault(p => p.GetAttributeValue("class", "") == "mazmun");
                return node;
            }
            catch (Exception)
            {
                return null;
            }


        }

        private List<NewsListModel> _GetNewsList(int typeID, int pageIndex)
        {
            string postData = "id=" + typeID + "&page=" + pageIndex + "&rnd=0.1139371762983501";
            byte[] dataArray = Encoding.UTF8.GetBytes(postData);
            //创建请求
            string newUrl = "http://www.nur.cn/index.php?a=" + (typeID == 18 || typeID == 19 ? "txt" : "tur") + "_json";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(newUrl);
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = "http://www.nur.cn/index.php?a=lists&catid=" + typeID;
            //创建输入流
            Stream dataStream = null;
            try
            {
                dataStream = request.GetRequestStream();
            }
            catch (Exception)
            {
                return null;//连接服务器失败
            }
            //发送请求
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            //读取返回消息
            string res = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
                return JsonConvert.DeserializeObject<List<NewsListModel>>(res);
            }
            catch (Exception)
            {
                return null;//连接服务器失败
            }

        }

        /// <summary>
        /// 新闻列表
        /// </summary>
        /// <param name="typeID">分类ID</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>返回新闻列表</returns>
        public List<NewsModel> GetNewsList(int typeID, int pageIndex = 1)
        {
            var _newsL = _GetNewsList(typeID, pageIndex);
            if (_newsL == null || _newsL.Count <= 0) return null;
            List<NewsModel> list = new List<NewsModel>();
            foreach (var news in _newsL)
                list.Add(new NewsModel()
                {
                    Source = "نۇر تورى",
                    SourceUrl = string.Format("http://www.nur.cn/news/2014/10/{0}.shtml", news.id.ToString()),
                    ID = news.id.ToString(),
                    Pic = news.thumb,
                    Title = news.title,
                    Types = NewsTypes.FirstOrDefault(c => c.CatID == typeID.ToString()),
                    IsGetted = IsGetted(news.id)
                });
            return list;
        }

        private List<NewsTypeModel> _Types;

        /// <summary>
        /// خەۋەر تۈرى توپلىمى
        /// </summary>
        public List<NewsTypeModel> NewsTypes
        {
            get
            {
                if (_Types == null)
                {
                    _Types = new List<NewsTypeModel>();
                    _Types.Add(new NewsTypeModel("7", "خەلىقئارا", 14));
                    _Types.Add(new NewsTypeModel("8", "مەملىكەت", 15));
                    _Types.Add(new NewsTypeModel("9", "شىنجاڭ", 16));
                    _Types.Add(new NewsTypeModel("10", "تاماشا", 57));
                    _Types.Add(new NewsTypeModel("11", "جەمئىيەت", 56));
                    _Types.Add(new NewsTypeModel("12", "ئىقتىساد", 43));
                    _Types.Add(new NewsTypeModel("13", "مەدەنىيەت", 26));
                    _Types.Add(new NewsTypeModel("15", "تېخنىكا", 19));
                    _Types.Add(new NewsTypeModel("16", "تەنتەربىيە", 36));
                    //_Types.Add(new NewsTypeModel("17", "رەسىم"));
                    _Types.Add(new NewsTypeModel("18", "يۇمۇر", 34));
                    _Types.Add(new NewsTypeModel("19", "ئەدەبىيات", 52));
                    _Types.Add(new NewsTypeModel("86", "ماشىنا", 67));
                    _Types.Add(new NewsTypeModel("91", "ساغلاملىق", 40));
                }
                return _Types;
            }
        }

        /// <summary>
        /// يىغىلغان يىغىلمىغانلىقىنى تەكشۈرىدۇ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsGetted(int id)
        {
            if (File.Exists(_setPath))
            {
                string str = File.ReadAllText(_setPath, Encoding.UTF8);
                if (str.Contains(id.ToString())) return true;
            }
            return false;
        }
    }
}
