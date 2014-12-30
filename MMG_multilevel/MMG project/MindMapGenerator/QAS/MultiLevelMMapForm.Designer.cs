namespace MMG
{
    partial class MultiLevelMMapForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.stopLayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rerunLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setLayoutToAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seAlltLocationsFromToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(721, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // stopLayToolStripMenuItem
            // 
            this.stopLayToolStripMenuItem.Name = "stopLayToolStripMenuItem";
            this.stopLayToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            this.stopLayToolStripMenuItem.Text = "Stop Layout";
            // 
            // rerunLayoutToolStripMenuItem
            // 
            this.rerunLayoutToolStripMenuItem.Name = "rerunLayoutToolStripMenuItem";
            this.rerunLayoutToolStripMenuItem.Size = new System.Drawing.Size(89, 20);
            this.rerunLayoutToolStripMenuItem.Text = "Rerun Layout";
            // 
            // initLayoutToolStripMenuItem
            // 
            this.initLayoutToolStripMenuItem.Name = "initLayoutToolStripMenuItem";
            this.initLayoutToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.initLayoutToolStripMenuItem.Text = "InitLayout";
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
            // 
            // MultiLevelMMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 359);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.Name = "MultiLevelMMapForm";
            this.Text = "MultiLevelMMap";
            this.Load += new System.EventHandler(this.MultiLevelMMapForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem stopLayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rerunLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setLayoutToAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem seAlltLocationsFromToolStripMenuItem;
    }
}