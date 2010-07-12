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

        public mainForm()
        {
            InitializeComponent();
            importFiles(null);

            //
            // Setup all the module tab actions and events.
            //
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

        void dgModules_SelectionChanged(object sender, EventArgs e)
        {
            if (dgModules.SelectedCells.Count > 0)
            {
                btnModulesCancel_Click(null, null);
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
            tbModuleName.Enabled = false;
            tbModuleURL.Enabled = false;
            tbModuleImagePath.Enabled = false;
            cbModuleAllowsChildModules.Enabled = false;
            tbModuleSourcePath.Enabled = false;
            tbModuleSourceImagePath.Enabled = false;
            tbModuleDescription.Enabled = false;
            btnModulesEdit.Enabled = true;
            btnModulesEdit.Text = "Edit";
            btnModulesCancel.Enabled = false;
        }

        private void btnAddModule_Click(object sender, EventArgs e)
        {
            dgModules.Rows.Add();
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["module_name"].Value = "New Module";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["module_url"].Value = "";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["image_path"].Value = "";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["allows_child_modules"].Value = "0";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["_source"].Value = "";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["_source_image"].Value = "";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["module_desc"].Value = "";

            dgModules_SelectionChanged(null, null);
        }

        private void btnRemoveModule_Click(object sender, EventArgs e)
        {
            if (dgModules.SelectedCells.Count > 0)
            {
                dgModules.Rows.RemoveAt(dgModules.SelectedCells[0].RowIndex);
            }
        }

        private void btnModulesCancel_Click(object sender, EventArgs e)
        {
            DataGridViewRow row;


            row = dgModules.Rows[dgModules.SelectedCells[0].RowIndex];
            try
            {
                tbModuleName.Text = row.Cells["module_name"].Value.ToString();
                tbModuleURL.Text = row.Cells["module_url"].Value.ToString();
                tbModuleImagePath.Text = row.Cells["image_path"].Value.ToString();
                cbModuleAllowsChildModules.Checked = (row.Cells["allows_child_modules"].Value.ToString() == "1" ? true : false);
                tbModuleSourcePath.Text = row.Cells["_source"].Value.ToString();
                tbModuleSourceImagePath.Text = row.Cells["_source_image"].Value.ToString();
                tbModuleDescription.Text = row.Cells["module_desc"].Value.ToString();
            }
            catch { }

            tbModuleName.Enabled = false;
            tbModuleURL.Enabled = false;
            tbModuleImagePath.Enabled = false;
            cbModuleAllowsChildModules.Enabled = false;
            tbModuleSourcePath.Enabled = false;
            tbModuleSourceImagePath.Enabled = false;
            tbModuleDescription.Enabled = false;

            btnModulesCancel.Enabled = false;
            btnModulesEdit.Text = "Edit";
        }

        private void btnModulesEdit_Click(object sender, EventArgs e)
        {
            Boolean enabled = false;


            if (btnModulesEdit.Text == "Edit")
            {
                //
                // Edit...
                //
                btnModulesCancel.Enabled = true;
                btnModulesEdit.Text = "Save";
                enabled = true;
            }
            else
            {
                DataGridViewRow row;

                //
                // Save...
                //
                row = dgModules.Rows[dgModules.SelectedCells[0].RowIndex];
                row.Cells["module_name"].Value = tbModuleName.Text;
                row.Cells["module_url"].Value = tbModuleURL.Text;
                row.Cells["image_path"].Value = tbModuleImagePath.Text;
                row.Cells["allows_child_modules"].Value = (cbModuleAllowsChildModules.Checked ? "1" : "0");
                row.Cells["_source"].Value = tbModuleSourcePath.Text;
                row.Cells["_source_image"].Value = tbModuleSourceImagePath.Text;
                row.Cells["module_desc"].Value = tbModuleDescription.Text;

                btnModulesCancel.Enabled = false;
                btnModulesEdit.Text = "Save";
            }

            tbModuleName.Enabled = enabled;
            tbModuleURL.Enabled = enabled;
            tbModuleImagePath.Enabled = enabled;
            cbModuleAllowsChildModules.Enabled = enabled;
            tbModuleSourcePath.Enabled = enabled;
            tbModuleSourceImagePath.Enabled = enabled;
            tbModuleDescription.Enabled = enabled;
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
        }

        void tvPages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvPages.SelectedNode == null)
                return;

            selectionChanging = true;
            dgPageSettings.RowCount = 13;

            if (typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == true)
            {
                PageInstance page = (PageInstance)tvPages.SelectedNode.Tag;

                tbPageName.Text = page.PageName;
                cbPageDisplayInNav.Checked = page.DisplayInNav;
                cbPageRequireSSL.Checked = page.RequireSSL;
                cbPageValidateRequest.Checked = page.ValidateRequest;
                tbPageDescription.Text = page.PageDescription;

                tcPages.SelectedIndex = 0;
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

    class PageInstance
    {
        private int _PageID;
        private TreeNode _TreeNode;
        private String _PageName;
        private List<PageSetting> _Settings;

        public int PageID { get { if (_PageID == 0) _PageID = NextAvailablePageID((PageInstance)_TreeNode.TreeView.Nodes[0].Tag); return _PageID; } }
        public TreeNode TreeNode { get { return _TreeNode; } }
        public String PageName { get { return _PageName; } set { _PageName = value; _TreeNode.Text = value; } }
        public Boolean DisplayInNav;
        public Boolean RequireSSL;
        public Boolean ValidateRequest;
        public String PageDescription;
        public List<PageSetting> Settings { get { return _Settings; } }
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
                    NextAvailablePageID((PageInstance)node.Tag);

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
}
