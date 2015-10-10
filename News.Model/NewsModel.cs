using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Model
{
    /// <summary>
    /// خەۋەر مودىل ئوبىكتى
    /// </summary>
    public class NewsModel
    {
        /// <summary>
        /// خەۋەرنىڭ مەنبەسىدىكى نۇمۇرى
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// تىما
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// مەزمۇن
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// قىسقىچە مەزمۇنى
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// ۋاقىت
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// مەنبە
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// مەنبە ئادرىسى
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// خەۋەر كىچىك رەسىمى
        /// </summary>
        public string Pic { get; set; }

        /// <summary>
        /// خەۋەر تۈرى
        /// </summary>
        public NewsTypeModel Types { get; set; }

        /// <summary>
        /// يىغىپ بولغانمۇ
        /// </summary>
        public bool IsGetted { get; set; }

    }
}
