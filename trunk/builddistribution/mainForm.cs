using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Arena.Custom.RC.Packager
{
    public partial class mainForm : Form
    {
        private Package package;
        private String currentFileName = null;
        private Boolean selectionChanging = false;

        public mainForm()
        {
            InitializeComponent();

            //
            // Initialize everything to empty.
            //
            package = new Package();

            //
            // Setup all the top-level actions and events.
            //
            tcMain.SelectedIndexChanged += new EventHandler(tcMain_SelectedIndexChanged);

            //
            // Setup the Package tab actions and events.
            //
            tbPackageReadme.TextChanged += new EventHandler(tbPackageReadme_TextChanged);
            //
            // Setup all the file tab actions and events.
            //
            dgFiles.VirtualMode = true;
            dgFiles.CellValueNeeded += new DataGridViewCellValueEventHandler(dgFiles_CellValueNeeded);
            dgFiles.CellValuePushed += new DataGridViewCellValueEventHandler(dgFiles_CellValuePushed);

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
            box.Sorted = true;
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

            return package.Modules[dgModules.SelectedCells[0].RowIndex];
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

        private TreeNode TreeNodeFromPageInstance(PageInstance parentPage)
        {
            TreeNode node = new TreeNode(parentPage.PageName);


            node.Tag = parentPage;

            foreach (ModuleInstance module in parentPage.Modules)
            {
                TreeNode moduleNode = new TreeNode(module.ModuleTitle);

                moduleNode.Tag = module;
                node.Nodes.Add(moduleNode);
            }

            foreach (PageInstance page in parentPage.Pages)
            {
                TreeNode pageNode = TreeNodeFromPageInstance(page);

                node.Nodes.Add(pageNode);
            }

            return node;
        }

        #endregion

        #region Package Tab User Interface

        void tbPackageReadme_TextChanged(object sender, EventArgs e)
        {
            package.Readme = tbPackageReadme.Text;
        }

        #endregion

        #region Files Tab User Interface

        void dgFiles_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            File file = package.Files[e.RowIndex];


            if (e.ColumnIndex == 0)
                e.Value = file.Path;
            else if (e.ColumnIndex == 1)
                e.Value = file.Source;
        }

        void dgFiles_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            File file = package.Files[e.RowIndex];


            if (e.ColumnIndex == 0)
                file.Path = e.Value.ToString();
            else if (e.ColumnIndex == 1)
                file.Source = e.Value.ToString();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            package.Files.Add(new File());
            dgFiles.Rows.Add();
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (dgFiles.CurrentCell != null)
            {
                package.Files.RemoveAt(dgFiles.CurrentCell.RowIndex);
                dgFiles.Rows.RemoveAt(dgFiles.CurrentCell.RowIndex);
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
                Module module = package.Modules[dgModules.SelectedCells[0].RowIndex];

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
            e.Value = package.Modules[e.RowIndex].Name;
        }

        private void btnAddModule_Click(object sender, EventArgs e)
        {
            Module module = new Module();


            package.Modules.Add(module);
            dgModules.Rows.Add();
            dgModules.CurrentCell = dgModules.Rows[dgModules.RowCount - 1].Cells[0];
        }

        private void btnRemoveModule_Click(object sender, EventArgs e)
        {
            if (dgModules.SelectedCells.Count > 0)
            {
                package.Modules.RemoveAt(dgModules.SelectedCells[0].RowIndex);
                dgModules.Rows.RemoveAt(dgModules.SelectedCells[0].RowIndex);
            }
        }

        #endregion

        #region Pages Tab User Interface

        private void btnPagesAddPage_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            PageInstance page, parentPage;


            if (tvPages.SelectedNode == null)
            {
                page = new PageInstance();
                package.Pages.Add(page);
                tvPages.Nodes.Add(node);
            }
            else
            {
                parentPage = SelectedPageInstance();
                if (parentPage == null)
                    return;

                page = new PageInstance();
                parentPage.Pages.Add(page);
                tvPages.SelectedNode.Nodes.Add(node);
            }

            node.Text = page.PageName;
            node.Tag = page;
            tvPages.SelectedNode = node;
        }

        private void btnPagesAddModule_Click(object sender, EventArgs e)
        {
            TreeNode node;
            ModuleInstance module;
            PageInstance parentPage = SelectedPageInstance();


            if (parentPage == null)
                return;

            node = new TreeNode();
            module = new ModuleInstance();
            parentPage.Modules.Add(module);

            node.Text = module.ModuleTitle;
            node.Tag = module;
            tvPages.SelectedNode.Nodes.Insert(parentPage.Modules.Count - 1, node);
            tvPages.SelectedNode = node;
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
            {
                page.PageName = tbPageName.Text;
                tvPages.SelectedNode.Text = page.PageName;
            }
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
            else if (e.ColumnIndex == 3)
                module.Settings[e.RowIndex].Guid = e.Value.ToString();
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
            else if (e.ColumnIndex == 3)
                e.Value = module.Settings[e.RowIndex].Guid;
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
                tvPages.SelectedNode.Text = instance.ModuleTitle;
            }
        }

        #endregion

        #region Top Level User Interface

        void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedValue;

            selectedValue = cbModuleInstanceType.SelectedValue;
            cbModuleInstanceType.DataSource = package.Modules;
            if (selectedValue != null)
                cbModuleInstanceType.SelectedValue = selectedValue;
        }

        private void newMenu_Click(object sender, EventArgs e)
        {
            currentFileName = null;

            tbPackageReadme.Text = "";
            dgFiles.RowCount = 0;
            dgModules.RowCount = 0;
            tvPages.Nodes.Clear();

            package = new Package();
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

        private void buildMenu_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();


            dialog.InitialDirectory = Environment.CurrentDirectory;
            dialog.Filter = "Arena Page Export|*.xml";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                BuildMessageCollection messages;


                //
                // Save the package to XML.
                //
                messages = package.Build();

                //
                // Check if there were any errors during the build.
                if (package.XmlPackage == null)
                {
                    MessageBox.Show(messages.ToString(), "Errors during build",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                if (messages.Count > 0)
                {
                    DialogResult result;

                    result = MessageBox.Show(messages.ToString(), "Continue with export?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                        return;
                }

                //
                // Dump result.
                //
                StreamWriter writer = new StreamWriter(dialog.FileName);
                writer.Write(XmlDocumentToString(package.XmlPackage));
                writer.Close();
            }
        }

        private void exitMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Import/Export methods

        private void openFromFile(String filename)
        {
            StreamReader reader = new StreamReader(filename);
            XmlDocument doc = new XmlDocument();


            doc.Load(reader);
            reader.Close();

            dgFiles.RowCount = 0;
            dgModules.RowCount = 0;
            tvPages.Nodes.Clear();

            package = new Package(doc);

            tbPackageReadme.Text = package.Readme;
            dgFiles.RowCount = package.Files.Count;
            dgModules.RowCount = package.Modules.Count;
            foreach (PageInstance page in package.Pages)
            {
                tvPages.Nodes.Add(TreeNodeFromPageInstance(page));
            }

            currentFileName = filename;
        }

        private void saveToFile(String filename)
        {
            //
            // Save the package to XML.
            //
            XmlDocument doc = package.Save();

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
}
