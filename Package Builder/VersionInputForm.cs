using RefreshCache.Packager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RefreshCache.Packager.Builder
{
    public partial class VersionInputForm : Form
    {
        public PackageVersion Version;


        /// <summary>
        /// Initialize a new windows form designed to retrieve a valid
        /// version number from the user.
        /// </summary>
        public VersionInputForm()
        {
            InitializeComponent();

            tbVersion.Validating += new CancelEventHandler(tbVersion_Validating);
            tbVersion.Validated += new EventHandler(tbVersion_Validated);
        }


        /// <summary>
        /// User has entered a value that needs to be validated. Make sure
        /// the text entered is a valid version number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbVersion_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                PackageVersion v = new PackageVersion(tbVersion.Text);
                errorProvider1.SetError(tbVersion, "");
            }
            catch
            {
                e.Cancel = true;
                errorProvider1.SetError(tbVersion, "Invalid version number specified.");
            }
        }


        /// <summary>
        /// User has entered a value in the version field. It has already been
        /// validated and needs to be stored.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbVersion_Validated(object sender, EventArgs e)
        {
            Version = new PackageVersion(tbVersion.Text);
            tbVersion.Text = Version.ToString();
        }
    }
}
