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
        /// Initialize all the user interface elements on this tab as
        /// well as any events and actions we need to listen for.
        /// </summary>
        private void InitModulesTab()
        {
            //
            // Setup all the tab actions and events.
            //
            tbModuleName.Validated += new EventHandler(tbModuleName_Validated);
            cbModuleSystem.Validated += new EventHandler(cbModuleSystem_Validated);
            tbModuleURL.Validated += new EventHandler(tbModuleURL_Validated);
            tbModuleImagePath.Validated += new EventHandler(tbModuleImagePath_Validated);
            cbModuleAllowsChildModules.Validated += new EventHandler(cbModuleAllowsChildModules_Validated);
            tbModuleSourcePath.Validated += new EventHandler(tbModuleSourcePath_Validated);
            tbModuleSourceImagePath.Validated += new EventHandler(tbModuleSourceImagePath_Validated);
            tbModuleDescription.Validated += new EventHandler(tbModuleDescription_Validated);
            dgModules.SelectionChanged += new EventHandler(dgModules_SelectionChanged);
            dgModules.VirtualMode = true;
            dgModules.CellValueNeeded += new DataGridViewCellValueEventHandler(dgModules_CellValueNeeded);
            dgModules.RowCount = 0;
            dgModules_SelectionChanged(null, null);
        }


        /// <summary>
        /// The package in memory has changed and we need to update
        /// the elements on this tab to reflect the new data.
        /// </summary>
        private void UpdateModulesTab()
        {
            dgModules.RowCount = 0;
            dgModules.RowCount = package.Modules.Count;
        }


        #region Helper Methods

        /// <summary>
        /// Determine the currently selected module in the datagrid. If
        /// there is nothing selected then return null.
        /// </summary>
        /// <returns>A Module reference or null if no module is selected.</returns>
        private Module SelectedModule()
        {
            if (dgModules.SelectedCells.Count == 0)
                return null;

            return package.Modules[dgModules.SelectedCells[0].RowIndex];
        }

        #endregion


        #region Actions and Events for Module Details

        /// <summary>
        /// The module description has changed and been validated, store the
        /// description.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleDescription_Validated(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.Description = tbModuleDescription.Text;
        }


        /// <summary>
        /// The image path for this module has been changed and validated, store
        /// the new value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleSourceImagePath_Validated(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.SourceImage = tbModuleSourceImagePath.Text;
        }


        /// <summary>
        /// The path to the module ascx file on the local disk has been
        /// changed and validated, store the new value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleSourcePath_Validated(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.Source = tbModuleSourcePath.Text;
        }


        /// <summary>
        /// The Allows Child Modules setting has been changed and validated
        /// and can now be stored.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbModuleAllowsChildModules_Validated(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.AllowsChildModules = cbModuleAllowsChildModules.Checked;
        }


        /// <summary>
        /// The Image Path on the Arena server has been changed and validated
        /// and can now be stored in the Module.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleImagePath_Validated(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.ImagePath = tbModuleImagePath.Text;
        }


        /// <summary>
        /// The Arena URL for this module has been changed and validated and
        /// can now be stored.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleURL_Validated(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.URL = tbModuleURL.Text;
        }


        /// <summary>
        /// The name of the module has been changed and validated. Store
        /// the new value and update the datagrid to reflect the new name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbModuleName_Validated(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
            {
                module.Name = tbModuleName.Text;
                dgModules.InvalidateCell(dgModules.CurrentCell.ColumnIndex, dgModules.CurrentCell.RowIndex);
            }
        }


        /// <summary>
        /// The checkbox for if this module is a System Module (pre-existing)
        /// has been changed and validated. Store the new value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbModuleSystem_Validated(object sender, EventArgs e)
        {
            Module module = SelectedModule();


            if (module != null && selectionChanging == false)
                module.IsSystem = cbModuleSystem.Checked;
        }

        #endregion


        #region Actions and Events for the Modules Data Grid


        /// <summary>
        /// The selection in the Modules datagrid has changed. We need to
        /// update the information in the Details pane to reflect the new
        /// selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgModules_SelectionChanged(object sender, EventArgs e)
        {
            Boolean enabled = false;


            selectionChanging = true;

            if (dgModules.SelectedCells.Count > 0)
            {
                Module module = package.Modules[dgModules.SelectedCells[0].RowIndex];

                tbModuleName.Text = module.Name;
                cbModuleSystem.Checked = module.IsSystem;
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
                cbModuleSystem.Checked = false;
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
            cbModuleSystem.Enabled = enabled;
            tbModuleURL.Enabled = enabled;
            tbModuleImagePath.Enabled = enabled;
            cbModuleAllowsChildModules.Enabled = enabled;
            tbModuleSourcePath.Enabled = enabled;
            tbModuleSourceImagePath.Enabled = enabled;
            tbModuleDescription.Enabled = enabled;

            selectionChanging = false;
        }


        /// <summary>
        /// The value for a cell in the modules datagrid is needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgModules_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = package.Modules[e.RowIndex].Name;
        }


        /// <summary>
        /// Add a new module to the list of modules. The module is
        /// added to the datagrid as well as to the package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddModule_Click(object sender, EventArgs e)
        {
            Module module = new Module();


            package.Modules.Add(module);
            dgModules.Rows.Add();
            dgModules.CurrentCell = dgModules.Rows[dgModules.RowCount - 1].Cells[0];
        }


        /// <summary>
        /// User wants to remove the currently selected module from the
        /// list of modules. Verify and then remove.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveModule_Click(object sender, EventArgs e)
        {
            if (dgModules.SelectedCells.Count > 0)
            {
                DialogResult result;

                result = MessageBox.Show("Really delete",
                    "Are you sure you wish to delete the " + package.Modules[dgModules.SelectedCells[0].RowIndex].Name + " module?",
                    MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    package.Modules.RemoveAt(dgModules.SelectedCells[0].RowIndex);
                    dgModules.Rows.RemoveAt(dgModules.SelectedCells[0].RowIndex);
                }
            }
        }

        #endregion
    }
}
