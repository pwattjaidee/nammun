using nammun.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nammun.Logic
{
    public static class LpgXmlConverter
    {
        /// <summary>
        /// Public dictionary for lpg type
        /// </summary>
        public static Dictionary<string, string> LpgTypeDict = new Dictionary<string, string>()
        {
            { "1", "ถังขนาด 4 กิโลกรัม / Kg" },
            { "2", "ถังขนาด 7 กิโลกรัม / Kg" },
            { "3", "ถังขนาด 11.5 กิโลกรัม / Kg" },
            { "4", "ถังขนาด 13.5 กิโลกรัม / Kg" },
            { "5", "ถังขนาด 15 กิโลกรัม / Kg" },
            { "6", "ถังขนาด 48 กิโลกรัม / Kg" },
        };

        /// <summary>
        /// Convert from price table to xml string.
        /// </summary>
        /// <param name="pricesTable"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public static string ConvertLpgTableToXml(Dictionary<string, List<string>> pricesTable, ObservableCollection<ItemViewModel> metaData)
        {
            StringBuilder xml = new StringBuilder();
            string[] fuelOpenAndEndTags = GetRootItems("fuel");
            xml.Append(fuelOpenAndEndTags[0]);

            foreach (string key in pricesTable.Keys)
            {
                string[] typeOpenAndEndTags = GetTypeItems(key);
                xml.Append(typeOpenAndEndTags[0]);

                if (key != "7")
                {
                    for (int i = 0; i < pricesTable[key].Count; i++)
                    {
                        xml.Append(GetDataItem(metaData[i].LineOne, pricesTable[key][i], metaData[i].LineThree));
                    }
                }
                else
                {
                    for (int i = 0; i < pricesTable[key].Count; i++)
                    {
                        xml.Append(GetDataItem2(metaData[i].LineOne, pricesTable[key][i]));
                    }
                }

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

        private static string GetDataItem(string provider, string price, string image)
        {
            string item = string.Format("<data provider=\"{0}\" price=\"{1}\" image=\"{2}\"/>", provider, price, image);
            return item;
        }

        private static string GetDataItem2(string provider, string date)
        {
            string item = string.Format("<data provider=\"{0}\" date=\"{1}\"/>", provider, date);
            return item;
        }
    }
}
