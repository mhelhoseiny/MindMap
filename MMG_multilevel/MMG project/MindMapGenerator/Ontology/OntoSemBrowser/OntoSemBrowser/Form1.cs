using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text;

namespace OntoSemBrowser
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
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
				if (components != null) 
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
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(168, 88);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "button1";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			string ontodir = @"G:\Ontology\Formatted OntoSem";
			StreamReader sr = new StreamReader(ontodir+"\\AllConcepts.txt");
			string concept=null;
			while((concept=sr.ReadLine())!=null)
			{
				File.Move(ontodir+'\\'+concept[0]+'\\'+concept+"-2.txt",
					ontodir+'\\'+concept[0]+'\\'+concept+".txt");
				/*
				StreamReader file=new StreamReader(
					ontodir+'\\'+concept[0]+'\\'+concept+".txt");
				StringBuilder sb = new StringBuilder();
				string line=null;
				bool spanish=false;
				while((line=file.ReadLine())!=null)
				{
					if(line.StartsWith("SPANISH1"))
						spanish=true;
					else if(spanish && line.StartsWith("Inherited from"))
					{
						spanish=false;
						sb.Append("\r\n");
					}
					else if(spanish && line.Length>1 && 
						char.IsUpper(line[0]) && char.IsUpper(line[1]))
						spanish=false;
					if(!spanish)
						sb.Append(line+"\r\n");
				}
				file.Close();
				StreamWriter sw = new StreamWriter(
					ontodir+'\\'+concept[0]+'\\'+concept+"-2.txt");
				sw.Write(sb.ToString());
				sw.Close();
				*/
			}
			sr.Close();
		}
	}
}
