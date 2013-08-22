using nammun.Logic;
using nammun.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace nammun.ViewModels
{
    public class PriceViewModel : INotifyPropertyChanged
    {
        // Name of the save files
        private string oilXml = "oilXml.xml";
        private string lpgXml = "lpgXml.xml";
        private string ngvXml = "ngvXml.xml";

        // Control Properties
        public bool IsDataLoaded
        {
            get;
            private set;
        }

        // Binding Properties
        public string ProviderName { get; set; }
        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public string Title { get; set; }

        public void LoadData(string type, string name)
        {
            if (type == "oil")
            {
                LoadOilData(name);
            }
            else if (type == "lpg")
            {
                LoadLpgData(name);
            }
            else if (type == "ngv")
            {
                LoadNgvData(name);
            }

            this.IsDataLoaded = true;
        }

        private void LoadOilData(string name)
        {
            XDocument xdoc = FileManager.LoadSaveStorage(oilXml);
            Items = new ObservableCollection<ItemViewModel>(
                        (from node in xdoc.Descendants()
                        where node.Attribute("price") != null && node.Attribute("provider").Value == name
                        select new ItemViewModel() 
                        {
                            LineOne = OilXmlConverter.OilTypeDict[node.Ancestors("type").First().Attribute("name").Value],
                            LineTwo = node.Attribute("price").Value
                        })
                    );
            ProviderName = name;
            Title = "Effective Date - " +
                            (from node in xdoc.Descendants()
                             where node.Attribute("date") != null && node.Attribute("provider").Value == name
                             select node.Attribute("date").Value).First();
        }

        private void LoadLpgData(string name)
        {
            XDocument xdoc = FileManager.LoadSaveStorage(lpgXml);
            Items = new ObservableCollection<ItemViewModel>(
                        (from node in xdoc.Descendants()
                         where node.Attribute("price") != null && node.Attribute("provider").Value == name
                         select new ItemViewModel()
                         {
                             LineOne = LpgXmlConverter.LpgTypeDict[node.Ancestors("type").First().Attribute("name").Value],
                             LineTwo = node.Attribute("price").Value
                         })
                    );
            ProviderName = name;
            Title = "Effective Date - " +
                            (from node in xdoc.Descendants()
                             where node.Attribute("date") != null && node.Attribute("provider").Value == name
                             select node.Attribute("date").Value).First();
        }

        private void LoadNgvData(string name)
        {
            XDocument xdoc = FileManager.LoadSaveStorage(ngvXml);
            Items = new ObservableCollection<ItemViewModel>(
                        (from node in xdoc.Descendants()
                         where node.Attribute("price") != null && node.Attribute("provider").Value == name
                         select new ItemViewModel()
                         {
                             LineOne = node.Ancestors("type").First().Attribute("name").Value,
                             LineTwo = node.Attribute("price").Value,
                             LineThree = node.Attribute("changes").Value
                         })
                    );
            ProviderName = name;
            Title = "Effective Date - " +
                            (from node in xdoc.Descendants()
                             where node.Attribute("date") != null && node.Attribute("provider").Value == name
                             select node.Attribute("date").Value).First();
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
