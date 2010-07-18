using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace Arena.Custom.RC.Packager
{
    public class PageInstance
    {
        internal int _PageID;
        private List<PageSetting> _Settings;

        public int PageID
        {
            get
            {
                if (_PageID == 0)
                    _PageID = Package.NextAvailablePageID();
                
                return _PageID;
            }
        }
        public String PageName { get; set; }
        public Boolean DisplayInNav { get; set; }
        public Boolean RequireSSL { get; set; }
        public Boolean ValidateRequest { get; set; }
        public String PageDescription { get; set; }
        public List<PageSetting> Settings { get { return _Settings; } }
        public Guid Guid { get; set; }

        private Package _Package;
        public Package Package
        {
            get { return _Package; }
            set {
                _Package = value;
                _Pages.Owner = value;
                _Modules.Owner = value;
            }
        }
        public PageInstance ParentPage
        {
            get
            {
                if (Package == null)
                    return null;

                return Package.ParentOfPage(this);
            }
        }
        private PageInstanceCollection _Pages;
        public PageInstanceCollection Pages { get { return _Pages; } }
        private ModuleInstanceCollection _Modules;
        public ModuleInstanceCollection Modules { get { return _Modules; } }

        public PageInstance()
        {
            _PageID = 0;
            PageName = "New Page";
            DisplayInNav = false;
            RequireSSL = false;
            ValidateRequest = true;
            PageDescription = "";
            Guid = Guid.NewGuid();
            SetupSettings();

            _Pages = new PageInstanceCollection();
            _Modules = new ModuleInstanceCollection();
        }

        public PageInstance(XmlNode node)
        {
            _PageID = Convert.ToInt32(node.Attributes["temp_page_id"].Value);
            PageName = node.Attributes["page_name"].Value;
            DisplayInNav = (node.Attributes["display_in_nav"].Value == "1" ? true : false);
            RequireSSL = (node.Attributes["require_ssl"].Value == "1" ? true : false);
            ValidateRequest = (node.Attributes["validate_request"].Value == "1" ? true : false);
            PageDescription = node.Attributes["page_desc"].Value;
            Guid = new Guid(node.Attributes["guid"].Value);
            SetupSettings(node.Attributes["page_settings"].Value.Split(new char[] { ';' }));

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
            }
        }

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

        public XmlElement Save(XmlDocument doc)
        {
            XmlElement pageNode = doc.CreateElement("Page");
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
                pageNode.AppendChild(module.Save(doc));
            }

            //
            // Add all page instances.
            //
            foreach (PageInstance page in Pages)
            {
                pageNode.AppendChild(page.Save(doc));
            }

            return pageNode;
        }

        private String PageSettings()
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

    public class PageInstanceCollection : Collection<PageInstance>
    {
        private Package _Owner;
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

        internal PageInstanceCollection()
            : base()
        {
        }

        internal PageInstanceCollection(Package owner)
            : base()
        {
            this.Owner = owner;
        }

        protected override void InsertItem(int index, PageInstance item)
        {
            item.Package = Owner;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, PageInstance item)
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

    public class PageSetting
    {
        public String Name;
        public String DisplayName;
        public String Value;

        public PageSetting()
        {
            Name = "";
            DisplayName = "";
            Value = "";
        }

        public PageSetting(String name)
        {
            Name = name;
            DisplayName = name;
            Value = "";
        }

        public PageSetting(String name, String value)
        {
            Name = name;
            DisplayName = name;
            Value = value;
        }

        public PageSetting(String name, String displayname, String value)
        {
            Name = name;
            DisplayName = displayname;
            Value = value;
        }

        public String SettingString()
        {
            if (String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Value))
                return "";

            return Name + "=" + Value.Replace(";", "^^");
        }
    }
}
