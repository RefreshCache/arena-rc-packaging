﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace RefreshCache.Packager
{
    /// <summary>
    /// This class provides all the information needed to construct a page
    /// that can be imported into Arena.
    /// </summary>
    public class PageInstance
    {
        #region Properties

        /// <summary>
        /// Unique ID number that identifies the page.
        /// </summary>
        public int PageID
        {
            get
            {
                if (_PageID == 0)
                    _PageID = Package.NextAvailablePageID();
                
                return _PageID;
            }
        }
        internal int _PageID;

        /// <summary>
        /// The name of the page as it will be displayed in Arena.
        /// </summary>
        public String PageName { get; set; }
        
        /// <summary>
        /// If set to true then this page will be displayed in any navigation
        /// modules.
        /// </summary>
        public Boolean DisplayInNav { get; set; }
        
        /// <summary>
        /// Wether or not this page will require SSL to access it.
        /// </summary>
        public Boolean RequireSSL { get; set; }
        
        /// <summary>
        /// Wether or not the Arena Framework will validate postback data
        /// when loading this page.
        /// </summary>
        public Boolean ValidateRequest { get; set; }
        
        /// <summary>
        /// The description of the Page as it will be displayed in Arena.
        /// </summary>
        public String PageDescription { get; set; }
        
        /// <summary>
        /// Collection of PageSettings whose values can be modified by the user.
        /// </summary>
        public List<PageSetting> Settings { get { return _Settings; } }
        private List<PageSetting> _Settings;
        
        /// <summary>
        /// The unique GUID that identifies this PageInstance. While technically
        /// not correct to store this as a String instead of a Guid object we do
        /// so to allow the user to enter whatever value they desire.
        /// </summary>
        public String Guid { get; set; }

        /// <summary>
        /// Retrieves a list of all File objects for this page. This property
        /// will contain an empty list if the package loaded is not a fully
        /// built package.
        /// </summary>
        public FileCollection Files { get { return _Files; } }
        private FileCollection _Files;

        /// <summary>
        /// The Package object that owns this PageInstance.
        /// </summary>
        public Package Package
        {
            get { return _Package; }
            set {
                _Package = value;
                _Pages.Owner = value;
                _Modules.Owner = value;
                _Files.Owner = value;
            }
        }
        private Package _Package;
        
        /// <summary>
        /// The parent PageInstance of this page. If this is a root page or
        /// the Package member has not yet been set then null is returned.
        /// </summary>
        public PageInstance ParentPage
        {
            get
            {
                if (Package == null)
                    return null;

                return Package.ParentOfPage(this);
            }
        }

        /// <summary>
        /// Collection of PageInstance objects that are sub-pages of this
        /// page.
        /// </summary>
        public PageInstanceCollection Pages { get { return _Pages; } }
        private PageInstanceCollection _Pages;

        /// <summary>
        /// Collection of ModuleInstance objects that are members of this page.
        /// </summary>
        public ModuleInstanceCollection Modules { get { return _Modules; } }
        private ModuleInstanceCollection _Modules;

        #endregion

        /// <summary>
        /// Creates a new, empty, PageInstance object that can be setup by
        /// the user to contain the required information.
        /// </summary>
        public PageInstance()
        {
            _PageID = 0;
            PageName = "New Page";
            DisplayInNav = false;
            RequireSSL = false;
            ValidateRequest = true;
            PageDescription = "";
            Guid = System.Guid.NewGuid().ToString();
            SetupSettings();

            _Pages = new PageInstanceCollection();
            _Modules = new ModuleInstanceCollection();
            _Files = new FileCollection(null);
        }

        /// <summary>
        /// Creates a new PageInstance object from the contents of a previously
        /// saved PageInstance. The user can then continue to make modifications
        /// to the PageInstance.
        /// </summary>
        /// <param name="node">The XmlNode that was generated by a previous call to the Save method.</param>
        public PageInstance(XmlNode node)
        {
            _PageID = Convert.ToInt32(node.Attributes["temp_page_id"].Value);
            PageName = node.Attributes["page_name"].Value;
            DisplayInNav = (node.Attributes["display_in_nav"].Value == "1" ? true : false);
            RequireSSL = (node.Attributes["require_ssl"].Value == "1" ? true : false);
            ValidateRequest = (node.Attributes["validate_request"].Value == "1" ? true : false);
            PageDescription = node.Attributes["page_desc"].Value;
            Guid = node.Attributes["guid"].Value;
            SetupSettings(node.Attributes["page_settings"].Value.Split(new char[] { ';' }));

            _Files = new FileCollection(null);
            _Pages = new PageInstanceCollection();
            _Modules = new ModuleInstanceCollection();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "Page")
                {
                    Pages.Add(new PageInstance(child));
                }
                else if (child.Name == "ModuleInstance")
                {
                    Modules.Add(new ModuleInstance(child));
                }
                else if (child.Name == "File")
                {
                    Files.Add(new File(child));
                }
            }
        }

        /// <summary>
        /// Initializes the basic page settings with blank values that
        /// can be set by the user later.
        /// </summary>
        private void SetupSettings()
        {
            _Settings = new List<PageSetting>();

            _Settings.Add(new PageSetting("EnableViewState", "Enable View State", ""));
            _Settings.Add(new PageSetting("ShowCss", "Default CSS Flag", ""));
            _Settings.Add(new PageSetting("MainStyle", "Main CSS File", ""));
            _Settings.Add(new PageSetting("TreeStyle", "Tree View CSS File", ""));
            _Settings.Add(new PageSetting("NavStyle", "Navigation CSS File", ""));
            _Settings.Add(new PageSetting("ShowScript", "Field Hint Script", ""));
            _Settings.Add(new PageSetting("BreadCrumbs", "Bread Crumbs", ""));
            _Settings.Add(new PageSetting("NavBarIcon", "Navigation Bar Icon", ""));
            _Settings.Add(new PageSetting("NavBarHoverIcon", "Navigation Bar Icon Hover", ""));
            _Settings.Add(new PageSetting("Target", "Target", ""));
            _Settings.Add(new PageSetting("ItemBgColor", "DataGrid Item BG Color", ""));
            _Settings.Add(new PageSetting("ItemAltBgColor", "DataGrid Alt Item BG Color", ""));
            _Settings.Add(new PageSetting("ItemMouseOverColor", "DataGrid Mouse BG Color", ""));
        }

        /// <summary>
        /// Initializes the settings for this PageInstance. Creates all the
        /// PageSetting objects and sets their initial values to those
        /// previously saved.
        /// </summary>
        /// <param name="savedSettings">An array of Name=Value strings that contains the saved setting values.</param>
        private void SetupSettings(String[] savedSettings)
        {
            SetupSettings();

            foreach (String s in savedSettings)
            {
                String[] splits = s.Split(new char[] { '=' });

                foreach (PageSetting setting in Settings)
                {
                    if (setting.Name == splits[0])
                        setting.Value = splits[1].Replace("^^", ";");
                }
            }
        }

        /// <summary>
        /// Creates a new XmlNode that contains all the information needed to
        /// re-load this PageInstance and all child pages and modules at a later
        /// point in time.
        /// </summary>
        /// <param name="doc">The XmlDocument that will contain this node.</param>
        /// <param name="isExport">Identifies if this Save operation is for exporting to Arena.</param>
        /// <returns>A new XmlNode that can then be added to the parent node.</returns>
        public XmlNode Save(XmlDocument doc, Boolean isExport)
        {
            XmlNode pageNode = doc.CreateElement("Page");
            XmlAttribute attrib;


            attrib = doc.CreateAttribute("temp_page_id");
            attrib.InnerText = PageID.ToString();
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("page_order");
            attrib.InnerText = "2147483647";
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("display_in_nav");
            attrib.InnerText = (DisplayInNav == true ? "1" : "0");
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("page_name");
            attrib.InnerText = PageName;
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("page_desc");
            attrib.InnerText = PageDescription;
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("page_settings");
            attrib.InnerText = PageSettings();
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("require_ssl");
            attrib.InnerText = (RequireSSL == true ? "1" : "0");
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("guid");
            attrib.InnerText = Guid.ToString();
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("system_flag");
            attrib.InnerText = "0";
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("friendly_url");
            attrib.InnerText = "";
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("validate_request");
            attrib.InnerText = (ValidateRequest == true ? "1" : "0");
            pageNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("temp_parent_page_id");
            attrib.InnerText = (ParentPage == null ? "0" : ParentPage.PageID.ToString());
            pageNode.Attributes.Append(attrib);

            //
            // Add all module instances.
            //
            foreach (ModuleInstance module in Modules)
            {
                pageNode.AppendChild(module.Save(doc, isExport));
            }

            //
            // Add all page instances.
            //
            foreach (PageInstance page in Pages)
            {
                pageNode.AppendChild(page.Save(doc, isExport));
            }

            return pageNode;
        }

        /// <summary>
        /// Retrieve a textual representation of this page's settings.
        /// </summary>
        /// <returns>A string in the format of Name=Value;Name=Value.</returns>
        public String PageSettings()
        {
            StringBuilder sb = new StringBuilder();


            foreach (PageSetting setting in Settings)
            {
                String value = setting.SettingString();

                if (!String.IsNullOrEmpty(value))
                {
                    if (sb.Length > 0)
                        sb.Append(";");
                    sb.Append(value);
                }
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// A collection to hold the PageInstance objects of a Package.
    /// This class provides helpful functions to keep all the objects
    /// in sync with their owning Package.
    /// </summary>
    public class PageInstanceCollection : Collection<PageInstance>
    {
        /// <summary>
        /// The owning Package of this collection. If the Owner is updated
        /// then all PageInstance items in the collection also have their
        /// Package property updated as well.
        /// </summary>
        public Package Owner
        {
            get { return _Owner; }
            set
            {
                _Owner = value;
                foreach (PageInstance page in this)
                {
                    page.Package = value;
                }
            }
        }
        private Package _Owner;

        /// <summary>
        /// Creates a new PageInstanceCollection which does not yet have a
        /// owner specified. You can specify the owner at a later point in
        /// time.
        /// </summary>
        internal PageInstanceCollection()
            : base()
        {
        }

        /// <summary>
        /// Creates a new collection object for holding PageInstance objects and
        /// specifies the Package that will own those PageInstances.
        /// </summary>
        /// <param name="owner">The Package that will own the PageInstances.</param>
        internal PageInstanceCollection(Package owner)
            : base()
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Inserts a new PageInstance in the collection and sets the Package
        /// reference to the Owner of the collection.
        /// </summary>
        /// <param name="index">The index at which to insert the new PageInstance.</param>
        /// <param name="item">The new PageInstance to insert.</param>
        protected override void InsertItem(int index, PageInstance item)
        {
            item.Package = Owner;
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replace an existing PageInstance object with a new PageInstance
        /// object. Also removes the Package reference from the old object
        /// and adds it to the new object.
        /// </summary>
        /// <param name="index">The index of the old item to be replaced.</param>
        /// <param name="item">The new PageInstance that will replace the old.</param>
        protected override void SetItem(int index, PageInstance item)
        {
            this[index].Package = null;
            item.Package = Owner;
            base.SetItem(index, item);
        }

        /// <summary>
        /// Remove a PageInstance from the collection and remove the reference
        /// to the Package owning that PageInstance.
        /// </summary>
        /// <param name="index">The index of the item being removed.</param>
        protected override void RemoveItem(int index)
        {
            this[index].Package = null;
            base.RemoveItem(index);
        }
    }

    /// <summary>
    /// This class provides the means to configure settings for PageInstance
    /// objects. The setting names are essentially hard-coded. The user cannot
    /// create extra PageSettings at whim, only edit existing ones.
    /// </summary>
    public class PageSetting
    {
        #region Properties

        /// <summary>
        /// The database name of this PageSetting. This is the internal name
        /// and should not be displayed to the user.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The name that will be used to display to the user.
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// The value that this setting has as a textual representation.
        /// </summary>
        public String Value { get; set; }

        #endregion

        /// <summary>
        /// Creates a new, empty, PageSetting object that can be configured
        /// at a later point in time.
        /// </summary>
        internal PageSetting()
        {
            Name = "";
            DisplayName = "";
            Value = "";
        }

        /// <summary>
        /// Creates a new PageSetting object with the specified setting name.
        /// </summary>
        /// <param name="name">The name this new setting will have.</param>
        internal PageSetting(String name)
            : this()
        {
            Name = name;
            DisplayName = name;
        }

        /// <summary>
        /// Creates a new PageSetting object that will have the specified name
        /// and value from the parameters.
        /// </summary>
        /// <param name="name">The name of this setting (will also be used for the DisplayName).</param>
        /// <param name="value">Initial value of this setting.</param>
        internal PageSetting(String name, String value)
            : this(name)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new PageSetting object that is fully populated with the
        /// specified parameter values.
        /// </summary>
        /// <param name="name">Identifies the name this setting should have.</param>
        /// <param name="displayname">The name that will be used to display this setting to the user.</param>
        /// <param name="value">Initial value for this setting.</param>
        internal PageSetting(String name, String displayname, String value)
        {
            Name = name;
            DisplayName = displayname;
            Value = value;
        }

        /// <summary>
        /// Retrieve a textual representation of this setting's name and
        /// value.
        /// </summary>
        /// <returns>A string in the format of Name=Value.</returns>
        public String SettingString()
        {
            if (String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Value))
                return "";

            return Name + "=" + Value.Replace(";", "^^");
        }
    }
}
