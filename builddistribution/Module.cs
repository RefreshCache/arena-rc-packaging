using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace Arena.Custom.RC.Packager
{
    public class Module
    {
        internal int _ModuleID;

        public int ModuleID
        {
            get
            {
                if (_ModuleID == 0 && Package != null)
                    _ModuleID = Package.NextAvailableModuleID();

                return _ModuleID;
            }
        }
        public String Name { get; set; }
        public String URL { get; set; }
        public String ImagePath { get; set; }
        public Boolean AllowsChildModules { get; set; }
        public String Source { get; set; }
        public String SourceImage { get; set; }
        public String Description { get; set; }
        public Package Package { get; set; }

        public Module()
        {
            _ModuleID = 0;
            Name = "New Module";
            URL = "";
            ImagePath = "";
            AllowsChildModules = false;
            Source = "";
            SourceImage = "";
            Description = "";
        }

        public Module(XmlNode node)
        {
            _ModuleID = Convert.ToInt32(node.Attributes["temp_module_id"].Value);
            Name = node.Attributes["module_name"].Value;
            URL = node.Attributes["module_url"].Value;
            ImagePath = node.Attributes["image_path"].Value;
            AllowsChildModules = (node.Attributes["allows_child_modules"].Value == "1" ? true : false);
            Source = node.Attributes["_source"].Value;
            SourceImage = node.Attributes["_source_image"].Value;
            Description = node.Attributes["module_desc"].Value;
        }

        public XmlElement Save(XmlDocument doc)
        {
            XmlElement node = doc.CreateElement("Module");
            XmlAttribute attrib;


            attrib = doc.CreateAttribute("temp_module_id");
            attrib.InnerText = ModuleID.ToString();
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("module_url");
            attrib.InnerText = URL;
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("module_name");
            attrib.InnerText = Name;
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("module_desc");
            attrib.InnerText = Description;
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("allows_child_modules");
            attrib.InnerText = (AllowsChildModules == true ? "1" : "0");
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("image_path");
            attrib.InnerText = ImagePath;
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("_source");
            attrib.InnerText = Source;
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("_source_image");
            attrib.InnerText = SourceImage;
            node.Attributes.Append(attrib);

            return node;
        }
    }

    public class ModuleCollection : Collection<Module>
    {
        private Package _Owner;
        public Package Owner
        {
            get { return _Owner; }
            set
            {
                _Owner = value;
                foreach (Module module in this)
                {
                    module.Package = value;
                }
            }
        }

        internal ModuleCollection(Package owner)
            : base()
        {
            this.Owner = owner;
        }

        protected override void InsertItem(int index, Module item)
        {
            item.Package = Owner;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, Module item)
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

    public class ModuleInstance
    {
        internal int _ModuleInstanceID;
        private List<ModuleInstanceSetting> _Settings;

        public int ModuleInstanceID
        {
            get
            {
                if (_ModuleInstanceID == 0 && Package != null)
                    _ModuleInstanceID = Package.NextAvailableModuleID();

                return _ModuleInstanceID;
            }
        }
        public int TemplateFrameOrder
        {
            get
            {
                int order = 0;

                foreach (ModuleInstance module in Page.Modules)
                {
                    if (this == module)
                        return order;

                    order += 1;
                }

                return Int32.MaxValue;
            }
        }
        public Boolean ShowTitle { get; set; }
        public String ModuleTitle { get; set; }
        public String TemplateFrameName { get; set; }
        public String ModuleDetails { get; set; }
        public Int32 ModuleTypeID { get; set; }
        public PageInstance Page
        {
            get
            {
                if (Package == null)
                    return null;

                return Package.ParentOfModule(this);
            }
        }
        public List<ModuleInstanceSetting> Settings { get { return _Settings; } }
        public Package Package;

        public ModuleInstance()
        {
            _ModuleInstanceID = 0;
            ModuleTitle = "New Module";
            ShowTitle = false;
            TemplateFrameName = "Main";
            ModuleDetails = "";
            ModuleTypeID = -1;
            _Settings = new List<ModuleInstanceSetting>();
        }

        public ModuleInstance(XmlNode node)
        {
            _ModuleInstanceID = Convert.ToInt32(node.Attributes["temp_module_instance_id"].Value);
            ModuleTitle = node.Attributes["module_title"].Value;
            ShowTitle = (node.Attributes["show_title"].Value == "1" ? true : false);
            TemplateFrameName = node.Attributes["template_frame_name"].Value;
            ModuleDetails = node.Attributes["module_details"].Value;
            ModuleTypeID = Convert.ToInt32(node.Attributes["temp_module_id"].Value);

            _Settings = new List<ModuleInstanceSetting>();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "Setting")
                {
                    ModuleInstanceSetting setting = new ModuleInstanceSetting(child);

                    Settings.Add(setting);
                }
            }
        }

        public XmlElement Save(XmlDocument doc)
        {
            XmlElement instNode = doc.CreateElement("ModuleInstance");
            XmlAttribute attrib;


            attrib = doc.CreateAttribute("temp_module_instance_id");
            attrib.InnerText = ModuleInstanceID.ToString();
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("module_title");
            attrib.InnerText = ModuleTitle;
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("show_title");
            attrib.InnerText = (ShowTitle == true ? "1" : "0");
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("template_frame_name");
            attrib.InnerText = TemplateFrameName;
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("template_frame_order");
            attrib.InnerText = TemplateFrameOrder.ToString();
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("module_details");
            attrib.InnerText = ModuleDetails;
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("system_flag");
            attrib.InnerText = "0";
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("mandatory");
            attrib.InnerText = "0";
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("movable");
            attrib.InnerText = "0";
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("description");
            attrib.InnerText = "";
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("image_path");
            attrib.InnerText = "";
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("temp_module_id");
            attrib.InnerText = ModuleTypeID.ToString();
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("temp_page_id");
            attrib.InnerText = Page.PageID.ToString();
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("module_settings");
            attrib.InnerText = ModuleSettings();
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("temp_page_or_template_id");
            attrib.InnerText = Page.PageID.ToString();
            instNode.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("page_instance");
            attrib.InnerText = "1";
            instNode.Attributes.Append(attrib);

            //
            // Add all the module instance settings.
            //
            foreach (ModuleInstanceSetting setting in Settings)
            {
                instNode.AppendChild(setting.Save(doc));
            }

            return instNode;
        }

        private String ModuleSettings()
        {
            StringBuilder sb = new StringBuilder();


            foreach (ModuleInstanceSetting setting in Settings)
            {
                String value = setting.SettingString();

                if (String.IsNullOrEmpty(value))
                {
                    if (sb.Length > 0)
                        sb.Append(";");
                    sb.Append(value);
                }
            }

            return sb.ToString();
        }
    }

    public class ModuleInstanceCollection : Collection<ModuleInstance>
    {
        private Package _Owner;
        public Package Owner
        {
            get { return _Owner; }
            set
            {
                _Owner = value;
                foreach (ModuleInstance module in this)
                {
                    module.Package = value;
                }
            }
        }

        internal ModuleInstanceCollection()
            : base()
        {
        }

        internal ModuleInstanceCollection(Package owner)
            : base()
        {
            this.Owner = owner;
        }

        protected override void InsertItem(int index, ModuleInstance item)
        {
            item.Package = Owner;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, ModuleInstance item)
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

    public class ModuleInstanceSetting
    {
        public String Name { get; set; }
        public String Value { get; set; }
        public ModuleInstanceSettingType Type { get; set; }
        public String Guid { get; set; }

        public ModuleInstanceSetting()
        {
            Name = "";
            Value = "";
            Type = ModuleInstanceSettingType.None;
            Guid = "";
        }

        public ModuleInstanceSetting(String name)
        {
            Name = name;
            Value = "";
            Type = ModuleInstanceSettingType.None;
            Guid = "";
        }

        public ModuleInstanceSetting(XmlNode node)
        {
            Name = node.Attributes["name"].Value;
            Value = node.Attributes["value"].Value;
            Type = (ModuleInstanceSettingType)Convert.ToInt32(node.Attributes["type_id"].Value);
            if (node.Attributes["guid"] != null)
                Guid = node.Attributes["guid"].Value;
            else
                Guid = "";
        }

        public String SettingString()
        {
            if (String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Value))
                return "";

            return Name + "=" + Value.Replace(";", "^^");
        }

        public XmlElement Save(XmlDocument doc)
        {
            XmlElement node = doc.CreateElement("Setting");
            XmlAttribute attrib;


            attrib = doc.CreateAttribute("name");
            attrib.InnerText = Name;
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("value");
            attrib.InnerText = Value;
            node.Attributes.Append(attrib);

            attrib = doc.CreateAttribute("type_id");
            attrib.InnerText = ((int)Type).ToString();
            node.Attributes.Append(attrib);

            if (!String.IsNullOrEmpty(Guid))
            {
                attrib = doc.CreateAttribute("guid");
                attrib.InnerText = Guid;
                node.Attributes.Append(attrib);
            }

            return node;
        }
    }

    public enum ModuleInstanceSettingType
    {
        None = 0,
        Text = 1,
        Number = 2,
        Page = 3,
        Boolean = 4,
        Css = 5,
        Image = 6,
        Tag = 7,
        Metric = 8,
        Date = 9,
        Lookup = 10,
        Cluster = 11,
        ClusterType = 12,
        CustomList = 13,
        File = 14,
        GatewayAccount = 15,
        PagesAsTabs = 16,
        ListFromSql = 17,
        Report = 18,
        Campus = 19,
        DocumentType = 20,
        Person = 21
    }
}
