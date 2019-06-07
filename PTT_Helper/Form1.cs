#region Form1
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
//using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace PTT_Helper
{
    public partial class Form1 : Form
    {
        #region AeroDefine
        //[StructLayout(LayoutKind.Sequential)]
        //public struct MARGINS
        //{
        //    public int Left;
        //    public int Right;
        //    public int Top;
        //    public int Bottom;
        //}
        ////DLL申明
        //[DllImport("dwmapi.dll", PreserveSig = false)]
        //static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS
        //margins);
        ////DLL申明
        //[DllImport("dwmapi.dll", PreserveSig = false)]
        //static extern bool DwmIsCompositionEnabled();
        ////直接添加代碼
        //protected override void OnLoad(EventArgs e)
        //{
        //    if (DwmIsCompositionEnabled())
        //    {
        //        MARGINS margins = new MARGINS();
        //        margins.Right = margins.Left = margins.Top = margins.Bottom =
        //        this.Width + this.Height;
        //        DwmExtendFrameIntoClientArea(this.Handle, ref margins);
        //    }
        //    base.OnLoad(e);
        //}
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    base.OnPaintBackground(e);
        //    if (DwmIsCompositionEnabled())
        //    {
        //        e.Graphics.Clear(Color.Black);
        //    }
        //}

        //-------------------------------------------------------------
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
        #endregion

        private int CPage = 1;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "PTT Helper";
            #region Data
            dataGridView1.ColumnCount = 5;                             // 定義所需要的行數
            //dataGridView1.AutoSize = true;
            dataGridView1.Columns[0].Name = "Pop";
            dataGridView1.Columns[1].Name = "Title";
            dataGridView1.Columns[2].Name = "author";
            dataGridView1.Columns[3].Name = "URL";
            dataGridView1.Columns[4].Name = "Text";

            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;

            this.dataGridView1.DefaultCellStyle.BackColor = Color.Black;

            //
            //dataGridView2.ColumnCount = 1;
            //dataGridView2.Columns[0].Name = "Push";
            //dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            #endregion
            #region Console
            AllocConsole();
            //Console.OutputEncoding = Encoding.UTF8;          
            ConsoleColor oriColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("* Don't close this console window or the application will also close.");
            Console.ForegroundColor = oriColor;
            #endregion
            #region ComboBox
            String[] str_combox = { "Gossiping","Gov_owned", "Stock","Option","FATE_GO", "Soft_Job","Tech_Job","HardwareSale","C_Chat",
                "nb-shopping","car","sex",
            };
            foreach (string fucktoken in str_combox)
            {
                comboBox1.Items.Add(fucktoken);
            }
            #endregion
            Properties.Settings MySetting = new Properties.Settings();//Resource Define
            textBox1.Text = MySetting.str_page;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings MySetting = new Properties.Settings();//Resource Define
            MySetting.str_page = textBox1.Text;
            MySetting.Save();
        }

        private void ProgressBar1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            Console.WriteLine("Running");
            try
            {
                CPage = Convert.ToInt32(textBox1.Text);
            }
            catch (Exception number)
            {
                Console.WriteLine(number.Message);
                textBox1.Text = "30";
                CPage = 30;
            }

            //StartCrawlerasync("", CPage);
            StartCrawlerasync("", CPage);

        }

        private void GridViewDoubleClick(object sender, EventArgs e)
        {

        }
        private async Task StartCrawlerasync(string url, int pages)
        {

            string baseurl = "https://www.ptt.cc";
            string PageUp = "";
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            int step = 100 / pages;
            string str_url = "https://www.ptt.cc/bbs/Gossiping/index.html";
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            Uri uri = new Uri(str_url);
            Uri uris = new Uri("https://www.ptt.cc/bbs/Gossiping/");
            handler.CookieContainer.Add(uri, new Cookie("over18", "1")); // Adding a Cookie
                                                                         //handler.CookieContainer.Add(new Cookie("over18", "1") { Domain = uris.Host }); // Adding a Cookie

            HttpClient httpClient = new HttpClient(handler);
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            string html = await httpClient.GetStringAsync(str_url);
            CookieCollection collection = handler.CookieContainer.GetCookies(uri); // Retrieving a                     
            HtmlDocument htmldocument = new HtmlDocument();
            htmldocument.LoadHtml(html);

            var list = new List<WebPTT>();
            for (int i = 1; i <= pages; i++)
            {
                try
                {
                    if (i > 1)
                    {
                        handler = new HttpClientHandler();
                        handler.CookieContainer = new CookieContainer();
                        uri = new Uri(str_url);
                        handler.CookieContainer.Add(uri, new Cookie("over18", "1"));
                        httpClient = new HttpClient(handler);
                        response = await httpClient.GetAsync(uri);
                        collection = handler.CookieContainer.GetCookies(uri);
                        html = await httpClient.GetStringAsync(str_url);
                        htmldocument = new HtmlDocument();
                        htmldocument.LoadHtml(html);
                    }

                    if (i <= 1)
                    {
                        var getpageup = htmldocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "").Equals("btn-group btn-group-paging")).ToList();
                        foreach (var geturl in getpageup)
                        {
                            PageUp = geturl.Descendants("a").ElementAtOrDefault(1).ChildAttributes("href").FirstOrDefault().Value.ToString();
                        }
                        PageUp = PageUp.Remove(0, 20);
                        PageUp = PageUp.Remove(PageUp.IndexOf('.'), 5);
                        PageUp.Trim();
                        //Console.WriteLine("Sub: " + PageUp);
                    }
                    else
                    {
                        PageUp = (Convert.ToInt32(PageUp) - 1).ToString();
                    }
                    //System.Threading.Thread.Sleep(50);//

                    var divs =
                        htmldocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "").Equals("r-ent")).ToList();

                    foreach (var div in divs)
                    {
                        if (
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("公告")
                            ||
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("徵求")
                            )
                            continue;
                        WebPTT webptt = new WebPTT();
                        if (div.Descendants("div").FirstOrDefault().InnerText.Trim().Contains("X"))
                        {
                            webptt.Popularity_ptt = webptt.Popularity_ptt = div.Descendants("div").FirstOrDefault().InnerText.Trim().Replace('X', '-');
                        }
                        else if (div.Descendants("div").FirstOrDefault().InnerText.Trim() == "")
                        {
                            webptt.Popularity_ptt = "0";
                        }
                        else
                        {
                            webptt.Popularity_ptt = div.Descendants("div").FirstOrDefault().InnerText.Trim();
                        }

                        webptt.title_ptt = div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim();
                        webptt.author_ptt = div.Descendants("div").ElementAtOrDefault(2).Descendants("div").FirstOrDefault().InnerText.Trim();

                        try
                        {
                            webptt.URL_ptt = div.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;
                        }
                        catch (Exception)
                        {
                            webptt.URL_ptt = "";
                        }

                        #region 爬內文
                        if (webptt.URL_ptt != "" && webptt.URL_ptt != null)
                        {

                            try
                            {
                                handler = new HttpClientHandler();
                                handler.CookieContainer = new CookieContainer();
                                uri = new Uri(baseurl + webptt.URL_ptt);
                                handler.CookieContainer.Add(uri, new Cookie("over18", "1"));
                                httpClient = new HttpClient(handler);
                                response = await httpClient.GetAsync(uri);
                                collection = handler.CookieContainer.GetCookies(uri);
                                html = await httpClient.GetStringAsync(baseurl + webptt.URL_ptt);
                                htmldocument = new HtmlDocument();
                                htmldocument.LoadHtml(html);

                                var contents =
                                           htmldocument.DocumentNode.Descendants("div")
                                           .Where(node => node.GetAttributeValue("class", "").Equals("bbs-screen bbs-content")).ToList();
                                foreach (var content in contents)
                                {
                                    webptt.InnerText_ptt = content.InnerText;
                                }
                            }
                            catch (Exception x)
                            {
                                Console.WriteLine("Context :" + x.Message);
                            }
                        }


                        #endregion

                        list.Add(webptt);

                    }
                    Console.WriteLine(PageUp);
                    str_url = baseurl + "/bbs/Gossiping/index" + PageUp + ".html";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                progressBar1.Value += step;
                //Console.Write(" " + i.ToString());
                foreach (var token in list)
                {
                    File.AppendAllText("log.text", token.Popularity_ptt + "   " + token.title_ptt + "   " + token.author_ptt + " " + token.URL_ptt + "\n");
                    dataGridView1.Rows.Add(token.Popularity_ptt, token.title_ptt, token.author_ptt, baseurl + token.URL_ptt, token.InnerText_ptt);
                }
                list.Clear();
            }

            #region Result
            progressBar1.Value = 0;
            Console.WriteLine("Done");
            Console.WriteLine($"TotalData: {dataGridView1.RowCount.ToString()}");
            #endregion
        }


        private async Task PTTWebCrawlerasync(int pages, string str_board)
        {
            this.WindowState = FormWindowState.Minimized;
            Console.WriteLine("PTT Crawler is Running");
            string baseurl = "https://www.ptt.cc";
            string str_index = "index.html";
            string PageUp = "";
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            int step = 100 / pages;
            //string str_url = "https://www.ptt.cc/bbs/Gossiping/index.html";
            string str_url = $"{baseurl}/bbs/{str_board}/{str_index}";
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            Uri uri = new Uri(str_url);
            //Uri uris = new Uri("https://www.ptt.cc/bbs/Gossiping/");
            Uri uris = new Uri($"{baseurl}/bbs/{str_board}/");
            handler.CookieContainer.Add(uri, new Cookie("over18", "1")); // Adding a Cookie
                                                                         //handler.CookieContainer.Add(new Cookie("over18", "1") { Domain = uris.Host }); // Adding a Cookie

            HttpClient httpClient = new HttpClient(handler);
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            string html = await httpClient.GetStringAsync(str_url);
            CookieCollection collection = handler.CookieContainer.GetCookies(uri); // Retrieving a                     
            HtmlDocument htmldocument = new HtmlDocument();
            htmldocument.LoadHtml(html);
            //MessageBox.Show(str_url);
            var list = new List<WebPTT>();
            for (int i = 1; i <= pages; i++)
            {
                try
                {
                    if (i > 1)
                    {
                        handler = new HttpClientHandler();
                        handler.CookieContainer = new CookieContainer();
                        uri = new Uri(str_url);
                        handler.CookieContainer.Add(uri, new Cookie("over18", "1"));
                        httpClient = new HttpClient(handler);
                        response = await httpClient.GetAsync(uri);
                        collection = handler.CookieContainer.GetCookies(uri);
                        html = await httpClient.GetStringAsync(str_url);
                        htmldocument = new HtmlDocument();
                        htmldocument.LoadHtml(html);
                    }

                    var getpageup = htmldocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("btn-group btn-group-paging")).ToList();
                    foreach (var geturl in getpageup)
                    {
                        PageUp = geturl.Descendants("a").ElementAtOrDefault(1).ChildAttributes("href").FirstOrDefault().Value.ToString();
                    }

                    //System.Threading.Thread.Sleep(50);

                    var divs =
                        htmldocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "").Equals("r-ent")).ToList();

                    foreach (var div in divs)
                    {
                        if (
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("公告")
                            ||
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("徵求")
                            ||
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("板務")
                            ||
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("問卷")
                            )
                            continue;
                        WebPTT webptt = new WebPTT();
                        if (div.Descendants("div").FirstOrDefault().InnerText.Trim().Contains("X"))
                        {
                            webptt.Popularity_ptt = webptt.Popularity_ptt = div.Descendants("div").FirstOrDefault().InnerText.Trim().Replace('X', '-');
                        }
                        else if (div.Descendants("div").FirstOrDefault().InnerText.Trim() == "")
                        {
                            webptt.Popularity_ptt = "0";
                        }
                        else
                        {
                            webptt.Popularity_ptt = div.Descendants("div").FirstOrDefault().InnerText.Trim();
                        }

                        webptt.title_ptt = div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim();
                        webptt.author_ptt = div.Descendants("div").ElementAtOrDefault(2).Descendants("div").FirstOrDefault().InnerText.Trim();

                        try
                        {
                            webptt.URL_ptt = div.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;
                        }
                        catch (Exception)
                        {
                            webptt.URL_ptt = null;
                        }

                        #region 爬內文
                        if (webptt.URL_ptt != "" && webptt.URL_ptt != null)
                        {

                            try
                            {
                                handler = new HttpClientHandler();
                                handler.CookieContainer = new CookieContainer();
                                uri = new Uri(baseurl + webptt.URL_ptt);
                                handler.CookieContainer.Add(uri, new Cookie("over18", "1"));
                                httpClient = new HttpClient(handler);
                                response = await httpClient.GetAsync(uri);
                                collection = handler.CookieContainer.GetCookies(uri);
                                html = await httpClient.GetStringAsync(baseurl + webptt.URL_ptt);
                                htmldocument = new HtmlDocument();
                                htmldocument.LoadHtml(html);

                                var contents =
                                           htmldocument.DocumentNode.Descendants("div")
                                           .Where(node => node.GetAttributeValue("class", "").Equals("bbs-screen bbs-content")).ToList();
                                foreach (var content in contents)
                                {
                                    try
                                    {
                                        webptt.InnerText_ptt = content.InnerText;
                                        webptt.InnerText_ptt = webptt.InnerText_ptt.Insert(webptt.InnerText_ptt.IndexOf("看板"), "\n");
                                        webptt.InnerText_ptt = webptt.InnerText_ptt.Insert(webptt.InnerText_ptt.IndexOf("時間"), "\n");
                                    }
                                    catch (Exception Exwebptt)
                                    {
                                        Console.WriteLine($"WebPtt Cotent phase Error:{Exwebptt.Message}");
                                    }

                                }

                                var pushs =
                                      htmldocument.DocumentNode.Descendants("div")
                                           .Where(node => node.GetAttributeValue("class", "").Equals("push")).ToList();
                                List<WebPTT_Push> List_webptt_push = new List<WebPTT_Push>();
                                foreach (var push in pushs)
                                {
                                    try
                                    {
                                        WebPTT_Push obpush = new WebPTT_Push();
                                        obpush.push_tag = push.Descendants("span").ElementAtOrDefault(0).InnerText.Trim();
                                        obpush.user_id = push.Descendants("span").ElementAtOrDefault(1).InnerText.Trim();
                                        obpush.context = push.Descendants("span").ElementAtOrDefault(2).InnerText.Trim();
                                        obpush.datetime = push.Descendants("span").ElementAtOrDefault(3).InnerText.Trim();
                                        List_webptt_push.Add(obpush);
                                    }
                                    catch (Exception ExGetPushData)
                                    {
                                        Console.WriteLine($"Get Push info Data Error:{ExGetPushData.Message}");
                                    }
                                }
                                webptt.ptt_Push_info = List_webptt_push;
                                List_webptt_push.Clear();//Clear Data
                            }
                            catch (Exception x)
                            {
                                Console.WriteLine("Context :" + x.Message);
                            }
                        }


                        #endregion

                        list.Add(webptt);

                    }
                    //Console.WriteLine(PageUp);
                    //str_url = baseurl + "/bbs/Gossiping/index" + PageUp + ".html";
                    //str_url = $"{baseurl}/{PageUp}";
                    str_url = $"{baseurl}{PageUp}";
                    Console.WriteLine(str_url);
                    //$"{baseurl}/bbs/{str_board}/"
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Crawling Error: {e.Message}");
                }
                progressBar1.Value += step;
                //Console.Write(" " + i.ToString());
                foreach (var token in list)
                {
                    File.AppendAllText("log.text", token.Popularity_ptt + "   " + token.title_ptt + "   " + token.author_ptt + " " + token.URL_ptt + "\n");
                    dataGridView1.Rows.Add(token.Popularity_ptt, token.title_ptt, token.author_ptt, baseurl + token.URL_ptt, token.InnerText_ptt);
                }
                list.Clear();


            }

            #region Result
            progressBar1.Value = 0;
            Console.Clear();
            Console.WriteLine("Done");
            Console.WriteLine("TotalData:" + dataGridView1.RowCount.ToString());
            #endregion
            this.WindowState = FormWindowState.Normal;
        }

        private async Task PTTWebCrawlerasync_DB(int pages, string str_board)
        {
            this.WindowState = FormWindowState.Minimized;
            Console.WriteLine("PTT Crawler_DB is Running");
            string baseurl = "https://www.ptt.cc";
            string str_index = "index.html";
            string PageUp = "";
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            progressBar1.Minimum = 0;
            int step = 100 / pages;
            //string str_url = "https://www.ptt.cc/bbs/Gossiping/index.html";
            string str_url = $"{baseurl}/bbs/{str_board}/{str_index}";
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            Uri uri = new Uri(str_url);
            //Uri uris = new Uri("https://www.ptt.cc/bbs/Gossiping/");
            Uri uris = new Uri($"{baseurl}/bbs/{str_board}/");
            handler.CookieContainer.Add(uri, new Cookie("over18", "1")); // Adding a Cookie
                                                                         //handler.CookieContainer.Add(new Cookie("over18", "1") { Domain = uris.Host }); // Adding a Cookie

            HttpClient httpClient = new HttpClient(handler);
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            string html = await httpClient.GetStringAsync(str_url);
            CookieCollection collection = handler.CookieContainer.GetCookies(uri); // Retrieving a                     
            HtmlDocument htmldocument = new HtmlDocument();
            htmldocument.LoadHtml(html);
            //MessageBox.Show(str_url);
            var list = new List<WebPTT>();
            for (int i = 1; i <= pages; i++)
            {
                try
                {
                    if (i > 1)
                    {
                        handler = new HttpClientHandler();
                        handler.CookieContainer = new CookieContainer();
                        uri = new Uri(str_url);
                        handler.CookieContainer.Add(uri, new Cookie("over18", "1"));
                        httpClient = new HttpClient(handler);
                        response = await httpClient.GetAsync(uri);
                        collection = handler.CookieContainer.GetCookies(uri);
                        html = await httpClient.GetStringAsync(str_url);
                        htmldocument = new HtmlDocument();
                        htmldocument.LoadHtml(html);
                    }

                    var getpageup = htmldocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Equals("btn-group btn-group-paging")).ToList();
                    foreach (var geturl in getpageup)
                    {
                        PageUp = geturl.Descendants("a").ElementAtOrDefault(1).ChildAttributes("href").FirstOrDefault().Value.ToString();
                    }

                    //System.Threading.Thread.Sleep(50);

                    var divs =
                        htmldocument.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "").Equals("r-ent")).ToList();

                    foreach (var div in divs)
                    {
                        if (
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("公告")
                            ||
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("徵求")
                            ||
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("板務")
                            ||
                            div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim().Contains("問卷")
                            )
                            continue;
                        WebPTT webptt = new WebPTT();
                        if (div.Descendants("div").FirstOrDefault().InnerText.Trim().Contains("X"))
                        {
                            webptt.Popularity_ptt = webptt.Popularity_ptt = div.Descendants("div").FirstOrDefault().InnerText.Trim().Replace('X', '-');
                        }
                        else if (div.Descendants("div").FirstOrDefault().InnerText.Trim() == "")
                        {
                            webptt.Popularity_ptt = "0";
                        }
                        else
                        {
                            webptt.Popularity_ptt = div.Descendants("div").FirstOrDefault().InnerText.Trim();
                        }

                        webptt.title_ptt = div.Descendants("div").ElementAtOrDefault(1).InnerText.Trim();
                        webptt.author_ptt = div.Descendants("div").ElementAtOrDefault(2).Descendants("div").FirstOrDefault().InnerText.Trim();

                        try
                        {
                            webptt.URL_ptt = div.Descendants("a").FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;
                        }
                        catch (Exception)
                        {
                            webptt.URL_ptt = null;
                        }

                        #region 爬內文
                        if (webptt.URL_ptt != "" && webptt.URL_ptt != null)
                        {

                            try
                            {
                                handler = new HttpClientHandler();
                                handler.CookieContainer = new CookieContainer();
                                uri = new Uri(baseurl + webptt.URL_ptt);
                                handler.CookieContainer.Add(uri, new Cookie("over18", "1"));
                                httpClient = new HttpClient(handler);
                                response = await httpClient.GetAsync(uri);
                                collection = handler.CookieContainer.GetCookies(uri);
                                html = await httpClient.GetStringAsync(baseurl + webptt.URL_ptt);
                                htmldocument = new HtmlDocument();
                                htmldocument.LoadHtml(html);

                                var contents =
                                           htmldocument.DocumentNode.Descendants("div")
                                           .Where(node => node.GetAttributeValue("class", "").Equals("bbs-screen bbs-content")).ToList();
                                foreach (var content in contents)
                                {
                                    try
                                    {
                                        webptt.InnerText_ptt = content.InnerText;
                                        webptt.InnerText_ptt = webptt.InnerText_ptt.Insert(webptt.InnerText_ptt.IndexOf("看板"), "\n");
                                        webptt.InnerText_ptt = webptt.InnerText_ptt.Insert(webptt.InnerText_ptt.IndexOf("時間"), "\n");
                                    }
                                    catch (Exception Exwebptt)
                                    {
                                        Console.WriteLine($"WebPtt Cotent phase Error:{Exwebptt.Message}");
                                    }

                                }

                                var pushs =
                                      htmldocument.DocumentNode.Descendants("div")
                                           .Where(node => node.GetAttributeValue("class", "").Equals("push")).ToList();
                                List<WebPTT_Push> List_webptt_push = new List<WebPTT_Push>();
                                foreach (var push in pushs)
                                {
                                    try
                                    {
                                        WebPTT_Push obpush = new WebPTT_Push();
                                        obpush.push_tag = push.Descendants("span").ElementAtOrDefault(0).InnerText.Trim();
                                        obpush.user_id = push.Descendants("span").ElementAtOrDefault(1).InnerText.Trim();
                                        obpush.context = push.Descendants("span").ElementAtOrDefault(2).InnerText.Trim();
                                        obpush.datetime = push.Descendants("span").ElementAtOrDefault(3).InnerText.Trim();
                                        List_webptt_push.Add(obpush);
                                    }
                                    catch (Exception ExGetPushData)
                                    {
                                        Console.WriteLine($"Get Push info Data Error:{ExGetPushData.Message}");
                                    }
                                }
                                webptt.ptt_Push_info = new List<WebPTT_Push>(List_webptt_push);
                                List_webptt_push.Clear();//Clear Data
                            }
                            catch (Exception x)
                            {
                                Console.WriteLine("Context :" + x.Message);
                            }
                        }


                        #endregion

                        list.Add(webptt);

                    }
                    
                    str_url = $"{baseurl}{PageUp}";
                    Console.WriteLine(str_url);
                    //$"{baseurl}/bbs/{str_board}/"
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Crawling Error: {e.Message}");
                }
                progressBar1.Value += step;

                #region DB_Write
                string dbHost = "127.0.0.1";//資料庫位址
                string dbUser = "PTTUser";//資料庫使用者帳號
                string dbPass = "root";//資料庫使用者密碼
                string dbName = "webptt";//資料庫名稱
                string connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand command = conn.CreateCommand();
                MySqlCommand command1 = conn.CreateCommand();

                try
                {
                    /*Insert into PTTDATA (ptt_pop,ptt_title,ptt_author,ptt_url,ptt_context,ptt_date) values('Test_Title','U9062009','URL','Context','Date');*/
                    conn.Open();
                    foreach (var token in list)
                    {
                        if (token.InnerText_ptt != null)
                        {
                            dataGridView1.Rows.Add(token.Popularity_ptt, token.title_ptt, token.author_ptt, baseurl + token.URL_ptt, token.InnerText_ptt);
                            command.CommandText = @"Insert into PTTDATA (ptt_pop,ptt_title,ptt_author,ptt_url,ptt_context,ptt_date) values('"
                    + token.Popularity_ptt + "','" + token.title_ptt + "','" + token.author_ptt + "','" + baseurl + token.URL_ptt + "','" + token.InnerText_ptt
                    + "','" + "DATE" + "')";
                            command.ExecuteNonQuery();
                        }
                        try
                        {
                            foreach (var pushtoken in token.ptt_Push_info)
                            {
                                if (pushtoken.context != null)
                                {
                                    command1.CommandText = @"Insert into PTTpushDATA (push_Tag,push_User,push_Content,push_URL,push_Date) values('"
                                + pushtoken.push_tag + "','" + pushtoken.user_id + "','" + pushtoken.context + "','" + token.URL_ptt + "','" + pushtoken.datetime + "')";
                                }
                                command1.ExecuteNonQuery();
                            }
                        }
                        catch(Exception Push_DB)
                        {
                            Console.WriteLine($"Push_DB Error:{Push_DB.Message}");
                        }
                    }
                }
                catch (Exception DBEX)
                {
                    Console.WriteLine($"DB Connect Error:{DBEX.Message}");
                }
                finally
                {
                    conn.Close();
                }
                #endregion
                list.Clear();
            }

            #region Result
            progressBar1.Value = 0;
            Console.Clear();
            Console.WriteLine("Done");
            Console.WriteLine("TotalData:" + dataGridView1.RowCount.ToString());
            Console.Beep();
            #endregion
            this.WindowState = FormWindowState.Normal;
        }
        private void DataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //try
            //{
            //    var target = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            //    if (target != null || target != string.Empty)
            //    {
            //        RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
            //        string s = key.GetValue("").ToString();
            //        //s就是你的預設瀏覽器，不過後面帶了引數，把它截去，不過需要注意的是：不同的瀏覽器後面的引數不一樣！
            //        //"D:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"
            //        System.Diagnostics.Process.Start(s.Substring(0, s.Length - 8), target);
            //    }

            //}
            //catch (Exception E)
            //{
            //    Console.Write(E.Message);
            //}

            try
            {
                FormView view = new FormView();
                var target = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                view.SetValue = target + "\n\n\n";
                view.Show();

            }
            catch (Exception E)
            {
                Console.Write(E.Message);
            }
        }

        private void TabPage1_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    CPage = Convert.ToInt32(textBox1.Text);
                }
                catch (Exception x)
                {
                    Console.WriteLine("Textbox1:" + x.Message);
                    textBox1.Text = "30";
                }
            }
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == this.dataGridView1.Columns["Pop"].Index)
            {
                if (e.Value != null)
                {

                    if (e.Value.ToString().Trim() == "爆")
                    {
                        this.dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                    }
                    else if (e.Value.ToString().Trim().Contains("-"))
                    {
                        this.dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                        //this.dataGridView1.Rows[e.RowIndex].DefaultCellStyle.Font
                    }
                    else
                    {
                        try
                        {
                            int pop = Convert.ToInt32(e.Value.ToString().Trim());
                            if (pop >= 10)
                            {
                                this.dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Yellow;
                            }
                            else
                            {
                                this.dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Green;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            //System.Diagnostics.Debug.WriteLine("Running");
            try
            {
                int i = Convert.ToInt32(textBox1.Text);
                PTTWebCrawlerasync(i, comboBox1.Text);
                tabControl1.SelectTab(tabPage1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PTTWebCrawler:{ex.Message}");
            }
        }

        private void DataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                try
                {
                    var target = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                    if (target != null || target != string.Empty)
                    {
                        RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                        string s = key.GetValue("").ToString();
                        //s就是你的預設瀏覽器，不過後面帶了引數，把它截去，不過需要注意的是：不同的瀏覽器後面的引數不一樣！
                        //"D:\Program Files (x86)\Google\Chrome\Application\chrome.exe" -- "%1"
                        System.Diagnostics.Process.Start(s.Substring(0, s.Length - 8), target);
                    }
                }
                catch (Exception E)
                {
                    Console.Write("KeyPress:" + E.Message);
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            //System.Diagnostics.Debug.WriteLine("Running");
            try
            {
                int i = Convert.ToInt32(textBox1.Text);
                PTTWebCrawlerasync_DB(i, comboBox1.Text);
                tabControl1.SelectTab(tabPage1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PTTWebCrawler:{ex.Message}");
            }
        }
    }
}
#endregion