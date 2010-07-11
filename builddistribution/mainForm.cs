using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace BuildDistribution
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
            importFiles(null);

            dgModules.SelectionChanged += new EventHandler(dgModules_SelectionChanged);
            dgModules_SelectionChanged(null, null);
        }

        #region Files Tab User Interface

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            dgFiles.Rows.Add();
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (dgFiles.SelectedCells.Count > 0)
            {
                dgFiles.Rows.RemoveAt(dgFiles.SelectedCells[0].RowIndex);
            }
        }


        #endregion

        #region Modules Tab User Interface

        void dgModules_SelectionChanged(object sender, EventArgs e)
        {
            Boolean enabled = false;


            if (dgModules.SelectedCells.Count > 0)
            {
                btnModulesCancel_Click(null, null);
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
            tbModuleName.Enabled = false;
            tbModuleURL.Enabled = false;
            tbModuleImagePath.Enabled = false;
            cbModuleAllowsChildModules.Enabled = false;
            tbModuleSourcePath.Enabled = false;
            tbModuleSourceImagePath.Enabled = false;
            tbModuleDescription.Enabled = false;
            btnModulesEdit.Enabled = true;
            btnModulesEdit.Text = "Edit";
            btnModulesCancel.Enabled = false;
        }

        private void btnAddModule_Click(object sender, EventArgs e)
        {
            dgModules.Rows.Add();
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["module_name"].Value = "New Module";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["module_url"].Value = "";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["image_path"].Value = "";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["allows_child_modules"].Value = "0";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["_source"].Value = "";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["_source_image"].Value = "";
            dgModules.Rows[dgModules.Rows.Count - 1].Cells["module_desc"].Value = "";

            dgModules_SelectionChanged(null, null);
        }

        private void btnRemoveModule_Click(object sender, EventArgs e)
        {
            if (dgModules.SelectedCells.Count > 0)
            {
                dgModules.Rows.RemoveAt(dgModules.SelectedCells[0].RowIndex);
            }
        }

        private void btnModulesCancel_Click(object sender, EventArgs e)
        {
            DataGridViewRow row;


            row = dgModules.Rows[dgModules.SelectedCells[0].RowIndex];
            try
            {
                tbModuleName.Text = row.Cells["module_name"].Value.ToString();
                tbModuleURL.Text = row.Cells["module_url"].Value.ToString();
                tbModuleImagePath.Text = row.Cells["image_path"].Value.ToString();
                cbModuleAllowsChildModules.Checked = (row.Cells["allows_child_modules"].Value.ToString() == "1" ? true : false);
                tbModuleSourcePath.Text = row.Cells["_source"].Value.ToString();
                tbModuleSourceImagePath.Text = row.Cells["_source_image"].Value.ToString();
                tbModuleDescription.Text = row.Cells["module_desc"].Value.ToString();
            }
            catch { }

            tbModuleName.Enabled = false;
            tbModuleURL.Enabled = false;
            tbModuleImagePath.Enabled = false;
            cbModuleAllowsChildModules.Enabled = false;
            tbModuleSourcePath.Enabled = false;
            tbModuleSourceImagePath.Enabled = false;
            tbModuleDescription.Enabled = false;

            btnModulesCancel.Enabled = false;
            btnModulesEdit.Text = "Edit";
        }

        private void btnModulesEdit_Click(object sender, EventArgs e)
        {
            Boolean enabled = false;


            if (btnModulesEdit.Text == "Edit")
            {
                //
                // Edit...
                //
                btnModulesCancel.Enabled = true;
                btnModulesEdit.Text = "Save";
                enabled = true;
            }
            else
            {
                DataGridViewRow row;

                //
                // Save...
                //
                row = dgModules.Rows[dgModules.SelectedCells[0].RowIndex];
                row.Cells["module_name"].Value = tbModuleName.Text;
                row.Cells["module_url"].Value = tbModuleURL.Text;
                row.Cells["image_path"].Value = tbModuleImagePath.Text;
                row.Cells["allows_child_modules"].Value = (cbModuleAllowsChildModules.Checked ? "1" : "0");
                row.Cells["_source"].Value = tbModuleSourcePath.Text;
                row.Cells["_source_image"].Value = tbModuleSourceImagePath.Text;
                row.Cells["module_desc"].Value = tbModuleDescription.Text;

                btnModulesCancel.Enabled = false;
                btnModulesEdit.Text = "Save";
            }

            tbModuleName.Enabled = enabled;
            tbModuleURL.Enabled = enabled;
            tbModuleImagePath.Enabled = enabled;
            cbModuleAllowsChildModules.Enabled = enabled;
            tbModuleSourcePath.Enabled = enabled;
            tbModuleSourceImagePath.Enabled = enabled;
            tbModuleDescription.Enabled = enabled;
        }


        #endregion

        private void importFiles(XmlDocument doc)
        {
            dgFiles.Rows.Add(new string[] { "UserControls/Test.ascx", "../../test.ascx" });
        }

        private void importModules(XmlDocument doc)
        {
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlDocument doc;
            XmlDeclaration decl;
            XmlNode nodeRoot, node;
            XmlAttribute attrib;


            //
            // Setup the XML document.
            //
            doc = new XmlDocument();
            decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(decl);
            nodeRoot = doc.CreateElement("ArenaPackage");
            attrib = doc.CreateAttribute("version");
            attrib.InnerText = "2009.2.100.1401";
            nodeRoot.Attributes.Append(attrib);
            doc.AppendChild(nodeRoot);

            //
            // Process the stand alone files.
            //
            nodeRoot.AppendChild(BuildFilesNode(doc));

            //
            // Process the modules.
            //
            nodeRoot.AppendChild(BuildModulesNode(doc));

            //
            // Dump result.
            //
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            doc.Save(writer);
            Console.WriteLine(sb.ToString());
        }

        private XmlNode BuildFilesNode(XmlDocument doc)
        {
            XmlNode nodeFiles, nodeFile;
            XmlAttribute attrib;
            int i;


            //
            // Process standalone files.
            //
            nodeFiles = doc.CreateElement("Files");
            for (i = 0; i < dgFiles.Rows.Count; i++)
            {
                nodeFile = doc.CreateElement("File");
                attrib = doc.CreateAttribute("path");
                attrib.InnerText = dgFiles.Rows[i].Cells[0].Value.ToString();
                nodeFile.Attributes.Append(attrib);
                attrib = doc.CreateAttribute("_source");
                attrib.InnerText = dgFiles.Rows[i].Cells[1].Value.ToString();
                nodeFile.Attributes.Append(attrib);

                nodeFiles.AppendChild(nodeFile);
            }

            return nodeFiles;
        }

        private XmlNode BuildModulesNode(XmlDocument doc)
        {
            XmlNode nodeModules, nodeModule;
            XmlAttribute attrib;
            int i;


            //
            // Process standalone files.
            //
            nodeModules = doc.CreateElement("Modules");
            for (i = 0; i < dgModules.Rows.Count; i++)
            {
                nodeModule = doc.CreateElement("Module");

                attrib = doc.CreateAttribute("module_name");
                attrib.InnerText = dgModules.Rows[i].Cells["module_name"].Value.ToString();
                nodeModule.Attributes.Append(attrib);
                
                nodeModules.AppendChild(nodeModule);
            }

            return nodeModules;
        }
    }
}
