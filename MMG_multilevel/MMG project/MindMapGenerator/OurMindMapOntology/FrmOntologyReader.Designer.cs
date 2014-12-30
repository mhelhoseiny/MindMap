namespace OurMindMapOntology
{
    partial class FrmOntologyReader
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
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.btnLoad = new System.Windows.Forms.Button();
			this.btnGetParent = new System.Windows.Forms.Button();
			this.btnGetChildren = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtConceptName = new System.Windows.Forms.TextBox();
			this.lboxChildren = new System.Windows.Forms.ListBox();
			this.lblParent = new System.Windows.Forms.Label();
			this.btnMaplex = new System.Windows.Forms.Button();
			this.btnDefinition = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(13, 231);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(75, 23);
			this.btnLoad.TabIndex = 0;
			this.btnLoad.Text = "Load";
			this.btnLoad.UseVisualStyleBackColor = true;
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			// 
			// btnGetParent
			// 
			this.btnGetParent.Location = new System.Drawing.Point(109, 231);
			this.btnGetParent.Name = "btnGetParent";
			this.btnGetParent.Size = new System.Drawing.Size(75, 23);
			this.btnGetParent.TabIndex = 1;
			this.btnGetParent.Text = "GetParent";
			this.btnGetParent.UseVisualStyleBackColor = true;
			this.btnGetParent.Click += new System.EventHandler(this.btnGetParent_Click);
			// 
			// btnGetChildren
			// 
			this.btnGetChildren.Location = new System.Drawing.Point(205, 231);
			this.btnGetChildren.Name = "btnGetChildren";
			this.btnGetChildren.Size = new System.Drawing.Size(75, 23);
			this.btnGetChildren.TabIndex = 2;
			this.btnGetChildren.Text = "GetChildren";
			this.btnGetChildren.UseVisualStyleBackColor = true;
			this.btnGetChildren.Click += new System.EventHandler(this.btnGetChildren_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Concept name";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(0, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "label2";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(0, 77);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "label3";
			// 
			// txtConceptName
			// 
			this.txtConceptName.Location = new System.Drawing.Point(109, 5);
			this.txtConceptName.Name = "txtConceptName";
			this.txtConceptName.Size = new System.Drawing.Size(252, 20);
			this.txtConceptName.TabIndex = 6;
			// 
			// lboxChildren
			// 
			this.lboxChildren.FormattingEnabled = true;
			this.lboxChildren.Location = new System.Drawing.Point(109, 77);
			this.lboxChildren.Name = "lboxChildren";
			this.lboxChildren.Size = new System.Drawing.Size(252, 95);
			this.lboxChildren.TabIndex = 7;
			// 
			// lblParent
			// 
			this.lblParent.AutoSize = true;
			this.lblParent.Location = new System.Drawing.Point(106, 35);
			this.lblParent.Name = "lblParent";
			this.lblParent.Size = new System.Drawing.Size(0, 13);
			this.lblParent.TabIndex = 8;
			// 
			// btnMaplex
			// 
			this.btnMaplex.Location = new System.Drawing.Point(400, 231);
			this.btnMaplex.Name = "btnMaplex";
			this.btnMaplex.Size = new System.Drawing.Size(75, 23);
			this.btnMaplex.TabIndex = 9;
			this.btnMaplex.Text = "Maplex";
			this.btnMaplex.UseVisualStyleBackColor = true;
			this.btnMaplex.Click += new System.EventHandler(this.btnMaplex_Click);
			// 
			// btnDefinition
			// 
			this.btnDefinition.Location = new System.Drawing.Point(302, 231);
			this.btnDefinition.Name = "btnDefinition";
			this.btnDefinition.Size = new System.Drawing.Size(75, 23);
			this.btnDefinition.TabIndex = 10;
			this.btnDefinition.Text = "Definition";
			this.btnDefinition.UseVisualStyleBackColor = true;
			this.btnDefinition.Click += new System.EventHandler(this.btnDefinition_Click);
			// 
			// FrmOntologyReader
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(702, 266);
			this.Controls.Add(this.btnDefinition);
			this.Controls.Add(this.btnMaplex);
			this.Controls.Add(this.lblParent);
			this.Controls.Add(this.lboxChildren);
			this.Controls.Add(this.txtConceptName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnGetChildren);
			this.Controls.Add(this.btnGetParent);
			this.Controls.Add(this.btnLoad);
			this.Name = "FrmOntologyReader";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button btnLoad;
		private System.Windows.Forms.Button btnGetParent;
		private System.Windows.Forms.Button btnGetChildren;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtConceptName;
		private System.Windows.Forms.ListBox lboxChildren;
		private System.Windows.Forms.Label lblParent;
		private System.Windows.Forms.Button btnMaplex;
		private System.Windows.Forms.Button btnDefinition;
    }
}

