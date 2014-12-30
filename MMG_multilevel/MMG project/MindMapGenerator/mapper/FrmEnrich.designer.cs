namespace WordologyManager
{
    partial class FrmEnrich
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
			this.button1 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.buttonConceptSenses = new System.Windows.Forms.Button();
			this.comboBoxConcepts = new System.Windows.Forms.ComboBox();
			this.comboBoxPos = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonMeaning = new System.Windows.Forms.Button();
			this.lstSenses = new System.Windows.Forms.ListBox();
			this.textBoxWord = new System.Windows.Forms.TextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.regenerateMapperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testMapperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.countOfMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnGetConcepts = new System.Windows.Forms.Button();
			this.btnUniqueMap = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(386, 417);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Manual Map";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Ontology Concept";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(158, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(87, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Part Of Speach :";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(570, 417);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "Save";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnUniqueMap);
			this.groupBox1.Controls.Add(this.listBox2);
			this.groupBox1.Controls.Add(this.buttonConceptSenses);
			this.groupBox1.Controls.Add(this.comboBoxConcepts);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(12, 37);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(437, 359);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Ontology";
			// 
			// listBox2
			// 
			this.listBox2.FormattingEnabled = true;
			this.listBox2.HorizontalScrollbar = true;
			this.listBox2.Location = new System.Drawing.Point(18, 78);
			this.listBox2.Name = "listBox2";
			this.listBox2.ScrollAlwaysVisible = true;
			this.listBox2.Size = new System.Drawing.Size(406, 251);
			this.listBox2.TabIndex = 12;
			// 
			// buttonConceptSenses
			// 
			this.buttonConceptSenses.Location = new System.Drawing.Point(261, 32);
			this.buttonConceptSenses.Name = "buttonConceptSenses";
			this.buttonConceptSenses.Size = new System.Drawing.Size(75, 23);
			this.buttonConceptSenses.TabIndex = 11;
			this.buttonConceptSenses.Text = "Senses";
			this.buttonConceptSenses.UseVisualStyleBackColor = true;
			this.buttonConceptSenses.Click += new System.EventHandler(this.buttonConceptSenses_Click);
			// 
			// comboBoxConcepts
			// 
			this.comboBoxConcepts.FormattingEnabled = true;
			this.comboBoxConcepts.Location = new System.Drawing.Point(125, 31);
			this.comboBoxConcepts.Name = "comboBoxConcepts";
			this.comboBoxConcepts.Size = new System.Drawing.Size(121, 21);
			this.comboBoxConcepts.TabIndex = 4;
			// 
			// comboBoxPos
			// 
			this.comboBoxPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPos.FormattingEnabled = true;
			this.comboBoxPos.Items.AddRange(new object[] {
            "noun",
            "verb",
            "adj",
            "adv"});
			this.comboBoxPos.Location = new System.Drawing.Point(156, 33);
			this.comboBoxPos.Name = "comboBoxPos";
			this.comboBoxPos.Size = new System.Drawing.Size(121, 21);
			this.comboBoxPos.TabIndex = 6;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboBoxPos);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.btnGetConcepts);
			this.groupBox2.Controls.Add(this.buttonMeaning);
			this.groupBox2.Controls.Add(this.lstSenses);
			this.groupBox2.Controls.Add(this.textBoxWord);
			this.groupBox2.Location = new System.Drawing.Point(463, 38);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(443, 358);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Word Net";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(14, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 13);
			this.label4.TabIndex = 11;
			this.label4.Text = "Word :";
			// 
			// buttonMeaning
			// 
			this.buttonMeaning.Location = new System.Drawing.Point(283, 31);
			this.buttonMeaning.Name = "buttonMeaning";
			this.buttonMeaning.Size = new System.Drawing.Size(75, 23);
			this.buttonMeaning.TabIndex = 10;
			this.buttonMeaning.Text = "Meaning";
			this.buttonMeaning.UseVisualStyleBackColor = true;
			this.buttonMeaning.Click += new System.EventHandler(this.buttonMeaning_Click);
			// 
			// lstSenses
			// 
			this.lstSenses.FormattingEnabled = true;
			this.lstSenses.HorizontalScrollbar = true;
			this.lstSenses.Location = new System.Drawing.Point(17, 78);
			this.lstSenses.Name = "lstSenses";
			this.lstSenses.Size = new System.Drawing.Size(406, 251);
			this.lstSenses.TabIndex = 8;
			// 
			// textBoxWord
			// 
			this.textBoxWord.Location = new System.Drawing.Point(17, 33);
			this.textBoxWord.Name = "textBoxWord";
			this.textBoxWord.Size = new System.Drawing.Size(133, 20);
			this.textBoxWord.TabIndex = 4;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(956, 24);
			this.menuStrip1.TabIndex = 11;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// actionsToolStripMenuItem
			// 
			this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regenerateMapperToolStripMenuItem,
            this.testMapperToolStripMenuItem,
            this.countOfMappingsToolStripMenuItem});
			this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
			this.actionsToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.actionsToolStripMenuItem.Text = "Actions";
			this.actionsToolStripMenuItem.Click += new System.EventHandler(this.actionsToolStripMenuItem_Click);
			// 
			// regenerateMapperToolStripMenuItem
			// 
			this.regenerateMapperToolStripMenuItem.Name = "regenerateMapperToolStripMenuItem";
			this.regenerateMapperToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
			this.regenerateMapperToolStripMenuItem.Text = "Regenerate Mapper-8 hours process";
			this.regenerateMapperToolStripMenuItem.Click += new System.EventHandler(this.regenerateMapperToolStripMenuItem_Click);
			// 
			// testMapperToolStripMenuItem
			// 
			this.testMapperToolStripMenuItem.Name = "testMapperToolStripMenuItem";
			this.testMapperToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
			this.testMapperToolStripMenuItem.Text = "Test Mapper-100 random sample";
			this.testMapperToolStripMenuItem.Click += new System.EventHandler(this.testMapperToolStripMenuItem_Click);
			// 
			// countOfMappingsToolStripMenuItem
			// 
			this.countOfMappingsToolStripMenuItem.Name = "countOfMappingsToolStripMenuItem";
			this.countOfMappingsToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
			this.countOfMappingsToolStripMenuItem.Text = "Count of Mappings";
			this.countOfMappingsToolStripMenuItem.Click += new System.EventHandler(this.countOfMappingsToolStripMenuItem_Click);
			// 
			// btnGetConcepts
			// 
			this.btnGetConcepts.Location = new System.Drawing.Point(135, 335);
			this.btnGetConcepts.Name = "btnGetConcepts";
			this.btnGetConcepts.Size = new System.Drawing.Size(171, 23);
			this.btnGetConcepts.TabIndex = 10;
			this.btnGetConcepts.Text = "Get Mapped Concepts";
			this.btnGetConcepts.UseVisualStyleBackColor = true;
			this.btnGetConcepts.Click += new System.EventHandler(this.btnGetConcepts_Click);
			// 
			// btnUniqueMap
			// 
			this.btnUniqueMap.Location = new System.Drawing.Point(356, 32);
			this.btnUniqueMap.Name = "btnUniqueMap";
			this.btnUniqueMap.Size = new System.Drawing.Size(75, 23);
			this.btnUniqueMap.TabIndex = 13;
			this.btnUniqueMap.Text = "Get unique mapping";
			this.btnUniqueMap.UseVisualStyleBackColor = true;
			this.btnUniqueMap.Click += new System.EventHandler(this.btnUniqueMap_Click);
			// 
			// FrmEnrich
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(956, 475);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FrmEnrich";
			this.Text = "Wordology-Mapping";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxPos;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonMeaning;
        private System.Windows.Forms.ListBox lstSenses;
        private System.Windows.Forms.TextBox textBoxWord;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxConcepts;
        private System.Windows.Forms.Button buttonConceptSenses;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regenerateMapperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testMapperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem countOfMappingsToolStripMenuItem;
        private System.Windows.Forms.Button btnGetConcepts;
		private System.Windows.Forms.Button btnUniqueMap;
    }
}

