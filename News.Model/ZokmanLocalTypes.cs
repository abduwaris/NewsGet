using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News.Model
{
    /// <summary>
    /// زوقمەننىڭ يەرلىكتىكى تۈرى
    /// </summary>
    public class ZokmanLocalType
    {
        public ZokmanLocalType(int id, string name)
        {
            this.Name = name;
            this.ID = id;
        }
        /// <summary>
        /// يەرلىكتىكى تۈر نۇمۇرى
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// يەرلىكتىكى تۈر ئىسمى
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// يەرلىكتىكى تۈر ناملىرى
        /// </summary>
        public static List<ZokmanLocalType> LocalTypes
        {
            get
            {
                List<ZokmanLocalType> typ = new List<ZokmanLocalType>();
                typ.Add(new ZokmanLocalType(14, "خەلىقئارا"));
                typ.Add(new ZokmanLocalType(15, "مەملىكەت"));
                typ.Add(new ZokmanLocalType(16, "شىنجاڭ"));
                typ.Add(new ZokmanLocalType(19, "تېخنىكا خەۋەرلىرى"));
                typ.Add(new ZokmanLocalType(56, "جەمئىيەت"));
                typ.Add(new ZokmanLocalType(43, "ئىقتىساد"));
                typ.Add(new ZokmanLocalType(26, "مەدەنىيەت"));
                typ.Add(new ZokmanLocalType(36, "تەنتەربىيە"));
                typ.Add(new ZokmanLocalType(19, "ئەدەبىيات"));
                //typ.Add(new ZokmanLocalType(1, ""));
                //typ.Add(new ZokmanLocalType(1, ""));
                //typ.Add(new ZokmanLocalType(1, ""));
                //typ.Add(new ZokmanLocalType(1, ""));
                //typ.Add(new ZokmanLocalType(1, ""));
                //typ.Add(new ZokmanLocalType(1, ""));
                //typ.Add(new ZokmanLocalType(1, ""));
                //typ.Add(new ZokmanLocalType(1, ""));
                return typ;
            }
        }
    }
}
