using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;

namespace Arena.Custom.RC.Packager
{
    /// <summary>
    /// This class identifies a single Module in a Package. The information
    /// is used to build Arena Module's that can be imported and used on
    /// a PageInstance, as a ModuleInstance.
    /// </summary>
    public class Module
    {
        #region Properties

        internal int _ModuleID;

        /// <summary>
        /// The ID that uniquely identifies this module inside of the Package. If
        /// this module has not yet been associated with a Package this value will
        /// be 0, otherwise it will be a negative number.
        /// </summary>
        public int ModuleID
        {
            get
            {
                if (_ModuleID == 0 && Package != null)
                    _ModuleID = Package.NextAvailableModuleID();

                return _ModuleID;
            }
        }

        /// <summary>
        /// The name of this module as it will appear in the Modules list and
        /// the Page editor.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The URL location of the .ascx file for this module relative to the
        /// Arena web folder (contains the web.config file).
        /// </summary>
        public String URL { get; set; }

        /// <summary>
        /// The URL path to the module's image, relative to the Arena
        /// folder which contains the web.config file.
        /// </summary>
        public String ImagePath { get; set; }

        /// <summary>
        /// Identifies wether or not this module will allow child modules
        /// to reside underneath it.
        /// </summary>
        public Boolean AllowsChildModules { get; set; }

        /// <summary>
        /// The path to the source .ascx file on the local file system. If there
        /// is an accompanying .ascx.cs file it will be encoded as well
        /// automatically. It is recommended this path be a relative path.
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// The path to the source image for this Module. This is a path to
        /// the local file system and is recommended it be a relative path.
        /// </summary>
        public String SourceImage { get; set; }

        /// <summary>
        /// The description of this module as it will appear in Arena.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// The Package object that owns this Module. This property should
        /// not be set directly, it will automatically be set when this
        /// Module is added to a ModuleCollection object.
        /// </summary>
        public Package Package { get; set; }

        #endregion

        /// <summary>
        /// Creates a new, empty, Module that can be added to a Package
        /// and have it's properties set after that.
        /// </summary>
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

        /// <summary>
        /// Re-instantiates a Module object from the information in the XmlNode.
        /// </summary>
        /// <param name="node">An XmlNode that was created previously with the Save method.</param>
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

        /// <summary>
        /// Saves the Module as an XmlNode that can later be re-loaded to
        /// continue editing and otherwise working with the Module.
        /// </summary>
        /// <param name="doc">The XmlDocument that the node will be a member of.</param>
        /// <param name="isExport">Identifies if this Save operation is for exporting to Arena.</param>
        /// <returns>A new XmlNode object containing all information about this object.</returns>
        public XmlNode Save(XmlDocument doc, Boolean isExport)
        {
            XmlNode node = doc.CreateElement("Module");
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

            if (isExport == false)
            {
                attrib = doc.CreateAttribute("_source");
                attrib.InnerText = Source;
                node.Attributes.Append(attrib);

                attrib = doc.CreateAttribute("_source_image");
                attrib.InnerText = SourceImage;
                node.Attributes.Append(attrib);
            }

            if (isExport)
            {
                //
                // Export the contents of the Source file if there is one.
                //
                if (String.IsNullOrEmpty(Source))
                {
                    Package.BuildMessages.Add(new BuildMessage(BuildMessageType.Error,
                        String.Format("The module {0} does not include a source file path.", Name)));
                }
                else
                {
                    File f;

                    //
                    // Export the .ascx file.
                    //
                    f = new File(URL, Source, Package);
                    node.AppendChild(f.Save(doc, isExport));

                    //
                    // Export the .ascx.cs file if it exists.
                    //
                    f = new File(URL + ".cs", Source + ".cs", Package);
                    if (f.Exists)
                        node.AppendChild(f.Save(doc, isExport));
                }

                //
                // Export the contents of the SourceImage file if there is one.
                //
                if (!String.IsNullOrEmpty(SourceImage))
                {
                    File f;

                    f = new File(ImagePath, SourceImage, Package);
                    node.AppendChild(f.Save(doc, isExport));
                }
            }

            return node;
        }
    }

    /// <summary>
    /// A collection of Module objects that will be owned by a Package.
    /// This class can not be instantiated directly.
    /// </summary>
    public class ModuleCollection : Collection<Module>
    {
        /// <summary>
        /// The Package object that owns this collection and all Modules
        /// which are members of the collection.
        /// </summary>
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
        private Package _Owner;

        /// <summary>
        /// Creates a new ModuleCollection that has the specified Package
        /// as it's owner.
        /// </summary>
        /// <param name="owner">The Package object that will own all Module's in this collection.</param>
        internal ModuleCollection(Package owner)
            : base()
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Inserts a new Module into the collection and sets its Package
        /// reference to the owning Package.
        /// </summary>
        /// <param name="index">The index to place the new Module at.</param>
        /// <param name="item">The new Module to add to the collection.</param>
        protected override void InsertItem(int index, Module item)
        {
            item.Package = Owner;
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replace an existing Module with a new Module. Invalidates the
        /// old Module's Package reference and sets the new Module's Package
        /// reference.
        /// </summary>
        /// <param name="index">The index of the old Module to be replaced.</param>
        /// <param name="item">The new Module item to use in the replacement.</param>
        protected override void SetItem(int index, Module item)
        {
            this[index].Package = null;
            item.Package = Owner;
            base.SetItem(index, item);
        }

        /// <summary>
        /// Removes a Module from the collection and invalidates the
        /// reference to the owning Package.
        /// </summary>
        /// <param name="index">The index of the Module to remove.</param>
        protected override void RemoveItem(int index)
        {
            this[index].Package = null;
            base.RemoveItem(index);
        }
    }

    /// <summary>
    /// Identifies a single instance of a Module on a PageInstance.
    /// Each ModuleInstance object must be a member of a
    /// ModuleInstanceCollection, which is in turn a member property
    /// of the PageInstance class.
    /// </summary>
    public class ModuleInstance
    {
        #region

        internal int _ModuleInstanceID;
        private List<ModuleInstanceSetting> _Settings;

        /// <summary>
        /// Retrieves a unique ID number to identify this ModuleInstance
        /// in the Package. If the Package property has not been set it
        /// will have a value of 0, otherwise a negative number will be
        /// returned.
        /// </summary>
        public int ModuleInstanceID
        {
            get
            {
                if (_ModuleInstanceID == 0 && Package != null)
                    _ModuleInstanceID = Package.NextAvailableModuleID();

                return _ModuleInstanceID;
            }
        }

        /// <summary>
        /// Retrieves the order of this module for the particular frame
        /// this ModuleInstance is a member of.
        /// </summary>
        public int TemplateFrameOrder
        {
            get
            {
                int order = 0;

                foreach (ModuleInstance module in Page.Modules)
                {
                    if (this == module)
                        return order;

                    if (module.TemplateFrameName == this.TemplateFrameName)
                        order += 1;
                }

                return Int32.MaxValue;
            }
        }

        /// <summary>
        /// Whether or not to show the title in a dock module.
        /// </summary>
        public Boolean ShowTitle { get; set; }

        /// <summary>
        /// The title of this module. The title is only used in the
        /// administration area or if the module is a member of a dock
        /// control.
        /// </summary>
        public String ModuleTitle { get; set; }

        /// <summary>
        /// The name of the Frame on the page that this module will be
        /// a member of.
        /// </summary>
        public String TemplateFrameName { get; set; }

        /// <summary>
        /// The details of each module instance depend on the type of module it
        /// is an instance of. Many modules do not have any details at all.
        /// </summary>
        public String ModuleDetails { get; set; }

        /// <summary>
        /// The ID number to identify which type of module this is. The
        /// value should be that of the ModuleID property of a Module
        /// object belonging to the same Package.
        /// </summary>
        public Int32 ModuleTypeID { get; set; }

        /// <summary>
        /// Retrieves the PageInstance object that this object belongs
        /// to. Returns null if the ModuleInstance has no parent Page or
        /// the Package property has not yet been set.
        /// </summary>
        public PageInstance Page
        {
            get
            {
                if (Package == null)
                    return null;

                return Package.ParentOfModule(this);
            }
        }

        /// <summary>
        /// The ModuleInstanceSettings associated with this object.
        /// The property is read-only but the collection can be modified.
        /// </summary>
        public List<ModuleInstanceSetting> Settings { get { return _Settings; } }

        /// <summary>
        /// The Package object that owns this Module. This property
        /// should not be set directly as it is automatically set when
        /// you add this object to a Page's Modules member.
        /// </summary>
        public Package Package;

        #endregion

        /// <summary>
        /// Creates a new, empty, ModuleInstance object that can be
        /// added to an existing Page and populated.
        /// </summary>
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

        /// <summary>
        /// Creates a new ModuleInstance object from the data in the
        /// XmlNode. The node should be one that was created by a previous
        /// call to the Save method.
        /// </summary>
        /// <param name="node">The node containing the saved data to re-load.</param>
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

        /// <summary>
        /// Creates a new XmlNode object that will represent this object
        /// in a format that can be re-loaded at a later time to continue
        /// working with this object.
        /// </summary>
        /// <param name="doc">The XmlDocument that this node will be a part of.</param>
        /// <param name="isExport">Identifies if this Save operation is for exporting to Arena.</param>
        /// <returns>New XmlNode representing this object.</returns>
        public XmlNode Save(XmlDocument doc, Boolean isExport)
        {
            XmlNode instNode = doc.CreateElement("ModuleInstance");
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
                instNode.AppendChild(setting.Save(doc, isExport));
            }

            return instNode;
        }

        /// <summary>
        /// Retrieves a string that represents the older style of
        /// storing module settings for a ModuleInstance. This string
        /// must still be included for compatibility of older versions
        /// of Arena.
        /// </summary>
        /// <returns>A string in the format of Name=Value;Name=Value</returns>
        private String ModuleSettings()
        {
            StringBuilder sb = new StringBuilder();


            foreach (ModuleInstanceSetting setting in Settings)
            {
                String value = setting.SettingString();

                //
                // Only include settings if they had a value set.
                //
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

    /// <summary>
    /// A collection of ModuleInstance objects that will be owned
    /// by a specific Package object.
    /// </summary>
    public class ModuleInstanceCollection : Collection<ModuleInstance>
    {
        /// <summary>
        /// The Package object that owns this collection and all
        /// individual ModuleInstances. When this property is
        /// updated it will be propogated to all items in the
        /// collection.
        /// </summary>
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
        private Package _Owner;

        /// <summary>
        /// Creates a new ModuleInstanceCollection that will hold
        /// the ModuleInstances.
        /// </summary>
        internal ModuleInstanceCollection()
            : base()
        {
        }

        /// <summary>
        /// Creates a new collection for ModuleInstance objects with
        /// a Package to own them already set.
        /// </summary>
        /// <param name="owner">The owning Package for this collection.</param>
        internal ModuleInstanceCollection(Package owner)
            : base()
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Inserts a new ModuleInstance into this collection. Also sets
        /// the Package reference to the Owner of the collection.
        /// </summary>
        /// <param name="index">The index the item will be placed at.</param>
        /// <param name="item">The new ModuleInstance to insert into the collection.</param>
        protected override void InsertItem(int index, ModuleInstance item)
        {
            item.Package = Owner;
            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replaces an existing ModuleInstance at the given index with
        /// a new object. The old object has it's reference to the owning
        /// Package removed and the new object has it's Package reference
        /// set to the Owner.
        /// </summary>
        /// <param name="index">The index of the object to be replaced.</param>
        /// <param name="item">The new ModuleInstance to use in replacing the old one.</param>
        protected override void SetItem(int index, ModuleInstance item)
        {
            this[index].Package = null;
            item.Package = Owner;
            base.SetItem(index, item);
        }

        /// <summary>
        /// Remove a ModuleInstance from the collection and remove
        /// it's reference to the owning Package.
        /// </summary>
        /// <param name="index">The index of the ModuleInstance being removed.</param>
        protected override void RemoveItem(int index)
        {
            this[index].Package = null;
            base.RemoveItem(index);
        }
    }

    /// <summary>
    /// This class provides all the information needed to create and
    /// work with a user-defined setting for a ModuleInstance. A module
    /// can have zero or more settings associated with it and usually
    /// reflect the name given them in your module's source code.
    /// </summary>
    public class ModuleInstanceSetting
    {
        #region Properties

        /// <summary>
        /// The name of this module instance setting as it would
        /// appear in the Arena database.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The value, as a string, of this setting. If you don't
        /// know what format your value should have as a string it
        /// is recommended you look at the database value for a
        /// similiar setting type.
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// The type of setting, for example Page or Lookup.
        /// </summary>
        public ModuleInstanceSettingType Type { get; set; }

        /// <summary>
        /// The Guid (as a string) for this setting. Only certain module
        /// instance settings should have Guids, such as Lookups.
        /// </summary>
        public String Guid { get; set; }

        #endregion

        /// <summary>
        /// Creates a new, empty, module instance setting. Both the name
        /// and value default to empty strings.
        /// </summary>
        public ModuleInstanceSetting()
        {
            Name = "";
            Value = "";
            Type = ModuleInstanceSettingType.None;
            Guid = "";
        }

        /// <summary>
        /// Creates a new ModuleInstanceSetting with the specified name.
        /// The value is set to be an empty string.
        /// </summary>
        /// <param name="name">The name for this new setting.</param>
        public ModuleInstanceSetting(String name)
        {
            Name = name;
            Value = "";
            Type = ModuleInstanceSettingType.None;
            Guid = "";
        }

        /// <summary>
        /// Creates a new ModuleInstanceSetting object from the data in
        /// the XmlNode previously created by calling the Save method.
        /// </summary>
        /// <param name="node">The XmlNode that contains the data to re-load.</param>
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

        /// <summary>
        /// Creates a string that can be used by previous version of Arena
        /// that do not understand the new format of a setting being a child
        /// node to the ModuleInstance.
        /// </summary>
        /// <returns>A string object in the format of Name=Value.</returns>
        public String SettingString()
        {
            if (String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Value))
                return "";

            return Name + "=" + Value.Replace(";", "^^");
        }

        /// <summary>
        /// Creates a new XML Node that contains all the information
        /// needed to re-load the object for later use.
        /// </summary>
        /// <param name="doc">The XmlDocument that this node will be a part of.</param>
        /// <param name="isExport">Identifies if this Save operation is for exporting to Arena.</param>
        /// <returns>New XmlNode object which identifies this ModuleInstanceSetting.</returns>
        public XmlNode Save(XmlDocument doc, Boolean isExport)
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

    /// <summary>
    /// The types of settings that can be applied to an instance
    /// of a module.
    /// </summary>
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
