﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;

namespace RefreshCache.Packager
{
    /// <summary>
    /// The Package class is a self-contained object that provides all
    /// the information needed to create and build a template that can
    /// be used to re-package new versions without having to
    /// re-construct the page.xml file each time. The Package class also
    /// provides the information needed to Export the page.xml that can
    /// then be loaded into Arena.
    /// </summary>
    public class Package
    {
        #region Properties

        /// <summary>
        /// Collection of File objects that will be associated with this
        /// Package and created in Arena during import.
        /// </summary>
        public FileCollection Files { get { return _Files; } }
        private FileCollection _Files;

        /// <summary>
        /// A collection of Modules that are a part of this Package. This is
        /// not the ModuleInstances, those are members of PageInstances.
        /// </summary>
        public ModuleCollection Modules { get { return _Modules; } }
        private ModuleCollection _Modules;

        /// <summary>
        /// The collection of root-level PageInstances in this Package. A Package
        /// should only have a single root-level page. However, loading an older
        /// Package could result in multiple root pages.
        /// </summary>
        public PageInstanceCollection Pages { get { return _Pages; } }
        private PageInstanceCollection _Pages;

        /// <summary>
        /// The Readme information associated with a Package. The Readme
        /// contents are displayed to the user during Import as "Important
        /// Information" that should be read before proceeding with the
        /// import operation.
        /// </summary>
        public String Readme { get; set; }

        /// <summary>
        /// Contains information about the package such as name, version,
        /// dependencies, etc.
        /// </summary>
        public PackageInfo Info { get { return _Info; } }
        private PackageInfo _Info;

        /// <summary>
        /// Retrieves the byte array of raw assembly data that comprises the
        /// Migration assembly that allows the package to install database
        /// changes. This property cannot be set, it is automatically set
        /// when a package is loaded that has Migration data.
        /// </summary>
        public Byte[] Migration { get { return _Migration; } }
        private Byte[] _Migration;

        /// <summary>
        /// The source of the migration DLL relative to the build path.
        /// When building a package the contents of this file will be loaded
        /// and stored along with the XML document for use when installing
        /// a package. This should point to a DLL with a single class which
        /// is a subclass of Migration.
        /// </summary>
        public String MigrationSource { get; set; }

        /// <summary>
        /// This list is reset each time the Export method is called and
        /// will contain a textual list of messages generated during the
        /// export. If no messages exist this list will be empty.
        /// </summary>
        internal BuildMessageCollection BuildMessages { get { return _BuildMessages; } }
        private BuildMessageCollection _BuildMessages;

        /// <summary>
        /// The base path to use for relative file names while building the package.
        /// </summary>
        internal String BasePath { get { return _BasePath; } }
        private String _BasePath;

        /// <summary>
        /// Retrieve the XmlPackage after calling the Build method.
        /// </summary>
        public XmlDocument XmlPackage { get { return _XmlPackage; } }
        private XmlDocument _XmlPackage;

        #endregion

        /// <summary>
        /// Creates a new, empty, Package object that can then be loaded with
        /// data by the user.
        /// </summary>
        public Package()
        {
            _Info = new PackageInfo();
            _Files = new FileCollection(this);
            _Modules = new ModuleCollection(this);
            _Pages = new PageInstanceCollection(this);
            Readme = "";
        }

        /// <summary>
        /// Creates a new Package object by loading and parsing the data
        /// contained in the XmlDocument. The Package object can then be
        /// further edited or exported.
        /// </summary>
        /// <param name="doc">The XmlDocument that was generated by a previous call to Save.</param>
        public Package(XmlDocument doc)
            : this()
        {
            XmlNode node;


            //
            // Load the readme.
            //
            if (doc.SelectSingleNode("//Readme") != null)
            {
                Readme = doc.SelectSingleNode("//Readme").InnerText;
            }

            //
            // Load the modules from the document.
            //
            node = doc.SelectSingleNode("//ArenaPackage/Files");
            if (node != null)
            {
                foreach (XmlNode f in node.ChildNodes)
                {
                    Files.Add(new File(f));
                }
            }

            //
            // Load the modules from the document.
            //
            node = doc.SelectSingleNode("//ArenaPackage/Modules");
            if (node != null)
            {
                foreach (XmlNode m in node.ChildNodes)
                {
                    Modules.Add(new Module(m));
                }
            }

            //
            // Load the pages from the document.
            //
            node = doc.SelectSingleNode("//ArenaPackage/Pages");
            if (node != null)
            {
                foreach (XmlNode p in node.ChildNodes)
                {
                    Pages.Add(new PageInstance(p));
                }
            }

            //
            // Load the Package Information.
            //
            node = doc.SelectSingleNode("//ArenaPackage/Info");
            if (node != null)
            {
                _Info = new PackageInfo(node);
            }
            else
            {
                _Info = new PackageInfo();
            }

            //
            // Load the migration information.
            //
            node = doc.SelectSingleNode("//ArenaPackage/Migration");
            if (node != null)
            {
                if (node.Attributes["_source"] != null)
                {
                    MigrationSource = node.Attributes["_source"].Value;
                }

                if (!String.IsNullOrEmpty(node.InnerText))
                {
                    _Migration = Convert.FromBase64String(node.InnerText);
                }
            }

            _XmlPackage = doc;
        }

        /// <summary>
        /// This method creates a new XmlDocument that contains all the
        /// information that is needed to re-load this Package object back
        /// into memory later for editing or Exporting to Arena. This method
        /// does not create an Xml document that can be used to Import directly
        /// into Arena.
        /// </summary>
        /// <param name="isExport">Identifies if this Save operation is for exporting to Arena.</param>
        /// <returns>A XmlDocument that can be written to disc or other storage medium.</returns>
        private XmlDocument Save(Boolean isExport)
        {
            XmlDocument doc;
            XmlDeclaration decl;
            XmlNode nodeRoot, node;
            XmlAttribute attrib;
            FileInfo fi;


            //
            // Setup the XML document.
            //
            doc = new XmlDocument();
            decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(decl);
            nodeRoot = doc.CreateElement("ArenaPackage");
            attrib = doc.CreateAttribute("version");
            attrib.InnerText = "2009.2.100.1401";
            nodeRoot.Attributes.Append(attrib);
            doc.AppendChild(nodeRoot);

            //
            // Process the readme.
            //
            node = doc.CreateElement("Readme");
            node.InnerText = Readme;
            nodeRoot.AppendChild(node);

            //
            // Process the modules.
            //
            node = doc.CreateElement("Modules");
            foreach (Module module in Modules)
            {
                node.AppendChild(module.Save(doc, isExport));
            }
            nodeRoot.AppendChild(node);

            //
            // Process the pages.
            //
            node = doc.CreateElement("Pages");
            foreach (PageInstance page in Pages)
            {
                node.AppendChild(page.Save(doc, isExport));
            }
            nodeRoot.AppendChild(node);

            //
            // Process the stand alone files.
            //
            if (isExport && node.ChildNodes.Count > 0)
            {
                node = node.ChildNodes[0];
            }
            else
            {
                node = doc.CreateElement("Files");
                nodeRoot.AppendChild(node);
            }
            foreach (File file in Files)
            {
                node.AppendChild(file.Save(doc, isExport));
            }

            //
            // Store the package information.
            //
            if (Info != null)
            {
                nodeRoot.AppendChild(Info.Save(doc, isExport));
            }

            //
            // Store the migration data.
            //
            node = doc.CreateElement("Migration");
            if (isExport && !String.IsNullOrEmpty(MigrationSource))
            {
                fi = null;
                try
                {
                    fi = new FileInfo((MigrationSource[1] == ':' ? "" : BasePath) + MigrationSource);
                }
                catch { }

                if (fi == null || fi.Exists == false)
                {
                    BuildMessages.Add(new BuildMessage(BuildMessageType.Error,
                        String.Format("The local migration file {0} does not exist.", MigrationSource)));
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
            else if (!isExport)
            {
                attrib = doc.CreateAttribute("_source");
                attrib.Value = (!String.IsNullOrEmpty(MigrationSource) ? MigrationSource : "");
                node.Attributes.Append(attrib);
            }
            nodeRoot.AppendChild(node);

            return doc;
        }

        /// <summary>
        /// Save this package as an XmlDocument that can be reloaded later to continue
        /// working with it.
        /// </summary>
        /// <returns>New XmlDocument that can be later reloaded.</returns>
        public XmlDocument Save()
        {
            return this.Save(false);
        }

        /// <summary>
        /// Build the Package information into a format that can be imported into
        /// Arena via the standard Page Import wizard. If any errors occurred during
        /// the build then the XmlPackage will remain null, otherwise it will be set
        /// to a valid XmlDocument.
        /// </summary>
        /// <param name="basePath">The base path to use for relative file names while building package.</param>
        /// <returns>A list of messages as a result of the build.</returns>
        public BuildMessageCollection Build(String basePath)
        {
            //
            // Begin building the package and log messages.
            //
            _BuildMessages = new BuildMessageCollection();
            _XmlPackage = null;
            _BasePath = basePath + (basePath.EndsWith("\\") ? "" : "\\");
            _XmlPackage = this.Save(true);
            _BasePath = null;

            //
            // Check if there were any errors during the build.
            //
            foreach (BuildMessage message in BuildMessages)
            {
                if (message.Type == BuildMessageType.Error)
                {
                    _XmlPackage = null;
                    break;
                }
            }

            return BuildMessages;
        }

        /// <summary>
        /// Retrieve a list of all File objects that are contained in this
        /// package. Base files as well as files for pages, modules, etc.
        /// are included in this list.
        /// </summary>
        /// <returns>A List of File objects for this package.</returns>
        public List<File> AllFiles()
        {
            List<File> files = new List<File>();


            files.AddRange(Files);

            foreach (Module m in Modules)
            {
                files.AddRange(m.Files);
            }

            foreach (PageInstance p in Pages)
            {
                files.AddRange(p.Files);
            }

            return files;
        }

        /// <summary>
        /// Retrieve an ordered list of all pages for this package. This
        /// list is ordered in such a way that stepping forward through the
        /// list would allow the caller to create pages from the top down.
        /// Reversing the order would allow the caller to delete pages from
        /// the bottom up.
        /// </summary>
        /// <returns>A list of PageInstance objects that is order-safe.</returns>
        public List<PageInstance> OrderedPages()
        {
            List<PageInstance> pages = new List<PageInstance>();


            foreach (PageInstance page in Pages)
            {
                pages.Add(page);
                OrderedPages(page, ref pages);
            }

            return pages;
        }

        /// <summary>
        /// Recursively add all child pages into the list of pages.
        /// </summary>
        /// <param name="page">The page to add all child pages of into the list.</param>
        /// <param name="pages">The list of pages to add pages into.</param>
        private void OrderedPages(PageInstance page, ref List<PageInstance> pages)
        {
            foreach (PageInstance p in page.Pages)
            {
                pages.Add(p);
                OrderedPages(p, ref pages);
            }
        }

        #region Traversal methods for determining next available ID numbers.
        
        /// <summary>
        /// Retrieves the next Module ID number that can be used in this
        /// Package.
        /// </summary>
        /// <returns>New ID number for a Module.</returns>
        internal int NextAvailableModuleID()
        {
            int nextID = -1;


            foreach (Module m in Modules)
            {
                if (m._ModuleID <= nextID)
                    nextID = m._ModuleID - 1;
            }

            return nextID;
        }

        /// <summary>
        /// Retrieves the next available PageInstance ID number that can be
        /// used in this Package.
        /// </summary>
        /// <returns>New PageInstance ID number.</returns>
        internal int NextAvailablePageID()
        {
            int nextID = -1;


            foreach (PageInstance page in Pages)
            {
                int tempID = NextAvailablePageID(page);

                if (tempID <= nextID)
                    nextID = tempID;
            }

            return nextID;
        }

        /// <summary>
        /// Retrieves the next available PageInstance ID number in the page
        /// heiarchy of the specified parent page. This method only determines
        /// what the lowest ID number in it's heiarchy would be, not for the
        /// entire Package. This method is recursive.
        /// </summary>
        /// <param name="parentPage">The PageInstance to begin searching at.</param>
        /// <returns>Next available ID number for a new page under the parent page.</returns>
        private int NextAvailablePageID(PageInstance parentPage)
        {
            int nextID = -1;


            if (parentPage == null)
                return -1;

            if (parentPage._PageID <= nextID)
                nextID = parentPage._PageID - 1;

            foreach (PageInstance page in parentPage.Pages)
            {
                int tempID = NextAvailablePageID(page);

                if (tempID <= nextID)
                    nextID = tempID;
            }

            return nextID;
        }

        /// <summary>
        /// Determines the next available module instance ID number that can be used
        /// for a new ModuleInstance in this Package.
        /// </summary>
        /// <returns>Next available ModuleInstance ID number in this Package.</returns>
        internal int NextAvailableModuleInstanceID()
        {
            int nextID = -1;


            foreach (PageInstance page in Pages)
            {
                int tempID = NextAvailableModuleInstanceID(page);

                if (tempID <= nextID)
                    nextID = tempID;
            }

            return nextID;
        }

        /// <summary>
        /// Determines the next available module instance ID number in the
        /// page heiarchy of the identified parent PageInstance. This search
        /// is recursive. This method does not guarentee that the returned ID
        /// number is the next one that should be used, only the next lowest
        /// number under this page.
        /// </summary>
        /// <param name="parentPage">The PageInstance to begin at.</param>
        /// <returns>The next available module instance ID that can be used under this page.</returns>
        private int NextAvailableModuleInstanceID(PageInstance parentPage)
        {
            int nextID = -1;


            if (parentPage == null)
                return -1;

            foreach (PageInstance page in parentPage.Pages)
            {
                int tempID = NextAvailableModuleInstanceID(page);

                if (tempID <= nextID)
                    nextID = tempID;
            }

            foreach (ModuleInstance module in parentPage.Modules)
            {
                int tempID = module._ModuleInstanceID;

                if (tempID <= nextID)
                    nextID = tempID - 1;
            }

            return nextID;
        }

        #endregion

        #region Traversal methods for determining parentage.

        /// <summary>
        /// Searches all PageInstances in the Package and determines which page
        /// contains the specified page as it's direct child.
        /// </summary>
        /// <param name="page">The child page to search for.</param>
        /// <returns>The parent PageInstance containing the child page or null if not found.</returns>
        public PageInstance ParentOfPage(PageInstance page)
        {
            foreach (PageInstance parent in Pages)
            {
                PageInstance parentPage = ParentOfPage(parent, page);

                if (parentPage != null)
                    return parentPage;
            }

            return null;
        }

        /// <summary>
        /// Searches the heiarchy of the specified PageInstance and determines
        /// which page, if any, contains the identified child page. This method is
        /// recursive.
        /// </summary>
        /// <param name="parentPage">The parent to begin searching at.</param>
        /// <param name="page">The child page to search for.</param>
        /// <returns>The PageInstance containing the child page or null if not found.</returns>
        private PageInstance ParentOfPage(PageInstance parentPage, PageInstance page)
        {
            if (parentPage.Pages.Contains(page))
                return parentPage;

            foreach (PageInstance parent in parentPage.Pages)
            {
                PageInstance tmp = ParentOfPage(parent, page);

                if (tmp != null)
                    return tmp;
            }

            return null;
        }

        /// <summary>
        /// Finds the page that contains the specified ModuleInstance. If no page
        /// in this Package contains the ModuleInstance then null is returned.
        /// </summary>
        /// <param name="module">The ModuleInstance to search for.</param>
        /// <returns>The PageInstance that contains the ModuleInstance or null if not found.</returns>
        public PageInstance ParentOfModule(ModuleInstance module)
        {
            foreach (PageInstance parent in Pages)
            {
                PageInstance parentPage = ParentOfModule(parent, module);

                if (parentPage != null)
                    return parentPage;
            }

            return null;
        }

        /// <summary>
        /// Determine if the module is on either the specified page or a child
        /// page and returns the matching page. This method is recursive.
        /// </summary>
        /// <param name="parentPage">The PageInstance to begin searching at.</param>
        /// <param name="module">The ModuleInstance to look for.</param>
        /// <returns>PageInstance containing module or null if not found.</returns>
        private PageInstance ParentOfModule(PageInstance parentPage, ModuleInstance module)
        {
            if (parentPage.Modules.Contains(module))
                return parentPage;

            foreach (PageInstance parent in parentPage.Pages)
            {
                PageInstance tmp = ParentOfModule(parent, module);

                if (tmp != null)
                    return tmp;
            }

            return null;
        }

        #endregion
    }


    /// <summary>
    /// An object that contains information about a single message that
    /// was the result of building a Package.
    /// </summary>
    public class BuildMessage
    {
        internal BuildMessageType Type { get { return _Type; } }
        private BuildMessageType _Type;
        private String Message;


        /// <summary>
        /// Create a new BuildMessage object with the given mesage type and
        /// textual description of the message.
        /// </summary>
        /// <param name="type">The type of BuildMessage object to create.</param>
        /// <param name="message">The user readable message.</param>
        internal BuildMessage(BuildMessageType type, String message)
        {
            _Type = type;
            Message = message;
        }

        /// <summary>
        /// Convert the BuildMessage object into a String that can be
        /// displayed to the user.
        /// </summary>
        /// <returns>Textual representation of the message in a manner the user can read.</returns>
        public override String ToString()
        {
            return Type.ToString() + ": " + Message;
        }
    }


    /// <summary>
    /// A collection of build messages that should be displayed to the user.
    /// If any of the build messages is marked as an error then the build
    /// has failed.
    /// </summary>
    public class BuildMessageCollection : Collection<BuildMessage>
    {
        internal BuildMessageCollection()
            : base()
        {
        }

        /// <summary>
        /// Convert the collection of build messages into a single string that
        /// can be displayed to the user. Each build message is separated by
        /// a newline character.
        /// </summary>
        /// <returns>A <see cref="String"/> object that should be displayed to the user.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();


            foreach (BuildMessage message in this)
            {
                if (sb.Length == 0)
                    sb.Append(message.ToString());
                else
                    sb.Append(Environment.NewLine + message.ToString());
            }

            return sb.ToString();
        }
    }


    /// <summary>
    /// The type of build message, warning, error etc.
    /// </summary>
    public enum BuildMessageType
    {
        /// <summary>
        /// Warning messages should denote messages that are probably not
        /// critical but should be fixed anyway.
        /// </summary>
        Warning = 0,

        /// <summary>
        /// Error messages are those that prevent a build from being successful
        /// such as trying to reference a file that doesn't exist.
        /// </summary>
        Error = 1
    }
}
