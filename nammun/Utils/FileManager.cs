using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace nammun.Utils
{
    public static class FileManager
    {
        // Create XML file
        public static void CreateSaveStorage(string filename, XDocument doc)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream =
                    new IsolatedStorageFileStream(filename, FileMode.Create, isoStore))
                {
                    doc.Save(isoStream);
                }
            }
        }

        // Load XML
        public static XDocument LoadSaveStorage(string filename)
        {
            XDocument doc = null;
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(filename))
                {
                    using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(filename, FileMode.Open, isoStore))
                    {
                        doc = XDocument.Load(isoStream);
                        return doc;
                    }
                }
                else
                {
                    return doc;
                }
            }
        }
    }
}
