namespace GraphTest
{
    partial class GraphForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButton_Move = new System.Windows.Forms.RadioButton();
            this.radioButton_insertEdge = new System.Windows.Forms.RadioButton();
            this.radioButton_insertNode = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.button_annrealing = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox_AllocationMode = new System.Windows.Forms.GroupBox();
            this.radioButton_circular = new System.Windows.Forms.RadioButton();
            this.radioButton_Random = new System.Windows.Forms.RadioButton();
            this.radioButton_UserDefined = new System.Windows.Forms.RadioButton();
            this.button4 = new System.Windows.Forms.Button();
            this.txt_N = new System.Windows.Forms.TextBox();
            this.txt_desiredEdgeLength = new System.Windows.Forms.TextBox();
            this.txt_desired = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button_spring = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_FloydW = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_InsertEdge = new System.Windows.Forms.Button();
            this.comboBox_Node2 = new System.Windows.Forms.ComboBox();
            this.comboBox_Node1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_Y = new System.Windows.Forms.TextBox();
            this.textBox_X = new System.Windows.Forms.TextBox();
            this.button_InsertNode = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox_AllocationMode.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.groupBox5);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(724, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(371, 684);
            this.panel1.TabIndex = 1;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButton_Move);
            this.groupBox5.Controls.Add(this.radioButton_insertEdge);
            this.groupBox5.Controls.Add(this.radioButton_insertNode);
            this.groupBox5.Location = new System.Drawing.Point(20, 244);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(327, 100);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Options";
            // 
            // radioButton_Move
            // 
            this.radioButton_Move.AutoSize = true;
            this.radioButton_Move.Location = new System.Drawing.Point(26, 70);
            this.radioButton_Move.Name = "radioButton_Move";
            this.radioButton_Move.Size = new System.Drawing.Size(81, 17);
            this.radioButton_Move.TabIndex = 2;
            this.radioButton_Move.Text = "Move Node";
            this.radioButton_Move.UseVisualStyleBackColor = true;
            // 
            // radioButton_insertEdge
            // 
            this.radioButton_insertEdge.AutoSize = true;
            this.radioButton_insertEdge.Location = new System.Drawing.Point(26, 49);
            this.radioButton_insertEdge.Name = "radioButton_insertEdge";
            this.radioButton_insertEdge.Size = new System.Drawing.Size(79, 17);
            this.radioButton_insertEdge.TabIndex = 1;
            this.radioButton_insertEdge.Text = "Insert Edge";
            this.radioButton_insertEdge.UseVisualStyleBackColor = true;
            // 
            // radioButton_insertNode
            // 
            this.radioButton_insertNode.AutoSize = true;
            this.radioButton_insertNode.Checked = true;
            this.radioButton_insertNode.Location = new System.Drawing.Point(26, 29);
            this.radioButton_insertNode.Name = "radioButton_insertNode";
            this.radioButton_insertNode.Size = new System.Drawing.Size(80, 17);
            this.radioButton_insertNode.TabIndex = 0;
            this.radioButton_insertNode.TabStop = true;
            this.radioButton_insertNode.Text = "Insert Node";
            this.radioButton_insertNode.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.groupBox7);
            this.groupBox4.Controls.Add(this.groupBox6);
            this.groupBox4.Location = new System.Drawing.Point(20, 415);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(327, 229);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Automatic Layout Allocation";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.button_annrealing);
            this.groupBox7.Controls.Add(this.button3);
            this.groupBox7.Location = new System.Drawing.Point(12, 175);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(188, 48);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Simulated Annealing";
            // 
            // button_annrealing
            // 
            this.button_annrealing.Location = new System.Drawing.Point(100, 19);
            this.button_annrealing.Name = "button_annrealing";
            this.button_annrealing.Size = new System.Drawing.Size(78, 23);
            this.button_annrealing.TabIndex = 4;
            this.button_annrealing.Text = "Run";
            this.button_annrealing.UseVisualStyleBackColor = true;
            this.button_annrealing.Click += new System.EventHandler(this.button_annrealing_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(6, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(76, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "single step";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.groupBox_AllocationMode);
            this.groupBox6.Controls.Add(this.button4);
            this.groupBox6.Controls.Add(this.txt_N);
            this.groupBox6.Controls.Add(this.txt_desiredEdgeLength);
            this.groupBox6.Controls.Add(this.txt_desired);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.button_spring);
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Location = new System.Drawing.Point(12, 28);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(309, 141);
            this.groupBox6.TabIndex = 5;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Spring Model";
            // 
            // groupBox_AllocationMode
            // 
            this.groupBox_AllocationMode.Controls.Add(this.radioButton_circular);
            this.groupBox_AllocationMode.Controls.Add(this.radioButton_Random);
            this.groupBox_AllocationMode.Controls.Add(this.radioButton_UserDefined);
            this.groupBox_AllocationMode.Location = new System.Drawing.Point(161, 52);
            this.groupBox_AllocationMode.Name = "groupBox_AllocationMode";
            this.groupBox_AllocationMode.Size = new System.Drawing.Size(130, 83);
            this.groupBox_AllocationMode.TabIndex = 7;
            this.groupBox_AllocationMode.TabStop = false;
            this.groupBox_AllocationMode.Text = "Allocation Mode";
            // 
            // radioButton_circular
            // 
            this.radioButton_circular.AutoSize = true;
            this.radioButton_circular.Location = new System.Drawing.Point(6, 57);
            this.radioButton_circular.Name = "radioButton_circular";
            this.radioButton_circular.Size = new System.Drawing.Size(60, 17);
            this.radioButton_circular.TabIndex = 2;
            this.radioButton_circular.TabStop = true;
            this.radioButton_circular.Text = "Circular";
            this.radioButton_circular.UseVisualStyleBackColor = true;
            // 
            // radioButton_Random
            // 
            this.radioButton_Random.AutoSize = true;
            this.radioButton_Random.Location = new System.Drawing.Point(6, 37);
            this.radioButton_Random.Name = "radioButton_Random";
            this.radioButton_Random.Size = new System.Drawing.Size(65, 17);
            this.radioButton_Random.TabIndex = 1;
            this.radioButton_Random.TabStop = true;
            this.radioButton_Random.Text = "Random";
            this.radioButton_Random.UseVisualStyleBackColor = true;
            // 
            // radioButton_UserDefined
            // 
            this.radioButton_UserDefined.AutoSize = true;
            this.radioButton_UserDefined.Checked = true;
            this.radioButton_UserDefined.Location = new System.Drawing.Point(6, 19);
            this.radioButton_UserDefined.Name = "radioButton_UserDefined";
            this.radioButton_UserDefined.Size = new System.Drawing.Size(87, 17);
            this.radioButton_UserDefined.TabIndex = 0;
            this.radioButton_UserDefined.TabStop = true;
            this.radioButton_UserDefined.Text = "User Defined";
            this.radioButton_UserDefined.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(75, 90);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "N Steps";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // txt_N
            // 
            this.txt_N.Location = new System.Drawing.Point(31, 92);
            this.txt_N.Name = "txt_N";
            this.txt_N.Size = new System.Drawing.Size(38, 20);
            this.txt_N.TabIndex = 5;
            this.txt_N.Text = "100";
            this.txt_N.TextChanged += new System.EventHandler(this.txt_desired_TextChanged);
            // 
            // txt_desiredEdgeLength
            // 
            this.txt_desiredEdgeLength.Location = new System.Drawing.Point(270, 26);
            this.txt_desiredEdgeLength.Name = "txt_desiredEdgeLength";
            this.txt_desiredEdgeLength.Size = new System.Drawing.Size(33, 20);
            this.txt_desiredEdgeLength.TabIndex = 5;
            this.txt_desiredEdgeLength.Text = "200";
            this.txt_desiredEdgeLength.TextChanged += new System.EventHandler(this.txt_desired_TextChanged);
            // 
            // txt_desired
            // 
            this.txt_desired.Location = new System.Drawing.Point(96, 26);
            this.txt_desired.Name = "txt_desired";
            this.txt_desired.Size = new System.Drawing.Size(38, 20);
            this.txt_desired.TabIndex = 5;
            this.txt_desired.Text = "600";
            this.txt_desired.TextChanged += new System.EventHandler(this.txt_desired_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "N:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(158, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Desired Edge Length";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Desired Length";
            // 
            // button_spring
            // 
            this.button_spring.Location = new System.Drawing.Point(96, 48);
            this.button_spring.Name = "button_spring";
            this.button_spring.Size = new System.Drawing.Size(55, 23);
            this.button_spring.TabIndex = 3;
            this.button_spring.Text = "Run";
            this.button_spring.UseVisualStyleBackColor = true;
            this.button_spring.Click += new System.EventHandler(this.button_spring_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Single Step";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(20, 650);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Align Nodes";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button_FloydW);
            this.groupBox3.Location = new System.Drawing.Point(20, 350);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(327, 59);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Find Shortest Path";
            // 
            // button_FloydW
            // 
            this.button_FloydW.Location = new System.Drawing.Point(26, 19);
            this.button_FloydW.Name = "button_FloydW";
            this.button_FloydW.Size = new System.Drawing.Size(153, 23);
            this.button_FloydW.TabIndex = 0;
            this.button_FloydW.Text = "Floyd Warshall Algorithm";
            this.button_FloydW.UseVisualStyleBackColor = true;
            this.button_FloydW.Click += new System.EventHandler(this.button_FloydW_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_InsertEdge);
            this.groupBox2.Controls.Add(this.comboBox_Node2);
            this.groupBox2.Controls.Add(this.comboBox_Node1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(20, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 110);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Insert Edge";
            // 
            // button_InsertEdge
            // 
            this.button_InsertEdge.Location = new System.Drawing.Point(134, 81);
            this.button_InsertEdge.Name = "button_InsertEdge";
            this.button_InsertEdge.Size = new System.Drawing.Size(56, 23);
            this.button_InsertEdge.TabIndex = 4;
            this.button_InsertEdge.Text = "Insert";
            this.button_InsertEdge.UseVisualStyleBackColor = true;
            this.button_InsertEdge.Click += new System.EventHandler(this.button_InsertEdge_Click);
            // 
            // comboBox_Node2
            // 
            this.comboBox_Node2.FormattingEnabled = true;
            this.comboBox_Node2.Location = new System.Drawing.Point(86, 52);
            this.comboBox_Node2.Name = "comboBox_Node2";
            this.comboBox_Node2.Size = new System.Drawing.Size(104, 21);
            this.comboBox_Node2.TabIndex = 3;
            // 
            // comboBox_Node1
            // 
            this.comboBox_Node1.FormattingEnabled = true;
            this.comboBox_Node1.Location = new System.Drawing.Point(86, 26);
            this.comboBox_Node1.Name = "comboBox_Node1";
            this.comboBox_Node1.Size = new System.Drawing.Size(104, 21);
            this.comboBox_Node1.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Node 2:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Node 1:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_Y);
            this.groupBox1.Controls.Add(this.textBox_X);
            this.groupBox1.Controls.Add(this.button_InsertNode);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(20, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 110);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Insert Node";
            // 
            // textBox_Y
            // 
            this.textBox_Y.Location = new System.Drawing.Point(86, 50);
            this.textBox_Y.Name = "textBox_Y";
            this.textBox_Y.Size = new System.Drawing.Size(104, 20);
            this.textBox_Y.TabIndex = 4;
            // 
            // textBox_X
            // 
            this.textBox_X.Location = new System.Drawing.Point(86, 22);
            this.textBox_X.Name = "textBox_X";
            this.textBox_X.Size = new System.Drawing.Size(104, 20);
            this.textBox_X.TabIndex = 3;
            // 
            // button_InsertNode
            // 
            this.button_InsertNode.Location = new System.Drawing.Point(134, 78);
            this.button_InsertNode.Name = "button_InsertNode";
            this.button_InsertNode.Size = new System.Drawing.Size(56, 23);
            this.button_InsertNode.TabIndex = 2;
            this.button_InsertNode.Text = "Insert";
            this.button_InsertNode.UseVisualStyleBackColor = true;
            this.button_InsertNode.Click += new System.EventHandler(this.button_InsertNode_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y-Position:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "X-Position:";
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(724, 684);
            this.panel2.TabIndex = 2;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseMove);
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseUp);
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1095, 684);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "GraphForm";
            this.Text = "Main Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.GraphForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GraphForm_Paint);
            this.panel1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox_AllocationMode.ResumeLayout(false);
            this.groupBox_AllocationMode.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_InsertNode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Y;
        private System.Windows.Forms.TextBox textBox_X;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_InsertEdge;
        private System.Windows.Forms.ComboBox comboBox_Node2;
        private System.Windows.Forms.ComboBox comboBox_Node1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button_FloydW;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_spring;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioButton_Move;
        private System.Windows.Forms.RadioButton radioButton_insertEdge;
        private System.Windows.Forms.RadioButton radioButton_insertNode;
        private System.Windows.Forms.Button button_annrealing;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox txt_N;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_desired;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_desiredEdgeLength;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox_AllocationMode;
        private System.Windows.Forms.RadioButton radioButton_Random;
        private System.Windows.Forms.RadioButton radioButton_UserDefined;
        private System.Windows.Forms.RadioButton radioButton_circular;

    }
}

