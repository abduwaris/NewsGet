using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using News.Model;
using News.MySql;
using News.IDAL;
using MySql.Data.MySqlClient;

namespace News.DAL
{
    public class ZuklanDAL : IGetBase
    {
        public int Insert(NewsModel news)
        {
            //DateTime test = new DateTime(2015, 5, 27, 22, 43, 31);
            //long lb = test.Ticks - 14327378110000000;
            //DateTime dt = new DateTime(lb);

            long times = news.Time.Ticks;
            DateTime timeold = new DateTime(1970, 1, 1, 8, 0, 0);
            long tic = times - timeold.Ticks;
            tic = tic / 10000000;
            string sql = "INSERT INTO `phpred_hawar` (`title` ,`tur` ,`tur2` ,`keywords`,`body` ,`litpic` ,`mid` ,`time` ,`menbe` ,`menbe_url` ,`ip`,`testiq` ,`tewis` ,`view` ,`last_view`) VALUES(@title,@tur,'0',@keywords,@content,@pic,'0',@time,@menbe,@menbeurl,'127.0.0.1','1','0','1','0');";
            MySqlParameter[] param = { 
                                     new MySqlParameter("@title",news.Title),
                                     new MySqlParameter("@tur",news.Types.LocalID),
                                     new MySqlParameter("@keywords",news.KeyWords),
                                     new MySqlParameter("@content",news.Content),
                                     new MySqlParameter("@pic",news.Pic),
                                     new MySqlParameter("@time", MySqlDbType.VarChar){Value = tic},
                                     new MySqlParameter("@menbe",news.Source),
                                     new MySqlParameter("@menbeurl",news.SourceUrl)
                                     };
            return Helper.Insert(sql, param);
        }
    }
}
