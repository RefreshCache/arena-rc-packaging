using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

namespace RefreshCache.Packager
{
    /// <summary>
    /// The File class contains all information required to export a
    /// single file for use with Arena.
    /// </summary>
    public class File
    {
        #region Properties

        /// <summary>
        /// The path in Arena that this file will be installed to. This
        /// path is relative to the Arena web folder (where the web.config
        /// file is located).
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// When loading a full package this parameter is filled with the
        /// file contents and can be used to create the file on the target
        /// machine.
        /// </summary>
        public Byte[] Contents { get { return _Contents; } }
        private Byte[] _Contents;

        /// <summary>
        /// The path which identifies the source data for this File
        /// object. While this path can be absolute it should be
        /// relative to the directory the package will be built
        /// against.
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// The Package object that will own this File. This should not
        /// be set directly, it will be set automatically when a File is
        /// added to a Packages Files member.
        /// </summary>
        public Package Package { get; set; }

        /// <summary>
        /// Determine if the file at the Source path exists or not.
        /// </summary>
        internal Boolean Exists
        {
            get
            {
                FileInfo fi = null;

                try
                {
                    fi = new FileInfo((Source[1] == ':' ? "" : Package.BasePath) + Source);
                }
                catch { }

                return (fi != null ? fi.Exists : false);
            }
        }

        #endregion

        /// <summary>
        /// Create a new empty File object which can be added to a
        /// FileCollection object.
        /// </summary>
        public File()
        {
            Path = "";
            Source = "";
        }

        /// <summary>
        /// Build a new File object that is ready to be exported. This is an
        /// internal use only method and should not be used by anything outside
        /// the Packager framework.
        /// </summary>
        /// <param name="path">The path in Arena for the file.</param>
        /// <param name="source">The path in the local filesystem for the file.</param>
        /// <param name="package">The Package object this File is associated with.</param>
        internal File(String path, String source, Package package)
        {
            Path = path;
            Source = source;
            Package = package;
        }

        /// <summary>
        /// Creates a new File object from a previously saved document.
        /// </summary>
        /// <param name="node">The node which represents the saved File object.</param>
        public File(XmlNode node)
        {
            Path = node.Attributes["path"].Value;
            if (node.Attributes["_source"] != null)
                Source = node.Attributes["_source"].Value;
            else
                Source = "";

            if (!String.IsNullOrEmpty(node.InnerText))
            {
                _Contents = Convert.FromBase64String(node.InnerText);
            }
        }

        /// <summary>
        /// Create a new XML node that will include all the information
        /// about this File object. This only includes information needed
        /// to reload this object later, it does not produce a final
        /// output that can be used to import into Arena.
        /// </summary>
        /// <param name="doc">The XmlDocument that the new node will be a part of.</param>
        /// <param name="isExport">Identifies if this Save operation is for exporting to Arena.</param>
        /// <returns>New XmlNode object which represents this File object.</returns>
        public XmlNode Save(XmlDocument doc, Boolean isExport)
        {
            XmlNode node = doc.CreateElement("File");
            XmlAttribute attrib;


            attrib = doc.CreateAttribute("path");
            attrib.InnerText = Path;
            node.Attributes.Append(attrib);

            if (isExport == false)
            {
                attrib = doc.CreateAttribute("_source");
                attrib.InnerText = Source;
                node.Attributes.Append(attrib);
            }

            if (isExport)
            {
                FileInfo fi = null;

                try
                {
                    fi = new FileInfo((Source[1] == ':' ? "" : Package.BasePath) + Source);
                }
                catch { }

                if (fi == null)
                {
                    Package.BuildMessages.Add(new BuildMessage(BuildMessageType.Error,
                        String.Format("Arena file at path {0} has no local file set.", Path)));
                }
                else if (fi.Exists == false)
                {
                    Package.BuildMessages.Add(new BuildMessage(BuildMessageType.Error,
                        String.Format("The local file {0} does not exist.", Source)));
                }
                else
                {
                    byte[] buffer = null;

                    using (FileStream stream = fi.OpenRead())
                    {
                        buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                        stream.Close();
                    }

                    node.InnerText = Convert.ToBase64String(buffer);
                }
            }

            return node;
        }
    }

    /// <summary>
    /// Provide an interface to hold a collection of all files
    /// in the package. Should only be instantiated by the
    /// Package class. Keeps all File objects in sync with
    /// their owning Package.
    /// </summary>
    public class FileCollection : Collection<File>
    {
        /// <summary>
        /// The owning Package object for all the Files associated
        /// with this collection.
        /// </summary>
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
        private Package _Owner;

        /// <summary>
        /// Create a new FileCollection object and set it's Owner
        /// to the indicated Package object.
        /// </summary>
        /// <param name="owner">The Package object that will own all these Files.</param>
        internal FileCollection(Package owner)
            : base()
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Insert a new File object into the collection and set
        /// it's Package to the Owner.
        /// </summary>
        /// <param name="index">The Index position to insert the object at.</param>
        /// <param name="item">The new object to insert.</param>
        protected override void InsertItem(int index, File item)
        {
            item.Package = Owner;
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Change the item at a given index. Remove the old Package
        /// reference and add it to the new File object.
        /// </summary>
        /// <param name="index">Index of the object to replace.</param>
        /// <param name="item">The File object to replace the old object with.</param>
        protected override void SetItem(int index, File item)
        {
            this[index].Package = null;
            item.Package = Owner;
            base.SetItem(index, item);
        }

        /// <summary>
        /// Remove a File object from this collection and also
        /// remove it's reference to the Package.
        /// </summary>
        /// <param name="index">Index of the object to be removed.</param>
        protected override void RemoveItem(int index)
        {
            this[index].Package = null;
            base.RemoveItem(index);
        }
    }
}
