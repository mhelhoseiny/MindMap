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
            this.richTextBox1.Location = new System.Drawing.Point(462, 68);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(391, 435);
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
            // frmOntologyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 627);
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
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnLoadCombo);
            this.Controls.Add(this.cmbConceptNames);
            this.Name = "frmOntologyEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "بسم الله الرحمن الرحيم";
            this.Load += new System.EventHandler(this.Form1_Load);
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
	}
}

