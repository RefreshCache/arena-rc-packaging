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
        private void InitRequirementsTab()
        {
            //
            // Setup the tab actions and events.
            //
            tbRequirements_ArenaMinVer.Validating += new CancelEventHandler(tbRequirements_ArenaMinVer_Validating);
            tbRequirements_ArenaMinVer.Validated += new EventHandler(tbRequirements_ArenaMinVer_Validated);
            tbRequirements_ArenaMaxVer.Validating += new CancelEventHandler(tbRequirements_ArenaMaxVer_Validating);
            tbRequirements_ArenaMaxVer.Validated += new EventHandler(tbRequirements_ArenaMaxVer_Validated);
            tbRequirements_ArenaExactVer.Validating += new CancelEventHandler(tbRequirements_ArenaExactVer_Validating);
            tbRequirements_ArenaExactVer.Validated += new EventHandler(tbRequirements_ArenaExactVer_Validated);
            dgRequirements_Required.CellValidating += new DataGridViewCellValidatingEventHandler(dgRequirements_Required_CellValidating);

            dgRequirements_Required.VirtualMode = true;
            dgRequirements_Required.CellValueNeeded += new DataGridViewCellValueEventHandler(dgRequirements_Required_CellValueNeeded);
            dgRequirements_Required.CellValuePushed += new DataGridViewCellValueEventHandler(dgRequirements_Required_CellValuePushed);
            dgRequirements_Required.UserDeletedRow += new DataGridViewRowEventHandler(dgRequirements_Required_UserDeletedRow);
        }


        #region Arena Version Event Handlers

        void tbRequirements_ArenaMinVer_Validating(object sender, CancelEventArgs e)
        {
            PackageVersion version;

            try
            {
                version = new PackageVersion(tbRequirements_ArenaMinVer.Text);
                this.errorProvider1.SetError(tbRequirements_ArenaMinVer, "");
            }
            catch
            {
                e.Cancel = true;
                this.errorProvider1.SetError(tbRequirements_ArenaMinVer, "Invalid version number");
            }
        }

        void tbRequirements_ArenaMinVer_Validated(object sender, EventArgs e)
        {
            if (package.Info.Arena == null)
                package.Info.Arena = new VersionRequirement();

            package.Info.Arena.MinVersion = new PackageVersion(tbRequirements_ArenaMinVer.Text);
        }


        void tbRequirements_ArenaMaxVer_Validating(object sender, CancelEventArgs e)
        {
            PackageVersion version;

            try
            {
                version = new PackageVersion(tbRequirements_ArenaMaxVer.Text);
                this.errorProvider1.SetError(tbRequirements_ArenaMaxVer, "");
            }
            catch
            {
                e.Cancel = true;
                this.errorProvider1.SetError(tbRequirements_ArenaMaxVer, "Invalid version number");
            }
        }

        void tbRequirements_ArenaMaxVer_Validated(object sender, EventArgs e)
        {
            if (package.Info.Arena == null)
                package.Info.Arena = new VersionRequirement();

            package.Info.Arena.MaxVersion = new PackageVersion(tbRequirements_ArenaMaxVer.Text);
        }


        void tbRequirements_ArenaExactVer_Validating(object sender, CancelEventArgs e)
        {
            PackageVersion version;

            try
            {
                version = new PackageVersion(tbRequirements_ArenaExactVer.Text);
                this.errorProvider1.SetError(tbRequirements_ArenaExactVer, "");
            }
            catch
            {
                e.Cancel = true;
                this.errorProvider1.SetError(tbRequirements_ArenaExactVer, "Invalid version number");
            }
        }

        void tbRequirements_ArenaExactVer_Validated(object sender, EventArgs e)
        {
            if (package.Info.Arena == null)
                package.Info.Arena = new VersionRequirement();

            package.Info.Arena.Version = new PackageVersion(tbRequirements_ArenaExactVer.Text);
        }


        #endregion


        #region Required Packages Event Handlers


        void dgRequirements_Required_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            throw new NotImplementedException();
        }

        
        /// <summary>
        /// Retrieve the a specific cell value for the Packages Required
        /// datagrid in the Requirements tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Required_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == package.Info.Requires.Count)
            {
                return;
            }

            if (e.ColumnIndex == 0)
                e.Value = package.Info.Requires[e.RowIndex].Name;
            else if (e.ColumnIndex == 1)
                e.Value = package.Info.Requires[e.RowIndex].MinVersion.ToString();
            else if (e.ColumnIndex == 2)
                e.Value = package.Info.Requires[e.RowIndex].MaxVersion.ToString();
            else if (e.ColumnIndex == 3)
                e.Value = package.Info.Requires[e.RowIndex].Version.ToString();

            throw new NotImplementedException();
        }


        /// <summary>
        /// Set the value of a specific cell in the Required Packages
        /// datagrid. If the user is editing a value in the "add" row
        /// then add a new package requirement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Required_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == package.Info.Requires.Count)
            {
                package.Info.Requires.Add(new PackageRequirement());
            }

            if (e.ColumnIndex == 0)
                package.Info.Requires[e.RowIndex].Name = e.Value.ToString();
            else if (e.ColumnIndex == 0)
                package.Info.Requires[e.RowIndex].MinVersion = new PackageVersion(e.Value.ToString());
            else if (e.ColumnIndex == 0)
                package.Info.Requires[e.RowIndex].MaxVersion = new PackageVersion(e.Value.ToString());
            else if (e.ColumnIndex == 0)
                package.Info.Requires[e.RowIndex].Version = new PackageVersion(e.Value.ToString());
        }


        /// <summary>
        /// User is deleting a row from the datagrid that handles the
        /// Required Packages list for this package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Required_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            package.Info.Requires.RemoveAt(e.Row.Index);
        }

        #endregion


        #region Recommended Packages Event Handlers

        #endregion
    }
}
