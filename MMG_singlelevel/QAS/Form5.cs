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
            //////////////////////////initialization ////////////////
            Wnlib.WNCommon.path = "C:\\Program Files (x86)\\WordNet\\2.1\\dict\\";
            //rr = new RulesReader("C:\\Rules.txt");
            rr = new RulesReader(Application.StartupPath + "\\EngParserCFG\\Rules.txt");
            ////////////////////////////////////////////////
            BeginConversion();
        }

        private void BeginConversion()
        {
            Parsing();
            DiscourseAnalysis();
            DisambiguationAnalysis();
            DiscourseAnalysis();
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
            parsetrees = new ArrayList();

            foreach (ParseTree ps in SParseTrees)
            {
                parsetrees.Add(ModifyParseTree(ps));
            }
            MindMapTMR MINDMAP = new MindMapTMR(parsetrees, null);
            MINDMAP.BuildTMR();
            tmr = MINDMAP;
        }
        private void Drawing()
        {
            PictureBox PB = new PictureBox();
            MMViewManeger mmvm = new MMViewManeger(pictureBox1, tmr);
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
        }

        private void textMeaningRepresentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox PB = new PictureBox();
            TMR_output tmro = new TMR_output();
            MMTMRoutput mmvm = new MMTMRoutput(tmro, tmr);
            tmro.Show();
        }



      
    }
}