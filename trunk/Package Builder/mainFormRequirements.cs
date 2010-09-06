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
        /// Initialize all the components on the Requirements tab.
        /// </summary>
        private void InitRequirementsTab()
        {
            //
            // Setup the actions and events for the Arena version.
            //
            tbRequirements_ArenaMinVer.Validating += new CancelEventHandler(tbRequirements_ArenaMinVer_Validating);
            tbRequirements_ArenaMinVer.Validated += new EventHandler(tbRequirements_ArenaMinVer_Validated);
            tbRequirements_ArenaMaxVer.Validating += new CancelEventHandler(tbRequirements_ArenaMaxVer_Validating);
            tbRequirements_ArenaMaxVer.Validated += new EventHandler(tbRequirements_ArenaMaxVer_Validated);
            tbRequirements_ArenaExactVer.Validating += new CancelEventHandler(tbRequirements_ArenaExactVer_Validating);
            tbRequirements_ArenaExactVer.Validated += new EventHandler(tbRequirements_ArenaExactVer_Validated);

            //
            // Setup the actions and events for the Required Packages.
            //
            dgRequirements_Required.VirtualMode = true;
            dgRequirements_Required.CellValidating += new DataGridViewCellValidatingEventHandler(dgRequirements_Required_CellValidating);
            dgRequirements_Required.CellValueNeeded += new DataGridViewCellValueEventHandler(dgRequirements_Required_CellValueNeeded);
            dgRequirements_Required.CellValuePushed += new DataGridViewCellValueEventHandler(dgRequirements_Required_CellValuePushed);
            dgRequirements_Required.UserDeletingRow += new DataGridViewRowCancelEventHandler(dgRequirements_Required_UserDeletingRow);

            //
            // Setup the actions and events for the Recommended Packages.
            //
            dgRequirements_Recommended.VirtualMode = true;
            dgRequirements_Recommended.CellValidating += new DataGridViewCellValidatingEventHandler(dgRequirements_Recommended_CellValidating);
            dgRequirements_Recommended.CellValueNeeded += new DataGridViewCellValueEventHandler(dgRequirements_Recommended_CellValueNeeded);
            dgRequirements_Recommended.CellValuePushed += new DataGridViewCellValueEventHandler(dgRequirements_Recommended_CellValuePushed);
            dgRequirements_Recommended.UserDeletingRow += new DataGridViewRowCancelEventHandler(dgRequirements_Recommended_UserDeletingRow);
        }


        /// <summary>
        /// The currently loaded package has changed. Update all the
        /// values on the Requirements tab.
        /// </summary>
        private void UpdateRequirementsTab()
        {
            if (package.Info.Arena != null && package.Info.Arena.MinVersion != null)
                tbRequirements_ArenaMinVer.Text = package.Info.Arena.MinVersion.ToString();
            else
                tbRequirements_ArenaMinVer.Text = "";

            if (package.Info.Arena != null && package.Info.Arena.MaxVersion != null)
                tbRequirements_ArenaMaxVer.Text = package.Info.Arena.MaxVersion.ToString();
            else
                tbRequirements_ArenaMaxVer.Text = "";

            if (package.Info.Arena != null && package.Info.Arena.Version != null)
                tbRequirements_ArenaExactVer.Text = package.Info.Arena.Version.ToString();
            else
                tbRequirements_ArenaExactVer.Text = "";

            dgRequirements_Required.RowCount = 1;
            dgRequirements_Required.RowCount = (package.Info.Requires.Count + 1);

            dgRequirements_Recommended.RowCount = 1;
            dgRequirements_Recommended.RowCount = (package.Info.Recommends.Count + 1);
        }


        #region Arena Version Event Handlers

        /// <summary>
        /// The Arena Minimum Version number has been updated, validate
        /// it to make sure the user entered a valid version number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// The Arena Minimum Version number has been validated. Save it
        /// and update the formatted value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbRequirements_ArenaMinVer_Validated(object sender, EventArgs e)
        {
            if (package.Info.Arena == null)
                package.Info.Arena = new VersionRequirement();

            package.Info.Arena.MinVersion = new PackageVersion(tbRequirements_ArenaMinVer.Text);
            tbRequirements_ArenaMinVer.Text = package.Info.Arena.MinVersion.ToString();
        }


        /// <summary>
        /// The Arena Maximum Version number has been updated, validate
        /// it to make sure the user entered a valid version number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// The Arena Maximum Version number has been validated. Save it
        /// and update the formatted value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbRequirements_ArenaMaxVer_Validated(object sender, EventArgs e)
        {
            if (package.Info.Arena == null)
                package.Info.Arena = new VersionRequirement();

            package.Info.Arena.MaxVersion = new PackageVersion(tbRequirements_ArenaMaxVer.Text);
            tbRequirements_ArenaMinVer.Text = package.Info.Arena.MinVersion.ToString();
        }


        /// <summary>
        /// The Arena Exact Version number has been updated, validate
        /// it to make sure the user entered a valid version number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// The Arena Exact Version number has been validated. Save it
        /// and update the formatted value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbRequirements_ArenaExactVer_Validated(object sender, EventArgs e)
        {
            if (package.Info.Arena == null)
                package.Info.Arena = new VersionRequirement();

            package.Info.Arena.Version = new PackageVersion(tbRequirements_ArenaExactVer.Text);
            tbRequirements_ArenaMinVer.Text = package.Info.Arena.MinVersion.ToString();
        }

        #endregion


        #region Required Packages Event Handlers

        /// <summary>
        /// User has entered a value in one of the Required Packages
        /// data grid. Validate that it is in good order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Required_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            PackageVersion version;

            if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                try
                {
                    version = new PackageVersion(e.FormattedValue.ToString());
                    this.errorProvider1.SetError(dgRequirements_Required, "");
                }
                catch
                {
                    e.Cancel = true;
                    this.errorProvider1.SetError(dgRequirements_Required, "Invalid version number");
                }
            }
            else
                this.errorProvider1.SetError(dgRequirements_Required, "");
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
            {
                if (package.Info.Requires[e.RowIndex].MinVersion != null)
                    e.Value = package.Info.Requires[e.RowIndex].MinVersion.ToString();
                else
                    e.Value = "";
            }
            else if (e.ColumnIndex == 2)
            {
                if (package.Info.Requires[e.RowIndex].MaxVersion != null)
                    e.Value = package.Info.Requires[e.RowIndex].MaxVersion.ToString();
                else
                    e.Value = "";
            }
            else if (e.ColumnIndex == 3)
            {
                if (package.Info.Requires[e.RowIndex].Version != null)
                    e.Value = package.Info.Requires[e.RowIndex].Version.ToString();
                else
                    e.Value = "";
            }
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
            else if (e.ColumnIndex == 1)
            {
                if (e.Value != null)
                    package.Info.Requires[e.RowIndex].MinVersion = new PackageVersion(e.Value.ToString());
                else
                    package.Info.Requires[e.RowIndex].MinVersion = null;
            }
            else if (e.ColumnIndex == 2)
            {
                if (e.Value != null)
                    package.Info.Requires[e.RowIndex].MaxVersion = new PackageVersion(e.Value.ToString());
                else
                    package.Info.Requires[e.RowIndex].MaxVersion = null;
            }
            else if (e.ColumnIndex == 3)
            {
                if (e.Value != null)
                    package.Info.Requires[e.RowIndex].Version = new PackageVersion(e.Value.ToString());
                else
                    package.Info.Requires[e.RowIndex].Version = null;
            }
        }


        /// <summary>
        /// User is deleting a row from the datagrid that handles the
        /// Required Packages list for this package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Required_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            package.Info.Requires.RemoveAt(e.Row.Index);
        }

        #endregion


        #region Recommended Packages Event Handlers

        /// <summary>
        /// User has entered a value in one of the Recommended Packages
        /// data grid. Validate that it is in good order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Recommended_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            PackageVersion version;

            if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                try
                {
                    version = new PackageVersion(e.FormattedValue.ToString());
                    this.errorProvider1.SetError(dgRequirements_Recommended, "");
                }
                catch
                {
                    e.Cancel = true;
                    this.errorProvider1.SetError(dgRequirements_Recommended, "Invalid version number");
                }
            }
            else
                this.errorProvider1.SetError(dgRequirements_Recommended, "");
        }


        /// <summary>
        /// Retrieve the a specific cell value for the Packages Recommended
        /// datagrid in the Requirements tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Recommended_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == package.Info.Recommends.Count)
            {
                return;
            }

            if (e.ColumnIndex == 0)
            {
                e.Value = package.Info.Recommends[e.RowIndex].Name;
            }
            else if (e.ColumnIndex == 1)
            {
                if (package.Info.Recommends[e.RowIndex].MinVersion != null)
                    e.Value = package.Info.Recommends[e.RowIndex].MinVersion.ToString();
                else
                    e.Value = "";
            }
            else if (e.ColumnIndex == 2)
            {
                if (package.Info.Recommends[e.RowIndex].MaxVersion != null)
                    e.Value = package.Info.Recommends[e.RowIndex].MaxVersion.ToString();
                else
                    e.Value = "";
            }
            else if (e.ColumnIndex == 3)
            {
                if (package.Info.Recommends[e.RowIndex].Version != null)
                    e.Value = package.Info.Recommends[e.RowIndex].Version.ToString();
                else
                    e.Value = "";
            }
            else if (e.ColumnIndex == 4)
            {
                e.Value = package.Info.Recommends[e.RowIndex].Description;
            }
        }


        /// <summary>
        /// Set the value of a specific cell in the Recommended Packages
        /// datagrid. If the user is editing a value in the "add" row
        /// then add a new package recommendation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Recommended_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == package.Info.Recommends.Count)
            {
                package.Info.Recommends.Add(new PackageRecommendation());
            }

            if (e.ColumnIndex == 0)
            {
                package.Info.Recommends[e.RowIndex].Name = e.Value.ToString();
            }
            else if (e.ColumnIndex == 1)
            {
                if (e.Value != null)
                    package.Info.Recommends[e.RowIndex].MinVersion = new PackageVersion(e.Value.ToString());
                else
                    package.Info.Recommends[e.RowIndex].MinVersion = null;
            }
            else if (e.ColumnIndex == 2)
            {
                if (e.Value != null)
                    package.Info.Recommends[e.RowIndex].MaxVersion = new PackageVersion(e.Value.ToString());
                else
                    package.Info.Recommends[e.RowIndex].MaxVersion = null;
            }
            else if (e.ColumnIndex == 3)
            {
                if (e.Value != null)
                    package.Info.Recommends[e.RowIndex].Version = new PackageVersion(e.Value.ToString());
                else
                    package.Info.Recommends[e.RowIndex].Version = null;
            }
            else if (e.ColumnIndex == 4)
            {
                package.Info.Recommends[e.RowIndex].Description = e.Value.ToString();
            }
        }


        /// <summary>
        /// User is deleting a row from the datagrid that handles the
        /// Recommended Packages list for this package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgRequirements_Recommended_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            package.Info.Recommends.RemoveAt(e.Row.Index);
        }

        #endregion
    }
}
