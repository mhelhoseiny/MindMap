namespace MapperTool
{
	partial class frmMREditor
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
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			this.pnlDrawing = new System.Windows.Forms.Panel();
			this.btnAddNounFrame = new System.Windows.Forms.Button();
			this.cmbNounFrames = new System.Windows.Forms.ComboBox();
			this.cmbVerbFrame = new System.Windows.Forms.ComboBox();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.btnDeleteNounFrame = new System.Windows.Forms.Button();
			this.btnEditNounFrame = new System.Windows.Forms.Button();
			this.btnEditVerbFrame = new System.Windows.Forms.Button();
			this.btnDeleteVerbFrame = new System.Windows.Forms.Button();
			this.btnAddVerbFrame = new System.Windows.Forms.Button();
			this.btnAddCaseRole = new System.Windows.Forms.Button();
			this.btnAddDomainRelation = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnLoad = new System.Windows.Forms.Button();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(12, 103);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(64, 13);
			label4.TabIndex = 7;
			label4.Text = "verbFrames";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(9, 15);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(66, 13);
			label3.TabIndex = 15;
			label3.Text = "nounFrames";
			// 
			// pnlDrawing
			// 
			this.pnlDrawing.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlDrawing.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlDrawing.Location = new System.Drawing.Point(258, 0);
			this.pnlDrawing.Name = "pnlDrawing";
			this.pnlDrawing.Size = new System.Drawing.Size(667, 655);
			this.pnlDrawing.TabIndex = 0;
			// 
			// btnAddNounFrame
			// 
			this.btnAddNounFrame.Location = new System.Drawing.Point(12, 50);
			this.btnAddNounFrame.Name = "btnAddNounFrame";
			this.btnAddNounFrame.Size = new System.Drawing.Size(70, 23);
			this.btnAddNounFrame.TabIndex = 0;
			this.btnAddNounFrame.Text = "Add";
			this.btnAddNounFrame.UseVisualStyleBackColor = true;
			this.btnAddNounFrame.Click += new System.EventHandler(this.btnAddNounFrame_Click);
			// 
			// cmbNounFrames
			// 
			this.cmbNounFrames.FormattingEnabled = true;
			this.cmbNounFrames.Location = new System.Drawing.Point(81, 12);
			this.cmbNounFrames.Name = "cmbNounFrames";
			this.cmbNounFrames.Size = new System.Drawing.Size(171, 21);
			this.cmbNounFrames.TabIndex = 2;
			// 
			// cmbVerbFrame
			// 
			this.cmbVerbFrame.FormattingEnabled = true;
			this.cmbVerbFrame.Location = new System.Drawing.Point(81, 100);
			this.cmbVerbFrame.Name = "cmbVerbFrame";
			this.cmbVerbFrame.Size = new System.Drawing.Size(171, 21);
			this.cmbVerbFrame.TabIndex = 3;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// btnDeleteNounFrame
			// 
			this.btnDeleteNounFrame.Location = new System.Drawing.Point(182, 50);
			this.btnDeleteNounFrame.Name = "btnDeleteNounFrame";
			this.btnDeleteNounFrame.Size = new System.Drawing.Size(70, 23);
			this.btnDeleteNounFrame.TabIndex = 27;
			this.btnDeleteNounFrame.Text = "Delete";
			this.btnDeleteNounFrame.UseVisualStyleBackColor = true;
			this.btnDeleteNounFrame.Click += new System.EventHandler(this.btnDeleteNounFrame_Click);
			// 
			// btnEditNounFrame
			// 
			this.btnEditNounFrame.Location = new System.Drawing.Point(97, 50);
			this.btnEditNounFrame.Name = "btnEditNounFrame";
			this.btnEditNounFrame.Size = new System.Drawing.Size(70, 23);
			this.btnEditNounFrame.TabIndex = 28;
			this.btnEditNounFrame.Text = "Edit";
			this.btnEditNounFrame.UseVisualStyleBackColor = true;
			this.btnEditNounFrame.Click += new System.EventHandler(this.btnEditNounFrame_Click);
			// 
			// btnEditVerbFrame
			// 
			this.btnEditVerbFrame.Location = new System.Drawing.Point(97, 136);
			this.btnEditVerbFrame.Name = "btnEditVerbFrame";
			this.btnEditVerbFrame.Size = new System.Drawing.Size(70, 23);
			this.btnEditVerbFrame.TabIndex = 31;
			this.btnEditVerbFrame.Text = "Edit";
			this.btnEditVerbFrame.UseVisualStyleBackColor = true;
			this.btnEditVerbFrame.Click += new System.EventHandler(this.btnEditVerbFrame_Click);
			// 
			// btnDeleteVerbFrame
			// 
			this.btnDeleteVerbFrame.Location = new System.Drawing.Point(182, 136);
			this.btnDeleteVerbFrame.Name = "btnDeleteVerbFrame";
			this.btnDeleteVerbFrame.Size = new System.Drawing.Size(70, 23);
			this.btnDeleteVerbFrame.TabIndex = 30;
			this.btnDeleteVerbFrame.Text = "Delete";
			this.btnDeleteVerbFrame.UseVisualStyleBackColor = true;
			this.btnDeleteVerbFrame.Click += new System.EventHandler(this.btnDeleteVerbFrame_Click);
			// 
			// btnAddVerbFrame
			// 
			this.btnAddVerbFrame.Location = new System.Drawing.Point(12, 136);
			this.btnAddVerbFrame.Name = "btnAddVerbFrame";
			this.btnAddVerbFrame.Size = new System.Drawing.Size(70, 23);
			this.btnAddVerbFrame.TabIndex = 29;
			this.btnAddVerbFrame.Text = "Add";
			this.btnAddVerbFrame.UseVisualStyleBackColor = true;
			this.btnAddVerbFrame.Click += new System.EventHandler(this.btnAddVerbFrame_Click);
			// 
			// btnAddCaseRole
			// 
			this.btnAddCaseRole.Location = new System.Drawing.Point(15, 222);
			this.btnAddCaseRole.Name = "btnAddCaseRole";
			this.btnAddCaseRole.Size = new System.Drawing.Size(110, 23);
			this.btnAddCaseRole.TabIndex = 32;
			this.btnAddCaseRole.Text = "Add case role";
			this.btnAddCaseRole.UseVisualStyleBackColor = true;
			this.btnAddCaseRole.Click += new System.EventHandler(this.btnAddCaseRole_Click);
			// 
			// btnAddDomainRelation
			// 
			this.btnAddDomainRelation.Location = new System.Drawing.Point(15, 262);
			this.btnAddDomainRelation.Name = "btnAddDomainRelation";
			this.btnAddDomainRelation.Size = new System.Drawing.Size(110, 23);
			this.btnAddDomainRelation.TabIndex = 33;
			this.btnAddDomainRelation.Text = "Add domain relation";
			this.btnAddDomainRelation.UseVisualStyleBackColor = true;
			this.btnAddDomainRelation.Click += new System.EventHandler(this.btnAddDomainRelation_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(15, 604);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 34;
			this.btnSave.Text = "save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(145, 604);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(75, 23);
			this.btnLoad.TabIndex = 35;
			this.btnLoad.Text = "Load";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// frmMREditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(925, 655);
			this.Controls.Add(this.btnLoad);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnAddDomainRelation);
			this.Controls.Add(this.btnAddCaseRole);
			this.Controls.Add(this.btnEditVerbFrame);
			this.Controls.Add(this.btnDeleteVerbFrame);
			this.Controls.Add(this.btnAddVerbFrame);
			this.Controls.Add(this.btnEditNounFrame);
			this.Controls.Add(this.btnDeleteNounFrame);
			this.Controls.Add(label3);
			this.Controls.Add(label4);
			this.Controls.Add(this.btnAddNounFrame);
			this.Controls.Add(this.cmbVerbFrame);
			this.Controls.Add(this.cmbNounFrames);
			this.Controls.Add(this.pnlDrawing);
			this.Name = "frmMREditor";
			this.Text = "MR editor";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel pnlDrawing;
		private System.Windows.Forms.Button btnAddNounFrame;
		private System.Windows.Forms.ComboBox cmbNounFrames;
		private System.Windows.Forms.ComboBox cmbVerbFrame;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button btnDeleteNounFrame;
		private System.Windows.Forms.Button btnEditNounFrame;
		private System.Windows.Forms.Button btnEditVerbFrame;
		private System.Windows.Forms.Button btnDeleteVerbFrame;
		private System.Windows.Forms.Button btnAddVerbFrame;
		private System.Windows.Forms.Button btnAddCaseRole;
		private System.Windows.Forms.Button btnAddDomainRelation;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
	}
}

