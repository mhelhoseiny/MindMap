namespace MapperTool
{
	partial class FrmAddEditFrame
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
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			this.cmbConcepts = new System.Windows.Forms.ComboBox();
			this.txtFrameText = new System.Windows.Forms.TextBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(4, 40);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(47, 13);
			label2.TabIndex = 18;
			label2.Text = "Concept";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(4, 10);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(29, 13);
			label1.TabIndex = 17;
			label1.Text = "Text";
			// 
			// cmbConcepts
			// 
			this.cmbConcepts.FormattingEnabled = true;
			this.cmbConcepts.Location = new System.Drawing.Point(119, 37);
			this.cmbConcepts.Name = "cmbConcepts";
			this.cmbConcepts.Size = new System.Drawing.Size(190, 21);
			this.cmbConcepts.TabIndex = 1;
			// 
			// txtFrameText
			// 
			this.txtFrameText.Location = new System.Drawing.Point(119, 10);
			this.txtFrameText.Name = "txtFrameText";
			this.txtFrameText.Size = new System.Drawing.Size(190, 20);
			this.txtFrameText.TabIndex = 0;
			// 
			// btnOk
			// 
			this.btnOk.Location = new System.Drawing.Point(7, 70);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(70, 23);
			this.btnOk.TabIndex = 2;
			this.btnOk.Text = "ok";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(239, 70);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(70, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// FrmAddEditFrame
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(322, 105);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.cmbConcepts);
			this.Controls.Add(this.txtFrameText);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(label2);
			this.Controls.Add(label1);
			this.Name = "FrmAddEditFrame";
			this.Text = "FrmAddNounFrame";
			this.Load += new System.EventHandler(this.FrmAddEditFrame_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbConcepts;
		private System.Windows.Forms.TextBox txtFrameText;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
	}
}