namespace MapperTool
{
	partial class FrmCaseRoleRelation
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
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label nounFrames;
			System.Windows.Forms.Label label1;
			this.cmbNounFrames = new System.Windows.Forms.ComboBox();
			this.cmbCaseRoles = new System.Windows.Forms.ComboBox();
			this.cmbVerbFrames = new System.Windows.Forms.ComboBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			label5 = new System.Windows.Forms.Label();
			nounFrames = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(6, 14);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(57, 13);
			label5.TabIndex = 25;
			label5.Text = "CaseRoles";
			// 
			// nounFrames
			// 
			nounFrames.AutoSize = true;
			nounFrames.Location = new System.Drawing.Point(6, 41);
			nounFrames.Name = "nounFrames";
			nounFrames.Size = new System.Drawing.Size(66, 13);
			nounFrames.TabIndex = 29;
			nounFrames.Text = "nounFrames";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 68);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(64, 13);
			label1.TabIndex = 31;
			label1.Text = "verbFrames";
			// 
			// cmbNounFrames
			// 
			this.cmbNounFrames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbNounFrames.FormattingEnabled = true;
			this.cmbNounFrames.Location = new System.Drawing.Point(95, 39);
			this.cmbNounFrames.Name = "cmbNounFrames";
			this.cmbNounFrames.Size = new System.Drawing.Size(190, 21);
			this.cmbNounFrames.TabIndex = 1;
			// 
			// cmbCaseRoles
			// 
			this.cmbCaseRoles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbCaseRoles.FormattingEnabled = true;
			this.cmbCaseRoles.Location = new System.Drawing.Point(95, 12);
			this.cmbCaseRoles.Name = "cmbCaseRoles";
			this.cmbCaseRoles.Size = new System.Drawing.Size(190, 21);
			this.cmbCaseRoles.TabIndex = 0;
			// 
			// cmbVerbFrames
			// 
			this.cmbVerbFrames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbVerbFrames.FormattingEnabled = true;
			this.cmbVerbFrames.Location = new System.Drawing.Point(95, 66);
			this.cmbVerbFrames.Name = "cmbVerbFrames";
			this.cmbVerbFrames.Size = new System.Drawing.Size(190, 21);
			this.cmbVerbFrames.TabIndex = 2;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(9, 93);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 3;
			this.btnOk.Text = "Ok";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(210, 93);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// FrmCaseRoleRelation
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(293, 126);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(label1);
			this.Controls.Add(this.cmbVerbFrames);
			this.Controls.Add(label5);
			this.Controls.Add(nounFrames);
			this.Controls.Add(this.cmbNounFrames);
			this.Controls.Add(this.cmbCaseRoles);
			this.Name = "FrmCaseRoleRelation";
			this.Text = "Case role relation";
			this.Load += new System.EventHandler(this.FrmCaseRoleRelation_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbNounFrames;
		private System.Windows.Forms.ComboBox cmbCaseRoles;
		private System.Windows.Forms.ComboBox cmbVerbFrames;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
	}
}