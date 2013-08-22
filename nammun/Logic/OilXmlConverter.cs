using nammun.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nammun.Logic
{
    public static class OilXmlConverter
    {
        /// <summary>
        /// Public dictionary for oil type.
        /// </summary>
        public static Dictionary<string, string> OilTypeDict = new Dictionary<string, string>()
        {
            { "1", "แก๊สโซฮอล ออกเทน 95 (Gasohol 95-E10)" },
            { "2", "แก๊สโซฮอล ออกเทน 95 (Gasohol 95-E20)" },
            { "3", "แก๊สโซฮอล ออกเทน 95 (Gasohol 95-E85)" },
            { "4", "แก๊สโซฮอล ออกเทน 91 (Gasohol 91-E10)" },
            { "5", "เบนซิน ออกเทน 95 (ULG 95 RON)" },
            { "6", "ดีเซลหมุนเร็ว (HSD, 0.035%S)" },
        };

        /// <summary>
        /// Convert from price table to xml string.
        /// </summary>
        /// <param name="pricesTable"></param>
        /// <param name="metaData"></param>
        /// <returns></returns>
        public static string ConvertOilTableToXml(Dictionary<string, List<string>> pricesTable, ObservableCollection<ItemViewModel> metaData)
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
