using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using nammun.Resources;
using HtmlAgilityPack;
using Microsoft.Phone.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Text;
using nammun.Logic;
using nammun.Utils;

namespace nammun.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
            this.OilList = new ObservableCollection<ItemViewModel>();
            this.LpgList = new ObservableCollection<ItemViewModel>();
            this.NgvList = new ObservableCollection<ItemViewModel>();

            // for PricesPage.xaml
            this.PriceList = new ObservableCollection<ItemViewModel>();
        }

        // URL to obtain data from
        public string oilSite = "http://www.eppo.go.th/retail_prices.html";
        public string lpgSite = "http://www.eppo.go.th/retail_LPG_prices.html";
        public string ngvSite = "http://www.eppo.go.th/retail_NG_prices.html";

        // Name of the save files
        private string oilXml = "oilXml.xml";
        private string lpgXml = "lpgXml.xml";
        private string ngvXml = "ngvXml.xml";

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<ItemViewModel> OilList { get; private set; }
        public ObservableCollection<ItemViewModel> LpgList { get; private set; }
        public ObservableCollection<ItemViewModel> NgvList { get; private set; }
        public ObservableCollection<ItemViewModel> PriceList { get; private set; }

        // List of retails
        public List<string> OilRetailsListEn = new List<string>();
        public List<string> OilRetailsListTh = new List<string>();
        public List<string> OilRetailsListPic = new List<string>();

        public List<string> LpgRetailsListEn = new List<string>();
        public List<string> LpgRetailsListTh = new List<string>();
        public List<string> LpgRetailsListPic = new List<string>();

        public List<string> NgvRetailsListEn = new List<string>();
        public List<string> NgvRetailsListTh = new List<string>();
        public List<string> NgvRetailsListPic = new List<string>();

        // Prices table
        Dictionary<string, List<string>> oilPricesTable = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> lpgPricesTable = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> ngvPricesTable = new Dictionary<string, List<string>>();

        private void GetOilList()
        {
            var htmlGet = new HtmlWeb();
            htmlGet.LoadCompleted += new EventHandler<HtmlDocumentLoadCompleted>(OilLoadedHandler);
            htmlGet.LoadAsync(oilSite);
        }

        private void OilLoadedHandler(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Error == null)
            {
                HtmlDocument doc = e.Document;
                if (doc != null)
                {
                    //Find text
                    HtmlNodeCollection oilPriceTable = doc.DocumentNode.SelectNodes(@"//table[@style='border-collapse: collapse']");

                    var query = from row in oilPriceTable[1].SelectNodes("tr").Cast<HtmlNode>()
                                from cell in row.SelectNodes("td").Cast<HtmlNode>()
                                select new { CellText = cell.InnerText };

                    var targetTable = oilPriceTable[1];

                    // remove useless string
                    targetTable.InnerHtml = targetTable.InnerHtml.Replace("\r", "");
                    targetTable.InnerHtml = targetTable.InnerHtml.Replace("\n", "");
                    targetTable.InnerHtml = targetTable.InnerHtml.Replace("<b>", "");
                    targetTable.InnerHtml = targetTable.InnerHtml.Replace("</b>", "");

                    var tableHeader = targetTable.SelectNodes("tr").Cast<HtmlNode>().First().SelectSingleNode("td");

                    var tempHtmlNodes = targetTable.SelectSingleNode("tr").SelectNodes("td").Cast<HtmlNode>();

                    //targetTable.SelectSingleNode("tr").Remove();
                    //targetTable.SelectSingleNode("tr").InsertBefore(tableHeader, oilPriceTable[1].SelectSingleNode("tr").FirstChild);

                    //int rowNum = targetTable.SelectNodes("tr").Cast<HtmlNode>().Count();
                    //int colNum = targetTable.SelectSingleNode("tr").SelectNodes("td").Cast<HtmlNode>().Count();

                    XDocument xdoc = new XDocument(new XElement("oil"));

                    bool flag = true;
                    bool flag2 = true;
                    bool flag3 = true;
                    int rownum = 0;

                    foreach (var row in targetTable.SelectNodes("tr").Cast<HtmlNode>())
                    {
                        bool flag4 = true;

                        if (flag)
                        {
                            if (flag2)
                            {
                                foreach (var col in row.SelectNodes("td").Cast<HtmlNode>())
                                {
                                    //var xmlNode = new XElement("company");

                                    if (flag3)
                                    {
                                        flag3 = false;
                                    }
                                    else
                                    {
                                        var img = col.ChildNodes[0].FirstChild.Attributes["src"].Value;
                                        OilRetailsListPic.Add(img);
                                    }
                                }
                                flag2 = false;
                                rownum++;
                                continue;
                            }

                            //row.ChildNodes[0].Remove();

                            foreach (var col in row.SelectNodes("td").Cast<HtmlNode>())
                            {
                                string[] s = Regex.Split(col.ChildNodes[0].InnerHtml, "<br>");

                                OilRetailsListEn.Add(s[s.Length - 1]);
                                //var companyName = new XElement("nameEn", s[s.Length - 1]);

                                s = s.Where(val => val != s[s.Length - 1]).ToArray();

                                OilRetailsListTh.Add(string.Join("", s));
                                //var companyNameTh = new XElement("nameTh", string.Join("", s));

                                //xmlNode.Add(companyName, companyNameTh);
                                //xdoc.Root.Add(xmlNode);
                            }
                            flag = false;
                            continue;
                        }
                        else
                        {
                            // insert data to xml
                            string oilName = string.Empty;
                            List<string> list = new List<string>();
                            foreach (var col in row.SelectNodes("td").Cast<HtmlNode>())
                            {
                                if (flag4)
                                {
                                    // column header
                                    oilName = rownum.ToString();

                                    flag4 = false;
                                    continue;
                                }
                                else
                                {
                                    // data
                                    list.Add(col.InnerHtml);
                                }
                                Console.WriteLine(col);
                            }

                            // got name and prices!
                            oilPricesTable.Add(oilName, list);

                            rownum++;

                            Console.WriteLine(row);
                        }

                        // got table!
                        Console.WriteLine(xdoc);
                        Console.WriteLine(row);
                    }

                    try
                    {
                        string xml = OilXmlConverter.ConvertOilTableToXml(oilPricesTable, this.OilList);
                        FileManager.CreateSaveStorage(oilXml, XDocument.Parse(xml));
                    }
                    catch (Exception)
                    {
                        MessageBoxResult res = MessageBox.Show("Something doesn't right on server site. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            WebBrowserTask webBrowserTask = new WebBrowserTask();
                            webBrowserTask.Uri = new Uri(oilSite, UriKind.Absolute);
                            webBrowserTask.Show();
                        }
                    }
                }
                else
                {
                    MessageBoxResult res = MessageBox.Show("Something doesn't right on server site. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        WebBrowserTask webBrowserTask = new WebBrowserTask();
                        webBrowserTask.Uri = new Uri(oilSite, UriKind.Absolute);
                        webBrowserTask.Show();
                    }
                }
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("Server cannot be reached, likely due to bad network connection. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(oilSite, UriKind.Absolute);
                    webBrowserTask.Show();
                }
            }
        }

        private void GetLpgList()
        {
            var htmlGet = new HtmlWeb();
            htmlGet.LoadCompleted += new EventHandler<HtmlDocumentLoadCompleted>(LpgLoadedHandler);
            htmlGet.LoadAsync(lpgSite);
        }

        private void LpgLoadedHandler(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Error == null)
            {
                HtmlDocument doc = e.Document;
                if (doc != null)
                {
                    //Find text
                    HtmlNodeCollection lpgPriceTable = doc.DocumentNode.SelectNodes(@"//table[@style='border-collapse: collapse']");

                    var query = from row in lpgPriceTable[0].SelectNodes("tr").Cast<HtmlNode>()
                                from cell in row.SelectNodes("td").Cast<HtmlNode>()
                                select new { CellText = cell.InnerText };

                    int rowNum = lpgPriceTable[0].SelectNodes("tr").Cast<HtmlNode>().Count();
                    int colNum = lpgPriceTable[0].SelectSingleNode("tr").SelectNodes("td").Cast<HtmlNode>().Count();

                    string[,] table = new string[rowNum, colNum];

                    int rownum = 0;
                    foreach (var row in lpgPriceTable[0].SelectNodes("tr").Cast<HtmlNode>())
                    {
                        if (rownum == 0)
                        {
                            rownum++;
                            continue;
                        }

                        string lpgName = string.Empty;
                        List<string> list = new List<string>();
                        int colnum = 0;
                        foreach (var col in row.SelectNodes("td"))
                        {
                            // remove useless string
                            col.InnerHtml = col.InnerHtml.Replace("\r", "");
                            col.InnerHtml = col.InnerHtml.Replace("\n", "");
                            col.InnerHtml = col.InnerHtml.Replace("<b>", "");
                            col.InnerHtml = col.InnerHtml.Replace("</b>", "");

                            // replace non-value space with -
                            col.InnerHtml = col.InnerHtml.Replace("&nbsp;", "-");

                            if (colnum == 0)
                            {
                                lpgName = rownum.ToString();
                                colnum++;
                                continue;
                            }

                            list.Add(col.InnerHtml);

                            colnum++;
                        }

                        // got name and prices!
                        lpgPricesTable.Add(lpgName, list);
                        rownum++;

                        // got table!
                        Console.WriteLine(row);
                    }

                    try
                    {
                        string xml = LpgXmlConverter.ConvertLpgTableToXml(lpgPricesTable, this.LpgList);
                        FileManager.CreateSaveStorage(lpgXml, XDocument.Parse(xml));
                    }
                    catch (Exception)
                    {
                        MessageBoxResult res = MessageBox.Show("Something doesn't right on server site. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            WebBrowserTask webBrowserTask = new WebBrowserTask();
                            webBrowserTask.Uri = new Uri(lpgSite, UriKind.Absolute);
                            webBrowserTask.Show();
                        }
                    }
                }
                else
                {
                    MessageBoxResult res = MessageBox.Show("Something doesn't right on server site. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        WebBrowserTask webBrowserTask = new WebBrowserTask();
                        webBrowserTask.Uri = new Uri(lpgSite, UriKind.Absolute);
                        webBrowserTask.Show();
                    }
                }
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("Server cannot be reached, likely due to bad network connection. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(lpgSite, UriKind.Absolute);
                    webBrowserTask.Show();
                }
            }
        }

        private void GetNgvList()
        {
            var htmlGet = new HtmlWeb();
            htmlGet.LoadCompleted += new EventHandler<HtmlDocumentLoadCompleted>(NgvLoadedHandler);
            htmlGet.LoadAsync(ngvSite);
        }

        private void NgvLoadedHandler(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Error == null)
            {
                HtmlDocument doc = e.Document;
                if (doc != null)
                {
                    //Find text
                    HtmlNodeCollection ngvPriceTable = doc.DocumentNode.SelectNodes(@"//table[@style='border-collapse: collapse']");

                    var query = from row in ngvPriceTable[1].SelectNodes("tr").Cast<HtmlNode>()
                                from cell in row.SelectNodes("td").Cast<HtmlNode>()
                                select new { CellText = cell.InnerText };

                    int rowNum = ngvPriceTable[1].SelectNodes("tr").Cast<HtmlNode>().Count();
                    int colNum = ngvPriceTable[1].SelectSingleNode("tr").SelectNodes("td").Cast<HtmlNode>().Count();

                    string[,] table = new string[rowNum, colNum];

                    //foreach (var row in ngvPriceTable[1].SelectNodes("tr").Cast<HtmlNode>())
                    //{
                    //    // got table!
                    //    Console.WriteLine(row);
                    //}

                    var lastRow = ngvPriceTable[1].SelectNodes("tr").Last();
                    string ngvName = "PTT";
                    List<string> list = new List<string>();

                    foreach (var col in lastRow.SelectNodes("td"))
                    {
                        // remove useless string
                        col.InnerHtml = col.InnerHtml.Replace("\r", "");
                        col.InnerHtml = col.InnerHtml.Replace("\n", "");
                        col.InnerHtml = col.InnerHtml.Replace("<b>", "");
                        col.InnerHtml = col.InnerHtml.Replace("</b>", "");

                        list.Add(col.InnerHtml);
                    }

                    // got name and prices!
                    ngvPricesTable.Add(ngvName, list);

                    try
                    {
                        string xml = NgvXmlConverter.ConvertNgvTableToXml(ngvPricesTable, this.NgvList);
                        FileManager.CreateSaveStorage(ngvXml, XDocument.Parse(xml));
                    }
                    catch (Exception)
                    {
                        MessageBoxResult res = MessageBox.Show("Something doesn't right on server site. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            WebBrowserTask webBrowserTask = new WebBrowserTask();
                            webBrowserTask.Uri = new Uri(ngvSite, UriKind.Absolute);
                            webBrowserTask.Show();
                        }
                    }
                }
                else
                {
                    MessageBoxResult res = MessageBox.Show("Something doesn't right on server site. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        WebBrowserTask webBrowserTask = new WebBrowserTask();
                        webBrowserTask.Uri = new Uri(ngvSite, UriKind.Absolute);
                        webBrowserTask.Show();
                    }
                }
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("Server cannot be reached, likely due to bad network connection. I can take you to the website if you wish.", "I'm sorry...", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(ngvSite, UriKind.Absolute);
                    webBrowserTask.Show();
                }
            }
        }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Add Oil list data
            this.OilList.Add(new ItemViewModel() { LineOne = "PTT", LineTwo = "ปตท", LineThree = @"\images\Oil\pt-BN-PTT.gif" });
            this.OilList.Add(new ItemViewModel() { LineOne = "BCP", LineTwo = "บางจาก", LineThree = @"\images\Oil\pt-BN-BCP.jpg" });
            this.OilList.Add(new ItemViewModel() { LineOne = "SHELL", LineTwo = "เชลล์", LineThree = @"\images\Oil\pt-BN-Shell3.gif" });
            this.OilList.Add(new ItemViewModel() { LineOne = "ESSO", LineTwo = "เอสโซ่", LineThree = @"\images\Oil\pt-BN-ESSO.jpg" });
            this.OilList.Add(new ItemViewModel() { LineOne = "Chevron", LineTwo = "เชฟรอน", LineThree = @"\images\Oil\pt-BN-chevron.jpg" });
            this.OilList.Add(new ItemViewModel() { LineOne = "IRPC", LineTwo = "ไออาร์พีซี", LineThree = @"\images\Oil\pt-bn-irpc.jpg" });
            this.OilList.Add(new ItemViewModel() { LineOne = "PT", LineTwo = "ภาคใต้เชื้อเพลิง", LineThree = @"\images\Oil\pt-BN-PT.jpg" });
            this.OilList.Add(new ItemViewModel() { LineOne = "SUSCO", LineTwo = "ซัสโก้", LineThree = @"\images\Oil\pt-BN-susco.jpg" });
            this.OilList.Add(new ItemViewModel() { LineOne = "PURE", LineTwo = "ระยองเพียว", LineThree = @"\images\Oil\pt-BN-pure.jpg" });
            this.OilList.Add(new ItemViewModel() { LineOne = "PETRONAS", LineTwo = "ปิโตรนาส", LineThree = @"\images\Oil\pt-BN-Petronas.gif" });

            // Add LPG list data
            this.LpgList.Add(new ItemViewModel() { LineOne = "PTT", LineTwo = "ปตท", LineThree = @"\images\LPG\pt-BN-PTT.jpg" });
            this.LpgList.Add(new ItemViewModel() { LineOne = "UNIQUE GAS", LineTwo = "ยูนิคแก๊ส", LineThree = @"\images\LPG\pt-BN-unique.jpg" });
            this.LpgList.Add(new ItemViewModel() { LineOne = "SIAM GAS", LineTwo = "สยามแก๊ส", LineThree = @"\images\LPG\pt-BN-siamgas.jpg" });
            this.LpgList.Add(new ItemViewModel() { LineOne = "PICNIC", LineTwo = "ปิคนิคแก๊ส", LineThree = @"\images\LPG\pt-BN-picnic.jpg" });
            this.LpgList.Add(new ItemViewModel() { LineOne = "WORLD GAS", LineTwo = "เวิลด์แก๊ส", LineThree = @"\images\LPG\pt-BN-worldgas.png" });
            this.LpgList.Add(new ItemViewModel() { LineOne = "V 2 GAS", LineTwo = "วี ทู แก๊ส", LineThree = @"\images\LPG\pt-BN-V2.jpg" });

            // Add NGV list data
            this.NgvList.Add(new ItemViewModel() { LineOne = "PTT", LineTwo = "ปตท", LineThree = @"\images\NGV\ngv_logo_90.gif" });

            // Sample data; replace with real data
            this.Items.Add(new ItemViewModel() { LineOne = "runtime one", LineTwo = "Maecenas praesent accumsan bibendum", LineThree = "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu" });
            this.Items.Add(new ItemViewModel() { LineOne = "runtime two", LineTwo = "Dictumst eleifend facilisi faucibus", LineThree = "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus" });

            GetOilList();
            GetLpgList();
            GetNgvList();

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}