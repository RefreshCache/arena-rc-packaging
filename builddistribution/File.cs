using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace Arena.Custom.RC.Packager
{
    public class File
    {
        public String Path { get; set; }
        public String Source { get; set; }
        public Package Package { get; set; }

        public File()
        {
            Path = "";
            Source = "";
        }

        public File(XmlNode node)
        {
            Path = node.Attributes["path"].Value;
            if (node.Attributes["_source"] != null)
                Source = node.Attributes["_source"].Value;
            else
                Source = "";
        }

        public XmlNode Save(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("File");
            XmlAttribute attrib;


            attrib = doc.CreateAttribute("path");
            attrib.InnerText = Path;
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("_source");
            attrib.InnerText = Source;
            node.Attributes.Append(attrib);

            return node;
        }
    }

    public class FileCollection : Collection<File>
    {
        private Package _Owner;
        public Package Owner
        {
            get { return _Owner; }
            set
            {
                _Owner = value;
                foreach (File file in this)
                {
                    file.Package = value;
                }
            }
        }

        internal FileCollection(Package owner)
            : base()
        {
            this.Owner = owner;
        }

        protected override void InsertItem(int index, File item)
        {
            item.Package = Owner;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, File item)
        {
            this[index].Package = null;
            item.Package = Owner;
            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            this[index].Package = null;
            base.RemoveItem(index);
        }
    }
}
