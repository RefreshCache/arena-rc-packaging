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
        /// The user interface has initialized. Prepare all interface
        /// elements on the Info tab.
        /// </summary>
        private void InitInfoTab()
        {
            //
            // Setup the tab actions and events.
            //
            tbInfo_Distributor.Validated += new EventHandler(tbInfo_Distributor_Validated);
            tbInfo_PackageName.Validated += new EventHandler(tbInfo_PackageName_Validated);
            tbInfo_Synopsis.Validated += new EventHandler(tbInfo_Synopsis_Validated);
            tbInfo_Description.Validated += new EventHandler(tbInfo_Description_Validated);
        }


        /// <summary>
        /// The loaded package has changed. Update all fields on the
        /// Info tab to reflect the new package.
        /// </summary>
        private void UpdateInfoTab()
        {
            tbInfo_Distributor.Text = package.Info.Distributor;
            tbInfo_PackageName.Text = package.Info.PackageName;
            tbInfo_Synopsis.Text = package.Info.Synopsis;
            tbInfo_Description.Text = package.Info.Description;
        }


        #region User Interface

        /// <summary>
        /// User has finished entering the value for the Distributor of
        /// this package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbInfo_Distributor_Validated(object sender, EventArgs e)
        {
            package.Info.Distributor = tbInfo_Distributor.Text;
        }


        /// <summary>
        /// User has finished entering the name of the package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbInfo_PackageName_Validated(object sender, EventArgs e)
        {
            package.Info.PackageName = tbInfo_PackageName.Text;
        }


        /// <summary>
        /// The synopsis for this package has been entered. Save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbInfo_Synopsis_Validated(object sender, EventArgs e)
        {
            package.Info.Synopsis = tbInfo_Synopsis.Text;
        }


        /// <summary>
        /// A long description of the package has been entered by the
        /// user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbInfo_Description_Validated(object sender, EventArgs e)
        {
            package.Info.Description = tbInfo_Description.Text;
        }

        #endregion

    }
}
