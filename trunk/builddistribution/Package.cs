using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Arena.Custom.RC.Packager
{
    public class Package
    {
        #region Properties

        private FileCollection _Files;
        public FileCollection Files { get { return _Files; } }

        private ModuleCollection _Modules;
        public ModuleCollection Modules { get { return _Modules; } }

        private PageInstanceCollection _Pages;
        public PageInstanceCollection Pages { get { return _Pages; } }

        public String Readme { get; set; }

        #endregion

        public Package()
        {
            _Files = new FileCollection(this);
            _Modules = new ModuleCollection(this);
            _Pages = new PageInstanceCollection(this);
            Readme = "";
        }

        public Package(XmlDocument doc)
            : this()
        {
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
            foreach (XmlNode node in doc.SelectSingleNode("//ArenaPackage/Files").ChildNodes)
            {
                Files.Add(new File(node));
            }

            //
            // Load the modules from the document.
            //
            foreach (XmlNode node in doc.SelectSingleNode("//ArenaPackage/Modules").ChildNodes)
            {
                Modules.Add(new Module(node));
            }

            //
            // Load the pages from the document.
            //
            foreach (XmlNode node in doc.SelectSingleNode("//ArenaPackage/Pages").ChildNodes)
            {
                Pages.Add(new PageInstance(node));
            }
        }

        public XmlDocument Save()
        {
            XmlDocument doc;
            XmlDeclaration decl;
            XmlNode nodeRoot, node;
            XmlAttribute attrib;


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
            // Process the stand alone files.
            //
            node = doc.CreateElement("Files");
            foreach (File file in Files)
            {
                node.AppendChild(file.Save(doc));
            }
            nodeRoot.AppendChild(node);

            //
            // Process the modules.
            //
            node = doc.CreateElement("Modules");
            foreach (Module module in Modules)
            {
                node.AppendChild(module.Save(doc));
            }
            nodeRoot.AppendChild(node);

            //
            // Process the pages.
            //
            node = doc.CreateElement("Pages");
            foreach (PageInstance page in Pages)
            {
                node.AppendChild(page.Save(doc));
            }
            nodeRoot.AppendChild(node);

            return doc;
        }

        #region Traversal methods for determining next available ID numbers.

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

        private int NextAvailablePageID(PageInstance parentPage)
        {
            int nextID = -1;


            if (parentPage == null)
                return -1;

            if (parentPage._PageID <= nextID)
                nextID = parentPage._PageID - 1;

            foreach (PageInstance page in parentPage.Pages)
            {
                if (page._PageID <= nextID)
                    nextID = page._PageID;
            }

            return nextID;
        }

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
}
