using RefreshCache.Packager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RefreshCache.Packager.Builder
{
    public partial class mainForm : Form
    {
        /// <summary>
        /// Intialize all the actions and event handlers for all the
        /// user elements on the Pages tab.
        /// </summary>
        private void InitPagesTab()
        {
            //
            // Setup all the tab actions and events at the top level.
            //
            tvPages.BeforeSelect += new TreeViewCancelEventHandler(tvPages_BeforeSelect);
            tvPages.AfterSelect += new TreeViewEventHandler(tvPages_AfterSelect);

            //
            // Setup all the actions and events for page details.
            //
            tbPageName.TextChanged += new EventHandler(tbPageName_Validated);
            cbPageDisplayInNav.Validated += new EventHandler(cbPageDisplayInNav_Validated);
            cbPageRequireSSL.Validated += new EventHandler(cbPageRequireSSL_Validated);
            cbPageValidateRequest.Validated += new EventHandler(cbPageValidateRequest_Validated);
            tbPageDescription.Validated += new EventHandler(tbPageDescription_Validated);
            tbPageGuid.Validated += new EventHandler(tbPageGuid_Validated);
            dgPageSettings.VirtualMode = true;
            dgPageSettings.CellValueNeeded += new DataGridViewCellValueEventHandler(dgPageSettings_CellValueNeeded);
            dgPageSettings.CellValuePushed += new DataGridViewCellValueEventHandler(dgPageSettings_CellValuePushed);
            dgPageSettings.RowCount = 0;

            //
            // Setup all the actions and events for module details.
            //
            tbModuleInstanceTitle.Validated += new EventHandler(tbModuleInstanceTitle_Validated);
            tbModuleInstanceTemplateFrameName.Validated += new EventHandler(tbModuleInstanceTemplateFrameName_Validated);
            tbModuleInstanceDetails.Validated += new EventHandler(tbModuleInstanceDetails_Validated);
            cbModuleInstanceShowTitle.Validated += new EventHandler(cbModuleInstanceShowTitle_Validated);
            cbModuleInstanceType.SelectedValueChanged += new EventHandler(cbModuleInstanceType_SelectedValueChanged);
            tbModuleInstanceGuid.Validating += new CancelEventHandler(tbModuleInstanceGuid_Validating);
            tbModuleInstanceGuid.Validated += new EventHandler(tbModuleInstanceGuid_Validated);
            DataGridViewComboBoxColumn box = (DataGridViewComboBoxColumn)dgModuleInstanceSettings.Columns["Type"];
            box.Items.AddRange(Enum.GetNames(typeof(ModuleInstanceSettingType)));
            box.Sorted = true;
            dgModuleInstanceSettings.VirtualMode = true;
            dgModuleInstanceSettings.CellValueNeeded += new DataGridViewCellValueEventHandler(dgModuleInstanceSettings_CellValueNeeded);
            dgModuleInstanceSettings.CellValuePushed += new DataGridViewCellValueEventHandler(dgModuleInstanceSettings_CellValuePushed);
            dgModuleInstanceSettings.UserDeletingRow += new DataGridViewRowCancelEventHandler(dgModuleInstanceSettings_UserDeletingRow);
            dgModuleInstanceSettings.RowCount = 1;
        }


        /// <summary>
        /// The loaded package has been changed. Update all the user
        /// interface values on the Pages tab.
        /// </summary>
        private void UpdatePagesTab()
        {
            tvPages.Nodes.Clear();
            foreach (PageInstance page in package.Pages)
            {
                tvPages.Nodes.Add(TreeNodeFromPageInstance(page));
            }
        }


        #region Helper Methods

        /// <summary>
        /// Determine the currently selected page instance in the treeview.
        /// </summary>
        /// <returns>A PageInstance reference or null if no page is selected.</returns>
        private PageInstance SelectedPageInstance()
        {
            if (tvPages.SelectedNode == null ||
                typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == false)
            {
                return null;
            }

            return (PageInstance)tvPages.SelectedNode.Tag;
        }


        /// <summary>
        /// Determine the currently selected ModuleInstance if there is one
        /// selected.
        /// </summary>
        /// <returns>A ModuleInstance reference or null if no module is selected.</returns>
        private ModuleInstance SelectedModuleInstance()
        {
            if (tvPages.SelectedNode == null ||
                typeof(ModuleInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == false)
            {
                return null;
            }

            return (ModuleInstance)tvPages.SelectedNode.Tag;
        }


        /// <summary>
        /// Create a new TreeNode object for the specified PageInstance
        /// and prepare it for use.
        /// </summary>
        /// <param name="parentPage">The page that will be represented by the TreeNode.</param>
        /// <returns>A new TreeNode instance that can be added to a TreeView.</returns>
        private TreeNode TreeNodeFromPageInstance(PageInstance parentPage)
        {
            TreeNode node = new TreeNode(parentPage.PageName);


            node.Tag = parentPage;

            //
            // Create a child TreeNode for each module instance on
            // this page.
            //
            foreach (ModuleInstance module in parentPage.Modules)
            {
                TreeNode moduleNode = new TreeNode(module.ModuleTitle);

                moduleNode.Tag = module;
                node.Nodes.Add(moduleNode);
            }

            //
            // Create a child TreeNode for each child page on this
            // page.
            //
            foreach (PageInstance page in parentPage.Pages)
            {
                TreeNode pageNode = TreeNodeFromPageInstance(page);

                node.Nodes.Add(pageNode);
            }

            return node;
        }

        #endregion


        #region Pages Tab User Interface

        /// <summary>
        /// Add a new page to the package. If there is no existing pages
        /// then create a root page, otherwise create a child page of the
        /// current page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPagesAddPage_Click(object sender, EventArgs e)
        {
            TreeNode node = new TreeNode();
            PageInstance page, parentPage;


            if (tvPages.SelectedNode == null)
            {
                //
                // Create a new root page.
                //
                page = new PageInstance();
                package.Pages.Add(page);
                tvPages.Nodes.Add(node);
            }
            else
            {
                //
                // Create a child page of the currently selected page.
                //
                parentPage = SelectedPageInstance();
                if (parentPage == null)
                    return;

                page = new PageInstance();
                parentPage.Pages.Add(page);
                tvPages.SelectedNode.Nodes.Add(node);
            }

            //
            // Set some default values for this page and the node.
            //
            node.Text = page.PageName;
            node.Tag = page;
            tvPages.SelectedNode = node;
        }


        /// <summary>
        /// Add a new module to the currently selected page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPagesAddModule_Click(object sender, EventArgs e)
        {
            TreeNode node;
            ModuleInstance module;
            PageInstance parentPage = SelectedPageInstance();


            //
            // If there is no selected page, ignore this request.
            //
            if (parentPage == null)
                return;

            //
            // Create the new tree node and the module, then add the
            // module to the page.
            //
            node = new TreeNode();
            module = new ModuleInstance();
            parentPage.Modules.Add(module);

            //
            // Set some values and select the new node.
            //
            node.Text = module.ModuleTitle;
            node.Tag = module;
            tvPages.SelectedNode.Nodes.Insert(parentPage.Modules.Count - 1, node);
            tvPages.SelectedNode = node;
        }

        #endregion


        #region Page Details User Interface

        /// <summary>
        /// The GUID for the page has been validated and can be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbPageGuid_Validated(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.Guid = tbPageGuid.Text;
        }


        /// <summary>
        /// The page description has been changed and validated and can
        /// now be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbPageDescription_Validated(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.PageDescription = tbPageDescription.Text;
        }


        /// <summary>
        /// The Validate Request checkbox has been modified and validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbPageValidateRequest_Validated(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.ValidateRequest = cbPageValidateRequest.Checked;
        }


        /// <summary>
        /// The Require SSL check box has been changed and validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbPageRequireSSL_Validated(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.RequireSSL = cbPageRequireSSL.Checked;
        }


        /// <summary>
        /// The Display In Navigation checkbox has been modified and validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbPageDisplayInNav_Validated(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
                page.DisplayInNav = cbPageDisplayInNav.Checked;
        }


        /// <summary>
        /// User has modified the name of the page and the change has been
        /// validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbPageName_Validated(object sender, EventArgs e)
        {
            PageInstance page = SelectedPageInstance();


            if (page != null && selectionChanging == false)
            {
                page.PageName = tbPageName.Text;
                tvPages.SelectedNode.Text = page.PageName;
            }
        }


        /// <summary>
        /// The selection in the Pages treeview is about to change. Clear
        /// out some of the details data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tvPages_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            dgPageSettings.RowCount = 0;
            dgModuleInstanceSettings.RowCount = 1;
        }


        /// <summary>
        /// The selection in the Pages treeview has finished changing. Update
        /// all the details information for this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tvPages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvPages.SelectedNode == null)
                return;

            selectionChanging = true;

            if (typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == true)
            {
                PageInstance page = (PageInstance)tvPages.SelectedNode.Tag;

                //
                // User has selected a page, switch to the appropriate tabcontrol
                // page and set the details values.
                //
                tbPageName.Text = page.PageName;
                lbPageID.Text = "Page ID: " + page.PageID;
                cbPageDisplayInNav.Checked = page.DisplayInNav;
                cbPageRequireSSL.Checked = page.RequireSSL;
                cbPageValidateRequest.Checked = page.ValidateRequest;
                tbPageDescription.Text = page.PageDescription;
                tbPageGuid.Text = page.Guid;
                dgPageSettings.RowCount = 13;

                tcPages.SelectedIndex = 0;
            }
            else if (typeof(ModuleInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == true)
            {
                ModuleInstance module = (ModuleInstance)tvPages.SelectedNode.Tag;

                //
                // User has selected a module, switch to the appropraite tabcontorl
                // page and set the details values.
                //
                tbModuleInstanceTitle.Text = module.ModuleTitle;
                cbModuleInstanceShowTitle.Checked = module.ShowTitle;
                tbModuleInstanceTemplateFrameName.Text = module.TemplateFrameName;
                cbModuleInstanceType.SelectedValue = module.ModuleTypeID;
                tbModuleInstanceDetails.Text = module.ModuleDetails;
                tbModuleInstanceGuid.Text = module.Guid.ToString();
                dgModuleInstanceSettings.RowCount = module.Settings.Count + 1;

                tcPages.SelectedIndex = 1;
            }

            selectionChanging = false;
        }


        /// <summary>
        /// A page setting value has been changed and needs to be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgPageSettings_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            PageInstance page = (PageInstance)tvPages.SelectedNode.Tag;


            page.Settings[e.RowIndex].Value = e.Value.ToString();
        }


        /// <summary>
        /// A page setting value is needed by the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgPageSettings_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (tvPages.SelectedNode == null ||
                typeof(PageInstance).IsAssignableFrom(tvPages.SelectedNode.Tag.GetType()) == false)
            {
                //
                // The current selection is empty or a module, this is mostly
                // a placeholder to prevent crashes.
                //
                e.Value = "";
            }
            else
            {
                PageInstance page = (PageInstance)tvPages.SelectedNode.Tag;

                //
                // Current selection is a page, retrieve the value.
                //
                if (e.ColumnIndex == 0)
                    e.Value = page.Settings[e.RowIndex].DisplayName;
                else
                    e.Value = page.Settings[e.RowIndex].Value;
            }
        }

        #endregion


        #region Module Details User Interface

        /// <summary>
        /// The user is deleting a module instance setting, remove the
        /// module setting from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgModuleInstanceSettings_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            ModuleInstance module = SelectedModuleInstance();


            if (module != null)
            {
                module.Settings.RemoveAt(e.Row.Index);
            }
        }


        /// <summary>
        /// The user has set a value in the Module Instance Settings, store
        /// the value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgModuleInstanceSettings_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            ModuleInstance module = SelectedModuleInstance();


            if (module == null)
                return;

            //
            // Create a new module instance setting if we need to.
            //
            if (e.RowIndex == module.Settings.Count)
            {
                module.Settings.Add(new ModuleInstanceSetting());
                dgModuleInstanceSettings.UpdateCellValue(1, e.RowIndex);
            }

            //
            // Store the value in the appropriate place.
            //
            if (e.ColumnIndex == 0)
                module.Settings[e.RowIndex].Name = e.Value.ToString();
            else if (e.ColumnIndex == 1)
                module.Settings[e.RowIndex].Type = (ModuleInstanceSettingType)Enum.Parse(typeof(ModuleInstanceSettingType), e.Value.ToString());
            else if (e.ColumnIndex == 2)
                module.Settings[e.RowIndex].Value = e.Value.ToString();
            else if (e.ColumnIndex == 3)
                module.Settings[e.RowIndex].Guid = e.Value.ToString();
        }


        /// <summary>
        /// A value is about to be displayed in the module instance settings
        /// datagrid, retrieve the appropriate value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// User has changed the selection in the module instance type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbModuleInstanceType_SelectedValueChanged(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.ModuleTypeID = Convert.ToInt32(cbModuleInstanceType.SelectedValue);
            }
        }


        /// <summary>
        /// User has made a change to the Show Title option and it has
        /// been validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbModuleInstanceShowTitle_Validated(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.ShowTitle = cbModuleInstanceShowTitle.Checked;
            }
        }


        /// <summary>
        /// The Details of a module instance has been changed and validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleInstanceDetails_Validated(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.ModuleDetails = tbModuleInstanceDetails.Text;
            }
        }


        /// <summary>
        /// The Template Frame Name of a module instance has been changed
        /// and validated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleInstanceTemplateFrameName_Validated(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.TemplateFrameName = tbModuleInstanceTemplateFrameName.Text;
            }
        }


        /// <summary>
        /// The Title of a module instance has been changed and validated.
        /// Update the node title and store the value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleInstanceTitle_Validated(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                instance.ModuleTitle = tbModuleInstanceTitle.Text;
                tvPages.SelectedNode.Text = instance.ModuleTitle;
            }
        }


        /// <summary>
        /// The user has entered a value for the module instance GUID and
        /// we need to validate that it is a valid GUID value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleInstanceGuid_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (tbModuleInstanceGuid.Text.Length > 0)
                    new System.Guid(tbModuleInstanceGuid.Text);
                errorProvider1.SetError(tbModuleInstanceGuid, "");
            }
            catch
            {
                e.Cancel = true;
                errorProvider1.SetError(tbModuleInstanceGuid, "Invalid GUID specified.");
            }
        }

        /// <summary>
        /// The GUID for the module instance has been validated and can
        /// be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleInstanceGuid_Validated(object sender, EventArgs e)
        {
            ModuleInstance instance = SelectedModuleInstance();


            if (instance != null && selectionChanging == false)
            {
                if (tbModuleInstanceGuid.Text.Length == 0)
                    instance.Guid = System.Guid.NewGuid();
                else
                    instance.Guid = new System.Guid(tbModuleInstanceGuid.Text);
            }
        }


        #endregion
    }
}
