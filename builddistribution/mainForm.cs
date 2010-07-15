using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace BuildDistribution
{
    public partial class mainForm : Form
    {
        private Boolean selectionChanging = false;
        static public List<Module> Modules = null;

        public mainForm()
        {
            InitializeComponent();
            Modules = new List<Module>();

            importFiles(null);

            tcMain.SelectedIndexChanged += new EventHandler(tcMain_SelectedIndexChanged);
            //
            // Setup all the module tab actions and events.
            //
            tbModuleName.TextChanged += new EventHandler(tbModuleName_TextChanged);
            tbModuleURL.TextChanged += new EventHandler(tbModuleURL_TextChanged);
            tbModuleImagePath.TextChanged += new EventHandler(tbModuleImagePath_TextChanged);
            cbModuleAllowsChildModules.CheckedChanged += new EventHandler(cbModuleAllowsChildModules_CheckedChanged);
            tbModuleSourcePath.TextChanged += new EventHandler(tbModuleSourcePath_TextChanged);
            tbModuleSourceImagePath.TextChanged += new EventHandler(tbModuleSourceImagePath_TextChanged);
            tbModuleDescription.TextChanged += new EventHandler(tbModuleDescription_TextChanged);
            dgModules.SelectionChanged += new EventHandler(dgModules_SelectionChanged);
            dgModules_SelectionChanged(null, null);

            //
            // Setup all the page tab actions and events.
            //
            tvPages.BeforeSelect += new TreeViewCancelEventHandler(tvPages_BeforeSelect);
            tvPages.AfterSelect += new TreeViewEventHandler(tvPages_AfterSelect);
            tbPageName.TextChanged += new EventHandler(tbPageName_TextChanged);
            cbPageDisplayInNav.CheckedChanged += new EventHandler(cbPageDisplayInNav_CheckedChanged);
            cbPageRequireSSL.CheckedChanged += new EventHandler(cbPageRequireSSL_CheckedChanged);
            cbPageValidateRequest.CheckedChanged += new EventHandler(cbPageValidateRequest_CheckedChanged);
            tbPageDescription.TextChanged += new EventHandler(tbPageDescription_TextChanged);
            dgPageSettings.VirtualMode = true;
            dgPageSettings.CellValueNeeded += new DataGridViewCellValueEventHandler(dgPageSettings_CellValueNeeded);
            dgPageSettings.CellValuePushed += new DataGridViewCellValueEventHandler(dgPageSettings_CellValuePushed);
            dgPageSettings.RowCount = 0;
            DataGridViewComboBoxColumn box = (DataGridViewComboBoxColumn)dgModuleInstanceSettings.Columns["Type"];
            box.Items.AddRange(Enum.GetNames(typeof(ModuleInstanceSettingType)));
            dgModuleInstanceSettings.VirtualMode = true;
            dgModuleInstanceSettings.CellValueNeeded += new DataGridViewCellValueEventHandler(dgModuleInstanceSettings_CellValueNeeded);
            dgModuleInstanceSettings.CellValuePushed += new DataGridViewCellValueEventHandler(dgModuleInstanceSettings_CellValuePushed);
            dgModuleInstanceSettings.UserDeletingRow += new DataGridViewRowCancelEventHandler(dgModuleInstanceSettings_UserDeletingRow);
            dgModuleInstanceSettings.RowCount = 1;
        }

        void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedValue;

            selectedValue = cbModuleInstanceType.SelectedValue;
            Console.WriteLine("Value " + (selectedValue == null ? "null" : selectedValue.ToString()));
            cbModuleInstanceType.DataSource = Modules.ToArray();
            if (selectedValue != null)
                cbModuleInstanceType.SelectedValue = selectedValue;
        }

        private PageInstance SelectedPageInstance()
        {
            if (tvPages.SelectedNode == null ||
                typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == false)
            {
                return null;
            }

            return (PageInstance)tvPages.SelectedNode.Tag;
        }

        private ModuleInstance SelectedModuleInstance()
        {
            if (tvPages.SelectedNode == null ||
                typeof(ModuleInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == false)
            {
                return null;
            }

            return (ModuleInstance)tvPages.SelectedNode.Tag;
        }

        private Module SelectedModule()
        {
            if (dgModules.SelectedCells.Count == 0)
                return null;

            return Modules[dgModules.SelectedCells[0].RowIndex];
        }

        #region Files Tab User Interface

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            dgFiles.Rows.Add();
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (dgFiles.SelectedCells.Count > 0)
            {
                dgFiles.Rows.RemoveAt(dgFiles.SelectedCells[0].RowIndex);
            }
        }


        #endregion

        #region Modules Tab User Interface

        void tbModuleDescription_TextChanged(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.Description = tbModuleDescription.Text;
        }

        void tbModuleSourceImagePath_TextChanged(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.SourceImage = tbModuleSourceImagePath.Text;
        }

        void tbModuleSourcePath_TextChanged(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.Source = tbModuleSourcePath.Text;
        }

        void cbModuleAllowsChildModules_CheckedChanged(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.AllowsChildModules = cbModuleAllowsChildModules.Checked;
        }

        void tbModuleImagePath_TextChanged(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.ImagePath = tbModuleImagePath.Text;
        }

        void tbModuleURL_TextChanged(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.URL = tbModuleURL.Text;
        }

        void tbModuleName_TextChanged(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
            {
                module.Name = tbModuleName.Text;
                dgModules.Rows[dgModules.SelectedCells[0].RowIndex].Cells[0].Value = tbModuleName.Text;
            }
        }

        void dgModules_SelectionChanged(object sender, EventArgs e)
        {
            Boolean enabled = false;


            selectionChanging = true;

            if (dgModules.SelectedCells.Count > 0)
            {
                Module module = Modules[dgModules.SelectedCells[0].RowIndex];

                tbModuleName.Text = module.Name;
                tbModuleURL.Text = module.URL;
                tbModuleImagePath.Text = module.ImagePath;
                cbModuleAllowsChildModules.Checked = module.AllowsChildModules;
                tbModuleSourcePath.Text = module.Source;
                tbModuleSourceImagePath.Text = module.SourceImage;
                tbModuleDescription.Text = module.Description;

                enabled = true;
            }
            else
            {
                tbModuleName.Text = "";
                tbModuleURL.Text = "";
                tbModuleImagePath.Text = "";
                cbModuleAllowsChildModules.Checked = false;
                tbModuleSourcePath.Text = "";
                tbModuleSourceImagePath.Text = "";
                tbModuleDescription.Text = "";
            }

            //
            // Enable or disable everything.
            //
            tbModuleName.Enabled = enabled;
            tbModuleURL.Enabled = enabled;
            tbModuleImagePath.Enabled = enabled;
            cbModuleAllowsChildModules.Enabled = enabled;
            tbModuleSourcePath.Enabled = enabled;
            tbModuleSourceImagePath.Enabled = enabled;
            tbModuleDescription.Enabled = enabled;

            selectionChanging = false;
        }

        private void btnAddModule_Click(object sender, EventArgs e)
        {
            Module module = new Module();


            Modules.Add(module);
            dgModules.Rows.Add();
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["module_name"].Value = module.Name;

            dgModules_SelectionChanged(null, null);
        }

        private void btnRemoveModule_Click(object sender, EventArgs e)
        {
            if (dgModules.SelectedCells.Count > 0)
            {
                Modules.RemoveAt(dgModules.SelectedCells[0].RowIndex);
                dgModules.Rows.RemoveAt(dgModules.SelectedCells[0].RowIndex);
            }
        }

        #endregion

        #region Pages Tab User Interface

        private void btnPagesAddPage_Click(object sender, EventArgs e)
        {
            PageInstance page;


            if (tvPages.SelectedNode != null && !typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()))
                return;

            page = new PageInstance();
            if (tvPages.SelectedNode != null)
                tvPages.SelectedNode.Nodes.Add(page.TreeNode);
            else
                tvPages.Nodes.Add(page.TreeNode);

            tvPages.SelectedNode = page.TreeNode;
        }

        private void btnPagesAddModule_Click(object sender, EventArgs e)
        {
            ModuleInstance module;
            PageInstance page;
            int i, lastIndex = -1;


            if (tvPages.SelectedNode == null || !typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()))
                return;
            page = (PageInstance)tvPages.SelectedNode.Tag;

            module = new ModuleInstance();
            for (i = 0; i < page.TreeNode.Nodes.Count; i++)
            {
                if (typeof(ModuleInstance).IsAssignableFrom(page.TreeNode.Nodes[i].Tag.GetType()))
                    lastIndex = i;
            }
            page.TreeNode.Nodes.Insert(lastIndex + 1, module.TreeNode);

            tvPages.SelectedNode = module.TreeNode;
        }

        void tbPageDescription_TextChanged(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.PageDescription = tbPageDescription.Text;
        }

        void cbPageValidateRequest_CheckedChanged(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.ValidateRequest = cbPageValidateRequest.Checked;
        }

        void cbPageRequireSSL_CheckedChanged(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.RequireSSL = cbPageRequireSSL.Checked;
        }

        void cbPageDisplayInNav_CheckedChanged(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.DisplayInNav = cbPageDisplayInNav.Checked;
        }

        void tbPageName_TextChanged(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.PageName = tbPageName.Text;
        }

        void tvPages_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            dgPageSettings.RowCount = 0;
            dgModuleInstanceSettings.RowCount = 1;
        }

        void tvPages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvPages.SelectedNode == null)
                return;

            selectionChanging = true;

            if (typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == true)
            {
                PageInstance page = (PageInstance)tvPages.SelectedNode.Tag;

                tbPageName.Text = page.PageName;
                cbPageDisplayInNav.Checked = page.DisplayInNav;
                cbPageRequireSSL.Checked = page.RequireSSL;
                cbPageValidateRequest.Checked = page.ValidateRequest;
                tbPageDescription.Text = page.PageDescription;
                dgPageSettings.RowCount = 13;

                tcPages.SelectedIndex = 0;
            }
            else if (typeof(ModuleInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == true)
            {
                ModuleInstance module = (ModuleInstance)tvPages.SelectedNode.Tag;

                tbModuleInstanceTitle.Text = module.ModuleTitle;
                cbModuleInstanceShowTitle.Checked = module.ShowTitle;
                tbModuleInstanceTemplateFrameName.Text = module.TemplateFrameName;
//                cbModuleInstanceType.SelectedIndex = 0;
                tbModuleInstanceDetails.Text = module.ModuleDetails;
                dgModuleInstanceSettings.RowCount = module.Settings.Count + 1;

                tcPages.SelectedIndex = 1;
            }

            selectionChanging = false;
        }

        void dgPageSettings_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            PageInstance page = (PageInstance)tvPages.SelectedNode.Tag;


            page.Settings[e.RowIndex].Value = e.Value.ToString();
        }

        void dgPageSettings_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (tvPages.SelectedNode == null ||
                typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == false)
            {
                e.Value = "";
            }
            else
            {
                PageInstance page = (PageInstance)tvPages.SelectedNode.Tag;

                if (e.ColumnIndex == 0)
                    e.Value = page.Settings[e.RowIndex].DisplayName;
                else
                    e.Value = page.Settings[e.RowIndex].Value;
            }
        }

        void dgModuleInstanceSettings_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            ModuleInstance module = SelectedModuleInstance();


            if (module != null)
            {
                module.Settings.RemoveAt(e.Row.Index);
            }
        }

        void dgModuleInstanceSettings_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            ModuleInstance module = SelectedModuleInstance();


            if (module == null)
                return;
            if (e.RowIndex == module.Settings.Count)
            {
                module.Settings.Add(new ModuleInstanceSetting());
                dgModuleInstanceSettings.UpdateCellValue(1, e.RowIndex);
            }

            if (e.ColumnIndex == 0)
                module.Settings[e.RowIndex].Name = e.Value.ToString();
            else if (e.ColumnIndex == 1)
                module.Settings[e.RowIndex].Type = (ModuleInstanceSettingType)Enum.Parse(typeof(ModuleInstanceSettingType), e.Value.ToString());
            else if (e.ColumnIndex == 2)
                module.Settings[e.RowIndex].Value = e.Value.ToString();
        }

        void dgModuleInstanceSettings_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            ModuleInstance module = SelectedModuleInstance();


            if (module == null || e.RowIndex == module.Settings.Count)
            {
                return;
            }

            if (e.ColumnIndex == 0)
                e.Value = module.Settings[e.RowIndex].Name;
            else if (e.ColumnIndex == 1)
                e.Value = module.Settings[e.RowIndex].Type.ToString();
            else if (e.ColumnIndex == 2)
                e.Value = module.Settings[e.RowIndex].Value;
        }

        #endregion

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlDocument doc;
            XmlDeclaration decl;
            XmlNode nodeRoot;
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
            // Process the stand alone files.
            //
            nodeRoot.AppendChild(BuildFilesNode(doc));

            //
            // Process the modules.
            //
            nodeRoot.AppendChild(BuildModulesNode(doc));

            //
            // Process the pages.
            //
            nodeRoot.AppendChild(BuildPagesNode(doc));

            //
            // Dump result.
            //
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            doc.Save(writer);
            Console.WriteLine(sb.ToString());
        }

        #region Import/Export methods

        private void importFiles(XmlDocument doc)
        {
            dgFiles.Rows.Add(new string[] { "UserControls/Test.ascx", "../../test.ascx" });
        }

        private void importModules(XmlDocument doc)
        {
        }

        private XmlNode BuildFilesNode(XmlDocument doc)
        {
            XmlNode nodeFiles, nodeFile;
            XmlAttribute attrib;
            int i;


            //
            // Process standalone files.
            //
            nodeFiles = doc.CreateElement("Files");
            for (i = 0; i < dgFiles.Rows.Count; i++)
            {
                nodeFile = doc.CreateElement("File");
                attrib = doc.CreateAttribute("path");
                attrib.InnerText = dgFiles.Rows[i].Cells[0].Value.ToString();
                nodeFile.Attributes.Append(attrib);
                attrib = doc.CreateAttribute("_source");
                attrib.InnerText = dgFiles.Rows[i].Cells[1].Value.ToString();
                nodeFile.Attributes.Append(attrib);

                nodeFiles.AppendChild(nodeFile);
            }

            return nodeFiles;
        }

        private XmlNode BuildModulesNode(XmlDocument doc)
        {
            XmlNode nodeModules, nodeModule;
            XmlAttribute attrib;
            int i;


            //
            // Process standalone files.
            //
            nodeModules = doc.CreateElement("Modules");
            for (i = 0; i < dgModules.Rows.Count; i++)
            {
                nodeModule = doc.CreateElement("Module");

                attrib = doc.CreateAttribute("module_name");
                attrib.InnerText = dgModules.Rows[i].Cells["module_name"].Value.ToString();
                nodeModule.Attributes.Append(attrib);
                
                nodeModules.AppendChild(nodeModule);
            }

            return nodeModules;
        }

        private XmlNode BuildPagesNode(XmlDocument doc)
        {
            //
            // Only allow a single root page.
            // When a module is selected show module editor:
            //      module_title, show_title, template_frame_name, template_frame_order,
            //      temp_module_id (drop down select from modules), module_settings,
            //      module_details (temp_page_or_template_id = temp_page_id, page_instance = 1)
            //
            XmlNode nodePages;


            //
            // Process standalone files.
            //
            nodePages = doc.CreateElement("Pages");
            if (tvPages.Nodes.Count > 0)
                BuildPageNode(doc, nodePages, (PageInstance)tvPages.Nodes[0].Tag);

            return nodePages;
        }

        void BuildPageNode(XmlDocument doc, XmlNode pagesNode, PageInstance page)
        {
            pagesNode.AppendChild(page.Export(doc));

            foreach (TreeNode node in page.TreeNode.Nodes)
            {
                if (typeof(PageInstance).IsAssignableFrom(node.Tag.GetType()))
                {
                    BuildPageNode(doc, pagesNode, (PageInstance)node.Tag);
                }
            }
        }

        #endregion

    }

    public class Module
    {
        private int _ModuleID;

        public int ModuleID
        {
            get
            {
                if (_ModuleID == 0)
                    _ModuleID = NextAvailableModuleID();

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

        static int NextAvailableModuleID()
        {
            int nextID = -1;


            foreach (Module m in mainForm.Modules)
            {
                if (m._ModuleID <= nextID)
                    nextID = m._ModuleID - 1;
            }

            Console.WriteLine("New ID is " + nextID.ToString());
            return nextID;
        }
    }

    class PageInstance
    {
        private int _PageID;
        private TreeNode _TreeNode;
        private String _PageName;
        private List<PageSetting> _Settings;
        private List<ModuleInstance> _Modules;

        public int PageID { get { if (_PageID == 0) _PageID = NextAvailablePageID((PageInstance)_TreeNode.TreeView.Nodes[0].Tag); return _PageID; } }
        public TreeNode TreeNode { get { return _TreeNode; } }
        public String PageName { get { return _PageName; } set { _PageName = value; _TreeNode.Text = value; } }
        public Boolean DisplayInNav;
        public Boolean RequireSSL;
        public Boolean ValidateRequest;
        public String PageDescription;
        public List<PageSetting> Settings { get { return _Settings; } }
        public List<ModuleInstance> Modules { get { return _Modules; } }
        public Guid Guid;

        static private int NextAvailablePageID(PageInstance parentPage)
        {
            int nextID = -1;


            if (parentPage == null)
                return -1;

            if (parentPage._PageID <= nextID)
                nextID = parentPage._PageID - 1;

            foreach (TreeNode node in parentPage._TreeNode.Nodes)
            {
                int tempID;

                if (typeof(PageInstance).IsAssignableFrom(node.Tag.GetType()))
                {
                    tempID = NextAvailablePageID((PageInstance)node.Tag);

                    if (tempID <= nextID)
                        nextID = tempID - 1;
                }
            }

            return nextID;
        }

        public PageInstance()
        {
            _PageID = 0;
            _TreeNode = new TreeNode();
            _TreeNode.Tag = this;

            PageName = "New Page";
            DisplayInNav = false;
            RequireSSL = false;
            ValidateRequest = true;
            PageDescription = "";
            Guid = Guid.NewGuid();
            _Modules = new List<ModuleInstance>();
            SetupSettings();
        }

        public PageInstance(XmlElement node)
        {
            _PageID = 0;
            _TreeNode = new TreeNode();
            _TreeNode.Tag = this;
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

        public XmlElement Export(XmlDocument doc)
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
            attrib.InnerText = (_TreeNode.Parent == null ? "0" : ((PageInstance)_TreeNode.Parent.Tag).PageID.ToString());
            pageNode.Attributes.Append(attrib);

            return pageNode;
        }

        private String PageSettings()
        {
            StringBuilder sb = new StringBuilder();


            foreach (PageSetting setting in Settings)
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

    class PageSetting
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

    class ModuleInstance
    {
        //
        // Automatic attributes:
        // template_frame_order
        //
        // Unknown attributes:
        // description="", image_path="", mandatory="0", movable="0".
        //
        // For backwards compatibility:
        // temp_page_or_template_id, page_instance
        //
        private int _ModuleInstanceID;
        private String _ModuleTitle;
        private TreeNode _TreeNode;
        private List<ModuleInstanceSetting> _Settings;

        public TreeNode TreeNode { get { return _TreeNode; } }
        public int ModuleInstanceID
        {
            get
            {
                if (_ModuleInstanceID == -1)
                    _ModuleInstanceID = NextAvailableModuleInstanceID(Page);
                
                return _ModuleInstanceID;
            }
        }
        public int TemplateFrameOrder
        {
            get
            {
                int order = 0;

                foreach (TreeNode node in Page.TreeNode.Nodes)
                {
                    if (typeof(ModuleInstance).IsAssignableFrom(node.Tag.GetType()))
                    {
                        if (this == (ModuleInstance)node.Tag)
                            return order;

                        order += 1;
                    }
                }

                return Int32.MaxValue;
            }
        }
        public Boolean ShowTitle { get; set; }
        public String ModuleTitle { get { return _ModuleTitle; } set { _ModuleTitle = value; _TreeNode.Text = value; } }
        public String TemplateFrameName { get; set; }
        public String ModuleDetails { get; set; }
        public PageInstance Page { get; set; }
        public List<ModuleInstanceSetting> Settings { get { return _Settings; } }

        static private int NextAvailableModuleInstanceID(PageInstance parentPage)
        {
            int nextID = -1;


            if (parentPage == null)
                return -1;

            foreach (TreeNode node in parentPage.TreeNode.Nodes)
            {
                int tempID;

                if (typeof(PageInstance).IsAssignableFrom(node.Tag.GetType()))
                {
                    tempID = NextAvailableModuleInstanceID((PageInstance)node.Tag);

                    if (tempID <= nextID)
                        nextID = tempID - 1;
                }
                else if (typeof(ModuleInstance).IsAssignableFrom(node.Tag.GetType()))
                {
                    tempID = ((ModuleInstance)node.Tag).ModuleInstanceID;

                    if (tempID <= nextID)
                        nextID = tempID - 1;
                }
            }

            return nextID;
        }


        public ModuleInstance()
        {
            _ModuleInstanceID = -1;
            ShowTitle = false;
            _ModuleTitle = "New Module";
            TemplateFrameName = "Main";
            ModuleDetails = "";
            Page = null;
            _Settings = new List<ModuleInstanceSetting>();

            _TreeNode = new TreeNode(ModuleTitle);
            _TreeNode.Tag = this;
        }
    }

    class ModuleInstanceSetting
    {
        public String Name;
        public String Value;
        public ModuleInstanceSettingType Type;

        public ModuleInstanceSetting()
        {
            Name = "";
            Value = "";
            Type = ModuleInstanceSettingType.None;
        }

        public ModuleInstanceSetting(String name)
        {
            Name = name;
            Value = "";
            Type = ModuleInstanceSettingType.None;
        }

        public ModuleInstanceSetting(String name, ModuleInstanceSettingType type, String value)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public String SettingString()
        {
            if (String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Value))
                return "";

            return Name + "=" + Value.Replace(";", "^^");
        }
    }

    enum ModuleInstanceSettingType
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
