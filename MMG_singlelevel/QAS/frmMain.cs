using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using WnLexicon;
using System.Runtime.Serialization.Formatters.Binary;
using Wnlib;
using WordsMatching;
using MMG;
using MindMapGenerator.Drawing_Management;
using DiscourseAnalysis;
using SyntacticAnalyzer;
using mmTMR;
using GoogleImageDrawingEntity;
using MindMapViewingManagement;
using OntologyLibrary.OntoSem;
using System.Collections.Generic;

namespace MMG
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		RulesReader rr;
        ArrayList words = new ArrayList();
        ArrayList DisambRes = new ArrayList();
		Ontology Onto;
        
        
		private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.StatusBar statusBar1;
        private System.Windows.Forms.Button btnAnalyze;
        private Button buttonWSD;
        private Button btnDiscourse;
        private ListBox listBox1;
        private Button button1;
        private System.Windows.Forms.Panel panel1;
        private Button button3;
        public ArrayList parsetrees;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//Wnlib.WNCommon.path = @"..\..\..\dict\";
            //rr = new RulesReader(Wnlib.WNCommon.path + "\\Rules.txt");
            Wnlib.WNCommon.path = "C:\\Program Files (x86)\\WordNet\\2.1\\dict\\";
            //rr = new RulesReader("C:\\Rules.txt");
            rr = new RulesReader(Application.StartupPath + "\\EngParserCFG\\Rules.txt");
            
            
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnDiscourse = new System.Windows.Forms.Button();
            this.buttonWSD = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter text here:";
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.textBox1.Location = new System.Drawing.Point(12, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(1163, 322);
            this.textBox1.TabIndex = 1;
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnalyze.Location = new System.Drawing.Point(1181, 103);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(75, 23);
            this.btnAnalyze.TabIndex = 2;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.Click += new System.EventHandler(this.button1_Click);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.treeView1.Location = new System.Drawing.Point(0, 357);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowPlusMinus = false;
            this.treeView1.Size = new System.Drawing.Size(1266, 393);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnDiscourse);
            this.panel1.Controls.Add(this.buttonWSD);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.btnAnalyze);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1269, 357);
            this.panel1.TabIndex = 4;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1180, 221);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "draw";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1180, 192);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "TMR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btnDiscourse
            // 
            this.btnDiscourse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDiscourse.Location = new System.Drawing.Point(1182, 134);
            this.btnDiscourse.Name = "btnDiscourse";
            this.btnDiscourse.Size = new System.Drawing.Size(75, 23);
            this.btnDiscourse.TabIndex = 5;
            this.btnDiscourse.Text = "Discourse";
            this.btnDiscourse.Click += new System.EventHandler(this.btnDiscourse_Click);
            // 
            // buttonWSD
            // 
            this.buttonWSD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonWSD.Location = new System.Drawing.Point(1181, 163);
            this.buttonWSD.Name = "buttonWSD";
            this.buttonWSD.Size = new System.Drawing.Size(75, 23);
            this.buttonWSD.TabIndex = 3;
            this.buttonWSD.Text = "wsd";
            this.buttonWSD.Click += new System.EventHandler(this.buttonWSD_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(1266, 357);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 393);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 750);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Size = new System.Drawing.Size(1269, 24);
            this.statusBar1.TabIndex = 9;
            this.statusBar1.Text = "Ready";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 356);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1266, 394);
            this.listBox1.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackgroundImage = global::MMG.Properties.Resources.computer_science_mindmap;
            this.ClientSize = new System.Drawing.Size(1269, 774);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusBar1);
            this.IsMdiContainer = true;
            this.Name = "Form1";
            this.Text = "MMG Demo";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			//Application.Run(new Form3());
            //Google.API.Search.GimageSearchClient cl = new Google.API.Search.GimageSearchClient("http://www.rutgers.edu/");
            //IList<Google.API.Search.IImageResult> imres =  cl.Search("book", 1);
            //Image im = DownloadImage(imres[0].TbImage.Url);
            //im.Save("D:\\im.bmp");
            //Bitmap bmp = MindMapViewingManagement.GoogleSearch.GetImage("book");
            Application.Run(new Form5());
		}

       
		ArrayList SentencesWords = new ArrayList();
		ArrayList SParseTrees;
		ArrayList[] Strees;
		ArrayList QParseTrees;
		ArrayList[] Qtrees;
        ArrayList WordInfoArr=new ArrayList();

		private void button1_Click(object sender, System.EventArgs e)
		{
			ArrayList words=new ArrayList();
			string[] sents = textBox1.Text.Trim().Split('.');
			SParseTrees = new ArrayList();
			Strees=new ArrayList[sents.Length];
			for(int i=0;i<sents.Length;i++)
			{
				string sen=sents[i];
				if(sen=="")
					continue;
				words = new ArrayList();
				string[] temp=sen.Trim().Split(' ');
				foreach(string w in temp)
				{
					if(w=="")
						continue;
					char last=w[w.Length-1];
					if(last==','||last==';'||last=='?')
					{
						string x=w.TrimEnd(last);
						if(x.Length>0)
							words.Add(x);
						words.Add(last.ToString());
					}
					else
						words.Add(w);
				}
				SentenceParser sp = new SentenceParser(rr,words);
				Strees[i]=sp.Parse();
				if(Strees[i]!=null && Strees[i].Count>0)
				{
					SParseTrees.AddRange(Strees[i]);
					SentencesWords.Add(words);
				}
			}			
			ViewTrees(SParseTrees,textBox1,treeView1);
		}

		private ParseTree ModifyParseTree(ParseTree oldps)
		{
			ParseTree ps = (ParseTree)oldps.Clone();
			SentenceParser.LinkParents(ps.Root);			
			ModifyRoot(new ArrayList(),ref ps.Root);
			return ps;
		}

		private void ModifyRoot(ArrayList path,ref ParseNode node)
		{
			bool still=false;
			switch(node.Goal)
			{
				case"CS":
				{
					if(node.Children.Count==1)
						node=(ParseNode)node.Children[0];
				}break;
				case"NNC":
				case"NN":
				{
					ParseNode fch=(ParseNode)node.Children[0];
					if(fch.Goal=="N")
						node=(ParseNode)node.Children[0];
					else
						node.Goal="N";
				}break;
				case"NP":
				case"NPC":
				case"NPF":
				case"NPADJ":
				{
					node.Goal="NP";
					ParseNode fch=(ParseNode)node.Children[0];
					if(fch.Goal=="NP"||fch.Goal=="NPC")
					{
						node.Children.RemoveAt(0);
						node.Children.InsertRange(0,fch.Children);
					}

				}break;
				case"ABPH":
					node.Goal="ABS_PH";break;
				case"PRPH":
					node.Goal="PAR_PH";break;
				case"ADVC":
					node.Goal="ADV_CL";break;
				case"NC":
					node.Goal="N_CL";break;
				case"ADJC":
					node.Goal="ADJ_CL";break;
				case"PRP":
					node.Goal="PREP_PH";break;
				case"INFPO":
				case"INFPH":
					node.Goal="INF_PH";break;
				case"PPN":
					node.Goal="PRO_N";break;
				case"CMPAJ":
				case"BADJ":
				{
					ParseNode fch=(ParseNode)node.Children[0];
					if(fch.Goal.EndsWith("ADJ"))
						node=(ParseNode)node.Children[0];
					node.Goal="ADJ";
				}break;
				case"FADJ":
					node.Goal="ADJ";break;
				case"FAVJ":
				case"AVJ":
				{
					ParseNode fch=(ParseNode)node.Children[0];
					if(fch.Goal.EndsWith("ADJS"))
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
					else
						node.Goal="ADV_ADJ";
				}break;
				case"CMPAV":
				{
					node=(ParseNode)node.Children[0];
					node.Goal="ADV";
				}break;
				case"CMPAVJ":
				{
					node=(ParseNode)node.Children[0];
					still=true;
				}break;
				case"XV":
					node.Goal="AUX_V";break;
				case"IVP":
				case"SVP":
				case"PVP":
				case"GVP":
					node.Goal="VP";break;
				case"IPRD":
				case"SPRD":
				case"PPRD":
				case"GPRD":
					node.Goal="PRD";break;
				case"ADV":
				{
					ParseNode fch=(ParseNode)node.Children[0];
					if(fch.Goal.EndsWith("ADV"))
					{
						node=(ParseNode)node.Children[0];
						node.Goal="ADV";
					}
				}break;
				case"PADJ":
				case"SADJ":
				case"CADJ":
					node.Goal="ADJ";break;
				case"PADV":
					node.Goal="ADV";break;
				case"ARC":
					node.Goal="DET";break;
				case"SRPN":
					node.Goal="RPN";break;
				case"PSRPN":
					node.Goal="RPN";break;
				case"RPN":
				{
					if(node.Children.Count==1)
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
				}break;
				case"IPRDS":
				case"GPRDS":
				case"SPRDS":
				case"PPRDS":
				case"PRDS":
				{
					node.Children=GetListItems(node);
					if(node.Children.Count==1)
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
					else
						node.Goal="PRD_LST";
				}break;
				case"PRPHS":
				{
					node.Children=GetListItems(node);
					if(node.Children.Count==1)
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
					else
						node.Goal="PAR_PH_LST";
				}break;
				case"CMPAJS":
				case"FADJS":
				case"BADJS":
				{
					node.Children=GetListItems(node);
					if(node.Children.Count==1)
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
					else
						node.Goal="ADJ_LST";
				}break;
				case"ADVS":
				case"CMPAVS":
				{
					node.Children=GetListItems(node);
					if(node.Children.Count==1)
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
					else
						node.Goal="ADV_LST";
				}break;
				case"LPRPS":
				case"PRPS":
				{
					node.Children=GetListItems(node);
					if(node.Children.Count==1)
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
					else
						node.Goal="PREP_PH_LST";
				}break;
				case"SBJ":
				case"OBJ":
				{
					node.Children=GetListItems(node);
					if(node.Children.Count==1)
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
				}break;
				case"CMPS":
				{
					node.Children=GetListItems(node);
					if(node.Children.Count==1)
					{
						node=(ParseNode)node.Children[0];
						still=true;
					}
					else
						node.Goal="CMP_LST";
				}break;
				
			}
			//end
			if(still)
				ModifyRoot(path,ref node);
			else if(node.Children!=null)
			{
				path.Add(node);
				for(int i=0 ; i<node.Children.Count ; i++)
				{
					ParseNode child=(ParseNode)node.Children[i];
					ModifyRoot(path,ref child);
					node.Children[i]=child;
				}
				path.RemoveAt(path.Count-1);
			}
		}

		private ArrayList GetListItems(ParseNode node)
		{
			ArrayList arr=new ArrayList();
			foreach(ParseNode n in node.Children)
			{
				if(n.Goal.EndsWith("1")||n.Goal.EndsWith("2")||n.Goal.EndsWith("3")
					||n.Goal.EndsWith("4")||n.Goal==node.Goal)
					arr.AddRange(GetListItems(n));
				else
					arr.Add(n);
			}
			return arr;
		}
		
		private void ViewTrees(ArrayList Trees,TextBox textBox,TreeView treeView)
		{
			ArrayList ParseTrees = new ArrayList();
			foreach(ParseTree ps in Trees)
				ParseTrees.Add(ModifyParseTree(ps));
//			ParseTrees=Trees;
			treeView.Nodes.Clear();
			if(ParseTrees==null)
				return;
			foreach(ParseTree tree in ParseTrees)
			{
				if(tree==null)
					continue;
				TreeNode root=new TreeNode();
				root.Tag=tree.Score.ToString();
				Stack nodes=new Stack();
				Stack items=new Stack();
				nodes.Push(tree.Root);
				items.Push(root);
				string [] words=textBox.Text.Split(' ');
				while(nodes.Count>0)
				{
					ParseNode node=(ParseNode)nodes.Pop();
					TreeNode item=(TreeNode)items.Pop();
					if(node.Senses!=null)
					{
						string vlist="";
						foreach(NodeSense s in node.Senses)
							vlist+=s.Sense+"\n";
						item.Tag=vlist;
					}
					item.Text=node.Goal+" : ";
					for(int i=node.Start;i<node.End;i++)
						item.Text+=" "+tree.Words[i].ToString().ToLower();
					if(node.Children==null)
						continue;
					for(int i=0;i<node.Children.Count;i++)
					{
						nodes.Push(node.Children[i]);
						TreeNode t=new TreeNode();
						item.Nodes.Add(t);
						items.Push(t);
					}
				}
				root.ExpandAll();
				root.Collapse();
				treeView.Nodes.Add(root);
			}
		}

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
		//	if(e.Node.Tag!=null)
		//		MessageBox.Show((string)e.Node.Tag);
		}
		void LoadOntology()
		{
			if(Onto==null)
			{
				BinaryFormatter bf=new BinaryFormatter();
				System.IO.FileStream fs = new System.IO.FileStream(
					@"..\..\..\Ontology\Formatted OntoSem\Ontology.bin",FileMode.Open);
				Onto=(Ontology)bf.Deserialize(fs);
				fs.Close();
			}
		}
        //****************TMR***********\\\
		private void button3_Click(object sender, System.EventArgs e)
		{
            //if(Onto==null)
            //    LoadOntology();
            //tmr=new TMR();
            //tmr.Onto=this.Onto;
            //tmr.Init(SParseTrees,SentencesWords,d.DistinctClasses,true);
            //btnQuery.Enabled=true;
            //textBox2.Enabled=true;
		}
        //*************
		private void button4_Click(object sender, System.EventArgs e)
		{
			Ontology onto=new Ontology(@"..\..\..\Ontology\Formatted OntoSem");
			StreamReader sr = new StreamReader(onto.OntologyPath+"\\Get.txt");
			string str=null;
			while((str=sr.ReadLine())!=null)
			{
				if(str=="")
					continue;
				onto.LoadConcept(str);
			}
			sr.Close();
			BinaryFormatter bf=new BinaryFormatter();
			FileStream fs = new FileStream(
				onto.OntologyPath+"\\Ontology.bin",FileMode.Create);
			bf.Serialize(fs,onto);
			fs.Close();
			MessageBox.Show("Finished");
		}

		private void Query_Click(object sender, System.EventArgs e)
		{
            //ArrayList words=new ArrayList();
            //string[] sents = textBox2.Text.Trim().Split('.');
            //QParseTrees = new ArrayList();
            //Qtrees=new ArrayList[sents.Length];
            //for(int i=0;i<sents.Length;i++)
            //{
            //    string sen=sents[i];
            //    if(sen=="")
            //        continue;
            //    words = new ArrayList();
            //    string[] temp=sen.Trim().Split(' ');
            //    foreach(string w in temp)
            //    {
            //        if(w=="")
            //            continue;
            //        char last=w[w.Length-1];
            //        if(last==','||last==';'||last=='?')
            //        {
            //            string x=w.TrimEnd(last);
            //            if(x.Length>0)
            //                words.Add(x);
            //            words.Add(last.ToString());
            //        }
            //        else
            //            words.Add(w);
            //    }
            //    SentenceParser sp = new SentenceParser(rr,words);
            //    Qtrees[i]=sp.Parse();
            //    if(Qtrees[i]!=null && Qtrees[i].Count>0)
            //    {
            //        QParseTrees.AddRange(Qtrees[i]);
            //        SentencesWords.Add(words);
            //    }
            //}			
            //ViewTrees(QParseTrees,textBox2,treeView2);
            //if(Onto==null)
            //    LoadOntology();
            //Query q=new Query();
            //q.Frames=tmr.MainFrames;
            //q.Concepts = this.tmr.Concepts;
            //q.Onto=this.Onto;
            //q.Question=(ParseTree)QParseTrees[0];
            //q.Begin();
            //ShowQuestionTree(true);
		}

/*
		private void button3_Click(object sender, System.EventArgs e)
		{
			StreamReader srname = new StreamReader(Wnlib.WNCommon.path+"other.txt");
			StreamWriter newnames = new StreamWriter(Wnlib.WNCommon.path+"comother.txt");
			string line=null;
			while((line=srname.ReadLine())!=null)
			{
				if(line=="")
					continue;
				line=line.Split(',')[0];
				WordInfo wi = WnLexicon.Lexicon.FindWordInfo(line,true,true);
				if(wi.senseCounts!=null && wi.Strength>0)
					newnames.WriteLine(line);
			}
			srname.Close();
			newnames.Close();
		}
		*/

		private void ShowQuestionTree(bool show)
		{
			//treeView2.Visible=show;
			splitter1.Visible=show;
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
            listBox1.Visible = false;
			ShowQuestionTree(false);
		}

       /*
        private void WordSenseDisambiguation(ParseTree parsetree)
        {
            int j = 0;
            ArrayList arr = new ArrayList();
            if (parsetree.Root.Children != null)
            {
               words= wsd2(parsetree);
               foreach (string str in words)
               {
                   string s = "";
                   if (str == "NPP" || str == "PPJ")
                   {
                       s = str + ":" + parsetree.ResolvedAnaphora[j];

                   }
                   else
                   {
                       s = str + ":" + parsetree.Words[j];
                   }
                   WordInfoArr.Add(s);
                   j++;
               }

               words = new ArrayList();
            }
            else
            {
                
                //string str = parsetree.Words[j] + ":" + parsetree.Root.Goal.ToString();
                foreach (string str in words)
                {
                     string s=str+":"+parsetree.Words[j]  ;
                    WordInfoArr.Add(s);
                    j++;
                }

                words = new ArrayList();
                
            }
        }
        private ArrayList wsd2(ParseTree parsetree)
        {
            
            ArrayList arr = new ArrayList();
            if (parsetree.Root.Children != null)
            {
                for (int i = 0; i < parsetree.Root.Children.Count; i++)
                {
                    ParseNode pn = new ParseNode();
                    pn = (ParseNode)parsetree.Root.Children[i];
                    ParseTree p = new ParseTree(pn, parsetree.Words);
                   words= wsd2(p);
                }
            }
            else
            {
                string goal = parsetree.Root.Goal;
                words.Add(goal);
                

            }

            return words;
        }
        */
        private void btnDiscourse_Click(object sender, EventArgs e)
        {
            ArrayList NewSParseTrees = new ArrayList();
            Discourse d = new Discourse(SParseTrees);
            bool modified = d.PrepareParseTrees();
            if (modified)
            {
                NewSParseTrees = d.NewSParseTrees;
                d.Begin(NewSParseTrees);
                Form2 f = new Form2();
                f.DisClasses = d.DistinctClasses;
                f.ParseTrees = NewSParseTrees;
                f.ShowDialog(this);
                d.ReturnSParseTrees();
                SParseTrees = d.SParseTrees;
            }
            else
            {
                d.Begin(SParseTrees);
                Form2 f = new Form2();
                f.DisClasses = d.DistinctClasses;
                f.ParseTrees = SParseTrees;
                f.ShowDialog(this);
            }
            
        }

        private void buttonWSD_Click(object sender, EventArgs e)
        {
            listBox1.Visible = true;
            Disambiguation D = new Disambiguation(SParseTrees);

            D.beginDisambiguate();
            DisambRes = D.DisambRes;
            SParseTrees = D.SParseTrees;
            foreach (string s in DisambRes)
            {
                listBox1.Items.Add((string)s);
            }
            //DisambRes -->contains the senses
        }

        MindMapTMR tmr;
        private void button1_Click_1(object sender, EventArgs e)
        {
            parsetrees = new ArrayList();
            foreach (ParseTree ps in SParseTrees)
            {
                parsetrees.Add(ModifyParseTree(ps));
            }
            MindMapTMR MINDMAP = new MindMapTMR(parsetrees, null);
            MINDMAP.BuildTMR();
            tmr = MINDMAP;   
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

            pics pic = new pics();
            MMViewManeger mmvm = new MMViewManeger(pic, tmr);
            pic.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ////GoogleSearch gs = new GoogleSearch("car");
            ////gs.GetPictures();
            ////Bitmap btm = gs.bitmap;
            ////pics   pic = new pics(btm);
            ////pic.Show();5

            //string query = "";
            //NounFrame NF = new NounFrame();
            //query=NF.Text;
            //GoogleSearch gs = new GoogleSearch(query);
            //gs.GetPictures();
            //Bitmap btm = gs.bitmap;
            ////Bitmap btm = new Bitmap("E:\\pics tanya\\roody.jpg");
            //pics pic = new pics();
            //Pics GR = new Pics(btm, pic);
            //pics picform = (MMG.pics)GR.PicForm;
            //picform.Show();
        }
     

        
	}

}
