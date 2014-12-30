using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//
using Wnlib;
//
using WnLexicon;
using System.Runtime.Serialization.Formatters.Binary;

using WordsMatching;
using MMG;
using MindMapGenerator.Drawing_Management;
using DiscourseAnalysis;
using SyntacticAnalyzer;
using mmTMR;
using MindMapViewingManagement;
using System.Collections;
using MapperTool;
using System.Threading;


namespace MMG
{
    public partial class TMRDemo : Form
    {
        public TMRDemo()
        {
            InitializeComponent();
            this.Text = "MMG 2nd Edition";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConvertToMindMap();
        }

        RulesReader rulesReader;
        private void ConvertToMindMap()
        {
            Wnlib.WNCommon.path = "C:\\Program Files (x86)\\WordNet\\2.1\\dict\\";
            rulesReader = new RulesReader(Application.StartupPath + "\\EngParserCFG\\Rules.txt");
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread th = new Thread(new ThreadStart(BeginConversion));
            //BeginConversion();
        }

        private void BeginConversion()
        {
            Parsing();
            DiscourseAnalysis();
            DisambiguationAnalysis();
            ParserOutputChoice();//<-----------
            DiscourseAnalysis();
            DisambiguationAnalysis();
            ParserOutputChoice();//<-----------
            TextMeaningRepresentation();
            Drawing();
        }

        ArrayList SParseTrees;
        ArrayList[] Strees;
        ArrayList SentencesWords = new ArrayList();
        private void Parsing()
        {
            ArrayList words = new ArrayList();
            string[] sents = textBox1.Text.Trim().Split('.');
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
                SentenceParser sp = new SentenceParser(rulesReader, words);
                Strees[i] = sp.Parse();
                if (Strees[i] != null && Strees[i].Count > 0)
                {
                    SParseTrees.AddRange(Strees[i]);
                    SentencesWords.Add(words);
                }
            }	
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
                //------------------------
                Form2 f = new Form2();
                f.DisClasses = d.DistinctClasses;
                f.ParseTrees = NewSParseTrees;
                f.ShowDialog(this);
                f.Close();
                //--------------------------
                d.ReturnSParseTrees();
                SParseTrees = d.SParseTrees;
            }
            else
            {
                d.Begin(SParseTrees);
                //-------------------
                Form2 f = new Form2();
                f.DisClasses = d.DistinctClasses;
                f.ParseTrees = SParseTrees;
                f.ShowDialog(this);
                f.Close();
            }
            
        }

        ArrayList DisambRes = new ArrayList();
        ArrayList Indices = new ArrayList();
        public ArrayList ChosenParseTrees = new ArrayList();
        private void DisambiguationAnalysis()
        {
            Disambiguation D = new Disambiguation(SParseTrees);

            D.beginDisambiguate();
            DisambRes = D.DisambRes;
            SParseTrees = D.SParseTrees;
            Indices = D.ArrIndxs;
        }


        public List<int> NoOfSentences = new List<int>();
        List<int> NewIndices;// = new List<int>();
        List<int> ChosenParseTreesIndices = new List<int>();

        private void ParserOutputChoice()
        {
             for (int i = 0; i < Indices.Count; i++)
            {
                if (!NoOfSentences.Contains((int)Indices[i]))
                {
                    NoOfSentences.Add((int)Indices[i]);

                }

            }
            int No = 0;
            NewIndices = new List<int>(ChosenParseTrees.Count);

           // for(int j=0;j<SentenceID.coun
            //int index = 0;
            int x = (int)Indices[0];
            NewIndices.Add(No);
            for (int i = 1; i < Indices.Count; i++)
            {
                if ((int)Indices[i] == x)
                {
                    NewIndices.Add(No);
                }
                else
                {
                    x = (int)Indices[i];
                    No++;
                    NewIndices.Add(No);
                }
            }

            //edit
            //for (int i = 0; i < NoOfSentences.Count; i++)
            //{
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(0);

            ChosenParseTreesIndices.Add(1);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(2);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(7);
            ChosenParseTreesIndices.Add(1);
            ChosenParseTreesIndices.Add(4);
            ChosenParseTreesIndices.Add(1);
            ChosenParseTreesIndices.Add(1);
            ChosenParseTreesIndices.Add(1);
            ChosenParseTreesIndices.Add(1);
            ChosenParseTreesIndices.Add(0);
            ChosenParseTreesIndices.Add(3);
            //}


                for (int i = 0; i < NoOfSentences.Count; i++)
                {
                    int ChosenIndex = 0;

                    ChosenIndex = NewIndices.IndexOf(i);
                    //int count = int.Parse(dataGridView1[1, i].Value.ToString());
                    ChosenParseTrees.Add(SParseTrees[ChosenIndex + ChosenParseTreesIndices[i]]);

                }

            SParseTrees = ChosenParseTrees;

        }

        public ArrayList parsetrees;
        MindMapTMR tmr;
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

            foreach (VerbFrame VF in tmr.VerbFrames)
            {
                string name;
                if (VF.VerbNegation)
                {
                    name = VF.VerbName;
                    VF.VerbName = "";
                    VF.VerbName += "NOT ";
                    VF.VerbName += name;
                    //make a copy then clear and add NOT fel awel then concatenate the old one.
                }
            }
        }

        private object ModifyParseTree(ParseTree oldps)
        {
            ParseTree ps = (ParseTree)oldps.Clone();
            SentenceParser.LinkParents(ps.Root);
            return ps;
        }

        private void Drawing()
        {
            MultilevelGenerator.MultiLevel ML = new MultilevelGenerator.MultiLevel(this.tmr);
            MindMapTMR NewTMR = ML.Run();

            pics pic = new pics(NewTMR, tmr, ML,0, new List<pics>());
            //MMViewManeger mmvm = new MMViewManeger(pic, tmr);
            pic.ShowDialog();
        }

    }
}
