using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RefreshCache.VisualStudio.Wizards
{
    public partial class PackageElementsForm : Form
    {
        public Boolean UserControls { get { return cbUserControls.Checked; } }
        public Boolean BusinessLogic { get { return cbBusinessLogic.Checked; } }
        public Boolean Setup { get { return cbSetup.Checked; } }
        public Boolean PackageBuilderTemplate { get { return cbPackageBuilderTemplate.Checked; } }
        public Boolean UseLongNames { get { return cbUseLongNames.Checked; } }

        public PackageElementsForm()
        {
            InitializeComponent();

            toolTip1.SetToolTip(cbUseLongNames, "If this is checked then the projects will use full names instead of short names, e.g. Arena.Custom.RC.PackageBuilder");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
