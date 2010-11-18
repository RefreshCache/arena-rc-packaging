using RefreshCache.Packager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace RefreshCache.Packager.Builder
{
    public partial class mainForm : Form
    {
        private Package package;
        private String currentFileName = null;
        private Boolean selectionChanging = false;


        /// <summary>
        /// Initialize a new mainForm and prepare the user interface
        /// for use.
        /// </summary>
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
            // Initialize and prepare all tabs for use.
            //
            InitTabs();
            UpdateTabs();
        }


        #region Helper methods

        /// <summary>
        /// Initialize all the tabs and prepare them for use.
        /// </summary>
        private void InitTabs()
        {
			InitPackageTab();
            InitInfoTab();
            InitRequirementsTab();
            InitVersionsTab();
            InitFilesTab();
            InitModulesTab();
            InitPagesTab();
        }


        /// <summary>
        /// Update all the user interface elements when a package is loaded.
        /// </summary>
        private void UpdateTabs()
        {
			UpdatePackageTab();
            UpdateInfoTab();
            UpdateRequirementsTab();
            UpdateVersionsTab();
            UpdateFilesTab();
            UpdateModulesTab();
            UpdatePagesTab();
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
            xmlWriterSettings.OmitXmlDeclaration = true;

            XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
            doc.Save(xmlWriter);
            xmlWriter.Flush();
            xmlWriter.Close();

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        #endregion


        #region Top Level User Interface

        void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selectedValue;

            selectedValue = cbModuleInstanceType.SelectedValue;
            cbModuleInstanceType.DataSource = null;
            cbModuleInstanceType.ValueMember = "ModuleID";
            cbModuleInstanceType.DisplayMember = "Name";
            cbModuleInstanceType.DataSource = package.Modules;
            if (selectedValue != null)
                cbModuleInstanceType.SelectedValue = selectedValue;
        }

        private void newMenu_Click(object sender, EventArgs e)
        {
            currentFileName = null;

            package = new Package();

            UpdateTabs();
        }

        private void openMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();


            dialog.Filter = "PBXML Files|*.pbxml";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                openFromFile(dialog.FileName);
            }
        }

        private void saveMenu_Click(object sender, EventArgs e)
        {
            if (tcMain.Focus() == false)
                return;

            if (currentFileName == null)
                saveAsMenu_Click(null, null);
            else
                saveToFile(currentFileName);
        }

        private void saveAsMenu_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();


            if (tcMain.Focus() == false)
                return;

            dialog.Filter = "PBXML Files|*.pbxml";
            dialog.FilterIndex = 0;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                saveToFile(dialog.FileName);
            }
        }

        private void buildMenu_Click(object sender, EventArgs e)
        {
            BuildMessageCollection messages;
            FolderBrowserDialog folder = new FolderBrowserDialog();
            SaveFileDialog save = new SaveFileDialog();


            if (tcMain.Focus() == false)
                return;

            //
            // Get the file to save the built package as.
            //
            save.Filter = "Arena Page Export|*.xml";
            save.FilterIndex = 0;
            if (save.ShowDialog() != DialogResult.OK)
                return;

            //
            // Get the base path to build from.
            //
            folder.Description = "Select the path to use as the base path when accessing local files.";
            folder.ShowNewFolderButton = false;
            if (!String.IsNullOrEmpty(currentFileName))
                folder.SelectedPath = new FileInfo(currentFileName).DirectoryName;
            if (folder.ShowDialog() != DialogResult.OK)
                return;

            //
            // Save the package to XML.
            //
            messages = package.Build(folder.SelectedPath);

            //
            // Check if there were any errors during the build.
            //
            if (package.XmlPackage == null)
            {
                MessageBox.Show(messages.ToString(), "Errors during build",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            //
            // If there were any warnings during the build, show those.
            //
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
            StreamWriter writer = new StreamWriter(save.FileName);
            writer.Write(XmlDocumentToString(package.XmlPackage));
            writer.Close();
        }
        
        private void verifyMenu_Click(object sender, EventArgs e)
        {
            BuildMessageCollection messages;
            FolderBrowserDialog folder = new FolderBrowserDialog();


            if (tcMain.Focus() == false)
                return;

            //
            // Get the base path to build from.
            //
            folder.Description = "Select the path to use as the base path when accessing local files.";
            folder.ShowNewFolderButton = false;
            if (!String.IsNullOrEmpty(currentFileName))
                folder.SelectedPath = new FileInfo(currentFileName).DirectoryName;
            if (folder.ShowDialog() != DialogResult.OK)
                return;

            //
            // Save the package to XML.
            //
            messages = package.Build(folder.SelectedPath);

            //
            // Display any messages that might have occurred.
            //
            MessageBox.Show((messages.Count > 0 ? messages.ToString() : "No errors or warnings."), "Verify Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            package = new Package(doc);
            UpdateTabs();

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
