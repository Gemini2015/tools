using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Windows;

namespace FilePost.Util
{
    public class XMLUtil
    {
        public static string ConfigFile = "config.xml";
        public static string ConfigPath = "data";

        private static XmlDocument GetDocument()
        {
            string fullname = Path.Combine(ConfigPath, ConfigFile);
            if(!File.Exists(fullname))
            {
                Directory.CreateDirectory(ConfigPath);
                File.Create(fullname);
            }
            XmlDocument doc = new XmlDocument();
            try
            {
            	doc.Load(fullname);
            }
            catch (System.Exception ex)
            {
                Logger.Instance.Print(ex.ToString());
                doc = null;
            }
            return doc;
        }

        private static void SaveDocument(XmlDocument doc)
        {
            string fullname = Path.Combine(ConfigPath, ConfigFile);
            try
            {
                doc.Save(fullname);
            }
            catch(XmlException ex)
            {
                Logger.Instance.Print(ex.ToString());
            }
        }

        public static bool LoadConfig(IList<PreferData> preferList )
        {
            //preferList = new List<PreferData>();
            XmlDocument doc = GetDocument();
            if (doc == null || !doc.HasChildNodes)
                return false;

            XmlNode root = doc.SelectSingleNode("config");
            System.Collections.IEnumerator it = root.ChildNodes.GetEnumerator();
            while (it.MoveNext())
            {
                XmlNode node = it.Current as XmlNode;
                PreferData preferdata = new PreferData(node.Attributes["name"].Value);
                preferdata.Name = node.Attributes["name"].Value;
                if (node.HasChildNodes)
                {
                    System.Collections.IEnumerator folderit = node.ChildNodes.GetEnumerator();
                    while(folderit.MoveNext())
                    {
                        XmlNode elem = folderit.Current as XmlNode;
                        PreferFolderData data = new PreferFolderData();
                        data.Name = elem.Attributes["name"].Value;
                        data.Path = elem.Attributes["path"].Value;

                        preferdata.mFolderList.Add(data);
                    }
                }
                
                if(preferdata.Name != "")
                {
                    preferList.Add(preferdata);
                }
            }

            return true;
        }

        public static bool SaveConfig(IList<PreferData> prefer )
        {
            if (prefer.Count == 0)
                return false;

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("config");
            doc.AppendChild(root);
            IEnumerator<PreferData> it = prefer.GetEnumerator();
            while(it.MoveNext())
            {
                XmlElement preferDataNode = doc.CreateElement("PreferData");
                preferDataNode.SetAttribute("name", it.Current.Name);
                root.AppendChild(preferDataNode);

                IEnumerator<PreferFolderData> folderIt = it.Current.mFolderList.GetEnumerator();
                while(folderIt.MoveNext())
                {
                    XmlElement preferFolderNode = doc.CreateElement("PreferFolderData");
                    preferDataNode.AppendChild(preferFolderNode);

                    preferFolderNode.SetAttribute("name", folderIt.Current.Name);
                    preferFolderNode.SetAttribute("path", folderIt.Current.Path);
                }
            }

            SaveDocument(doc);
            return true;
        }
    }

    public struct PreferFolderData
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public struct PreferData
    {
        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        public IList<PreferFolderData> mFolderList;

        public PreferData(string name)
        {
            mName = name;
            mFolderList = new List<PreferFolderData>();
        }
    }
}
