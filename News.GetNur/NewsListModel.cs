using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.GetNur
{
    internal class NewsListModel
    {
        public int id { get; set; }
        public int bahanum { get; set; }
        public string thumb { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string updatetime { get; set; }
    }
}
