namespace OntologyConceptsEditor
{
	partial class frmOntologyEditor
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
            this.cmbConceptNames = new System.Windows.Forms.ComboBox();
            this.btnLoadCombo = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnFillTreeView = new System.Windows.Forms.Button();
            this.btnGetPath = new System.Windows.Forms.Button();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.cmbFirstConcept = new System.Windows.Forms.ComboBox();
            this.cmbSecondConcept = new System.Windows.Forms.ComboBox();
            this.btnErrors = new System.Windows.Forms.Button();
            this.txtFullPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFillerName = new System.Windows.Forms.TextBox();
            this.txtFillerValue = new System.Windows.Forms.TextBox();
            this.btnAddFiller = new System.Windows.Forms.Button();
            this.lblCurrentConcept = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNewConceptName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbConceptNames
            // 
            this.cmbConceptNames.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbConceptNames.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbConceptNames.FormattingEnabled = true;
            this.cmbConceptNames.Location = new System.Drawing.Point(462, 41);
            this.cmbConceptNames.Name = "cmbConceptNames";
            this.cmbConceptNames.Size = new System.Drawing.Size(391, 21);
            this.cmbConceptNames.TabIndex = 0;
            this.cmbConceptNames.SelectedIndexChanged += new System.EventHandler(this.cmbConceptNames_SelectedIndexChanged);
            // 
            // btnLoadCombo
            // 
            this.btnLoadCombo.Location = new System.Drawing.Point(684, 592);
            this.btnLoadCombo.Name = "btnLoadCombo";
            this.btnLoadCombo.Size = new System.Drawing.Size(99, 23);
            this.btnLoadCombo.TabIndex = 1;
            this.btnLoadCombo.Text = "Load Ontology";
            this.btnLoadCombo.UseVisualStyleBackColor = true;
            this.btnLoadCombo.Click += new System.EventHandler(this.btnLoadCombo_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(377, 296);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(25, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.PathSeparator = "/";
            this.treeView1.Size = new System.Drawing.Size(410, 327);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // btnFillTreeView
            // 
            this.btnFillTreeView.Location = new System.Drawing.Point(789, 592);
            this.btnFillTreeView.Name = "btnFillTreeView";
            this.btnFillTreeView.Size = new System.Drawing.Size(65, 23);
            this.btnFillTreeView.TabIndex = 6;
            this.btnFillTreeView.Text = "Fill Tree";
            this.btnFillTreeView.UseVisualStyleBackColor = true;
            this.btnFillTreeView.Click += new System.EventHandler(this.btnFillTreeView_Click);
            // 
            // btnGetPath
            // 
            this.btnGetPath.Location = new System.Drawing.Point(463, 563);
            this.btnGetPath.Name = "btnGetPath";
            this.btnGetPath.Size = new System.Drawing.Size(391, 23);
            this.btnGetPath.TabIndex = 9;
            this.btnGetPath.Text = "Get Path";
            this.btnGetPath.UseVisualStyleBackColor = true;
            this.btnGetPath.Click += new System.EventHandler(this.btnGetPath_Click);
            // 
            // treeView2
            // 
            this.treeView2.Location = new System.Drawing.Point(25, 370);
            this.treeView2.Name = "treeView2";
            this.treeView2.PathSeparator = "/";
            this.treeView2.Size = new System.Drawing.Size(410, 245);
            this.treeView2.TabIndex = 10;
            this.treeView2.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterSelect);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(631, 592);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(47, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(462, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(391, 23);
            this.btnLoad.TabIndex = 12;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // cmbFirstConcept
            // 
            this.cmbFirstConcept.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbFirstConcept.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbFirstConcept.FormattingEnabled = true;
            this.cmbFirstConcept.Location = new System.Drawing.Point(463, 509);
            this.cmbFirstConcept.Name = "cmbFirstConcept";
            this.cmbFirstConcept.Size = new System.Drawing.Size(391, 21);
            this.cmbFirstConcept.TabIndex = 13;
            // 
            // cmbSecondConcept
            // 
            this.cmbSecondConcept.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbSecondConcept.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSecondConcept.FormattingEnabled = true;
            this.cmbSecondConcept.Location = new System.Drawing.Point(463, 536);
            this.cmbSecondConcept.Name = "cmbSecondConcept";
            this.cmbSecondConcept.Size = new System.Drawing.Size(391, 21);
            this.cmbSecondConcept.TabIndex = 14;
            // 
            // btnErrors
            // 
            this.btnErrors.Location = new System.Drawing.Point(526, 592);
            this.btnErrors.Name = "btnErrors";
            this.btnErrors.Size = new System.Drawing.Size(99, 23);
            this.btnErrors.TabIndex = 15;
            this.btnErrors.Text = "Find onto Errors";
            this.btnErrors.UseVisualStyleBackColor = true;
            this.btnErrors.Click += new System.EventHandler(this.btnErrors_Click);
            // 
            // txtFullPath
            // 
            this.txtFullPath.Location = new System.Drawing.Point(73, 345);
            this.txtFullPath.Name = "txtFullPath";
            this.txtFullPath.Size = new System.Drawing.Size(362, 20);
            this.txtFullPath.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 348);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Full Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(460, 418);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Filler Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(461, 444);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Filler Value";
            // 
            // txtFillerName
            // 
            this.txtFillerName.Location = new System.Drawing.Point(526, 415);
            this.txtFillerName.Name = "txtFillerName";
            this.txtFillerName.Size = new System.Drawing.Size(257, 20);
            this.txtFillerName.TabIndex = 20;
            // 
            // txtFillerValue
            // 
            this.txtFillerValue.Location = new System.Drawing.Point(526, 441);
            this.txtFillerValue.Multiline = true;
            this.txtFillerValue.Name = "txtFillerValue";
            this.txtFillerValue.Size = new System.Drawing.Size(257, 53);
            this.txtFillerValue.TabIndex = 21;
            // 
            // btnAddFiller
            // 
            this.btnAddFiller.Location = new System.Drawing.Point(790, 415);
            this.btnAddFiller.Name = "btnAddFiller";
            this.btnAddFiller.Size = new System.Drawing.Size(63, 79);
            this.btnAddFiller.TabIndex = 22;
            this.btnAddFiller.Text = "Add Filler";
            this.btnAddFiller.UseVisualStyleBackColor = true;
            this.btnAddFiller.Click += new System.EventHandler(this.btnAddFiller_Click);
            // 
            // lblCurrentConcept
            // 
            this.lblCurrentConcept.AutoSize = true;
            this.lblCurrentConcept.Location = new System.Drawing.Point(461, 65);
            this.lblCurrentConcept.Name = "lblCurrentConcept";
            this.lblCurrentConcept.Size = new System.Drawing.Size(122, 13);
            this.lblCurrentConcept.TabIndex = 23;
            this.lblCurrentConcept.Text = "Current Concept  --->   ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(462, 81);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(391, 328);
            this.tabControl1.TabIndex = 24;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(383, 302);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Concept\'s Fillers";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.txtNewConceptName);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(383, 302);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "New Concept Establishment";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "New Concept Name";
            // 
            // txtNewConceptName
            // 
            this.txtNewConceptName.Location = new System.Drawing.Point(106, 6);
            this.txtNewConceptName.Name = "txtNewConceptName";
            this.txtNewConceptName.Size = new System.Drawing.Size(271, 20);
            this.txtNewConceptName.TabIndex = 26;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 265);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(377, 34);
            this.button1.TabIndex = 25;
            this.button1.Text = "Add Filler";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
            this.dataGridView1.Location = new System.Drawing.Point(3, 30);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(377, 232);
            this.dataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Filler Name";
            this.Column1.Name = "Column1";
            this.Column1.Width = 120;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Filler Value";
            this.Column2.Name = "Column2";
            this.Column2.Width = 200;
            // 
            // frmOntologyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 627);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblCurrentConcept);
            this.Controls.Add(this.btnAddFiller);
            this.Controls.Add(this.txtFillerValue);
            this.Controls.Add(this.txtFillerName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFullPath);
            this.Controls.Add(this.btnErrors);
            this.Controls.Add(this.cmbSecondConcept);
            this.Controls.Add(this.cmbFirstConcept);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.treeView2);
            this.Controls.Add(this.btnGetPath);
            this.Controls.Add(this.btnFillTreeView);
            this.Controls.Add(this.btnLoadCombo);
            this.Controls.Add(this.cmbConceptNames);
            this.Name = "frmOntologyEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "بسم الله الرحمن الرحيم";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbConceptNames;
		private System.Windows.Forms.Button btnLoadCombo;
		private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btnFillTreeView;
        private System.Windows.Forms.Button btnGetPath;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ComboBox cmbFirstConcept;
        private System.Windows.Forms.ComboBox cmbSecondConcept;
        private System.Windows.Forms.Button btnErrors;
        private System.Windows.Forms.TextBox txtFullPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFillerName;
        private System.Windows.Forms.TextBox txtFillerValue;
        private System.Windows.Forms.Button btnAddFiller;
        private System.Windows.Forms.Label lblCurrentConcept;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNewConceptName;
        private System.Windows.Forms.Button button1;
	}
}

