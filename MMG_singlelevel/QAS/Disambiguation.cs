using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using WordsMatching;
using Wnlib;
using SyntacticAnalyzer;
namespace MMG
{
    class Disambiguation
    {
        public ArrayList SParseTrees = new ArrayList();
        public ArrayList DisambRes = new ArrayList();

        ArrayList WordInfoArr = new ArrayList();
        ArrayList WordInfoArr2 = new ArrayList();
        ArrayList Senses = new ArrayList();
        ArrayList NodesSenses = new ArrayList();
        ArrayList SensesNos = new ArrayList();
        ArrayList NewParseTreeSenses = new ArrayList();
        ArrayList ScoresOfParseTree = new ArrayList();
        ArrayList NewSParseTrees;
        List<String> Stems=new List<string>();
        int size = 0;
        MyWordInfo[] mwi2;
        MyWordInfo[] DisambiguateRes;

        public Disambiguation(ArrayList Spt)
        {
            SParseTrees = Spt;
        }
        public void beginDisambiguate()
        {
            Disambiguate(SParseTrees);
            ///////////////////////////get the text of senses ///////////////////////
            for (int i = 0; i < NewParseTreeSenses.Count; i++)
            {
                MyWordInfo[] mwiArr = (MyWordInfo[])NewParseTreeSenses[i];
                ParseTree pt;
                pt = (ParseTree)NewSParseTrees[i];
                AddArrStems(NewSParseTrees);
                for (int j = 0; j < mwiArr.Length; j++)
                {
                    Wnlib.PartOfSpeech p = Wnlib.PartOfSpeech.of((Wnlib.PartsOfSpeech)mwiArr[j].Pos);
                    try
                    {
                        //i need the stems here to get index
                        Wnlib.Index index = Wnlib.Index.lookup(mwiArr[j].Word.ToLower(), p);
                        SynSet sense = new SynSet(index, mwiArr[j].Sense, null);
                        NodesSenses.Add(sense.defn);
                        SensesNos.Add(mwiArr[j].Sense);

                        string s = mwiArr[j].Word.ToLower() + " : " + sense.defn;
                        DisambRes.Add(s);
                    }
                    catch
                    {
                        try
                        {
                            Wnlib.Index index = Wnlib.Index.lookup(Stems[j], p);
                            SynSet sense = new SynSet(index, mwiArr[j].Sense, null);
                            NodesSenses.Add(sense.defn);
                            SensesNos.Add(mwiArr[j].Sense);

                            string s = Stems[j].ToLower() + " : " + sense.defn;
                            DisambRes.Add(s);
                        }
                        catch
                        { };
                    };
                }
                Senses = NodesSenses;
            }
            //////////////////////////add sense text & sense no to the nodes//////////////////////////////
            AddNodesSenses(NewSParseTrees);
            //////////////////////////put the output parsetrees in SparseTree again//////////////////////////////
            SParseTrees = NewSParseTrees;

        }
        private void Disambiguate(ArrayList sparsetree)
        {
            foreach (ParseTree ps in SParseTrees)
            {
                ListifyParseTree(ps);
                PrepareInputArrOfDisambiguation();

                double Score;
                WordSenseDisambiguator wsd = new WordSenseDisambiguator();
                DisambiguateRes = new MyWordInfo[size];
                DisambiguateRes = wsd.MMG_Disambiguate(mwi2, out Score);
                ScoresOfParseTree.Add(Score);
                Senses.Add(DisambiguateRes);

            }
            checkParseTreeRepitition();
           // AddArrStems(NewSParseTrees);
            // fe problem fel disambiguation law ra7laha parsetrees fehom kaza pt l 1 sintence
            //law fe gomla liha 3 parse trees el code da hayetsaraf ?
        }
        private void PrepareInputArrOfDisambiguation()
        {
            int counter = 0;

            MyWordInfo[] mwi = new MyWordInfo[WordInfoArr2.Count];

            foreach (string s in WordInfoArr2)
            {

                string[] strarr = s.Split(':');
                if (strarr[0] == "VING")
                {
                    if (mwi[counter - 1].Pos == Wnlib.PartsOfSpeech.Verb)
                    {
                        mwi[counter] = new MyWordInfo(strarr[1], Wnlib.PartsOfSpeech.Verb);
                        counter++;
                    }
                    else
                    {
                        mwi[counter] = new MyWordInfo(strarr[1], Wnlib.PartsOfSpeech.Noun);
                        counter++;
                    }
                }
                else if (strarr[0] == "N" || (strarr[0].Contains("NPP") || strarr[0].Contains("PPJ")) )
                {
                    mwi[counter] = new MyWordInfo(strarr[1], Wnlib.PartsOfSpeech.Noun);
                    counter++;
                }
                else if (strarr[0] == "V" || strarr[0] == "VPSP" || strarr[0] == "BE1" )
                {
                    mwi[counter] = new MyWordInfo(strarr[1], Wnlib.PartsOfSpeech.Verb);
                    counter++;
                }
                else if (strarr[0].Contains("CPADJ") || strarr[0].Contains("ADJ"))
                {
                    mwi[counter] = new MyWordInfo(strarr[1], Wnlib.PartsOfSpeech.Adj);
                    counter++;
                }
                else if (strarr[0].Contains("PADV") || strarr[0].Contains("ADV"))
                {
                    mwi[counter] = new MyWordInfo(strarr[1], Wnlib.PartsOfSpeech.Adv);
                    counter++;
                }



                //MessageBox.Show((string)s);

            }
            size = 0;
            for (int k = 0; k < WordInfoArr2.Count; k++)
            {
                if (mwi[k] != null)
                {
                    size++;
                }

            }
            mwi2 = new MyWordInfo[size];
            for (int k = 0; k < size; k++)
            {
                mwi2[k] = mwi[k];
            }
        }
        private void ListifyParseTree(ParseTree parsetree)
        {
            WordInfoArr2 = new ArrayList();
            int j = 0;
            ArrayList arr = new ArrayList();
            if (parsetree.Root.Children != null)
            {
                ArrayList types = new ArrayList();
                FillWOrds(parsetree, parsetree.Root, ref WordInfoArr2);
            }
        }
        private void AddNodesSenses(ArrayList PTarr)
        {
            int j = 0;
            foreach (ParseTree PT in PTarr)
                if (PT.Root.Children != null)
                {
                    ArrayList types = new ArrayList();
                    FillSenses(PT, PT.Root, ref WordInfoArr2,ref j);
                    
                }
        }
        private void FillSenses(ParseTree parsetree, ParseNode node, ref ArrayList wordinfoArr,ref int j)
        {
            
            if (node.Children != null)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    ParseNode pn = (ParseNode)node.Children[i];
                    FillSenses(parsetree, pn, ref wordinfoArr,ref j);
                }
            }
            else
            {
                string str = node.Goal;
                if (str == "N" || str.Contains("NPP") || str.Contains("PPJ") || (str == "V") || str.Contains("CPADJ") || str.Contains("ADJ") || str.Contains("PADV") || str.Contains("ADV"))
                {
                    ///ta3deelat 5/7/////////
                    string [] spltstr=DisambRes[j].ToString().Split(':');
                    VerbSense VS = new VerbSense();
                    string[] spltstr2 =new string[10];
                    string[] spltstr3 = new string[10];
                    if (node.Senses != null && node.Goal.Contains("V"))
                    {
                        VS = (VerbSense)node.Senses[0];
                        spltstr2 = VS.Sense.ToString().Split('#');
                        spltstr3 = spltstr[0].Split(' ');

                        if (spltstr3[0] == spltstr2[0])
                        {
                            if (NodesSenses.Count > 0)
                            {
                                node.Sense = (string)NodesSenses[0];
                                node.SenseNo = (int)SensesNos[0];
                                SensesNos.RemoveAt(0);
                                NodesSenses.RemoveAt(0);
                                j++;
                            }

                        }
                    }
                    else
                    {
                        string NodeWord=SyntacticAnalyzer.SentenceParser.GetWordString(parsetree, node);
                        spltstr3 = spltstr[0].Split(' ');
                        if (spltstr3[0] == NodeWord.ToLower())
                        {
                            if (NodesSenses.Count > 0)
                            {
                                node.Sense = (string)NodesSenses[0];
                                node.SenseNo = (int)SensesNos[0];
                                SensesNos.RemoveAt(0);
                                NodesSenses.RemoveAt(0);
                                j++;
                            }

                        }
 
                    }
                    
                    
                }
            }
        }
        private void checkParseTreeRepitition()
        {
            bool flag = false;
            ArrayList ArrRepeatedPS = new ArrayList();
            ArrayList ArrIndxs = new ArrayList();
            NewSParseTrees = new ArrayList();

            ParseTree pt = (ParseTree)SParseTrees[0];
            ParseTree Ppt;
            ArrRepeatedPS.Add(pt);
            ArrIndxs.Add(0);

            bool flag2 = false;
            //check this part 
            for (int i = 1; i < SParseTrees.Count; i++)
            {
                pt = (ParseTree)SParseTrees[i];
                Ppt = (ParseTree)SParseTrees[i - 1];
                if (pt.Words.Count == Ppt.Words.Count)
                {
                    for (int j = 0; j < pt.Words.Count; j++)
                    {
                        if (pt.Words[j] != Ppt.Words[j])
                        {
                            flag = false;
                            ArrRepeatedPS.Add(pt);
                            ArrIndxs.Add(i);
                            break;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        flag2 = true;
                        ArrRepeatedPS.Add(pt);
                        ArrIndxs.Add(ArrIndxs[i - 1]);

                    }
                }
                else
                {
                    flag = false;
                    ArrRepeatedPS.Add(pt);
                    ArrIndxs.Add(i);
                    //el gomla msh heya heya
                }


            }
            //            ArrayList ParseTreesScore = new ArrayList();
            if (flag2)
            {
                int j = 1;
                NewSParseTrees = new ArrayList();
                NewParseTreeSenses = new ArrayList();
                NewSParseTrees.Add(SParseTrees[0]);
                NewParseTreeSenses.Add(Senses[0]);
                double max = (double)ScoresOfParseTree[0];
                for (int k = 1; k < SParseTrees.Count; k++)
                {
                    int current = (int)ArrIndxs[k];
                    int prev = (int)ArrIndxs[k - 1];
                    if (current == prev)
                    {

                        if ((double)ScoresOfParseTree[k] > max)
                        {
                            NewSParseTrees[j - 1] = SParseTrees[k];
                            NewParseTreeSenses[j - 1] = Senses[k];
                        }
                    }
                    else
                    {
                        j++;
                        max = (double)ScoresOfParseTree[k];
                        NewSParseTrees.Add(SParseTrees[k]);
                        NewParseTreeSenses.Add(Senses[k]);
                    }
                }
            }
            else
            {
                foreach (ParseTree ps in SParseTrees)
                {
                    NewSParseTrees.Add(ps);
                }
                for (int i = 0; i < Senses.Count; i++)
                {
                    NewParseTreeSenses.Add(Senses[i]);
                }
                // beytala3 el sense beta3 el noun ely bey refer leh el pronoun. :D 
            }
        }
        private void FillWOrds(ParseTree parsetree, ParseNode node, ref ArrayList wordinfoArr)
        {

            if (node.Children != null)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    ParseNode pn = (ParseNode)node.Children[i];
                    string WordString = SentenceParser.GetWordString(parsetree, node);
                    node.Text = WordString;
                    FillWOrds(parsetree, pn, ref wordinfoArr);
                    
                }
            }
            else
            {
                string WordString = SentenceParser.GetWordString(parsetree, node);
                node.Text = WordString;
                string goal = node.Goal;
                if (goal == "CNP") // a person's
                {
                    string s = "";

                    for (int i = node.Start; i < node.End; i++)
                    {
                        s += parsetree.Words[i].ToString();
                        if (node.End - i > 1)
                            s += " ";
                    }

                    string[] spltstr = s.Split(' ', '\'');
                    s = "N" + ":" + spltstr[1];
                    wordinfoArr.Add(s);
                }
                if ((goal == "NPP" || goal == "PPJ" || goal == "PRONOUN"))
                {
                    try
                    {
                        string s = goal + ":" + parsetree.ResolvedAnaphora[0];

                        wordinfoArr.Add(s);
                    }
                    catch
                    {
                        //throw new Exception("mala2ash refered word");
                    }
                }
                else
                {

                    string s = "";

                    for (int i = node.Start; i < node.End; i++)
                    {
                        //s += parsetree.Words[i].ToString().ToLower();
                        s += parsetree.Words[i].ToString();
                        if (node.End - i > 1)
                            s += " ";
                    }
                    if (node.End - node.Start > 1)
                    {
                        int c = 0;
                        string[] ss = s.Split(' ');
                        if (ss.Length > 1)
                        {
                            for (c = 0; c < ss.Length; c++)
                            {
                                if (ss[c] != "")
                                {
                                    s = "";
                                    s += ss[c];
                                    break;
                                }
                            }
                        }
                        //if(!s.Contains("_"))
                        //ss = s.Split(' ');
                        for (int k = c + 1; k < ss.Length; k++)
                            s += "_" + ss[k];
                    }
                    s = goal + ":" + s;
                    wordinfoArr.Add(s.ToUpper());
                }

            }


        }
        private void AddArrStems(ArrayList PTarr)
        {
            foreach (ParseTree PT in PTarr)
                if (PT.Root.Children != null)
                {
                    ArrayList types = new ArrayList();
                    FillStems(PT, PT.Root, ref Stems);
                }
        }
        private void FillStems(ParseTree parsetree, ParseNode node, ref List<string> stemsArr)
        {
            if (node.Children != null)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    ParseNode pn = (ParseNode)node.Children[i];

                    FillStems(parsetree, pn, ref stemsArr);
                }
            }
            else
            {
                string str = node.Goal;
                if (str == "N" || str.Contains("NPP") || str.Contains("PPJ") || (str == "V")  || str.Contains("ADJ") || str.Contains("PADV") || str.Contains("ADV"))
                {
                    ///ta3deelat 5/7/////////
                    if (node.Senses != null)
                    {
                        Type T = node.Senses[0].GetType();
                        VerbSense VS = (VerbSense)node.Senses[0];
                        string[] spltstr = VS.Sense.ToString().Split('#');
                        stemsArr.Add(spltstr[0]);
                    }
                }
            }
        }
    }
}
