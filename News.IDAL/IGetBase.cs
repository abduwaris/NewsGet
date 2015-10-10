using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using News.Model;

namespace News.IDAL
{
    public interface IGetBase
    {
        int Insert(NewsModel news);
    }
}
