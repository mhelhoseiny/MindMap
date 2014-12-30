namespace MMG
{
    partial class ChooseParseTreesForm
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.SentenceID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChosenParseTreeIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_addToparsetrees = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SentenceID,
            this.ChosenParseTreeIndex});
            this.dataGridView1.Location = new System.Drawing.Point(8, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(251, 228);
            this.dataGridView1.TabIndex = 0;
            // 
            // SentenceID
            // 
            this.SentenceID.HeaderText = "SentenceID";
            this.SentenceID.Name = "SentenceID";
            // 
            // ChosenParseTreeIndex
            // 
            this.ChosenParseTreeIndex.HeaderText = "ChosenParseTreeIndex";
            this.ChosenParseTreeIndex.Name = "ChosenParseTreeIndex";
            // 
            // btn_addToparsetrees
            // 
            this.btn_addToparsetrees.Location = new System.Drawing.Point(265, 23);
            this.btn_addToparsetrees.Name = "btn_addToparsetrees";
            this.btn_addToparsetrees.Size = new System.Drawing.Size(75, 23);
            this.btn_addToparsetrees.TabIndex = 1;
            this.btn_addToparsetrees.Text = "Add ";
            this.btn_addToparsetrees.UseVisualStyleBackColor = true;
            this.btn_addToparsetrees.Click += new System.EventHandler(this.btn_addToparsetrees_Click);
            // 
            // ChooseParseTreesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 247);
            this.Controls.Add(this.btn_addToparsetrees);
            this.Controls.Add(this.dataGridView1);
            this.Name = "ChooseParseTreesForm";
            this.Text = "ChooseParseTreesForm";
            this.Load += new System.EventHandler(this.ChooseParseTreesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SentenceID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChosenParseTreeIndex;
        private System.Windows.Forms.Button btn_addToparsetrees;

    }
}