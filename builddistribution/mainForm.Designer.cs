namespace BuildDistribution
{
    partial class mainForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabFiles = new System.Windows.Forms.TabPage();
            this.btnRemoveFile = new System.Windows.Forms.Button();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.dgFiles = new System.Windows.Forms.DataGridView();
            this.path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabModules = new System.Windows.Forms.TabPage();
            this.btnModulesCancel = new System.Windows.Forms.Button();
            this.btnModulesEdit = new System.Windows.Forms.Button();
            this.tbModuleDescription = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbModuleAllowsChildModules = new System.Windows.Forms.CheckBox();
            this.tbModuleSourceImagePath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbModuleSourcePath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbModuleImagePath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbModuleURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbModuleName = new System.Windows.Forms.TextBox();
            this.dgModules = new System.Windows.Forms.DataGridView();
            this.module_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.module_url = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.module_desc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.allows_child_modules = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.image_path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._source_image = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRemoveModule = new System.Windows.Forms.Button();
            this.btnAddModule = new System.Windows.Forms.Button();
            this.tabPages = new System.Windows.Forms.TabPage();
            this.tcPages = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.dgPageSettings = new System.Windows.Forms.DataGridView();
            this.page_setting_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.page_setting_value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label9 = new System.Windows.Forms.Label();
            this.tbPageDescription = new System.Windows.Forms.TextBox();
            this.cbPageRequireSSL = new System.Windows.Forms.CheckBox();
            this.cbPageValidateRequest = new System.Windows.Forms.CheckBox();
            this.cbPageDisplayInNav = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPageName = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnPagesDelete = new System.Windows.Forms.Button();
            this.btnPagesAddModule = new System.Windows.Forms.Button();
            this.btnPagesAddPage = new System.Windows.Forms.Button();
            this.tvPages = new System.Windows.Forms.TreeView();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFiles)).BeginInit();
            this.tabModules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgModules)).BeginInit();
            this.tabPages.SuspendLayout();
            this.tcPages.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPageSettings)).BeginInit();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabFiles);
            this.tabControl1.Controls.Add(this.tabModules);
            this.tabControl1.Controls.Add(this.tabPages);
            this.tabControl1.Location = new System.Drawing.Point(12, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(768, 534);
            this.tabControl1.TabIndex = 0;
            // 
            // tabFiles
            // 
            this.tabFiles.Controls.Add(this.btnRemoveFile);
            this.tabFiles.Controls.Add(this.btnAddFile);
            this.tabFiles.Controls.Add(this.dgFiles);
            this.tabFiles.Location = new System.Drawing.Point(4, 22);
            this.tabFiles.Name = "tabFiles";
            this.tabFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabFiles.Size = new System.Drawing.Size(760, 508);
            this.tabFiles.TabIndex = 0;
            this.tabFiles.Text = "Files";
            this.tabFiles.UseVisualStyleBackColor = true;
            // 
            // btnRemoveFile
            // 
            this.btnRemoveFile.Location = new System.Drawing.Point(598, 479);
            this.btnRemoveFile.Name = "btnRemoveFile";
            this.btnRemoveFile.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveFile.TabIndex = 3;
            this.btnRemoveFile.Text = "Remove File";
            this.btnRemoveFile.UseVisualStyleBackColor = true;
            this.btnRemoveFile.Click += new System.EventHandler(this.btnRemoveFile_Click);
            // 
            // btnAddFile
            // 
            this.btnAddFile.Location = new System.Drawing.Point(679, 479);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(75, 23);
            this.btnAddFile.TabIndex = 2;
            this.btnAddFile.Text = "Add File";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // dgFiles
            // 
            this.dgFiles.AllowUserToAddRows = false;
            this.dgFiles.AllowUserToDeleteRows = false;
            this.dgFiles.AllowUserToResizeRows = false;
            this.dgFiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgFiles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.path,
            this.source});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgFiles.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgFiles.Location = new System.Drawing.Point(6, 6);
            this.dgFiles.Name = "dgFiles";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgFiles.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgFiles.Size = new System.Drawing.Size(748, 467);
            this.dgFiles.TabIndex = 1;
            // 
            // path
            // 
            this.path.DataPropertyName = "path";
            this.path.HeaderText = "Arena Path";
            this.path.Name = "path";
            // 
            // source
            // 
            this.source.DataPropertyName = "source";
            this.source.HeaderText = "Source";
            this.source.Name = "source";
            // 
            // tabModules
            // 
            this.tabModules.Controls.Add(this.btnModulesCancel);
            this.tabModules.Controls.Add(this.btnModulesEdit);
            this.tabModules.Controls.Add(this.tbModuleDescription);
            this.tabModules.Controls.Add(this.label4);
            this.tabModules.Controls.Add(this.cbModuleAllowsChildModules);
            this.tabModules.Controls.Add(this.tbModuleSourceImagePath);
            this.tabModules.Controls.Add(this.label6);
            this.tabModules.Controls.Add(this.tbModuleSourcePath);
            this.tabModules.Controls.Add(this.label5);
            this.tabModules.Controls.Add(this.tbModuleImagePath);
            this.tabModules.Controls.Add(this.label3);
            this.tabModules.Controls.Add(this.tbModuleURL);
            this.tabModules.Controls.Add(this.label2);
            this.tabModules.Controls.Add(this.label1);
            this.tabModules.Controls.Add(this.tbModuleName);
            this.tabModules.Controls.Add(this.dgModules);
            this.tabModules.Controls.Add(this.btnRemoveModule);
            this.tabModules.Controls.Add(this.btnAddModule);
            this.tabModules.Location = new System.Drawing.Point(4, 22);
            this.tabModules.Name = "tabModules";
            this.tabModules.Padding = new System.Windows.Forms.Padding(3);
            this.tabModules.Size = new System.Drawing.Size(760, 508);
            this.tabModules.TabIndex = 1;
            this.tabModules.Text = "Modules";
            this.tabModules.UseVisualStyleBackColor = true;
            // 
            // btnModulesCancel
            // 
            this.btnModulesCancel.Location = new System.Drawing.Point(598, 315);
            this.btnModulesCancel.Name = "btnModulesCancel";
            this.btnModulesCancel.Size = new System.Drawing.Size(75, 23);
            this.btnModulesCancel.TabIndex = 20;
            this.btnModulesCancel.Text = "Cancel";
            this.btnModulesCancel.UseVisualStyleBackColor = true;
            this.btnModulesCancel.Click += new System.EventHandler(this.btnModulesCancel_Click);
            // 
            // btnModulesEdit
            // 
            this.btnModulesEdit.Location = new System.Drawing.Point(679, 315);
            this.btnModulesEdit.Name = "btnModulesEdit";
            this.btnModulesEdit.Size = new System.Drawing.Size(75, 23);
            this.btnModulesEdit.TabIndex = 19;
            this.btnModulesEdit.Text = "Edit";
            this.btnModulesEdit.UseVisualStyleBackColor = true;
            this.btnModulesEdit.Click += new System.EventHandler(this.btnModulesEdit_Click);
            // 
            // tbModuleDescription
            // 
            this.tbModuleDescription.Location = new System.Drawing.Point(383, 162);
            this.tbModuleDescription.Multiline = true;
            this.tbModuleDescription.Name = "tbModuleDescription";
            this.tbModuleDescription.Size = new System.Drawing.Size(371, 113);
            this.tbModuleDescription.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(271, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Module Description";
            // 
            // cbModuleAllowsChildModules
            // 
            this.cbModuleAllowsChildModules.AutoSize = true;
            this.cbModuleAllowsChildModules.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbModuleAllowsChildModules.Location = new System.Drawing.Point(269, 86);
            this.cbModuleAllowsChildModules.Name = "cbModuleAllowsChildModules";
            this.cbModuleAllowsChildModules.Size = new System.Drawing.Size(128, 17);
            this.cbModuleAllowsChildModules.TabIndex = 16;
            this.cbModuleAllowsChildModules.Text = "Allows Child Modules ";
            this.cbModuleAllowsChildModules.UseVisualStyleBackColor = true;
            // 
            // tbModuleSourceImagePath
            // 
            this.tbModuleSourceImagePath.Location = new System.Drawing.Point(383, 136);
            this.tbModuleSourceImagePath.Name = "tbModuleSourceImagePath";
            this.tbModuleSourceImagePath.Size = new System.Drawing.Size(371, 20);
            this.tbModuleSourceImagePath.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(271, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Source Image Path";
            // 
            // tbModuleSourcePath
            // 
            this.tbModuleSourcePath.Location = new System.Drawing.Point(383, 110);
            this.tbModuleSourcePath.Name = "tbModuleSourcePath";
            this.tbModuleSourcePath.Size = new System.Drawing.Size(371, 20);
            this.tbModuleSourcePath.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(271, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Source Path";
            // 
            // tbModuleImagePath
            // 
            this.tbModuleImagePath.Location = new System.Drawing.Point(383, 58);
            this.tbModuleImagePath.Name = "tbModuleImagePath";
            this.tbModuleImagePath.Size = new System.Drawing.Size(371, 20);
            this.tbModuleImagePath.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(271, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Image Path";
            // 
            // tbModuleURL
            // 
            this.tbModuleURL.Location = new System.Drawing.Point(383, 32);
            this.tbModuleURL.Name = "tbModuleURL";
            this.tbModuleURL.Size = new System.Drawing.Size(371, 20);
            this.tbModuleURL.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Module URL";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(271, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Module Name";
            // 
            // tbModuleName
            // 
            this.tbModuleName.Location = new System.Drawing.Point(383, 6);
            this.tbModuleName.Name = "tbModuleName";
            this.tbModuleName.Size = new System.Drawing.Size(158, 20);
            this.tbModuleName.TabIndex = 4;
            // 
            // dgModules
            // 
            this.dgModules.AllowUserToAddRows = false;
            this.dgModules.AllowUserToDeleteRows = false;
            this.dgModules.AllowUserToResizeColumns = false;
            this.dgModules.AllowUserToResizeRows = false;
            this.dgModules.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgModules.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgModules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgModules.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.module_name,
            this.module_url,
            this.module_desc,
            this.allows_child_modules,
            this.image_path,
            this._source,
            this._source_image});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgModules.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgModules.Location = new System.Drawing.Point(6, 6);
            this.dgModules.MultiSelect = false;
            this.dgModules.Name = "dgModules";
            this.dgModules.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgModules.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgModules.RowHeadersVisible = false;
            this.dgModules.Size = new System.Drawing.Size(259, 467);
            this.dgModules.TabIndex = 3;
            // 
            // module_name
            // 
            this.module_name.HeaderText = "Module";
            this.module_name.Name = "module_name";
            this.module_name.ReadOnly = true;
            this.module_name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // module_url
            // 
            this.module_url.HeaderText = "module_url";
            this.module_url.Name = "module_url";
            this.module_url.ReadOnly = true;
            this.module_url.Visible = false;
            // 
            // module_desc
            // 
            this.module_desc.HeaderText = "module_desc";
            this.module_desc.Name = "module_desc";
            this.module_desc.ReadOnly = true;
            this.module_desc.Visible = false;
            // 
            // allows_child_modules
            // 
            this.allows_child_modules.HeaderText = "allows_child_modules";
            this.allows_child_modules.Name = "allows_child_modules";
            this.allows_child_modules.ReadOnly = true;
            this.allows_child_modules.Visible = false;
            // 
            // image_path
            // 
            this.image_path.HeaderText = "image_path";
            this.image_path.Name = "image_path";
            this.image_path.ReadOnly = true;
            this.image_path.Visible = false;
            // 
            // _source
            // 
            this._source.HeaderText = "_source";
            this._source.Name = "_source";
            this._source.ReadOnly = true;
            this._source.Visible = false;
            // 
            // _source_image
            // 
            this._source_image.HeaderText = "_source_image";
            this._source_image.Name = "_source_image";
            this._source_image.ReadOnly = true;
            this._source_image.Visible = false;
            // 
            // btnRemoveModule
            // 
            this.btnRemoveModule.Location = new System.Drawing.Point(109, 479);
            this.btnRemoveModule.Name = "btnRemoveModule";
            this.btnRemoveModule.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveModule.TabIndex = 2;
            this.btnRemoveModule.Text = "Remove";
            this.btnRemoveModule.UseVisualStyleBackColor = true;
            this.btnRemoveModule.Click += new System.EventHandler(this.btnRemoveModule_Click);
            // 
            // btnAddModule
            // 
            this.btnAddModule.Location = new System.Drawing.Point(190, 479);
            this.btnAddModule.Name = "btnAddModule";
            this.btnAddModule.Size = new System.Drawing.Size(75, 23);
            this.btnAddModule.TabIndex = 1;
            this.btnAddModule.Text = "Add";
            this.btnAddModule.UseVisualStyleBackColor = true;
            this.btnAddModule.Click += new System.EventHandler(this.btnAddModule_Click);
            // 
            // tabPages
            // 
            this.tabPages.Controls.Add(this.tcPages);
            this.tabPages.Controls.Add(this.btnPagesDelete);
            this.tabPages.Controls.Add(this.btnPagesAddModule);
            this.tabPages.Controls.Add(this.btnPagesAddPage);
            this.tabPages.Controls.Add(this.tvPages);
            this.tabPages.Location = new System.Drawing.Point(4, 22);
            this.tabPages.Name = "tabPages";
            this.tabPages.Size = new System.Drawing.Size(760, 508);
            this.tabPages.TabIndex = 2;
            this.tabPages.Text = "Pages";
            this.tabPages.UseVisualStyleBackColor = true;
            // 
            // tcPages
            // 
            this.tcPages.Controls.Add(this.tabPage1);
            this.tcPages.Controls.Add(this.tabPage2);
            this.tcPages.Location = new System.Drawing.Point(271, 6);
            this.tcPages.Name = "tcPages";
            this.tcPages.SelectedIndex = 0;
            this.tcPages.Size = new System.Drawing.Size(483, 496);
            this.tcPages.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.dgPageSettings);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.tbPageDescription);
            this.tabPage1.Controls.Add(this.cbPageRequireSSL);
            this.tabPage1.Controls.Add(this.cbPageValidateRequest);
            this.tabPage1.Controls.Add(this.cbPageDisplayInNav);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.tbPageName);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(475, 470);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Page Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 214);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Settings";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // dgPageSettings
            // 
            this.dgPageSettings.AllowUserToAddRows = false;
            this.dgPageSettings.AllowUserToDeleteRows = false;
            this.dgPageSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPageSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.page_setting_name,
            this.page_setting_value});
            this.dgPageSettings.Location = new System.Drawing.Point(6, 230);
            this.dgPageSettings.Name = "dgPageSettings";
            this.dgPageSettings.RowHeadersVisible = false;
            this.dgPageSettings.Size = new System.Drawing.Size(463, 162);
            this.dgPageSettings.TabIndex = 7;
            // 
            // page_setting_name
            // 
            this.page_setting_name.FillWeight = 101.5228F;
            this.page_setting_name.HeaderText = "Name";
            this.page_setting_name.Name = "page_setting_name";
            this.page_setting_name.Width = 135;
            // 
            // page_setting_value
            // 
            this.page_setting_value.FillWeight = 98.47716F;
            this.page_setting_value.HeaderText = "Value";
            this.page_setting_value.Name = "page_setting_value";
            this.page_setting_value.Width = 309;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(68, 104);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Description";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbPageDescription
            // 
            this.tbPageDescription.Location = new System.Drawing.Point(134, 101);
            this.tbPageDescription.Multiline = true;
            this.tbPageDescription.Name = "tbPageDescription";
            this.tbPageDescription.Size = new System.Drawing.Size(335, 106);
            this.tbPageDescription.TabIndex = 5;
            // 
            // cbPageRequireSSL
            // 
            this.cbPageRequireSSL.AutoSize = true;
            this.cbPageRequireSSL.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbPageRequireSSL.Location = new System.Drawing.Point(59, 55);
            this.cbPageRequireSSL.Name = "cbPageRequireSSL";
            this.cbPageRequireSSL.Size = new System.Drawing.Size(89, 17);
            this.cbPageRequireSSL.TabIndex = 4;
            this.cbPageRequireSSL.Text = "Require SSL ";
            this.cbPageRequireSSL.UseVisualStyleBackColor = true;
            // 
            // cbPageValidateRequest
            // 
            this.cbPageValidateRequest.AutoSize = true;
            this.cbPageValidateRequest.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbPageValidateRequest.Location = new System.Drawing.Point(38, 78);
            this.cbPageValidateRequest.Name = "cbPageValidateRequest";
            this.cbPageValidateRequest.Size = new System.Drawing.Size(110, 17);
            this.cbPageValidateRequest.TabIndex = 3;
            this.cbPageValidateRequest.Text = "Validate Request ";
            this.cbPageValidateRequest.UseVisualStyleBackColor = true;
            // 
            // cbPageDisplayInNav
            // 
            this.cbPageDisplayInNav.AutoSize = true;
            this.cbPageDisplayInNav.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbPageDisplayInNav.Location = new System.Drawing.Point(50, 32);
            this.cbPageDisplayInNav.Name = "cbPageDisplayInNav";
            this.cbPageDisplayInNav.Size = new System.Drawing.Size(98, 17);
            this.cbPageDisplayInNav.TabIndex = 2;
            this.cbPageDisplayInNav.Text = "Display In Nav ";
            this.cbPageDisplayInNav.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(65, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Page Name";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbPageName
            // 
            this.tbPageName.Location = new System.Drawing.Point(134, 6);
            this.tbPageName.Name = "tbPageName";
            this.tbPageName.Size = new System.Drawing.Size(155, 20);
            this.tbPageName.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(475, 470);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Module Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnPagesDelete
            // 
            this.btnPagesDelete.Location = new System.Drawing.Point(6, 318);
            this.btnPagesDelete.Name = "btnPagesDelete";
            this.btnPagesDelete.Size = new System.Drawing.Size(75, 23);
            this.btnPagesDelete.TabIndex = 4;
            this.btnPagesDelete.Text = "Delete";
            this.btnPagesDelete.UseVisualStyleBackColor = true;
            // 
            // btnPagesAddModule
            // 
            this.btnPagesAddModule.Location = new System.Drawing.Point(109, 318);
            this.btnPagesAddModule.Name = "btnPagesAddModule";
            this.btnPagesAddModule.Size = new System.Drawing.Size(75, 23);
            this.btnPagesAddModule.TabIndex = 3;
            this.btnPagesAddModule.Text = "Add Module";
            this.btnPagesAddModule.UseVisualStyleBackColor = true;
            // 
            // btnPagesAddPage
            // 
            this.btnPagesAddPage.Location = new System.Drawing.Point(190, 318);
            this.btnPagesAddPage.Name = "btnPagesAddPage";
            this.btnPagesAddPage.Size = new System.Drawing.Size(75, 23);
            this.btnPagesAddPage.TabIndex = 2;
            this.btnPagesAddPage.Text = "Add Page";
            this.btnPagesAddPage.UseVisualStyleBackColor = true;
            this.btnPagesAddPage.Click += new System.EventHandler(this.btnPagesAddPage_Click);
            // 
            // tvPages
            // 
            this.tvPages.HideSelection = false;
            this.tvPages.Location = new System.Drawing.Point(6, 6);
            this.tvPages.Name = "tvPages";
            this.tvPages.Size = new System.Drawing.Size(259, 306);
            this.tvPages.TabIndex = 0;
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mainMenu.Size = new System.Drawing.Size(792, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(97, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox4.Location = new System.Drawing.Point(59, 55);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(89, 17);
            this.checkBox4.TabIndex = 4;
            this.checkBox4.Text = "Require SSL ";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox5.Location = new System.Drawing.Point(38, 78);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(110, 17);
            this.checkBox5.TabIndex = 3;
            this.checkBox5.Text = "Validate Request ";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox6.Location = new System.Drawing.Point(50, 32);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(98, 17);
            this.checkBox6.TabIndex = 2;
            this.checkBox6.Text = "Display In Nav ";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(65, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Page Name";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(134, 6);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(155, 20);
            this.textBox2.TabIndex = 0;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "mainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Build Distribution";
            this.tabControl1.ResumeLayout(false);
            this.tabFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFiles)).EndInit();
            this.tabModules.ResumeLayout(false);
            this.tabModules.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgModules)).EndInit();
            this.tabPages.ResumeLayout(false);
            this.tcPages.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPageSettings)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabFiles;
        private System.Windows.Forms.TabPage tabModules;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.DataGridView dgFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn path;
        private System.Windows.Forms.DataGridViewTextBoxColumn source;
        private System.Windows.Forms.Button btnRemoveFile;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.Button btnRemoveModule;
        private System.Windows.Forms.Button btnAddModule;
        private System.Windows.Forms.DataGridView dgModules;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbModuleName;
        private System.Windows.Forms.TextBox tbModuleSourcePath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbModuleImagePath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbModuleURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbModuleAllowsChildModules;
        private System.Windows.Forms.TextBox tbModuleSourceImagePath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnModulesCancel;
        private System.Windows.Forms.Button btnModulesEdit;
        private System.Windows.Forms.TextBox tbModuleDescription;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn module_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn module_url;
        private System.Windows.Forms.DataGridViewTextBoxColumn module_desc;
        private System.Windows.Forms.DataGridViewTextBoxColumn allows_child_modules;
        private System.Windows.Forms.DataGridViewTextBoxColumn image_path;
        private System.Windows.Forms.DataGridViewTextBoxColumn _source;
        private System.Windows.Forms.DataGridViewTextBoxColumn _source_image;
        private System.Windows.Forms.TabPage tabPages;
        private System.Windows.Forms.TabControl tcPages;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnPagesDelete;
        private System.Windows.Forms.Button btnPagesAddModule;
        private System.Windows.Forms.Button btnPagesAddPage;
        private System.Windows.Forms.TreeView tvPages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPageName;
        private System.Windows.Forms.CheckBox cbPageRequireSSL;
        private System.Windows.Forms.CheckBox cbPageValidateRequest;
        private System.Windows.Forms.CheckBox cbPageDisplayInNav;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbPageDescription;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridView dgPageSettings;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_setting_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn page_setting_value;
    }
}