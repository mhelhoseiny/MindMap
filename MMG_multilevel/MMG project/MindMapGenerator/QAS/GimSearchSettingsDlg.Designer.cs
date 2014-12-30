namespace MMG
{
    partial class GimSearchSettingsDlg
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbImSize = new System.Windows.Forms.ComboBox();
            this.cmbImType = new System.Windows.Forms.ComboBox();
            this.cmb_Colorization = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Image Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Image Colorization";
            // 
            // cmbImSize
            // 
            this.cmbImSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImSize.FormattingEnabled = true;
            this.cmbImSize.Items.AddRange(new object[] {
            "All",
            "Huge",
            "Icon",
            "Large",
            "Medium",
            "Small",
            "Xlarge",
            "Xxlarge"});
            this.cmbImSize.Location = new System.Drawing.Point(182, 53);
            this.cmbImSize.Name = "cmbImSize";
            this.cmbImSize.Size = new System.Drawing.Size(121, 21);
            this.cmbImSize.TabIndex = 2;
            // 
            // cmbImType
            // 
            this.cmbImType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImType.FormattingEnabled = true;
            this.cmbImType.Items.AddRange(new object[] {
            "All",
            "Clipart",
            "Face",
            "LineArt",
            "News",
            "Photo"});
            this.cmbImType.Location = new System.Drawing.Point(182, 97);
            this.cmbImType.Name = "cmbImType";
            this.cmbImType.Size = new System.Drawing.Size(121, 21);
            this.cmbImType.TabIndex = 2;
            // 
            // cmb_Colorization
            // 
            this.cmb_Colorization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Colorization.FormattingEnabled = true;
            this.cmb_Colorization.Items.AddRange(new object[] {
            "All",
            "Color",
            "Gray"});
            this.cmb_Colorization.Location = new System.Drawing.Point(182, 134);
            this.cmb_Colorization.Name = "cmb_Colorization";
            this.cmb_Colorization.Size = new System.Drawing.Size(121, 21);
            this.cmb_Colorization.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(140, 206);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Count";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(182, 27);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // GimSearchSettingsDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 241);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmb_Colorization);
            this.Controls.Add(this.cmbImType);
            this.Controls.Add(this.cmbImSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GimSearchSettingsDlg";
            this.Text = "GimSearchSettingsDlg";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbImSize;
        private System.Windows.Forms.ComboBox cmbImType;
        private System.Windows.Forms.ComboBox cmb_Colorization;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}