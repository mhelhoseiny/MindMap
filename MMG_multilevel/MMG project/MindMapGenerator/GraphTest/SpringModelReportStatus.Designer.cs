namespace GraphTest
{
    partial class SpringModelReportStatus
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
            this.txt_Report = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txt_Report
            // 
            this.txt_Report.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_Report.Location = new System.Drawing.Point(0, 0);
            this.txt_Report.Name = "txt_Report";
            this.txt_Report.Size = new System.Drawing.Size(609, 394);
            this.txt_Report.TabIndex = 1;
            this.txt_Report.Text = "";
            // 
            // SpringModelReportStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 394);
            this.Controls.Add(this.txt_Report);
            this.Name = "SpringModelReportStatus";
            this.Text = "SpringModelReportStatus";
            this.Load += new System.EventHandler(this.SpringModelReportStatus_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txt_Report;
    }
}