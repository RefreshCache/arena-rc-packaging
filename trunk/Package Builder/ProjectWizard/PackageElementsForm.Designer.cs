namespace RefreshCache.VisualStudio.Wizards
{
    partial class PackageElementsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cbUserControls = new System.Windows.Forms.CheckBox();
            this.cbSetup = new System.Windows.Forms.CheckBox();
            this.cbBusinessLogic = new System.Windows.Forms.CheckBox();
            this.cbPackageBuilderTemplate = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cbUseLongNames = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // cbUserControls
            // 
            this.cbUserControls.AutoSize = true;
            this.cbUserControls.Checked = true;
            this.cbUserControls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUserControls.Location = new System.Drawing.Point(102, 44);
            this.cbUserControls.Name = "cbUserControls";
            this.cbUserControls.Size = new System.Drawing.Size(89, 17);
            this.cbUserControls.TabIndex = 0;
            this.cbUserControls.Text = "User Controls";
            this.cbUserControls.UseVisualStyleBackColor = true;
            // 
            // cbSetup
            // 
            this.cbSetup.AutoSize = true;
            this.cbSetup.Checked = true;
            this.cbSetup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSetup.Location = new System.Drawing.Point(102, 90);
            this.cbSetup.Name = "cbSetup";
            this.cbSetup.Size = new System.Drawing.Size(155, 17);
            this.cbSetup.TabIndex = 1;
            this.cbSetup.Text = "Setup (Database Migration)";
            this.cbSetup.UseVisualStyleBackColor = true;
            // 
            // cbBusinessLogic
            // 
            this.cbBusinessLogic.AutoSize = true;
            this.cbBusinessLogic.Checked = true;
            this.cbBusinessLogic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBusinessLogic.Location = new System.Drawing.Point(102, 67);
            this.cbBusinessLogic.Name = "cbBusinessLogic";
            this.cbBusinessLogic.Size = new System.Drawing.Size(165, 17);
            this.cbBusinessLogic.TabIndex = 2;
            this.cbBusinessLogic.Text = "Business Logic (Class Library)";
            this.cbBusinessLogic.UseVisualStyleBackColor = true;
            // 
            // cbPackageBuilderTemplate
            // 
            this.cbPackageBuilderTemplate.AutoSize = true;
            this.cbPackageBuilderTemplate.Enabled = false;
            this.cbPackageBuilderTemplate.Location = new System.Drawing.Point(102, 113);
            this.cbPackageBuilderTemplate.Name = "cbPackageBuilderTemplate";
            this.cbPackageBuilderTemplate.Size = new System.Drawing.Size(151, 17);
            this.cbPackageBuilderTemplate.TabIndex = 3;
            this.cbPackageBuilderTemplate.Text = "Package Builder Template";
            this.cbPackageBuilderTemplate.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(333, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Please select the items to include in this new package.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(270, 163);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbUseLongNames
            // 
            this.cbUseLongNames.AutoSize = true;
            this.cbUseLongNames.Location = new System.Drawing.Point(15, 163);
            this.cbUseLongNames.Name = "cbUseLongNames";
            this.cbUseLongNames.Size = new System.Drawing.Size(108, 17);
            this.cbUseLongNames.TabIndex = 6;
            this.cbUseLongNames.Text = "Use Long Names";
            this.cbUseLongNames.UseVisualStyleBackColor = true;
            // 
            // PackageElementsForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 198);
            this.ControlBox = false;
            this.Controls.Add(this.cbUseLongNames);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbPackageBuilderTemplate);
            this.Controls.Add(this.cbBusinessLogic);
            this.Controls.Add(this.cbSetup);
            this.Controls.Add(this.cbUserControls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PackageElementsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbUserControls;
        private System.Windows.Forms.CheckBox cbSetup;
        private System.Windows.Forms.CheckBox cbBusinessLogic;
        private System.Windows.Forms.CheckBox cbPackageBuilderTemplate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbUseLongNames;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}