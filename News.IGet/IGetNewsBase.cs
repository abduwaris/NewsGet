using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using News.Model;

namespace News.IGet
{
    /// <summary>
    /// 新闻采集基类
    /// </summary>
    public interface INewsBase
    {
        /// <summary>
        /// تەڭرىتاغ ئۇيغۇرچە قانىلىدىن ئەدەبى ئەسەر ۋە خەۋەرلەرگە ئىرىشىش
        /// </summary>
        /// <param name="news">خەۋەرنىڭ نۇمۇرى</param>
        /// <returns>خەۋەرنى قايتۇرىدۇ</returns>
        NewsModel GetNews(NewsModel news);

        /// <summary>
        /// خەۋەر توپلىمىنى قايتۇرىدۇ
        /// </summary>
        /// <param name="typeID">تۈر نۇمۇرى</param>
        /// <param name="pageIndex">بەت</param>
        /// <returns></returns>
        List<NewsModel> GetNewsList(int typeID, int pageIndex = 1);

        /// <summary>
        /// خەۋەر تۈرىنى قايتۇرىدۇ
        /// </summary>
        List<Model.NewsTypeModel> NewsTypes { get;}

        void WriteGettedLog(NewsModel news);
    }
}
