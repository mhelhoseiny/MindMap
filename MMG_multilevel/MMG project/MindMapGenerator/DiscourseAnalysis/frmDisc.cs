using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using SyntacticAnalyzer;
using MMG;

namespace QAS
{
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
	public class Form2 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		public ArrayList DisClasses;
		public ArrayList ParseTrees;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form2()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.Color.White;
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(178)));
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(456, 430);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			this.textBox1.WordWrap = false;
			// 
			// Form2
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(456, 430);
			this.Controls.Add(this.textBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "Form2";
			this.Text = "Discourse Viewer";
			this.Load += new System.EventHandler(this.Form2_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void Form2_Load(object sender, System.EventArgs e)
        {
            #region Diaplay Discourse Results
            foreach (ArrayList arr in DisClasses)
			{
				foreach(DiscourseEntry de in arr)
				{
                    ParseTree p = (ParseTree)ParseTrees[de.TreeNum];
                   
                    string str=GetWordString(p,de.Node)+
						" ("+((int)(de.TreeNum+1)).ToString()+"."+((int)(de.Node.Start+1)).ToString()+")";
					
                    textBox1.Text+=str+"\r\n";
                    
				}
				textBox1.Text+="***************************************************\r\n";
			}
            textBox1.Select(0,0);

            #endregion

            #region Assign Discourse Data                

            foreach (ArrayList arr in DisClasses)
            {
                DiscourseEntry referredNounDE = (DiscourseEntry)arr[0];
                ParseTree ReferredNounParseTree = (ParseTree)ParseTrees[referredNounDE.TreeNum];
                string word = GetWordString(ReferredNounParseTree, referredNounDE.Node);

                for (int i = 1; i < arr.Count; i++)
                {
                    DiscourseEntry de = (DiscourseEntry)arr[i];
                    ParseTree p = (ParseTree)ParseTrees[de.TreeNum];
                    if (p.ResolvedAnaphora.Count != p.Words.Count)
                    {
                        for (int j = 0; j < p.Words.Count; j++)
                            p.ResolvedAnaphora.Add(null);
                    }
                    p.ResolvedAnaphora[de.Node.Start] = word;
                    //ParseNode node= GetReferedNode(ReferredNounParseTree);
                    de.Node.ReferedAnaphoraNode = referredNounDE.Node;
                }
            }
            
            #endregion
        }
        internal void assigndiscoursedata()
        {
            foreach (ArrayList arr in DisClasses)
            {
                DiscourseEntry referredNounDE = (DiscourseEntry)arr[0];
                ParseTree ReferredNounParseTree = (ParseTree)ParseTrees[referredNounDE.TreeNum];
                string word = GetWordString(ReferredNounParseTree, referredNounDE.Node);

                for (int i = 1; i < arr.Count; i++)
                {
                    DiscourseEntry de = (DiscourseEntry)arr[i];
                    ParseTree p = (ParseTree)ParseTrees[de.TreeNum];
                    if (p.ResolvedAnaphora.Count != p.Words.Count)
                    {
                        for (int j = 0; j < p.Words.Count; j++)
                            p.ResolvedAnaphora.Add(null);
                    }
                    p.ResolvedAnaphora[de.Node.Start] = word;
                    //ParseNode node= GetReferedNode(ReferredNounParseTree);
                    de.Node.ReferedAnaphoraNode = referredNounDE.Node;
                }
            }

        }
       
		private static string GetWordString(ParseTree tree,ParseNode node)
		{
			string str=(string)tree.Words[node.Start];
            ParseNode PN = (ParseNode)tree.Root;
			for(int i=node.Start+1;i<node.End;i++)
				str+="_"+(string)tree.Words[i];
			return str;
		}


	}
}
