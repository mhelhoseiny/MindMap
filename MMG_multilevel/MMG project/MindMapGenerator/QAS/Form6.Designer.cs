namespace MMG
{
    partial class Form6
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
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.stopLayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rerunLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setLayoutToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seAlltLocationsFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar1.Location = new System.Drawing.Point(0, 393);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(714, 17);
            this.hScrollBar1.TabIndex = 0;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar1.Location = new System.Drawing.Point(697, 24);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 369);
            this.vScrollBar1.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stopLayToolStripMenuItem,
            this.rerunLayoutToolStripMenuItem,
            this.initLayoutToolStripMenuItem,
            this.setLayoutToAllToolStripMenuItem,
            this.seAlltLocationsFromToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(714, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // stopLayToolStripMenuItem
            // 
            this.stopLayToolStripMenuItem.Name = "stopLayToolStripMenuItem";
            this.stopLayToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            this.stopLayToolStripMenuItem.Text = "Stop Layout";
            this.stopLayToolStripMenuItem.Click += new System.EventHandler(this.stopLayToolStripMenuItem_Click);
            // 
            // rerunLayoutToolStripMenuItem
            // 
            this.rerunLayoutToolStripMenuItem.Name = "rerunLayoutToolStripMenuItem";
            this.rerunLayoutToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.rerunLayoutToolStripMenuItem.Text = "Rerun Layout";
            this.rerunLayoutToolStripMenuItem.Click += new System.EventHandler(this.rerunLayoutToolStripMenuItem_Click);
            // 
            // initLayoutToolStripMenuItem
            // 
            this.initLayoutToolStripMenuItem.Name = "initLayoutToolStripMenuItem";
            this.initLayoutToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.initLayoutToolStripMenuItem.Text = "InitLayout";
            this.initLayoutToolStripMenuItem.Click += new System.EventHandler(this.initLayoutToolStripMenuItem_Click);
            // 
            // setLayoutToAllToolStripMenuItem
            // 
            this.setLayoutToAllToolStripMenuItem.Name = "setLayoutToAllToolStripMenuItem";
            this.setLayoutToAllToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.setLayoutToAllToolStripMenuItem.Text = "SetLayoutToAll";
            this.setLayoutToAllToolStripMenuItem.Click += new System.EventHandler(this.setLayoutToAllToolStripMenuItem_Click);
            // 
            // seAlltLocationsFromToolStripMenuItem
            // 
            this.seAlltLocationsFromToolStripMenuItem.Name = "seAlltLocationsFromToolStripMenuItem";
            this.seAlltLocationsFromToolStripMenuItem.Size = new System.Drawing.Size(128, 20);
            this.seAlltLocationsFromToolStripMenuItem.Text = "SeAlltLocationsFrom";
            this.seAlltLocationsFromToolStripMenuItem.Click += new System.EventHandler(this.seAlltLocationsFromToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(571, 1);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(100, 20);
            this.txtSize.TabIndex = 4;
            // 
            // Form6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(714, 410);
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form6";
            this.Text = "Form6";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form6_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem stopLayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rerunLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setLayoutToAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem seAlltLocationsFromToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtSize;


    }
}