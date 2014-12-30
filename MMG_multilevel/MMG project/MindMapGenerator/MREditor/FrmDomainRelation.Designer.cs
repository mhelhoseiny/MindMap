namespace MapperTool
{
	partial class FrmDomainRelation
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
            System.Windows.Forms.Label nounFrames;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label5;
            this.cmbverbFrames1 = new System.Windows.Forms.ComboBox();
            this.cmbVerbFrames2 = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbDomainRelation = new System.Windows.Forms.ComboBox();
            nounFrames = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nounFrames
            // 
            nounFrames.AutoSize = true;
            nounFrames.Location = new System.Drawing.Point(6, 43);
            nounFrames.Name = "nounFrames";
            nounFrames.Size = new System.Drawing.Size(61, 13);
            nounFrames.TabIndex = 29;
            nounFrames.Text = "Verb Frame";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 70);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(61, 13);
            label1.TabIndex = 31;
            label1.Text = "Verb Frame";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(6, 15);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(85, 13);
            label5.TabIndex = 25;
            label5.Text = "Domain Relation";
            // 
            // cmbverbFrames1
            // 
            this.cmbverbFrames1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbverbFrames1.FormattingEnabled = true;
            this.cmbverbFrames1.Location = new System.Drawing.Point(105, 40);
            this.cmbverbFrames1.Name = "cmbverbFrames1";
            this.cmbverbFrames1.Size = new System.Drawing.Size(180, 21);
            this.cmbverbFrames1.TabIndex = 1;
            // 
            // cmbVerbFrames2
            // 
            this.cmbVerbFrames2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVerbFrames2.FormattingEnabled = true;
            this.cmbVerbFrames2.Location = new System.Drawing.Point(105, 67);
            this.cmbVerbFrames2.Name = "cmbVerbFrames2";
            this.cmbVerbFrames2.Size = new System.Drawing.Size(180, 21);
            this.cmbVerbFrames2.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(9, 94);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(210, 94);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cmbDomainRelation
            // 
            this.cmbDomainRelation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDomainRelation.FormattingEnabled = true;
            this.cmbDomainRelation.Location = new System.Drawing.Point(105, 12);
            this.cmbDomainRelation.Name = "cmbDomainRelation";
            this.cmbDomainRelation.Size = new System.Drawing.Size(180, 21);
            this.cmbDomainRelation.TabIndex = 0;
            // 
            // FrmDomainRelation
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 123);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(label1);
            this.Controls.Add(this.cmbVerbFrames2);
            this.Controls.Add(label5);
            this.Controls.Add(nounFrames);
            this.Controls.Add(this.cmbverbFrames1);
            this.Controls.Add(this.cmbDomainRelation);
            this.Name = "FrmDomainRelation";
            this.Text = "Domain relation";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbverbFrames1;
		private System.Windows.Forms.ComboBox cmbVerbFrames2;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ComboBox cmbDomainRelation;
	}
}