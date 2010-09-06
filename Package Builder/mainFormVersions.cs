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
        /// Initialize all user interface elements on the Versions
        /// tab.
        /// </summary>
        private void InitVersionsTab()
        {
            cbVersions_Version.SelectedIndexChanged += new EventHandler(cbVersions_Version_SelectedIndexChanged);
            tbVersions_Changelog.Validated += new EventHandler(tbVersions_Changelog_Validated);
        }


        /// <summary>
        /// The active package has changed. Update all the user
        /// interface elements in this tab.
        /// </summary>
        private void UpdateVersionsTab()
        {
            //
            // Clear out any old selections.
            //
            cbVersions_Version.SelectedIndex = -1;
            cbVersions_Version.Items.Clear();
            tbVersions_Changelog.Text = "";
            tbVersions_Changelog.Enabled = false;

            //
            // Add each changelog version to the combobox.
            //
            foreach (PackageChangelog changelog in package.Info.Changelog)
            {
                cbVersions_Version.Items.Add(changelog.Version.ToString());
            }

            //
            // Update the selection.
            //
            if (cbVersions_Version.Items.Count > 0)
                cbVersions_Version.SelectedIndex = 0;
        }


        #region User Interface

        /// <summary>
        /// User wants to add a new version to the changelog list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVersions_Add_Click(object sender, EventArgs e)
        {
            VersionInputForm vInput;
            PackageChangelog changelog;
            int idx;


            //
            // Ask the user what version number to add.
            //
            vInput = new VersionInputForm();
            if (vInput.ShowDialog(this) == DialogResult.Cancel)
                return;

            //
            // Find the index to insert the version at.
            //
            for (idx = 0; idx < package.Info.Changelog.Count; idx++)
            {
                if (vInput.Version.CompareTo(package.Info.Changelog[idx].Version) > 0)
                    break;
            }

            //
            // Insert the new changelog entry at the identified index.
            //
            changelog = new PackageChangelog();
            changelog.Version = vInput.Version;
            changelog.Description = "";
            package.Info.Changelog.Insert(idx, changelog);

            //
            // Update the user interface and select the new changelog entry.
            //
            cbVersions_Version.SelectedIndex = -1;
            UpdateVersionsTab();
            cbVersions_Version.SelectedIndex = idx;
        }


        /// <summary>
        /// Remove the selected version from the changelog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVersions_Remove_Click(object sender, EventArgs e)
        {
            DialogResult result;


            if (cbVersions_Version.SelectedIndex == -1)
                return;

            //
            // Ask the user if they really want to delete the
            // current version number.
            //
            result = MessageBox.Show("Confirm delete",
                "Are you sure you want to delete version " + cbVersions_Version.SelectedValue,
                MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
                return;

            //
            // Delete and update the user interface.
            //
            cbVersions_Version.SelectedIndex = -1;
            package.Info.Changelog.RemoveAt(cbVersions_Version.SelectedIndex);
            UpdateVersionsTab();
            cbVersions_Version.SelectedIndex = (package.Info.Changelog.Count > 0 ? 0 : -1);
        }


        /// <summary>
        /// Selected version number has changed, update the changelog
        /// description.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbVersions_Version_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVersions_Version.SelectedIndex >= 0)
            {
                tbVersions_Changelog.Text = package.Info.Changelog[cbVersions_Version.SelectedIndex].Description;
                tbVersions_Changelog.Enabled = true;
            }
            else
            {
                tbVersions_Changelog.Text = "";
                tbVersions_Changelog.Enabled = false;
            }
        }


        /// <summary>
        /// User has changed the changelog description value for the
        /// currently selected version number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbVersions_Changelog_Validated(object sender, EventArgs e)
        {
            package.Info.Changelog[cbVersions_Version.SelectedIndex].Description = tbVersions_Changelog.Text;
        }

        #endregion
    }
}
