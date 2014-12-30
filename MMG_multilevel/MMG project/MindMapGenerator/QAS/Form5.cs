using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
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
using System.Collections;
using OntologyLibrary.OntoSem;
using OntologyConceptsEditor;
using MapperTool;
using Google.API.Search;
using System.Drawing.Imaging;

namespace MMG
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }
        String Text = "";
        RulesReader rr;
        GoogleImageSearchSettings gImSearchSettings = new GoogleImageSearchSettings();
        ArrayList words = new ArrayList();
        ArrayList DisambRes = new ArrayList();
        Ontology Onto;
        public ArrayList parsetrees;
        ArrayList SentencesWords = new ArrayList();
        ArrayList SParseTrees;
        ArrayList[] Strees;
        ArrayList QParseTrees;
        ArrayList[] Qtrees;
        ArrayList WordInfoArr = new ArrayList();
        MindMapTMR tmr;

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFile();

        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        private void toolStripButtonConvert_Click(object sender, EventArgs e)
        {
            ConvertToMindMap();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertToMindMap();
        }

        private void LoadFile()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = openFileDialog1.FileName;
                string textOfFile = "";

                FileStream FS = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                StreamReader SR = new StreamReader(FS);
                textOfFile = SR.ReadToEnd();
                SR.Close();
                richTextBox1.Text = textOfFile;
                richTextBox1.Visible = true;
            }

        }

        private void ConvertToMindMap()
        {
          
            
            ////////////////////////////////////////////////
            BeginConversion();
        }

        private void BeginConversion()
        {
            Parsing();
            DiscourseAnalysis();
            DisambiguationAnalysis();
            DiscourseAnalysis();
            DisambiguationAnalysis();
            TextMeaningRepresentation();
            Drawing();       


        }

        private void Parsing()
        {

            Text = richTextBox1.Text;
            ArrayList words = new ArrayList();
            string[] sents = Text.Trim().Split('.');
            SParseTrees = new ArrayList();
            Strees = new ArrayList[sents.Length];
            for (int i = 0; i < sents.Length; i++)
            {
                string sen = sents[i];
                if (sen == "")
                    continue;
                words = new ArrayList();
                string[] temp = sen.Trim().Split(' ');
                foreach (string w in temp)
                {
                    if (w == "")
                        continue;
                    char last = w[w.Length - 1];
                    if (last == ',' || last == ';' || last == '?')
                    {
                        string x = w.TrimEnd(last);
                        if (x.Length > 0)
                            words.Add(x);
                        words.Add(last.ToString());
                    }
                    else
                        words.Add(w);
                }
                SentenceParser sp = new SentenceParser(rr, words);
                Strees[i] = sp.Parse();
                if (Strees[i] != null && Strees[i].Count > 0)
                {
                    SParseTrees.AddRange(Strees[i]);
                    SentencesWords.Add(words);
                }
            }
            AllParseTrees = SParseTrees;
            ViewTrees(SParseTrees, textBox1, treeView1);
        }
        private void FilterParseTrees()
        {
            ArrayList snPtrees = new ArrayList();
            for (int i = 0; i < SParseTrees.Count; i++)
            {
                if (i + 1 == SParseTrees.Count)
                    snPtrees.Add(SParseTrees[i]);
                else
                {
                    ParseTree cPtree = (ParseTree)SParseTrees[i];
                    ParseTree nPtree = (ParseTree)SParseTrees[i + 1];
                    if (cPtree.Words.Count != nPtree.Words.Count)
                    {
                        snPtrees.Add(cPtree);
                    }
                    else
                    {
                        for (int j = 0; j < cPtree.Words.Count; j++)
                        {
                            if (cPtree.Words[j] != nPtree.Words[j])
                            {
                                snPtrees.Add(cPtree);
                                break;
                            }
                        }
 
                    }
                }
              

            }
            SParseTrees = snPtrees;
            ViewTrees(SParseTrees, textBox1, treeView1);
        }
        private void DisambiguationAnalysis()
        {

            Disambiguation D = new Disambiguation(SParseTrees);

            D.beginDisambiguate();
            DisambRes = D.DisambRes;
            SParseTrees = D.SParseTrees;
            foreach (string s in DisambRes)
            {
                listBox1.Items.Add((string)s);
            }
            dockContainer1.Visible = true;
            dockControl1.Visible = true;
            ViewTrees(SParseTrees, textBox1, treeView1);
        }

        private void DiscourseAnalysis()
        {
            ArrayList NewSParseTrees = new ArrayList();
            Discourse d = new Discourse(SParseTrees);
            bool modified = d.PrepareParseTrees();
            if (modified)
            {
                NewSParseTrees = d.NewSParseTrees;
                d.Begin(NewSParseTrees);
                DisplayDiscourseRes(d.DistinctClasses, NewSParseTrees);
                d.assigndiscoursedata(d.DistinctClasses, NewSParseTrees);
                d.ReturnSParseTrees();
                SParseTrees = d.SParseTrees;
            }
            else
            {
                d.Begin(SParseTrees);
                DisplayDiscourseRes(d.DistinctClasses, SParseTrees);
                d.assigndiscoursedata(d.DistinctClasses, SParseTrees);
            }
        }

        private void DisplayDiscourseRes(ArrayList DisClasses, ArrayList ParseTrees)
        {
            foreach (ArrayList arr in DisClasses)
            {
                foreach (DiscourseEntry de in arr)
                {
                    ParseTree p = (ParseTree)ParseTrees[de.TreeNum];
                    
                    string str = GetWordString(p, de.Node) +
                        " (" + ((int)(de.TreeNum + 1)).ToString() + "." + ((int)(de.Node.Start + 1)).ToString() + ")";

                    textBox1.Text += str + "\r\n";

                }
                textBox1.Text += "***************************************************\r\n";
            }
            textBox1.Select(0, 0);

        }

        private static string GetWordString(ParseTree tree, ParseNode node)
        {
            string str = (string)tree.Words[node.Start];
            for (int i = node.Start + 1; i < node.End; i++)
                str += "_" + (string)tree.Words[i];
            return str;
        }

        private void TextMeaningRepresentation()
        {
            //parsetrees = new ArrayList();

            //foreach (ParseTree ps in SParseTrees)
            //{
            //    parsetrees.Add(ModifyParseTree(ps));
            //}
            MindMapTMR MINDMAP = new MindMapTMR(SParseTrees, null);
            MINDMAP.BuildTMR();
            tmr = MINDMAP;

            DisplayMappingsInFile();
          

            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo("\"D:\\Program Files\\Notepad++\\notepad++.exe\"", "\"F:\\Mohamed ELhoseiny\\Research\\English2MindMap\\Test cases\\01. Shakespear\\mappedConceprs.txt");
            proc.Start();
        }

        private void DisplayMappingsInFile()
        {
            int mapped, total;
            List<string> concepts = tmr.GetFramesConcepts(out mapped, out total);
            string s = mapped.ToString() + "Mapped out of " + total.ToString() + "\n\n";
            foreach (string str in concepts)
                s += str + "\n\n";
            StreamWriter sw = new StreamWriter(@"F:\Mohamed ELhoseiny\Research\English2MindMap\Test cases\01. Shakespear\mappedConceprs.txt", false);
            //MessageBox.Show(s,"", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, MessageBoxOptions.);
            sw.Write(s);
            sw.Close();
        }
        private void Drawing()
        {
           // PictureBox PB = new PictureBox();
           // MMViewManeger mmvm = new MMViewManeger(pictureBox1, tmr);
            Form6 form6 = new Form6(this);
            //Point p = this.panel2.AutoScrollPosition;
            MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, gImSearchSettings);
            form6.mmvm = mmvm;
            form6.Show();
            groupBox2.Visible = true;
            dockContainer1.Visible = true;
            dockControl1.Visible = true;
            dockControl2.Visible = true;
            dockControl3.Visible = true;
            

        }


        private ParseTree ModifyParseTree(ParseTree oldps)
        {
            ParseTree ps = (ParseTree)oldps.Clone();
            SentenceParser.LinkParents(ps.Root);
            ModifyRoot(new ArrayList(), ref ps.Root);
            return ps;
        }

        private void ModifyRoot(ArrayList path, ref ParseNode node)
        {
            bool still = false;
            switch (node.Goal)
            {
                case "CS":
                    {
                        if (node.Children.Count == 1)
                            node = (ParseNode)node.Children[0];
                    } break;
                case "NNC":
                case "NN":
                    {
                        ParseNode fch = (ParseNode)node.Children[0];
                        if (fch.Goal == "N")
                            node = (ParseNode)node.Children[0];
                        else
                            node.Goal = "N";
                    } break;
                case "NP":
                case "NPC":
                case "NPF":
                case "NPADJ":
                    {
                        node.Goal = "NP";
                        ParseNode fch = (ParseNode)node.Children[0];
                        if (fch.Goal == "NP" || fch.Goal == "NPC")
                        {
                            node.Children.RemoveAt(0);
                            node.Children.InsertRange(0, fch.Children);
                        }

                    } break;
                case "ABPH":
                    node.Goal = "ABS_PH"; break;
                case "PRPH":
                    node.Goal = "PAR_PH"; break;
                case "ADVC":
                    node.Goal = "ADV_CL"; break;
                case "NC":
                    node.Goal = "N_CL"; break;
                case "ADJC":
                    node.Goal = "ADJ_CL"; break;
                case "PRP":
                    node.Goal = "PREP_PH"; break;
                case "INFPO":
                case "INFPH":
                    node.Goal = "INF_PH"; break;
                case "PPN":
                    node.Goal = "PRO_N"; break;
                case "CMPAJ":
                case "BADJ":
                    {
                        ParseNode fch = (ParseNode)node.Children[0];
                        if (fch.Goal.EndsWith("ADJ"))
                            node = (ParseNode)node.Children[0];
                        node.Goal = "ADJ";
                    } break;
                case "FADJ":
                    node.Goal = "ADJ"; break;
                case "FAVJ":
                case "AVJ":
                    {
                        ParseNode fch = (ParseNode)node.Children[0];
                        if (fch.Goal.EndsWith("ADJS"))
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                        else
                            node.Goal = "ADV_ADJ";
                    } break;
                case "CMPAV":
                    {
                        node = (ParseNode)node.Children[0];
                        node.Goal = "ADV";
                    } break;
                case "CMPAVJ":
                    {
                        node = (ParseNode)node.Children[0];
                        still = true;
                    } break;
                case "XV":
                    node.Goal = "AUX_V"; break;
                case "IVP":
                case "SVP":
                case "PVP":
                case "GVP":
                    node.Goal = "VP"; break;
                case "IPRD":
                case "SPRD":
                case "PPRD":
                case "GPRD":
                    node.Goal = "PRD"; break;
                case "ADV":
                    {
                        ParseNode fch = (ParseNode)node.Children[0];
                        if (fch.Goal.EndsWith("ADV"))
                        {
                            node = (ParseNode)node.Children[0];
                            node.Goal = "ADV";
                        }
                    } break;
                case "PADJ":
                case "SADJ":
                case "CADJ":
                    node.Goal = "ADJ"; break;
                case "PADV":
                    node.Goal = "ADV"; break;
                case "ARC":
                    node.Goal = "DET"; break;
                case "SRPN":
                    node.Goal = "RPN"; break;
                case "PSRPN":
                    node.Goal = "RPN"; break;
                case "RPN":
                    {
                        if (node.Children.Count == 1)
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                    } break;
                case "IPRDS":
                case "GPRDS":
                case "SPRDS":
                case "PPRDS":
                case "PRDS":
                    {
                        node.Children = GetListItems(node);
                        if (node.Children.Count == 1)
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                        else
                            node.Goal = "PRD_LST";
                    } break;
                case "PRPHS":
                    {
                        node.Children = GetListItems(node);
                        if (node.Children.Count == 1)
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                        else
                            node.Goal = "PAR_PH_LST";
                    } break;
                case "CMPAJS":
                case "FADJS":
                case "BADJS":
                    {
                        node.Children = GetListItems(node);
                        if (node.Children.Count == 1)
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                        else
                            node.Goal = "ADJ_LST";
                    } break;
                case "ADVS":
                case "CMPAVS":
                    {
                        node.Children = GetListItems(node);
                        if (node.Children.Count == 1)
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                        else
                            node.Goal = "ADV_LST";
                    } break;
                case "LPRPS":
                case "PRPS":
                    {
                        node.Children = GetListItems(node);
                        if (node.Children.Count == 1)
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                        else
                            node.Goal = "PREP_PH_LST";
                    } break;
                case "SBJ":
                case "OBJ":
                    {
                        node.Children = GetListItems(node);
                        if (node.Children.Count == 1)
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                    } break;
                case "CMPS":
                    {
                        node.Children = GetListItems(node);
                        if (node.Children.Count == 1)
                        {
                            node = (ParseNode)node.Children[0];
                            still = true;
                        }
                        else
                            node.Goal = "CMP_LST";
                    } break;

            }
            //end
            if (still)
                ModifyRoot(path, ref node);
            else if (node.Children != null)
            {
                path.Add(node);
                for (int i = 0; i < node.Children.Count; i++)
                {
                    ParseNode child = (ParseNode)node.Children[i];
                    ModifyRoot(path, ref child);
                    node.Children[i] = child;
                }
                path.RemoveAt(path.Count - 1);
            }
        }

        private ArrayList GetListItems(ParseNode node)
        {
            ArrayList arr = new ArrayList();
            foreach (ParseNode n in node.Children)
            {
                if (n.Goal.EndsWith("1") || n.Goal.EndsWith("2") || n.Goal.EndsWith("3")
                    || n.Goal.EndsWith("4") || n.Goal == node.Goal)
                    arr.AddRange(GetListItems(n));
                else
                    arr.Add(n);
            }
            return arr;
        }
        private void ViewTrees(ArrayList Trees, TextBox textBox, TreeView treeView)
        {
            ArrayList ParseTrees = new ArrayList();
            foreach (ParseTree ps in Trees)
                ParseTrees.Add(ModifyParseTree(ps));
            //			ParseTrees=Trees;
            treeView.Nodes.Clear();
            if (ParseTrees == null)
                return;
            foreach (ParseTree tree in ParseTrees)
            {
                if (tree == null)
                    continue;
                TreeNode root = new TreeNode();
                root.Tag = tree.Score.ToString();
                Stack nodes = new Stack();
                Stack items = new Stack();
                nodes.Push(tree.Root);
                items.Push(root);
                string[] words = textBox.Text.Split(' ');
                while (nodes.Count > 0)
                {
                    ParseNode node = (ParseNode)nodes.Pop();
                    TreeNode item = (TreeNode)items.Pop();
                    if (node.Senses != null)
                    {
                        string vlist = "";
                        foreach (NodeSense s in node.Senses)
                            vlist += s.Sense + "\n";
                        item.Tag = vlist;
                    }
                    item.Text = node.Goal + " : ";
                    for (int i = node.Start; i < node.End; i++)
                        item.Text += " " + tree.Words[i].ToString().ToLower();
                    if (node.Children == null)
                        continue;
                    for (int i = 0; i < node.Children.Count; i++)
                    {
                        nodes.Push(node.Children[i]);
                        TreeNode t = new TreeNode();
                        item.Nodes.Add(t);
                        items.Push(t);
                    }
                }
                root.ExpandAll();
                root.Collapse();
                treeView1.Nodes.Add(root);
            }
        }
        void LoadOntology()
        {
            if (Onto == null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                System.IO.FileStream fs = new System.IO.FileStream(
                    @"..\..\..\Ontology\Formatted OntoSem\Ontology.bin", FileMode.Open);
                Onto = (Ontology)bf.Deserialize(fs);
                fs.Close();
            }
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
            dockContainer1.Visible = false;
            dockControl1.Visible = false;
            dockControl2.Visible = false;
            dockControl3.Visible = false;
            //////////////////////////initialization ////////////////
            Wnlib.WNCommon.path = "C:\\Program Files (x86)\\WordNet\\2.1\\dict\\";
            //rr = new RulesReader("C:\\Rules.txt");
            rr = new RulesReader(Application.StartupPath + "\\EngParserCFG\\Rules.txt");
        }

        private void textMeaningRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox PB = new PictureBox();
            TMR_output tmro = new TMR_output();
            MMTMRoutput mmvm = new MMTMRoutput(tmro, tmr,new ViewingManeger.NewGoogleSearch());
            tmro.Show();
        }

        private void phasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
        }

        private void ontologyEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmOntologyEditor().Show();
            //frm.Show();
        }

        private void mREditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
			frmMREditor form = new frmMREditor();
            form.Show();
        }

        private void tMRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TMRDemo tmrdemo = new TMRDemo();
            tmrdemo.ShowDialog();
        }
        ArrayList AllParseTrees;

        private void parsingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parsing();
        }

        private void discourseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DiscourseAnalysis();
        }

        private void wSDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisambiguationAnalysis();
        }

        private void tMRToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TextMeaningRepresentation();
        }

        private void drawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Drawing();
        }

        private void filterTreesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterParseTrees();
        }
        List<int> SelectedParseTrees ;

        private void filterTreeDlgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterParseTreesForm frm = new FilterParseTreesForm(AllParseTrees,SelectedParseTrees);
            
            frm.ShowDialog();
            SelectedParseTrees = frm.SelectedParseTrees;
            SParseTrees = frm.NewParseTrees;
        }

        private void drawWithEdgeTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form6 form6 = new Form6(this);
            //Point p = this.panel2.AutoScrollPosition;
            MMViewManeger mmvm = new MMViewManeger(form6, tmr, true);
            form6.mmvm = mmvm;

            form6.Show();
            groupBox2.Visible = true;
            dockContainer1.Visible = true;
            dockControl1.Visible = true;
            dockControl2.Visible = true;
            dockControl3.Visible = true;
        }

        private void googleImageSearchSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GimSearchSettingsDlg gimDlg = new GimSearchSettingsDlg(gImSearchSettings);
            gimDlg.ShowDialog();
        }

        private void frontEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parsing();
            FilterParseTreesForm frm = new FilterParseTreesForm(SParseTrees,SelectedParseTrees);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                SelectedParseTrees = frm.SelectedParseTrees;
                SParseTrees = frm.NewParseTrees;
                DiscourseAnalysis();
                DisambiguationAnalysis();
            }
          
        }

        private void tMRToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            TextMeaningRepresentation();
        }

        private void drawingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterTMRForMultiLevel(tmr);
            Drawing();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BinaryFormatter frm = new BinaryFormatter();
                FileStream fs = new FileStream(saveFileDialog1.FileName,FileMode.OpenOrCreate,FileAccess.Write,FileShare.Write);
                frm.Serialize(fs, richTextBox1.Text);
                frm.Serialize(fs, AllParseTrees);
                frm.Serialize(fs, SelectedParseTrees);
                frm.Serialize(fs, SParseTrees);
                frm.Serialize(fs, tmr);
                fs.Close();
            }
        }

        private void openPrjToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BinaryFormatter frm = new BinaryFormatter();
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                richTextBox1.Text = (string)frm.Deserialize(sr.BaseStream);
                AllParseTrees = (ArrayList) frm.Deserialize(sr.BaseStream);
                SelectedParseTrees = (List<int>)frm.Deserialize(sr.BaseStream); 
                SParseTrees = (ArrayList)frm.Deserialize(sr.BaseStream);
                tmr = (MindMapTMR)frm.Deserialize(sr.BaseStream);
                sr.Close();
            }
        }

        List<Form6> forms = new List<Form6>();
        private void genAllDrawingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //FilterTMRForMultiLevel(tmr);
            for (int i = 0; i < forms.Count; i++)
            {
                forms[i].Close();
            }
            forms.Clear();
           
            // MMViewManeger mmvm = new MMViewManeger(pictureBox1, tmr);
          
            //Different types
            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 1;
                form6.Text = "Size All, Type All";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.All;
                settings.Imtype = ImageType.All;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 2;

                form6.Text = "Size Medium, Type All";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Medium;
                settings.Imtype = ImageType.All;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }//)
           // );

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 3;
                form6.Text = "Size Small, Type All";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Small;
                settings.Imtype = ImageType.All;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 4;
                form6.Text = "Size Auto, Type All";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.Imtype = ImageType.All;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings, DrawingSizeMode.AutoSize);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }//)
            //);


            #region ClipArt

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 5;
                form6.Text = "Size All, Type Clipart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.All;
                settings.Imtype = ImageType.Clipart;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 6;
                form6.Text = "Size Medium, Type Clipart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Medium;
                settings.Imtype = ImageType.Clipart;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 7;
                form6.Text = "Size Small, Type Clipart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Small;
                settings.Imtype = ImageType.Clipart;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 8;
                form6.Text = "Size Auto, Type Clipart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.Imtype = ImageType.Clipart;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings, DrawingSizeMode.AutoSize);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }//)
            //);
            #endregion


            #region LineArt

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 9;
                form6.Text = "Size All, Type Lineart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.All;
                settings.Imtype = ImageType.Lineart;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }
            //)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 10;
                form6.Text = "Size Medium, Type Lineart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Medium;
                settings.Imtype = ImageType.Lineart;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }
            //)            );

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 11;
                form6.Text = "Size Small, Type Lineart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Small;
                settings.Imtype = ImageType.Lineart;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }
            //));

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 12;
                form6.Text = "Size Auto, Type Lineart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.Imtype = ImageType.Lineart;
                MMViewManeger mmvm = new MMViewManeger(form6, tmr, false, settings, DrawingSizeMode.AutoSize);
                form6.mmvm = mmvm;
                forms.Add(form6);
                form6.Show();
            }
            //));
            #endregion
            groupBox2.Visible = true;
            dockContainer1.Visible = true;
            dockControl1.Visible = true;
            dockControl2.Visible = true;
            dockControl3.Visible = true;
           
        }
        public void SetFormLayoutToAll(Form6 frm)
        {
            foreach (Form6 f6 in forms)
            {
                if (f6 != frm)
                    f6.mmvm.AssignLayoutFrom(frm.mmvm);
            }

        }
        private void SaveToImage(Form6 form6, string path)
        {
            var bm = new Bitmap(form6.Width, form6.Height);
            form6.DrawToBitmap(bm, new Rectangle(0,0,form6.Width, form6.Height));
            bm.Save(path+".jpg", ImageFormat.Jpeg);
        }

        private void saveImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string path = saveFileDialog1.FileName;
            string pathwithoutDot = path.Substring(0, path.LastIndexOf('.'));
            foreach (Form6 fm in forms)
            {
                string pathFm="";
                switch (fm.Type)
                {
                    case 1:
                        pathFm = pathwithoutDot + "AllAll";
                        break;
                    case 2:
                        pathFm = pathwithoutDot + "MediumAll";

                        break;

                    case 3:
                        pathFm = pathwithoutDot + "SmallAll";
                        break;
                    case 4:
                        pathFm = pathwithoutDot + "AutoAll";
                        break;
                    case 5:
                        pathFm = pathwithoutDot + "AllClipArt";
                        break;
                    case 6:
                        pathFm = pathwithoutDot + "MediumClipArt";

                        break;

                    case 7:
                        pathFm = pathwithoutDot + "SmallClipArt";
                        break;
                    case 8:
                        pathFm = pathwithoutDot + "AutoClipArt";
                        break;

                    case 9:
                        pathFm = pathwithoutDot + "AllLineArt";
                        break;
                    case 10:
                        pathFm = pathwithoutDot + "MediumLineArt";
                        break;
                    case 11:
                        pathFm = pathwithoutDot + "SmallLineArt";
                        break;
                    case 12:
                        pathFm = pathwithoutDot + "AutoLineArt";
                        break;
                }
                SaveToImage(fm, pathFm);
            }
            BinaryFormatter bf = new BinaryFormatter();
            List<List<PointF>> layoutsToSave = new List<List<PointF>>();
            for (int i = 0; i < forms.Count; i++)
            {
                List<PointF> pts = new List<PointF>();
                for (int j = 0; j < forms[i].mmvm.Entities.Count; j++)
                {
                    pts.Add(forms[i].mmvm.Entities[j].Position); 
                }
                layoutsToSave.Add(pts);
            }
            StreamWriter sw = new StreamWriter(pathwithoutDot+".drw");
            bf.Serialize(sw.BaseStream, layoutsToSave);
            sw.Close();
        }

        public List<List<PointF>> LoadLocationsFrom(string fileName)
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            StreamReader sr = new StreamReader(fileName);
            List<List<PointF>> Allentities = (List<List<PointF>>)bf.Deserialize(sr.BaseStream);
            for (int i = 0; i < forms.Count; i++)
            {
                forms[i].mmvm.LoadLocationFrom(Allentities[i]);
            }
            sr.Close();

            return Allentities;
        }

        private void openSavedFormsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            BinaryFormatter bf = new BinaryFormatter();
            StreamWriter sr = new StreamWriter(openFileDialog1.FileName);
            forms = (List<Form6>)bf.Deserialize(sr.BaseStream);
            sr.Close();
            foreach(Form6 f in forms)
                f.Show();
        }

        private void genMultiLveelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tmr.MapMeaning();
            FilterTMRForMultiLevel(tmr);
            MultilevelGenerator.MultiLevel ML = new MultilevelGenerator.MultiLevel(this.tmr);
            MindMapTMR NewTMR = ML.Run();
            GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
            DrawingSizeMode drwSize = DrawingSizeMode.Normal;
            MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this,NewTMR, tmr, ML, settings, drwSize);
            MLForm.Show(this);

        }

        private void FilterTMRForMultiLevel(MindMapTMR tmr)
        {
            foreach (VerbFrame vf in tmr.VerbFrames)
            {
                bool Flag  = false;
                NounFrame nf = null;

                if (vf.CaseRoles.ContainsKey(CaseRole.Theme) && vf.CaseRoles[CaseRole.Theme].Count == 1)
                {
                    Flag = true;
                    nf= vf.CaseRoles[CaseRole.Theme][0];
                }
                else if(vf.CaseRoles.ContainsKey(CaseRole.location) && vf.CaseRoles[CaseRole.location].Count == 1)
                {
                    Flag = true;
                    nf= vf.CaseRoles[CaseRole.location][0];
                }
                else if(vf.CaseRoles.ContainsKey(CaseRole.unknown) && vf.CaseRoles[CaseRole.unknown].Count == 1)
                {
                    Flag = true;
                    nf= vf.CaseRoles[CaseRole.unknown][0];
                }
                if(Flag==true)
                {

                    if(nf.Ownerof.ContainsKey(CaseRole.location))
                        for (int i = 0; i < nf.Ownerof[CaseRole.location].Count; i++)
                        {
                            NounFrame nfi = nf.Ownerof[CaseRole.location][i];
                            if (nfi.Ownerof.Count == 0)
                            {
                                nf.Ownerof[CaseRole.location].RemoveAt(i);
                                vf.AddCaseRole(CaseRole.location, nfi);
                                i--;
                            }
                        }
                    if (nf.Ownerof.ContainsKey(CaseRole.unknown))
                        for (int i = 0; i < nf.Ownerof[CaseRole.unknown].Count; i++)
                        {
                            NounFrame nfi = nf.Ownerof[CaseRole.unknown][i];
                            if (nfi.Ownerof.Count == 0)
                            {
                                nf.Ownerof[CaseRole.unknown].RemoveAt(i);
                                vf.AddCaseRole(CaseRole.unknown, nfi);
                                i--;
                            }
                        }
                    if (nf.Ownerof.ContainsKey(CaseRole.time))
                        for (int i = 0; i < nf.Ownerof[CaseRole.time].Count; i++)
                        {
                            NounFrame nfi = nf.Ownerof[CaseRole.time][i];
                            if (nfi.Ownerof.Count == 0)
                            {
                                nf.Ownerof[CaseRole.time].RemoveAt(i);
                                vf.AddCaseRole(CaseRole.time, nfi);
                                i--;
                            }
                        }
                }
            }

            foreach (NounFrame NFK in tmr.Nounframes)
            {
                bool Flag = false;
                NounFrame nf = null;

                if (NFK.Ownerof.ContainsKey(CaseRole.Theme) && NFK.Ownerof[CaseRole.Theme].Count == 1)
                {
                    Flag = true;
                    nf = NFK.Ownerof[CaseRole.Theme][0];
                }
                else if (NFK.Ownerof.ContainsKey(CaseRole.location) && NFK.Ownerof[CaseRole.location].Count == 1)
                {
                    Flag = true;
                    nf = NFK.Ownerof[CaseRole.location][0];
                }
                else if (NFK.Ownerof.ContainsKey(CaseRole.unknown) && NFK.Ownerof[CaseRole.unknown].Count == 1)
                {
                    Flag = true;
                    nf = NFK.Ownerof[CaseRole.unknown][0];
                }
                else if (NFK.Ownerof.ContainsKey(CaseRole.Possession) && NFK.Ownerof[CaseRole.Possession].Count == 1)
                {
                    Flag = true;
                    nf = NFK.Ownerof[CaseRole.Possession][0];
                }
                if (Flag == true)
                {

                    if (nf.Ownerof.ContainsKey(CaseRole.location))
                        for (int i = 0; i < nf.Ownerof[CaseRole.location].Count; i++)
                        {
                            NounFrame nfi = nf.Ownerof[CaseRole.location][i];
                            if (nfi.Ownerof.Count == 0)
                            {
                                nf.Ownerof[CaseRole.location].RemoveAt(i);
                                NFK.AddCaseRolenouns(CaseRole.location, nfi);
                                i--;
                            }
                        }
                    if (nf.Ownerof.ContainsKey(CaseRole.unknown))
                        for (int i = 0; i < nf.Ownerof[CaseRole.unknown].Count; i++)
                        {
                            NounFrame nfi = nf.Ownerof[CaseRole.unknown][i];
                            if (nfi.Ownerof.Count == 0)
                            {
                                nf.Ownerof[CaseRole.unknown].RemoveAt(i);
                                NFK.AddCaseRolenouns(CaseRole.unknown, nfi);
                                i--;
                            }
                        }
                    if (nf.Ownerof.ContainsKey(CaseRole.time))
                        for (int i = 0; i < nf.Ownerof[CaseRole.time].Count; i++)
                        {
                            NounFrame nfi = nf.Ownerof[CaseRole.time][i];
                            if (nfi.Ownerof.Count == 0)
                            {
                                nf.Ownerof[CaseRole.time].RemoveAt(i);
                                NFK.AddCaseRolenouns(CaseRole.time, nfi);
                                i--;
                            }
                        }
                }
            }
        }

        private void showMappingsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tmr.MapMeaning();
            DisplayMappingsInFile();
        }

        public List<MultiLevelMMapForm> MLForms = new List<MultiLevelMMapForm>();

        public void SetMLLyaoutToAll(MultiLevelMMapForm frm)
        {
            List<List<PointF>> layout = frm.GETMLDrawingLayout();
            for (int i = 0; i < MLForms.Count; i++)
            {
                if (MLForms[i] != frm)
                    MLForms[i].LoadLayoutFrom(layout);
            }
 
        }
        private void genAllDrawingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tmr.MapMeaning();
            FilterTMRForMultiLevel(tmr);
            MultilevelGenerator.MultiLevel ML = new MultilevelGenerator.MultiLevel(this.tmr);
            MindMapTMR NewTMR = ML.Run();

            //pics pic = new pics(NewTMR, tmr, ML, 0, new List<pics>());

            for (int i = 0; i < forms.Count; i++)
            {
                MLForms[i].Close();
            }
            MLForms.Clear();

            // MMViewManeger mmvm = new MMViewManeger(pictureBox1, tmr);

            //Different types
            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
              
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.All;
                settings.Imtype = ImageType.All;
                DrawingSizeMode drwSize= DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 1;
                MLForm.Text = "Size All, Type All";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
               
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Medium;
                settings.Imtype = ImageType.All;
                DrawingSizeMode drwSize = DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 2;
                MLForm.Text = "Size Medium, Type All";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }//)
            // );

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
               
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Small;
                settings.Imtype = ImageType.All;
                DrawingSizeMode drwSize = DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 3;
                MLForm.Text = "Size Small, Type All";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
              
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.Imtype = ImageType.All;
            
                DrawingSizeMode drwSize = DrawingSizeMode.AutoSize;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 4;
                MLForm.Text = "Size Auto, Type All";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }//)
            //);


            #region ClipArt

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.All;
                settings.Imtype = ImageType.Clipart;
                DrawingSizeMode drwSize = DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 5;
                MLForm.Text = "Size All, Type Clipart";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
               
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Medium;
                settings.Imtype = ImageType.Clipart;
                DrawingSizeMode drwSize = DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 6;
                MLForm.Text = "Size Medium, Type Clipart";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                form6.Type = 7;
                form6.Text = "Size Small, Type Clipart";
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Small;
                settings.Imtype = ImageType.Clipart;
                DrawingSizeMode drwSize = DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 7;
                MLForm.Text = "Size Small, Type Clipart";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }//)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
               
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.Imtype = ImageType.Clipart;
                DrawingSizeMode drwSize = DrawingSizeMode.AutoSize;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 8;
                MLForm.Text = "Size Auto, Type Clipart";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }//)
            //);
            #endregion


            #region LineArt

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
               
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.All;
                settings.Imtype = ImageType.Lineart;
                DrawingSizeMode drwSize = DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 9;
                MLForm.Text = "Size All, Type Lineart";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }
            //)
            //);

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
               
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Medium;
                settings.Imtype = ImageType.Lineart;
                DrawingSizeMode drwSize = DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 10;
                MLForm.Text = "Size Medium, Type Lineart";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }
            //)            );

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.ImSize = ImageSize.Small;
                settings.Imtype = ImageType.Lineart;
                DrawingSizeMode drwSize = DrawingSizeMode.Normal;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 11;
                MLForm.Text = "Size Small, Type Lineart";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }
            //));

            //ThreadPool.QueueUserWorkItem(new WaitCallback(f =>
            {
                Form6 form6 = new Form6(this);
                
                GoogleImageSearchSettings settings = new GoogleImageSearchSettings();
                settings.Imtype = ImageType.Lineart;
                DrawingSizeMode drwSize = DrawingSizeMode.AutoSize;
                MultiLevelMMapForm MLForm = new MultiLevelMMapForm(this, NewTMR, tmr, ML, settings, drwSize);
                MLForm.Type = 12;
                MLForm.Text = "Size Auto, Type Lineart";
                MLForms.Add(MLForm);
                MLForm.Show(this);
            }
            //));
            #endregion
        }

        private void saveFormsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string path = openFileDialog1.FileName;
            string pathwithoutDot = path.Substring(0, path.LastIndexOf('.'))+"ML";
            foreach (MultiLevelMMapForm mlform in MLForms)
            {
                string pathFm = "";
                switch (mlform.Type)
                {
                    case 1:
                        pathFm = pathwithoutDot + "AllAll";
                        break;
                    case 2:
                        pathFm = pathwithoutDot + "MediumAll";

                        break;

                    case 3:
                        pathFm = pathwithoutDot + "SmallAll";
                        break;
                    case 4:
                        pathFm = pathwithoutDot + "AutoAll";
                        break;
                    case 5:
                        pathFm = pathwithoutDot + "AllClipArt";
                        break;
                    case 6:
                        pathFm = pathwithoutDot + "MediumClipArt";

                        break;

                    case 7:
                        pathFm = pathwithoutDot + "SmallClipArt";
                        break;
                    case 8:
                        pathFm = pathwithoutDot + "AutoClipArt";
                        break;

                    case 9:
                        pathFm = pathwithoutDot + "AllLineArt";
                        break;
                    case 10:
                        pathFm = pathwithoutDot + "MediumLineArt";
                        break;
                    case 11:
                        pathFm = pathwithoutDot + "SmallLineArt";
                        break;
                    case 12:
                        pathFm = pathwithoutDot + "AutoLineArt";
                        break;
                }
                mlform.SaveCaseGIF(pathFm + ".gif");
                Thread.Sleep(2000);
            }
            BinaryFormatter bf = new BinaryFormatter();
            List<List<List<PointF>>> layoutsToSave = new List<List<List<PointF>>>();
            for (int i = 0; i < forms.Count; i++)
            {
                layoutsToSave.Add(MLForms[i].GETMLDrawingLayout());
            }
            StreamWriter sw = new StreamWriter(pathwithoutDot + ".drw");
            bf.Serialize(sw.BaseStream, layoutsToSave);
            sw.Close();
        }





      
    }
}