using nammun.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nammun.Logic
{
    public static class NgvXmlConverter
    {
        /// <summary>
        /// Convert from price table to xml string.
        /// </summary>
        /// <param name="pricesTable"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public static string ConvertNgvTableToXml(Dictionary<string, List<string>> pricesTable, ObservableCollection<ItemViewModel> metaData)
        {
            StringBuilder xml = new StringBuilder();
            string[] fuelOpenAndEndTags = GetRootItems("fuel");
            xml.Append(fuelOpenAndEndTags[0]);

            foreach (string key in pricesTable.Keys)
            {
                string[] typeOpenAndEndTags = GetTypeItems(key);
                xml.Append(typeOpenAndEndTags[0]);

                xml.Append(GetDataItem(metaData[0].LineOne, pricesTable[key][0], pricesTable[key][1], pricesTable[key][2], metaData[0].LineThree));

                xml.Append(typeOpenAndEndTags[1]);
            }

            xml.Append(fuelOpenAndEndTags[1]);

            return xml.ToString();
        }

        private static string[] GetRootItems(string rootName)
        {
            string[] tuple = new string[]
            {
                string.Format("<{0}>", rootName),
                string.Format("</{0}>", rootName)
            };
            return tuple;
        }

        private static string[] GetTypeItems(string type)
        {
            string[] tuple = new string[]
            {
                string.Format("<type name=\"{0}\">", type),
                string.Format("</type>")
            };
            return tuple;
        }

        private static string GetDataItem(string provider, string date, string price, string changes, string image)
        {
            string item = string.Format("<data provider=\"{0}\" price=\"{1}\" date=\"{2}\" changes=\"{3}\" image=\"{4}\"/>", provider, price, date, changes, image);
            return item;
        }
    }
}
