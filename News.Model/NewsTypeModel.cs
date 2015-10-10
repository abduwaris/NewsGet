using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Model
{
    /// <summary>
    /// خەۋەر تۈرى مودىلى
    /// </summary>
    public class NewsTypeModel
    {
        public NewsTypeModel() { }
        public NewsTypeModel(string id, string name, int loid)
        {
            this.CatName = name;
            this.CatID = id;
            this.LocalID = loid;
        }
        /// <summary>
        /// تۈر نۇمۇرى
        /// </summary>
        public string CatID { get; set; }

        /// <summary>
        /// تۈر نامى
        /// </summary>
        public string CatName { get; set; }

        /// <summary>
        /// 本地新闻ID
        /// </summary>
        public int LocalID { get; set; }

        public override string ToString()
        {
            return CatName;
        }
    }
}
