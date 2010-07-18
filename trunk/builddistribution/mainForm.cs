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
        private String currentFileName = null;
        private Boolean selectionChanging = false;

        public mainForm()
        {
            InitializeComponent();

            //
            // Initialize everything to empty.
            //
            Module.Modules = new List<Module>();

            //
            // Setup all the top-level actions and events.
            //
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
            dgModules.VirtualMode = true;
            dgModules.CellValueNeeded += new DataGridViewCellValueEventHandler(dgModules_CellValueNeeded);
            dgModules.RowCount = 0;
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
            tbModuleInstanceTitle.TextChanged += new EventHandler(tbModuleInstanceTitle_TextChanged);
            tbModuleInstanceTemplateFrameName.TextChanged += new EventHandler(tbModuleInstanceTemplateFrameName_TextChanged);
            tbModuleInstanceDetails.TextChanged += new EventHandler(tbModuleInstanceDetails_TextChanged);
            cbModuleInstanceShowTitle.CheckedChanged += new EventHandler(cbModuleInstanceShowTitle_CheckedChanged);
            cbModuleInstanceType.SelectedValueChanged += new EventHandler(cbModuleInstanceType_SelectedValueChanged);
            DataGridViewComboBoxColumn box = (DataGridViewComboBoxColumn)dgModuleInstanceSettings.Columns["Type"];
            box.Items.AddRange(Enum.GetNames(typeof(ModuleInstanceSettingType)));
            dgModuleInstanceSettings.VirtualMode = true;
            dgModuleInstanceSettings.CellValueNeeded += new DataGridViewCellValueEventHandler(dgModuleInstanceSettings_CellValueNeeded);
            dgModuleInstanceSettings.CellValuePushed += new DataGridViewCellValueEventHandler(dgModuleInstanceSettings_CellValuePushed);
            dgModuleInstanceSettings.UserDeletingRow += new DataGridViewRowCancelEventHandler(dgModuleInstanceSettings_UserDeletingRow);
            dgModuleInstanceSettings.RowCount = 1;
        }

        #region Helper methods

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

            return Module.Modules[dgModules.SelectedCells[0].RowIndex];
        }

        //
        // Work around idiot framework trying to make the XML utf-16 instead
        // of what I tell it to be.
        //
        public static String XmlDocumentToString(XmlDocument doc)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(false);
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlWriterSettings.Indent = true;

            XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
            doc.Save(xmlWriter);
            xmlWriter.Flush();
            xmlWriter.Close();

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        #endregion

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
                dgModules.InvalidateCell(dgModules.CurrentCell.ColumnIndex, dgModules.CurrentCell.RowIndex);
            }
        }

        void dgModules_SelectionChanged(object sender, EventArgs e)
        {
            Boolean enabled = false;


            selectionChanging = true;

            if (dgModules.SelectedCells.Count > 0)
            {
                Module module = Module.Modules[dgModules.SelectedCells[0].RowIndex];

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

        void dgModules_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = Module.Modules[e.RowIndex].Name;
        }

        private void btnAddModule_Click(object sender, EventArgs e)
        {
            Module module = new Module();


            Module.Modules.Add(module);
            dgModules.Rows.Add();
            dgModules.CurrentCell = dgModules.Rows[dgModules.RowCount - 1].Cells[0];
        }

        private void btnRemoveModule_Click(object sender, EventArgs e)
        {
            if (dgModules.SelectedCells.Count > 0)
            {
                Module.Modules.RemoveAt(dgModules.SelectedCells[0].RowIndex);
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
                cbModuleInstanceType.SelectedValue = module.ModuleTypeID;
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

        void cbModuleInstanceType_SelectedValueChanged(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.ModuleTypeID = Convert.ToInt32(cbModuleInstanceType.SelectedValue);
            }
        }

        void cbModuleInstanceShowTitle_CheckedChanged(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.ShowTitle = cbModuleInstanceShowTitle.Checked;
            }
        }

        void tbModuleInstanceDetails_TextChanged(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.ModuleDetails = tbModuleInstanceDetails.Text;
            }
        }

        void tbModuleInstanceTemplateFrameName_TextChanged(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.TemplateFrameName = tbModuleInstanceTemplateFrameName.Text;
            }
        }

        void tbModuleInstanceTitle_TextChanged(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.ModuleTitle = tbModuleInstanceTitle.Text;
            }
        }

        #endregion

        #region Top Level User Interface

        void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedValue;

            selectedValue = cbModuleInstanceType.SelectedValue;
            cbModuleInstanceType.DataSource = Module.Modules.ToArray();
            if (selectedValue != null)
                cbModuleInstanceType.SelectedValue = selectedValue;
        }

        private void newMenu_Click(object sender, EventArgs e)
        {
            currentFileName = null;

            importFiles(null);
            importModules(null);
            importPages(null);
        }

        private void openMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();


            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter = "XML Files|*.xml";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                openFromFile(dialog.FileName);
            }
        }

        private void saveMenu_Click(object sender, EventArgs e)
        {
            if (currentFileName == null)
                saveAsMenu_Click(null, null);
            else
                saveToFile(currentFileName);
        }

        private void saveAsMenu_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();


            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter = "XML Files|*.xml";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                saveToFile(dialog.FileName);
            }
        }

        private void exitMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Import/Export methods

        private void importFiles(XmlDocument doc)
        {
            dgFiles.RowCount = 0;

            if (doc != null)
            {
                XmlNode filesNode = doc.SelectSingleNode("//ArenaPackage/Files");

                foreach (XmlNode node in filesNode.ChildNodes)
                {
                    dgFiles.Rows.Add(node.Attributes["path"].Value, node.Attributes["_source"].Value);
                }
            }
        }

        private void importModules(XmlDocument doc)
        {
            Module.Modules.Clear();
            dgModules.RowCount = 0;

            if (doc != null)
            {
                XmlNode modulesNode = doc.SelectSingleNode("//ArenaPackage/Modules");

                foreach (XmlNode node in modulesNode.ChildNodes)
                {
                    Module module = new Module(node);

                    Module.Modules.Add(module);
                    dgModules.Rows.Add();
                }
            }
        }

        private void importPages(XmlDocument doc)
        {
            tvPages.Nodes.Clear();

            if (doc != null)
            {
                XmlNode pagesNode = doc.SelectSingleNode("//ArenaPackage/Pages");

                if (pagesNode.ChildNodes.Count > 0)
                {
                    PageInstance page = new PageInstance(pagesNode.ChildNodes[0]);

                    tvPages.Nodes.Add(page.TreeNode);
                }
            }
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
            XmlNode nodeModules;


            //
            // Process standalone files.
            //
            nodeModules = doc.CreateElement("Modules");
            foreach (Module module in Module.Modules)
            {
                nodeModules.AppendChild(module.Export(doc));
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

        private void BuildPageNode(XmlDocument doc, XmlNode pagesNode, PageInstance page)
        {
            XmlNode pageNode = page.Export(doc);

            
            pagesNode.AppendChild(pageNode);

            foreach (TreeNode node in page.TreeNode.Nodes)
            {
                if (typeof(PageInstance).IsAssignableFrom(node.Tag.GetType()))
                {
                    BuildPageNode(doc, pagesNode, (PageInstance)node.Tag);
                }
            }
        }

        private void openFromFile(String filename)
        {

            StreamReader reader = new StreamReader(filename);
            XmlDocument doc = new XmlDocument();


            doc.Load(reader);
            reader.Close();

            importFiles(doc);
            importModules(doc);
            importPages(doc);

            currentFileName = filename;
        }

        private void saveToFile(String filename)
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
            StreamWriter writer = new StreamWriter(filename);
            writer.Write(XmlDocumentToString(doc));
            writer.Close();

            currentFileName = filename;
        }

        #endregion

    }

    class Module
    {
        static public List<Module> Modules { get; set; }

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

        static int NextAvailableModuleID()
        {
            int nextID = -1;


            foreach (Module m in Modules)
            {
                if (m._ModuleID <= nextID)
                    nextID = m._ModuleID - 1;
            }

            return nextID;
        }

        public XmlElement Export(XmlDocument doc)
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

    class PageInstance
    {
        private int _PageID;
        private TreeNode _TreeNode;
        private String _PageName;
        private List<PageSetting> _Settings;

        public PageInstance ParentPage
        {
            get
            {
                if (_TreeNode.Parent == null)
                    return null;

                return (PageInstance)_TreeNode.Parent.Tag;
            }
        }
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
                    tempID = NextAvailablePageID((PageInstance)node.Tag);

                    if (tempID <= nextID)
                        nextID = tempID - 1;
                }
            }

            return nextID;
        }

        public PageInstance()
        {
            _TreeNode = new TreeNode();
            _TreeNode.Tag = this;

            _PageID = 0;
            PageName = "New Page";
            DisplayInNav = false;
            RequireSSL = false;
            ValidateRequest = true;
            PageDescription = "";
            Guid = Guid.NewGuid();
            SetupSettings();
        }

        public PageInstance(XmlNode node)
        {
            _TreeNode = new TreeNode();
            _TreeNode.Tag = this;

            _PageID = Convert.ToInt32(node.Attributes["temp_page_id"].Value);
            PageName = node.Attributes["page_name"].Value;
            DisplayInNav = (node.Attributes["display_in_nav"].Value == "1" ? true : false);
            RequireSSL = (node.Attributes["require_ssl"].Value == "1" ? true : false);
            ValidateRequest = (node.Attributes["validate_request"].Value == "1" ? true : false);
            PageDescription = node.Attributes["page_desc"].Value;
            Guid = new Guid(node.Attributes["guid"].Value);
            SetupSettings(node.Attributes["page_settings"].Value.Split(new char[] { ';' }));

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "ModuleInstance")
                {
                    ModuleInstance module = new ModuleInstance(child);

                    _TreeNode.Nodes.Add(module.TreeNode);
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

            //
            // Add all module instances.
            //
            foreach (TreeNode node in _TreeNode.Nodes)
            {
                if (typeof(ModuleInstance).IsAssignableFrom(node.Tag.GetType()))
                {
                    pageNode.AppendChild(((ModuleInstance)node.Tag).Export(doc));
                }
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
        private int _ModuleInstanceID;
        private String _ModuleTitle;
        private TreeNode _TreeNode;
        private List<ModuleInstanceSetting> _Settings;

        public TreeNode TreeNode { get { return _TreeNode; } }
        public int ModuleInstanceID
        {
            get
            {
                if (_ModuleInstanceID == 0)
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
        public Int32 ModuleTypeID { get; set; }
        public PageInstance Page { get { return (PageInstance)_TreeNode.Parent.Tag; } }
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
                        nextID = tempID;
                }
                else if (typeof(ModuleInstance).IsAssignableFrom(node.Tag.GetType()))
                {
                    tempID = ((ModuleInstance)node.Tag)._ModuleInstanceID;

                    if (tempID <= nextID)
                        nextID = tempID - 1;
                }
            }

            return nextID;
        }

        public ModuleInstance()
        {
            _TreeNode = new TreeNode();
            _TreeNode.Tag = this;

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
            _TreeNode = new TreeNode();
            _TreeNode.Tag = this;

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

        public XmlElement Export(XmlDocument doc)
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
                instNode.AppendChild(setting.Export(doc));
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

    class ModuleInstanceSetting
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

        public XmlElement Export(XmlDocument doc)
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
