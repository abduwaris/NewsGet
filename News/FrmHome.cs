using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using News.Model;
using News.IGet;
using News.GetNur;
using News.GetTS;
using News.IDAL;
using News.DAL;
using System.Threading;
using System.Diagnostics;

namespace News
{
    public partial class FrmHome : Form
    {
        INewsBase news;
        int page = 1;
        public FrmHome()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        List<NewsSourceModel> GetNewsSource()
        {
            List<NewsSourceModel> source = new List<NewsSourceModel>();
            source.Add(new NewsSourceModel() { SourceName = "نۇر تورى", Source = NewsSource.Nur });
            source.Add(new NewsSourceModel() { SourceName = "تەڭرىتاغ ئۇيغۇرچە تورى", Source = NewsSource.TS });
            return source;
        }

        private void FrmHome_Load(object sender, EventArgs e)
        {
            var list = GetNewsSource();
            list.Insert(0, new NewsSourceModel() { SourceName = "تاللاڭ", Source = NewsSource.Null });
            cbSource.DataSource = list;
            cbSource.SelectedIndex = 0;
            btnNext.Enabled = false;
            btnPrev.Enabled = false;
            btnGetAll.Enabled = false;
        }

        private void cbSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSource.SelectedIndex > 0)
            {
                cbTypes.Enabled = true;
                NewsSourceModel mod = cbSource.SelectedItem as NewsSourceModel;
                switch (mod.Source)
                {
                    case NewsSource.Nur:
                        news = new NurNews();
                        BindTypes(news.NewsTypes);
                        break;
                    case NewsSource.TS:
                        news = new TSNews();
                        BindTypes(news.NewsTypes);
                        break;
                }
            }
            else
            {
                cbTypes.Enabled = false;
            }
        }

        void BindTypes(List<NewsTypeModel> list)
        {
            list.Insert(0, new NewsTypeModel("0", "تۈرنى تاللاڭ", 0));
            cbTypes.DataSource = list;
            cbTypes.SelectedIndex = 0;
        }

        private void cbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTypes.SelectedIndex > 0)
            {
                //获取数据对象
                NewsSourceModel mod = (NewsSourceModel)cbSource.SelectedItem;
                int id = Convert.ToInt32(((NewsTypeModel)cbTypes.SelectedItem).CatID);
                BindToDgv(news, id);
                btnNext.Enabled = true;
                btnGetAll.Enabled = true;
            }
            else
            {
                dgv.Rows.Clear();

                btnNext.Enabled = false;
                btnPrev.Enabled = false;
                btnGetAll.Enabled = false;
            }
        }

        void BindToDgv(INewsBase news, int typeID, int pageIndex = 1)
        {
            dgv.Rows.Clear();
            if (news != null)
            {
                var newsList = news.GetNewsList(typeID, pageIndex);
                if (newsList == null || newsList.Count <= 0) return;
                foreach (var item in newsList)
                {
                    int index = dgv.Rows.Add();
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = item.ID;
                    dgv.Rows[index].Cells["ID"] = cell;

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = item.Title;
                    dgv.Rows[index].Cells["Title"] = cell;

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = item.Source;
                    dgv.Rows[index].Cells["Source"] = cell;

                    DataGridViewButtonCell cellb = new DataGridViewButtonCell();
                    cellb.Value = item.IsGetted ? "تامام" : "يىغىش";
                    dgv.Rows[index].Cells["Make"] = cellb;

                    dgv.Rows[index].Tag = item;
                    if (item.IsGetted)
                    {
                        dgv.Rows[index].DefaultCellStyle = new DataGridViewCellStyle() { ForeColor = Color.Gray };
                    }
                }
            }
        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            NewsModel mod = (NewsModel)dgv.Rows[e.RowIndex].Tag;
            if (mod.IsGetted)
            {
                MessageBox.Show("سىز بۇ خەۋەرنى يىغىپ بولغان");
                return;
            }
            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell is DataGridViewButtonCell)
            {
                DataGridViewButtonCell cel = (DataGridViewButtonCell)cell;
                mod = news.GetNews(mod);
                if (mod == null)
                {
                    MessageBox.Show("بۇ خەۋەرنى يىغىشتا خاتالىق بار، قايتا سىناپ بېقىڭ");
                    return;
                }
                IGetBase dal = new ZuklanDAL();
                if (dal.Insert(mod) > 0)
                {
                    //写入日记
                    news.WriteGettedLog(mod);
                    mod.IsGetted = true;
                    dgv.Rows[e.RowIndex].DefaultCellStyle = new DataGridViewCellStyle() { ForeColor = Color.Gray };
                    MessageBox.Show("بولدى");
                }
                else
                {
                    MessageBox.Show("بۇ خەۋەرنى يىغىشتا خاتالىق بار");
                }

            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            page--;
            if (page == 1) btnPrev.Enabled = false;
            int id = Convert.ToInt32(((NewsTypeModel)cbTypes.SelectedItem).CatID);
            BindToDgv(news, id, page);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            page++;
            btnPrev.Enabled = true;
            int id = Convert.ToInt32(((NewsTypeModel)cbTypes.SelectedItem).CatID);
            BindToDgv(news, id, page);
        }

        private void btnGetAll_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                NewsModel mod = (NewsModel)dgv.Rows[i].Tag;
                if (mod.IsGetted) continue;

                mod = news.GetNews(mod);
                IGetBase dal = new ZuklanDAL();
                if (dal.Insert(mod) > 0)
                {
                    //写入日记
                    news.WriteGettedLog(mod);
                    mod.IsGetted = true;
                    dgv.Rows[i].DefaultCellStyle = new DataGridViewCellStyle() { ForeColor = Color.Gray };
                    count++;
                }
            }
            MessageBox.Show(string.Format("{0} پارۋە خەۋەر يىغىلدى", count));
        }

        Thread th = null;
        private void tmrAuto_Tick(object sender, EventArgs e)
        {
            if (th != null && th.ThreadState == System.Threading.ThreadState.Background)
            {
                tmrAuto.Interval = 30000;
                return;
            }
            tmrAuto.Interval = 1800000;
            th = new Thread(GetAuto);
            th.Name = "NewsAutoGet";
            th.IsBackground = true;
            th.Start();
        }

        private void GetAuto()
        {
            //设置状态
            btnAuto.Enabled = false;
            lblAuto.Text = "ئاپتۇماتىك ئۇچۇر يىغىۋاتىدۇ، سىستىمىنى تاقىماڭ ياكى مەجبۇرى چىكىندۈرمەڭ";
            INewsBase newsAuto = null;
            int autoPage = 1;
            IGetBase dal = new ZuklanDAL();
            //获取所有涞源
            List<NewsSourceModel> sources = GetNewsSource();
            //根据涞源获取所有所有类别
            foreach (var source in sources)
            {
                //根据类别获取获取新闻集合
                switch (source.Source)
                {
                    case NewsSource.Nur:
                        newsAuto = new NurNews();
                        break;
                    case NewsSource.TS:
                        newsAuto = new TSNews();
                        break;
                    default: continue;
                }
                foreach (var typ in newsAuto.NewsTypes)
                {
                NextPage:
                    List<NewsModel> newsList = newsAuto.GetNewsList(Convert.ToInt32(typ.CatID), autoPage);
                    if (newsList == null || newsList.Count <= 0) continue;
                    bool b = true;
                    NewsModel mod;
                    foreach (var newsItem in newsList)
                    {
                        //新闻集合中搜索没有记录的
                        if (newsItem.IsGetted)
                        {
                            b = false;
                            autoPage = 1;
                            break;
                        }
                        mod = newsAuto.GetNews(newsItem);
                        if (mod == null) continue;
                        //写入到数据库
                        if (dal.Insert(mod) > 0)
                        {
                            newsAuto.WriteGettedLog(mod);
                        }
                    }
                    if (!b) continue;
                    autoPage++;
                    goto NextPage;
                }

            }
            btnAuto.Enabled = true;
            DateTime tim = DateTime.Now;
            TimeSpan tp = new TimeSpan(36000000000);
            tim = tim + tp;
            lblAuto.Text = "ئاپتۇماتىك ئۇچۇر يىغىشقا تەڭشىدىڭىز، كىيىنكى قېتىم ئاپتۇماتىك ئۇچۇر يىغىدىغان ۋاقىت\r\n" + tim.ToString("0000-00-00 00:00:00");
        }
        private void btnAuto_Click(object sender, EventArgs e)
        {
            if (tmrAuto.Enabled)
            {
                tmrAuto.Enabled = false;
                tmrAuto.Interval = 10000;
                dgv.Enabled = true;
                cbSource.Enabled = true;
                btnAuto.Text = "ئاپتۇماتىك يىغىشنى باشلاش";
                dgv.Visible = true;
                lblAuto.Visible = false;
            }
            else
            {
                if (cbTypes.Items.Count > 0)
                {
                    cbTypes.SelectedIndex = 0;
                }
                tmrAuto.Enabled = true;
                cbSource.Enabled = false;
                cbTypes.Enabled = false;
                dgv.Visible = false;
                btnAuto.Text = "ئاپتۇماتىك يىغىشنى توختىتىش";
                lblAuto.Visible = true;
            }
        }

    }
}
