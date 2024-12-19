using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using CefSharp;
using CefSharp.WinForms;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.IO;

namespace TNH_DATA_IMPORT
{
    public partial class frmmain : Form
    {
        SLDHSXKD_DAO tk1 = new SLDHSXKD_DAO();
        private Thread seleniumThread = null;
        private IWebDriver driver;
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(frmmain));

        private ChromiumWebBrowser browser;
        private string cookiePath = Path.Combine(Environment.CurrentDirectory, "cookies");


        public frmmain()
        {
            InitializeComponent();


        }

        public struct data_url
        {
            public data_url(string strTen, string strTabname, string strXpath, string strLink)
            {
                TENHT = strTen;
                TAB_NAME = strTabname;
                XPATH = strXpath;
                LINKDN = strLink;

            }

            public string TENHT { get; private set; }
            public string LINKDN { get; private set; }
            public string XPATH { get; private set; }
            public string TAB_NAME { get; private set; }
        }


        public DataTable ConvertHtmlTableToDataTable(string htmlTable)
        {
            DataTable dataTable = new DataTable();
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(htmlTable);

            // Lấy các hàng trong tbody
            var rowNodes = document.DocumentNode.SelectNodes("//tr");
            if (rowNodes == null)
                throw new Exception("Không có hàng nào trong bảng!");

            // Lấy tiêu đề từ hàng đầu tiên (nếu có)
            var headerNodes = rowNodes[0].SelectNodes("th | td");
            if (headerNodes != null)
            {
                foreach (var headerNode in headerNodes)
                {
                    string columnName = headerNode.InnerText.Trim();
                    dataTable.Columns.Add(columnName.Replace("&nbsp;", ""));
                }
            }

            // Lấy dữ liệu từ các hàng còn lại
            for (int i = 1; i < rowNodes.Count; i++)
            {
                var cellNodes = rowNodes[i].SelectNodes("td");
                if (cellNodes == null) continue;

                DataRow dataRow = dataTable.NewRow();
                for (int j = 0; j < cellNodes.Count; j++)
                {
                    dataRow[j] = cellNodes[j].InnerText.Trim().Replace("&nbsp;", "");
                }
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        private Task WaitForPageLoadAsync(ChromiumWebBrowser browser)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                if (!args.IsLoading) // Trang đã tải xong
                {
                    browser.LoadingStateChanged -= handler; // Hủy đăng ký sự kiện
                    tcs.TrySetResult(true); // Báo hoàn thành Task
                }
            };

            browser.LoadingStateChanged += handler; // Đăng ký sự kiện
            return tcs.Task;
        }

        private Task Browser_FrameLoadEndAsync(ChromiumWebBrowser browser)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {

                if (browser != null)
                    if (browser.Address.Contains("https://accounts.google.com/v3/signin"))
                    {
                        // Điền email và click vào nút "Next"
                        browser.ExecuteScriptAsync(@"
                                                    let emailField = document.querySelector('#identifierId');
                                                    if (emailField) {
                                                        emailField.value = 'duongthanhhien9187@gmail.com';
                                                        document.querySelector('#identifierNext').click();
                                                    }
                                                    ");

                        // Chờ và xử lý bước tiếp theo (mật khẩu)
                        Task.Delay(3000).Wait();
                        browser.ExecuteScriptAsync(@"
                                                    let passwordField = document.querySelector('#password > div.aCsJod.oJeWuf > div > div.Xb9hP > input');
                                                    if (passwordField) {
                                                        passwordField.value = 'angellifehln';
                                                        document.querySelector('#passwordNext').click();
                                                    }
                                                    ");

                    }
                    else if (browser.Address.Contains("https://messages.google.com/web/welcome"))
                    {
                        Task.Delay(3000).Wait();
                        browser.ExecuteScriptAsync(@"
                        document.querySelector('body > mw-app > mw-bootstrap > div > main > mw-welcome-page-container > div > div > div > div:nth-child(3) > button > span.mat-mdc-button-touch-target').click();
                        ");
                    }
                if (!args.IsLoading) // Trang đã tải xong
                {
                    browser.LoadingStateChanged -= handler; // Hủy đăng ký sự kiện
                    tcs.TrySetResult(true); // Báo hoàn thành Task
                }
            };

            browser.LoadingStateChanged += handler; // Đăng ký sự kiện
            return tcs.Task;
        }

        private async Task LoginInNewTab(string url, string username, string password)
        {

            var settings = new CefSettings();
            settings.CachePath = cookiePath;
            Cef.Initialize(settings);

            // URL của Google Messages Web
            string messagesUrl = "https://messages.google.com/web";

            browser = new ChromiumWebBrowser(messagesUrl)
            {
                Dock = DockStyle.Fill
            };
            maintab.SelectedTab = maintab.TabPages["tb_ggms"];
            maintab.TabPages["tb_ggms"].Controls.Add(browser);

            // Đợi tải trang xong
            await Browser_FrameLoadEndAsync(browser);
            AddLog(listBox1, DateTime.Now.ToString() + " Đăng nhập GG MS thành công");

            // Tạo Tab mới và trình duyệt
            //var newTab = new TabPage($"Tab {maintab.TabCount + 1}");

            // Gắn trình duyệt vào Tab
            //newTab.Controls.Add(browser);
            //maintab.TabPages.Add(newTab);
            //maintab.SelectedTab = newTab;

            browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill
            };

            maintab.TabPages["tb_ccbs_home"].Controls.Add(browser);
            maintab.SelectedTab = maintab.TabPages["tb_ccbs_home"];
            // Đợi tải trang xong
            await WaitForPageLoadAsync(browser);

            // Thực thi script để đăng nhập
            string script = $@"
                                    document.getElementById('username').value = '{username}';
                                    document.getElementById('passWord').value = '{password}';
                                    document.getElementById('btnLogin').click();
                                ";

            var result = await browser.GetMainFrame().EvaluateScriptAsync(script);
            if (result.Success)
            {
                AddLog(listBox1, DateTime.Now.ToString() + " Đăng nhập CCBS thành công");
            }
            else
            {
                AddLog(listBox1, DateTime.Now.ToString() + " Đăng nhập CCBS thất bại");
            }

            DateTime dtResult = DateTime.ParseExact(txt_thang.Text, "yyyyMM", CultureInfo.InvariantCulture);
            string tungay = dtResult.AddDays((-dtResult.Day) + 1).ToString("dd/MM/yyyy");
            string denngay = dtResult.AddMonths(1).AddDays((-dtResult.Day)).ToString("dd/MM/yyyy");

            List<data_url> data = new List<data_url>();
            List<data_url> data_download = new List<data_url>();
            //đk gói mới 1_DKM_GOICUOC_
            data.Add(new data_url("1_DKM_GOICUOC", "tb_dkg_moi", "//*[@id='ContentTable']/div/table[1]/tbody", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=xkvuxz5VZZHeHGUIGU5vgiqgmkexkvuxz5hiejgtmqegpg~&{ykx=minhly_tnh&vgiqgmk=0&tevgiqgmk=&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&jut|o=&h{{i{i=&tm{uoezn=0&jkttmg="
                                        + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=%26%23272%3B%26%23259%3Bng+k%26%23253%3B+m%26%237899%3Bi&yzgz{y=1&{ykx=minhly_tnh&vgiqgmk=0&tevgiqgmk=&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&jut|o=&h{{i{i=&tm{uoezn=0&jkttmg="
                                        + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=%26%23272%3B%26%23259%3Bng+k%26%23253%3B+m%26%237899%3Bi&yzgz{y=1"));

            //hủy gói 2_HUY_GOICUOC_
            data.Add(new data_url("2_HUY_GOICUOC", "tb_dkg_huy", "//*[@id='ContentTable']/div/table[1]/tbody", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=xkvuxz5VZZHeHGUIGU5vgiqgmkexkvuxz5hiejgtmqegpg~&{ykx=minhly_tnh&vgiqgmk=0&tevgiqgmk=&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&jut|o=&h{{i{i=&tm{uoezn=0&jkttmg="
                                        + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=H%26%237911%3By&yzgz{y=3&{ykx=minhly_tnh&vgiqgmk=0&tevgiqgmk=&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&jut|o=&h{{i{i=&tm{uoezn=0&jkttmg="
                                        + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=H%26%237911%3By&yzgz{y=3"));

            //1896 3_PTM_
            data.Add(new data_url("3_PTM", "tb_ptm", "//*[@id='ajaxDSBC']/table", "http://ccbs.vnpt.vn/ccbs/main?iutlomLork=report/pttb_bc_cuoingay/bcct_cuoingay_ajax&yZ{Tmg=" + tungay + "&yJktTmg=" + denngay + "&yjut|owr=0&ySgHI=0&y[ykx=0&yZxgtmZngoNJ=5&yHguIguIZ=1&yJYNJ=0,3,5,9,15,16&yTmgehi=1&yJT=0"));

            //4456 4_DDTT_KH_
            data.Add(new data_url("4_DDTT_KH", "tb_kh", @"/html/body/table/tbody/tr/td[2]/table", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=hguigu5hguiguekrugjejgtmq5krugjeinozokz4px~sr&vyeyinksg=CCS_TNH.&vyegmktziujk=TNH&vye{ykxoj=minhly_tnh&vyez{tmg=" + tungay + "&vyejkttmg=" + denngay + "&pxXkvuxzZvk=3&tm{uoezn=D%26%23432%3B%26%23417%3Bng+Minh+L%26%23253%3B&pxXkvuxzJgzgYu{xik=CMDVOracleDS"));

            //home combo ptm 5_HOME_PTM_
            data.Add(new data_url("5_HOME_PTM", "tb_home_ptm", @"/html/body/table/tbody/tr/td[2]/table", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=sz|5xkvuxz5xvzenusk5nuskeinozokz4px~sr&vyz{tmg%7F=" + tungay + "&vyjkttmg%7F=" + denngay + "&vyyinksg=TNH&vygmktz=TNH&vyyinksg=TNH&vygmktztgsk=T%26%23226%3By+Ninh&pxXkvuxzZ%7Fvk=3&vy{ykx=minhly_tnh"));

            //home combo huy 5_HOME_HUY_
            data.Add(new data_url("5_HOME_HUY", "tb_home_huy", @"/html/body/table/tbody/tr/td[2]/table", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=sz|5xkvuxz5xvzenusk5nuskezn{khguen{%7Fejkzgor4px~sr&vyz{tmg%7F=" + tungay + "&vyjkttmg%7F=" + denngay + "&vyjut|owr=0&vyyinksg=TNH&vygmktz=TNH&vyvgiqgmkoj=0&vygmktztgsk=T%26%23226%3By+Ninh&pxXkvuxzZ%7Fvk=3&v{ykx=minhly_tnh"));

            await LoadUrlsToTabsSequentially(data);

            tk1.KK_DDTS_1090(0, "999", "999", txt_thang.Text);

            AddLog(listBox1, DateTime.Now.ToString() + " Cập nhật dữ liệu xong");

        }

        private async Task LoadUrlsToTabsSequentially(List<data_url> urls)
        {
            foreach (var url in urls)
            {
                // Tạo một TabPage mới
                //var newTab = new TabPage(url.TENHT);

                // Tạo ChromiumWebBrowser và gắn vào TabPage
                browser = new ChromiumWebBrowser(url.LINKDN)
                {
                    Dock = DockStyle.Fill
                };

                //newTab.Controls.Add(browser);
                //tabControl1.TabPages.Add(newTab);

                // Hiển thị tab hiện tại
                //tabControl1.SelectedTab = newTab;

                maintab.TabPages[url.TAB_NAME].Controls.Add(browser);
                maintab.SelectedTab = maintab.TabPages[url.TAB_NAME];
                // Đợi tải trang xong
                await WaitForPageLoadAsync(browser);

                DataTable dataTable = new DataTable();

                string script = $@"(function() {{
                            var element = document.evaluate(
                                ""{url.XPATH}"",
                                document,
                                null,
                                XPathResult.FIRST_ORDERED_NODE_TYPE,
                                null
                            ).singleNodeValue;

                            if (element) {{
                                return element.outerHTML; // Trả về HTML của bảng
                            }}
                            return null; // Không tìm thấy phần tử
                        }})();";

                var result = browser.GetMainFrame().EvaluateScriptAsync(script).Result;

                if (result.Success && result.Result != null)
                {
                    string htmlTable = result.Result.ToString();
                    dataTable = ConvertHtmlTableToDataTable(htmlTable);
                    await NHAP_DULIEU(dataTable, url.TENHT);

                    AddLog(listBox1, DateTime.Now.ToString() + " DataTable với " + dataTable.Rows.Count.ToString() + " " + url.TENHT);
                }
                else
                {
                    AddLog(listBox1, DateTime.Now.ToString() + "Không nhận được kết quả từ trình duyệt " + url.TENHT);
                }

                AddLog(listBox1, "Đã tải xong " + url.TENHT);
            }
        }

        private async Task LoginInCTVXHH(string url, string username, string password)
        {
            // Tạo Tab mới và trình duyệt
            //var newTab = new TabPage($"Tab {maintab.TabCount + 1}");
            browser = new ChromiumWebBrowser("https://shop-ctv.vnpt.vn/backend/web/index.php?r=site%2Flogin")
            {
                Dock = DockStyle.Fill
            };

            // Gắn trình duyệt vào Tab
            //newTab.Controls.Add(browser);
            //maintab.TabPages.Add(newTab);
            //maintab.SelectedTab = newTab;

            maintab.TabPages["tb_ctv_xhh"].Controls.Add(browser);
            maintab.SelectedTab = maintab.TabPages["tb_ctv_xhh"];
            // Đợi tải trang xong
            await WaitForPageLoadAsync(browser);

            // Thực thi script để đăng nhập
            string script = $@"
                            document.getElementById('loginform-username').value = '{username}';
                            document.getElementById('loginform-password').value = '{password}';
                        ";
            var result = await browser.GetMainFrame().EvaluateScriptAsync(script);

            script = $@"document.querySelector('button[name=""login-button""]').click();";

            result = await browser.GetMainFrame().EvaluateScriptAsync(script);

            browser.Load("https://shop-ctv.vnpt.vn/backend/web/index.php?r=order%2Findex&OrderSearch%5Border_code%5D=&OrderSearch%5Bbatch_code%5D=&OrderSearch%5Bbatch_type%5D=&OrderSearch%5Bhinh_thuc_ban%5D=&OrderSearch%5Bvnpt_it_ma_gd%5D=&OrderSearch%5BphoneNumber%5D=&OrderSearch%5Buser_code%5D=&OrderSearch%5Bcat_id%5D=&OrderSearch%5Bproduct_name%5D=&OrderSearch%5Bttkd_export%5D=50&OrderSearch%5Bpbh_export%5D=&OrderSearch%5Bstatus_export%5D=&OrderSearch%5BfromDate%5D=06%2F12%2F2024&OrderSearch%5BtoDate%5D=06%2F12%2F2024&OrderSearch%5Bso_dt_sim%5D=&OrderSearch%5Bpayment_type%5D=&OrderSearch%5Buser_phone_number%5D=&OrderSearch%5Border_code%5D=&OrderSearch%5Bphone_number%5D=&OrderSearch%5Buser_code%5D=&OrderSearch%5Bcat_id%5D=&OrderSearch%5Bproduct_name%5D=&OrderSearch%5Bpbh_id%5D=&OrderSearch%5Borg_ids%5D=&OrderSearch%5Bbatch_type%5D=&OrderSearch%5Bbatch_code%5D=&OrderSearch%5Bhinh_thuc_ban%5D=&OrderSearch%5Bstatus%5D=&OrderSearch%5Bvnpt_it_ma_gd%5D=&OrderSearch%5Bma_nguoi_pt_ctv%5D=&OrderSearch%5Bvai_tro%5D=&OrderSearch%5Buser_phone_number%5D=&OrderSearch%5Bfrom_date%5D=01%2F12%2F2024&OrderSearch%5Bto_date%5D=06%2F12%2F2024&OrderSearch%5Bso_dt_sim%5D=&OrderSearch%5Bpayment_type%5D=");

            // Đợi tải trang xong
            await WaitForPageLoadAsync(browser);



            string xpath = "//*[@id='w1 - container']/table";
            script = $@"(function() {{
                            var element = document.evaluate(
                                ""{xpath}"",
                                document,
                                null,
                                XPathResult.FIRST_ORDERED_NODE_TYPE,
                                null
                            ).singleNodeValue;

                            if (element) {{
                                return element.outerHTML; // Trả về HTML của bảng
                            }}
                            return null; // Không tìm thấy phần tử
                        }})();";

            result = browser.GetMainFrame().EvaluateScriptAsync(script).Result;

            DataTable dataTable = new DataTable();
            if (result.Success && result.Result != null)
            {
                string htmlTable = result.Result.ToString();
                dataTable = ConvertHtmlTableToDataTable(htmlTable);

                AddLog(listBox1, DateTime.Now.ToString() + " DataTable với " + dataTable.Rows.Count.ToString() + " " + "CTV XHH");
            }


            if (result.Success)
            {
                AddLog(listBox1, DateTime.Now.ToString() + " Đăng nhập CTVXHH thành công");

                //*[@id="w1-container"]/table
            }
            else
            {
                AddLog(listBox1, DateTime.Now.ToString() + " Đăng nhập CTVXHH thất bại");
            }

        }

        public async Task NHAP_DULIEU(DataTable dt, string loai)
        {
            DataTable dt_check = new DataTable();
            DataRow[] dr_check;

            int count = 0;

            dt_check = tk1.ExecuteQuery("select * from DULIEU_TNH.CHOT_DICH_SOLIEU_BANG where chot = 1 and thang = '" + txt_thang.Text + "'");

            if (loai == "1_DKM_GOICUOC" || loai == "2_HUY_GOICUOC")
            {
                if (dt.Rows.Count > 1)
                {
                    dr_check = dt_check.Select("BANG = 'BaoCaoGoiCuoc_KM_3816' and chot = 1 and thang = '" + txt_thang.Text + "'");
                    if (dr_check.Length == 0)
                    {
                        lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = loai));
                        AddLog(listBox1, DateTime.Now.ToString() + " Datatable " + loai);

                        int l = tk1.INSERT_CCBS_GOICUOC(loai == "1_DKM_GOICUOC" ? 0 : 1, "", "", "", "", "", "", "", "", "", "", "", "", "", "", txt_thang.Text, ""); //delete dang ky moi

                        foreach (DataRow row in dt.Rows)
                        {
                            try
                            {
                                if (row[0].ToString().Trim() != "")
                                {
                                    int k = tk1.INSERT_CCBS_GOICUOC(2, row[1].ToString(), row[2].ToString(), row[3].ToString()
                                        , row[4].ToString(), row[5].ToString(), row[9].ToString(), row[10].ToString(), row[11].ToString().Trim() == "" ? "01/01/1900" : row[11].ToString()
                                            , row[12].ToString().Trim() == "" ? "01/01/1900" : row[12].ToString(), row[15].ToString(), row[16].ToString(), row[14].ToString()
                                            , row[13].ToString(), row[17].ToString(), txt_thang.Text, row[8].ToString());

                                    count = count + k;
                                    await Task.Delay(50);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex.ToString());
                                AddLog(listBox1, DateTime.Now.ToString() + " " + row[10].ToString() + " " + ex.ToString()
                                                                                                        + " Lỗi file " + "1_DKM_GOICUOC");
                            }
                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[4].ToString()));
                        }

                        AddLog(listBox1, DateTime.Now.ToString() + " Đã nhập " + count.ToString() + " dòng " + loai);
                    }
                }
            }
            else if (loai == "3_PTM")
            {
                dr_check = dt_check.Select("BANG = 'CCBS_AUTO' and chot = 1 and thang = '" + txt_thang.Text + "'");
                if (dr_check.Length == 0)
                {
                    if (dt.Rows.Count > 2)
                    {
                        lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = "3_PTM_"));
                        AddLog(listBox1, DateTime.Now.ToString() + " Datatable " + "3_PTM");

                        int trangthai = 0;
                        string tien_hm;
                        string tien_dc;
                        string tien_dathu;
                        string tien_km = "";
                        string mabc = "";
                        string madv = "";

                        int l = tk1.INSERT_CCBS_AUTO(0, txt_thang.Text, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0");

                        foreach (DataRow row in dt.Rows)
                        {
                            try
                            {
                                if (row[1].ToString().StartsWith("TNH-LD"))
                                    trangthai = 1;
                                else if (row[1].ToString().StartsWith("TNH-CQ"))
                                    trangthai = 2;
                                else if (row[1].ToString().StartsWith("TNH-TL"))
                                    trangthai = 3;
                                else
                                    continue;

                                if (trangthai > 0)
                                {
                                    tien_hm = row[14].ToString().Trim();
                                    tien_dc = row[15].ToString().Trim();
                                    tien_dathu = row[16].ToString().Trim();
                                    tien_km = row[17].ToString().Trim();

                                    if (tien_hm.Length > 0)
                                        tien_hm = tien_hm.Trim().Replace(",", "");
                                    else
                                        tien_hm = "0";

                                    if (tien_dc.Length > 0)
                                        tien_dc = tien_dc.Trim().Replace(",", "");
                                    else
                                        tien_dc = "0";

                                    if (tien_dathu.Length > 0)
                                        tien_dathu = tien_dathu.Trim().Replace(",", "");
                                    else
                                        tien_dathu = "0";

                                    if (tien_km.Length > 0)
                                        tien_km = tien_km.Trim().Replace(",", "");
                                    else
                                        tien_km = "0";


                                    mabc = row[19].ToString();
                                    madv = row[20].ToString();

                                    int k = tk1.INSERT_CCBS_AUTO(1, txt_thang.Text, row[1].ToString(), row[2].ToString(), row[3].ToString()
                                            , row[4].ToString(), row[5].ToString(), row[6].ToString(), row[7].ToString(), row[8].ToString()
                                            , row[9].ToString(), row[10].ToString(), row[11].ToString(), trangthai == 1 ? row[12].ToString() : "", row[13].ToString()
                                            , tien_hm, tien_dc, tien_dathu, tien_km, row[18].ToString(), mabc, madv, row[21].ToString()
                                            , row[23].ToString(), row[24].ToString(), row[25].ToString(), row[26].ToString(), trangthai.ToString());

                                    count = count + k;
                                    await Task.Delay(50);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex.ToString());
                                AddLog(listBox1, DateTime.Now.ToString() + " " + row[4].ToString() + " " + ex.ToString()
                                                                                                        + " Lỗi file " + "3_PTM");
                            }
                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[4].ToString()));
                        }

                        AddLog(listBox1, DateTime.Now.ToString() + " Đã nhập " + count.ToString() + " dòng " + loai);
                    }
                }
            }
            else if (loai == "4_DDTT_KH")
            {
                dr_check = dt_check.Select("BANG = 'DDTT_CCBS_4456' and chot = 1 and thang = '" + txt_thang.Text + "'");
                if (dr_check.Length == 0)
                {
                    if (dt.Rows.Count > 7)
                    {
                        lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = "4_DDTT_KH"));
                        AddLog(listBox1, DateTime.Now.ToString() + " Datatable " + "4_DDTT_KH");

                        string eload = string.Empty;
                        int l = tk1.INSERT_DDTT_KH_4456(0, txt_thang.Text, "", "", "");

                        foreach (DataRow row in dt.Rows)
                        {
                            try
                            {
                                int index = dt.Rows.IndexOf(row);
                                if (index <= 5)
                                    continue;

                                if (row[1].ToString() == "Eload:")
                                {
                                    eload = row[2].ToString();
                                    continue;
                                }

                                if (row[3].ToString() == "")
                                    continue;

                                int k = tk1.INSERT_DDTT_KH_4456(1, txt_thang.Text, eload, row[2].ToString(), row[3].ToString());

                                count = count + k;
                                await Task.Delay(50);
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex.ToString());
                                AddLog(listBox1, DateTime.Now.ToString() + " " + row[4].ToString() + " " + ex.ToString()
                                                                                                        + " Lỗi file " + "4_DDTT_KH");
                            }
                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[2].ToString()));
                        }

                        AddLog(listBox1, DateTime.Now.ToString() + " Đã nhập " + count.ToString() + " dòng " + loai);
                    }
                }
            }
            else if (loai == "5_HOME_PTM" || loai == "5_HOME_HUY")
            {
                if (loai == "5_HOME_PTM")
                {
                    dr_check = dt_check.Select("BANG = 'GOI_HOME_COMBO' and chot = 1 and thang = '" + txt_thang.Text + "'");
                    if (dr_check.Length == 0)
                    {
                        if (dt.Rows.Count > 9)
                        {
                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = "5_HOME_PTM"));
                            AddLog(listBox1, DateTime.Now.ToString() + " Datatable " + "5_HOME_PTM");
                            int l = tk1.INSERT_GOI_HOME(0, txt_thang.Text, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                            foreach (DataRow row in dt.Rows)
                            {
                                try
                                {
                                    int index = dt.Rows.IndexOf(row);
                                    if (index <= 7)
                                        continue;

                                    if (row[3].ToString() == "")
                                        continue;

                                    int k = tk1.INSERT_GOI_HOME(1, txt_thang.Text, row[22].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString()
                                           , row[7].ToString(), row[8].ToString(), row[9].ToString(), row[11].ToString(), row[12].ToString(), row[13].ToString()
                                           , row[14].ToString(), row[15].ToString(), row[17].ToString(), row[18].ToString(), row[19].ToString()
                                               , row[20].ToString(), row[21].ToString(), row[23].ToString(), row[24].ToString(), row[26].ToString());

                                    count = count + k;
                                    await Task.Delay(50);

                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex.ToString());
                                    AddLog(listBox1, DateTime.Now.ToString() + " " + row[4].ToString() + " " + ex.ToString()
                                                                                                            + " Lỗi file " + "5_HOME_PTM");
                                }
                                lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[3].ToString()));
                            }

                            AddLog(listBox1, DateTime.Now.ToString() + " Đã nhập " + count.ToString() + " dòng " + loai);
                        }
                    }
                }
                else if (loai == "5_HOME_HUY")
                {
                    dr_check = dt_check.Select("BANG = 'GOI_HOME_COMBO' and chot = 1 and thang = '" + txt_thang.Text + "'");
                    if (dr_check.Length == 0)
                    {
                        if (dt.Rows.Count > 9)
                        {
                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = "5_HOME_HUY"));
                            AddLog(listBox1, DateTime.Now.ToString() + " Datatable " + "5_HOME_HUY");
                            int l = tk1.INSERT_GOI_HOME_HUY(0, txt_thang.Text, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                            foreach (DataRow row in dt.Rows)
                            {
                                try
                                {
                                    int index = dt.Rows.IndexOf(row);
                                    if (index <= 7)
                                        continue;

                                    if (row[3].ToString() == "")
                                        continue;

                                    int k = tk1.INSERT_GOI_HOME_HUY(1, txt_thang.Text, row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString(), row[8].ToString()
                                                                   , row[10].ToString(), row[9].ToString(), row[11].ToString(), row[13].ToString(), row[12].ToString()
                                                                   , row[14].ToString(), row[15].ToString(), row[16].ToString(), row[19].ToString(), row[17].ToString()
                                                                   , row[20].ToString(), row[21].ToString(), row[22].ToString());

                                    count = count + k;
                                    await Task.Delay(50);

                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex.ToString());
                                    AddLog(listBox1, DateTime.Now.ToString() + " " + row[5].ToString() + " " + ex.ToString()
                                                                                                            + " Lỗi file " + "5_HOME_HUY");
                                }
                                lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[4].ToString()));
                            }

                            AddLog(listBox1, DateTime.Now.ToString() + " Đã nhập " + count.ToString() + " dòng " + loai);
                        }
                    }
                }
            }
        }

        /*
        public void StartSelenium()
        {
            // Chạy Selenium trong một luồng riêng

            try
            {
                AddLog(listBox1, "Khởi động Selenium...");
                driver = new ChromeDriver();

                DataTable dt_check = new DataTable();
                DataRow[] dr_check;
                WebDriverWait wait;
                try
                {
                    AddLog(DateTime.Now.ToString() + " Start")));
                    // Mở trang web
                    driver.Navigate().GoToUrl("http://ccbs.vnpt.vn");

                    // Đợi trang tải (nếu cần)
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => d.FindElement(By.Id("username"))); // Thay "username" bằng id đúng của trường username

                    // Tìm các trường nhập liệu và nhập thông tin
                    IWebElement usernameField = driver.FindElement(By.Id("username")); // Thay "username" bằng id chính xác
                    IWebElement passwordField = driver.FindElement(By.Id("passWord")); // Thay "password" bằng id chính xác
                    IWebElement loginButton = driver.FindElement(By.Id("btnLogin")); // Thay "loginButton" bằng id chính xác

                    // Nhập username và password
                    usernameField.SendKeys(txtusername.Text);
                    passwordField.SendKeys(txtmk.Text);

                    // Nhấn nút đăng nhập
                    loginButton.Click();

                    string expectedUrl = "http://ccbs.vnpt.vn/ccbs/main?1y%7Fyezksvrgzkelork=rg%7Fu{z5iihy5zvreiihy&1iutlomLork=gjsot5otjk~"; // URL sau đăng nhập
                    wait.Until(d => d.Url == expectedUrl);

                    DateTime dtResult = DateTime.ParseExact(txt_thang.Text, "yyyyMM", CultureInfo.InvariantCulture);
                    string tungay = dtResult.AddDays((-dtResult.Day) + 1).ToString("dd/MM/yyyy");
                    string denngay = dtResult.AddMonths(1).AddDays((-dtResult.Day)).ToString("dd/MM/yyyy");

                    List<data_url> data = new List<data_url>();
                    List<data_url> data_download = new List<data_url>();
                    //đk gói mới 1_DKM_GOICUOC_
                    data.Add(new data_url("1_DKM_GOICUOC", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=xkvuxz5VZZHeHGUIGU5vgiqgmkexkvuxz5hiejgtmqegpg~&{ykx=minhly_tnh&vgiqgmk=0&tevgiqgmk=&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&jut|o=&h{{i{i=&tm{uoezn=0&jkttmg="
                                                + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=%26%23272%3B%26%23259%3Bng+k%26%23253%3B+m%26%237899%3Bi&yzgz{y=1&{ykx=minhly_tnh&vgiqgmk=0&tevgiqgmk=&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&jut|o=&h{{i{i=&tm{uoezn=0&jkttmg="
                                                + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=%26%23272%3B%26%23259%3Bng+k%26%23253%3B+m%26%237899%3Bi&yzgz{y=1"));

                    //hủy gói 2_HUY_GOICUOC_
                    data.Add(new data_url("2_HUY_GOICUOC", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=xkvuxz5VZZHeHGUIGU5vgiqgmkexkvuxz5hiejgtmqegpg~&{ykx=minhly_tnh&vgiqgmk=0&tevgiqgmk=&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&jut|o=&h{{i{i=&tm{uoezn=0&jkttmg="
                                                + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=H%26%237911%3By&yzgz{y=3&{ykx=minhly_tnh&vgiqgmk=0&tevgiqgmk=&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&jut|o=&h{{i{i=&tm{uoezn=0&jkttmg="
                                                + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=H%26%237911%3By&yzgz{y=3"));

                    //1896 3_PTM_
                    data.Add(new data_url("3_PTM", "http://ccbs.vnpt.vn/ccbs/main?iutlomLork=report/pttb_bc_cuoingay/bcct_cuoingay_ajax&yZ{Tmg=" + tungay + "&yJktTmg=" + denngay + "&yjut|owr=0&ySgHI=0&y[ykx=0&yZxgtmZngoNJ=5&yHguIguIZ=1&yJYNJ=0,3,5,9,15,16&yTmgehi=1&yJT=0"));

                    //4456 4_DDTT_KH_
                    data.Add(new data_url("4_DDTT_KH", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=hguigu5hguiguekrugjejgtmq5krugjeinozokz4px~sr&vyeyinksg=CCS_TNH.&vyegmktziujk=TNH&vye{ykxoj=minhly_tnh&vyez{tmg=" + tungay + "&vyejkttmg=" + denngay + "&pxXkvuxzZvk=3&tm{uoezn=D%26%23432%3B%26%23417%3Bng+Minh+L%26%23253%3B&pxXkvuxzJgzgYu{xik=CMDVOracleDS"));

                    //home combo ptm 5_HOME_PTM_
                    data.Add(new data_url("5_HOME_PTM", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=sz|5xkvuxz5xvzenusk5nuskeinozokz4px~sr&vyz{tmg%7F=" + tungay + "&vyjkttmg%7F=" + denngay + "&vyyinksg=TNH&vygmktz=TNH&vyyinksg=TNH&vygmktztgsk=T%26%23226%3By+Ninh&pxXkvuxzZ%7Fvk=3&vy{ykx=minhly_tnh"));

                    //home combo huy 5_HOME_HUY_
                    data.Add(new data_url("5_HOME_HUY", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=sz|5xkvuxz5xvzenusk5nuskezn{khguen{%7Fejkzgor4px~sr&vyz{tmg%7F=" + tungay + "&vyjkttmg%7F=" + denngay + "&vyjut|owr=0&vyyinksg=TNH&vygmktz=TNH&vyvgiqgmkoj=0&vygmktztgsk=T%26%23226%3By+Ninh&pxXkvuxzZ%7Fvk=3&v{ykx=minhly_tnh"));


                    //đk gói mới 1_DKM_GOICUOC_
                    data_download.Add(new data_url("1_DKM_GOICUOC", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=xkvuxz5VZZHeHGUIGU5vgiqgmkexkvuxz5hiejgtmqemuo4px~sr&{ykx=minhly_tnh&yinksg=TNH&tevgiqgmk=&vgiqgmk=0&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&te{ykx=D%26%23432%3B%26%23417%3Bng+Minh+L%26%23253%3B&jut|o=&h{{i{i=&tm{uozn=0&jkttmg=" + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=%26%23272%3B%26%23259%3Bng+k%26%23253%3B+m%26%237899%3Bi&yzgz{y=1&pxXkvuxzZvk=2"));

                    //hủy gói 2_HUY_GOICUOC_
                    data_download.Add(new data_url("2_HUY_GOICUOC", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=xkvuxz5VZZHeHGUIGU5vgiqgmkexkvuxz5hiejgtmqemuo4px~sr&{ykx=minhly_tnh&yinksg=TNH&tevgiqgmk=&vgiqgmk=0&joyvgzin=0&tejoyvgzin=Kh%26%23244%3Bng+ch%26%237885%3Bn&te{ykx=D%26%23432%3B%26%23417%3Bng+Minh+L%26%23253%3B&jut|o=&h{{i{i=&tm{uozn=0&jkttmg=" + denngay + "&z{tmg=" + tungay + "&yzgz{yezk~z=H%26%237911%3By&yzgz{y=3&pxXkvuxzZvk=2"));

                    //1896 3_PTM_
                    data_download.Add(new data_url("3_PTM", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=xkvuxz5vzzhehiei{uotmg5hiznei{uotmg4px~sr&jgzgyinksg=CCS_TNH.&{ykxoj=minhly_tnh&gmktzeiujk=TNH&z{tmg=" + tungay + "&jkttmg=" + denngay + "&jut|owr=0&sghi=0&{ykx=0&zxgtmzngonj=5&hguiguiz=1&jynj=0,3,5,9,15,16&tmghi=1&jt=0&pxXkvuxzZvk=2"));

                    //4456 4_DDTT_KH_
                    data_download.Add(new data_url("4_DDTT_KH", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=hguigu5hguiguekrugjejgtmq5krugjeinozokz4px~sr&vyeyinksg=CCS_TNH.&vyegmktziujk=TNH&vye{ykxoj=minhly_tnh&vyez{tmg=" + tungay + "&vyejkttmg=" + denngay + "&pxXkvuxzZvk=2&tm{uoezn=D%26%23432%3B%26%23417%3Bng+Minh+L%26%23253%3B&pxXkvuxzJgzgYu{xik=CMDVOracleDS"));

                    //home combo ptm 5_HOME_PTM_
                    data_download.Add(new data_url("5_HOME_PTM", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=sz|5xkvuxz5xvzenusk5nuskeinozokz4px~sr&vyz{tmg=" + tungay + "&vyjkttmg=" + denngay + "&vyyinksg=TNH&vygmktz=TNH&vyyinksg=TNH&vygmktztgsk=T%26%23226%3By+Ninh&pxXkvuxzZvk=2&vy{ykx=minhly_tnh"));

                    //home combo huy 5_HOME_HUY_
                    data_download.Add(new data_url("5_HOME_HUY", "http://ccbs.vnpt.vn/ccbs/main?1iutlomLork=sz|5xkvuxz5xvzenusk5nuskezn{khguen{ejkzgor4px~sr&vyz{tmg=" + tungay + "&vyjkttmg=" + denngay + "&vyjut|owr=0&vyyinksg=TNH&vygmktz=TNH&vyvgiqgmkoj=0&vygmktztgsk=T%26%23226%3By+Ninh&pxXkvuxzZvk=2&v{ykx=minhly_tnh"));


                    dt_check = tk1.ExecuteQuery("select * from DULIEU_TNH.CHOT_DICH_SOLIEU_BANG where chot = 1 and thang = '" + txt_thang.Text + "'");
                    if (driver.Url == expectedUrl)
                    {
                        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                        // Khởi tạo DataTable
                        DataTable dt = new DataTable();

                        int i = 0;
                        foreach (data_url item in data)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                            var tabs = driver.WindowHandles;
                            driver.SwitchTo().Window(tabs[i + 1]);
                            driver.Navigate().GoToUrl(item.text);

                            if (item.value == "1_DKM_GOICUOC" || item.value == "2_HUY_GOICUOC")
                            {
                                var element = driver.FindElement(By.XPath("//*[@id='ContentTable']/div/table[1]/tbody"));
                                AddLog(DateTime.Now.ToString() + " Bắt đầu đọc html " + item.value)));
                                dt = ExtractTableData(driver, element);
                                AddLog(DateTime.Now.ToString() + " Đọc xong html " + item.value)));

                                if (dt.Rows.Count > 1)
                                {
                                    string stt = dt.Rows[1]["Column1"].ToString();
                                    if (string.IsNullOrEmpty(stt))
                                        continue;

                                    dr_check = dt_check.Select("BANG = 'BaoCaoGoiCuoc_KM_3816' and chot = 1 and thang = '" + txt_thang.Text + "'");
                                    if (dr_check.Length == 0)
                                    {
                                        if (dt.Rows.Count > 0)
                                        {
                                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = item.value));
                                            AddLog(DateTime.Now.ToString() + " Datatable " + item.value)));

                                            int l = tk1.INSERT_CCBS_GOICUOC(item.value == "1_DKM_GOICUOC" ? 0 : 1, "", "", "", "", "", "", "", "", "", "", "", "", "", "", txt_thang.Text, ""); //delete dang ky moi

                                            foreach (DataRow row in dt.Rows)
                                            {
                                                try
                                                {
                                                    if (dt.Rows.IndexOf(row) >= 1 && row[0].ToString().Trim() != "")
                                                    {
                                                        int k = tk1.INSERT_CCBS_GOICUOC(2, row[1].ToString(), row[2].ToString(), row[3].ToString()
                                                            , row[4].ToString(), row[5].ToString(), row[9].ToString(), row[10].ToString(), row[11].ToString().Trim() == "" ? "01/01/1900" : row[11].ToString()
                                                                , row[12].ToString().Trim() == "" ? "01/01/1900" : row[12].ToString(), row[15].ToString(), row[16].ToString(), row[14].ToString()
                                                                , row[13].ToString(), row[17].ToString(), txt_thang.Text, row[8].ToString());

                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    log.Error(ex.ToString());
                                                    AddLog(DateTime.Now.ToString() + " " + row[10].ToString() + " " + ex.ToString()
                                                                                                                            + " Lỗi file " + "1_DKM_GOICUOC")));
                                                }
                                                lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[4].ToString()));
                                            }
                                        }
                                    }
                                }
                            }
                            else if (item.value == "3_PTM")
                            {
                                var element = driver.FindElement(By.XPath("//*[@id='ajaxDSBC']/table"));

                                AddLog(DateTime.Now.ToString() + " Bắt đầu đọc html " + item.value)));
                                dt = ExtractTableData(driver, element);
                                AddLog(DateTime.Now.ToString() + " Đọc xong html " + item.value)));

                                dr_check = dt_check.Select("BANG = 'CCBS_AUTO' and chot = 1 and thang = '" + txt_thang.Text + "'");
                                if (dr_check.Length == 0)
                                {
                                    if (dt.Rows.Count > 2)
                                    {
                                        lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = "3_PTM_"));
                                        AddLog(DateTime.Now.ToString() + " Datatable " + "3_PTM")));

                                        int trangthai = 0;
                                        string tien_hm;
                                        string tien_dc;
                                        string tien_dathu;
                                        string tien_km = "";
                                        string mabc = "";
                                        string madv = "";

                                        int l = tk1.INSERT_CCBS_AUTO(0, txt_thang.Text, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0");

                                        foreach (DataRow row in dt.Rows)
                                        {
                                            try
                                            {
                                                if (row[1].ToString().StartsWith("TNH-LD"))
                                                    trangthai = 1;
                                                else if (row[1].ToString().StartsWith("TNH-CQ"))
                                                    trangthai = 2;
                                                else if (row[1].ToString().StartsWith("TNH-TL"))
                                                    trangthai = 3;
                                                else
                                                    continue;

                                                if (trangthai > 0)
                                                {
                                                    tien_hm = row[14].ToString().Trim();
                                                    tien_dc = row[15].ToString().Trim();
                                                    tien_dathu = row[16].ToString().Trim();
                                                    tien_km = row[17].ToString().Trim();

                                                    if (tien_hm.Length > 0)
                                                        tien_hm = tien_hm.Trim().Replace(",", "");
                                                    else
                                                        tien_hm = "0";

                                                    if (tien_dc.Length > 0)
                                                        tien_dc = tien_dc.Trim().Replace(",", "");
                                                    else
                                                        tien_dc = "0";

                                                    if (tien_dathu.Length > 0)
                                                        tien_dathu = tien_dathu.Trim().Replace(",", "");
                                                    else
                                                        tien_dathu = "0";

                                                    if (tien_km.Length > 0)
                                                        tien_km = tien_km.Trim().Replace(",", "");
                                                    else
                                                        tien_km = "0";


                                                    mabc = row[19].ToString();
                                                    madv = row[20].ToString();

                                                    int k = tk1.INSERT_CCBS_AUTO(1, txt_thang.Text, row[1].ToString(), row[2].ToString(), row[3].ToString()
                                                            , row[4].ToString(), row[5].ToString(), row[6].ToString(), row[7].ToString(), row[8].ToString()
                                                            , row[9].ToString(), row[10].ToString(), row[11].ToString(), trangthai == 1 ? row[12].ToString() : "", row[13].ToString()
                                                            , tien_hm, tien_dc, tien_dathu, tien_km, row[18].ToString(), mabc, madv, row[21].ToString()
                                                            , row[23].ToString(), row[24].ToString(), row[25].ToString(), row[26].ToString(), trangthai.ToString());
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                log.Error(ex.ToString());
                                                AddLog(DateTime.Now.ToString() + " " + row[4].ToString() + " " + ex.ToString()
                                                                                                                        + " Lỗi file " + "3_PTM")));
                                            }
                                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[4].ToString()));
                                        }
                                    }
                                }
                            }
                            else if (item.value == "4_DDTT_KH")
                            {
                                var element = driver.FindElement(By.XPath(@"/html/body/table/tbody/tr/td[2]/table"));

                                AddLog(DateTime.Now.ToString() + " Bắt đầu đọc html " + item.value)));
                                dt = ExtractTableData(driver, element);
                                AddLog(DateTime.Now.ToString() + " Đọc xong html " + item.value)));

                                dr_check = dt_check.Select("BANG = 'DDTT_CCBS_4456' and chot = 1 and thang = '" + txt_thang.Text + "'");
                                if (dr_check.Length == 0)
                                {
                                    if (dt.Rows.Count > 7)
                                    {
                                        lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = "4_DDTT_KH"));
                                        AddLog(DateTime.Now.ToString() + " Datatable " + "4_DDTT_KH")));

                                        string eload = string.Empty;
                                        int l = tk1.INSERT_DDTT_KH_4456(0, txt_thang.Text, "", "", "");

                                        foreach (DataRow row in dt.Rows)
                                        {
                                            try
                                            {
                                                int index = dt.Rows.IndexOf(row);
                                                if (index <= 6)
                                                    continue;

                                                if (row[1].ToString() == "Eload:")
                                                {
                                                    eload = row[2].ToString();
                                                    continue;
                                                }

                                                if (row[3].ToString() == "")
                                                    continue;

                                                int k = tk1.INSERT_DDTT_KH_4456(1, txt_thang.Text, eload, row[2].ToString(), row[3].ToString());
                                            }
                                            catch (Exception ex)
                                            {
                                                log.Error(ex.ToString());
                                                AddLog(DateTime.Now.ToString() + " " + row[4].ToString() + " " + ex.ToString()
                                                                                                                        + " Lỗi file " + "4_DDTT_KH")));
                                            }
                                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[2].ToString()));
                                        }
                                    }
                                }
                            }
                            else if (item.value == "5_HOME_PTM" || item.value == "5_HOME_HUY")
                            {
                                var element = driver.FindElement(By.XPath(@"/html/body/table/tbody/tr/td[2]/table"));

                                AddLog(DateTime.Now.ToString() + " Bắt đầu đọc html " + item.value)));
                                dt = ExtractTableData(driver, element);
                                AddLog(DateTime.Now.ToString() + " Đọc xong html " + item.value)));

                                if (item.value == "5_HOME_PTM")
                                {
                                    dr_check = dt_check.Select("BANG = 'GOI_HOME_COMBO' and chot = 1 and thang = '" + txt_thang.Text + "'");
                                    if (dr_check.Length == 0)
                                    {
                                        if (dt.Rows.Count > 9)
                                        {
                                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = "5_HOME_PTM"));
                                            AddLog(DateTime.Now.ToString() + " Datatable " + "5_HOME_PTM")));
                                            int l = tk1.INSERT_GOI_HOME(0, txt_thang.Text, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                                            foreach (DataRow row in dt.Rows)
                                            {
                                                try
                                                {
                                                    int index = dt.Rows.IndexOf(row);
                                                    if (index <= 8)
                                                        continue;

                                                    if (row[3].ToString() == "")
                                                        continue;

                                                    int k = tk1.INSERT_GOI_HOME(1, txt_thang.Text, row[22].ToString(), row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString()
                                                           , row[7].ToString(), row[8].ToString(), row[9].ToString(), row[11].ToString(), row[12].ToString(), row[13].ToString()
                                                           , row[14].ToString(), row[15].ToString(), row[17].ToString(), row[18].ToString(), row[19].ToString()
                                                               , row[20].ToString(), row[21].ToString(), row[23].ToString(), row[24].ToString(), row[26].ToString());

                                                }
                                                catch (Exception ex)
                                                {
                                                    log.Error(ex.ToString());
                                                    AddLog(DateTime.Now.ToString() + " " + row[4].ToString() + " " + ex.ToString()
                                                                                                                            + " Lỗi file " + "5_HOME_PTM")));
                                                }
                                                lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[3].ToString()));
                                            }
                                        }
                                    }
                                }
                                else if (item.value == "5_HOME_HUY")
                                {
                                    dr_check = dt_check.Select("BANG = 'GOI_HOME_COMBO' and chot = 1 and thang = '" + txt_thang.Text + "'");
                                    if (dr_check.Length == 0)
                                    {
                                        if (dt.Rows.Count > 9)
                                        {
                                            lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = "5_HOME_HUY"));
                                            AddLog(DateTime.Now.ToString() + " Datatable " + "5_HOME_HUY")));
                                            int l = tk1.INSERT_GOI_HOME_HUY(0, txt_thang.Text, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");

                                            foreach (DataRow row in dt.Rows)
                                            {
                                                try
                                                {
                                                    int index = dt.Rows.IndexOf(row);
                                                    if (index <= 8)
                                                        continue;

                                                    if (row[3].ToString() == "")
                                                        continue;

                                                    int k = tk1.INSERT_GOI_HOME_HUY(1, txt_thang.Text, row[3].ToString(), row[4].ToString(), row[5].ToString(), row[6].ToString(), row[8].ToString()
                                                                                   , row[10].ToString(), row[9].ToString(), row[11].ToString(), row[13].ToString(), row[12].ToString()
                                                                                   , row[14].ToString(), row[15].ToString(), row[16].ToString(), row[19].ToString(), row[17].ToString()
                                                                                   , row[20].ToString(), row[21].ToString(), row[22].ToString());

                                                }
                                                catch (Exception ex)
                                                {
                                                    log.Error(ex.ToString());
                                                    AddLog(DateTime.Now.ToString() + " " + row[5].ToString() + " " + ex.ToString()
                                                                                                                            + " Lỗi file " + "5_HOME_HUY")));
                                                }
                                                lbl_trangthai.Invoke(new Action(() => lbl_trangthai.Text = row[4].ToString()));
                                            }
                                        }
                                    }
                                }
                            }



                            i = i + 1;
                        }

                        AddLog(DateTime.Now.ToString() + " Done")));
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }

                AddLog(listBox1, "Hoàn thành tìm kiếm.");
            }
            catch (Exception ex)
            {
                AddLog(listBox1, $"Lỗi: {ex.Message}");
            }
        }
        


        private void StopSelenium()
        {
            try
            {
                AddLog(listBox1, "Đang đóng Selenium...");
                driver?.Quit();
                seleniumThread?.Abort();
                AddLog(listBox1, "Đã dừng Selenium.");
            }
            catch (Exception ex)
            {
                AddLog(listBox1, $"Lỗi khi dừng Selenium: {ex.Message}");
            }
        }
        */

        private void AddLog(ListBox logBox, string message)
        {
            // Cập nhật giao diện từ luồng Selenium
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => logBox.Items.Add(message)));
            }
            else
            {
                logBox.Items.Add(message);
            }
        }

        private async void btn_ccbs_Click(object sender, EventArgs e)
        {
            //btn_ccbs.Text = "Dừng";
            //btn_ccbs.Click -= new EventHandler(btn_ccbs_Click);
            //btn_ccbs.Click += new EventHandler(Stop_Click);

            //open_tab_control();

            //await LoginInCTVXHH("http://ccbs.vnpt.vn", "danght1.tnh", "Dhnv@123");


            //Gọi hàm đăng nhập
            try
            {
                await LoginInNewTab("http://ccbs.vnpt.vn", txtusername.Text, txtmk.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

            ////Thread thrd = new Thread(StartSelenium);
            ////thrd.IsBackground = true;
            ////thrd.Start();

        }

        private void Stop_Click(object sender, EventArgs e)
        {
            //chuyen sang chuc nang start
            btn_ccbs.Text = "Chạy";
            btn_ccbs.Click -= new EventHandler(Stop_Click);
            btn_ccbs.Click += new EventHandler(btn_ccbs_Click);

            //StopSelenium();
        }

        private void btn_capnhatmk_CCBS_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = tk1.INSERT_MATKHAU(1, DateTime.Now.ToString("yyyyMMdd"), txtusername.Text, txtmk.Text, "CCBS");

            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("Cập nhật thành công");
                txtmk.Text = dt.Rows[0]["MATKHAU"].ToString();
            }
            else
                MessageBox.Show("Cập nhật lỗi");
        }

        private void frmmain_Load(object sender, EventArgs e)
        {
            //this.TopMost = true;
            txt_thang.Text = DateTime.Now.ToString("yyyyMM");
            DataTable dt = new DataTable();
            dt = tk1.INSERT_MATKHAU(2, DateTime.Now.ToString("yyyyMMdd"), txtusername.Text, txtmk.Text, "CCBS");

            if (dt.Rows.Count > 0)
            {
                txtmk.Text = dt.Rows[0]["MATKHAU"].ToString();
            }
            else
                txtmk.Text = string.Empty;
        }

        static DataTable ExtractTableData(IWebDriver driver, IWebElement table)
        {
            DataTable dataTable = new DataTable();
            try
            {
                // Lấy tất cả các hàng trong bảng
                var rows = table.FindElements(By.XPath(".//tr"));
                foreach (var row in rows)
                {
                    // Lấy tất cả các cột (dòng đầu tiên xác định số cột)
                    var cells = row.FindElements(By.XPath(".//td"));

                    // Nếu là dòng đầu tiên (xác định cột)
                    if (dataTable.Columns.Count == 0)
                    {
                        for (int i = 0; i < cells.Count; i++)
                        {
                            dataTable.Columns.Add($"Column{i + 1}");
                        }
                    }
                    // Tạo một dòng dữ liệu mới
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < cells.Count; i++)
                    {
                        dataRow[i] = cells[i].Text.Trim(); // Lấy nội dung của ô
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }

            return dataTable;
        }
    }
}
