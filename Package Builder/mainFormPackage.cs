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
        /// Initialize all the events and actions associated with
        /// the Package tab and prepare for use.
        /// </summary>
        private void InitPackageTab()
        {
            //
            // Setup the tab actions and events.
            //
            tbPackageReadme.Validated += new EventHandler(tbPackageReadme_Validated);

        }


        /// <summary>
        /// Update the user interface elements on the package tab in
        /// response to a new package being loaded into memory.
        /// </summary>
        private void UpdatePackageTab()
        {
            tbPackageReadme.Text = package.Readme;
        }

        
        #region Actions and Events

        /// <summary>
        /// The value for the package readme has been updated and validated.
        /// Store the new readme into the package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbPackageReadme_Validated(object sender, EventArgs e)
        {
            package.Readme = tbPackageReadme.Text;
        }

        #endregion
    }
}
