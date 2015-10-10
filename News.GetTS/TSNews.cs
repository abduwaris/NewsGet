using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using News.IGet;
using News.Model;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;

namespace News.GetTS
{
    /// <summary>
    /// تەڭرىتاغ ئۇيغۇرتورى
    /// </summary>
    public class TSNews : INewsBase
    {
        private string _setRoot = "setting\\ugts";
        private string _setPath = "setting\\ugts\\getted.bulaq.net";
        private string _setDefaultImg = "/autoimages/tip.png";

        public NewsModel GetNews(NewsModel news)
        {
            //html
            string htm = Html.GetHtml(news.SourceUrl, "http://uy.ts.cn");
            if (string.IsNullOrEmpty(htm)) return null;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htm);
            HtmlNode node = doc.GetElementbyId("content_value");
            if (node == null) return null;
            //图片
            //获取基URl
            //http://uy.ts.cn/news/node_900.htm
            //string url = string.Format("http://uy.ts.cn/{0}/node_{1}.htm", GetUrlBefore(Convert.ToInt32(news.Types.CatID)), news.Types.CatID);
            Html.EachImages(node, news.SourceUrl);
            //删除 注释
            IEnumerable<HtmlNode> commons = node.Elements("#comment");
        Remove:
            if (commons != null && commons.Count() > 0)
            {
                node.RemoveChild(commons.First());
                goto Remove;
            }

            news.Content = node.InnerHtml.Trim(' ','\r','\n');
            news.Time = DateTime.Now;
            news.Pic = _GetFirstImg(node);
            string innerText = node.InnerText;
            news.KeyWords = innerText.Length > 240 ? innerText.Substring(0, 240) : innerText;
            return news;
        }

        private List<NewsTypeModel> _Types;

        public List<NewsTypeModel> NewsTypes
        {
            get
            {
                if (_Types == null)
                {
                    _Types = new List<NewsTypeModel>();
                    _Types.Add(new NewsTypeModel("924", "تەرمىلەر", 32));
                    _Types.Add(new NewsTypeModel("922", "ئەسەر", 52));
                    _Types.Add(new NewsTypeModel("925", "ھىكمەت", 33));
                    _Types.Add(new NewsTypeModel("926", "شېئىرىيەت", 53));

                    _Types.Add(new NewsTypeModel("900", "مەملىكەت", 15));
                    _Types.Add(new NewsTypeModel("899", "شىنجاڭ", 16));
                    _Types.Add(new NewsTypeModel("901", "خەلىقئارا", 14));
                    _Types.Add(new NewsTypeModel("903", "ئىقتىساد", 43));

                }
                return _Types;
            }
        }


        public List<NewsModel> GetNewsList(int typeID, int pageIndex = 1)
        {
            //url  http://uy.ts.cn/wenxue/node_924.htm
            string urlB = GetUrlBefore(typeID);
            if (string.IsNullOrEmpty(urlB)) return null;
            string url = string.Format("http://uy.ts.cn/{0}/node_{1}.htm", urlB, pageIndex == 1 ? typeID.ToString() : typeID + "_" + pageIndex);
            string htm = Html.GetHtml(url, "http://uy.ts.cn/");
            if (string.IsNullOrEmpty(htm)) return null;
            //html
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htm);
            HtmlNode node = doc.GetElementbyId("item_list");
            if (node == null) return null;
            //a list
            IEnumerable<HtmlNode> aList = node.Elements("a");
            if (aList == null || aList.Count() <= 0) return null;
            List<NewsModel> list = new List<NewsModel>();
            foreach (var a in aList)
            {
                HtmlNode em = a.Element("em");
                if (em == null) continue;
                //HtmlNode span = a.Element("span");
                //if (span == null) continue;
                //DateTime time = Convert.ToDateTime(span.InnerHtml);
                string href = a.GetAttributeValue("href", "#");
                //http://uy.ts.cn/wenxue/content/2013-12/25/content_319157.htm
                Regex reg = new Regex("content_(\\d+)\\.htm");
                int id = 0;
                if (reg.IsMatch(href))
                {
                    string sid = reg.Match(href).Groups[1].Value;
                    id = Convert.ToInt32(sid);
                }
                else continue;
                //string urls = string.Format("http://uy.ts.cn/{0}/{1}" ,GetUrlBefore(typeID), href);
                list.Add(new NewsModel()
                {
                    Source = "تەڭرىتاغ ئۇيغۇرچە تورى",
                    SourceUrl = string.Format("http://uy.ts.cn/{0}/{1}", GetUrlBefore(typeID), href),
                    Title = em.InnerHtml,
                    ID = id.ToString(),
                    IsGetted = IsGetted(id),
                    Types = this.NewsTypes.FirstOrDefault(c => c.CatID == typeID.ToString())
                });
            }
            return list.Count > 0 ? list : null;
        }


        private string GetUrlBefore(int typeid)
        {
            switch (typeid)
            {
                case 924:
                case 922:
                case 925:
                case 926: return "wenxue";
                case 899:
                case 900:
                case 901:
                case 903: return "news";
                default: return string.Empty;
            }
        }
        /// <summary>
        /// 获取第一个图片
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string _GetFirstImg(HtmlNode node)
        {
            string img = Html.GetFirstImg(node);
            return string.IsNullOrEmpty(img) ? _setDefaultImg : img;
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


        public void WriteGettedLog(NewsModel news)
        {
            //写入记录
            if (!File.Exists(_setPath))
            {
                if (!Directory.Exists(_setRoot)) Directory.CreateDirectory(_setRoot);
            }
            File.AppendAllText(_setPath, news.ID.ToString() + ":", Encoding.UTF8);
        }
    }
}
