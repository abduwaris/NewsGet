using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Model
{
    public class NewsSourceModel
    {
        public string SourceName { get; set; }

        public NewsSource Source { get; set; }
        public override string ToString()
        {
            return SourceName;
        }
    }

    public enum NewsSource
    {
        Nur,
        TS,
        Null
    }
}
