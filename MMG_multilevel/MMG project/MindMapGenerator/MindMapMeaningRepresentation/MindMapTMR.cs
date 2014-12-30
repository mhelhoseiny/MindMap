using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using SyntacticAnalyzer;
using OntologyLibrary;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using OntologyLibrary.OntoSem;
using WordsMatching;
using OurMindMapOntology;

namespace mmTMR
{
	[Serializable]
    public partial class MindMapTMR
    {
        //public Hashtable PrepositionFiller;
        //public Hashtable WordToConcept;
        //public Hashtable ADJAndADVFiller;

        //public Hashtable Concepts = new Hashtable();


        //public Ontology Onto;
        public OntologyLibrary.OntoSem.Ontology Onto_mo2a;
        public WordOlogy Wordology;
        public ArrayList ArrWordology;
        //public string _ontologyDirectoryPath =
        //        @"D:\GP\our work\MMG Final  8-7 8 belel\MMG Final  8-7 8 belel\MindMapGenerator\Ontology\Formatted OntoSem";
        //public string _ontologyDirectoryPath =
        //      @"F:\MMG project\MMG Final  8-7 8 belel\MindMapGenerator\Ontology\Formatted OntoSem";
        public static string OntologyDirectoryPath
        {
            get
            {
                return Application.StartupPath + "\\Formatted OntoSem";
            }
        }
        PrepositionOnto prepositionOnto = new PrepositionOnto();
       // public ParseNode pncmplst = new ParseNode();
        //Discourse output
        ArrayList _discourse = new ArrayList();
        //Parser output
        ArrayList _parseTrees = new ArrayList();
        public string w;
        public bool Sor=false;
        public string Sorname = "";
        public string WordStringafter = "";
        public string WordStringbefore = "";

        public bool adj_noun = false;
        public NounFrame _n_f;

        public bool Cor = false;
        public string Corname = "";
        public ArgumentType argumenttype;
        public NounFrame CurrentNounFrame;
        bool fromprep = false;
        public bool Prs = false;
        public string Prsname = "";
        public bool k = false;
        public bool isflag = false;



        public int SentenceID = 0;


        List<NounFrame> _nounframes = new List<NounFrame>();

        public List<NounFrame> Nounframes
        {
            get { return _nounframes; }
			set
			{
				_nounframes = value;
			}
        }
        List<VerbFrame> _verbFrames = new List<VerbFrame>();

        public List<VerbFrame> VerbFrames
        {
            get { return _verbFrames; }
			set
			{
				_verbFrames = value;
			}
        }
        void AddNounFrame(NounFrame nf)
        {
            AssignNFConcept(nf);
            Nounframes.Add(nf);
        }


        void AddVerbrame(VerbFrame vf)
        {
            AssignVFConcept(vf);
            VerbFrames.Add(vf);
        }


        /// <summary>
        /// builds an empty TMR for manual filling
        /// </summary>
        public MindMapTMR()
        {
            
        }

        /// <summary>
        /// builds a TMR from Disambiguated parse tree
        /// </summary>
        /// <param name="parseTrees"></param>
        /// <param name="discourse"></param>
        public MindMapTMR(ArrayList parseTrees, ArrayList discourse)
        {
            _parseTrees = parseTrees;
            _discourse = discourse;
            this._ontology = new MindMapOntology("1.owl");

        }
        VerbFrame currentVerbFrame = null;
        
    
        private List<NounFrame> currentsubjects=new List<NounFrame>();
        private List<NounFrame> currentObjects = new List<NounFrame>();
        

        public void BuildSentenceTMR(ParseTree parsetree)
        {
            //Test_DS(parsetree.Root);
            BuildNodeTMR(parsetree, parsetree.Root, -1);
        }

        #region New Code
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //Start of OUR CODE 2009
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>///////////////////////////////////////////////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// OUR CODE: elahi ya rab yeshtaghal ba2a!!!!!!!!!!!!!!!!!!!!!
        /// </summary>/////////////////////////////////////////////////////////////////////////////////////////////
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// zaii mabey2ool esmo keda bezzabt, get NounFrame associated actions, aka the CaseRole relations
        /// </summary>
        /// <param name="nounFrameIndex"></param>
        /// <returns></returns>
        public Dictionary<CaseRole, List<VerbFrame>> GetNounFrameAssociatedactions(int nounFrameIndex)
        {
            Dictionary<CaseRole, List<VerbFrame>> Associatedactions = new Dictionary<CaseRole, List<VerbFrame>>();
            NounFrame NF = Nounframes[nounFrameIndex];
            foreach (VerbFrame VF in VerbFrames)
            {
                for (int i = 0; i < Enum.GetNames(typeof(CaseRole)).Length; i++)
                {
                    if (VF.CaseRoles.ContainsKey((CaseRole)i))
                    {
                        List<NounFrame> ListofAssociatedNF = VF.CaseRoles[(CaseRole)i];
                        int count = ListofAssociatedNF.Count;
                        if (count != 0)
                        {
                            if (ListofAssociatedNF.Contains(NF))
                            {
                                if (Associatedactions.ContainsKey((CaseRole)i))
                                    Associatedactions[(CaseRole)i].Add(VF);
                                else
                                {
                                    List<VerbFrame> list = new List<VerbFrame>();
                                    list.Add(VF);
                                    Associatedactions.Add((CaseRole)i, list);
                                }
                            }
                        }
                    }
                }
            }
            return Associatedactions;
        }
        
        public List<VerbFrame> GET_CS(ParseNode node, ParseTree parseTree)
        {
            List<VerbFrame> verbframelist1 = new List<VerbFrame>();
            List<VerbFrame> verbframelist2 = new List<VerbFrame>();


            //CS=SS
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SS"))//
            {
                verbframelist1.AddRange(GET_SS((ParseNode)node.Children[0], parseTree));
            }
            //CS=INS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "INS"))//
            {
                verbframelist1.AddRange(GET_SS((ParseNode)node.Children[0], parseTree));
            }
            //CS=DS+SCLN+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "DS", "SCLN", "DS"))//
            {
                //2 independent closely related clauses, with no connecting words
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[0], parseTree));
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[2], parseTree));
            }
            //CS=IMS+SCLN+IMS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "IMS", "SCLN", "IMS"))//
            {
                //2 independent closely related clauses, with no connecting words
                verbframelist1.AddRange(GET_IMS((ParseNode)node.Children[0], parseTree));
                verbframelist2.AddRange(GET_IMS((ParseNode)node.Children[2], parseTree));
            }
            //CS=ETH+DS+OR+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "DS", "OR", "DS"))//
            {
                //ETH --> "EITHER"
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[1], parseTree));
                //OR --> exclusive or
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.ExclusiveOR, V2);
                    }
                }

            }
            //CS=NTH+DS+NOR+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "DS", "NOR", "DS"))//
            {
                //NTH -->"NEITHER"
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[1], parseTree));
                //NOR? --> inclusive or
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.InclusiveOR, V2);
                    }
                }
                //negation
                List<VerbFrame> tempList = new List<VerbFrame>();
                tempList.AddRange(verbframelist1);
                tempList.AddRange(verbframelist2);
                foreach (VerbFrame VF in tempList)
                {
                    VF.VerbNegation = true;
                }
            }
            //CS=NOTN+DS+BUTL+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "DS", "BUTL", "DS"))//
            {
                //NOTN -->"NOT_ONLY"
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[1], parseTree));
                //BUTL -->"BUT_ALSO" --> addition
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.Addition, V2);
                    }
                }

            }
            //CS=BTH+DS+AND+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "DS", "AND", "DS"))//
            {
                //BTH -->"BOTH"
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[1], parseTree));
                //AND? --> enumeration
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.Enumeration, V2);
                    }
                }
            }
            //CS=WTH+DS+OR+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WITH", "DS", "OR", "DS"))//
            {
                //WITH?
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[1], parseTree));
                //OR? --> inclusive or
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.InclusiveOR, V2);
                    }
                }

            }
            //CS=DS+COR+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "DS", "COR", "DS"))//
            {
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[0], parseTree));
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[2], parseTree));
                //COR
                #region COR
                string COR = ((ParseNode)node.Children[1]).Goal;
                if (COR == "FOR")//because/since////1 reason of 2
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Reason, V2);
                        }
                    }
                }
                else if (COR == "BUT")//except----unexpected result--------
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.UnExpectedResult, V2);
                        }
                    }
                }
                else if (COR == "YET")//she went to school yet she learned nothing----unexpected res <----
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.UnExpectedResult, V2);
                        }
                    }
                }
                else if (COR == "TO")//2 reason of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (COR == "IN_ORDER_TO")//2 reason of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (COR == "SO")//2 reason of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (COR == "AND")//enumeration
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Enumeration, V2);
                        }
                    }
                }
                else if (COR == "NOR")//inclusive or //the negation
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V2.VerbNegation = true;
                            V1.AddDomainRelation(DomainRelationType.InclusiveOR, V2);
                        }
                    }
                }
                else if (COR == "OR")//inclusive or
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.InclusiveOR, V2);
                        }
                    }
                }
                #endregion
            }
            //CS=IMS+COR+IMS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "IMS", "COR", "IMS"))//
            {
                verbframelist1.AddRange(GET_IMS((ParseNode)node.Children[0], parseTree));
                verbframelist2.AddRange(GET_IMS((ParseNode)node.Children[2], parseTree));
                //COR
                #region COR
                string COR = ((ParseNode)node.Children[1]).Goal;
                if (COR == "FOR")//since/because
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Reason, V2);
                        }
                    }
                }
                else if (COR == "BUT")//except
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.UnExpectedResult, V2);
                        }
                    }
                }
                else if (COR == "YET")
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.UnExpectedResult, V2);
                        }
                    }
                }
                else if (COR == "TO")//2 reason of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (COR == "IN_ORDER_TO")//2 reason of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (COR == "SO")//2 reason of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (COR == "AND")//enumeration
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Enumeration, V2);
                        }
                    }
                }
                else if (COR == "NOR")//inclusive or //the negation
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V2.VerbNegation = true;
                            V1.AddDomainRelation(DomainRelationType.InclusiveOR, V2);
                        }
                    }
                }
                else if (COR == "OR")//inclusive or
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.InclusiveOR, V2);
                        }
                    }
                }
                #endregion
            }
            //CS=ETH+IMS+OR+IMS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "IMS", "OR", "IMS"))//
            {
                //ETH? -->"EITHER"
                verbframelist1.AddRange(GET_IMS((ParseNode)node.Children[1], parseTree));
                //OR? --> exclusive or
                verbframelist2.AddRange(GET_IMS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.ExclusiveOR, V2);
                    }
                }
            }
            //CS=NTH+IMS+NOR+IMS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "IMS", "NOR", "IMS"))//
            {
                //NTH -->"NEITHER"
                verbframelist1.AddRange(GET_IMS((ParseNode)node.Children[1], parseTree));
                //NOR? -->"NOR" --> inclusive or
                verbframelist2.AddRange(GET_IMS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.InclusiveOR, V2);
                    }
                }
                //negation
                List<VerbFrame> tempList = new List<VerbFrame>();
                tempList.AddRange(verbframelist1);
                tempList.AddRange(verbframelist2);
                foreach (VerbFrame VF in tempList)
                {
                    VF.VerbNegation = true;
                }
            }
            //CS=NOTN+IMS+BUTL+IMS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "IMS", "BUTL", "IMS"))//
            {
                //NOTN? -->"NOT_ONLY"
                verbframelist1.AddRange(GET_IMS((ParseNode)node.Children[1], parseTree));
                //BUTL? -->"BUT_ALSO" --> addition
                verbframelist2.AddRange(GET_IMS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.Addition, V2);
                    }
                }
            }
            //CS=BTH+IMS+AND+IMS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "IMS", "AND", "IMS"))//
            {
                //BTH? -->"BOTH"
                verbframelist1.AddRange(GET_IMS((ParseNode)node.Children[1], parseTree));
                //AND? -->"AND" --> enumeration
                verbframelist2.AddRange(GET_IMS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.Enumeration, V2);
                    }
                }
            }
            //CS=WTH+IMS+OR+IMS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH", "IMS", "OR", "IMS"))//
            {
                //WTH? -->"WHETHER"
                verbframelist1.AddRange(GET_IMS((ParseNode)node.Children[1], parseTree));
                //OR? -->"OR" --> inclusive or
                verbframelist2.AddRange(GET_IMS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame V1 in verbframelist1)
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        V1.AddDomainRelation(DomainRelationType.InclusiveOR, V2);
                    }
                }
            }
            //CS=THE+CADJ+DS+CMA+THE+CADJ+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "THE", "CADJ", "DS", "CMA", "THE", "CADJ", "DS"))
            {
                //THE?
                //CADJ?
                MyWordInfo ADJ1 = new MyWordInfo();
                ADJ1.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ1.Sense = ((ParseNode)node.Children[1]).SenseNo;
                ((ParseNode)node.Children[1]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[1]);
                ADJ1.Word = ((ParseNode)node.Children[1]).Text;
                //DS
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[2], parseTree));
                //,
                //THE?
                //CADJ?
                MyWordInfo ADJ2 = new MyWordInfo();
                ADJ1.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ1.Sense = ((ParseNode)node.Children[5]).SenseNo;
                ((ParseNode)node.Children[5]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[5]);
                ADJ1.Word = ((ParseNode)node.Children[5]).Text;
                //DS
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[6], parseTree));
                //condition/comparison?????????????
            }
            //CS=THE+MORE+CPADJ+DS+CMA+THE+MORE+CPADJ+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "THE", "MORE", "CPADJ", "DS", "CMA", "THE", "MORE", "CPADJ", "DS"))
            {
                //THE?
                //MORE?
                //CPADJ?
                MyWordInfo ADJ1 = new MyWordInfo();
                ADJ1.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ1.Sense = ((ParseNode)node.Children[2]).SenseNo;
                ((ParseNode)node.Children[2]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[2]);
                ADJ1.Word = ((ParseNode)node.Children[2]).Text;
                //DS
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                //,
                //THE?
                //MORE?
                //CPADJ?
                MyWordInfo ADJ2 = new MyWordInfo();
                ADJ2.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ2.Sense = ((ParseNode)node.Children[7]).SenseNo;
                ((ParseNode)node.Children[2]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[7]);
                ADJ2.Word = ((ParseNode)node.Children[7]).Text;
                //DS
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[8], parseTree));
                //condition/comparison???????????????
            }
            //CS=THE+LESS+CPADJ+DS+CMA+THE+LESS+CPADJ+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "THE", "LESS", "CPADJ", "DS", "CMA", "THE", "LESS", "CPADJ", "DS"))
            {
                //THE?
                //LESS?
                //CPADJ?
                MyWordInfo ADJ1 = new MyWordInfo();
                ADJ1.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ1.Sense = ((ParseNode)node.Children[2]).SenseNo;
                ((ParseNode)node.Children[2]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[2]);
                ADJ1.Word = ((ParseNode)node.Children[2]).Text;
                //DS
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                //,
                //THE?
                //LESS?
                //CPADJ?
                MyWordInfo ADJ2 = new MyWordInfo();
                ADJ2.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ2.Sense = ((ParseNode)node.Children[7]).SenseNo;
                ((ParseNode)node.Children[7]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[7]);
                ADJ2.Word = ((ParseNode)node.Children[7]).Text;
                //DS
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[8], parseTree));
                //condition/comparison???????????????
            }
            //CS=THE+MORE+PADV+DS+CMA+THE+MORE+PADV+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "THE", "MORE", "PADV", "DS", "CMA", "THE", "MORE", "PADV", "DS"))
            {
                //THE?
                //MORE?
                List<MyWordInfo> PADV_1 = GET_PADV_adj((ParseNode)node.Children[2], parseTree);//????????????????
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame VF1 in verbframelist1)
                {
                    VF1.AdverbsInfo = PADV_1;
                }
                //,
                //THE?
                //MORE?
                List<MyWordInfo> PADV_2 = GET_PADV_adj((ParseNode)node.Children[7], parseTree);//????????????????
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[8], parseTree));
                foreach (VerbFrame VF2 in verbframelist2)
                {
                    VF2.AdverbsInfo = PADV_2;
                }
                //condition/comparison???????????????
            }
            //CS=THE+LESS+PADV+DS+CMA+THE+LESS+PADV+DS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "THE", "LESS", "PADV", "DS", "CMA", "THE", "LESS", "PADV", "DS"))
            {
                //THE?
                //LESS?
                List<MyWordInfo> PADV_1 = GET_PADV_adj((ParseNode)node.Children[4], parseTree);//????????????????
                verbframelist1.AddRange(Get_DS((ParseNode)node.Children[3], parseTree));
                foreach (VerbFrame VF1 in verbframelist1)
                {
                    VF1.AdverbsInfo = PADV_1;
                }
                //,
                //THE?
                //LESS?
                List<MyWordInfo> PADV_2 = GET_PADV_adj((ParseNode)node.Children[7], parseTree);//????????????????
                verbframelist2.AddRange(Get_DS((ParseNode)node.Children[8], parseTree));
                foreach (VerbFrame VF2 in verbframelist2)
                {
                    VF2.AdverbsInfo = PADV_2;
                }
                //condition/comparison???????????????
            }
            //CS=SS+SCLN+SOR+CMA+SS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SS", "SCLN", "SOR", "CMA", "SS"))
            {
                verbframelist1.AddRange(GET_SS((ParseNode)node.Children[0], parseTree));
                verbframelist2.AddRange(GET_SS((ParseNode)node.Children[4], parseTree));
                //SOR------
                ((ParseNode)node.Children[2]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[2]);
                #region SOR
                string SOR = ((ParseNode)node.Children[2]).Text;
                if (SOR == "BEFORE")
                { }
                else if (SOR == "AFTER")
                {
                }
                else if (SOR == "AS_A_RESULT_OF" || SOR == "AS_RESULT_OF")
                {
                }
                else if (SOR == "AS")
                {
                }
                else if (SOR == "SINCE")
                {
                }
                else if (SOR == "HENCE")
                {
                }
                else if (SOR == "THEREFORE")
                {
                }
                else if (SOR == "THAT")
                {
                }
                else if (SOR == "ALTHOUGH" || SOR == "THOUGH" || SOR == "EVEN_THOUGH" || SOR == "EVEN_IF")
                // Concessive <--- A is typically not believed to be a result of B.
                {
                }
                else if (SOR == "AS_IF" || SOR == "AS_THOUGH")//2 comparison of 1 (conjunction domain)
                {
                }
                else if (SOR == "AS_LONG_AS")//concurrent
                {
                }
                else if (SOR == "AS_MUCH_AS")
                {
                }
                else if (SOR == "AS_SOON_AS")//2 after 1
                {
                }
                else if (SOR == "BECAUSE")//1 reason of 2
                {
                }
                else if (SOR == "IF" || SOR == "IF_ONLY")//condition.
                {
                }
                else if (SOR == "INASMUCH" || SOR == "INASMUCH_AS")//comparison
                {
                }
                else if (SOR == "IN_ORDER_THAT")
                {
                }
                else if (SOR == "LEST")
                {
                }
                else if (SOR == "NOW_THAT")
                {
                }
                else if (SOR == "SO_THAT")
                {
                }
                else if (SOR == "ONCE")//(once = until = till)
                {
                }
                else if (SOR == "UNTIL" || SOR == "TILL")
                {

                }
                else if (SOR == "UNLESS")
                {
                }
                else if (SOR == "WHEN")
                {
                }
                else if (SOR == "WHENEVER")
                {
                }
                else if (SOR == "WHERE") //place???               
                {
                }
                else if (SOR == "WHILE")//concurrent
                {
                }
                else if (SOR == "WHEREAS")
                {
                }
                else if (SOR == "PROVIDED" || SOR == "PROVIDED_THAT" || SOR == "PROVIDING" || SOR == "PROVIDING_THAT")//condition
                {
                }
                else if (SOR == "IN_CASE" || SOR == "IN_CASE_THAT")//|| SOR == "IN_CASE_OF"
                {
                }
                else if (SOR == "THAN")
                { }
                else if (SOR == "HOW")
                { }
                else if (SOR == "RATHER_THAN")
                { }
                else if (SOR == "WHEREVER")
                { }
                else if (SOR == "WHAT")
                { }
                else if (SOR == "WHETHER")
                { }
                #endregion
            }
            //CS=SS+SOR+SS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SS", "SOR", "SS"))
            {
                verbframelist1.AddRange(GET_SS((ParseNode)node.Children[0], parseTree));
                verbframelist2.AddRange(GET_SS((ParseNode)node.Children[2], parseTree));

                ((ParseNode)node.Children[1]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[1]);
                #region SOR "other"
                string SOR = ((ParseNode)node.Children[1]).Text;
                if (SOR == "BEFORE")//temporal: 1 before 2
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.Before, V2);
                        }
                    }
                }
                else if (SOR == "AFTER")//temporal: 1 after 2
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.After, V2);
                        }
                    }
                }
                else if (SOR == "AS_A_RESULT_OF" || SOR == "AS_RESULT_OF")//1 RESULT OF 2
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.ExpectedResult, V1);
                            //dunno about whether the result is expected or not!!!!!
                        }
                    }
                }
                else if (SOR == "AS")//temporal or domain? domain: he ate as he was hungry. as: meaning because
                //temporal: concurrent won! eg: he did this as she did that.
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.Concurrent, V2);
                        }
                    }
                }
                else if (SOR == "SINCE")// temporal or domain???? domain: 1 reason of 2. eg: i will do this since you did that
                //temporal: he has been running since he ate this afternoon?????????????????
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);

                        }
                    }
                }
                else if (SOR == "HENCE")// 2 result of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.ExpectedResult, V1);
                            //dunno about whether the result is expected or not!!!!!
                        }
                    }
                }
                else if (SOR == "THEREFORE")// 2 result of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.ExpectedResult, V1);
                            //dunno about whether the result is expected or not!!!!!
                        }
                    }
                }

                else if (SOR == "THAT")//2 reason of 1? he was v hungry that he ate the whole thing
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Reason, V2);
                        }
                    }
                }
                else if (SOR == "ALTHOUGH" || SOR == "THOUGH" || SOR == "EVEN_THOUGH" || SOR == "EVEN_IF")//x although y. 
                // Concessive <--- A is typically not believed to be a result of B.
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Concessive, V2);
                        }
                    }
                }
                else if (SOR == "AS_IF" || SOR == "AS_THOUGH")//2 comparison of 1 (conjunction domain)
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Comparison, V2);
                        }
                    }
                }
                else if (SOR == "AS_LONG_AS")//concurrent or condition. condition won!
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Condition, V2);
                        }
                    }
                }
                else if (SOR == "AS_MUCH_AS")//eg: he ate as much as he drank... comparison
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Comparison, V2);
                        }
                    }
                }
                else if (SOR == "AS_SOON_AS")//1 after 2
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.After, V2);
                        }
                    }
                }
                else if (SOR == "BECAUSE")//2 reason of 1
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Reason, V2);
                        }
                    }
                }
                else if (SOR == "IF" || SOR == "IF_ONLY")//condition. 2 condition of 1
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Condition, V2);
                        }
                    }
                }
                else if (SOR == "INASMUCH" || SOR == "INASMUCH_AS")//comparison
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Comparison, V2);
                        }
                    }
                }
                else if (SOR == "IN_ORDER_THAT")// 1 reason of 2
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (SOR == "LEST")//not of 2 reason of 1
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V2.VerbNegation = true;
                            V1.AddDomainRelation(DomainRelationType.Reason, V2);
                        }
                    }
                }
                else if (SOR == "NOW_THAT")//1 condition of 2? eg: he can play now that he can run.                    
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "SO_THAT")// 1 reason of 2 eg: he ran so that he can play.
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (SOR == "ONCE")//(once = until = till), 2 before 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddTemporalRelation(TemporalRelationType.Before, V1);
                        }
                    }
                }
                else if (SOR == "UNTIL" || SOR == "TILL")// 1 before 2
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.Before, V2);
                        }
                    }
                }
                else if (SOR == "UNLESS")//condition, but... the negation?? eg: he will not sleep unless he is fed.
                //i will do it unless u change ur mind 
                // condition of 1 is the negation of 2
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Condition, V2);
                        }
                    }
                }
                else if (SOR == "WHEN")//CONCURRENT
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.Concurrent, V2);
                        }
                    }
                }
                else if (SOR == "WHENEVER")//concurrent or after? eg: he will run whenever she hides (she hides first, then he runs)
                //condition. eg: i will do this whenever u say so. eg: the filly was trained to stop whenever a jockey used a whip
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Condition, V2);
                        }
                    }
                }
                else if (SOR == "WHERE")
                //eg: he found a sandwich bar where he could get a snack. enablment? event A enabled event B.
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Enablement, V2);
                        }
                    }
                }

                else if (SOR == "WHILE")//concurrent
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.Concurrent, V2);
                        }
                    }
                }

                else if (SOR == "WHEREAS")//comparison
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Comparison, V2);
                        }
                    }
                }
                else if (SOR == "PROVIDED" || SOR == "PROVIDED_THAT" || SOR == "PROVIDING" || SOR == "PROVIDING_THAT")//condition
                {// 2 condition of 1
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Condition, V2);
                        }
                    }
                }
                else if (SOR == "IN_CASE" || SOR == "IN_CASE_THAT")//|| SOR == "IN_CASE_OF"
                {// 2 condition of 1
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Condition, V2);
                        }
                    }
                }
                //---------------------??               
                else if (SOR == "HOW")// manner. eg:
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.How, V2);
                        }
                    }
                }
                else if (SOR == "WHEREVER")// place
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.place, V2);
                        }
                    }
                }
                else if (SOR == "RATHER_THAN")//"and not"
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V2.VerbNegation = true;
                            V1.AddDomainRelation(DomainRelationType.Enumeration, V2);
                        }
                    }
                }
                else if (SOR == "THAN")//except..eg:We had no choice than to return home.//when..eg:We had barely arrived than we had to leave again.
                //Non-volitional --> The relation between a non-intentional action or a state of an intelligent agent and its consequence.
                { }
                else if (SOR == "WHAT")
                { }
                else if (SOR == "WHETHER")//if
                //eg: We should find out whether the museum is open.
                { }
                #endregion
            }
            //CS=SOR+SS+CMA+SS
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOR", "SS", "CMA", "SS"))
            {
                verbframelist1.AddRange(GET_SS((ParseNode)node.Children[1], parseTree));
                verbframelist2.AddRange(GET_SS((ParseNode)node.Children[3], parseTree));
                //SOR------
                ((ParseNode)node.Children[0]).Text = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
                #region SOR
                string SOR = ((ParseNode)node.Children[0]).Text;
                if (SOR == "BEFORE")// 2 before 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddTemporalRelation(TemporalRelationType.Before, V1);
                        }
                    }
                }
                else if (SOR == "AFTER")//2 after 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddTemporalRelation(TemporalRelationType.After, V1);
                        }
                    }
                }
                else if (SOR == "AS_A_RESULT_OF" || SOR == "AS_RESULT_OF")// 2 result of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.ExpectedResult, V1);
                        }
                    }
                }
                else if (SOR == "AS")//concurrent
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.Concurrent, V2);
                        }
                    }
                }
                else if (SOR == "SINCE")// temporal ---> 2 after 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddTemporalRelation(TemporalRelationType.After, V1);
                        }
                    }
                }
                else if (SOR == "HENCE")//for this reason-- 2 reason of 1
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (SOR == "ALTHOUGH" || SOR == "THOUGH" || SOR == "EVEN_THOUGH" || SOR == "EVEN_IF")
                // Concessive <--- A is typically not believed to be a result of B.
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Concessive, V1);
                        }
                    }
                }
                else if (SOR == "AS_LONG_AS")//1 condition of 2
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "AS_MUCH_AS")//eg: as much as he drank, he ate... comparison
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Comparison, V1);
                        }
                    }
                }
                else if (SOR == "AS_SOON_AS")//2 after 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddTemporalRelation(TemporalRelationType.After, V1);
                        }
                    }
                }
                else if (SOR == "BECAUSE")//2 reason of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (SOR == "IF" || SOR == "IF_ONLY")//condition. 1 condition of 2
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "INASMUCH" || SOR == "INASMUCH_AS")//comparison
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V2.AddDomainRelation(DomainRelationType.Comparison, V1);
                        }
                    }
                }
                else if (SOR == "IN_CASE" || SOR == "IN_CASE_THAT")//|| SOR == "IN_CASE_OF"//1 condition of 2
                {

                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "NOW_THAT" || SOR == "SO_THAT")// 1 condition of 2
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "ONCE")//condition
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "UNTIL" || SOR == "TILL")//2 before 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddTemporalRelation(TemporalRelationType.Before, V1);
                        }
                    }
                }
                else if (SOR == "WHEN")//concurrent
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddTemporalRelation(TemporalRelationType.Concurrent, V1);
                        }
                    }
                }
                else if (SOR == "WHENEVER") //1 condition of 2               
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "WHILE")//concurrent
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddTemporalRelation(TemporalRelationType.Concurrent, V2);
                        }
                    }
                }
                else if (SOR == "PROVIDED" || SOR == "PROVIDED_THAT" || SOR == "PROVIDING" || SOR == "PROVIDING_THAT")//condition
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "WHERE") //place???               
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.place, V2);
                        }
                    }
                }
                else if (SOR == "WHEREVER")
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.place, V2);
                        }
                    }
                }
                else if (SOR == "HOW")//2 manner of 1
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.How, V2);
                        }
                    }
                }
                else if (SOR == "WHAT")//1 reason of 2
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (SOR == "RATHER_THAN")//"and not"
                {//eg: rather than play the game, he went to study.
                    //study and not play
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V1.VerbNegation = true;
                            V2.AddDomainRelation(DomainRelationType.Enumeration, V1);
                        }
                    }
                }
                else if (SOR == "UNLESS") // neg of 1 condition of 2               
                {
                    foreach (VerbFrame V1 in verbframelist1)
                    {
                        V1.VerbNegation = true;
                        foreach (VerbFrame V2 in verbframelist2)
                        {
                            V1.AddDomainRelation(DomainRelationType.Condition, V2);
                        }
                    }
                }
                else if (SOR == "LEST")// negation of 1 reason of 2
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V1.VerbNegation = true;
                            V2.AddDomainRelation(DomainRelationType.Reason, V1);
                        }
                    }
                }
                else if (SOR == "IN_ORDER_THAT")// 2 condition of 1
                {
                    foreach (VerbFrame V2 in verbframelist2)
                    {
                        foreach (VerbFrame V1 in verbframelist1)
                        {
                            V2.AddDomainRelation(DomainRelationType.Condition, V1);
                        }
                    }
                }
                else if (SOR == "THEREFORE")//???
                { }
                else if (SOR == "THAT")//???
                { }
                else if (SOR == "AS_IF" || SOR == "AS_THOUGH")
                { }
                else if (SOR == "WHEREAS")
                { }
                else if (SOR == "THAN")
                { }
                else if (SOR == "WHETHER")
                { }
                #endregion
            }
            verbframelist1.AddRange(verbframelist2);
            return verbframelist1;
        }

        private void GET_S(ParseNode node, ParseTree parsetree)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            List<VerbFrame> verbframelist = new List<VerbFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOR", "PRPHS", "CMA", "CS"))
            {
                nounframelist.AddRange(GET_PRPHS((ParseNode)node.Children[1], parsetree));
                verbframelist.AddRange(GET_CS((ParseNode)node.Children[3], parsetree));
                //SOR<<<<<<<<<--------------------------------------------------------------------------
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CS"))
            {
                verbframelist.AddRange(GET_CS((ParseNode)node.Children[0], parsetree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CS", "CMA", "ABPH"))
            {
                verbframelist.AddRange(GET_CS((ParseNode)node.Children[0], parsetree));
                foreach (VerbFrame vf in verbframelist)
                {
                    nounframelist.AddRange(GET_ABPH((ParseNode)node.Children[2], parsetree, vf,null));
                }///////////////////////////////////////////????????????????????????????????????????????check
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPHS", "CMA", "CS"))
            {
                nounframelist.AddRange(GET_PRPHS((ParseNode)node.Children[0], parsetree));
                verbframelist.AddRange(GET_CS((ParseNode)node.Children[2], parsetree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS", "CMA", "CS"))
            {
                List<MyWordInfo> advs = new List<MyWordInfo>();
                advs.AddRange(GET_ADVS((ParseNode)node.Children[0], parsetree));
                verbframelist.AddRange(GET_CS((ParseNode)node.Children[2], parsetree));
                foreach (VerbFrame VF in verbframelist)
                {
                    VF.AdverbsInfo = advs;
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NPF", "CMA", "CS"))
            {
                nounframelist.AddRange(GET_NPF((ParseNode)node.Children[0], parsetree));
                verbframelist.AddRange(GET_CS((ParseNode)node.Children[2], parsetree));
            }

        }

        private List<NounFrame> GET_PRPHS(ParseNode node, ParseTree parsetree)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPH"))
            {
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[0], parsetree,null));
            }
            return nounframelist;
        }

        private List<NounFrame> GET_NPF(ParseNode node, ParseTree parsetree)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> adjs = new List<MyWordInfo>();
            //NPF=AVJ+NN+FAVJ
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "AVJ", "NN", "FAVJ"))
            { }
            //NPF=NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NN", "FAVJ"))
            { }
            //NPF=ARC+AVJ+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "NN", "FAVJ"))
            { }
            //NPF=ARC+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "NN", "FAVJ"))
            { }
            //NPF=PPJ+AVJ+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "AVJ", "NN", "FAVJ"))
            { }
            //NPF=PPJ+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "NN", "FAVJ"))
            { }
            //NPF=CNP+AVJ+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CNP", "AVJ", "NN", "FAVJ"))
            { }
            //NPF=CNP+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CNP", "NN", "FAVJ"))
            { }
            //NPF=ARC+CNP+AVJ+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "CNP", "AVJ", "NN", "FAVJ"))
            { }
            //NPF=ARC+CNP+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "CNP", "NN", "FAVJ"))
            { }
            //NPF=ARC+AVJ+OF+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "OF", "NN", "FAVJ"))
            { }
            //NPF=ARC+OF+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "OF", "NN", "FAVJ"))
            { }
            //NPF=ARC+AVJ+OF+ARC+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "OF", "ARC", "NN", "FAVJ"))
            { }
            //NPF=ARC+OF+ARC+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "OF", "ARC", "AVJ", "NN", "FAVJ"))
            { }
            //NPF=ALL+AVJ+ARC+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ALL", "AVJ", "ARC", "NN", "FAVJ"))
            { }
            //NPF=ALL+ARC+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ALL", "ARC", "NN", "FAVJ"))
            { }
            return nounframelist;
        }
        

        public List<VerbFrame> Get_DS(ParseNode node, ParseTree parseTree)
        {

            List<VerbFrame> vfs = new List<VerbFrame>();
            List<NounFrame> nfs = new List<NounFrame>(); ;
            List<NounFrame> nfs_Temp = new List<NounFrame>();


            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SBJ", "PRDS"))
            {
                //obtain subject then attach it to verb
                //nfs.AddRange(GET_SBJ((ParseNode)node.Children[0], parseTree, verbFrame));
                nfs.AddRange(GET_SBJ((ParseNode)node.Children[0], parseTree, nfs_Temp));


                vfs.AddRange(GET_PRDS(((ParseNode)node.Children[1]), parseTree, nfs)); ;


                List<NounFrame> temp = new List<NounFrame>();
               
                //foreach (VerbFrame vf in vfs)
                //{
                //    if (vf.VerbName == "HAS" || vf.VerbName == "HAVE" || vf.VerbName == "HAD")
                //    {
                //        //law 3andi ali has caza..ali agent w kaza theme...asheel el caseroles di wel VF 3ashan ali owner of el
                //        foreach (CaseRole cr in vf.CaseRoles)
                //        {
                //            if (vf.CaseRoles.ContainsKey(cr))
                //            {
                //                temp.AddRange(vf.CaseRoles.Values[cr]);
                //                vf.CaseRoles.Remove(cr);
                //            }
                //        }


                //    }
                //}
                

                //return vfs;
            }
            return vfs;
        }

        //habolli
        public List<NounFrame> Get_DS_noun(ParseNode node, ParseTree parseTree)
        {

            List<VerbFrame> vfs = new List<VerbFrame>();
            List<NounFrame> nfs = new List<NounFrame>(); ;
            List<NounFrame> nfs_Temp = new List<NounFrame>();


            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SBJ", "PRDS"))
            {
                //obtain subject then attach it to verb
                //nfs.AddRange(GET_SBJ((ParseNode)node.Children[0], parseTree, verbFrame));
                nfs.AddRange(GET_SBJ((ParseNode)node.Children[0], parseTree, nfs_Temp));


                vfs.AddRange(GET_PRDS(((ParseNode)node.Children[1]), parseTree, nfs)); ;


                // di keda ghalat 3ashan kol ewl nounframes elli ba3d keda marboota be el verb da
                //ShakesPear is a great writer in the history of literature
                //    for (int i = 0; i < nfs.Count; i++)
                //    {
                //        vfv.AddCaseRole(CaseRole.Agent, nfs[i]);
                //    }

                //    vfs.Add(vfv);
                //}


                //return vfs;
            }
            return nfs;
        }
        //habolli

        //public List<VerbFrame> Get_DS(ParseNode node,ParseTree parseTree)
        //{
        //    List<VerbFrame> vfs = new List<VerbFrame>();
        //    List<NounFrame> nfs;


        //    if (((ParseNode)node.Children[0]).Goal == "SBJ")
        //    {

        //        nfs = GET_SBJ((ParseNode)node.Children[0],parseTree);

        //    }

        //    if (((ParseNode)node.Children[1]).Goal == "PRDS")
        //    {

        //       // int i = 0;
        //       // while (node.Children[1]!=null)
              
        //          // VerbFrame vfv = GET_PRDS((ParseNode)((ParseNode)node.Children[1]).Children[i]);
        //           List<VerbFrame> vfv = GET_PRDS(((ParseNode)node.Children[1]),parseTree);
                   
        //            vfs.AddRange(vfv);
        //          //  i++;
                
        //    }

        //    return vfs;
        //}

     

        //public List<NounFrame> GET_CMPS(ParseNode node, ParseTree parseTree)
        //{
        //   List<NounFrame> nflist = new List<NounFrame>();
        //    //List<VerbFrame> vflist = new List<VerbFrame>();
        //    int i = 0;
        //    foreach (ParseNode n in node.Children)
        //    {
        //        if (((ParseNode)n).Goal == "CMP")
        //        {
        //            nflist.AddRange(GET_CMP((ParseNode)node.Children[i], parseTree));
        //            //vflist.Add(GET_V((ParseNode)node.Children[i], parseTree));
        //        }
        //        i++;
        //    }
        //    return nflist;

        //}

        


        //public List<NounFrame> GET_OBJ(ParseNode node, ParseTree parseTree)
        //{
        //    //incomplete.....foreach with i
        //    List<NounFrame> nflist = new List<NounFrame>();
        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ"))
        //    {
        //        nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree));
        //    }
        //    //if (((ParseNode)node.Children[0]).Goal == "SOBJ")
        //    //{
        //    //    nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree));
        //    //}
        //    return nflist;
        //}

        #region ADD_YA_MERO
        public List<VerbFrame> GET_PRDS(ParseNode node, ParseTree parseTree, List<NounFrame> Subjects)
        {

            //transitive and passive verbs were checked for in PRDS in old code....the same here???

            VerbFrame VF = new VerbFrame(node);
            List<VerbFrame> VFL = new List<VerbFrame>();
            if (node.Children != null)
            {
                //int i = 0;


                if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRDS1"))
                {

                    VFL.AddRange(GET_PRDS1((ParseNode)node.Children[0], parseTree,Subjects));

                }

                else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRD"))
                {
                    //checking this now
                    VF = GET_PRD((ParseNode)node.Children[0], parseTree, Subjects);
                    
                    //I am ahmad....caserole?
                    //they are playing...they agents
                    //ali is a student....ali theme w student co-theme
                    //ali is good...good adjective
                    if (VF.VerbName == "IS" || VF.VerbName == "WAS" || VF.VerbName == "ARE" || VF.VerbName == "WERE")
                    {
                        for (int i = 0; i < Subjects.Count; i++)
                        {

                            VF.AddCaseRole(CaseRole.Theme, Subjects[i]);

                        }
                    }
                    // Mohamed ELhoseiny 03/16/2012 Becasue the logic seems incorrect
                    //else if (VF.VerbName == "HAS" || VF.VerbName == "HAVE" || VF.VerbName == "HAD")
                    //{
                    //    List<NounFrame> Temp = new List<NounFrame>();
                    //    foreach (NounFrame nf in Subjects)
                    //    {//le kol subj law la2et el VF feeh cothem aw theme bashelhom waddi el nfs lel nf da ka owner of
                    //        if (VF.CaseRoles.ContainsKey(CaseRole.Cotheme))
                    //        { 
                    //            //verbFrame
                    //           Temp.AddRange(VF.CaseRoles[CaseRole.Cotheme]);
                    //           if (Temp.Count != 0)
                    //           {
                    //               for (int i = 0; i < Temp.Count; i++)
                    //               {
                    //                   VF.CaseRoles[CaseRole.Cotheme].Remove(Temp[i]);
                    //               }
                    //               VF.CaseRoles.Remove(CaseRole.Cotheme);
                    //           }
                    //            //verbFrame.CaseRoles[CaseRole.Cotheme].Remove(nf);
                    //        }
                    //        else if (VF.CaseRoles.ContainsKey(CaseRole.Theme))
                    //        {
                    //            Temp.AddRange(VF.CaseRoles[CaseRole.Theme]);
                    //            if (Temp.Count != 0)
                    //            {
                    //                for (int i = 0; i < Temp.Count; i++)
                    //                {
                    //                    VF.CaseRoles[CaseRole.Theme].Remove(Temp[i]);

                    //                }
                    //                VF.CaseRoles.Remove(CaseRole.Theme);
                    //            }
                    //            //verbFrame.CaseRoles[CaseRole.Theme].Remove(nf);
                    //        }
                    //        nf.Ownerof.Add(CaseRole.OwnerOf, Temp);
                            
                    //   }
                    //    VerbFrames.Remove(VF);
                    //}

                    else if (!VF.Passive && Subjects != null)
                    {
                        for (int i = 0; i < Subjects.Count; i++)
                        {
                            VF.AddCaseRole(CaseRole.Agent, Subjects[i]);
                        }
                    }
                    else if (VF.Passive)
                    {
                        for (int i = 0; i < Subjects.Count; i++)
                        {
                            VF.AddCaseRole(CaseRole.Theme, Subjects[i]);
                        }
                    }

                    //for (int i = 0; i < Subjects.Count; i++)
                    //{
                    //    //if(
                    //    VF.AddCaseRole(CaseRole.Agent,Subjects[i]);
                    //}

                    VFL.Add(VF);
                }



                else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRDS2"))
                {
                    //take subjects list and when PRD is reached........add caseroles.

                    VFL.AddRange(GET_PRDS2((ParseNode)node.Children[0],parseTree,Subjects));
                }

            }

            return VFL;
        }

        public List<VerbFrame> GET_PRDS1(ParseNode node, ParseTree parseTree, List<NounFrame> Subjects)
        {
            //PRDS1=PRD+CMA+PRDS1
            //PRDS1=PRDS2+CMA+PRDS1
            //PRDS1=PRD+COR+PRD
            //PRDS1=PRD+COR+PRDS2
            //PRDS1=PRDS2+COR+PRDS2
            VerbFrame vf = new VerbFrame(node);
            List<VerbFrame> vfl = new List<VerbFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRD", "CMA", "PRDS1"))
            {
                vf = GET_PRD((ParseNode)node.Children[0], parseTree, Subjects);
                vfl.Add(vf);
                vfl.AddRange(GET_PRDS1((ParseNode)node.Children[2], parseTree,Subjects));

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRDS2", "CMA", "PRDS1"))
            {
                vfl.AddRange(GET_PRDS2((ParseNode)node.Children[0], parseTree, Subjects));
                vfl.AddRange(GET_PRDS1((ParseNode)node.Children[2], parseTree, Subjects));

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRD", "COR", "PRD"))
            {
                //cor--> for, to, in order to, but, and, nor, or, yet, so
                vf = GET_PRD((ParseNode)node.Children[0], parseTree,Subjects);
                vfl.Add(vf);
                if (vf.VerbName == "IS" || vf.VerbName == "WAS" || vf.VerbName == "ARE" || vf.VerbName == "WERE")
                {
                    for (int i = 0; i < Subjects.Count; i++)
                    {

                        vf.AddCaseRole(CaseRole.Theme, Subjects[i]);

                    }
                }
                else if (!vf.Passive)
                {
                    for (int i = 0; i < Subjects.Count; i++)
                    {
                        vf.AddCaseRole(CaseRole.Agent, Subjects[i]);
                    }
                }
                else if (vf.Passive)
                {
                    for (int i = 0; i < Subjects.Count; i++)
                    {
                        vf.AddCaseRole(CaseRole.Theme, Subjects[i]);
                    }
                }
                vf = GET_PRD((ParseNode)node.Children[2], parseTree,Subjects);
                if (vf.VerbName == "IS" || vf.VerbName == "WAS" || vf.VerbName == "ARE" || vf.VerbName == "WERE")
                {
                    for (int i = 0; i < Subjects.Count; i++)
                    {

                        vf.AddCaseRole(CaseRole.Theme, Subjects[i]);

                    }
                }
                else if (!vf.Passive)
                {
                    for (int i = 0; i < Subjects.Count; i++)
                    {
                        vf.AddCaseRole(CaseRole.Agent, Subjects[i]);
                    }
                }
                else if (vf.Passive)
                {
                    for (int i = 0; i < Subjects.Count; i++)
                    {
                       vf.AddCaseRole(CaseRole.Theme, Subjects[i]);
                    }
                }
                vfl.Add(vf);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRD", "COR", "PRDS2"))
            {
                //cor--> for, to, in order to, but, and, nor, or, yet, so
                vf = GET_PRD((ParseNode)node.Children[0], parseTree,Subjects);
                vfl.Add(vf);
                vfl.AddRange(GET_PRDS2((ParseNode)node.Children[2], parseTree,Subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRDS2", "COR", "PRDS2"))
            {
                //cor--> for, to, in order to, but, and, nor, or, yet, so
                vfl.AddRange(GET_PRDS2((ParseNode)node.Children[0], parseTree,Subjects));
                vfl.AddRange(GET_PRDS2((ParseNode)node.Children[2], parseTree,Subjects));
            }


            return vfl;
        }
        public List<VerbFrame> GET_PRDS2(ParseNode node, ParseTree parseTree,List<NounFrame>Subjects)
        {
            //will it make a difference what separates between the predicates????????

            //PRDS2=ETH+PRDS3+OR+PRDS3
            //PRDS2=NTH+PRDS3+NOR+PRDS3
            //PRDS2=NOTN+PRDS3+BUTL+PRDS3
            //PRDS2=BTH+PRDS3+AND+PRDS3
            //PRDS2=WTH+PRDS3+OR+PRDS3
            VerbFrame vf = new VerbFrame(node);
            List<VerbFrame> vfl = new List<VerbFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "PRDS3", "OR", "PRDS3"))
            {
                //vf = GET_PRD((ParseNode)node.Children[0], parseTree);
                //vfl.Add(vf);
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[1], parseTree,Subjects));
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[3], parseTree,Subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "PRDS3", "NOR", "PRDS3"))
            {
                //vf = GET_PRD((ParseNode)node.Children[0], parseTree);
                //vfl.Add(vf);
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[1], parseTree,Subjects));
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[3], parseTree,Subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "PRDS3", "BUTL", "PRDS3"))
            {
                //vf = GET_PRD((ParseNode)node.Children[0], parseTree);
                //vfl.Add(vf);
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[1], parseTree,Subjects));
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[3], parseTree,Subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "PRDS3", "AND", "PRDS3"))
            {
                //vf = GET_PRD((ParseNode)node.Children[0], parseTree);
                //vfl.Add(vf);
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[1], parseTree,Subjects));
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[3], parseTree,Subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH", "PRDS3", "OR", "PRDS3"))
            {
                //vf = GET_PRD((ParseNode)node.Children[0], parseTree);
                //vfl.Add(vf);
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[1], parseTree,Subjects));
                vfl.AddRange(GET_PRDS3((ParseNode)node.Children[3], parseTree,Subjects));
            }

            return vfl;
        }
        public List<VerbFrame> GET_PRDS3(ParseNode node, ParseTree parseTree,List<NounFrame>Subjects)
        {
            //PRDS3=PRD
            //PRDS3=PRDS4

            VerbFrame vf = new VerbFrame(node);
            List<VerbFrame> vfl = new List<VerbFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRD"))
            {
                vf = GET_PRD((ParseNode)node.Children[0], parseTree,Subjects);
                vfl.Add(vf);
                //vfl.AddRange(GET_PRDS1((ParseNode)node.Children[0], parseTree));

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRDS4"))
            {
                vfl.AddRange(GET_PRDS4((ParseNode)node.Children[0], parseTree,Subjects));

            }


            return vfl;
        }
        public List<VerbFrame> GET_PRDS4(ParseNode node, ParseTree parseTree,List<NounFrame>Subjects)
        {
            //PRDS4=PRD+CMA+PRDS4
            //PRDS4=PRD+COR+PRD


            VerbFrame vf = new VerbFrame(node);
            List<VerbFrame> vfl = new List<VerbFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRD", "CMA", "PRDS4"))
            {
                vf = GET_PRD((ParseNode)node.Children[0], parseTree,Subjects);
                vfl.Add(vf);
                vfl.AddRange(GET_PRDS4((ParseNode)node.Children[2], parseTree,Subjects));

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRD", "COR", "PRD"))
            {
                vf = GET_PRD((ParseNode)node.Children[0], parseTree,Subjects);
                vfl.Add(vf);
                vf = GET_PRD((ParseNode)node.Children[2], parseTree,Subjects);
                vfl.Add(vf);

            }

            return vfl;
        }

        //CMP->infph
        //private List<NounFrame> GET_INFPH(ParseNode node, ParseTree parseTree)
        //{
        //    //INFPH=TO+VINF
        //    //INFPH=TO+ADVS+VINF
        //    //INFPH=TO+VINF+CMPS
        //    //INFPH=TO+ADVS+VINF+CMPS
        //    //return list of nounframes(CMPS) or verbs(VINF)?

        //    List<NounFrame> nflist = new List<NounFrame>();

        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "TO", "VINF"))
        //    {
        //        //nflist.AddRange(GET_OBJ((ParseNode)node.Children[0], parseTree, verbFrame));
        //        //wut to do?
        //    }
        //    else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "TO", "ADVS", "VINF"))
        //    {
        //        //add adverbs to

        //        //nflist.AddRange(GET_INFPO((ParseNode)node.Children[0], parseTree));
        //    }
        //    else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "TO", "VINF", "CMPS"))
        //    {
        //        //??????????????
        //        //nflist.AddRange(GET_CMPAVJ((ParseNode)node.Children[0], parseTree));
        //    }
        //    else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "TO", "ADVS", "VINF", "CMPS"))
        //    {
        //        //??????????????????????????

        //    }
        //    return 
        //}

        //convert adverbs into lists of wordinfo
        public List<MyWordInfo> GET_ADV(ParseNode node, ParseTree parseTree)
        {
            //ADV=PADV TEEMA
            //ADV=INFPH
            //ADV=PRP
            //ADV=CMADV
            //ADV=ADVC

            List<string> stlist = new List<string>();
            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> adv = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PADV"))
            {
                //convert to list of <wordinfo>
                //stlist.AddRange(GET_PADV_adj((ParseNode)node.Children[0], parseTree));
                adv.AddRange(GET_PADV_adj((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "INFPH"))
            {
                //stlist.AddRange(GET_CMADV_st((ParseNode)node.Children[0], parseTree));
                //nflist.AddRange(GET_CMADV_nf((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP"))
            {
                //stlist.AddRange(GET_CMADV_st((ParseNode)node.Children[0], parseTree));
                //nflist.AddRange(GET_CMADV_nf((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree, null));

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMADV"))
            {
                //convert camadv into list of wordinfo
                //stlist.AddRange(GET_CMADV_st((ParseNode)node.Children[0], parseTree));
                //nflist.AddRange(GET_CMADV_nf((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVC"))
            {
                //return eah?
                //ADVC=SOR+DS
            }

            return adv;
            //return list of wordinfo

        }



        #endregion

        //public List<VerbFrame> GET_PRDS(ParseNode node,ParseTree parseTree, List<NounFrame> Subjects)
        //{
        //    //transitive and passive verbs were checked for in PRDS in old code....the same here???

        //    VerbFrame VF = new VerbFrame(node);
        //    List<VerbFrame> VFL=new List<VerbFrame>();
        //    if (node.Children != null)
        //    {
        //        //int i = 0;


        //        if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRDS1"))
        //        {

        //            //VF = GET_PRDS1((ParseNode)node.Children[0],parseTree);

        //        }

        //        else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"PRD"))
        //        {
        //            //checking this now
        //            VF = GET_PRD((ParseNode)node.Children[0],parseTree);
        //            //I am ahmad....caserole?
        //            //they are playing...they agents
        //            //ali is a student....ali theme w student co-theme
        //            //ali is good...good adjective
        //            if (VF.VerbName == "IS" || VF.VerbName == "WAS" || VF.VerbName == "ARE" || VF.VerbName == "WERE")
        //            {
        //                for (int i = 0; i < Subjects.Count; i++)
        //                {
        //                    VF.AddCaseRole(CaseRole.Theme, Subjects[i]);
        //                }
        //            }
        //            else if (!VF.Passive)
        //            {
        //                for (int i = 0; i < Subjects.Count; i++)
        //                {
        //                    VF.AddCaseRole(CaseRole.Agent, Subjects[i]);
        //                }
        //            }
        //            else if (VF.Passive)
        //            {
        //                for (int i = 0; i < Subjects.Count; i++)
        //                {
        //                    VF.AddCaseRole(CaseRole.Theme, Subjects[i]);
        //                }
        //            }
        //            //for (int i = 0; i < Subjects.Count; i++)
        //            //{
        //            //    //if(
        //            //    VF.AddCaseRole(CaseRole.Agent,Subjects[i]);
        //            //}

        //           VFL.Add(VF);
        //        }

                

        //        else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRDS2"))
        //        {
        //            //take subjects list and when PRD is reached........add caseroles.
                    
        //            //VF = GET_PRDS2((ParseNode)node.Children[0]);
                    
                    

        //        }


        //        //vfs.Add(vfv);
                
        //    }

        //    return VFL;
        //}

        public VerbFrame GET_PRD(ParseNode node,ParseTree parseTree,List<NounFrame>Subjects)
        {

            //PRD=V
            //PRD=V+CMPS
            //PRD=PADV+V
            //PRD=PADV+V+CMPS
            //PRD=VP+CMPS

            //check for passive and active verbs???

            //in the old code they did that....and they checked for the verbs....a3mel eah?
           // bool passive = CheckPassiveVerb(node);
           // bool transitive = CheckTransVerb(node, parseTree);

            //that's it?
            //VerbFrame vf = new VerbFrame(node);
            //vf.Passive = passive;
            //vf.Transitive = transitive;

           
            #region OLDPRD
            //VerbFrame _Verb;
            //List<ParseNode> CurrentAdverbs = new List<ParseNode>();
            //isflag = false;
            //if (node.Children != null)
            //{
            //    //ParseNode pn =(ParseNode) node.Children[1];
            //    bool trans = CheckTransVerb(node, parsetree);
            //    bool passive = CheckPassiveVerb(node);
            //    if (passive == true)// sara add it
            //    {
            //        trans = true;
            //    }
            //    int i = 0;
            //    string verbname;

            //    foreach (ParseNode n in node.Children)
            //    {
            //        //if (pncmplst.Goal!=null)//repeat adding caseroles
            //        //{
            //        //   n.Goal = pncmplst.Goal;
            //        //}
            //        if (n.Goal == "V")
            //        {
            //            if (Sor == false && Cor == false)
            //            {
            //                if (passive == true)
            //                {
            //                    string s = SentenceParser.GetWordString(parsetree, n);
            //                    if (s == "IS" || s == "ARE" || s == "WAS" || s == "WERE")
            //                    {
            //                        ParseNode cmpn = (ParseNode)node.Children[1];
            //                        ParseNode adjn = (ParseNode)cmpn.Children[0];
            //                        ParseNode VPSPn = (ParseNode)adjn.Children[0];
            //                        if (VPSPn.Goal == "VPSP")
            //                        {

            //                            currentVerbFrame = new VerbFrame(VPSPn);
            //                            verbname = SentenceParser.GetWordString(parsetree, VPSPn);
            //                            currentVerbFrame.VerbName = verbname;
            //                            AddVerbrame(currentVerbFrame);
            //                        }
            //                    }
            //                    else if (s == "WILL")
            //                    {
            //                        ParseNode cmpn = (ParseNode)node.Children[1];
            //                        ParseNode SOBJn = (ParseNode)cmpn.Children[0];
            //                        if (SOBJn.Goal == "SOBJ")
            //                        {
            //                            ParseNode NPn = (ParseNode)SOBJn.Children[0];
            //                            ParseNode adjN = (ParseNode)NPn.Children[1];
            //                            ParseNode par_ph = (ParseNode)adjN.Children[0];
            //                            ParseNode VPSPN = (ParseNode)par_ph.Children[0];
            //                            if (VPSPN.Goal == "VPSP")
            //                            {
            //                                currentVerbFrame = new VerbFrame(VPSPN);
            //                                verbname = SentenceParser.GetWordString(parsetree, VPSPN);
            //                                currentVerbFrame.VerbName = verbname;
            //                                AddVerbrame(currentVerbFrame);
            //                            }
            //                        }
            //                        else if (SOBJn.Goal == "INF_PH")
            //                        {
            //                            ParseNode cmpnn = (ParseNode)SOBJn.Children[1];
            //                            ParseNode adjn = (ParseNode)cmpnn.Children[0];
            //                            ParseNode VPSPNN = (ParseNode)adjn.Children[0];
            //                            if (VPSPNN.Goal == "VPSP")
            //                            {
            //                                currentVerbFrame = new VerbFrame(VPSPNN);
            //                                verbname = SentenceParser.GetWordString(parsetree, VPSPNN);
            //                                currentVerbFrame.VerbName = verbname;
            //                                AddVerbrame(currentVerbFrame);
            //                            }


            //                        }


            //                    }

            //                }
            #region ActiveVerb

            //                else //active
            //                {
            //                    string nwill = SentenceParser.GetWordString(parsetree, n);

            //                    if (nwill == "WILL")
            //                    {
            //                        ParseNode CMPn = (ParseNode)node.Children[1];
            //                        ParseNode VINFpn = (ParseNode)CMPn.Children[0];
            //                        ParseNode VINFn = (ParseNode)VINFpn.Children[0];
            //                        if (VINFn.Goal == "VINF")
            //                        {
            //                            currentVerbFrame = new VerbFrame(VINFn);
            //                            verbname = SentenceParser.GetWordString(parsetree, VINFn);
            //                            currentVerbFrame.VerbName = verbname;
            //                            AddVerbrame(currentVerbFrame);
            //                        }
            //                        else if (VINFn.Goal == "VING")
            //                        {
            //                            currentVerbFrame = new VerbFrame(VINFn);
            //                            verbname = SentenceParser.GetWordString(parsetree, VINFn);
            //                            currentVerbFrame.VerbName = verbname;
            //                            AddVerbrame(currentVerbFrame);
            //                        }



            //                    }
            //                    else
            //                    {
            //                        currentVerbFrame = new VerbFrame(n);
            //                        verbname = SentenceParser.GetWordString(parsetree, n);
            //                        currentVerbFrame.VerbName = verbname;
            //                        AddVerbrame(currentVerbFrame);

            //                    }

            //                }
            #endregion
            //            }
            //            else
            //            {

            //                FillVerbAccordingToProposition(n, parsetree, passive);

            //            }
            //        }
            //        if (n.Goal == "VP")
            //        {
            //            if (passive == true)
            //            {
            //                if (n.Children.Count <= 2)
            //                {
            //                    ParseNode pnode = (ParseNode)node.Children[1];//CMP
            //                    ParseNode Adjn = (ParseNode)pnode.Children[0];
            //                    ParseNode Vpspn = (ParseNode)Adjn.Children[0];
            //                    if (Vpspn.Goal == "VPSP")
            //                    {
            //                        if (Sor == false && Cor == false)
            //                        {
            //                            currentVerbFrame = new VerbFrame(Vpspn);
            //                            verbname = SentenceParser.GetWordString(parsetree, Vpspn);
            //                            currentVerbFrame.VerbName = verbname;
            //                            AddVerbrame(currentVerbFrame);
            //                        }
            //                        else
            //                        {
            //                            FillRelations(parsetree, Vpspn);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        if (Sor == false && Cor == false)
            //                        {
            //                            ParseNode vn = (ParseNode)n.Children[1];
            //                            currentVerbFrame = new VerbFrame(vn);
            //                            verbname = SentenceParser.GetWordString(parsetree, vn);
            //                            currentVerbFrame.VerbName = verbname;
            //                            AddVerbrame(currentVerbFrame);
            //                        }
            //                        else
            //                        {
            //                            if (Vpspn.Goal == "VPSP")
            //                            {
            //                                FillRelations(parsetree, Vpspn);
            //                            }
            //                            else
            //                            {
            //                                ParseNode vn = (ParseNode)n.Children[1];
            //                                FillRelations(parsetree, vn);
            //                            }

            //                        }

            //                    }

            //                }
            //                else
            //                {
            //                    ParseNode vn = (ParseNode)n.Children[2];
            //                    currentVerbFrame = new VerbFrame(vn);
            //                    verbname = SentenceParser.GetWordString(parsetree, vn);
            //                    currentVerbFrame.VerbName = verbname;
            //                    AddVerbrame(currentVerbFrame);
            //                }



            //            }
            #region ActiveVerb
            //            else//active
            //            {
            //                ParseNode pnode = (ParseNode)n.Children[1];
            //                if (pnode.Goal == "VING")//is doing
            //                {
            //                    currentVerbFrame = new VerbFrame(pnode);
            //                    verbname = SentenceParser.GetWordString(parsetree, pnode);
            //                    currentVerbFrame.VerbName = verbname;
            //                    AddVerbrame(currentVerbFrame);

            //                }
            //                else if (pnode.Goal == "VPSP")//has eaten
            //                {
            //                    if (Sor == true)
            //                    {
            //                        FillRelations(parsetree, pnode);

            //                    }
            //                    else
            //                    {
            //                        currentVerbFrame = new VerbFrame(pnode);
            //                        verbname = SentenceParser.GetWordString(parsetree, pnode);
            //                        currentVerbFrame.VerbName = verbname;
            //                        AddVerbrame(currentVerbFrame);
            //                    }

            //                }
            //                else if (pnode.Goal == "VINF")//3yzen ne3rf zy 2lle fo2eha walla l2 :D:D:D
            //                {
            //                    currentVerbFrame = new VerbFrame(pnode);
            //                    verbname = SentenceParser.GetWordString(parsetree, pnode);
            //                    currentVerbFrame.VerbName = verbname;
            //                    AddVerbrame(currentVerbFrame);
            //                }





            //            }
            #endregion


            //        }

            //        if (n.Goal == "CMP_LST")
            //        {
            //            //if (pncmplst.Goal != null)
            //            //{
            //            //    n.Goal = pncmplst.Goal;
            //            //    n.Children = pncmplst.Children;
            //            //}
            //            foreach (ParseNode pe in n.Children)
            //            {


            //                if (pe.Goal == "CMP")
            //                {
            //                    ParseNode N_obj = (ParseNode)pe.Children[0];
            //                    if (N_obj.Goal == "SOBJ" || N_obj.Goal == "OBJ")
            //                    {
            //                        currentObjects = GetObjects(N_obj, parsetree, passive);
            //                    }
            //                    if (N_obj.Goal == "ADV_LST")
            //                    {
            //                        foreach (ParseNode p1 in N_obj.Children)
            //                        {
            //                            if (p1.Goal == "ADV")
            //                            {
            //                                string s = SentenceParser.GetWordString(parsetree, p1);
            //                                p1.Text = s;

            //                                CurrentAdverbs.Add(p1);
            //                            }
            //                        }
            //                    }
            //                    else if (N_obj.Goal == "ADV")
            //                    {
            //                        string s = SentenceParser.GetWordString(parsetree, N_obj);
            //                        N_obj.Text = s;

            //                        CurrentAdverbs.Add(N_obj);
            //                    }

            //                    currentVerbFrame.Adverb = CurrentAdverbs;
            //                    if (N_obj.Goal == "PREP_PH")
            //                    {
            //                        FillVerbAccordingToProposition(N_obj, parsetree, passive);
            //                    }
            //                    if (N_obj.Goal == "INF_PH")
            //                    {
            //                        FillVerbAccordingToProposition(N_obj, parsetree, passive);
            //                    }



            //                }
            //            }
            //        }


            //        if (n.Goal == "CMP")
            //        {
            //            ParseNode N_obj = (ParseNode)n.Children[0];
            //            if (N_obj.Goal == "SOBJ" || N_obj.Goal == "OBJ")
            //            {
            //                currentObjects = GetObjects(N_obj, parsetree, passive);
            //            }
            //            if (N_obj.Goal == "ADV_LST")
            //            {
            //                foreach (ParseNode p1 in N_obj.Children)
            //                {
            //                    if (p1.Goal == "ADV")
            //                    {
            //                        string s = SentenceParser.GetWordString(parsetree, p1);
            //                        p1.Text = s;

            //                        CurrentAdverbs.Add(p1);
            //                    }
            //                }
            //            }
            //            else if (N_obj.Goal == "ADV")
            //            {
            //                string s = SentenceParser.GetWordString(parsetree, N_obj);
            //                N_obj.Text = s;

            //                CurrentAdverbs.Add(N_obj);
            //            }

            //            currentVerbFrame.Adverb = CurrentAdverbs;

            //            if (N_obj.Goal == "PREP_PH")
            //            {
            //                FillVerbAccordingToProposition(N_obj, parsetree, passive);
            //            }
            //            if (N_obj.Goal == "INF_PH")
            //            {
            //                FillVerbAccordingToProposition(N_obj, parsetree, passive);
            //            }

            #region CommentedOld
            //            //if (N_obj.Goal == "INF_PH")
            //            //{
            //            //    pncmplst =(ParseNode) N_obj.Children[1];
            //            //    n.Goal = pncmplst.Goal;
            //            //    goto case "PRD";
            //            //   // BuildNodeTMR(parsetree, pncmplst, node.Start + i);
            //            //   // i++;

            //            //}


            //            //if (passive == false)
            //            //   {
            //            //       if (trans == true)
            //            //       {
            //            //           foreach (NounFrame nff in currentsubjects)
            //            //           {

            //            //               currentVerbFrame.AddCaseRole(CaseRole.Agent, nff);

            //            //           }

            //            //       }
            //            //       else
            //            //       {
            //            //           if (currentVerbFrame.VerbName == "is" | currentVerbFrame.VerbName == "are" | currentVerbFrame.VerbName == "am" | currentVerbFrame.VerbName == "was" | currentVerbFrame.VerbName == "were")
            //            //           {
            //            //               foreach (NounFrame nounf in currentsubjects)
            //            //               {
            //            //                   ParseNode nf = nounf.ParseNode;
            //            //                   if (nf.Children != null)
            //            //                   {
            //            //                       ParseNode eNP = (ParseNode)nf.Children[0];
            //            //                       ParseNode eP = (ParseNode)eNP.Children[0];
            //            //                       ParseNode ePRO_NOUN = (ParseNode)eP.Children[0];

            //            //                       currentVerbFrame.AddCaseRole(CaseRole.Theme, nounf);

            //            //                   }
            //            //               }
            //            //           }

            //            //       }

            //            //   }
            #endregion
            //        }


            //        // ParseNode parsenode1 = (ParseNode)node.Children[0];
            //        BuildNodeTMR(parsetree, n, node.Start + i);
            //        i++;
            //    }
            //    if (passive == false)
            //    {
            //        if (trans == true)
            //        {
            //            if (currentVerbFrame.VerbName == "IS" || currentVerbFrame.VerbName == "ARE" || currentVerbFrame.VerbName == "WAS" || currentVerbFrame.VerbName == "WERE")
            //            {
            //                if (isflag == true)
            //                {
            //                    foreach (NounFrame nff in currentObjects)
            //                    {

            //                        currentVerbFrame.AddCaseRole(CaseRole.Cotheme, nff);
            //                        ParseNode pppn = nff.ParseNode;
            //                        string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
            //                        ss = nff.Text;



            //                    }
            //                }

            //            }
            //            foreach (NounFrame nff in currentsubjects)
            //            {
            //                if (isflag == false)
            //                {
            //                    currentVerbFrame.AddCaseRole(CaseRole.Agent, nff);
            //                    ParseNode pppn = nff.ParseNode;
            //                    string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
            //                    ss = nff.Text;
            //                }
            //                else
            //                {
            //                    currentVerbFrame.AddCaseRole(CaseRole.Theme, nff);
            //                    ParseNode pppn = nff.ParseNode;
            //                    string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
            //                    ss = nff.Text;
            //                }


            //            }
            //            foreach (NounFrame nff in currentObjects)
            //            {
            //                if (isflag == false)
            //                {

            //                    currentVerbFrame.AddCaseRole(CaseRole.Theme, nff);
            //                    ParseNode pppn = nff.ParseNode;
            //                    string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
            //                    ss = nff.Text;
            //                }



            //            }
            //            //if (currentVerbFrame.VerbName == "IS" | currentVerbFrame.VerbName == "ARE" | currentVerbFrame.VerbName == "AM" | currentVerbFrame.VerbName == "WAS" | currentVerbFrame.VerbName == "WERE")
            //            //{
            //            //    foreach (NounFrame nounf in currentsubjects)
            //            //    {
            //            //        ParseNode nf = nounf.ParseNode;
            //            //        if (nf.Children != null)
            //            //        {
            //            //            ParseNode eNP = (ParseNode)nf.Children[0];
            //            //            ParseNode eP = (ParseNode)eNP.Children[0];
            //            //            ParseNode ePRO_NOUN = (ParseNode)eP.Children[0];

            //            //            currentVerbFrame.AddCaseRole(CaseRole.Theme, nounf);
            //            //            ParseNode pppn = nounf.ParseNode;
            //            //            string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
            //            //            ss = nounf.Text;




            //            //        }
            //            //    }
            //            //}



            //        }
            //        else
            //        {
            //            foreach (NounFrame nfe in currentsubjects)
            //            {
            //                currentVerbFrame.AddCaseRole(CaseRole.Theme, nfe);

            //                ParseNode pppn = nfe.ParseNode;
            //                string ss = SentenceParser.GetWordString(nfe.Parsetree, pppn);//nff.text=ali after it be null
            //                ss = nfe.Text;


            //            }

            //        }

            //    }
            //    else //passive & trans
            //    {
            //        foreach (NounFrame e in currentsubjects)
            //        {
            //            currentVerbFrame.AddCaseRole(CaseRole.Theme, e);

            //            ParseNode pppn = e.ParseNode;
            //            string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
            //            ss = e.Text;
            //        }


            //    }
            //}
            //break;
            #endregion

            //CheckRule(node, "P1", "P2", "P3"));

            List<MyWordInfo> Desc = new List<MyWordInfo>();
         
            //int i = 0;
            List<NounFrame> nflist = new List<NounFrame>();

            //foreach (ParseNode n in node.Children)
            //{
            VerbFrame vf = null;
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "V"))
            {
                vf = GET_V((ParseNode)node.Children[0], parseTree);
                vf.Transitive = false;
              
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "V", "CMPS"))
            {
                vf = GET_V((ParseNode)node.Children[0], parseTree);
              
                nflist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree, vf, Subjects));
               
                


                //hal dah sa7? it depends on whether or not the verb is passive/active/transitive
                
                //vf.AddCaseRole(CaseRole.Theme, nflist[0]);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PADV", "V"))
            {
                ///PADV????
                Desc.AddRange(GET_PADV_adj((ParseNode)node.Children[0], parseTree));
                vf = GET_V((ParseNode)node.Children[1], parseTree);
                vf.Transitive = false;
                vf.AdverbsInfo.AddRange(Desc);


            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PADV", "V", "CMPS"))
            {
                ///PADV????
                Desc.AddRange(GET_PADV_adj((ParseNode)node.Children[0], parseTree));
                vf = GET_V((ParseNode)node.Children[1], parseTree);
                vf.AdverbsInfo.AddRange(Desc);
                nflist.AddRange(GET_CMPS((ParseNode)node.Children[2], parseTree, vf,Subjects));

            } 
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VP", "CMPS"))
            {
                //what to do????????????
                vf = (GET_VP((ParseNode)node.Children[0], parseTree));
                //CMPS ba3den
                nflist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree, vf, Subjects));
            } 


            //check


            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING", "CMPS"))
            {
                
                vf = (GET_VING_Verb((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree, vf, Subjects));
            }  

                //i++;
            //}
            return vf;
        }

        //  public List<Frame> GET_CMPSTest(ParseNode node, ParseTree parseTree, VerbFrame verbFrame,List<NounFrame>Subjects)
        //{
        //    //List<NounFrame> nflist = new List<NounFrame>();
        //    List<Frame> nflist = new List<Frame>();
        //    List<MyWordInfo> Adjectives = new List<MyWordInfo>();
        //    //int i = 0;
        //    //foreach (ParseNode n in node.Children)
        //    //{
        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMP"))
        //    {
        //        nflist.AddRange(GET_CMP((ParseNode)node.Children[0], parseTree, verbFrame,Subjects));
               
        //    }
        //    else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMP", "CMPS"))
        //    {
        //        nflist.AddRange(GET_CMP((ParseNode)node.Children[0], parseTree, verbFrame,Subjects));
                
        //        nflist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree, verbFrame,Subjects));
        //    }


        //    //check for verb type w keda

        //    ////if (nflist.Count == 0)
        //    //{
        //    //    verbFrame.Transitive = true; //?????????
        //    //    //transitive verb
        //    //}
        //    //else
        //    //{
        //    //    verbFrame.Transitive = false;

        //    //}
        //        //i++;

               
        //    //}
        //    return  nflist;

        //}

        bool flag = false;
        List<NounFrame> Temp4PrpNouns = new List<NounFrame>();
        public List<NounFrame> GET_CMPS(ParseNode node, ParseTree parseTree, VerbFrame verbFrame,List<NounFrame>Subjects)
        {
            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();

            List<VerbFrame> vflist = new List<VerbFrame>();
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMP"))
            {
                nflist.AddRange(GET_CMP((ParseNode)node.Children[0], parseTree, verbFrame, Subjects));

                if (nflist.Count > 0)
                    verbFrame.Transitive = true;
               
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMP", "CMPS"))
            {
                nflist.AddRange(GET_CMP((ParseNode)node.Children[0], parseTree, verbFrame, Subjects));

                Temp4PrpNouns.AddRange(nflist);

                flag = true;
                nflist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree, verbFrame, Subjects));
                if (nflist.Count > 0)
                    verbFrame.Transitive = true;
                //nflist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree, verbFrame, nflist));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRDS"))
            {
                vflist.AddRange(GET_PRDS((ParseNode)node.Children[0], parseTree, Subjects));

                
            }


            //check for verb type w keda

            ////if (nflist.Count == 0)
            //{
            //    verbFrame.Transitive = true; //?????????
            //    //transitive verb
            //}
            //else
            //{
            //    verbFrame.Transitive = false;

            //}
                //i++;

               
            //}
            return nflist;

        }
        bool PADV = false;
        bool CMP_OBJ = false;
        List<NounFrame> CurrentNFS = new List<NounFrame>();
        //ali and barry have similar traits....mayodkholsh fel CMPAVJ khales..aroo7 lel rule eltany avj+sbj.
        public List<NounFrame> GET_CMP(ParseNode node, ParseTree parseTree, VerbFrame verbFrame,List<NounFrame>Subjects)
        {
            //CMP=OBJ
            //CMP=INFPO
            //CMP=CMPAVJ
            //CMP=PRS
            //CMP=LPRPS
            //CMP=INFPH
            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> description = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OBJ"))
            {
                nflist.AddRange(GET_OBJ((ParseNode)node.Children[0], parseTree, verbFrame));
                //law el CMP gat hena yeb2a el flag = true w law dakhalt 3al prepositions wel flag = true yeb2a hakhod el subject DI(ana 3amaltaha global) w add leeha el prep
                if (CMP_OBJ == true)
                {
                    CMP_OBJ = false;
                    CurrentNFS.Clear();
                    CurrentNFS.AddRange(nflist);
                    //CMP_OBJ = true;
                }
                else
                {
                    CMP_OBJ = true;
                    CurrentNFS.AddRange(nflist);
                }
                
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "INFPO"))
            {
                nflist.AddRange(GET_INFPO((ParseNode)node.Children[0], parseTree, verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAVJ"))
            {
                CMP_OBJ = false;
                CurrentNFS.Clear();
                description.AddRange(GET_CMPAVJ((ParseNode)node.Children[0], parseTree,nflist));//remove nflist
                //have similar symptoms.....add to noun
                if (verbFrame.VerbName == "IS" || verbFrame.VerbName == "WAS" || verbFrame.VerbName == "ARE" || verbFrame.VerbName == "WERE")// || verbFrame.VerbName == "HAVE" || verbFrame.VerbName == "HAD" || verbFrame.VerbName == "HAS")
                {
                    for (int i = 0; i < Subjects.Count; i++)
                    {
                        Subjects[i].AdjectivesInfo = description;
                        //for (int j = 0; j < nfl.Count; j++)
                        //{
                        //    nflist[i].TestAdj[j] = nfl[j];
                        //}
                        VerbFrames.Remove(verbFrame);
                    }
                }
                //a and b have similar symptoms.........

                //else if (verbFrame.VerbName == "HAVE" || verbFrame.VerbName == "HAD" || verbFrame.VerbName == "HAS")
                //{
                //    for (int i = 0; i < Subjects.Count; i++)
                //    {
                //        Subjects[i].AdjectivesInfo = description;
                //        //for (int j = 0; j < nfl.Count; j++)
                //        //{
                //        //    nflist[i].TestAdj[j] = nfl[j];
                //        //}
                //        //VerbFrames.Remove(verbFrame);
                //    }
                //}

                    //ali runs quickly
                else if(PADV)//any other verb
                {
                    //he runs quickly......
                    //if not is/was............
                    for (int i = 0; i < VerbFrames.Count; i++)
                    {
                        VerbFrames[i].AdverbsInfo = description;
                    }
                }
                   

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRS"))
            {
                CMP_OBJ = false;
                CurrentNFS.Clear();
                //terminal
                NounFrame nf = new NounFrame(node, parseTree);
                nflist.Add(nf);
                //AddNounFrame(nf);

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "LPRPS"))
            {
                //prepositions
                nflist.AddRange(GET_LPRPS((ParseNode)node.Children[0], parseTree, verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "INFPH"))
            {
                //prepositions
                GET_INFPH((ParseNode)node.Children[0], parseTree, verbFrame);
            }
            return nflist;
        }

        private void GET_INFPH(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //INFPH=TO+VINF
            //INFPH=TO+ADVS+VINF
            //INFPH=TO+VINF+CMPS
            //INFPH=TO+ADVS+VINF+CMPS
            VerbFrame vinf = null;
            List<NounFrame> nounFrames = new List<NounFrame>() ;
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "TO", "VINF"))
            {
                vinf = GET_VINF((ParseNode)node.Children[1], parseTree);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "TO", "ADVS", "VINF"))
            {
                List<MyWordInfo> advs = GET_ADVS((ParseNode)node.Children[1], parseTree);
                vinf = GET_VINF((ParseNode)node.Children[2], parseTree);
                vinf.AdverbsInfo = advs;
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "TO", "VINF", "CMPS"))
            {
                vinf = GET_VINF((ParseNode)node.Children[1], parseTree);
                nounFrames = GET_CMPS((ParseNode)node.Children[2], parseTree, vinf, new List<NounFrame>());

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "TO", "ADVS", "VINF", "CMPS"))
            {
                vinf = GET_VINF((ParseNode)node.Children[2], parseTree);
                List<MyWordInfo> advs = GET_ADVS((ParseNode)node.Children[1], parseTree);
                vinf.AdverbsInfo = advs;
                nounFrames = GET_CMPS((ParseNode)node.Children[3], parseTree, vinf, new List<NounFrame>());

            }
            verbFrame.AddDomainRelation(DomainRelationType.Reason, vinf);

        }

        private List<NounFrame> GET_LPRPS(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //LPRPS=PRP
            //LPRPS=LPRPS1
            //LPRPS=LPRPS2

            List<NounFrame> nflist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP"))
            {
                if (CMP_OBJ || CurrentNFS.Count != 0)
                {
                    nflist.AddRange(GET_PRP_nouns((ParseNode)node.Children[0], parseTree, CurrentNFS));
                }
                else
                {
                    nflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree, verbFrame));
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "LPRPS1"))
            {
                nflist.AddRange(GET_LPRPS1((ParseNode)node.Children[0], parseTree,verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "LPRPS2"))
            {
                nflist.AddRange(GET_LPRPS2((ParseNode)node.Children[0], parseTree));
            }

            return nflist;
            //throw new NotImplementedException();
        }
        private List<NounFrame> GET_LPRPS1(ParseNode node, ParseTree parseTree,VerbFrame verbframe)
        {
            //LPRPS1=PRP+CMA+LPRPS1
            //LPRPS1=LPRPS2+CMA+LPRPS1
            //LPRPS1=PRP+COR+PRP
            //LPRPS1=PRP+COR+LPRPS2
            //LPRPS1=LPRPS2+COR+LPRPS2

            List<NounFrame> nflist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP", "CMA", "LPRPS1"))
            {
                //nflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree, verbFrame));
                nflist.AddRange(GET_LPRPS1((ParseNode)node.Children[2], parseTree,verbframe));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "LPRPS2", "CMA", "LPRPS1"))
            {
                nflist.AddRange(GET_LPRPS2((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS1((ParseNode)node.Children[2], parseTree,verbframe));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP", "COR", "PRP"))
            {
                nflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree,verbframe));
                nflist.AddRange(GET_PRP((ParseNode)node.Children[2], parseTree,verbframe));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP", "COR", "LPRPS2"))
            {
                //nflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS2((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "LPRPS2", "COR", "LPRPS2"))
            {
                nflist.AddRange(GET_LPRPS2((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS2((ParseNode)node.Children[2], parseTree));
            }

            return nflist;
            //throw new NotImplementedException();
        }
        private List<NounFrame> GET_LPRPS2(ParseNode node, ParseTree parseTree)
        {
            //LPRPS2=ETH+LPRPS3+OR+LPRPS3
            //LPRPS2=NTH+LPRPS3+NOR+LPRPS3
            //LPRPS2=NOTN+LPRPS3+BUTL+LPRPS3
            //LPRPS2=BTH+LPRPS3+AND+LPRPS3
            //LPRPS2=WTH+LPRPS3+OR+LPRPS3

            List<NounFrame> nflist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "LPRPS3", "OR", "LPRPS3"))
            {
                
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "LPRPS3", "NOR", "LPRPS3"))
            {
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "LPRPS3", "BUTL", "LPRPS3"))
            {
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "LPRPS3", "AND", "LPRPS3"))
            {
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH", "LPRPS3", "OR", "LPRPS3"))
            {
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS3((ParseNode)node.Children[3], parseTree));
            }
            return nflist;

            //throw new NotImplementedException();
        }
        private List<NounFrame> GET_LPRPS3(ParseNode node, ParseTree parseTree)
        {
            //LPRPS3=PRP
            //LPRPS3=LPRPS4

            List<NounFrame> nflist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP"))
            {
                //nflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "LPRPS4"))
            {
                nflist.AddRange(GET_LPRPS4((ParseNode)node.Children[0], parseTree));
            }

            return nflist;

            //throw new NotImplementedException();
        }
        private List<NounFrame> GET_LPRPS4(ParseNode node, ParseTree parseTree)
        {
            //LPRPS4=PRP+CMA+LPRPS4
            //LPRPS4=PRP+COR+PRP

            List<NounFrame> nflist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP", "CMA", "LPRPS4"))
            {
                //nflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_LPRPS4((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP", "COR", "PRP"))
            {
                //nflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree));
                //nflist.AddRange(GET_PRP((ParseNode)node.Children[2], parseTree));
            }
            return nflist;
            //throw new NotImplementedException();
        }
        private List<NounFrame> GET_INFPO(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //    INFPO=VINF
            //    INFPO=ADVS+VINF
            //    INFPO=VINF+CMPS
            //    INFPO=ADVS+VINF+CMPS

            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> AdvList = new List<MyWordInfo>();
            

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VINF"))
            {
                VerbFrame vf = new VerbFrame((ParseNode)node.Children[0]);
                vf = GET_V((ParseNode)node.Children[0], parseTree);
                vf.Passive = false;
                verbFrame.AddDomainRelation(DomainRelationType.Completion, vf);
                
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS", "VINF"))
            {
                AdvList.AddRange(GET_ADVS((ParseNode)node.Children[0], parseTree));
                VerbFrame vf = new VerbFrame((ParseNode)node.Children[0]);
                vf = GET_V((ParseNode)node.Children[0], parseTree);
                verbFrame.AddDomainRelation(DomainRelationType.Completion, vf);
                vf.Passive = false;
                verbFrame.AdverbsInfo = AdvList;
               
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VINF", "CMPS"))
            {

                VerbFrame vf = GET_V((ParseNode)node.Children[0], parseTree);
                vf.Passive = false;
                verbFrame.AddDomainRelation(DomainRelationType.Completion, vf);
                nflist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree, verbFrame, null));
                
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS", "VINF", "CMPS"))
            {
                AdvList.AddRange(GET_ADVS((ParseNode)node.Children[0], parseTree));
               // nflist.AddRange(GET_VINF((ParseNode)node.Children[1], parseTree));
                VerbFrame vf = GET_V((ParseNode)node.Children[0], parseTree);
                vf.Passive = false;
                verbFrame.AddDomainRelation(DomainRelationType.Completion, vf);
                nflist.AddRange(GET_CMPS((ParseNode)node.Children[2], parseTree, verbFrame,null));
                verbFrame.AdverbsInfo = AdvList;
            }


            //return eah hena??
            return nflist;
            //tub wel adverbs?

        }

        private List<MyWordInfo> GET_CMPAVJ(ParseNode node, ParseTree parseTree,List<NounFrame>nflist)
        {
            //CMPAVJ=CMPAJS
            //CMPAVJ=CMPAVS

            //List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJS"))
            {
                Adjectives.AddRange(GET_CMPAJS((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAVS"))
            {
                Adjectives.AddRange(GET_CMPAVS((ParseNode)node.Children[0], parseTree));
            }
            return Adjectives;
        }

         //returns strings or nounframes????????????
        private List<MyWordInfo> GET_CMPAVS(ParseNode node, ParseTree parseTree)
        {
            //CMPAVS=CMPAV
            //CMPAVS=CMPAVS1
            //CMPAVS=CMPAVS2

            List<NounFrame> nflist = new List<NounFrame>();
            List<string> stlist = new List<string>();
            List<MyWordInfo> adv = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAV"))
            {
                adv.AddRange(GET_CMPAV_nf((ParseNode)node.Children[0], parseTree));
                //stlist.AddRange(GET_CMPAV((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAVS1"))
            {
                nflist.AddRange(GET_CMPAVS1((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAVS2"))
            {
                nflist.AddRange(GET_CMPAVS2((ParseNode)node.Children[0], parseTree));
            }
            return adv;

            //throw new NotImplementedException();
        }


        //returns nouframes or strings???
        private List<MyWordInfo> GET_CMPAV_nf(ParseNode node, ParseTree parseTree)
        {
            //CMPAV=PADV
            //CMPAV=CMADV

            List<NounFrame> nflist = new List<NounFrame>();
            List<string> stlist = new List<string>();
            List<MyWordInfo> adv = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PADV"))
            {
                adv.AddRange(GET_PADV_adj((ParseNode)node.Children[0], parseTree));
                //stlist.AddRange(GET_PADV((ParseNode)node.Children[0], parseTree));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMADV"))
            {
                nflist.AddRange(GET_CMADV_nf((ParseNode)node.Children[0], parseTree));
                //stlist.AddRange(GET_PADV((ParseNode)node.Children[0], parseTree));
            }
            return adv;
           // return stlist;


        }
        private List<string> GET_CMPAV_st(ParseNode node, ParseTree parseTree)
        {
            //CMPAV=PADV
            //CMPAV=CMADV

            List<NounFrame> nflist = new List<NounFrame>();
            List<string> stlist = new List<string>();
            List<MyWordInfo> adj = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PADV"))
            {
                adj.AddRange(GET_PADV_adj((ParseNode)node.Children[0], parseTree));
                
                //stlist.AddRange(GET_PADV_st((ParseNode)node.Children[0], parseTree));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPADV"))
            {
                nflist.AddRange(GET_CMADV_nf((ParseNode)node.Children[0], parseTree));
                //stlist.AddRange(GET_PADV_nf((ParseNode)node.Children[0], parseTree));
            }
            //return nflist;
            return stlist;


        }

      

        private List<NounFrame> GET_CMPAVS1(ParseNode node, ParseTree parseTree)
        {
            //CMPAVS1=CMPAV+CMA+CMPAVS1
            //CMPAVS1=CMPAVS2+CMA+CMPAVS1
            //CMPAVS1=CMPAV+COR+CMPAV
            //CMPAVS1=CMPAV+COR+CMPAVS2
            //CMPAVS1=CMPAVS2+COR+CMPAVS2

            throw new NotImplementedException();
        }
        private List<NounFrame> GET_CMPAVS2(ParseNode node, ParseTree parseTree)
        {
            //CMPAVS2=ETH+CMPAVS3+OR+CMPAVS3
            //CMPAVS2=NTH+CMPAVS3+NOR+CMPAVS3
            //CMPAVS2=NOTN+CMPAVS3+BUTL+CMPAVS3
            //CMPAVS2=BTH+CMPAVS3+AND+CMPAVS3
            //CMPAVS2=WTH+CMPAVS3+OR+CMPAVS3
            throw new NotImplementedException();
        }
        private List<NounFrame> GET_CMPAVS3(ParseNode node, ParseTree parseTree)
        {
            //CMPAVS3=CMPAV
            //CMPAVS3=CMPAVS4
            throw new NotImplementedException();
        }
        private List<NounFrame> GET_CMPAVS4(ParseNode node, ParseTree parseTree)
        {
            //CMPAVS4=CMPAV+CMA+CMPAVS4
            //CMPAVS4=CMPAV+COR+CMPAV
            throw new NotImplementedException();
        }

        #endregion

        private List<MyWordInfo> GET_CMPAJS(ParseNode node, ParseTree parseTree)
        {
            //CMPAJS=CMPAJ
            //CMPAJS=CMPAJS1
            //CMPAJS=CMPAJS2

            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJ"))
            {
                Adjectives.AddRange(GET_CMPAJ((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJS1"))
            {
                Adjectives.AddRange(GET_CMPAJS1((ParseNode)node.Children[0], parseTree));
            }
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJS2"))
            {
                Adjectives.AddRange(GET_CMPAJS2((ParseNode)node.Children[0], parseTree));
            }
            return Adjectives;
        }

        #region CMPAJS1-4
        private List<MyWordInfo> GET_CMPAJS1(ParseNode node, ParseTree parseTree)
        {
            //CMPAJS1=CMPAJ+CMA+CMPAJS1
            //CMPAJS1=CMPAJS2+CMA+CMPAJS1
            //CMPAJS1=CMPAJ+COR+CMPAJ
            //CMPAJS1=CMPAJ+COR+CMPAJS2
            //CMPAJS1=CMPAJS2+COR+CMPAJS2

            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> adj = new List<MyWordInfo>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJ", "CMA", "CMPAJS3"))
            {
                adj.AddRange(GET_CMPAJ((ParseNode)node.Children[0], parseTree));
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJS2", "CMA", "CMPAJS1"))
            {
                adj.AddRange(GET_CMPAJS2((ParseNode)node.Children[0], parseTree));
                adj.AddRange(GET_CMPAJS1((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJ", "COR", "CMPAJ"))
            {
                adj.AddRange(GET_CMPAJ((ParseNode)node.Children[0], parseTree));
                adj.AddRange(GET_CMPAJ((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJ", "COR", "CMPAJS2"))
            {
                adj.AddRange(GET_CMPAJ((ParseNode)node.Children[0], parseTree));
                adj.AddRange(GET_CMPAJS2((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJS2", "COR", "CMPAJS2"))
            {
                adj.AddRange(GET_CMPAJS2((ParseNode)node.Children[0], parseTree));
                adj.AddRange(GET_CMPAJS2((ParseNode)node.Children[2], parseTree));
            }
            return adj;
            //throw new NotImplementedException();
        }

        private List<MyWordInfo> GET_CMPAJS2(ParseNode node, ParseTree parseTree)
        {
            //CMPAJS2=ETH+CMPAJS3+OR+CMPAJS3
            //CMPAJS2=NTH+CMPAJS3+NOR+CMPAJS3
            //CMPAJS2=NOTN+CMPAJS3+BUTL+CMPAJS3
            //CMPAJS2=BTH+CMPAJS3+AND+CMPAJS3
            //CMPAJS2=WTH+CMPAJS3+OR+CMPAJS3


            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> adj = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "CMPAJS3", "OR", "CMPAJS3"))
            {
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[1], parseTree));
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "CMPAJS3", "NOR", "CMPAJS3"))
            {
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[1], parseTree));
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "CMPAJS3", "BUTL", "CMPAJS3"))
            {
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[1], parseTree));
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "CMPAJS3", "AND", "CMPAJS3"))
            {
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[1], parseTree));
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH", "CMPAJS3", "OR", "CMPAJS3"))
            {
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[1], parseTree));
                adj.AddRange(GET_CMPAJS3((ParseNode)node.Children[3], parseTree));
            }
           
            return adj;

            //throw new NotImplementedException();
        }

        private List<MyWordInfo> GET_CMPAJS3(ParseNode node, ParseTree parseTree)
        {
            //CMPAJS3=CMPAJ
            //CMPAJS3=CMPAJS4
            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> adj = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJ"))
            {
                adj.AddRange(GET_CMPAJ((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJS4"))
            {
                adj.AddRange(GET_CMPAJS4((ParseNode)node.Children[0], parseTree));
            }
            
            return adj;

            //throw new NotImplementedException();
        }

        private List<MyWordInfo> GET_CMPAJS4(ParseNode node, ParseTree parseTree)
        {
            //CMPAJS4=CMPAJ+CMA+CMPAJS4
            //CMPAJS4=CMPAJ+COR+CMPAJ
            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> adj = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJ", "CMA", "CMPAJS4"))
            {
                adj.AddRange(GET_CMPAJ((ParseNode)node.Children[0], parseTree));
                adj.AddRange(GET_CMPAJS4((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJ", "COR", "CMPAJ"))
            {
                adj.AddRange(GET_CMPAJ((ParseNode)node.Children[0], parseTree));
                adj.AddRange(GET_CMPAJ((ParseNode)node.Children[2], parseTree));
            }
            
            return adj;
            //throw new NotImplementedException();
        }

        #endregion

        private List<MyWordInfo> GET_CMPAJ(ParseNode node, ParseTree parseTree)
        {
            //CMPAJ=CPADJ
            //CMPAJ=CADJ
            //CMPAJ=VPSP
            //CMPAJ=CMADJ

            List<NounFrame> nflist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            MyWordInfo ADJ=new MyWordInfo();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CPADJ"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0]);
                //nflist.Add
               // List<NounFrame> nflist = new List<NounFrame>();
                //NounFrame nf = new NounFrame(node, parseTree);
                //AddNounFrame(nf);
                //nflist.Add(nf);
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                ADJ.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ.Sense = ((ParseNode)node.Children[0]).SenseNo;
                ADJ.Word = nframe.Text;
                Adjectives.Add(ADJ);
                
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CADJ"))
            {
                
            }

            return Adjectives;
        }




        //private List<NounFrame> GET_CMPAVJ(ParseNode node, ParseTree parseTree)
        //{
        //    List<NounFrame> nflist = new List<NounFrame>();

        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJS"))
        //    {
        //        nflist.AddRange(GET_CMPAJS((ParseNode)node.Children[0], parseTree));
        //    }
        //    return nflist;
        //}

        //private List<NounFrame> GET_CMPAJS(ParseNode node, ParseTree parseTree)
        //{
        //    //CMPAJ
        //    List<NounFrame> nflist = new List<NounFrame>();

        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMPAJ"))
        //    {
        //        nflist.AddRange(GET_CMPAJ((ParseNode)node.Children[0], parseTree));
        //    }
        //    return nflist;
        //}

        //private List<NounFrame> GET_CMPAJ(ParseNode node, ParseTree parseTree)
        //{
        //    List<NounFrame> nflist = new List<NounFrame>();
        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CPADJ"))
        //    {
        //        //NounFrame nframe = new NounFrame((ParseNode)node.Children[0]);
        //        //nflist.Add
        //       // List<NounFrame> nflist = new List<NounFrame>();
        //        NounFrame nf = new NounFrame(node, parseTree);
        //        AddNounFrame(nf);
        //        nflist.Add(nf);
                
        //    }
            
        //    return nflist;
        //}


        //public List<NounFrame> GET_OBJ(ParseNode node, ParseTree parseTree)
        //{
        //    //OBJ=SOBJ
        //    //OBJ=OBJ1
        //    //OBJ=OBJ2
            
        //    List<NounFrame> nflist = new List<NounFrame>();
        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ"))
        //    {
        //        nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree));
        //    }
        //    else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OBJ1"))
        //    {
        //        //nflist.AddRange(GET_OBJ1((ParseNode)node.Children[0], parseTree));
        //    }
        //    else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OBJ2"))
        //    {
        //        //nflist.AddRange(GET_OBJ2((ParseNode)node.Children[0], parseTree));
        //    }
        //    return nflist;
        //}

        public List<NounFrame> GET_OBJ(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //OBJ=SOBJ
            //OBJ=OBJ1
            //OBJ=OBJ2
            //if (verbFrame != null)
            //{
            //    verbFrame.Passive = false;
            //    verbFrame.Transitive = true;
            //}

            //Passive/Active?

            List<NounFrame> nflist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ"))
            {
                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree, verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OBJ1"))
            {
                nflist.AddRange(GET_OBJ1((ParseNode)node.Children[0], parseTree, verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OBJ2"))
            {
                nflist.AddRange(GET_OBJ2((ParseNode)node.Children[0], parseTree, verbFrame));
            }

            if (verbFrame != null)
            {   //ali was a student......ali theme student cotheme...
                //ali and wa2el are lawyers...are good lawyers
                //ali is good......good adjective
                if (verbFrame.VerbName == "IS" || verbFrame.VerbName == "WAS" || verbFrame.VerbName == "WERE" || verbFrame.VerbName == "ARE")
                {
                    for (int j = 0; j < nflist.Count; j++)
                    {
                        //if(nflist[j].ContainsA)
                        verbFrame.AddCaseRole(CaseRole.Cotheme, nflist[j]);
                    }

                }
                
                else//check here
                {
                    bool ThereIsRelation = false;

                    foreach (TemporalRelationType RT in verbFrame.TemporalRelations_n.Keys)
                    {
                        List<NounFrame> temp = verbFrame.TemporalRelations_n[RT];
                        for (int j = 0; j < nflist.Count; j++)
                        {
                            if (temp.Contains(nflist[j]))
                            {
                                ThereIsRelation = true;
                            }
                            else ThereIsRelation = false;
                        }
                    }
                    foreach (DomainRelationType RT in verbFrame.DomainRelations_n.Keys)
                    {
                        List<NounFrame> temp = verbFrame.DomainRelations_n[RT];
                        for (int j = 0; j < nflist.Count; j++)
                        {
                            if (temp.Contains(nflist[j]))
                            {
                                ThereIsRelation = true;
                            }
                            else ThereIsRelation = false;
                        }
                    }
                    foreach (CaseRole CR in verbFrame.CaseRoles.Keys)
                    {
                        List<NounFrame> temp = verbFrame.CaseRoles[CR];
                        for (int j = 0; j < nflist.Count; j++)
                        {
                            if (temp.Contains(nflist[j]))
                            {
                                ThereIsRelation = true;
                            }
                            else ThereIsRelation = false;
                        }
                    }

                    if (!ThereIsRelation)
                    {
                        for (int i = 0; i < nflist.Count; i++)
                        {
                            verbFrame.AddCaseRole(CaseRole.Theme, nflist[i]);
                        }
                    }


                }
                //replace verb in verbframeslist
                //int i=0;
               // for (int i = 0; i < VerbFrames.Count; i++)
               // {
               //     if (VerbFrames[i].Sentence_ID == SentenceID)
               //     {
               //         if (VerbFrames[i].VerbName == verbFrame.VerbName)
               //         {
               //             VerbFrames[i] = verbFrame;
               //         }
               //     }
               //}
            

           
              //  i++;
            }


            return nflist;


        }

        #region OBJ1-4

        private List<NounFrame> GET_OBJ1(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //OBJ1=SOBJ+CMA+OBJ1jkhkj
            //OBJ1=OBJ2+CMA+OBJ1kjhkj
            //OBJ1=SOBJ+COR+SOBJsdfsdf
            //OBJ1=SOBJ+COR+OBJ2liih
            //OBJ1=OBJ2+COR+OBJ2

            List<NounFrame> nflist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ", "CMA", "OBJ1"))
            {
                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree,verbFrame));
                nflist.AddRange(GET_OBJ1((ParseNode)node.Children[2], parseTree,verbFrame));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OBJ2", "CMA", "OBJ1"))
            {

                nflist.AddRange(GET_OBJ2((ParseNode)node.Children[0], parseTree,verbFrame));
                nflist.AddRange(GET_OBJ1((ParseNode)node.Children[2], parseTree,verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ", "COR", "SOBJ"))
            {
                //COR=AND
                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree, verbFrame));
                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[2], parseTree,verbFrame));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ", "COR", "OBJ2"))
            {

                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree,verbFrame));
                nflist.AddRange(GET_OBJ2((ParseNode)node.Children[2], parseTree,verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OBJ2", "COR", "OBJ2"))
            {

                nflist.AddRange(GET_OBJ2((ParseNode)node.Children[0], parseTree,verbFrame));
                nflist.AddRange(GET_OBJ2((ParseNode)node.Children[2], parseTree,verbFrame));
            }


            //throw new NotImplementedException();
            return nflist;
        }
        private List<NounFrame> GET_OBJ2(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //    OBJ2=ETH+OBJ3+OR+OBJ3
            //    OBJ2=NTH+OBJ3+NOR+OBJ3
            //    OBJ2=NOTN+OBJ3+BUTL+OBJ3
            //    OBJ2=BTH+OBJ3+AND+OBJ3
            //    OBJ2=WTH+OBJ3+OR+OBJ3

            List<NounFrame> nflist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "OBJ3", "OR", "OBJ3"))
            {
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[1],parseTree,verbFrame));
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[3],parseTree,verbFrame));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "OBJ3", "NOR", "OBJ3"))
            {
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[1],parseTree,verbFrame));
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[3],parseTree,verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "OBJ3", "BUTL", "OBJ3"))
            {
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[1],parseTree,verbFrame));
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[3],parseTree,verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "OBJ3", "AND", "OBJ3"))
            {
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[1],parseTree,verbFrame));
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[3],parseTree,verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH", "OBJ3", "OR", "OBJ3"))
            {
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[1], parseTree,verbFrame ));
                nflist.AddRange(GET_OBJ3((ParseNode)node.Children[3], parseTree,verbFrame));
            }

            // throw new NotImplementedException();
            return nflist;
        }
        private List<NounFrame> GET_OBJ3(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //OBJ3=SOBJ
            //OBJ3=OBJ4

            List<NounFrame> nflist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ"))
            {
                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree,verbFrame));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OBJ4"))
            {
                nflist.AddRange(GET_OBJ4((ParseNode)node.Children[0], parseTree,verbFrame));
            }

            return nflist;

        }

        private List<NounFrame> GET_OBJ4(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //OBJ4=SOBJ+CMA+OBJ4
            //OBJ4=SOBJ+COR+SOBJ

            List<NounFrame> nflist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ", "CMA", "OBJ4"))
            {
                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree, verbFrame));
                nflist.AddRange(GET_OBJ4((ParseNode)node.Children[2], parseTree,verbFrame));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOBJ", "COR", "SOBJ"))
            {
                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[0], parseTree,verbFrame));
                nflist.AddRange(GET_SOBJ((ParseNode)node.Children[2], parseTree,verbFrame));
            }
            return nflist;

        }




        public List<NounFrame> GET_SOBJ(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            List<NounFrame> nflist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NPC"))
            {
                nflist.AddRange(GET_NPC((ParseNode)node.Children[0], parseTree, verbFrame));
            }
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NC"))
            {
                nflist.AddRange(GET_NC((ParseNode)node.Children[0], parseTree, verbFrame));
            }

            return nflist;

        }


        //public List<NounFrame> GET_NPC(ParseNode node,ParseTree parseTree)
        //{
        //    //incomplete
        //    List<NounFrame> nflist = new List<NounFrame>();
        //    int i = 0;
        //    //foreach (ParseNode n in node.Children)
        //    //{
        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NNC"))
        //    {
        //        nflist.AddRange(GET_NNC((ParseNode)node.Children[0], parseTree));
        //    }
        //    else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "NNC"))
        //    {
        //        //ARC=the......
        //        nflist.AddRange(GET_NNC((ParseNode)node.Children[1], parseTree));
        //    }
        //    //}
        //    return nflist;
        //}
   //public List<NounFrame> GET_NNC(ParseNode node, ParseTree parseTree)
   //     {
   //         //NNC=N
   //         //NNC=PPN
   //         //NNC=VING
   //         List<NounFrame> nflist = new List<NounFrame>();

   //         if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "N"))
   //         {
   //             nflist.AddRange(GET_N((ParseNode)node.Children[0], parseTree));
   //         }
   //         else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPN"))
   //         {
   //             //nflist.AddRange(GET_PPN((ParseNode)node.Children[0], parseTree));
   //         }
   //         else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING"))
   //         {
   //             nflist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
   //         }

   //         return nflist;
   //     }

        //NOTE
        public List<NounFrame> GET_NPC(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            //incomplete

                        
            //NPC=NNC
            //NPC=OPP
            //NPC=OPP+RXPN
            //NPC=RXPN
            //NPC=IDFP
            //NPC=DEMP
            //NPC=EACHO
            //NPC=ONEAN
            //NPC=VING+AVJ
            //NPC=AVJ+NNC
            //NPC=AVJ+NNC+FAVJ
            //NPC=NNC+FAVJ
            //NPC=ARC+NNC
            //NPC=ARC+AVJ+NNC
            //NPC=ARC+AVJ+NNC+FAVJ
            //NPC=ARC+NNC+FAVJ
            //NPC=PPJ+NNC
            //NPC=PPJ+AVJ+NNC
            //NPC=PPJ+AVJ+NNC+FAVJ
            //NPC=PPJ+NNC+FAVJ
            //NPC=REPC+NNC
            //NPC=REPC+AVJ+NNC
            //NPC=REPC+AVJ+NNC+FAVJ
            //NPC=REPC+NNC+FAVJ
            //NPC=CNP+NNC
            //NPC=CNP+AVJ+NNC
            //NPC=CNP+AVJ+NNC+FAVJ
            //NPC=CNP+NNC+FAVJ
            //NPC=ARC+CNP+NNC
            //NPC=ARC+CNP+AVJ+NNC
            //NPC=ARC+CNP+AVJ+NNC+FAVJ
            //NPC=ARC+CNP+NNC+FAVJ
            //NPC=ARC+OF+NNC
            //NPC=ARC+OF+NNC+FAVJ
            //NPC=ARC+OF+ARC+NNC
            //NPC=ARC+OF+ARC+NNC+FAVJ
            //NPC=ALL+ARC+NNC
            //NPC=ALL+ARC+NNC+FAVJ
            //NPC=NPC+RXPN


            List<NounFrame> nflist = new List<NounFrame>();

            List<MyWordInfo> Adjectives=new List<MyWordInfo>();

            //if AVJ=adjectives...then GET_AVJ returns strings

            //should adjectives be strings or parsenodes?
            //List<string> AdjList = new List<string>();

            //List<ParseNode> AdjList = new List<ParseNode>();

            //int i = 0;

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NNC"))
            {
                nflist.AddRange(GET_NNC((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "NNC"))
            {
                //ARC=the......
                nflist.AddRange(GET_NNC((ParseNode)node.Children[1], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "NNC", "FAVJ"))
            {
                //ARC=the......
                nflist.AddRange(GET_NNC((ParseNode)node.Children[1], parseTree));
                //3ashan arbot el adjectives bel noun elli gebto


                List<NounFrame> temp = new List<NounFrame>();

                //of literature gebtaha hena...bas already rabattah bel history as a noun....fa no need to return
                //coz it will then be added like history b case role lel verb which i don't want
                temp.AddRange(GET_FAVJ_noun((ParseNode)node.Children[2], parseTree, nflist));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"NNC", "FAVJ"))
            {
                //ARC=the......
                nflist.AddRange(GET_NNC((ParseNode)node.Children[0], parseTree));
                
                List<NounFrame> temp = new List<NounFrame>();
                //pain in chest....pain ma3aya w 3ayza chest.....
                //same as above
                temp.AddRange(GET_FAVJ_noun((ParseNode)node.Children[1], parseTree, nflist));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "AVJ", "NNC", "FAVJ"))
            {
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_NNC((ParseNode)node.Children[1], parseTree));

                for (int i = 0; i < nflist.Count; i++)
                {
                    nflist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }

                //of influenza
                List<NounFrame> temp = new List<NounFrame>();
                temp.AddRange(GET_FAVJ_noun((ParseNode)node.Children[2], parseTree, nflist));
            }
                //NPC=ARC+AVJ+NNC+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "NNC", "FAVJ"))
            {
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                nflist.AddRange(GET_NNC((ParseNode)node.Children[2], parseTree));

                for (int i = 0; i < nflist.Count; i++)
                {
                    nflist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }

                //of influenza
                List<NounFrame> temp = new List<NounFrame>();
                temp.AddRange(GET_FAVJ_noun((ParseNode)node.Children[3], parseTree, nflist));
            }
            //NPC=PPJ+NNC+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "NNC", "FAVJ"))
            {
                nflist = GET_NNC((ParseNode)node.Children[1], parseTree);
                List<NounFrame> temp = new List<NounFrame>();
                temp.AddRange(GET_FAVJ((ParseNode)node.Children[2], parseTree, nflist));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "AVJ", "NNC"))
            {
                //AVJ=3...
                //AdjList.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_NNC((ParseNode)node.Children[1], parseTree));


                //testing
                //nfl.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));

                //for (int i = 0; i < nflist.Count; i++)
                //{
                //    nflist[i].TestAdj = nfl;
                //    //for (int j = 0; j < nfl.Count; j++)
                //    //{
                //    //    nflist[i].TestAdj[j] = nfl[j];
                //    //}
                //}

                for (int i = 0; i < nflist.Count; i++)
                {
                    nflist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                /////////////////////////////////////////////////////////////////////////////////////

                //amshy 3ala kol else nouns elli 3andy w a7ott feeha else adjectives beta3ti
                //for (int i = 0; i < nflist.Count; i++)
                //{
                //    for (int j = 0; j < AdjList.Count; j++)
                //    {
                //        nflist[i].Adjective[j] = AdjList[j];
                //    }
                //}

                //nflist[0].Adjective = node.Children[0].ToString();
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "NNC"))
            {
                //ARC=the
                //AVJ=adjective

                nflist.AddRange(GET_NNC((ParseNode)node.Children[2], parseTree));
                // AdjList.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));

                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));

                for (int i = 0; i < nflist.Count; i++)
                {
                    nflist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }

                //for (int i = 0; i < nflist.Count; i++)
                //{
                //    nflist[i].TestAdj = nfl;
                //    //for (int j = 0; j < nfl.Count; j++)
                //    //{
                //    //    nflist[i].TestAdj[j] = nfl[j];
                //    //}
                //}

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "NNC"))
            {
                nflist.AddRange(GET_NNC((ParseNode)node.Children[1], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN")
                {

                    foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                    {
                        foreach (NounFrame nf in Nounframes)
                        {
                            string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                            ParseNode pn1 = nf.ParseNode;

                            if (nf.Text == n.Text)
                            {
                                // return nounframelist;
                                // nfs.Add(n1);
                                //                    // Nounframes.(nf);
                                // nounframelist.Add(nf);
                                //if(nf.Ownerof.ContainsValue(
                                nf.AddCaseRolenouns(CaseRole.OwnerOf, nflist[0]);
                                //                    //List<NounFrame> nn =new List<NounFrame>();
                                //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                //                    //nf.Ppj.AddRange(nn);
                                break;
                            }
                        }
                    }
                }


                //nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //return nounframelist;
            }
            ////NPC=PPJ+AVJ+NNC
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "AVJ", "NNC"))
            {


                nflist.AddRange(GET_NNC((ParseNode)node.Children[2], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN")
                {

                    foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                    {
                        foreach (NounFrame nf in Nounframes)
                        {
                            string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                            ParseNode pn1 = nf.ParseNode;

                            if (nf.Text == n.Text)
                            {
                                // return nounframelist;
                                // nfs.Add(n1);
                                //                    // Nounframes.(nf);
                                // nounframelist.Add(nf);
                                //if(nf.Ownerof.ContainsValue(
                                nf.AddCaseRolenouns(CaseRole.OwnerOf, nflist[0]);
                                //                    //List<NounFrame> nn =new List<NounFrame>();
                                //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                //                    //nf.Ppj.AddRange(nn);
                                break;
                            }
                        }
                    }
                }
               

                //nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //return nounframelist;
                //testing
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nflist.Count; i++)
                {
                    nflist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }

                //for (int i = 0; i < nflist.Count; i++)
                //{
                //    nflist[i].TestAdj = nfl;
                //    //for (int j = 0; j < nfl.Count; j++)
                //    //{
                //    //    nflist[i].TestAdj[j] = nfl[j];
                //    //}
                //}




            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OPP"))
            {
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode != null)
                    foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                    {
                        foreach (NounFrame nf in Nounframes)
                        {
                            string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                            ParseNode pn1 = nf.ParseNode;

                            if (nf.Text == n.Text)
                            {
                                // return nounframelist;
                                // nfs.Add(n1);
                                //                    // Nounframes.(nf);
                                // nounframelist.Add(nf);
                                //if(nf.Ownerof.ContainsValue(
                                //nf.AddCaseRolenouns(CaseRole.OwnerOf, nflist[0]);
                                //                    //List<NounFrame> nn =new List<NounFrame>();
                                //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                //                    //nf.Ppj.AddRange(nn);
                                nflist.Add(nf);
                                break;
                            }
                        }
                    }
            }
            return nflist;
        }

        

        public List<NounFrame> GET_NNC(ParseNode node, ParseTree parseTree)
        {
            //NNC=N
            //NNC=PPN
            //NNC=VING
            List<NounFrame> nflist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "N"))
            {
                nflist.AddRange(GET_N((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPN"))
            {
                nflist.AddRange(GET_PPN((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING"))
            {
                nflist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
            }

            return nflist;
        }



        public List<NounFrame> GET_N(ParseNode node, ParseTree parseTree)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(node, parseTree);
            int length = Nounframes.Count;
            // + fillers
            bool found = false;
            if (Nounframes.Count != 0)
            {
                //foreach (NounFrame nf in Nounframes)
                for(int i=0;i<length;i++)
                {
                    NounFrame nf = Nounframes[i];
                    if (nf.Text == nframe.Text)
                    {
                        found = true;

                        nframe = nf;
                        break;
                    }
                    
                }
                if (!found)
                {
                    nframe.Sentence_ID = SentenceID;
                    AddNounFrame(nframe);
                }

            }
            else
            {
                AddNounFrame(nframe);
            }
            nounframelist.Add(nframe);
            return nounframelist;
            
        }


        //public VerbFrame GET_PRDS1(ParseNode node,ParseTree parseTree)
        //{
        //    VerbFrame vf = new VerbFrame(node);
        //    if (node.Children != null)
        //    {
        //        int i = 0;
        //        foreach (ParseNode n in node.Children)
        //        {
        //            if (((ParseNode)n).Goal == "PRD")
        //            {
        //                vf = GET_V((ParseNode)node.Children[i], parseTree);
                        
        //            }

        //            else if (((ParseNode)n).Goal == "COR")
        //            {
        //                //terminal//to
        //            }
        //        }
        //    }
        //    vf.Sentence_ID = SentenceID;
        //    return vf;
        //}


       public VerbFrame GET_V(ParseNode node,ParseTree parseTree)
        {
            

            //List<VerbFrame> vflist = new List<VerbFrame>();
            //VerbFrame v = new VerbFrame(node);
            //AddVerbrame(v);
            //vflist.Add(v);
            //return vflist;

            VerbFrame v = new VerbFrame(node);
            string verbname = SentenceParser.GetWordString(parseTree, node);
            v.VerbName = verbname;
            v.Sentence_ID = SentenceID;
            AddVerbrame(v);
            return v;
          
            
        }
       public VerbFrame GET_VP(ParseNode node, ParseTree parseTree)
       {
           VerbFrame vf = new VerbFrame(node);

          // List<string> advlist = new List<string>();
           List<MyWordInfo> advlist = new List<MyWordInfo>();
           if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1", "ADVS", "VPSP"))
           {
               advlist = GET_ADVS((ParseNode)node.Children[1], parseTree);
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               vf.Passive = true;
               //string verbname = SentenceParser.GetWordString(parseTree,(ParseNode) node.Children[2]);
               //vf.VerbName = verbname;
               vf.AdverbsInfo.AddRange(advlist);
               //AddVerbrame(vf);
               
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1","VPSP"))
           {
              // advlist = GET_ADVS((ParseNode)node.Children[1], parseTree);
               vf = GET_V((ParseNode)node.Children[1], parseTree);
               vf.Passive = true;
               vf.VerbTense = VerbTense.Present;
               //string verbname = SentenceParser.GetWordString(parseTree,(ParseNode) node.Children[2]);
               //vf.VerbName = verbname;
              // vf.Adverb.AddRange(advlist);
               //AddVerbrame(vf);

           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "HAV1", "VPSP"))
           {
               //nflist.AddRange(GET_ADVS((ParseNode)node.Children[i], parseTree));
               vf = GET_V((ParseNode)node.Children[1], parseTree);
               vf.Passive = false;
               if (((ParseNode)node.Children[0]).Text == "HAS" || ((ParseNode)node.Children[0]).Text == "HAVE")
               {
                   vf.VerbTense = VerbTense.PresentPerfect;

               }
               else if (((ParseNode)node.Children[0]).Text == "HAD")
               {
                   vf.VerbTense = VerbTense.PastPerfect;
               }
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "HAV1", "ADVS", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               vf.Passive = false;
               if (((ParseNode)node.Children[0]).Text == "HAS" || ((ParseNode)node.Children[0]).Text == "HAVE")
               {
                   vf.VerbTense = VerbTense.PresentPerfect;

               }
               else if (((ParseNode)node.Children[0]).Text == "HAD")
               {
                   vf.VerbTense = VerbTense.PastPerfect;
               }

              // vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));


           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "HAV1", "BEN", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               vf.Passive = true;
               if (((ParseNode)node.Children[0]).Text == "HAS" || ((ParseNode)node.Children[0]).Text == "HAVE")
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.PresentPerfect;

               }
               else if (((ParseNode)node.Children[0]).Text == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.PastPerfect;
               }

           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "HAV1", "BEN", "ADVS", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[3], parseTree);
               vf.Passive = true;
               if (((ParseNode)node.Children[0]).Text == "HAS" || ((ParseNode)node.Children[0]).Text == "HAVE")
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.PresentPerfect;

               }
               else if (((ParseNode)node.Children[0]).Text == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.PastPerfect;
               }

               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "HAV1", "BEN", "VING"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               vf.Passive = false;

               if (((ParseNode)node.Children[0]).Text == "HAS" || ((ParseNode)node.Children[0]).Text == "HAVE")
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.PresentPerfectContinuous;

               }
               else if (((ParseNode)node.Children[0]).Text == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.PastPerfectContinuous;
               }


           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "HAV1", "BEN", "ADVS", "VING"))
           {
               vf = GET_V((ParseNode)node.Children[3], parseTree);
               vf.Passive = false;
               if (((ParseNode)node.Children[0]).Text == "HAS" || ((ParseNode)node.Children[0]).Text == "HAVE")
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.PresentPerfectContinuous;

               }
               else if (((ParseNode)node.Children[0]).Text == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.PastPerfectContinuous;
               }
              // vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "HAV", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf.Passive = false;
               if (((ParseNode)node.Children[0]).Text == "WILL" || ((ParseNode)node.Children[0]).Text == "SHALL")
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.FuturePerfect;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.unknown;
               }

               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                 || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                 || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                 || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "HAV", "ADVS", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[3], parseTree);
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf.Passive = false;
               if (((ParseNode)node.Children[0]).Text == "WILL" || ((ParseNode)node.Children[0]).Text == "SHALL")
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.FuturePerfect;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.Aux;
               }

               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                  || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                  || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                  || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }
               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.Passive = true;
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "HAV", "BEN", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[3], parseTree);
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf.Passive = true;
               if (((ParseNode)node.Children[0]).Text == "WILL")//
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.FuturePerfect;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.Aux;
               }

               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                   || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }

           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "HAV", "BEN", "ADVS", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[4], parseTree);
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf.Passive = true;
               if (((ParseNode)node.Children[0]).Text == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.unknown;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.unknown;
               }
               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                   || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }

               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[3], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[3], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "HAV", "BEN", "VING"))
           {
               vf.Passive = false;
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf = GET_V((ParseNode)node.Children[4], parseTree);
               if (((ParseNode)node.Children[0]).Text == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.FuturePerfectContinuous;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.Aux;
               }
               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                   || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }


           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "HAV", "BEN", "ADVS", "VING"))
           {
               vf = GET_V((ParseNode)node.Children[5], parseTree);
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf.Passive = false;
               if (((ParseNode)node.Children[0]).Text == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.FuturePerfectContinuous;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.Aux;
               }

               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                   || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }
              // vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[4], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[4], parseTree));
           }
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "HAV", "BEN", "ADVS", "VING"))
           //{
               
           //}
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "BE", "VPSP"))
           {
               vf.Passive = true;
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               if (XV == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.FuturePerfect;

               }

               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.Aux;
               }

               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                   || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "BE", "ADVS", "VPSP"))
           {
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);

               vf = GET_V((ParseNode)node.Children[3], parseTree);
               if (XV == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.unknown;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.unknown;
               }

               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                   || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }

               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.Passive = true;

           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "BE", "VING"))
           {
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf.Passive = false;
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               if (XV == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.FutureContinuous;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.Aux;
               }
               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                   || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "BE", "ADVS", "VING"))
           {
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               if (XV == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.FutureContinuous;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.Aux;
               }

               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                   || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }

              // vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "VINF"))
           {
               //not added
               //WOULD SHALL CAN COULD MUST MAY MIGHT    CANNOT 
               // MUST_NOT MAY_NOT MIGHT_NOT MUSTN'T
               //added
               //SHOULD_NOT SHOULDN'T SHALL_NOT SHAN'T COULD_NOT COULDN'T CAN'T
               //WOULD_NOT
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);;

               vf = GET_V((ParseNode)node.Children[1], parseTree);
               if (XV == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.Future;

               }
               else
               {
                   vf.VerbTense = VerbTense.Aux;
               }
                             
               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT" 
                   || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T" 
                   || XV == "WOULD_NOT" || XV == "WOULDN'T"|| XV ==  "CANNOT" || XV ==  "COULD_NOT"
                   || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }
               
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "XV", "ADVS", "VINF"))
           {
               vf.Passive = false;
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               string XV = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]); ;
               if (((ParseNode)node.Children[0]).Text == "WILL")//maybe more
               {
                   //vf.VerbTense = VerbTense.PresentPerfect;
                   vf.VerbTense = VerbTense.Future;

               }
               else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               {
                   //vf.VerbTense = VerbTense.PastPerfect;
                   vf.VerbTense = VerbTense.unknown;
               }
               if (XV == "SHOULD_NOT" || XV == "SHOULDN'T" || XV == "SHALL_NOT"
                  || XV == "SHAN'T" || XV == "COULD_NOT" || XV == "CAN'T" || XV == "COULDN'T"
                  || XV == "WOULD_NOT" || XV == "WOULDN'T" || XV == "CANNOT" || XV == "COULD_NOT"
                  || XV == "MUST_NOT" || XV == "MAY_NOT" || XV == "MIGHT_NOT" || XV == "MUSTN'T")
               {
                   vf.VerbNegation = true;
               }
              // vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "VINF"))
           {
               //"OUGHT_TO"
               //zay should be
               vf = GET_V((ParseNode)node.Children[1], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}

               vf.VerbTense = VerbTense.unknown;
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "ADVS", "VINF"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}
               vf.VerbTense = VerbTense.unknown;
             //  vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "BE", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}
               vf.VerbTense = VerbTense.unknown;
               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "ADVS", "VINF"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}
               vf.VerbTense = VerbTense.unknown;
              // vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "BE", "ADVS", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[3], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}
               //PAST eah?
               vf.VerbTense = VerbTense.unknown;
               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "BE", "VING"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}
               //continuous eah?
               vf.VerbTense = VerbTense.unknown;
               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "BE", "ADVS", "VING"))
           {
               vf = GET_V((ParseNode)node.Children[3], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}
               //CONTINUOUS eah?
               vf.VerbTense = VerbTense.unknown;
              // vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));

           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "HAV", "VPSP"))
           {
               //ought to have taken
               //should have taken
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}
               vf.VerbTense = VerbTense.unknown;
               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "HAV", "ADVS", "VPSP"))
           {
               vf = GET_V((ParseNode)node.Children[3], parseTree);
               //if ((ParseNode)node.Children[0].ToString() == "WILL")//maybe more
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.Future;

               //}
               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}
               vf.VerbTense = VerbTense.unknown;
               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
           }

               //VP=BE1+ADVS+V

           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1", "ADVS", "V"))
           {
               vf = GET_V((ParseNode)node.Children[2], parseTree);
               
               vf.VerbTense = VerbTense.unknown;
               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));
               
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
               //ADVS-->ADV-->PADV-->MyWordInfo
               foreach (MyWordInfo w in vf.AdverbsInfo)
               {
                   if (w.Word == "NOT")
                       vf.VerbNegation = true;
                        
               }
                
           }
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "HAV", "BEN", "VPSP"))
           //{
           //    //OUT+HAV+BEN+VPSP
           //    //kjhkjhgkjhgk
           //}
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "HAV", "BEN", "ADVS", "VPSP"))
           //{
           //    //OUT+HAV+BEN+ADVS+VPSP
           //    //kjhkjhgkjhgk
           //}
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "HAV", "BEN", "VING"))
           //{
           //    //OUT+HAV+BEN+VING
           //    //kjhkjhgkjhgk
           //}
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "OUT", "HAV", "BEN", "ADVS", "VING"))
           //{
           //    //OUT+HAV+BEN+ADVS+VING
           //    //kjhkjhgkjhgk
           //}
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "DO1", "VINF"))
           //{
           //    //DO1+VINF
           //    //kjhkjhgkjhgk
           //}
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "DO1", "ADVS", "VINF"))
           {
               //DO1+ADVS+VINF

               vf = GET_VINF((ParseNode)node.Children[2], parseTree);
               //if (((ParseNode)node.Children[0]).Text == "DOES" || ((ParseNode)node.Children[0]).Text == "DOES" || ((ParseNode)node.Children[0]).Text == "DID" )
               //{
               //    //vf.VerbTense = VerbTense.PresentPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //    vf.VerbNegation = true;

               //}
               List<MyWordInfo> advs = new List<MyWordInfo>();
               advs.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
               // vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
               //ADVS-->ADV-->PADV-->MyWordInfo
               foreach (MyWordInfo w in advs)
               {
                   if (w.Word == "NOT")
                       vf.VerbNegation = true;
                        
               }
               //vf.Passive = false;

               vf.VerbTense = VerbTense.unknown;

               //else// if ((ParseNode)node.Children[0].ToString() == "HAD")
               //{
               //    //vf.VerbTense = VerbTense.PastPerfect;
               //    vf.VerbTense = VerbTense.unknown;
               //}



           }

           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1", "VING"))
           {
               //BE1+VING
               vf = GET_V((ParseNode)node.Children[1], parseTree);

               vf.VerbTense = VerbTense.PastContinuous;

               //vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[2], parseTree));

           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1", "ADVS", "VING"))
           {
               //BE1+ADVS+VING
               vf = GET_V((ParseNode)node.Children[2], parseTree);

               vf.VerbTense = VerbTense.PastContinuous;
               // vf.Adverb.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
               vf.AdverbsInfo.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
           }
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1", "BING", "VPSP"))
           //{
           //    //BE1+BING+VPSP
           //    //kjhkjhgkjhgk
           //}
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1", "BING", "ADVS", "VPSP"))
           //{
           //    //BE1+BING+ADVS+VPSP
           //    
           //}
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1", "GNTO", "VINF"))
           //{
           //    //BE1+GNTO+VINF
           //    //kjhkjhgkjhgk
           //}
           //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BE1", "GNTO", "ADVS", "VINF"))
           //{
           //    //BE1+GNTO+ADVS+VINF
           //    //kjhkjhgkjhgk
           //}

           for (int i = 0; i < advlist.Count; i++)
           {
               MyWordInfo wi = advlist[i];
               if (wi.Word.ToUpper() == "NOT")
               {
                   advlist.RemoveAt(i);
                   vf.VerbNegation = true;
                   break;
               }
           }
           return vf;
       }

        //nouns

       public List<NounFrame> GET_SBJ(ParseNode node, ParseTree parseTree, List<NounFrame> nflist)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();

                  if(MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"SSBJ"))
                  {
                      //nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree, verbFrame));
                      List<NounFrame> temp = GET_SSBJ((ParseNode)node.Children[0], parseTree, nflist);
                      foreach (NounFrame nf in temp)
                      {
                          if (!nounframelist.Contains(nf))
                          {
                              nounframelist.Add(nf);
                          }
                      }
                  }
                  else  if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SBJ1"))
                  {
                      //nounframelist.AddRange(GET_SBJ1((ParseNode)node.Children[0], parseTree, verbFrame));
                      //List<NounFrame> temp = new List<NounFrame>();
                      nounframelist.AddRange(GET_SBJ1((ParseNode)node.Children[0], parseTree, nflist));
                  }
                  else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SBJ2"))
                  {
                      //nounframelist.AddRange(GET_SBJ2((ParseNode)node.Children[0], parseTree, verbFrame));
                      nounframelist.AddRange(GET_SBJ2((ParseNode)node.Children[0], parseTree, nflist));
                  }
                  
                  //foreach (NounFrame nf in Nounframes)
                  //{
                     //nf.Ppj.AddRange(nounframelist);
                  //}

            //if (((ParseNode)node.Children[0]).Goal == "SSBJ")
            //{
            //    nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree));

            //}
            //else if (((ParseNode)node.Children[0]).Goal == "SBJ1")
            //{
            //    nounframelist.AddRange(GET_SBJ1((ParseNode)node.Children[0], parseTree));
            //}
            //else if (((ParseNode)node.Children[0]).Goal == "SBJ2")
            //{
            //    nounframelist.AddRange(GET_SBJ2((ParseNode)node.Children[0], parseTree));
            //}
            return nounframelist;

        }


       public List<NounFrame> GET_SBJ1(ParseNode node, ParseTree parseTree, List<NounFrame> nflist)//@@na2es COR msh 3arfa de eh@@
        {
            List<NounFrame> nounframelist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SSBJ", "CMA", "SBJ1"))
            {
                nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree, nflist));
                //nounframelist.AddRange(GET_CMA((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_SBJ1((ParseNode)node.Children[2], parseTree, nflist));
            }
           
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SSBJ", "COR", "SSBJ"))
            {
                nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree, nflist));
               // nounframelist.AddRange(GET_COR((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[2], parseTree, nflist));
            }
            //SBJ1=SBJ2+CMA+SBJ1
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SBJ2", "CMA", "SBJ1"))
            {
                nounframelist.AddRange(GET_SBJ2((ParseNode)node.Children[0], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ1((ParseNode)node.Children[2], parseTree, nflist));
            }
            //SBJ1=SSBJ+COR+SBJ2

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SSBJ", "COR", "SBJ2"))
            {
                nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ2((ParseNode)node.Children[2], parseTree, nflist));
            }
            //SBJ1=SBJ2+COR+SBJ2
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SBJ2", "COR", "SBJ2"))
            {
                nounframelist.AddRange(GET_SBJ2((ParseNode)node.Children[0], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ2((ParseNode)node.Children[2], parseTree, nflist));
            }

            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "SSBJ")
            //    {
            //       nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[i],parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "SBJ1")
            //    {
            //       nounframelist.AddRange(GET_SBJ1((ParseNode)node.Children[i],parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "SBJ2")
            //    {
            //      nounframelist.AddRange( GET_SBJ2((ParseNode)node.Children[i],parseTree));
            //    }

            //    i++;
            //}
            return nounframelist;
        }


       public List<NounFrame> GET_SBJ2(ParseNode node, ParseTree parseTree, List<NounFrame> nflist)//@@ questions look in rules@@
        {
            List<NounFrame> nounframelist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "SBJ3", "OR", "SBJ3"))
            {
               // NounFrame nf = new NounFrame((ParseNode)node.Children[0], parseTree);
               //AddNounFrame(nf);
               // nounframelist.Add(nf);
               // nf.Adjective.Add((ParseNode)node.Children[0]);
                

                
                //NounFrame nf1 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //AddNounFrame(nf1);
                //nounframelist.Add(nf1);
               // nf.Adjective.Add((ParseNode)node.Children[2]);

                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[1], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[3], parseTree, nflist));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "SBJ3", "NOR", "SBJ3"))
            {
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[1], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[3], parseTree, nflist));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH", "SBJ3", "OR", "SBJ3"))
            {
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[1], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[3], parseTree, nflist));
            }
            //SBJ2=NOTN+SBJ3+BUTL+SBJ3

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "SBJ3", "BUTL", "SBJ3"))
            {
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[1], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[3], parseTree, nflist));
            }
            //SBJ2=BTH+SBJ3+AND+SBJ3

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "SBJ3", "AND", "SBJ3"))
            {
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[1], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[3], parseTree, nflist));
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "SBJ3")
            //    {
            //        nounframelist.AddRange(GET_SBJ3((ParseNode)node.Children[i], parseTree));
            //    }

            //    //else if (((ParseNode)n).Goal == "")
            //    //{
            //    //    GET_((ParseNode)node.Children[i]);
            //    //}

            //    //else if (((ParseNode)n).Goal == "")
            //    //{
            //    //    GET_((ParseNode)node.Children[i]);
            //    //}

            //    i++;
            //}
           return nounframelist;
        }


       public List<NounFrame> GET_SBJ3(ParseNode node, ParseTree parseTree, List<NounFrame> nflist)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SSBJ"))
            {
                nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree, nflist));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SBJ4"))
            {
                nounframelist.AddRange(GET_SBJ4((ParseNode)node.Children[0], parseTree, nflist));
            }
            // if (((ParseNode)node.Children[0]).Goal == "SSBJ")
            //{

            //   nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree));

            //}

            // if (((ParseNode)node.Children[1]).Goal == "SBJ4")
            // {
            //    nounframelist.AddRange(GET_SBJ4((ParseNode)node.Children[0], parseTree));

            // }
             return nounframelist;
        }


        public List<NounFrame> GET_SBJ4(ParseNode node, ParseTree parseTree, List<NounFrame> nflist)//@@bardo COR na2sa@@
        {
            
            List<NounFrame> nounframelist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SSBJ", "CMA", "SBJ4"))
            {
                nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree, nflist));
                nounframelist.AddRange(GET_SBJ4((ParseNode)node.Children[2], parseTree, nflist));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SSBJ", "COR", "SSBJ"))
            {
                nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0], parseTree, nflist));
                nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[2], parseTree, nflist));
            }

            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)node.Children[0]).Goal == "SSBJ")
            //    {

            //       nounframelist.AddRange(GET_SSBJ((ParseNode)node.Children[0],parseTree));

            //    }

            //    if (((ParseNode)node.Children[1]).Goal == "SBJ4")
            //    {
            //       nounframelist.AddRange(GET_SBJ4((ParseNode)node.Children[0],parseTree));

            //    }
               
            //}
            return nounframelist;
        }

        /// <summary>
        /// Integration from Fatma
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parseTree"></param>
        /// <returns></returns>
        public List<NounFrame> GET_SSBJ(ParseNode node, ParseTree parseTree, List<NounFrame> nflist)
        {
                //SSBJ=PRPS
                //SSBJ=NC
                //SSBJ=NP
                //SSBJ=INFPH
            List<NounFrame> nounframelist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NP"))
            {
                //nounframelist.AddRange(GET_NP((ParseNode)node.Children[0], parseTree,verbFrame));
                nounframelist.AddRange(GET_NP((ParseNode)node.Children[0], parseTree, nflist));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS"))
            {
                //NounFrame nframe = GET_PRPS((ParseNode)node.Children[0], parseTree);
               // nounframelist.Add(nframe);

                nounframelist.AddRange(GET_PRPS((ParseNode)node.Children[0], parseTree, null));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NC"))
            {
                //nounframelist.AddRange(GET_NP((ParseNode)node.Children[0], parseTree,verbFrame));
                nounframelist.AddRange(GET_NC((ParseNode)node.Children[0], parseTree, null));
                //check di
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "INFPH"))
            {
               // nounframelist.AddRange(GET_INFPH((ParseNode)node.Children[0], parseTree));
            }

                //if (((ParseNode)node.Children[0]).Goal == "PRPS")
                //{
                //   nounframelist.AddRange(GET_PRPS((ParseNode)node.Children[0], parseTree));
                //}
                //else if (((ParseNode)node.Children[0]).Goal == "NC")
                //{
                //    nounframelist.AddRange(GET_NC((ParseNode)node.Children[0], parseTree));
                //}
                //else if (((ParseNode)node.Children[0]).Goal == "NP")
                //{
                //  nounframelist.AddRange(GET_NP((ParseNode)node.Children[0],parseTree));
                //}
                //else if (((ParseNode)node.Children[0]).Goal == "INFPH")
                //{
                //   //nounframelist.AddRange(GET_INFPH((ParseNode)node.Children[0],parseTree));//@@beta3 TEEMA
                //}

                return nounframelist;
        }



        public List<NounFrame> GET_PRPS_nouns(ParseNode node, ParseTree parseTree, List<NounFrame> nounframes)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP"))
            {
                nounframelist.AddRange(GET_PRP_nouns((ParseNode)node.Children[0], parseTree, nounframes));
               
            }
            return nounframelist;
 
        }

        private List<NounFrame> GET_PRP_nouns(ParseNode parseNode, ParseTree parseTree,List<NounFrame> nounframes)
        {
             List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(parseNode);
            
            string Preposition = SentenceParser.GetWordString(parseTree, (ParseNode)parseNode.Children[0]);
                
                //((ParseNode)node.Children[0]).Text.ToString();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(parseNode, "PRS", "OBJ"))
            {
                //NounFrame n = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(n);
                //AddNounFrame(n);

                nounframelist.AddRange(GET_OBJ((ParseNode)parseNode.Children[1], parseTree, null));
                List<MyWordInfo> Adjectives=new List<MyWordInfo>();
                MyWordInfo w=new MyWordInfo();

                //obtain nounframes then check
                #region Prepositions
                if (Preposition == "IN")
                {
                    //maybe location---in the house
                    //time---in 1990
                    //in the history???---time???????
                    //check in file
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (Is_Time(nf))
                            {
                                //pain in chest and abdomen....pain f nounframes w heya elli me7tag a add leeha caseroles le kol 7aga ba3d el "in" i.e chest, abdomen

                                //verbFrame.AddCaseRole(CaseRole.time, nf);

                            }
                            else if (Is_Location(nf))
                            {
                                // verbFrame.AddCaseRole(CaseRole.location, nf);

                            }
                            else
                            {
                                //testing purposes
                                //verbFrame.AddCaseRole(CaseRole.unknown, nf);
                                n.AddCaseRolenouns(CaseRole.location, nf);
                            }

                        }
                    }


                }
                else if (Preposition == "OF")
                {
                    //Cairo is the capital of Egypt
                   //history of literature....literature adjective of history
                    //foreach (NounFrame nf in nounframelist)
                    //{
                    //    if (Is_Location(nf))
                    //    {
                            //3ayza a-return egypt fa tetrabbat b capital
                            //nounframes[0].AdjectivesInfo.AddRange(nounframelist);
                        //}
                        //else
                        //{
                        //    //return literature which is attached to history fo2
                        //    return nounframelist;
                        //   // nf.Addedto = modaf_eleh;
                        //}

                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            n.AddCaseRolenouns(CaseRole.Possession, nf);
                        }
                    }
                    //adjective returned, convert to my wordinfo
                    //foreach(NounFrame nf in nounframelist)
                    //{
                    //    w.Sense = nf.ParseNode.SenseNo;
                    //    w.Word = nf.Text;
                    //    w.Pos = Wnlib.PartsOfSpeech.Adj;
                    //    Adjectives.Add(w);

                    //}
                    //el objects...history
                    //foreach(NounFrame nf in nounframes)
                    //{
                    //    nf.AdjectivesInfo.AddRange(Adjectives);
                    //}
                        //nf.AdjectivesInfo.AddRange(nounframelist);

                    //int length = Nounframes.Count;
                    //for (int i = 0; i < length; i++)
                    //{
                    //    if (Nounframes[i] != null)
                    //    {
                    //        NounFrame nf = Nounframes[i];
                    //        if (nf.Text == w.Word)
                    //        {
                    //            Nounframes.RemoveAt(i);
                    //            break;
                    //        }
                    //    }
                    //}
                    
                    //return nounframelist;
                    
                    
                    //return the adjective as nounframelist then cast into parsenodes afterwards
                    
                }

                else if (Preposition == "BEFORE")
                {
                    //before the queen?
                    foreach (NounFrame nf in nounframelist)
                    {
                        //verbFrame.AddCaseRole(CaseRole.unknown, nf);
                    }

                    //return nounframelist;
                   
                }
                else if (Preposition == "THROUGH")
                {
                                       
                    
                }
                else if (Preposition == "ALONG")
                {
 
                }
                else if (Preposition == "DURING")
                {
                  

                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            n.AddCaseRolenouns(CaseRole.time, nf);
                        }
                    }
                }
                else if (Preposition == "ACCORDING_TO")
                {
                }
                else if (Preposition == "BY_MEANS_OF")
                {
                }
                else if (Preposition == "ABOUT")
                {
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            n.AddCaseRolenouns(CaseRole.About, nf);
                        }
                    }
                }
                else if (Preposition == "AT")
                {
                }
                else if (Preposition == "NEXT")
                {
                }
                else if (Preposition == "WITH")
                {
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (nf.Concept != null)
                            {
                                if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.location, nf);
                                }
                                else if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.time, nf);

                                }
                                else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.Beneficiary, nf);//might be purpose
                                }
                                else if (nf.Concept.NonVisualProperties["ISOBJECT"][0] == "true")
                                {
                                    //testing purposes
                                    //verbFrame.AddCaseRole(CaseRole.unknown, nf);
                                    n.AddCaseRolenouns(CaseRole.purpose, nf);//for the food
                                }
                                else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.Action, nf);//check
                                }
                                else n.AddCaseRolenouns(CaseRole.unknown, nf);


                            }
                            else n.AddCaseRolenouns(CaseRole.unknown, nf);

                        }
                    }
                }
                else if (Preposition == "FOR")
                {//we headed for the house
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (nf.Concept != null)
                            {
                                if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.location, nf);
                                }
                                else if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.time, nf);

                                }
                                else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.Beneficiary, nf);//might be purpose
                                }
                                else if (nf.Concept.NonVisualProperties["ISOBJECT"][0] == "true")
                                {
                                    //testing purposes
                                    //verbFrame.AddCaseRole(CaseRole.unknown, nf);
                                    n.AddCaseRolenouns(CaseRole.purpose, nf);//for the food
                                }
                                else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.Action, nf);//check
                                }
                                else n.AddCaseRolenouns(CaseRole.unknown, nf);


                            }
                            else n.AddCaseRolenouns(CaseRole.unknown, nf);

                        }
                    }


                }
                else if (Preposition == "AGAINST")
                {
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (nf.Concept != null)
                            {
                                if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                                {
                                    // verbFrame.AddCaseRole(CaseRole.location, nf);

                                }
                            }
                            else
                            {
                                n.AddCaseRolenouns(CaseRole.Against, nf);
 
                            }
                        }
                    }
                    
                }
                else if (Preposition == "UNTO")
                {
                }
                else if (Preposition == "AS_FAR_AS")
                {
                }
                else if (Preposition == "ON")
                {//location or object

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null) 
                            if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                //verbFrame.AddCaseRole(CaseRole.location, nf);

                            }
                    }
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (nf.Concept != null)
                            {
                                if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                                {
                                    // verbFrame.AddCaseRole(CaseRole.location, nf);

                                }
                            }
                            else
                            {
                                n.AddCaseRolenouns(CaseRole.On, nf);

                            }
                        }
                    }
                }
                else if (Preposition == "BEHIND")
                {//location or object

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                        {
                            //verbFrame.AddCaseRole(CaseRole.location, nf);

                        }
                    }
                }
                else if (Preposition == "WITHOUT")
                {
                }
                else if (Preposition == "BY")
                {
                    //by ahmad fa a7mad agent
                    //by the river....location
                    //by 10pm....time
                }
                else if (Preposition == "NEAR_TO")
                {
                }
                else if (Preposition == "TO")
                {
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (nf.Concept != null)
                            {
                                if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.location, nf);
                                }
                                else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.Action, nf);
                                }
                                else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.location, nf);
                                }
                                else
                                {
                                    //testing purposes
                                    n.AddCaseRolenouns(CaseRole.unknown, nf);
                                }

                            }
                            else { n.AddCaseRolenouns(CaseRole.unknown, nf); }
                        }
                    }

                }
                else if (Preposition == "IN_ORDER_TO")
                {
                }
                else if (Preposition == "UPON")
                {
                }
                else if (Preposition == "FROM")
                {
                    //check el concepts+caseroles
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (nf.Concept != null)
                            {
                                if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.location, nf);
                                }
                                else if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.time, nf);

                                }
                                else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                                {
                                    n.AddCaseRolenouns(CaseRole.Beneficiary, nf);//might be purpose
                                }
                                else if (nf.Concept.NonVisualProperties["ISOBJECT"][0] == "true")
                                {
                                    //testing purposes
                                    //verbFrame.AddCaseRole(CaseRole.unknown, nf);
                                    n.AddCaseRolenouns(CaseRole.purpose, nf);//for the food
                                }
                                else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                                {//virus from reproducing
                                    //n.AddCaseRolenouns(CaseRole.Action, nf);//check
                                    n.AddCaseRolenouns(CaseRole.purpose, nf);
                                }
                                else n.AddCaseRolenouns(CaseRole.unknown, nf);


                            }
                            else n.AddCaseRolenouns(CaseRole.unknown, nf);

                        }
                    }

                }
                else if (Preposition == "CONCERNING")
                {
                }
                else if (Preposition == "TOUCHING")
                {
                }
                else if (Preposition == "DURING")
                {
                }
                else if (Preposition == "BECAUSE_OF")
                {
                }
                else if (Preposition == "DUE_TO")
                {
                }
                else if (Preposition == "AFTER")
                {
                }
                else if (Preposition == "AS_A_RESULT_OF")
                {
                }
                else if (Preposition == "BESIDE")
                {
                }
                else if (Preposition == "OUT_OF")
                {
                }
                else if (Preposition == "BEYOND")
                {
                }
                else if (Preposition == "WITHIN")
                {
                }
                else if (Preposition == "BENEATH")
                {
                }
                else if (Preposition == "BETWIXT" || Preposition == "BETWEEN")
                {
                }
                else if (Preposition == "AMONG" || Preposition == "AMID" || Preposition == "AMIDST")
                {
                }
                else if (Preposition == "IN_THE_MIDST" || Preposition == "IN_THE_MIDST_OF")
                {
                }
                else if (Preposition == "NEAR")
                {
                }
                else if (Preposition == "ABOVE")
                {
                }
                else if (Preposition == "EXCEPT")
                {
                }
                else if (Preposition == "IN_ADDITION_TO")
                {
                }
                else if (Preposition == "BESIDES")
                {
                }
                else if (Preposition == "ON_ACCOUNT_OF")
                {
                }
                else if (Preposition == "INSTEAD_OF")
                {
                }
                else if (Preposition == "THROUGHOUT")
                {
                }
                else if (Preposition == "OVER")
                {
                }
                else if (Preposition == "FROM_WITHIN")
                {
                }
                else if (Preposition == "OFF")
                {
                }
                else if (Preposition == "OPPOSITE")
                {
                }
                else if (Preposition == "TOWARDS")
                {
                }
                else if (Preposition == "IN_REPLY_TO")
                {
                }
                else if (Preposition == "UP_TO")
                {
                }
                else if (Preposition == "DURING")
                {
                }
                else if (Preposition == "UNDER")
                {
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (nf.Concept != null)
                            {
                                n.AddCaseRolenouns(CaseRole.Under, nf);
                                //        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                                //        {
                                //            n.AddCaseRolenouns(CaseRole.location, nf);
                                //        }
                                //        else n.AddCaseRolenouns(CaseRole.unknown, nf);
                            }
                            else n.AddCaseRolenouns(CaseRole.Under, nf);
                            //    else n.AddCaseRolenouns(CaseRole.unknown, nf);
                        }
                    }
                }
                else if (Preposition == "AS")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        w.Sense = nf.ParseNode.SenseNo;
                        w.Word = nf.Text;
                        w.Pos = Wnlib.PartsOfSpeech.Adj;
                        Adjectives.Add(w);

                    }
                    //el objects...history
                    foreach (NounFrame nf in nounframes)
                    {
                        nf.AdjectivesInfo.AddRange(Adjectives);
                    }
                }
                else if (Preposition == "LIKE")
                {
                    foreach (NounFrame n in nounframes)
                    {
                        foreach (NounFrame nf in nounframelist)
                        {
                            if (nf.Concept != null)
                            {
                                n.AddCaseRolenouns(CaseRole.example, nf);
                                //        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                                //        {
                                //            n.AddCaseRolenouns(CaseRole.location, nf);
                                //        }
                                //        else n.AddCaseRolenouns(CaseRole.unknown, nf);
                            }
                            else n.AddCaseRolenouns(CaseRole.example, nf);
                        //    else n.AddCaseRolenouns(CaseRole.unknown, nf);
                        }
                    }
                }

                /*
                 * underneath,prs
                    outside_of,prs
                    opposite_to,prs
                    around,prs,padv
                    aboard,prs,padv
                    across,prs
                    anti,prs,adj
                    as,prs,sor
                    below,prs,adv
                    but,prs,cor
                    considering,prs,ving
                    despite,prs
                    down,prs,padj,padv,vinf
                    excepting,prs,ving
                    excluding,prs,ving
                    following,prs,ving,padj
                    inside,prs,padj,padv
                    into,prs
                    like,prs,vinf,padv,padj
                    minus,prs,padj
                    onto,prs
                    outside,prs,padj,padv
                    past,prs,n,padj
                    per,prs
                    plus,prs,padj
                    regarding,prs,ving
                    round,prs,n,vinf,padj
                    since,prs,sor
                    hence,prs,sor
                    therefore,prs,sor 
                    than,sor
                    through,prs
                    toward,prs
                    under,prs,padj,padv
                    unlike,prs,padj
                    until,prs,sor
                    up,prs,padj,padv
                    versus,prs
                    via,prs*/
                    #endregion
            }
           
            return nounframelist;
            //return nframe;
        }
    
      
        

        public List <NounFrame> GET_PRPS(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(node,parseTree);
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP"))
            {
                nounframelist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree, verbFrame));
             ///// nounframelist.Add(nframe);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS1"))
            {
                nounframelist.AddRange( GET_PRPS1((ParseNode)node.Children[0], parseTree,verbFrame));
                //nframe.Equals(nounframelist);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS2"))
            {
                nounframelist.AddRange(GET_PRPS2((ParseNode)node.Children[0], parseTree));
                //nframe.Equals(nounframelist);
            }
            //if (((ParseNode)node.Children[0]).Goal == "PRP")
            //{
            //   nounframelist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree));
            //}
            //else if (((ParseNode)node.Children[0]).Goal == "PRPS1")
            //{
            //   nounframelist.AddRange(GET_PRPS1((ParseNode)node.Children[0], parseTree));
            //}
            //else if (((ParseNode)node.Children[0]).Goal == "PRPS2")
            //{
            //    nounframelist.AddRange(GET_PRPS2((ParseNode)node.Children[0], parseTree));
            //}

            //return nounframelist;
            return nounframelist;

        }

        public List<NounFrame> GET_PRP(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)//@@ PRS terminal as preposition @@
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(node);
            List<NounFrame> modaf_eleh = new List<NounFrame>();
            string Preposition = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);
                
                //((ParseNode)node.Children[0]).Text.ToString();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRS", "OBJ"))
            {
                //NounFrame n = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(n);
                //AddNounFrame(n);

                nounframelist.AddRange(GET_OBJ((ParseNode)node.Children[1], parseTree, verbFrame));
                
                //get the nounframe concept
                foreach (NounFrame nf in nounframelist)
                {
                    ParseNode pn = nf.ParseNode;
                    string Sense = pn.Sense;
                    int SenseNo = pn.SenseNo;
                    string Word = nf.Text;
                    SenseNo++;
                    MyWordInfo myi = new MyWordInfo(Word, Wnlib.PartsOfSpeech.Noun);
                    myi.Sense = SenseNo;
                    //the pos is obtained from Wordology.....right?
                    nf.Concept = MindMapMapper.GetConcept(myi, this._ontology);
                }

                if (verbFrame != null)
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (verbFrame.CaseRoles.ContainsKey(CaseRole.Cotheme))
                        {
                            verbFrame.CaseRoles[CaseRole.Cotheme].Remove(nf);
                            verbFrame.CaseRoles.Remove(CaseRole.Cotheme);
                        }
                        if (verbFrame.CaseRoles.ContainsKey(CaseRole.Theme))
                        {
                            verbFrame.CaseRoles[CaseRole.Theme].Remove(nf);
                            verbFrame.CaseRoles.Remove(CaseRole.Theme);
                        }
                    }
                }
              //  modaf_eleh.AddRange(nounframelist);
                //obtain nounframes then check
                #region Prepositions
                if (Preposition == "IN")
                {
                    //maybe location---in the house
                    //time---in 1990
                    //in the history???---time???????
                    //check in file

                    foreach (NounFrame nf in nounframelist)
                    {

                        if (nf.Concept != null)
                        {

                            if (nf.Concept.NonVisualProperties.ContainsKey("ISTIME")&& nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                            {

                                verbFrame.AddCaseRole(CaseRole.time, nf);

                            }
                            else if (nf.Concept.NonVisualProperties.ContainsKey("ISLOCATION")&&nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {//in USA

                                verbFrame.AddCaseRole(CaseRole.location, nf);

                            }//in ppl
                            else if (nf.Concept.NonVisualProperties.ContainsKey("ISPERSON")&&nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.location, nf);
                            }
                            else 
                            {
                                verbFrame.AddCaseRole(CaseRole.unknown, nf);//default location
                            }

                        }
                        else verbFrame.AddCaseRole(CaseRole.unknown, nf);
                    }

                }
                else if (Preposition == "OF")
                {
                    //Cairo is the capital of Egypt
                   //history of literature....literature adjective of history
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                //3ayza a-return egypt fa tetrabbat b capital
                                verbFrame.AddCaseRole(CaseRole.location, nf);
                            }
                            else//not location
                            {
                                verbFrame.AddCaseRole(CaseRole.unknown, nf);
                                return nounframelist;
                            }

                        }
                        else//null///////check
                        {
                            //return literature which is attached to history fo2
                            verbFrame.AddCaseRole(CaseRole.unknown, nf);
                            return nounframelist;
                            // nf.Addedto = modaf_eleh;
                        }
                        
                    }
                    //return nounframelist;
                    
                    
                    //return the adjective as nounframelist then cast into parsenodes afterwards
                    
                }

                else if (Preposition == "BEFORE")
                {
                    //before the queen?
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties.ContainsKey("ISLOCATION") && nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.location, nf);
                            }
                            else
                            {
                                verbFrame.AddCaseRole(CaseRole.unknown, nf);
                            }
                        }
                        else
                        {
                            verbFrame.AddCaseRole(CaseRole.unknown, nf);
                        }
                    }

                    //return nounframelist;
                   
                }
                else if (Preposition == "ALONG")
                {
 
                }
                else if (Preposition == "AS")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        verbFrame.AddCaseRole(CaseRole.unknown, nf);
                    }
                }
                else if (Preposition == "DURING")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        verbFrame.AddTemporalRelation_n(TemporalRelationType.Concurrent, nf);
                    }
                }
                else if (Preposition == "UNTIL")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        verbFrame.AddTemporalRelation_n(TemporalRelationType.Until, nf);
                    }
                }
                else if (Preposition == "ACCORDING_TO")
                {
                }
                else if (Preposition == "BY_MEANS_OF")
                {
                }
                else if (Preposition == "ABOUT")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        verbFrame.AddCaseRole(CaseRole.About, nf);
                    }
                }
                else if (Preposition == "AT")
                {

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.location, nf);
                            }
                            else if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.time, nf);
                            }
                            else
                            {
                                verbFrame.AddCaseRole(CaseRole.unknown, nf);
                            }
                        }
                    }
                }
                else if (Preposition == "NEXT")
                {
                }
                else if (Preposition == "WITH")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties["ISOBJECT"][0] == "true")
                            {//with disenfectant
                                verbFrame.AddCaseRole(CaseRole.Instrument, nf);
                            }
                            //else if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                            //{


                               //}//person
                            else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.Accompanier, nf);
                            }
                            else verbFrame.AddCaseRole(CaseRole.Accompanier, nf);
                        }

                        else
                            verbFrame.AddCaseRole(CaseRole.Accompanier, nf);
                    }

                }
                else if (Preposition == "FOR")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.location, nf);

                            }
                            else if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.time, nf);

                            }//i did it for ali......I played for mary...mary benificiary           i bought food for my dog
                            else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true" || nf.Concept.NonVisualProperties["ISOBJECT"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.Beneficiary, nf);

                            }
                            //else if (nf.Concept.NonVisualProperties["ISOBJECT"][0] == "true")
                            //{
                            //    verbFrame.AddCaseRole(CaseRole.purpose, nf);

                            //}

                            else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.purpose, nf);

                            }
                            else verbFrame.AddCaseRole(CaseRole.unknown, nf);
                        }
                        else { verbFrame.AddCaseRole(CaseRole.unknown, nf); }

                    }
                   
                   
                }
                else if (Preposition == "AGAINST")
                {
                }
                else if (Preposition == "UNTO")
                {
                }
                else if (Preposition == "AS_FAR_AS")
                {
                }
                else if (Preposition == "ON")
                {
                }
                else if (Preposition == "BEHIND")
                {
                }
                else if (Preposition == "WITHOUT")
                {
                }
                else if (Preposition == "BY")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.location, nf);
                            }
                            if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.time, nf);
                            }
                            else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                            {
                                if (verbFrame.Passive)
                                    verbFrame.AddCaseRole(CaseRole.Agent, nf);
                                else
                                    verbFrame.AddCaseRole(CaseRole.location, nf);

                            }
                            else if (nf.Concept.NonVisualProperties["ISOBJECT"][0] == "true" || nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.Means, nf);
                            }

                            else
                            {
                                verbFrame.AddCaseRole(CaseRole.unknown, nf);
                            }
                        }
                        else { verbFrame.AddCaseRole(CaseRole.Means, nf); }
                    }
                    //by ahmad fa a7mad agent
                    //by the river....location
                    //by 10pm....time
                }
                else if (Preposition == "UNDER")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            verbFrame.AddCaseRole(CaseRole.Under, nf);
                        }

                        else
                        {
                            verbFrame.AddCaseRole(CaseRole.Under, nf);
                        }
                    }

                }
                else if (Preposition == "NEAR_TO")
                {
                }
                else if (Preposition == "TO")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.location, nf);
                            }
                            else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.Action, nf);
                            }
                            else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.location, nf);
                            }
                            else
                            {
                                //testing purposes
                                verbFrame.AddCaseRole(CaseRole.unknown, nf);
                            }

                        }
                        else { verbFrame.AddCaseRole(CaseRole.unknown, nf); }
                    }
                }
                else if (Preposition == "IN_ORDER_TO")
                {
                }
                else if (Preposition == "UPON")
                {
                }
                else if (Preposition == "FROM")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true" || nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.Source, nf);
                                //verbFrame.AddCaseRole(CaseRole.location, nf);/////////////////////
                            }
                                //from reproducing
                            //else if(nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                            //{

                            //}
                            else//from agricultural products
                                verbFrame.AddCaseRole(CaseRole.unknown, nf);
                        }
                        else//from agricultural products
                            verbFrame.AddCaseRole(CaseRole.unknown, nf);
                    }


                    ////////foreach (NounFrame nf in nounframelist)
                    ////////{
                    ////////    if (nf.Concept != null)
                    ////////    {
                    ////////        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                    ////////        {
                    ////////            verbFrame.AddCaseRole(CaseRole.Source, nf);//OR
                    ////////            //verbFrame.AddCaseRole(CaseRole.location, nf);
                    ////////        }
                    ////////        else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                    ////////        {
                    ////////            verbFrame.AddCaseRole(CaseRole.Action, nf);
                    ////////        }
                    ////////        else if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                    ////////        {
                    ////////            verbFrame.AddCaseRole(CaseRole.location, nf);
                    ////////        }
                    ////////        else
                    ////////        {
                    ////////            //testing purposes
                    ////////            verbFrame.AddCaseRole(CaseRole.unknown, nf);
                    ////////        }
                    ////////    }
                    ////////}

                }
                else if (Preposition == "LIKE")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        //if (Is_Location(nf))
                        //{
                        //    verbFrame.AddCaseRole(CaseRole.Source, nf);
                        //    verbFrame.AddCaseRole(CaseRole.location, nf);
                        //}
                        //else//from agricultural products
                            //verbFrame.AddCaseRole(CaseRole.Means, nf);
                        verbFrame.AddCaseRole(CaseRole.example, nf);
                    }



                }

                else if (Preposition == "CONCERNING")
                {
                }
                else if (Preposition == "TOUCHING")
                {
                }
                else if (Preposition == "DURING")
                {

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.time, nf);
                        }
                    }
                }
                else if (Preposition == "BECAUSE_OF")
                {
                }
                else if (Preposition == "DUE_TO")
                {
                }
                else if (Preposition == "AFTER")
                {

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {//wash after coughing...wash hands after coughin and sneezing...reason?
                            if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.time, nf);
                            }
                            else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                            {//reasonn for washing
                                verbFrame.AddCaseRole(CaseRole.reason, nf);
 
                            }
                            else
                                verbFrame.AddCaseRole(CaseRole.unknown, nf);
                        }
                        else verbFrame.AddCaseRole(CaseRole.time, nf);

                    }
                }
                else if (Preposition == "AS_A_RESULT_OF")
                {
                }
                else if (Preposition == "BESIDE")
                {
                }
                else if (Preposition == "OUT_OF")
                {
                }
                else if (Preposition == "BEYOND")
                {
                }
                else if (Preposition == "WITHIN")
                {   //2minutes

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.time, nf);
                        }//within the car.......object or location???
                        else if (nf.Concept.NonVisualProperties["ISOBJECT"][0] == "true")//if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.location, nf);
                        }
                    }

                }
                else if (Preposition == "BENEATH")
                {

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.location, nf);
                        }
                    }
                }
                else if (Preposition == "BETWIXT")
                {

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.location, nf);

                        }
                    }
                }
                else if (Preposition == "BETWEEN")
                {

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {
                            if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.location, nf);

                            }
                            else if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.time, nf);

                            }
                        }
                        else
                        {
                            if (!verbFrame.VerbName.Contains("BETWEEN"))
                            {
                                verbFrame.VerbName = verbFrame.VerbName + "_BETWEEN";
                            }
                            verbFrame.AddCaseRole(CaseRole.Theme, nf);

 
                        }
                    
                    }
                }
                else if (Preposition == "AMONG" || Preposition == "AMID" || Preposition == "AMIDST")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept != null)
                        {//spreads among ppl
                            if (nf.Concept.NonVisualProperties["ISPERSON"][0] == "true")
                            {
                                verbFrame.AddCaseRole(CaseRole.Path, nf);

                            }
                            else
                            {
                                //testing purposes
                                verbFrame.AddCaseRole(CaseRole.among, nf);
                            }
                        }
                        else
                        { 
                            verbFrame.AddCaseRole(CaseRole.among, nf);
                        
                        }

                    }
                }
                else if (Preposition == "IN_THE_MIDST" || Preposition == "IN_THE_MIDST_OF")
                {
                }
                else if (Preposition == "NEAR")
                {
                }
                else if (Preposition == "ABOVE")
                {
                }
                else if (Preposition == "EXCEPT")
                {
                }
                else if (Preposition == "IN_ADDITION_TO")
                {
                }
                else if (Preposition == "BESIDES")
                {
                }
                else if (Preposition == "ON_ACCOUNT_OF")
                {
                }
                else if (Preposition == "INSTEAD_OF")
                {
                }
                else if (Preposition == "THROUGHOUT")
                {
                }
                else if (Preposition == "THROUGH")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.location, nf);
                        }
                        else if (nf.Concept.NonVisualProperties["ISTIME"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.time, nf);
                        }
                        else if (nf.Concept.NonVisualProperties["ISACTION"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.Means, nf);
                        }
                        else
                        {
                            verbFrame.AddCaseRole(CaseRole.unknown, nf);
                        }
                    }
                }

                else if (Preposition == "OVER")
                {
                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.location, nf);
                        }
                        else
                        {
                            verbFrame.AddCaseRole(CaseRole.unknown, nf);
                        }
                    }
                }
                else if (Preposition == "FROM_WITHIN")
                {
                }
                else if (Preposition == "OFF")
                {
                }
                else if (Preposition == "OPPOSITE")
                {
                }
                else if (Preposition == "TOWARDS")
                {
                }
                else if (Preposition == "IN_REPLY_TO")
                {
                }
                else if (Preposition == "UP_TO")
                {
                }
                else if (Preposition == "DURING")
                {
                }
                else if (Preposition == "INTO")
                {
                    //into the house////location or object?

                    foreach (NounFrame nf in nounframelist)
                    {
                        if (nf.Concept.NonVisualProperties["ISLOCATION"][0] == "true")
                        {
                            verbFrame.AddCaseRole(CaseRole.location, nf);

                        }
                        else
                        {
                            verbFrame.AddCaseRole(CaseRole.unknown, nf);
                        }
                    }
                    
                    
                }

                /*
                 * underneath,prs
                    outside_of,prs
                    opposite_to,prs
                    around,prs,padv
                    aboard,prs,padv
                    across,prs
                    anti,prs,adj
                    as,prs,sor
                    below,prs,adv
                    but,prs,cor
                    considering,prs,ving
                    despite,prs
                    down,prs,padj,padv,vinf
                    excepting,prs,ving
                    excluding,prs,ving
                    following,prs,ving,padj
                    inside,prs,padj,padv
                    into,prs
                    like,prs,vinf,padv,padj
                    minus,prs,padj
                    onto,prs
                    outside,prs,padj,padv
                    past,prs,n,padj
                    per,prs
                    plus,prs,padj
                    regarding,prs,ving
                    round,prs,n,vinf,padj
                    since,prs,sor
                    hence,prs,sor
                    therefore,prs,sor 
                    than,sor
                    through,prs
                    toward,prs
                    under,prs,padj,padv
                    unlike,prs,padj
                    until,prs,sor
                    up,prs,padj,padv
                    versus,prs
                    via,prs*/
#endregion
            }
           
            return nounframelist;
            //return nframe;
        }

        private bool Is_Location(NounFrame nf)
        {
           // throw new NotImplementedException();
            return false;
        }

        private bool Is_Time(NounFrame nf)
        {
            //throw new NotImplementedException();
            return false;
        }

        public List<NounFrame> GET_NC(ParseNode node,ParseTree parseTree, VerbFrame vf)//@@na2es el SOR@@
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            List<VerbFrame> vflist = new List<VerbFrame>();
            
                            //NC=SOR+DS
                            //NC=RPN+PRD
            //cleansers for hands can  be used when soap and water are unavailable
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SOR", "DS"))
            {
                string SOR = SentenceParser.GetWordString(parseTree, (ParseNode)node.Children[0]);

               vflist.AddRange(Get_DS((ParseNode)node.Children[1], parseTree));
               if (vflist.Count ==1)
               {
                   string verb = vflist[0].VerbName;
                   if (verb == "IS" || verb == "ARE" || verb == "WERE")
                   {
                       nounframelist.AddRange(Get_DS_noun((ParseNode)node.Children[1], parseTree));

                       if (nounframelist.Count != 0)
                       {

                           //foreach (NounFrame nf in nounframelist)
                           //{
                           //    if (vflist[0].CaseRoles[CaseRole.Theme].Contains(nf))
                           //    {
                           //        vflist[0].CaseRoles[CaseRole.Theme].Remove(nf);
                           //    }
                           //    //vflist[0].CaseRoles.ContainsKey(CaseRole.Theme);

                           //}
                           //add el nouns bel verb elli gayli 3ala 7asab el sor
                           if (SOR == "WHEN")
                           {
                               foreach (NounFrame nf in nounframelist)
                               {
                                   vf.AddDomainRelation_n(DomainRelationType.Condition, nf);

                               }
                           }

                       }

                   }
                   else
                   {
                       if (SOR == "THAT")
                       {
                           vf.AddDomainRelation(DomainRelationType.Completion, vflist[0]);
                       }
                       else if (SOR == "HOW")
                       {
                           vf.AddDomainRelation(DomainRelationType.How, vflist[0]);
                       }
                   }

               }
               else 
               {
                   //if there's verb returning then add relation between verbs
               }


                //if no verbs...arbot bel nouns b relation...law feeh verb...arbot beeh howa...ya3ni hanadi marra DS verbs then DS nouns
              // vflist[0].AddTemporalRelation_n(TemporalRelationType.Concurrent, soap / water);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "RPN", "PRD"))
            {
                nounframelist.AddRange(GET_RPN((ParseNode)node.Children[0], parseTree));
               // nounframelist.AddRange(GET_PRD((ParseNode)node.Children[1], parseTree));
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "DS")
            //    {
            //   vflist.AddRange(Get_DS((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "RPN")
            //    {
            //      nounframelist.AddRange(GET_RPN((ParseNode)node.Children[i],parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "PRD")
            //    {
            //        //vflist.AddRange(GET_PRD((ParseNode)node.Children[i], parseTree));
            //    }

            //    i++;
            //}
            return nounframelist;
         
        }

        public List<NounFrame> GET_RPN(ParseNode node,ParseTree parseTree)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
                            //RPN=SRPN
                            //RPN=PSRPN+NN
                            //RPN=PRS+SRPN
                            //RPN=PRS+PSRPN+NN
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SRPN"))
            {
                // GET_SRPN((ParseNode)node.Children[0]);
                //terminal
                NounFrame nf = new NounFrame((ParseNode)node.Children[0],parseTree);
                nounframelist.Add(nf);
                
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PSRPN", "NN"))
            {
                //////GET_PSRPN((ParseNode)node.Children[0]);
                //////terminal
                
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRS", "SRPN"))
            {
                //nounframelist.AddRange(GET_PRS((ParseNode)node.Children[0], parseTree));
                //PRS=terminal
                //SRPN=terminal
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRS", "PSRPN", "NN"))
            {
                //////nounframelist.AddRange(GET_PRS((ParseNode)node.Children[0], parseTree));
                //////terminal
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
            }

            return nounframelist;
        }


        public List<NounFrame> GET_PRPS1(ParseNode node, ParseTree parseTree,VerbFrame verbframe)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(node, parseTree);
            NounFrame nframe1 = new NounFrame(node, parseTree);

                        //PRPS1=PRP+CMA+PRPS1
                        //PRPS1=PRPS2+CMA+PRPS1
                        //PRPS1=PRP+COR+PRP
                        //PRPS1=PRP+COR+PRPS2
                        //PRPS1=PRPS2+COR+PRPS2
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP", "CMA", "PRPS1"))
            {
               nounframelist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree,verbframe));
                //nounframelist.Add(nframe);

                nounframelist.AddRange(GET_PRPS1((ParseNode)node.Children[2], parseTree,verbframe));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS2", "CMA", "PRPS1"))
            {
                nounframelist.AddRange(GET_PRPS2((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_PRPS1((ParseNode)node.Children[2], parseTree,verbframe));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"PRP","COR"," PRP"))
            {
               nounframelist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree,verbframe));
               ///// nounframelist.Add(nframe);
                nounframelist.AddRange(GET_PRP((ParseNode)node.Children[2], parseTree,verbframe));
                /////nounframelist.Add(nframe1);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP","COR","PRPS2"))
            {
                nounframelist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree,verbframe));
                /////nounframelist.Add(nframe);
                nounframelist.AddRange(GET_PRPS2((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS2","COR","PRPS2"))
            {
                nounframelist.AddRange(GET_PRPS2((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_PRPS2((ParseNode)node.Children[2], parseTree));
            }


            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "PRP")
            //    {
            //       //nounframelist.AddRange(GET_PRP((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "PRPS1")
            //    {
            //      nounframelist.AddRange(GET_PRPS1((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "PRPS2")
            //    {
            //      nounframelist.AddRange(GET_PRPS2((ParseNode)node.Children[i], parseTree));
            //    }

            //    i++;
            //}
            return nounframelist;
        }

        public List<NounFrame> GET_PRPS2(ParseNode node, ParseTree parseTree)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();

                        //PRPS2=ETH+PRPS3+OR+PRPS3
                        //PRPS2=NTH+PRPS3+NOR+PRPS3
                        //PRPS2=NOTN+PRPS3+BUTL+PRPS3
                        //PRPS2=BTH+PRPS3+AND+PRPS3
                        //PRPS2=WTH+PRPS3+OR+PRPS3

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "PRPS3", "OR", "PRPS3"))
            {
                //NounFrame nf = new NounFrame((ParseNode)node.Children[0], parseTree);
                //AddNounFrame(nf);
                //nounframelist.Add(nf);
                //NounFrame nf1 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //AddNounFrame(nf1);
                //nounframelist.Add(nf1);

                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH","PRPS3","NOR","PRPS3"))
            {
                //NounFrame nf = new NounFrame((ParseNode)node.Children[0], parseTree);
                //AddNounFrame(nf);
                //nounframelist.Add(nf);
                //NounFrame nf1 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //AddNounFrame(nf1);
                //nounframelist.Add(nf1);

                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN","PRPS3","BUTL","PRPS3"))
            {
                //NounFrame nf = new NounFrame((ParseNode)node.Children[0], parseTree);
                //AddNounFrame(nf);
                //nounframelist.Add(nf);
                //NounFrame nf1 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //AddNounFrame(nf1);
                //nounframelist.Add(nf1);

                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[3], parseTree));
            }
            //BTH+PRPS3+AND+PRPS3
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH","PRPS3","AND","PRPS3"))
            {
                //NounFrame nf = new NounFrame((ParseNode)node.Children[0], parseTree);
                //AddNounFrame(nf);
                //nounframelist.Add(nf);
                //NounFrame nf1 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //AddNounFrame(nf1);
                //nounframelist.Add(nf1);

                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[3], parseTree));
            }
            //WTH+PRPS3+OR+PRPS3
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH","PRPS3","OR","PRPS3"))
            {
                //NounFrame nf = new NounFrame((ParseNode)node.Children[0], parseTree);
                //AddNounFrame(nf);
                //nounframelist.Add(nf);
                //NounFrame nf1 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //AddNounFrame(nf1);
                //nounframelist.Add(nf1);

                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[3], parseTree));
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "PRPS3")
            //    {
            //       nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[i], parseTree));
            //    }

            //    //else if (((ParseNode)n).Goal == "")
            //    //{
            //    //    GET_((ParseNode)node.Children[i]);
            //    //}

            //    //else if (((ParseNode)n).Goal == "")
            //    //{
            //    //    GET_((ParseNode)node.Children[i]);
            //    //}

            //    i++;
            //}
            return nounframelist;
        }


        public List<NounFrame> GET_PRPS3(ParseNode node, ParseTree parseTree)
        {
                                //PRPS3=PRP
                                //PRPS3=PRPS4
                                //PRPS3=PRP+PRPS3
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(node, parseTree);
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP"))
            {
                nounframelist.AddRange (GET_PRP((ParseNode)node.Children[0], parseTree,null));
                ///////nounframelist.Add(nframe);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS4"))
            {
                nounframelist.AddRange(GET_PRPS4((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"PRP","PRPS3"))
            {
                nounframelist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree,null));
                //////nounframelist.Add(nframe);
                nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[1], parseTree));
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "PRP")
            //    {
            //     // nounframelist.AddRange(GET_PRP((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "PRPS4")
            //    {
            //       nounframelist.AddRange(GET_PRPS4((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "PRPS3")
            //    {
            //       nounframelist.AddRange(GET_PRPS3((ParseNode)node.Children[i], parseTree));
            //    }

            //    i++;
            //}
            return nounframelist;
        }


        public List<NounFrame> GET_PRPS4(ParseNode node, ParseTree parseTree)//@@Bardo COR msh 3arfa and CMA
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(node, parseTree);
            
                                //PRPS4=PRP+CMA+PRPS4
                                //PRPS4=PRP+COR+PRP
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP", "CMA", "PRPS4"))
            {
                nounframelist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree,null));
               ////// nounframelist.Add(nframe);
                nounframelist.AddRange(GET_PRPS4((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRP", "COR", "PRP"))
            {
                nounframelist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree,null));
                
                //////nounframelist.Add(nframe);

                nounframelist.AddRange(GET_PRP((ParseNode)node.Children[2], parseTree,null));

                //////nounframelist.Add(nframe);
                
            } 
           
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "PRP")
            //    {
            //      // nounframelist.AddRange(GET_PRP((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "PRPS4")
            //    {
            //       nounframelist.AddRange(GET_PRPS4((ParseNode)node.Children[i], parseTree));
            //    }

            //    i++;
            //}
            return nounframelist;
        }




        public List<NounFrame> GET_NP(ParseNode node, ParseTree parseTree,List<NounFrame>subjects)
        {
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            List<NounFrame> nounframelist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NN"))
            {
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[0], parseTree));
                //subjects.AddRange(GET_NN((ParseNode)node.Children[0], parseTree));

            }
            //NPP

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NPP"))
            {
                //List<NounFrame> nounframelist=new List<NounFrame>();
                //  nounframelist.AddRange(GET_NPP((ParseNode)node.Children[0], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode != null)
                {
                    if (node.Text == "THEY")
                    {
                        string referedTxt = ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Text;
                        string[] refered = referedTxt.Split(new string[] { "_AND_" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < refered.Length; i++)
                        {
                            foreach (NounFrame nf in Nounframes)
                            {

                                if (nf.Text.ToUpper() == refered[i].ToUpper())
                                {
                                    // return nounframelist;
                                    // nfs.Add(n1);
                                    // Nounframes.(nf);
                                    nounframelist.Add(nf);

                                    break;
                                }
                            }

                        }
                    }
                    else if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN" || ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NNC")
                    {
                        //foreach (ParseNode n in node.ReferedAnaphoraNode.Children)
                        //
                        //ParseNode newnode = new ParseNode();
                        
                            //foreach (ParseNode n1 in ((ParseNode)(node.Children[0])).ReferedAnaphoraNode.Children)
                            //{
                            //   nounframelist= GET_PPN(n1,parseTree);
                            //   foreach (NounFrame nf in nounframelist)
                            //   {
                      
                        
                            foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                            {
                                foreach (NounFrame nf in Nounframes)
                                {

                                    if (nf.Text == n.Text)
                                    {
                                        // return nounframelist;
                                        // nfs.Add(n1);
                                        // Nounframes.(nf);
                                        nounframelist.Add(nf);

                                        break;
                                    }
                                }
                            }
                        
                    }
                }
                return nounframelist;
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "AVJ","NN"))
            {
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }

                
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "NN"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                //nounframelist.AddRange(GET_ARC((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "IDFP"))
            {
                NounFrame nf = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nf);
                AddNounFrame(nf);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "DEMP"))
            {
                NounFrame nf = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nf);
                AddNounFrame(nf);
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "NN","FAVJ"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[2], parseTree,verbFrame));

                List<NounFrame> temp = new List<NounFrame>();

                //of literature gebtaha hena...bas already rabattah bel history as a noun....fa no need to return
                //coz it will then be added like history b case role lel verb which i don't want
                temp.AddRange(GET_FAVJ_noun((ParseNode)node.Children[2], parseTree, nounframelist));
                //nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[2], parseTree,subjects));
            }
            //NP=NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NN", "FAVJ"))
            {
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[0], parseTree));
                //nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[1], parseTree, verbFrame));
                //nounframelist.AddRange(GET_FAVJ_noun((ParseNode)node.Children[1], parseTree, null));
                List<NounFrame> temp = new List<NounFrame>();
                temp.AddRange(GET_FAVJ_noun((ParseNode)node.Children[1], parseTree, nounframelist));

            }
            //NP=AVJ+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "AVJ", "NN", "FAVJ"))
            {
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[2], parseTree, verbFrame));
                //nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[2], parseTree, null));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[2], parseTree, subjects));
            }
            //NP=ARC+AVJ+NN
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "NN"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                // nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
            }
            //NP=ARC+AVJ+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "NN", "FAVJ"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                //nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[3], parseTree, verbFrame));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[3], parseTree, subjects));

            }

                        ////NP=PPJ+NN
            ////NP=PPJ+AVJ+NN
            ////NP=PPJ+AVJ+NN+FAVJ
            ////NP=PPJ+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "NN"))
            {
                // nounframelist.AddRange(GET_PPJ((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode != null)
                {
                    if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN"||((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NNC")
                    {

                        foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                        {
                            foreach (NounFrame nf in Nounframes)
                            {
                                string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                                ParseNode pn1 = nf.ParseNode;

                                if (nf.Text == n.Text)
                                {
                                    // return nounframelist;
                                    // nfs.Add(n1);
                                    //                    // Nounframes.(nf);
                                    // nounframelist.Add(nf);
                                    //if(nf.Ownerof.ContainsValue(
                                    nf.AddCaseRolenouns(CaseRole.OwnerOf, nounframelist[0]);
                                    //                    //List<NounFrame> nn =new List<NounFrame>();
                                    //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                    //                    //nf.Ppj.AddRange(nn);
                                    break;
                                }
                            }
                        }
                    }
                }

                //nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //return nounframelist;

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "AVJ", "NN"))
            {
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN")
                {

                    foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                    {
                        foreach (NounFrame nf in Nounframes)
                        {
                            string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                            ParseNode pn1 = nf.ParseNode;

                            if (nf.Text == n.Text)
                            {
                                // return nounframelist;
                                // nfs.Add(n1);
                                //                    // Nounframes.(nf);
                                // nounframelist.Add(nf);
                                //if(nf.Ownerof.ContainsValue(
                                nf.AddCaseRolenouns(CaseRole.OwnerOf, nounframelist[0]);
                                //                    //List<NounFrame> nn =new List<NounFrame>();
                                //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                //                    //nf.Ppj.AddRange(nn);
                                break;
                            }
                        }
                    }
                }


                //nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //return nounframelist;
                //nounframelist.AddRange(GET_PPJ((ParseNode)node.Children[0], parseTree));
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "AVJ", "NN", "FAVJ"))
            {
                // nounframelist.AddRange(GET_PPJ((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN")
                {

                    foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                    {
                        foreach (NounFrame nf in Nounframes)
                        {
                            string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                            ParseNode pn1 = nf.ParseNode;

                            if (nf.Text == n.Text)
                            {
                                // return nounframelist;
                                // nfs.Add(n1);
                                //                    // Nounframes.(nf);
                                // nounframelist.Add(nf);
                                //if(nf.Ownerof.ContainsValue(
                                nf.AddCaseRolenouns(CaseRole.OwnerOf, nounframelist[0]);
                                //                    //List<NounFrame> nn =new List<NounFrame>();
                                //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                //                    //nf.Ppj.AddRange(nn);
                                break;
                            }
                        }
                    }
                }


                //nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));

                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                //
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[3], parseTree, subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "NN", "FAVJ"))
            {
                // nounframelist.AddRange(GET_PPJ((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN")
                {

                    foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                    {
                        foreach (NounFrame nf in Nounframes)
                        {
                            string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                            ParseNode pn1 = nf.ParseNode;

                            if (nf.Text == n.Text)
                            {
                                // return nounframelist;
                                // nfs.Add(n1);
                                //                    // Nounframes.(nf);
                                // nounframelist.Add(nf);
                                //if(nf.Ownerof.ContainsValue(
                                nf.AddCaseRolenouns(CaseRole.OwnerOf, nounframelist[0]);
                                //                    //List<NounFrame> nn =new List<NounFrame>();
                                //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                //                    //nf.Ppj.AddRange(nn);
                                break;
                            }
                        }
                    }
                }


                //nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //return nounframelist;
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[2], parseTree, subjects));
            }


                    //NP=CNP+NN
            //NP=CNP+AVJ+NN
            //NP=CNP+AVJ+NN+FAVJ
            //NP=CNP+NN+FAVJ
            //NP=ARC+CNP+NN

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CNP", "NN"))
            {
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nframe);
                AddNounFrame(nframe);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CNP", "AVJ", "NN"))
            {
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nframe);
                AddNounFrame(nframe);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CNP", "AVJ", "NN", "FAVJ"))
            {
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nframe);
                AddNounFrame(nframe);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[3], parseTree, subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CNP", "NN", "FAVJ"))
            {
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nframe);
                AddNounFrame(nframe);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[2], parseTree, subjects));
            }
            ////NP=ARC+CNP+NN
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "CNP", "NN"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, " ARC", "CNP", " AVJ ", " NN"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[2], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[3], parseTree));

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, " ARC", "CNP", " AVJ ", " NN", "FAVJ"))
            {
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nframe);
                AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[2], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[3], parseTree));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[4], parseTree, subjects));
            }
            //NP=ARC+CNP+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "CNP", " NN", "FAVJ"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);

                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[3], parseTree, subjects));

            }
            //NP=ARC+OF+NN
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "OF", "NN"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));

            }
            //NP=ARC+OF+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "OF", "NN", "FAVJ"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[3], parseTree, subjects));
            }
            //NP=ARC+OF+ARC+NN
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "OF", "ARC", "NN"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                //NounFrame nframe2 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //nounframelist.Add(nframe2);
                //AddNounFrame(nframe2);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[3], parseTree));
            }
            //NP=ARC+OF+ARC+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "OF", "ARC", "NN", "FAVJ"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                //NounFrame nframe2 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //nounframelist.Add(nframe2);
                //AddNounFrame(nframe2);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[3], parseTree));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[4], parseTree, subjects));
            }
            //NP=NP+RXPN
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NP", "RXPN"))
            {
                nounframelist.AddRange(GET_NP((ParseNode)node.Children[0], parseTree, subjects));
                NounFrame nf = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nf);
                AddNounFrame(nf);
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NPP", "RXPN"))
            {
                //nounframelist.AddRange(GET_NPP((ParseNode)node.Children[0], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode != null)
                {
                    if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN")
                    {
                        foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                        {
                            foreach (NounFrame nf in Nounframes)
                            {
                                string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                                ParseNode pn1 = nf.ParseNode;

                                if (nf.Text == n.Text)
                                {
                                    nounframelist.Add(nf);
                                    break;
                                }
                            }
                        }
                    }
                }
                NounFrame nf1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nf1);
                AddNounFrame(nf1);
            }


            //NP=ALL+ARC+NN
            //NP=ALL+ARC+NN+FAVJ
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ALL", "ARC", "NN"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ALL", "ARC", "NN", "FAVJ"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[3], parseTree, subjects));
            }
            //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ", "NN"))
            //{
            //    //NounFrame nf = new NounFrame((ParseNode)node.Children[0]);
            //    //nounframelist.Add(nf);
            //    //AddNounFrame(nf);
            //   // nounframelist.AddRange(GET_PPJ((ParseNode)node.Children[1], parseTree));
            //    nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
            //}

            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "NN")
            //    {
            //       nounframelist.AddRange(GET_NN((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "IDFP")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_IDFP((ParseNode)node.Children[i], parseTree));
            //    }


            //    else if (((ParseNode)n).Goal == "DEMP")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_DEMP((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "NPP")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_NPP((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "RXPN")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_RXPN((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "AVJ")
            //    {
            //        //nounframelist.AddRange(GET_AVJ((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "FAVJ")
            //    {
            //        //nounframelist.AddRange(GET_FAVJ((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "ARC")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_ARC((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "PPJ")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_PPJ((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "CNP")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_CNP((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "OF")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_OF((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "ALL")//@@TERMINAL
            //    {
            //        //nounframelist.AddRange(GET_ALL((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "NP")
            //    {
            //        //nounframelist.AddRange(GET_NP((ParseNode)node.Children[i], parseTree));
            //    }
            //    //throw new NotImplementedException();
            //    //where is the LIST????!!!!!!
            //    i++;
            //}
            return nounframelist;
        }

        //private List<NounFrame> GET_PPJ(ParseNode node, ParseTree parseTree)
        //{
        //   // throw new NotImplementedException();
        //    List<NounFrame> nounframelist=new List<NounFrame>();
        //    if (((ParseNode)node).ReferedAnaphoraNode != null)
        //    {
        //        if (((ParseNode)node).ReferedAnaphoraNode.Goal == "NN")
        //        {
                   
        //            foreach (ParseNode n in ((ParseNode)node).ReferedAnaphoraNode.Children)
        //            {
        //                foreach (NounFrame nf in Nounframes)
        //                {
        //                    string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
        //                    ParseNode pn1 = nf.ParseNode;

        //                    if (nf.Text == n.Text)
        //                    {
        //                        // return nounframelist;
        //                        // nfs.Add(n1);
        //                        // Nounframes.(nf);
        //                        nounframelist.Add(nf);

        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return nounframelist;
        //}

        //public NounFrame GET_ARC(ParseNode node, ParseTree parseTree)
        //{
        //    NounFrame nframe = new NounFrame(node,parseTree);
        //    AddNounFrame(nframe);
        //    return nframe;
        //}
        public List<NounFrame> GET_VING(ParseNode node, ParseTree parseTree)
        {
           // NounFrame nframe = new NounFrame(node, parseTree);
           // string bname = SentenceParser.GetWordString(parseTree, node);
           // AddNounFrame(nframe);

           // stlist.Add(bname.ToString());
           // return stlist;
            /////////////

            //List<NounFrame> nounframelist = new List<NounFrame>();
            //NounFrame nframe = new NounFrame(node, parseTree);
            //string bname = SentenceParser.GetWordString(parseTree, node);
            //AddNounFrame(nframe);
            //stlist.Add(bname.ToString());
            //return stlist;
            ////////////////////////////////////////right code///////////
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(node, parseTree);
            nframe.Sentence_ID = SentenceID;
            AddNounFrame(nframe);
            nounframelist.Add(nframe);
            return nounframelist;
            ///////////////////////////////////
        }
        //Mohamed ELhoseiny 03/17/2012
        public bool HasNoRelationsExceptFrom(NounFrame nf, NounFrame fromnf)
        {

            int idx = Nounframes.IndexOf(fromnf);
            if (nf.Ownerof.Count != 0)
                return false;
            for (int i = 0; i < Nounframes.Count; i++)
            {
                if (i != idx)
                {
                    NounFrame cnf = Nounframes[i];
                    foreach (CaseRole cr in cnf.Ownerof.Keys)
                    {
                        if(cnf.Ownerof[cr].Contains(nf))
                            return false;
                    }
                   
                }
            }

            for (int i = 0; i < VerbFrames.Count; i++)
            {
                VerbFrame cvf = VerbFrames[i];
                foreach (CaseRole cr in cvf.CaseRoles.Keys)
                    {
                        if (cvf.CaseRoles[cr].Contains(nf))
                            return false;
                    }
                foreach (DomainRelationType dr in cvf.DomainRelations_n.Keys)
                {
                    if (cvf.DomainRelations_n[dr].Contains(nf))
                        return false;
                }
                foreach (DomainRelationType dr in cvf.DomainRelations_n.Keys)
                {
                    if (cvf.DomainRelations_n[dr].Contains(nf))
                        return false;
                }
                foreach (TemporalRelationType tr in cvf.TemporalRelations_n.Keys)
                {
                    if (cvf.TemporalRelations_n[tr].Contains(nf))
                        return false;
                }
                
            }
            return true;

        }
        public VerbFrame GET_VING_Verb(ParseNode node, ParseTree parseTree)
        {
            VerbFrame v = new VerbFrame(node);
            string verbname = SentenceParser.GetWordString(parseTree, node);

            v.VerbName = verbname;
            v.Sentence_ID = SentenceID;
            AddVerbrame(v);
            return v;
        }
        private List<NounFrame> GET_VING(ParseNode parseNode, ParseTree parseTree, List<MyWordInfo> advs)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(parseNode, parseTree);
            //nframe.Adv_descriptive.AddRange(advs);
            nframe.Sentence_ID = SentenceID;
            AddNounFrame(nframe);
            nounframelist.Add(nframe);
            return nounframelist;

        }

        public List<NounFrame> GET_NN(ParseNode node, ParseTree parseTree)
        {
            //LIST????!!!!
                                //NN=N
                                //NN=PPN
                                //NN=VING
                                //NN=VING+CMPS
                                //NN=ADVS+VING
                                //NN=ADVS+VING+CMPS
            List<NounFrame> nounframelist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "N"))
            {
                nounframelist.AddRange(GET_N((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPN"))
            {
                nounframelist.AddRange(GET_PPN((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING"))
            {
                nounframelist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING","CMPS"))
            {
                nounframelist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
                //nounframelist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree, verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS", "VING"))
            {
               // List<string> advs = GET_ADVS((ParseNode)node.Children[0], parseTree);//Q
                List<MyWordInfo> advs = GET_ADVS((ParseNode)node.Children[0], parseTree);//Q
                
                nounframelist.AddRange(GET_VING((ParseNode)node.Children[1], parseTree, advs));
               // nounframelist.AddRange(GET_VING((ParseNode)node.Children[1], parseTree));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS", "VING","CMPS"))
            {
                //List<string> advs = GET_ADVS((ParseNode)node.Children[0], parseTree);//Q
                List<MyWordInfo> advs = GET_ADVS((ParseNode)node.Children[0], parseTree);//Q

                nounframelist.AddRange(GET_VING((ParseNode)node.Children[1], parseTree));
                //nounframelist.AddRange(GET_CMPS((ParseNode)node.Children[2], parseTree, verbFrame));
                //need to pass verb to this 3ashan el cmps
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "PPN")
            //    {
            //        nounframelist.AddRange(GET_PPN((ParseNode)node.Children[i], parseTree));
            //    }
            //   else if (((ParseNode)n).Goal == "N")
            //    {
            //     nounframelist.AddRange(GET_N((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "CMPS")
            //    {
            //        //nounframelist.AddRange(GET_CMPS((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "VING")
            //    {
            //        //nounframelist.AddRange(GET_VING((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "ADVS")
            //    {
            //        //nounframelist.AddRange(GET_ADVS((ParseNode)node.Children[i], parseTree));
            //    }
            //    i++;
            //}
            return nounframelist;
        }

       


        public List<NounFrame> GET_PPN(ParseNode node,ParseTree parseTree)
        {
            List<NounFrame> nounframelist = new List<NounFrame>();
            NounFrame nframe = new NounFrame(node, parseTree);
            int length = Nounframes.Count;
            // + fillers
            bool found = false;
            if (Nounframes.Count != 0)
            {
                //foreach (NounFrame nf in Nounframes)
                for (int i = 0; i < length; i++)
                {
                    NounFrame nf = Nounframes[i];
                    if (nf.Text == nframe.Text)
                    {
                        found = true;

                        nframe = nf;
                        break;
                    }

                }
                if (!found)
                {
                    nframe.Sentence_ID = SentenceID;
                    AddNounFrame(nframe);
                }

            }
            else
            {
                AddNounFrame(nframe);
            }
            nounframelist.Add(nframe);
            return nounframelist;
            
        }


        public List<MyWordInfo> GET_AVJ(ParseNode node, ParseTree parseTree)
        {

            List<NounFrame> nounframelist = new List<NounFrame>();
            //List<NounFrame> Adj_list = new List<NounFrame>();

            List<MyWordInfo> AdjectivesList = new List<MyWordInfo>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS", "BADJS"))
            {
                AdjectivesList.AddRange(GET_ADVS((ParseNode)node.Children[0], parseTree));//coz advs are list of strings
                AdjectivesList.AddRange(GET_BADJS((ParseNode)node.Children[1], parseTree));
               // AdjectivesList.AddRange(GET_BADJS((ParseNode)node.Children[1], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"BADJS"))
            { 
                //nounframelist.AddRange(GET_BADJS((ParseNode)node.Children[0], parseTree));

                AdjectivesList.AddRange(GET_BADJS((ParseNode)node.Children[0], parseTree));


                //int i=0;
                //foreach (NounFrame nf in nounframelist)
                //{
                   
                //    ParseNode p = new ParseNode();
                //    AdjectivesList.Add(((ParseNode)nf).Goal);
                //}

            }
            //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "AVJ", "NNC"))
            //{
            //    nounframelist.AddRange(GET_NNC((ParseNode)node.Children[1], parseTree));
            //    Adj_list.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));
              
            //    for (int i = 0; i < nounframelist.Count; i++)
            //    {
            //        for (int j = 0; j < Adj_list.Count; j++)
            //        {
            //            nounframelist[i].TestAdj[j] = Adj_list[j];
            //        }
            //    }

            //    //int i = 0;
            //    //foreach (NounFrame nf in nounframelist)
            //    //{

            //    //    ParseNode p = new ParseNode();
            //    //    AdjectivesList.Add(((ParseNode)nf).Goal);
            //    //}

            //}
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "BADJS")
            //    {
            //        nounframelist.AddRange(GET_BADJS((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "ADVS")
            //    {
            //        //nounframelist.AddRange(GET_ADVS((ParseNode)node.Children[i], parseTree));
            //    }
            //    i++;
            //}
            return AdjectivesList;
        }


        public List<MyWordInfo> GET_BADJS(ParseNode node, ParseTree parseTree)
        {

                        //BADJS=BADJ
                        //BADJS=BADJS1
                        //BADJS=BADJS2
                        //BADJS=BADJ+BADJS
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJ"))
            {
                Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
                
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJS1"))
            {
                Adjectives.AddRange(GET_BADJS1((ParseNode)node.Children[0], parseTree));
                
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJS2"))
            {
                Adjectives.AddRange(GET_BADJS2((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"BADJ", "BADJS"))
            {
                Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
                Adjectives.AddRange(GET_BADJS((ParseNode)node.Children[1], parseTree));
            }
            return Adjectives;
        }
        //adjectives
        public List<NounFrame> GET_FAVJ(ParseNode node, ParseTree parseTree,List<NounFrame>subjects)//VerbFrame verbFrame
        {
            //adjectives.....should be attached to their objects.
            //not nounframe list
            List<NounFrame> nounframelist = new List<NounFrame>();
            //List<ParseNode> Adjs = new List<ParseNode>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS"))
            {
                nounframelist.AddRange(GET_FADJS((ParseNode)node.Children[0], parseTree,subjects));

                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                //adjectives here are parsenodes
            }

            //int i=0;
            //foreach(NounFrame nf in nounframelist)
            //{
            //    Adjs.Add(nf.ParseNode);
            //    //i++;
            //}

            //i got the adjectibves khalaas...should only add them to my objects
            //foreach (NounFrame nf in Objects)
            //{
            //    nf.Addedto = nounframelist;  
            //}


            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "FADJS")
            //    {
            //        nounframelist.AddRange(GET_FADJS((ParseNode)node.Children[i], parseTree));
            //    }
            //    i++;
            //}
            //return nounframelist;
            return nounframelist;
        }

        public List<NounFrame> GET_FAVJ_noun(ParseNode node, ParseTree parseTree, List<NounFrame> Objects)//VerbFrame verbFrame
        {
            //adjectives.....should be attached to their objects.
            //not nounframe list
            List<NounFrame> nounframelist = new List<NounFrame>();
            //List<ParseNode> Adjs = new List<ParseNode>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS"))
            {
                nounframelist.AddRange(GET_FADJS_nouns((ParseNode)node.Children[0], parseTree, Objects));

                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                //adjectives here are parsenodes
            }

            //int i=0;
            //foreach(NounFrame nf in nounframelist)
            //{
            //    Adjs.Add(nf.ParseNode);
            //    //i++;
            //}

            //i got the adjectibves khalaas...should only add them to my objects
            //foreach (NounFrame nf in Objects)
            //{
            //    nf.Addedto = nounframelist;
            //}


            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "FADJS")
            //    {
            //        nounframelist.AddRange(GET_FADJS((ParseNode)node.Children[i], parseTree));
            //    }
            //    i++;
            //}
            //return nounframelist;
            return nounframelist;
        }

       

        //public List<NounFrame> GET_FAVJ_noun(ParseNode node, ParseTree parseTree, VerbFrame verbFrame, List<NounFrame> Objects)//VerbFrame verbFrame
        //{
        //    //adjectives.....should be attached to their objects.
        //    //not nounframe list
        //    List<NounFrame> nounframelist = new List<NounFrame>();
        //    //List<ParseNode> Adjs = new List<ParseNode>();
        //    List<MyWordInfo> Adjectives = new List<MyWordInfo>();
        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS"))
        //    {
        //        nounframelist.AddRange(GET_FADJS((ParseNode)node.Children[0], parseTree, verbFrame, Objects));

        //        //for (int i = 0; i < nounframelist.Count; i++)
        //        //{
        //        //    nounframelist[i].AdjectivesInfo = Adjectives;
        //        //}
        //        //adjectives here are parsenodes
        //    }

        //    //int i=0;
        //    //foreach(NounFrame nf in nounframelist)
        //    //{
        //    //    Adjs.Add(nf.ParseNode);
        //    //    //i++;
        //    //}

        //    //i got the adjectibves khalaas...should only add them to my objects
        //    foreach (NounFrame nf in Objects)
        //    {
        //        nf.Addedto = nounframelist;
        //    }


        //    //int i = 0;
        //    //foreach (ParseNode n in node.Children)
        //    //{
        //    //    if (((ParseNode)n).Goal == "FADJS")
        //    //    {
        //    //        nounframelist.AddRange(GET_FADJS((ParseNode)node.Children[i], parseTree));
        //    //    }
        //    //    i++;
        //    //}
        //    //return nounframelist;
        //    return nounframelist;
        //}

        //FADJS

        public List<NounFrame> GET_FADJS(ParseNode node, ParseTree parseTree,List<NounFrame>subjects)//VerbFrame verbFrame
        {
                            //FADJS=FADJ
                            //FADJS=FADJS1
                            //FADJS=FADJS2


            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJ"))
            {
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree,subjects));//verbFrame

                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS1"))
            {
                //nounframelist.AddRange(GET_FADJS1((ParseNode)node.Children[0], parseTree,verbFrame));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS2"))
            {
                //nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[0], parseTree,verbFrame));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            //if (((ParseNode)node.Children[0]).Goal == "FADJ")
            //{
            //    nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree));
            //}

            //else if (((ParseNode)node.Children[0]).Goal == "FADJS1")
            //{
            //    nounframelist.AddRange(GET_FADJS1((ParseNode)node.Children[0], parseTree));
            //}
            //else if (((ParseNode)node.Children[0]).Goal == "FADJS2")
            //{
            //    nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[0], parseTree));
            //}


            return nounframelist;
        }

        public List<NounFrame> GET_FADJS_nouns(ParseNode node, ParseTree parseTree, List<NounFrame> Objects)//VerbFrame verbFrame
        {
            //FADJS=FADJ
            //FADJS=FADJS1
            //FADJS=FADJS2


            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJ"))
            {
                nounframelist.AddRange(GET_FADJ_noun((ParseNode)node.Children[0], parseTree, Objects));//verbFrame

                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS1"))
            {
                //nounframelist.AddRange(GET_FADJS1((ParseNode)node.Children[0], parseTree, verbFrame));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS2"))
            {
                nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[0], parseTree, null));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            //if (((ParseNode)node.Children[0]).Goal == "FADJ")
            //{
            //    nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree));
            //}

            //else if (((ParseNode)node.Children[0]).Goal == "FADJS1")
            //{
            //    nounframelist.AddRange(GET_FADJS1((ParseNode)node.Children[0], parseTree));
            //}
            //else if (((ParseNode)node.Children[0]).Goal == "FADJS2")
            //{
            //    nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[0], parseTree));
            //}


            return nounframelist;
        }


        //FADJS1
        public List<NounFrame> GET_FADJS1(ParseNode node, ParseTree parseTree,List<NounFrame>subjects)//@@COR , CMA MSH MAWGODEEN
        {
                        //FADJS1=FADJ+CMA+FADJS1
                        //FADJS1=FADJS2+CMA+FADJS1
                        //FADJS1=FADJ+COR+FADJ
                        //FADJS1=FADJ+COR+FADJS2
                        //FADJS1=FADJS2+COR+FADJS2


            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJ","CMA","FADJS1"))
            {
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS1((ParseNode)node.Children[2], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS2", "CMA", "FADJS1"))
            {
                nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS1((ParseNode)node.Children[2], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJ","COR","FADJ"))
            {
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[2], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJ","COR","FADJS2"))
            {
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[2], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS2", "COR", "FADJS2"))
            {
                nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[2], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "FADJ")
            //    {
            //        nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "FADJS1")
            //    {
            //        nounframelist.AddRange(GET_FADJS1((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "FADJS2")
            //    {
            //        nounframelist.AddRange(GET_FADJS2((ParseNode)node.Children[i], parseTree));
            //    }

            //    i++;
            //}
            return nounframelist;
        }

        //FADJS2

        public List<NounFrame> GET_FADJS2(ParseNode node, ParseTree parseTree,List<NounFrame>subjects)//@@SHEWAYET QUESTIONS
        {
                            //FADJS2=ETH+FADJS3+OR+FADJS3
                            //FADJS2=NTH+FADJS3+NOR+FADJS3
                            //FADJS2=NOTN+FADJS3+BUTL+FADJS3
                            //FADJS2=BTH+FADJS3+AND+FADJS3
                            //FADJS2=WTH+FADJS3+OR+FADJS3

            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH","FADJS3","OR","FADJS3"))
            {
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[1], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[3], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "FADJS3", "NOR", "FADJS3"))
            {
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[1], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[3], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "FADJS3", "BUTL", "FADJS3"))
            {
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[1], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[3], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "FADJS3", "AND", "FADJS3"))
            {
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[1], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[3], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH", "FADJS3", "OR", "FADJS3"))
            {
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[1], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[3], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "FADJS3")
            //    {
            //        nounframelist.AddRange(GET_FADJS3((ParseNode)node.Children[i], parseTree));
            //    }

            //    //else if (((ParseNode)n).Goal == "")
            //    //{
            //    //    nounframelist.AddRange(GET_((ParseNode)node.Children[i], parseTree));
            //    //}

            //    //else if (((ParseNode)n).Goal == "")
            //    //{
            //    //    nounframelist.AddRange(GET_((ParseNode)node.Children[i], parseTree));
            //    //}

            //    i++;
            //}
            return nounframelist;
        }

        //FADJS3

        public List<NounFrame> GET_FADJS3(ParseNode node, ParseTree parseTree, List<NounFrame>subjects)
        {
                                //FADJS3=FADJ
                                //FADJS3=FADJS4

            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJ"))
            {
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJS4"))
            {
                nounframelist.AddRange(GET_FADJS4((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            //if (((ParseNode)node.Children[0]).Goal == "FADJ")
            //{
            //    nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree));
            //}

            //else if (((ParseNode)node.Children[0]).Goal == "FADJS4")
            //{
            //    nounframelist.AddRange(GET_FADJS4((ParseNode)node.Children[0], parseTree));
            //}


            return nounframelist;
        }



        //FADJS4

        public List<NounFrame> GET_FADJS4(ParseNode node, ParseTree parseTree, List<NounFrame>subjects)//@@CMA, COR
        {

            //FADJS4=FADJ+CMA+FADJS4
            //FADJS4=FADJ+COR+FADJ

            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJ","CMA","FADJS4"))
            {
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJS4((ParseNode)node.Children[2], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "FADJ","COR","FADJ"))
            {
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[0], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
                nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[2], parseTree,subjects));
                //for (int i = 0; i < nounframelist.Count; i++)
                //{
                //    nounframelist[i].AdjectivesInfo = Adjectives;
                //}
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "FADJ")
            //    {
            //        nounframelist.AddRange(GET_FADJ((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "FADJS4")
            //    {
            //        nounframelist.AddRange(GET_FADJS4((ParseNode)node.Children[i], parseTree));
            //    }


            //    i++;
            //}
            return nounframelist;
        }


        //FADJ

        public List<NounFrame> GET_FADJ(ParseNode node, ParseTree parseTree,List<NounFrame>subjects)//@@CMA ALSO QUESTION TA7T
        {
                        //FADJ=PRPH
                        //FADJ=PRPS //ok
                        //FADJ=ADJC //1 ONLY
                        //FADJ=CMA+ADJC
                        //FADJ=CMA+ADJC+CMA
                        //FADJ=CMA+ABPH  //TOO
                        //FADJ=CMA+ABPH+CMA
            List<NounFrame> nounframelist = new List<NounFrame>();
            // List<VerbFrame> VFlist = new List<VerbFrame>();

             if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPH"))
             {
                 nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[0], parseTree,subjects));
             }
             //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS"))
             //{
             //    nounframelist.AddRange(GET_PRPS((ParseNode)node.Children[0], parseTree, Objects));
             //}
             else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADJC"))
             {
                 nounframelist.AddRange(GET_ADJC((ParseNode)node.Children[0], parseTree));
             }
             else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMA","ADJC"))
             {
                 nounframelist.AddRange(GET_ADJC((ParseNode)node.Children[1], parseTree));
             }
             else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMA", "ADJC","CMA"))
             {
                 nounframelist.AddRange(GET_ADJC((ParseNode)node.Children[1], parseTree));
             }
             else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMA", "ABPH"))
             {
                 nounframelist.AddRange(GET_ABPH((ParseNode)node.Children[1], parseTree,null,subjects));
             }
             else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMA", "ABPH","CMA"))
             {
                 nounframelist.AddRange(GET_ABPH((ParseNode)node.Children[1], parseTree,null,subjects));
             }
             else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS"))
             {
                 nounframelist.AddRange(GET_PRPS_nouns((ParseNode)node.Children[0], parseTree,subjects));
             }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "PRPH")
            //    {
            //       //VFlist.AddRange(GET_PRPH((ParseNode)node.Children[i], parseTree));
            //        //return VFlist;//@@QUESTION
            //    }

            //    else if (((ParseNode)n).Goal == "PRPS")
            //    {
            //        //nounframelist.AddRange(GET_PRPS((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "ADJC")
            //    {
            //        //nounframelist.AddRange(GET_ADJC((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "ABPH")
            //    {
            //        //nounframelist.AddRange(GET_ABPH((ParseNode)node.Children[i], parseTree));
            //    }


            //    i++;
            //}
             return nounframelist;
        }
        public List<NounFrame> GET_FADJ_noun(ParseNode node, ParseTree parseTree, List<NounFrame> Objects)//@@CMA ALSO QUESTION TA7T
        {
            //FADJ=PRPH
            //FADJ=PRPS //ok
            //FADJ=ADJC //1 ONLY
            //FADJ=CMA+ADJC
            //FADJ=CMA+ADJC+CMA
            //FADJ=CMA+ABPH  //TOO
            //FADJ=CMA+ABPH+CMA
            List<NounFrame> nounframelist = new List<NounFrame>();
            // List<VerbFrame> VFlist = new List<VerbFrame>();

            //if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPH"))
            //{
            //    nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[0], parseTree));
            //}
            //else 
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PRPS"))
            {
                nounframelist.AddRange(GET_PRPS_nouns((ParseNode)node.Children[0], parseTree, Objects));
            }
            //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADJC"))
            //{
            //    nounframelist.AddRange(GET_ADJC((ParseNode)node.Children[0], parseTree));
            //}
            //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMA", "ADJC"))
            //{
            //    nounframelist.AddRange(GET_ADJC((ParseNode)node.Children[1], parseTree));
            //}
            //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMA", "ADJC", "CMA"))
            //{
            //    nounframelist.AddRange(GET_ADJC((ParseNode)node.Children[1], parseTree));
            //}
            //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMA", "ABPH"))
            //{
            //    nounframelist.AddRange(GET_ABPH((ParseNode)node.Children[1], parseTree, verbFrame));
            //}
            //else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMA", "ABPH", "CMA"))
            //{
            //    nounframelist.AddRange(GET_ABPH((ParseNode)node.Children[1], parseTree, verbFrame));
            //}


            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "PRPH")
            //    {
            //       //VFlist.AddRange(GET_PRPH((ParseNode)node.Children[i], parseTree));
            //        //return VFlist;//@@QUESTION
            //    }

            //    else if (((ParseNode)n).Goal == "PRPS")
            //    {
            //        //nounframelist.AddRange(GET_PRPS((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "ADJC")
            //    {
            //        //nounframelist.AddRange(GET_ADJC((ParseNode)node.Children[i], parseTree));
            //    }
            //    else if (((ParseNode)n).Goal == "ABPH")
            //    {
            //        //nounframelist.AddRange(GET_ABPH((ParseNode)node.Children[i], parseTree));
            //    }


            //    i++;
            //}
            return nounframelist;
        }

    

        private List<NounFrame> GET_ABPH(ParseNode node, ParseTree parseTree,VerbFrame verbframe, List<NounFrame> subjects)
        {
                    //ABPH=NN+PRPH
                    //ABPH=AVJ+NN+PRPH
                    //ABPH=ARC+NN+PRPH
            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NN","PRPH"))
            {
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[1], parseTree,subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"AVJ","NN","PRPH"))
            {
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[2], parseTree,subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "NN", "PRPH"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[2], parseTree,subjects));
            }
                            //ABPH=ARC+AVJ+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC","AVJ", "NN", "PRPH"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[3], parseTree,subjects));
            }
                            //ABPH=PPJ+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ","NN","PRPH"))
            {
                
                //nounframelist.AddRange(GET_PPJ((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN")
                {

                    foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                    {
                        foreach (NounFrame nf in Nounframes)
                        {
                            string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                            ParseNode pn1 = nf.ParseNode;

                            if (nf.Text == n.Text)
                            {
                                // return nounframelist;
                                // nfs.Add(n1);
                                //                    // Nounframes.(nf);
                                // nounframelist.Add(nf);
                                //if(nf.Ownerof.ContainsValue(
                                nf.AddCaseRolenouns(CaseRole.OwnerOf, nounframelist[0]);
                                //                    //List<NounFrame> nn =new List<NounFrame>();
                                //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                //                    //nf.Ppj.AddRange(nn);
                                break;
                            }
                        }
                    }
                }


                //nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //return nounframelist;
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[2], parseTree,subjects));
            }
                            //ABPH=PPJ+AVJ+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PPJ","AVJ","NN","PRPH"))
            {
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                if (((ParseNode)node.Children[0]).ReferedAnaphoraNode.Goal == "NN")
                {

                    foreach (ParseNode n in ((ParseNode)node.Children[0]).ReferedAnaphoraNode.Children)
                    {
                        foreach (NounFrame nf in Nounframes)
                        {
                            string n1Text = SentenceParser.GetWordString(nf.Parsetree, n);
                            ParseNode pn1 = nf.ParseNode;

                            if (nf.Text == n.Text)
                            {
                                // return nounframelist;
                                // nfs.Add(n1);
                                //                    // Nounframes.(nf);
                                // nounframelist.Add(nf);
                                //if(nf.Ownerof.ContainsValue(
                                nf.AddCaseRolenouns(CaseRole.OwnerOf, nounframelist[0]);
                                //                    //List<NounFrame> nn =new List<NounFrame>();
                                //                    //nn.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                                //                    //nf.Ppj.AddRange(nn);
                                break;
                            }
                        }
                    }
                }


                //nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                //return nounframelist;
                //nounframelist.AddRange(GET_PPJ((ParseNode)node.Children[0], parseTree));
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
               
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[3], parseTree,subjects));
            }
            //================
                        //ABPH=CNP+NN+PRPH
                        //ABPH=CNP+AVJ+NN+PRPH
                        //ABPH=ARC+CNP+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CNP","NN","PRPH"))
            {
                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);

                nounframelist.AddRange(GET_NN((ParseNode)node.Children[1], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[2], parseTree,subjects));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CNP","AVJ","NN","PRPH"))
            {
                NounFrame nframe1 = new NounFrame((ParseNode)node.Children[0], parseTree);
                nounframelist.Add(nframe1);
                AddNounFrame(nframe1);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[3], parseTree,subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC","CNP","NN","PRPH"))
            {
                //NounFrame nframe1 = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe1);
                //AddNounFrame(nframe1);
                NounFrame nframe = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe);
                AddNounFrame(nframe);
                
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[3], parseTree,subjects));
            }
                                //ABPH=ARC+CNP+AVJ+NN+PRPH
                                //ABPH=ARC+OF+NN+PRPH
                                //ABPH=ARC+AVJ+OF+NN+PRPH
                                //ABPH=ARC+OF+ARC+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "CNP", "AVJ", "NN", "PRPH"))
            {
                //NounFrame nframe1 = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe1);
                //AddNounFrame(nframe1);
                NounFrame nframe = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nframe);
                AddNounFrame(nframe);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[2], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[3], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[4], parseTree,subjects));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "OF", "NN", "PRPH"))
            {
                //NounFrame nframe1 = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe1);
                //AddNounFrame(nframe1);

                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[3], parseTree,subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "OF", "NN", "PRPH"))
            {
                //NounFrame nframe1 = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe1);
                //AddNounFrame(nframe1);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                nounframelist.AddRange(GET_NN((ParseNode)node.Children[3], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[4], parseTree,subjects));
            }
            //ARC+OF+ARC+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "OF", " ARC", "NN", "PRPH"))
            {
                //NounFrame nframe1 = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe1);
                //AddNounFrame(nframe1);
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[2], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                nounframelist.AddRange(GET_NN((ParseNode)node.Children[3], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[4], parseTree,subjects));
            }

                                //ABPH=ARC+AVJ+OF+ARC+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ARC", "AVJ", "OF", "ARC", "NN", "PRPH"))
            {
                //NounFrame nframe1 = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe1);
                //AddNounFrame(nframe1);
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[3], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                nounframelist.AddRange(GET_NN((ParseNode)node.Children[4], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[5], parseTree,subjects));
            }
            //ABPH=ALL+ARC+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ALL", "ARC", "NN", "PRPH"))
            {
                //NounFrame nframe1 = new NounFrame((ParseNode)node.Children[1], parseTree);
                //nounframelist.Add(nframe1);
                //AddNounFrame(nframe1);

                nounframelist.AddRange(GET_NN((ParseNode)node.Children[2], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[3], parseTree,subjects));
            }
            //ABPH=ALL+AVJ+ARC+NN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ALL", "AVJ", "ARC", "NN", "PRPH"))
            {
                Adjectives.AddRange(GET_AVJ((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                //NounFrame nframe1 = new NounFrame((ParseNode)node.Children[2], parseTree);
                //nounframelist.Add(nframe1);
                //AddNounFrame(nframe1);

                nounframelist.AddRange(GET_NN((ParseNode)node.Children[3], parseTree));
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[4], parseTree,subjects));
            }
            //ABPH=NP+RXPN+PRPH
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NP", "RXPN", "PRPH"))
            {
                nounframelist.AddRange(GET_NP((ParseNode)node.Children[0], parseTree,subjects));
                NounFrame nf = new NounFrame((ParseNode)node.Children[1], parseTree);
                nounframelist.Add(nf);
                AddNounFrame(nf);
                nounframelist.AddRange(GET_PRPH((ParseNode)node.Children[2], parseTree,subjects));
            }
            return nounframelist;
        }
        //ADJC
        public List<NounFrame> GET_ADJC(ParseNode node, ParseTree parseTree)
        {
                                //ADJC=AJPN+PRD
                                //ADJC=AJPN+DS

            List<NounFrame> nounframelist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "AJPN","PRD"))
            {
                nounframelist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
            }
            return nounframelist;
        }
        //PRPH
        public List<NounFrame> GET_PRPH(ParseNode node, ParseTree parseTree,List<NounFrame>subjects)
        {

            //PRPH=VING
            //PRPH=ADVS+VING
            //PRPH=VING+CMPS
            //PRPH=ADVS+VING+CMPS
            //PRPH=VPSP
            //PRPH=ADVS+VPSP
            //PRPH=VPSP+CMPS
            //PRPH=ADVS+VPSP+CMPS

            List<NounFrame> nflist = new List<NounFrame>();

            if(MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"VING"))
            {
                nflist.AddRange(GET_VING((ParseNode)node.Children[0],parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS","VING"))
            {
               // nflist.AddRange(GET_ADVS((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_VING((ParseNode)node.Children[1], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING","CMPS"))
            {
                nflist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree,null,subjects));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS","VING","CMPS"))
            {
                //////// nflist.AddRange(GET_ADVS((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_VING((ParseNode)node.Children[1], parseTree));
                //nflist.AddRange(GET_CMPS((ParseNode)node.Children[2], parseTree));
            }//eb3at el verb
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VPSP"))
            {
                
            }
            
            return nflist;
        }

        
        //public List<NounFrame> GET_BADJS(ParseNode node, ParseTree parseTree)
        //{
        //    List<NounFrame> nounframelist = new List<NounFrame>();

        //    int i = 0;
        //    foreach (ParseNode n in node.Children)
        //    {
        //        if (((ParseNode)n).Goal == "BADJ")
        //        {
        //            nounframelist.AddRange(GET_BADJ((ParseNode)node.Children[i], parseTree));
        //        }

        //        else if (((ParseNode)n).Goal == "BADJS1")
        //        {
        //            nounframelist.AddRange(GET_BADJS1((ParseNode)node.Children[i], parseTree));
        //        }

        //        else if (((ParseNode)n).Goal == "BADJS2")
        //        {
        //            nounframelist.AddRange(GET_BADJS2((ParseNode)node.Children[i], parseTree));
        //        }

        //        else if (((ParseNode)n).Goal == "BADJS")
        //        {
        //            nounframelist.AddRange(GET_BADJS((ParseNode)node.Children[i], parseTree));
        //        }


        //        i++;
        //    }
        //    return nounframelist;
        //}
        //BADJS1
        public List<MyWordInfo> GET_BADJS1(ParseNode node, ParseTree parseTree)//@@COR , CMA MSH MAWGODEEN
        {


            List<NounFrame> nounframelist = new List<NounFrame>();
                            //BADJS1=BADJ+CMA+BADJS1
                            //BADJS1=BADJS2+CMA+BADJS1
                            //BADJS1=BADJ+COR+BADJ
                            //BADJS1=BADJ+COR+BADJS2
                            //BADJS1=BADJS2+COR+BADJS2
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJ","CMA","BADJS1"))
            {
                Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                Adjectives.AddRange(GET_BADJS1((ParseNode)node.Children[2], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJS2","CMA","BADJS1"))
            {
                Adjectives.AddRange(GET_BADJS2((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
                Adjectives.AddRange(GET_BADJS1((ParseNode)node.Children[2], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJ","COR","BADJ"))
            {
                Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
                Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[2], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJ","COR","BADJS2"))
            {
              Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
              for (int i = 0; i < nounframelist.Count; i++)
              {
                  nounframelist[i].AdjectivesInfo = Adjectives;
                  //for (int j = 0; j < nfl.Count; j++)
                  //{
                  //    nflist[i].TestAdj[j] = nfl[j];
                  //}
              }
                Adjectives.AddRange(GET_BADJS2((ParseNode)node.Children[2], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJS2", "COR", "BADJS2"))
            {
                Adjectives.AddRange(GET_BADJS2((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
                Adjectives.AddRange(GET_BADJS2((ParseNode)node.Children[2], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }

            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "BADJ")
            //    {
            //        nounframelist.AddRange(GET_BADJ((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "BADJS1")
            //    {
            //        nounframelist.AddRange(GET_BADJS1((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "BADJS2")
            //    {
            //        nounframelist.AddRange(GET_BADJS2((ParseNode)node.Children[i], parseTree));
            //    }
               
            //    i++;
            //}
            return Adjectives;
        }

        //BADJS2

        public List<MyWordInfo> GET_BADJS2(ParseNode node, ParseTree parseTree)//@@SHEWAYET QUESTIONS
        {
                            //BADJS2=ETH+BADJS3+OR+BADJS3
                            //BADJS2=NTH+BADJS3+NOR+BADJS3
                            //BADJS2=NOTN+BADJS3+BUTL+BADJS3
                            //BADJS2=BTH+BADJS3+AND+BADJS3
                            //BADJS2=WTH+BADJS3+OR+BADJS3

            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH","BADJS3","OR","BADJS3"))
            {
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[3], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NTH", "BADJS3", "NOR", "BADJS3"))
            {
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[1], parseTree)); for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[3], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "NOTN", "BADJS3", "BUTL", "BADJS3"))
            {
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[1], parseTree)); for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[3], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH", "BADJS3", "AND", "BADJS3"))
            {
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[3], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH", "BADJS3", "OR", "BADJS3"))
            {
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[1], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
                Adjectives.AddRange(GET_BADJS3((ParseNode)node.Children[3], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "BADJS3")
            //    {
            //        nounframelist.AddRange(GET_BADJS3((ParseNode)node.Children[i], parseTree));
            //    }

            //    //else if (((ParseNode)n).Goal == "")
            //    //{
            //    //    nounframelist.AddRange(GET_((ParseNode)node.Children[i], parseTree));
            //    //}

            //    //else if (((ParseNode)n).Goal == "")
            //    //{
            //    //    nounframelist.AddRange(GET_((ParseNode)node.Children[i], parseTree));
            //    //}

            //    i++;
            //}
            return Adjectives;
        }

        //BADJS3




        public List<MyWordInfo> GET_BADJS3(ParseNode node, ParseTree parseTree)
        {
                            //BADJS3=BADJ
                            //BADJS3=BADJS4
            List<NounFrame> nounframelist = new List<NounFrame>();
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJ"))
            {
                Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                    //for (int j = 0; j < nfl.Count; j++)
                    //{
                    //    nflist[i].TestAdj[j] = nfl[j];
                    //}
                }
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJS4"))
            {
                Adjectives.AddRange(GET_BADJS4((ParseNode)node.Children[0], parseTree));
                for (int i = 0; i < nounframelist.Count; i++)
                {
                    nounframelist[i].AdjectivesInfo = Adjectives;
                }
            }
                //if (((ParseNode)node.Children[0]).Goal == "BADJ")
                //{
                //    nounframelist.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
                //}

                //else if (((ParseNode)node.Children[0]).Goal == "BADJS4")
                //{
                //    nounframelist.AddRange(GET_BADJS4((ParseNode)node.Children[0], parseTree));
                //}

            
            return Adjectives;
        }

        //BADJS4
        public List<MyWordInfo> GET_BADJS4(ParseNode node, ParseTree parseTree)//@@CMA, COR
        {

                            //BADJS4=BADJ+CMA+BADJS4
                            //BADJS4=BADJ+COR+BADJ

           List<NounFrame> nounframelist = new List<NounFrame>();
           List<MyWordInfo> Adjectives = new List<MyWordInfo>();
           if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJ","CMA","BADJS4"))
           {
               //Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
               Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
               for (int i = 0; i < nounframelist.Count; i++)
               {
                   nounframelist[i].AdjectivesInfo = Adjectives;
                   //for (int j = 0; j < nfl.Count; j++)
                   //{
                   //    nflist[i].TestAdj[j] = nfl[j];
                   //}
               }
               Adjectives.AddRange(GET_BADJS4((ParseNode)node.Children[2], parseTree));
               for (int i = 0; i < nounframelist.Count; i++)
               {
                   nounframelist[i].AdjectivesInfo = Adjectives;
               }
           }
           else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BADJ","COR","BADJ"))
           {
               Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[0], parseTree));
               for (int i = 0; i < nounframelist.Count; i++)
               {
                   nounframelist[i].AdjectivesInfo = Adjectives;
                   //for (int j = 0; j < nfl.Count; j++)
                   //{
                   //    nflist[i].TestAdj[j] = nfl[j];
                   //}
               }
               Adjectives.AddRange(GET_BADJ((ParseNode)node.Children[2], parseTree));
               for (int i = 0; i < nounframelist.Count; i++)
               {
                   nounframelist[i].AdjectivesInfo = Adjectives;
                   //for (int j = 0; j < nfl.Count; j++)
                   //{
                   //    nflist[i].TestAdj[j] = nfl[j];
                   //}
               }
           }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "BADJ")
            //    {
            //        nounframelist.AddRange(GET_BADJ((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "BADJS4")
            //    {
            //        nounframelist.AddRange(GET_BADJS4((ParseNode)node.Children[i], parseTree));
            //    }


            //    i++;
            //}
            return Adjectives;
        }
        
        //BADJ

        public List<MyWordInfo> GET_BADJ(ParseNode node, ParseTree parseTree)
        {

                        //BADJ=BPADJ
                        //BADJ=CADJ
                        //BADJ=SADJ
                        //BADJ=VPSP
                        //BADJ=VING
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            MyWordInfo ADJ = new MyWordInfo();

            List<NounFrame> nounframelist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING"))
            {
                //nounframelist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                ADJ.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ.Sense = ((ParseNode)node.Children[0]).SenseNo;
                ADJ.Word = nframe.Text;
                Adjectives.Add(ADJ);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BPADJ"))
            {
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                ADJ.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ.Sense = ((ParseNode)node.Children[0]).SenseNo;
                ADJ.Word = nframe.Text;
                Adjectives.Add(ADJ);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);

                //nframe.Adjectives.Add((ParseNode)node.Children[0]);

            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CADJ"))
            {
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                ADJ.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ.Sense = ((ParseNode)node.Children[0]).SenseNo;
                ADJ.Word = nframe.Text;
                Adjectives.Add(ADJ);
                
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "SADJ"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                ADJ.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ.Sense = ((ParseNode)node.Children[0]).SenseNo;
                ADJ.Word = nframe.Text;
                Adjectives.Add(ADJ);
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VPSP"))
            {
                //NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                //nounframelist.Add(nframe);
                //AddNounFrame(nframe);
                NounFrame nframe = new NounFrame((ParseNode)node.Children[0], parseTree);
                ADJ.Pos = Wnlib.PartsOfSpeech.Adj;
                ADJ.Sense = ((ParseNode)node.Children[0]).SenseNo;
                ADJ.Word = nframe.Text;
                Adjectives.Add(ADJ);
            }
                //if (((ParseNode)node.Children[0]).Goal == "BPADJ")//@@TERMINAL
                //{
                //    //nounframelist.AddRange(GET_BPADJ((ParseNode)node.Children[0], parseTree));
                //}

                //else if (((ParseNode)node.Children[0]).Goal == "CADJ")//@@TERMINAL
                //{
                //    //nounframelist.AddRange(GET_CADJ((ParseNode)node.Children[0], parseTree));
                //}
                //else if (((ParseNode)node.Children[0]).Goal == "SADJ")//@@TERMINAL
                //{
                //    //nounframelist.AddRange(GET_SADJ((ParseNode)node.Children[0], parseTree));
                //}
                //else if (((ParseNode)node.Children[0]).Goal == "VPSP")//@@TERMINAL
                //{
                //    //nounframelist.AddRange(GET_VPSP((ParseNode)node.Children[0], parseTree));
                //}
                //else if (((ParseNode)node.Children[0]).Goal == "VING")//@TERMINAL
                //{
                //    //nounframelist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
                //}
                
            //return nounframelist;
            return Adjectives;
        }




        //ADVS

        public List<MyWordInfo> GET_ADVS(ParseNode node, ParseTree parseTree)
        {
                            //ADVS=ADV
                            //ADVS=ADVS1
                            //ADVS=ADVS2
                            //ADVS=ADV+ADVS
            List<MyWordInfo> adv = new List<MyWordInfo>();
            List<NounFrame> nflist = new List<NounFrame>();


            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADV"))
            {
                //stlist.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
                adv.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_ADV_nf((ParseNode)node.Children[0], parseTree));
            }
          else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS1"))
            {
                adv.AddRange(GET_ADVS1((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS2"))
            {
                //stlist.AddRange(GET_ADVS2((ParseNode)node.Children[0], parseTree, verbFrame));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"ADV", "ADVS"))
            {
                //stlist.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
                adv.AddRange(GET_ADVS((ParseNode)node.Children[1], parseTree));
            }

            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "ADV")
            //    {
            //        vflist.AddRange(GET_ADV((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "ADVS1")
            //    {
            //        //vflist.AddRange(GET_ADVS1((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "ADVS2")
            //    {
            //        //vflist.AddRange(GET_ADVS2((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "ADVS")
            //    {
            //        //vflist.AddRange(GET_ADVS((ParseNode)node.Children[i], parseTree));
            //    }


            //    i++;
            //}
            return adv;
        }


        //ADVS1
     

        public List<MyWordInfo> GET_ADVS1(ParseNode node, ParseTree parseTree)//@@COR , CMA MSH MAWGODEEN
        {       
            //ADVS1=ADV+COR+ADVS2
            //ADVS1=ADVS2+COR+ADVS2

            List<string> stlist = new List<string>();
            List<MyWordInfo> adv = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADV","CMA","ADVS1")) 
            {
                adv.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
              //  stlist.AddRange(GET_CMA((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS1((ParseNode)node.Children[2], parseTree));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADV","COR","ADV")) 
            {
                adv.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
               // stlist.AddRange(GET_COR((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADV((ParseNode)node.Children[2], parseTree));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS2", "CMA", "ADVS1"))
            {
                adv.AddRange(GET_ADVS2((ParseNode)node.Children[0], parseTree));
                // stlist.AddRange(GET_COR((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS1((ParseNode)node.Children[2], parseTree));
            }

            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADV", "COR", "ADVS2"))
            {
                adv.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
                // stlist.AddRange(GET_COR((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS2((ParseNode)node.Children[2], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS2", "COR", "ADVS2"))
            {
                adv.AddRange(GET_ADVS2((ParseNode)node.Children[0], parseTree));
                // stlist.AddRange(GET_COR((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS2((ParseNode)node.Children[2], parseTree));
            }
            
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "ADV")
            //    {
            //        //nounframelist.AddRange(GET_ADV((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "ADVS1")
            //    {
            //        nounframelist.AddRange(GET_ADVS1((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "ADVS2")
            //    {
            //        nounframelist.AddRange(GET_ADVS2((ParseNode)node.Children[i], parseTree));
            //    }

            //    i++;
            //}
            return adv;
        }


        //ADVS2
        public List<MyWordInfo> GET_ADVS2(ParseNode node, ParseTree parseTree)//@@SHEWAYET QUESTIONS
        {

                    //ADVS2=ETH+ADVS3+OR+ADVS3
                    //ADVS2=NTH+ADVS3+NOR+ADVS3
                    //ADVS2=NOTN+ADVS3+BUTL+ADVS3
                    //ADVS2=BTH+ADVS3+AND+ADVS3
                    //ADVS2=WTH+ADVS3+OR+ADVS3
            List<string> stlist = new List<string>();
            List<MyWordInfo> adv = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ETH", "ADVS3", "OR", "ADVS3"))
            {
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"NTH","ADVS3","NOR","ADVS3" ))
            {
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node,"NOTN","ADVS3","BUTL","ADVS3"))
            {
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "BTH","ADVS3","AND","ADVS3"))
            {
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[3], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "WTH","ADVS3","OR","ADVS3"))
            {
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[1], parseTree));
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[3], parseTree));
            }
            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "ADVS3")
            //    {
            //        nounframelist.AddRange(GET_ADVS3((ParseNode)node.Children[i], parseTree));
            //    }

                //else if (((ParseNode)n).Goal == "")
                //{
                //    nounframelist.AddRange(GET_((ParseNode)node.Children[i], parseTree));
                //}

                //else if (((ParseNode)n).Goal == "")
                //{
                //    nounframelist.AddRange(GET_((ParseNode)node.Children[i], parseTree));
                //}

            //    i++;
            //}
            return adv;
        }


        //ADVS3
        public List<MyWordInfo> GET_ADVS3(ParseNode node, ParseTree parseTree)
        {
            //ADVS3=ADV
            //ADVS3=ADVS4
            //ADVS3=ADV+ADVS3

            List<string> stlist = new List<string>();
            List<MyWordInfo> adv = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADV"))
            {
                adv.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADVS4"))
            {
                adv.AddRange(GET_ADVS4((ParseNode)node.Children[0], parseTree));
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADV","ADVS3"))
            {
                adv.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
                adv.AddRange(GET_ADVS3((ParseNode)node.Children[1], parseTree));
            }
            //int i = 0;
            //foreach(ParseNode n in node.Children)
            //{

            //if (((ParseNode)n).Goal == "ADV")
            //{
            //    //nounframelist.AddRange(GET_ADV((ParseNode)node.Children[i], parseTree));
            //}

            //else if (((ParseNode)n).Goal == "ADVS4")
            //{
            //    //nounframelist.AddRange(GET_ADVS4((ParseNode)node.Children[i], parseTree));
            //}
            //else if (((ParseNode)n).Goal == "ADVS3")
            //{
            //    //nounframelist.AddRange(GET_ADVS3((ParseNode)node.Children[i], parseTree));
            //}
            //i++;
            //}
            return adv;
        }

        //ADVS4

        public List<MyWordInfo> GET_ADVS4(ParseNode node, ParseTree parseTree)//@@CMA, COR
        {

                        //ADVS4=ADV+CMA+ADVS4
                        //ADVS4=ADV+COR+ADV

            List<string> stlist = new List<string>();
            List<MyWordInfo> adv = new List<MyWordInfo>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "ADV", "CMA", "ADVS4"))
            {
                adv.AddRange(GET_ADV((ParseNode)node.Children[0], parseTree));
                adv.AddRange(GET_ADVS4((ParseNode)node.Children[2], parseTree));
            }

            //int i = 0;
            //foreach (ParseNode n in node.Children)
            //{
            //    if (((ParseNode)n).Goal == "ADV")
            //    {
            //        //nounframelist.AddRange(GET_ADV((ParseNode)node.Children[i], parseTree));
            //    }

            //    else if (((ParseNode)n).Goal == "ADVS4")
            //    {
            //        nounframelist.AddRange(GET_ADVS4((ParseNode)node.Children[i], parseTree));
            //    }


            //    i++;
            //}
            return adv;
        }


        //ADV
        public List<NounFrame> GET_ADV_nf(ParseNode node, ParseTree parseTree)
        {

                                //ADV=ADVC
                                //ADV=INFPH
                                //ADV=PRP
                                //ADV=PADV TEEMA
                                //ADV=CMADV
            List<string> stlist = new List<string>();
            List<NounFrame> nflist = new List<NounFrame>();
            
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PADV"))
            {
               
               // stlist.AddRange(GET_PADV((ParseNode)node.Children[0], parseTree));
                
               
            }
            else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMADV"))
            {
                //stlist.AddRange(GET_CMADV((ParseNode)node.Children[0], parseTree));
                nflist.AddRange(GET_CMADV_nf((ParseNode)node.Children[0], parseTree));
            }
            //if (((ParseNode)node.Children[0]).Goal == "ADVC")
            //{
            //   //vflist.AddRange(GET_ADVC((ParseNode)node.Children[0], parseTree));
            //}

            //else if (((ParseNode)node.Children[0]).Goal == "INFPH")//TEEMA
            //{
            //    //vflist.AddRange(GET_INFPH((ParseNode)node.Children[0], parseTree));
            //}
            //else if (((ParseNode)node.Children[0]).Goal == "PRP")
            //{
            //    //vflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree));
            //}
            //else if (((ParseNode)node.Children[0]).Goal == "PADV")//@@TEEMA
            //{
            //   //vflist.AddRange(GET_PADV((ParseNode)node.Children[0], parseTree));
            //}
            //else if (((ParseNode)node.Children[0]).Goal == "CMADV")//@@TEEMA
            //{
            //  //vflist.AddRange(GET_CMADV((ParseNode)node.Children[0], parseTree));
            //}

            //return stlist;
            return nflist;
        }

        //public List<string> GET_ADV(ParseNode node, ParseTree parseTree)
        //{
        //    //ADV=ADVC
        //    //ADV=INFPH
        //    //ADV=PRP
        //    //ADV=PADV TEEMA
        //    //ADV=CMADV


        //    List<string> stlist = new List<string>();
        //    List<NounFrame> nflist = new List<NounFrame>();

        //    if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "PADV"))
        //    {

        //         stlist.AddRange(GET_PADV_st((ParseNode)node.Children[0], parseTree));


        //    }
        //    else if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "CMADV"))
        //    {
        //        stlist.AddRange(GET_CMADV_st((ParseNode)node.Children[0], parseTree));
        //        nflist.AddRange(GET_CMADV_nf((ParseNode)node.Children[0], parseTree));
        //    }
        //    //if (((ParseNode)node.Children[0]).Goal == "ADVC")
        //    //{
        //    //   //vflist.AddRange(GET_ADVC((ParseNode)node.Children[0], parseTree));
        //    //}

        //    //else if (((ParseNode)node.Children[0]).Goal == "INFPH")//TEEMA
        //    //{
        //      //  vflist.AddRange(GET_INFPH((ParseNode)node.Children[0], parseTree));
        //    //}
        //    //else if (((ParseNode)node.Children[0]).Goal == "PRP")
        //    //{
        //    //    //vflist.AddRange(GET_PRP((ParseNode)node.Children[0], parseTree));
        //    //}
        //    //else if (((ParseNode)node.Children[0]).Goal == "PADV")//@@TEEMA
        //    //{
        //    //   //vflist.AddRange(GET_PADV((ParseNode)node.Children[0], parseTree));
        //    //}
        //    //else if (((ParseNode)node.Children[0]).Goal == "CMADV")//@@TEEMA
        //    //{
        //    //  //vflist.AddRange(GET_CMADV((ParseNode)node.Children[0], parseTree));
        //    //}

        //    return stlist;
        //    //return nflist;
        //}

        public List<NounFrame> GET_CMADV_nf(ParseNode node, ParseTree parseTree)
        { 
            //List<string> stlist = new List<string>();
            List<NounFrame> nounframelist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING", "CMPS"))
            {
                nounframelist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
                //ab3at anhi verb? null walla earning?
                //nounframelist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree));
                //adds "from"
               // nounframelist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree,));

                //eb3at el verb
               ////////// stlist.AddRange(nounframelist.ToString());
            }


                //CMADV=AS+PADV+AS+SBJ
                //CMADV=AS+PADV+AS+SBJ+PRD
                //CMADV=AS+PADV+CMPS+AS+SBJ
                //CMADV=AS+PADV+CMPS+AS+SBJ+PRD
                //CMADV=MORE+PADV+THAN+SBJ
                //CMADV=MORE+PADV+THAN+SBJ+PRD
                //CMADV=MORE+PADV+CMPS+THAN+SBJ
                //CMADV=MORE+PADV+CMPS+THAN+SBJ+PRD
                //CMADV=LESS+PADV+THAN+SBJ
                //CMADV=LESS+PADV+THAN+SBJ+PRD
                //CMADV=LESS+PADV+CMPS+THAN+SBJ
                //CMADV=LESS+PADV+CMPS+THAN+SBJ+PRD
                //CMADV=CADJ+THAN+SBJ
                //CMADV=CADJ+THAN+SBJ+PRD
                //CMADV=CADJ+CMPS+THAN+SBJ
                //CMADV=CADJ+CMPS+THAN+SBJ+PRD
                //CMADV=THE+MOST+PADV
                //CMADV=THE+LST+PADV
                //CMADV=THE+SADJ
                //CMADV=VING+CMPS

            return nounframelist;
        }
        public List<string> GET_CMADV_st(ParseNode node, ParseTree parseTree)
        {
            List<string> stlist = new List<string>();
            List<NounFrame> nounframelist = new List<NounFrame>();

            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VING", "CMPS"))
            {
                nounframelist.AddRange(GET_VING((ParseNode)node.Children[0], parseTree));
                //nounframelist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree));
                //eb3at el verb
                ////////// stlist.AddRange(nounframelist.ToString());
            }


            return stlist;
        }

        public List<string> GET_PADV_nf(ParseNode node, ParseTree parseTree)
        {
            List<string> stlist = new List<string>();
            NounFrame nframe = new NounFrame(node, parseTree);
        
            AddNounFrame(nframe);
            stlist.Add(nframe.Text);
            //stlist.Add(nframe);
            return stlist;
        }
        public List<MyWordInfo> GET_PADV_adj(ParseNode node, ParseTree parseTree)
        {
            //List<string> stlist = new List<string>();
            //List<NounFrame> nflist = new List<NounFrame>();
            //NounFrame nframe = new NounFrame(node, parseTree);

            //AddNounFrame(nframe);
            //nflist.Add(nframe);
            ////stlist.Add(nframe.Text);
            ////stlist.Add(nframe);
            //return nflist;
            List<MyWordInfo> Adjectives = new List<MyWordInfo>();
            MyWordInfo ADJ = new MyWordInfo();
            NounFrame nframe = new NounFrame(node, parseTree);
            ADJ.Pos = Wnlib.PartsOfSpeech.Adj;
            ADJ.Sense = (node).SenseNo;
            ADJ.Word = nframe.Text;
            Adjectives.Add(ADJ);
            PADV = true;
            return Adjectives;
        }

        //ADVC

        public List<VerbFrame> GET_ADVC(ParseNode node, ParseTree parseTree, VerbFrame verbFrame)//@@VERYYYY IMP QUESTION IN VERBFRAME
        {
            //ADVC=SOR+DS

            //List<NounFrame> nounframelist = new List<NounFrame>();
            List<VerbFrame> vflist = new List<VerbFrame>();

            int i = 0;
            foreach (ParseNode n in node.Children)
            {
                if (((ParseNode)n).Goal == "SOR")//@@terminal msh ma7tota ka function.
                {
                    //nounframelist.AddRange(GET_SOR((ParseNode)node.Children[i], parseTree));
                }
                else if (((ParseNode)n).Goal == "DS")
                {
                     vflist.AddRange(Get_DS((ParseNode)node.Children[i], parseTree));
                }
                i++;
            }
            return vflist;
         }


        ///////////////////////////////SS////////
        //START OF SS

        public List<VerbFrame> GET_SS(ParseNode node, ParseTree parseTree)
        {

           // List<NounFrame> nounframelist = new List<NounFrame>();
            List<VerbFrame> vflist=new List<VerbFrame>();

            if (((ParseNode)node.Children[0]).Goal == "DS")
            {
                vflist.AddRange(Get_DS((ParseNode)node.Children[0], parseTree));
            }
            else if (((ParseNode)node.Children[0]).Goal == "IMS")
            {
                vflist.AddRange(GET_IMS((ParseNode)node.Children[0], parseTree));
            }

            else if (((ParseNode)node.Children[0]).Goal == "SS")
            {
                vflist.AddRange(GET_IMS((ParseNode)node.Children[2], parseTree));
            }
            
            return vflist;
        }


        public List<VerbFrame> GET_IMS(ParseNode node, ParseTree parseTree)
        {
                            //IMS=VINF
                            //IMS=ADVS+VINF
                            //IMS=VINF+CMPS
                            //IMS=ADVS+VINF+CMPS
            VerbFrame vframe;
            List<VerbFrame> vflist = new List<VerbFrame>();
            List<NounFrame> nounframelist = new List<NounFrame>();
            if (MindMapMeaningRepresentation.MRHelperClass.CheckRule(node, "VINF", "CMPS"))
            {
                vframe=(GET_VINF((ParseNode)node.Children[0], parseTree));
                nounframelist.AddRange(GET_CMPS((ParseNode)node.Children[1], parseTree,vframe,null));
                vflist.Add(vframe);
                //eb3at el verb
                ////////// stlist.AddRange(nounframelist.ToString());
            }
             return vflist;
                           
        }

        public VerbFrame GET_VINF(ParseNode node, ParseTree parseTree)
        {
            //throw new NotImplementedException();
            VerbFrame v = new VerbFrame(node);
            string verbname = SentenceParser.GetWordString(parseTree, node);

            v.VerbName = verbname;
            v.Sentence_ID = SentenceID;
            AddVerbrame(v);
            return v;


        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //END OF OUR CODE
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        

        public void BuildTMR()
        {
            GetWordologyList();
            //MapMeaning();
            foreach (ParseTree ptree in _parseTrees)
            {
                
                BuildSentenceTMR(ptree);
                SentenceID++;
                CurrentNFS.Clear();
                CMP_OBJ = false;
            }
            MapMeaning();
            
        }
        public void LoadOntology()
        {
            if (Onto_mo2a == null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                System.IO.FileStream fs = new System.IO.FileStream(
                    OntologyDirectoryPath + @"\Ontology.bin", FileMode.Open);
                Onto_mo2a = (OntologyLibrary.OntoSem.Ontology)bf.Deserialize(fs);
                Onto_mo2a.OntologyPath = OntologyDirectoryPath;
                fs.Close();
            }

        }
        //private void MapMeaning()
        //{
        //    LoadOntology();
        //    #region New Mapping Code
        //    //foreach (NounFrame nf in Nounframes)
        //    //{
        //    //    ParseNode pn = nf.ParseNode;
        //    //    string Sense = pn.Sense;
        //    //    int SenseNo = pn.SenseNo;
        //    //    string Word = nf.Text;

        //    //    //the pos is obtained from Wordology.....right?
        //     //   nf.Concept = MindMapMapper.GetConcept(Word, SenseNo, Sense, Wnlib.PartsOfSpeech.Noun);


        //    //}
        //    //foreach (VerbFrame vf in VerbFrames)
        //    //{
        //    //    ParseNode pn = vf._parseNode;
        //    //    string Sense = pn.Sense;
        //    //    int SenseNo = pn.SenseNo;
        //    //    string Word = vf.VerbName;

        //    //    //the pos is obtained from Wordology.....right?
        //    //    vf.Concept = MindMapMapper.GetConcept(Word, SenseNo, Sense, Wnlib.PartsOfSpeech.Verb);


        //    //}
        //    #endregion
        //    #region Old Code
        //    ////foreach (NounFrame Nf in Nounframes)
        //    ////{
        //    ////    ParseNode pn = Nf.ParseNode;
        //    ////    string pn_text = Nf.Text;
                
        //    ////    Nf.Adjective_fillerType = new List<string>();
        //    ////    if (Nf.Adjective == null)
        //    ////    {
        //    ////        continue;
        //    ////    }
        //    ////    for (int i = 0; i < Nf.Adjective.Count; i++)
        //    ////    {
        //    ////        Nf.Adjective_fillerType.Add("");
        //    ////    }
        //    ////    foreach (WordOlogy w in ArrWordology)
        //    ////    {

        //    ////        if (pn.Sense == w.Sense && w.Pos == "noun" && w.Word == pn_text.ToLower())
        //    ////        {
        //    ////            //FileStream allConceptsFile = new FileStream(_ontologyDirectoryPath + @"\AllConcepts.txt", FileMode.Open);
        //    ////            //StreamReader allConceptsFileReader = new StreamReader(allConceptsFile);
        //    ////            string concept = w.Concept;
        //    ////            string Conceptpath = OntologyDirectoryPath + @"\" + concept[0] + @"\" + concept + ".txt";
        //    ////            OntologyLibrary.OntoSem.Concept C = (OntologyLibrary.OntoSem.Concept)Onto_mo2a[concept];

        //    ////            Nf.Concept = C;
        //    ////            OntologyLibrary.OntoSem.PropertiesDictionary pd = Nf.Concept.FullProperties;


        //    ////            foreach (Property de in pd.Properties)
        //    ////            {

        //    ////                OntologyLibrary.OntoSem.FillerList OF = de.Fillers;
        //    ////                foreach (OntologyLibrary.OntoSem.Filler Ofiller in OF)
        //    ////                {

        //    ////                    if (de.Name == "IS-A")
        //    ////                    {
        //    ////                        string is_a = Ofiller.ConceptFiller.Name;
        //    ////                        Nf.IS_a = is_a;
        //    ////                    }

        //    ////                    for (int i = 0; i < Nf.Adjective.Count;i++ )
        //    ////                    {
        //    ////                        ParseNode pnn = Nf.Adjective[i];

        //    ////                        if (pnn.Text == Ofiller.ScalarFiller)
        //    ////                        {
        //    ////                            Nf.Adjective_fillerType[i] = de.Name.ToString();
        //    ////                        }
                                    
        //    ////                    }




        //    ////                }

        //    ////            }




        //    ////            Property maplexProperty = C.FullProperties["FILLER"];





        //    ////        }
        //    ////    }

        //    ////}
        //    #endregion
        //}
        SentenceParser dummy = new SentenceParser();
        MindMapOntology _ontology;
        public void MapMeaning()
        {
            _ontology = new MindMapOntology("1.owl");
            //LoadOntology();

            #region New Mapping Code
            int assigned = 0;
            foreach (NounFrame nf in Nounframes)
            {
                if (nf.Concept == null)
                {
                    if (AssignNFConcept(nf))
                        assigned++;
                }
                else
                    assigned++;

            }
            foreach (VerbFrame vf in VerbFrames)
            {
                if (vf.Concept == null)
                    if(AssignVFConcept(vf))
                        assigned++;
                else
                    assigned++;

            }
            float AssignmentPercent = (float)assigned / (Nounframes.Count + VerbFrames.Count);
            #endregion

        }

        public List<string> GetFramesConcepts(out int mapped,out int total)
        {
            int assigned = 0;
            List<string> retList = new List<string>();
            foreach (NounFrame nf in Nounframes)
            {
                AssignNFConcept(nf);
                string s = "NF("+nf.Text + "): \n";
                s += "Sense:"+nf.ParseNode.SenseNo +" "+nf.ParseNode.Sense;
                s += "\nConcept:";
                if (nf.Concept == null)
                {
                    s += "NULL";
                }
                else
                {
                    s = "* " + s;
                    s += nf.Concept.Name+":"+ nf.Concept.Defintion;
                    foreach (MyWordInfo mywi in nf.Concept.Maplex)
                    {
                        s += "\n" + mywi.ToString();
                    }
                    assigned++;
                }
                retList.Add(s);

            }
            foreach (VerbFrame vf in VerbFrames)
            {
                AssignVFConcept(vf);
                string s = "VF(" + vf.VerbName+ "): \n";
                s += "Sense:" + vf._parseNode.SenseNo + " " + vf._parseNode.Sense;
                s += "\nConcept:";
                if (vf.Concept == null)
                {
                    s += "NULL";
                }
                else
                {
                    s = "* " + s;
                    foreach (MyWordInfo mywi in vf.Concept.Maplex)
                    {
                        s += "\n" + mywi.ToString();
                    }
                    assigned++;
                }
                retList.Add(s);

            }
            float AssignmentPercent = (float)assigned / (Nounframes.Count + VerbFrames.Count);
            mapped = assigned;
            total = Nounframes.Count + VerbFrames.Count;
            return retList;
        }
        bool AssignNFConcept(NounFrame nf)
        {
            ParseNode pn = nf.ParseNode;
            string Sense = pn.Sense;
            int SenseNo = pn.SenseNo;
            string Word = nf.Text;
            SenseNo++;
            MyWordInfo myi = new MyWordInfo(Word, Wnlib.PartsOfSpeech.Noun);
            myi.Sense = SenseNo;

            //the pos is obtained from Wordology.....right?
            nf.Concept = MindMapMapper.GetConcept(myi, this._ontology);
            return nf.Concept != null;
        }

        bool AssignVFConcept(VerbFrame vf)
        {
            ParseNode pn = vf._parseNode;
            string Sense = pn.Sense;
            int SenseNo = pn.SenseNo;
            SenseNo++;
            try
            {
                string Word = (string)dummy.GetINFOfVerb(vf.VerbName)[0];
                if (Word.ToUpper() == "BECOME")
                {
                    if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "LORD")
                    {
                        Word += "CHANCELLOR";
                    }
                    else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "SAVILIAN")
                    {
                        Word += "PROFESSOR";
                    } else if (vf.CaseRoles[CaseRole.Theme].Count == 1)
                    {
                        if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "PROFESSOR")
                        {
                            Word += "PROFESSOR";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "PHYSICIAN")
                        {
                            Word += "PHYSICIAN";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "ARTIST")
                        {
                            Word += "ARTIST";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "CITIZEN")
                        {
                            Word += "CITIZEN";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "KING")
                        {
                            Word += "KING";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "LOVERS")
                        {
                            Word += "Lovers";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "REGENTS")
                        {
                            Word += "REGENTS";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "PROVINCE")
                        {
                            Word += "PROVINCE";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "FIGURE")
                        {
                            Word += "FIGURE";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "LAWYER")
                        {
                            Word += "LAWYER";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "PRESIDENT")
                        {
                            Word += "PRESIDENT";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "CAPITAL")
                        {
                            Word += "CAPTIAL";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "BLIND")
                        {
                            Word += "BLIND";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "LEADER")
                        {
                            Word += "LEADER";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "DIRECTOR")
                        {
                            Word += "DIRECTOR";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "DIRECTOR")
                        {
                            Word += "DIRECTOR";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "SURVEYOR")
                        {
                            Word += "SURVEYOR";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "COMMANDER")
                        {
                            Word += "COMMANDER";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "MEMBER")
                        {
                            Word += "MEMBER";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "MEMBER")
                        {
                            Word += "MEMBER";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "PUBLCATION")
                        {
                            Word += "PUBLCATION";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "SURGEON")
                        {
                            Word += "SURGEON";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "FELLOW")
                        {
                            Word += "FELLOW";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "SECRETARY_OF_STATE")
                        {
                            Word += "SECRETARY_OF_STATE";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "ADVISOR")
                        {
                            Word += "ADVISOR";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "MEMBER_OF_PARLIAMENT")
                        {
                            Word += "MEMBER_OF_PARLIAMENT";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "ASSISTANT")
                        {
                            Word += "ASSISTANT";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "LECTURER")
                        {
                            Word += "LECTURER";
                        }
                    }
                } else if (Word.ToUpper() == "MAKE")
                {
                    if (vf.CaseRoles[CaseRole.Theme].Count == 1)
                    {
                        if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "VICTORIES")
                        {
                            Word += "VICTORIES";
                        }
                    }

                }
                 else if (Word.ToUpper() == "BEGIN")
                {
                    if (vf.CaseRoles[CaseRole.Theme].Count == 1)
                    {
                        if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "RESEARCH")
                        {
                            Word += "RESEARCH";
                        }
                    }

                }

                else if (Word.ToUpper() == "COMMIT")
                {
                    if (vf.CaseRoles[CaseRole.Theme].Count == 1)
                    {
                        if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "CRIMES")
                        {
                            Word += "CRIME";
                        }
                        else if (vf.CaseRoles[CaseRole.Theme][0].Text.ToUpper() == "SUICIDE")
                        {
                            Word += "SUICIDE";
                        }
                    }
                    

                }
               
                MyWordInfo myi = new MyWordInfo(Word, Wnlib.PartsOfSpeech.Verb);
                //the pos is obtained from Wordology.....right?
                myi.Sense = SenseNo;
                vf.Concept = MindMapMapper.GetConcept(myi, this._ontology);
             
                
            }
            catch (Exception e) 
            {
                try
                {
                    string[] strList = vf.VerbName.Split('_');
                    string verb = strList[0];
                    string Word = (string)dummy.GetINFOfVerb(verb)[0] +"_"+ strList[1];
                    MyWordInfo myi = new MyWordInfo(Word, Wnlib.PartsOfSpeech.Verb);
                    //the pos is obtained from Wordology.....right?
                    myi.Sense = SenseNo;
                    vf.Concept = MindMapMapper.GetConcept(myi, this._ontology);
                }
                catch (Exception ex) { }

            }
            return vf.Concept != null;
           
        }

        

        private void GetWordologyList()
        {
            string _wordologyDirectoryPath =
              @"wordology\";

            _wordologyDirectoryPath = Application.ExecutablePath;
            int index = _wordologyDirectoryPath.LastIndexOf("\\");
            _wordologyDirectoryPath = _wordologyDirectoryPath.Substring(0, index);
            ArrayList strreader = new ArrayList();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(
                _wordologyDirectoryPath + @"\wordology.txt", FileMode.Open);
            StreamReader SR = new StreamReader(fs);
            ArrayList arr = new ArrayList();
            ArrWordology = (ArrayList)bf.Deserialize(fs);
            fs.Close();
        }
        /// <summary>
        ///for given predicate node ,it returns whether its verb is transitive or not
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CheckTransVerb(ParseNode node,ParseTree parsetree)
        {
            ParseNode TEMP=(ParseNode)node.Children[0];
            if(TEMP.Goal=="V")
            {
                ParseNode vnode = (ParseNode)node.Children[0];
                string s= SentenceParser.GetWordString(parsetree, vnode);
                vnode.Text = s;
                if (vnode.Text == "IS" || vnode.Text == "ARE" || vnode.Text == "WAS" || vnode.Text == "WERE" || vnode.Text == "AM")
                {
                    isflag = true;
                    return true;
                }
                
            }
            if (node.Children.Count == 1)
                return false;

            ParseNode cmpnode = (ParseNode)node.Children[1];
            if (cmpnode.Children != null)
            {
                ParseNode pn = (ParseNode)cmpnode.Children[0];
                if (pn.Goal == "SOBJ" || pn.Goal == "INF_PH" || pn.Goal == "OBJ")//INFPH is for Ali drinks water
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
            
           
           

        }
        public bool CheckPassiveVerb(ParseNode node)
        {
            ParseNode pn = null;
            ParseNode pn1 = null;
            if (node.Children.Count == 1)
                return false;
            ParseNode cmpnode = (ParseNode)node.Children[0];
            
            if(cmpnode.Children==null)
            {
                if (cmpnode.Goal == "V")
                {
                    ParseNode cmp_verb_pp = (ParseNode)node.Children[1];
                    ParseNode adjnode = (ParseNode)cmp_verb_pp.Children[0];
                    if (adjnode.Children != null)
                    {
                        ParseNode verb_pp = (ParseNode)adjnode.Children[0];

                        if (verb_pp.Goal == "VPSP")
                        {
                            return true;
                        }
                        else if (verb_pp.Goal == "NP")
                        {
                            ParseNode adjn=(ParseNode) verb_pp.Children[1];
                            if (adjn.Children != null)
                            {
                                ParseNode Par_phN = (ParseNode)adjn.Children[0];
                                ParseNode vpspn = (ParseNode)Par_phN.Children[0];
                                if (vpspn.Goal == "VPSP")
                                {
                                    return true;
                                }
                            }
                            
                            
                           
                        }
                        else if (verb_pp.Goal=="VINF")
                        {
                            ParseNode cmpn = (ParseNode)adjnode.Children[1];
                            ParseNode Adjn = (ParseNode)cmpn.Children[0];
                            ParseNode VPSPN = (ParseNode)Adjn.Children[0];
                            if (VPSPN.Goal == "VPSP")
                            {
                                return true ;

                            }
                            
                        }

                    }
                }
                return false;
            }


            if (cmpnode.Goal == "VP")
            {
                ParseNode cmp_verb_pp = (ParseNode)node.Children[1];
                ParseNode adjnode = (ParseNode)cmp_verb_pp.Children[0];
                
                if (adjnode.Goal == "ADJ")
                {
                    ParseNode passnode = (ParseNode)adjnode.Children[0];
                    if (passnode.Goal == "VPSP")
                    {
                        return true;
                    }
                }

            }
                
            
                pn = (ParseNode)cmpnode.Children[0];
                pn1 = (ParseNode)cmpnode.Children[1];
            
         
            
            if (cmpnode.Children.Count>2)
            {
                ParseNode pn2 = (ParseNode)cmpnode.Children[2];
                if(pn.Goal == "BE1" && pn1.Goal == "BING" && pn2.Goal == "VPSP"||pn.Goal=="HAV1"&&pn1.Goal=="BEN"&&pn2.Goal=="VPSP")
                {
                    return true;
                }
                else
                {
                    if(pn.Goal == "AUX_V" && pn1.Goal == "BE" && pn2.Goal == "VPSP")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                
            }
            if (pn.Goal == "BE1" && pn1.Goal == "VPSP")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        List<NounFrame> _currentSubjects;

        public void BuildNodeTMR(ParseTree parsetree, ParseNode node, int nodeindex)
        {
           bool still = false;


           switch (node.Goal)
           {

               case "S":
                   if(node.Children.Count==1 && ((ParseNode)node.Children[0]).Goal == "SS")
                       GET_SS((ParseNode)node.Children[0], parsetree);
                   else if (node.Children.Count == 1 && ((ParseNode)node.Children[0]).Goal == "CS")
                       BuildNodeTMR(parsetree, (ParseNode)node.Children[0], -1);
                   //if (node.Children != null)
                   //{
                   //    int i = 0;
                   //    foreach (ParseNode n in node.Children)
                   //    {

                   //        BuildNodeTMR(parsetree, n, node.Start + i);
                   //        i++;
                   //    }

                   //}
                   break;

               case "SS":
                   GET_SS(node, parsetree);

                   //if (node.Children != null)
                   //{
                   //    int i = 0;
                   //    foreach (ParseNode n in node.Children)
                   //    {

                   //        BuildNodeTMR(parsetree, n, node.Start + i);
                   //        i++;
                   //    }

                   //}
                   break;
               case "CS":

                   if (node.Children != null)
                   {
                       int i = 0;
                       foreach (ParseNode n in node.Children)
                       {

                           BuildNodeTMR(parsetree, n, node.Start + i);
                           i++;
                       }

                   }
                   break;
               case "DS":
                   
                   Get_DS(node, parsetree);
                   //Get_DS(node, parsetree,ver);
                   //GET_PPN(Node);

                   //if (node.Children != null)
                   //{
                   //    int i = 0;
                   //    foreach (ParseNode n in node.Children)
                   //    {
                   //        // ParseNode parsenode1 = (ParseNode)node.Children[0];
                   //        BuildNodeTMR(parsetree, n, node.Start + i);
                   //        i++;
                   //    }

                   //}
                   break;
               case "PRD_LST":
                   if (node.Children != null)
                   {
                       int i = 0;
                       foreach (ParseNode n in node.Children)
                       {
                           // ParseNode parsenode1 = (ParseNode)node.Children[0];
                           BuildNodeTMR(parsetree, n, node.Start + i);
                           i++;
                       }

                   }
                   break;


               case "CMP_LST":


                   if (node.Children != null)
                   {
                       int i = 0;
                       foreach (ParseNode n in node.Children)
                       {
                           // ParseNode parsenode1 = (ParseNode)node.Children[0];
                           BuildNodeTMR(parsetree, n, node.Start + i);
                           i++;
                       }

                   }
                   break;
               case "SBJ":

                   currentsubjects = GetSubjects(parsetree, node);

                   break;


               case "SSBJ":

                   currentsubjects = GetSubjects(parsetree, node);


                   break;

                 
               case "PRD":
                   
                      
                       VerbFrame _Verb;
                       List<ParseNode> CurrentAdverbs=new List<ParseNode>();
                       isflag = false;
                       if (node.Children != null)
                       {
                           //ParseNode pn =(ParseNode) node.Children[1];
                           bool trans = CheckTransVerb(node, parsetree);
                           bool passive = CheckPassiveVerb(node);
                           if (passive == true)// sara add it
                           {
                               trans = true;
                           }
                           int i = 0;
                           string verbname;

                           foreach (ParseNode n in node.Children)
                           {
                               //if (pncmplst.Goal!=null)//repeat adding caseroles
                               //{
                               //   n.Goal = pncmplst.Goal;
                               //}
                               if (n.Goal == "V")
                               {
                                   if (Sor == false && Cor == false)
                                   {
                                       if (passive == true)
                                       {
                                           string s = SentenceParser.GetWordString(parsetree, n);
                                           if (s == "IS" || s == "ARE" || s == "WAS" || s == "WERE")
                                           {
                                               ParseNode cmpn = (ParseNode)node.Children[1];
                                               ParseNode adjn = (ParseNode)cmpn.Children[0];
                                               ParseNode VPSPn = (ParseNode)adjn.Children[0];
                                               if (VPSPn.Goal == "VPSP")
                                               {

                                                   currentVerbFrame = new VerbFrame(VPSPn);
                                                   verbname = SentenceParser.GetWordString(parsetree, VPSPn);
                                                   currentVerbFrame.VerbName = verbname;
                                                   AddVerbrame(currentVerbFrame);
                                               }
                                           }
                                           else if (s == "WILL")
                                           {
                                               ParseNode cmpn =  (ParseNode)node.Children[1];
                                               ParseNode SOBJn = (ParseNode)cmpn.Children[0];
                                               if (SOBJn.Goal == "SOBJ")
                                               {
                                                   ParseNode NPn = (ParseNode)SOBJn.Children[0];
                                                   ParseNode adjN = (ParseNode)NPn.Children[1];
                                                   ParseNode par_ph = (ParseNode)adjN.Children[0];
                                                   ParseNode VPSPN = (ParseNode)par_ph.Children[0];
                                                   if (VPSPN.Goal == "VPSP")
                                                   {
                                                       currentVerbFrame = new VerbFrame(VPSPN);
                                                       verbname = SentenceParser.GetWordString(parsetree, VPSPN);
                                                       currentVerbFrame.VerbName = verbname;
                                                       AddVerbrame(currentVerbFrame);
                                                   }
                                               }
                                               else if (SOBJn.Goal == "INF_PH")
                                               {
                                                   ParseNode cmpnn = (ParseNode)SOBJn.Children[1];
                                                   ParseNode adjn = (ParseNode)cmpnn.Children[0];
                                                   ParseNode VPSPNN = (ParseNode)adjn.Children[0];
                                                   if (VPSPNN.Goal == "VPSP")
                                                   {
                                                       currentVerbFrame = new VerbFrame(VPSPNN);
                                                       verbname = SentenceParser.GetWordString(parsetree, VPSPNN);
                                                       currentVerbFrame.VerbName = verbname;
                                                       AddVerbrame(currentVerbFrame);
                                                   }


                                               }
                                               

                                           }

                                       }
                                       else //active
                                       {
                                           string nwill=SentenceParser.GetWordString(parsetree,n);

                                           if (nwill == "WILL")
                                           {
                                              ParseNode CMPn=(ParseNode)node.Children[1];
                                              ParseNode VINFpn = (ParseNode)CMPn.Children[0];
                                              ParseNode VINFn = (ParseNode)VINFpn.Children[0];
                                              if (VINFn.Goal == "VINF")
                                              {
                                                  currentVerbFrame = new VerbFrame(VINFn);
                                                  verbname = SentenceParser.GetWordString(parsetree, VINFn);
                                                  currentVerbFrame.VerbName = verbname;
                                                  AddVerbrame(currentVerbFrame);
                                              }
                                              else if (VINFn.Goal == "VING")
                                              {
                                                  currentVerbFrame = new VerbFrame(VINFn);
                                                  verbname = SentenceParser.GetWordString(parsetree, VINFn);
                                                  currentVerbFrame.VerbName = verbname;
                                                  AddVerbrame(currentVerbFrame);
                                              }
                                             


                                           }
                                           else
                                           {
                                               currentVerbFrame = new VerbFrame(n);
                                               verbname = SentenceParser.GetWordString(parsetree, n);
                                               currentVerbFrame.VerbName = verbname;
                                               AddVerbrame(currentVerbFrame);

                                           }
                                         
                                       }
                                   }
                                   else
                                   {
                                       
                                       FillVerbAccordingToProposition(n, parsetree, passive);

                                   }
                               }
                               if (n.Goal == "VP")
                               {
                                   if (passive == true)
                                   {
                                       if(n.Children.Count<=2)
                                       {
                                           ParseNode pnode = (ParseNode)node.Children[1];//CMP
                                           ParseNode Adjn = (ParseNode)pnode.Children[0];
                                           ParseNode Vpspn = (ParseNode)Adjn.Children[0];
                                           if (Vpspn.Goal == "VPSP")
                                           {
                                               if (Sor == false && Cor == false)
                                               {
                                                   currentVerbFrame = new VerbFrame(Vpspn);
                                                   verbname = SentenceParser.GetWordString(parsetree, Vpspn);
                                                   currentVerbFrame.VerbName = verbname;
                                                   AddVerbrame(currentVerbFrame);
                                               }
                                               else
                                               {
                                                   FillRelations(parsetree, Vpspn);
                                               }
                                           }
                                           else
                                           {
                                               if (Sor == false && Cor == false)
                                               {
                                                   ParseNode vn = (ParseNode)n.Children[1];
                                                   currentVerbFrame = new VerbFrame(vn);
                                                   verbname = SentenceParser.GetWordString(parsetree, vn);
                                                   currentVerbFrame.VerbName = verbname;
                                                   AddVerbrame(currentVerbFrame);
                                               }
                                               else
                                               {
                                                   if (Vpspn.Goal == "VPSP")
                                                   {
                                                       FillRelations(parsetree, Vpspn);
                                                   }
                                                   else
                                                   {
                                                       ParseNode vn = (ParseNode)n.Children[1];
                                                       FillRelations(parsetree, vn);
                                                   }

                                               }
                                               
                                           }

                                       }
                                       else
                                       {
                                           ParseNode vn = (ParseNode)n.Children[2];
                                           currentVerbFrame = new VerbFrame(vn);
                                           verbname = SentenceParser.GetWordString(parsetree, vn);
                                           currentVerbFrame.VerbName = verbname;
                                           AddVerbrame(currentVerbFrame);
                                       }
                                       


                                   }
                                   else//active
                                   {
                                       ParseNode pnode = (ParseNode)n.Children[1];
                                       if (pnode.Goal == "VING")//is doing
                                       {
                                           currentVerbFrame = new VerbFrame(pnode);
                                           verbname = SentenceParser.GetWordString(parsetree, pnode);
                                           currentVerbFrame.VerbName = verbname;
                                           AddVerbrame(currentVerbFrame);

                                       }
                                       else if (pnode.Goal == "VPSP")//has eaten
                                       {
                                           if (Sor == true)
                                           {
                                               FillRelations(parsetree, pnode);

                                           }
                                           else
                                           {
                                               currentVerbFrame = new VerbFrame(pnode);
                                               verbname = SentenceParser.GetWordString(parsetree, pnode);
                                               currentVerbFrame.VerbName = verbname;
                                               AddVerbrame(currentVerbFrame);
                                           }
                                           
                                       }
                                       else if (pnode.Goal == "VINF")//3yzen ne3rf zy 2lle fo2eha walla l2 :D:D:D
                                       {
                                           currentVerbFrame = new VerbFrame(pnode);
                                           verbname = SentenceParser.GetWordString(parsetree, pnode);
                                           currentVerbFrame.VerbName = verbname;
                                           AddVerbrame(currentVerbFrame);
                                       }





                                   }
                                   
                                   
                               }

                               if (n.Goal == "CMP_LST")
                               {
                                   //if (pncmplst.Goal != null)
                                   //{
                                   //    n.Goal = pncmplst.Goal;
                                   //    n.Children = pncmplst.Children;
                                   //}
                                   foreach (ParseNode pe in n.Children)
                                   {
                                       

                                       if (pe.Goal == "CMP")
                                       {
                                           ParseNode N_obj = (ParseNode)pe.Children[0];
                                           if (N_obj.Goal == "SOBJ" || N_obj.Goal == "OBJ")
                                           {
                                               currentObjects = GetObjects(N_obj,parsetree,passive);
                                           }
                                           if (N_obj.Goal == "ADV_LST")
                                           {
                                               foreach(ParseNode p1 in N_obj.Children )
                                               {
                                                   if(p1.Goal=="ADV")
                                                   {
                                                       string s = SentenceParser.GetWordString(parsetree, p1);
                                                       p1.Text = s;

                                                       CurrentAdverbs.Add(p1);
                                                   }
                                               }
                                           }
                                           else if (N_obj.Goal == "ADV")
                                           {
                                               string s = SentenceParser.GetWordString(parsetree,N_obj);
                                               N_obj.Text = s;

                                               CurrentAdverbs.Add(N_obj);
                                           }

                                           //currentVerbFrame.Adverb = CurrentAdverbs;
                                           if (N_obj.Goal == "PREP_PH")
                                           {
                                               FillVerbAccordingToProposition(N_obj, parsetree, passive);
                                           }
                                           if(N_obj.Goal=="INF_PH")
                                           {
                                               FillVerbAccordingToProposition(N_obj,parsetree,passive);
                                           }



                                       }
                                   }
                               }


                               if (n.Goal == "CMP")
                               {
                                   ParseNode N_obj = (ParseNode)n.Children[0];
                                   if (N_obj.Goal == "SOBJ" || N_obj.Goal == "OBJ")
                                   {
                                       currentObjects = GetObjects(N_obj,parsetree,passive);
                                   }
                                   if (N_obj.Goal == "ADV_LST")
                                   {
                                       foreach (ParseNode p1 in N_obj.Children)
                                       {
                                           if (p1.Goal == "ADV")
                                           {
                                               string s = SentenceParser.GetWordString(parsetree, p1);
                                               p1.Text = s;

                                               CurrentAdverbs.Add(p1);
                                           }
                                       }
                                   }
                                   else if (N_obj.Goal == "ADV")
                                   {
                                       string s = SentenceParser.GetWordString(parsetree, N_obj);
                                       N_obj.Text = s;

                                       CurrentAdverbs.Add(N_obj);
                                   }

                                   //currentVerbFrame.Adverb = CurrentAdverbs;

                                   if (N_obj.Goal == "PREP_PH")
                                   {
                                       FillVerbAccordingToProposition(N_obj, parsetree, passive);
                                   }
                                   if (N_obj.Goal == "INF_PH")
                                   {
                                       FillVerbAccordingToProposition(N_obj, parsetree, passive);
                                   }
                                   
                                   //if (N_obj.Goal == "INF_PH")
                                   //{
                                   //    pncmplst =(ParseNode) N_obj.Children[1];
                                   //    n.Goal = pncmplst.Goal;
                                   //    goto case "PRD";
                                   //   // BuildNodeTMR(parsetree, pncmplst, node.Start + i);
                                   //   // i++;
                                       
                                   //}


                                   //if (passive == false)
                                   //   {
                                   //       if (trans == true)
                                   //       {
                                   //           foreach (NounFrame nff in currentsubjects)
                                   //           {

                                   //               currentVerbFrame.AddCaseRole(CaseRole.Agent, nff);

                                   //           }

                                   //       }
                                   //       else
                                   //       {
                                   //           if (currentVerbFrame.VerbName == "is" | currentVerbFrame.VerbName == "are" | currentVerbFrame.VerbName == "am" | currentVerbFrame.VerbName == "was" | currentVerbFrame.VerbName == "were")
                                   //           {
                                   //               foreach (NounFrame nounf in currentsubjects)
                                   //               {
                                   //                   ParseNode nf = nounf.ParseNode;
                                   //                   if (nf.Children != null)
                                   //                   {
                                   //                       ParseNode eNP = (ParseNode)nf.Children[0];
                                   //                       ParseNode eP = (ParseNode)eNP.Children[0];
                                   //                       ParseNode ePRO_NOUN = (ParseNode)eP.Children[0];

                                   //                       currentVerbFrame.AddCaseRole(CaseRole.Theme, nounf);

                                   //                   }
                                   //               }
                                   //           }

                                   //       }

                                   //   }
                               }


                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }
                           if (passive == false)
                           {
                               if (trans == true)
                               {
                                   if (currentVerbFrame.VerbName == "IS" || currentVerbFrame.VerbName == "ARE" || currentVerbFrame.VerbName == "WAS" || currentVerbFrame.VerbName == "WERE")
                                   {
                                       if (isflag == true)
                                       {
                                           foreach (NounFrame nff in currentObjects)
                                           {

                                               currentVerbFrame.AddCaseRole(CaseRole.Cotheme, nff);
                                               ParseNode pppn = nff.ParseNode;
                                               string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
                                               ss = nff.Text;



                                           }
                                       }
                                       
                                   }
                                   foreach (NounFrame nff in currentsubjects)
                                   {
                                       if (isflag == false)
                                       {
                                           currentVerbFrame.AddCaseRole(CaseRole.Agent, nff);
                                           ParseNode pppn = nff.ParseNode;
                                           string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
                                           ss = nff.Text;
                                       }
                                       else
                                       {
                                           currentVerbFrame.AddCaseRole(CaseRole.Theme, nff);
                                           ParseNode pppn = nff.ParseNode;
                                           string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
                                           ss = nff.Text;
                                       }
                                       

                                   }
                                   foreach (NounFrame nff in currentObjects)
                                   {
                                       if (isflag == false)
                                       {

                                           currentVerbFrame.AddCaseRole(CaseRole.Theme, nff);
                                           ParseNode pppn = nff.ParseNode;
                                           string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
                                           ss = nff.Text;
                                       }



                                   }
                                   //if (currentVerbFrame.VerbName == "IS" | currentVerbFrame.VerbName == "ARE" | currentVerbFrame.VerbName == "AM" | currentVerbFrame.VerbName == "WAS" | currentVerbFrame.VerbName == "WERE")
                                   //{
                                   //    foreach (NounFrame nounf in currentsubjects)
                                   //    {
                                   //        ParseNode nf = nounf.ParseNode;
                                   //        if (nf.Children != null)
                                   //        {
                                   //            ParseNode eNP = (ParseNode)nf.Children[0];
                                   //            ParseNode eP = (ParseNode)eNP.Children[0];
                                   //            ParseNode ePRO_NOUN = (ParseNode)eP.Children[0];

                                   //            currentVerbFrame.AddCaseRole(CaseRole.Theme, nounf);
                                   //            ParseNode pppn = nounf.ParseNode;
                                   //            string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
                                   //            ss = nounf.Text;




                                   //        }
                                   //    }
                                   //}



                               }
                               else
                               {
                                   foreach (NounFrame nfe in currentsubjects)
                                   {
                                       currentVerbFrame.AddCaseRole(CaseRole.Theme, nfe);

                                       ParseNode pppn = nfe.ParseNode;
                                       string ss = SentenceParser.GetWordString(nfe.Parsetree, pppn);//nff.text=ali after it be null
                                       ss = nfe.Text;


                                   }

                               }

                           }
                           else //passive & trans
                           {
                               foreach(NounFrame e in currentsubjects )
                               {
                                   currentVerbFrame.AddCaseRole(CaseRole.Theme, e);

                                   ParseNode pppn = e.ParseNode;
                                   string ss = SentenceParser.GetWordString(parsetree, pppn);//nff.text=ali after it be null
                                   ss = e.Text;
                               }


                           }
                       }
                  break;

               case "CMP":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                   
                   break;
               case "N_CL":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                   
                   break;
               case "SOR":

                   if (node.Children != null)
                   {
                       int i = 0;
                       foreach (ParseNode n in node.Children)
                       {
                           // ParseNode parsenode1 = (ParseNode)node.Children[0];
                           BuildNodeTMR(parsetree, n, node.Start + i);
                           i++;
                       }

                   }
                   Sorname = SentenceParser.GetWordString(parsetree, node);
                   Sor = true;
                  

                   break;
               case "HAV1":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                   
                   break;

               case "VPSP":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                   
                   break;
               case "VINF":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                   
                   break;

               case "INF_PH":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                   
                   break;
               case "BE1":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                   
                   break;
               case "PREP_PH":
                   

                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }
                           string fillertype = prepositionOnto.DeduceFillerType(w, argumenttype);


                       }
                   

                   break;

               case "SOBJ":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }

                   
                   break;
               case "NP":
                   

                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                   
                   break;
               case"COR":
                   if (node.Children != null)
                   {
                       int i = 0;
                       foreach (ParseNode n in node.Children)
                       {
                           // ParseNode parsenode1 = (ParseNode)node.Children[0];
                           BuildNodeTMR(parsetree, n, node.Start + i);
                           i++;
                       }

                   }
                   Corname = SentenceParser.GetWordString(parsetree, node);

                   Cor = true;
                   break;
               case "N":
                   
                       argumenttype = ArgumentType.Noun;

                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }



                   
                   break;
               case "PRO_N":
                   
                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }


                       }
                       else
                       {
                           return;
                       }


                   
                   break;

               case "PRS":
                   
                       //w = parsetree.Words[node.Start].ToString();
                       //currentVerbFrame.VerbName = parsetree.Words[node.Start].ToString();
                       //if (w == "To")
                       //{
                       //    currentVerbFrame.GetDomainRelation(DomainRelationType.Reason);
                       //}

                       if (node.Children != null)
                       {
                           int i = 0;
                           foreach (ParseNode n in node.Children)
                           {
                               // ParseNode parsenode1 = (ParseNode)node.Children[0];
                               BuildNodeTMR(parsetree, n, node.Start + i);
                               i++;
                           }

                       }
                       Prs = true;
                   break;
           }
                   
           
        }
        
       
    
        private List<NounFrame> GetObjects(ParseNode node,ParseTree parseTree,bool passive)
        {
            List<NounFrame> nfs = new List<NounFrame>();
            List<ParseNode> currentAdjectives = new List<ParseNode>();
            if (node.Goal == "OBJ")
            {
                foreach (ParseNode pn in node.Children)
                {
                    if (pn.Goal == "SOBJ")
                    {
                        //string name = pn.Text;
                        //nfs.Add(new NounFrame(pn));

                        ParseNode pnn = (ParseNode)pn.Children[0];
                        
                        foreach (ParseNode ppn in pnn.Children)
                        {

                            if (ppn.Goal == "N")
                            {
                                CurrentNounFrame = new NounFrame(ppn,parseTree);
                                nfs.Add(CurrentNounFrame);
                                AddNounFrame(CurrentNounFrame);
                            }

                        }
                        foreach (ParseNode p2 in pnn.Children)
                        {
                            if (p2.Goal == "ADJ_LST")
                            {
                                
                                foreach (ParseNode p3 in p2.Children)
                                {
                                    if (p3.Goal == "ADJ")
                                    {
                                        string s = SentenceParser.GetWordString(parseTree, p3);
                                        p3.Text = s;
                                        currentAdjectives.Add(p3);



                                    }
                                }
                            }
                            else if (p2.Goal == "ADJ")
                            {
                                string s = SentenceParser.GetWordString(parseTree, p2);
                                p2.Text = s;
                                currentAdjectives.Add(p2);
                                //NounFrame n1 = new NounFrame(p2, parseTree);
                                //AddNounFrame(n1);
                            }

                        }
                        if (currentAdjectives.Count != null)
                        {
                            CurrentNounFrame.Adjective = currentAdjectives;
                        }
                    }




                }

            }
            else//node.Goal == "SSBJ"
            {
                if (node.Children != null)
                {
                    ParseNode pnn = (ParseNode)node.Children[0];
                    foreach (ParseNode ppn in pnn.Children)
                    {

                        if (ppn.Goal == "N")
                        {
                            CurrentNounFrame = new NounFrame(ppn, parseTree);
                            nfs.Add(CurrentNounFrame);
                            AddNounFrame(CurrentNounFrame);
                        }

                    }
                    foreach (ParseNode p2 in pnn.Children)
                    {
                        if (p2.Goal == "ADJ_LST")
                        {
                            foreach (ParseNode p3 in p2.Children)
                            {
                                if(p3.Goal=="ADJ")
                                {
                                    if (p3.Children != null)
                                    {
                                        ParseNode p4 = (ParseNode)p3.Children[0];
                                        if (p4.Goal == "PREP_PH")
                                        {
                                            ParseNode prsnode = (ParseNode)p4.Children[0];
                                            if (prsnode.Goal == "PRS")
                                            {
                                                string ss2 = SentenceParser.GetWordString(parseTree, prsnode);
                                                if (ss2 == "FROM")
                                                {
                                                    fromprep = true;
                                                    FillVerbAccordingToProposition(p4, parseTree, passive);
                                                }
                                            }

                                        }
                                        if (fromprep == true)
                                        {
                                            if (p4.Goal == "PAR_PH")
                                            {

                                                FillVerbAccordingToProposition(p4, parseTree, passive);
                                                fromprep = false;

                                            }
                                        }

                                    }
                                    else
                                    {
                                        string s = SentenceParser.GetWordString(parseTree, p3);
                                        p3.Text = s;
                                        currentAdjectives.Add(p3);
                                        //NounFrame nj = new NounFrame(p3, parseTree);
                                        //AddNounFrame(nj);


                                    }
                                }
                                
                            }
                        }
                        else if (p2.Goal == "ADJ")
                        {

                            if (p2.Children != null)
                            {
                                foreach (ParseNode n in p2.Children)
                                {
                                    if (n.Goal == "PREP_PH")
                                    {
                                        FillVerbAccordingToProposition(n, parseTree, passive);
                                    }
                                }
                            }
                            else
                            {
                                string s = SentenceParser.GetWordString(parseTree, p2);
                                p2.Text = s;
                                currentAdjectives.Add(p2);
                                //NounFrame nj = new NounFrame(p2, parseTree);
                                //AddNounFrame(nj);
                            }
                        }

                    }
                    if (currentAdjectives.Count != null)
                    {
                        CurrentNounFrame.Adjective = currentAdjectives;
                    }
                }


                



                
                //NounFrame nf = new NounFrame(node);

                //nfs.Add(nf);

            }
            return nfs;

        }
        private List<NounFrame> GetSubjects(ParseTree parseTree,ParseNode node)
        {
            List<NounFrame> nfs = new List<NounFrame>();
            List<ParseNode> currentAdjectives = new List<ParseNode>();
            if (node.Goal == "SBJ")
            {
                foreach (ParseNode pn in node.Children)
                {
                    if (pn.Goal == "SSBJ")
                    {
                        //string name = pn.Text;
                        //nfs.Add(new NounFrame(pn));

                        ParseNode pnn =(ParseNode) pn.Children[0];
                        foreach (ParseNode ppn in pnn.Children)
                        {
                            
                            if (ppn.Goal == "N")
                            {
                               CurrentNounFrame = new NounFrame(ppn,parseTree);
                                nfs.Add(CurrentNounFrame);
                                AddNounFrame(CurrentNounFrame);
                            }
                            
                           
                        }

                        foreach (ParseNode p2 in pnn.Children)
                        {
                            if (p2.Goal == "ADJ_LST")
                            {
                                foreach (ParseNode p3 in p2.Children)
                                {
                                    if (p3.Goal == "ADJ")
                                    {
                                        string s = SentenceParser.GetWordString(parseTree, p3);
                                        p3.Text = s;
                                        currentAdjectives.Add(p3);
                                        //NounFrame n1 = new NounFrame(p3, parseTree);
                                        //AddNounFrame(n1);


                                    }
                                }
                            }
                            else if (p2.Goal == "ADJ")
                            {
                                string s = SentenceParser.GetWordString(parseTree, p2);
                                p2.Text = s;
                                currentAdjectives.Add(p2);
                                //NounFrame n1 = new NounFrame(p2, parseTree);
                                //AddNounFrame(n1);

                            }

                        }
                        if (currentAdjectives.Count != null)
                        {
                            CurrentNounFrame.Adjective = currentAdjectives;
                        }
                    }
                    
                        
                        

                }
            }
            else//node.Goal == "SSBJ"
            {
                ParseNode pnn = (ParseNode)node.Children[0];//node.goal=="NP"
                ParseNode mynode = new ParseNode();
                                
                foreach (ParseNode ppn in pnn.Children)
                {
                    

                    if (ppn.Goal == "N")
                    {
                        CurrentNounFrame = new NounFrame(ppn,parseTree);
                        nfs.Add(CurrentNounFrame);
                        AddNounFrame(CurrentNounFrame);
                    }

                    else if (ppn.Goal == "NPP")
                    {




                        if (ppn.ReferedAnaphoraNode != null)
                        {
                            // mynode = p;
                            ModifyRoot(new ArrayList(), ref ppn.ReferedAnaphoraNode);
                        }
                        if (ppn.ReferedAnaphoraNode.Goal == "SBJ")
                        {
                            foreach (ParseNode pn in ppn.ReferedAnaphoraNode.Children)
                            {
                                if (pn.Goal == "SSBJ")
                                {
                                    //string name = pn.Text;
                                    //nfs.Add(new NounFrame(pn));

                                    pnn = (ParseNode)pn.Children[0];
                                    foreach (ParseNode ppn1 in pnn.Children)
                                    {

                                        if (ppn1.Goal == "N")
                                        {


                                            foreach (NounFrame n1 in Nounframes)
                                            {

                                                string ppn1Text = SentenceParser.GetWordString(n1.Parsetree, ppn1);

                                                ParseNode pn1 = n1.ParseNode;
                                                
                                                if (n1.Text == ppn1Text)
                                                {
                                                    nfs.Add(n1);
                                                    break;
                                                }
                                               

                                            }
                                        }


                                    }

                                }




                            }
                        }
                        else //if (ppn.ReferedAnaphoraNode.Goal == "SSBJ")//SSBJ
                        {
                         
                                    ParseNode p_n= (ParseNode)ppn.ReferedAnaphoraNode.Parent;
                                    foreach (ParseNode ppn1 in p_n.Children)
                                    {

                                        if (ppn1.Goal == "N")
                                        {


                                            foreach (NounFrame n1 in Nounframes)
                                            {

                                                string ppn1Text = SentenceParser.GetWordString(n1.Parsetree, ppn1);

                                                ParseNode pn1 = n1.ParseNode;

                                                if (n1.Text == ppn1Text)
                                                {
                                                    nfs.Add(n1);
                                                    break;
                                                }


                                            }
                                        }


                                    

                                }




                            
                        }

                        

                    }
                    
                }

                foreach (ParseNode p2 in pnn.Children)
                {
                    if (p2.Goal == "ADJ_LST")
                    {
                        foreach(ParseNode p3 in p2.Children  )
                        {
                          if (p3.Goal == "ADJ")
                           {
                               string s = SentenceParser.GetWordString(parseTree, p3);
                               p3.Text = s;
                               currentAdjectives.Add(p3);
                               //NounFrame nj = new NounFrame(p3, parseTree);
                               //AddNounFrame(nj);


                           }
                        }
                    }
                    else if (p2.Goal == "ADJ")
                    {
                        string s = SentenceParser.GetWordString(parseTree, p2);
                        p2.Text = s;
                        currentAdjectives.Add(p2);
                        //NounFrame nj = new NounFrame(p2, parseTree);
                        //AddNounFrame(nj);
                    }

                }
                if(currentAdjectives.Count!=0)
                {
                    CurrentNounFrame.Adjective = currentAdjectives;
                }
                


                //NounFrame nf =  new NounFrame(node);

                //nfs.Add(nf);

            }
            return nfs;
            //throw new Exception("The method or operation is not implemented.");
        }

        private void FillRelations(ParseTree parsetree, ParseNode pnode)
        {
            if (Sorname == "AFTER")
            {
                string s = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfafter = new VerbFrame(pnode);
                vfafter.VerbName = s;
                AddVerbrame(vfafter);
                currentVerbFrame.AddTemporalRelation(TemporalRelationType.Before, vfafter);
                //currentVerbFrame.AddTemporalRelation(TemporalRelationType.Before, currentVerbFrame);
            }
            if (Sorname == "SO_THAT")
            {
                string sss = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfreason = new VerbFrame(pnode);
                vfreason.VerbName = sss;
                AddVerbrame(vfreason);
                currentVerbFrame.AddDomainRelation(DomainRelationType.Reason, vfreason);
            }
            if (Sorname == "BECAUSE")
            {
                string sss = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfreason = new VerbFrame(pnode);
                vfreason.VerbName = sss;
                AddVerbrame(vfreason);
                currentVerbFrame.AddDomainRelation(DomainRelationType.Reason, vfreason);
            }
            if (Sorname == "THAT" && WordStringafter == "AFTER")
            {
                string s11 = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfafter = new VerbFrame(pnode);
                vfafter.VerbName = s11;
                AddVerbrame(vfafter);
                currentVerbFrame.AddTemporalRelation(TemporalRelationType.After, vfafter);
                // currentVerbFrame.AddTemporalRelation(TemporalRelationType.Before, currentVerbFrame);
            }
            if (Sorname == "THAT" && WordStringbefore == "BEFORE")
            {
                string s22 = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfbefore = new VerbFrame(pnode);
                vfbefore.VerbName = s22;
                AddVerbrame(vfbefore);
                currentVerbFrame.AddTemporalRelation(TemporalRelationType.Before, vfbefore);
                currentVerbFrame.AddTemporalRelation(TemporalRelationType.After, currentVerbFrame);
            }
        }

        private void FillVerbAccordingToProposition(ParseNode n, ParseTree parsetree,bool passive)
        {
            string ft1 = "";
            string ft2 = "";
            List<ParseNode> currentAdjectives = new List<ParseNode>();
            k = false;
            /*ParseNode prepphnode = (ParseNode)n.Children[0];
            if (prepphnode.Goal == "CMP")
            {
                prepphnode = (ParseNode)prepphnode.Children[0];
            }*/
            //to (verb) not handled bec the parser  cant parse it
            if (n.Goal == "PAR_PH")
            {

                ParseNode pnSource = (ParseNode)n.Children[0];
                string WordString = SentenceParser.GetWordString(parsetree, pnSource);
                // ft1 = prepositionOnto.DeduceFillerType(passive, WordString, ArgumentType.Noun);


                NounFrame nfSource = new NounFrame(pnSource, parsetree);
                AddNounFrame(nfSource);
                currentVerbFrame.AddCaseRole(CaseRole.Source, nfSource);




                ParseNode cmpnode = (ParseNode)n.Children[1];
                ParseNode sobjnode = (ParseNode)cmpnode.Children[0];
                ParseNode npnode = (ParseNode)sobjnode.Children[0];
                foreach (ParseNode pp in npnode.Children)
                {
                    if (pp.Goal == "N")
                    {
                        // ParseNode pnSource1 = (ParseNode)pp.Children[0];
                        NounFrame nfSource1 = new NounFrame(pp, parsetree);
                        _n_f = nfSource1;
                        AddNounFrame(_n_f);
                        currentVerbFrame.AddCaseRole(CaseRole.Source, nfSource1);
                        
                        
                    }
                }
                foreach (ParseNode pp in npnode.Children)
                {
                    if (pp.Goal == "ADJ")
                    {
                        // ParseNode pnSource1 = (ParseNode)pp.Children[0];
                        NounFrame nfSource1 = new NounFrame(pp, parsetree);
                        //AddNounFrame(nfSource1);
                        string s = SentenceParser.GetWordString(parsetree, pp);
                        pp.Text = s;
                        currentAdjectives.Add(pp);
                        _n_f.Adjective = currentAdjectives;
                        //currentVerbFrame.AddCaseRole(CaseRole.Source, nfSource1);

                    }
                }

            }
                if (n.Goal == "PREP_PH")
                {
                    ParseNode pn =(ParseNode) n.Children[0];
                    string WordString = SentenceParser.GetWordString(parsetree, pn);
                    if(WordString=="IN" )
                    {
                        
                        ParseNode NSOBJ = (ParseNode)n.Children[1];
                        ParseNode NNP = (ParseNode)NSOBJ.Children[0];
                        ParseNode NN1 = (ParseNode)NNP.Children[0];
                        string years = SentenceParser.GetWordString(parsetree, NN1);
                        if (years[0].ToString() == "0" || years[0].ToString() == "1" || years[0].ToString() == "2" || years[0].ToString() == "3" || years[0].ToString()== "4" || years[0].ToString() == "5" || years[0].ToString() == "6" || years[0].ToString() == "7" || years[0].ToString() == "8" || years[0].ToString() == "9")
                        {
                            k = true;
                            NounFrame nftime = new NounFrame(NN1, parsetree);
                            //AddNounFrame(nftime);
                            currentVerbFrame.Aspect.Duration.ActionTime = years;
                        }
                             
                            
                        
                    }

                    if (WordString == "AFTER")
                    {
                        WordStringafter = WordString;
                    }
                    if (WordString == "BEFORE")
                    {
                        WordStringbefore = WordString;
                    }

                    ft1 = prepositionOnto.DeduceFillerType(passive, WordString, ArgumentType.Noun);
                    if (ft1 == "NOUNBEFORE")
                    {
                        ParseNode nsobj = (ParseNode)n.Children[1];
                        ParseNode npn = (ParseNode)nsobj.Children[0];
                        foreach (ParseNode nnn in npn.Children)
                        {
                            if (nnn.Goal == "N")
                            {
                                string s22 = SentenceParser.GetWordString(parsetree, nnn);
                                NounFrame nfbefore = new NounFrame(nnn, parsetree);
                                s22 = nfbefore.Text;
                                AddNounFrame(nfbefore);
                                currentVerbFrame.AddTemporalRelation_n(TemporalRelationType.Before, nfbefore);
                            }
                        }

                    }

                 
                    if(ft1=="REASON")
                    {
                        ParseNode SOBJn = (ParseNode)n.Children[1];
                        ParseNode NPn = (ParseNode)SOBJn.Children[0];
                        ParseNode Nn = (ParseNode)NPn.Children[0];
                        if (Nn.Goal=="N")
                        {
                            if (Nn.Children == null)
                            {
                                NounFrame nfreason = new NounFrame(Nn, parsetree);
                                AddNounFrame(nfreason);
                                currentVerbFrame.AddDomainRelation_n(DomainRelationType.Reason, nfreason);
                            }
                            else
                            {
                                ParseNode VINGN = (ParseNode)Nn.Children[0];
                                if (VINGN.Goal == "VING")
                                {
                                    NounFrame nfreason = new NounFrame(Nn, parsetree);
                                    AddNounFrame(nfreason);
                                    currentVerbFrame.AddDomainRelation_n(DomainRelationType.Reason, nfreason);

                                }
                            }
                        }
                        else if (Nn.Goal == "ADJ")
                        {
                            ParseNode VINGn = (ParseNode)Nn.Children[0];
                            if (VINGn.Goal == "VING")
                            {
                                NounFrame nfreason = new NounFrame(VINGn, parsetree);
                                AddNounFrame(nfreason);
                                currentVerbFrame.AddDomainRelation_n(DomainRelationType.Reason, nfreason);

                            }
                        }

                    }
                    
                    if (ft1 == "ACCOMPANION")
                    {
                        ParseNode pnaccomp = (ParseNode)n.Children[1];
                        NounFrame nfaccomp = new NounFrame(pnaccomp, parsetree);
                        AddNounFrame(nfaccomp);
                        currentVerbFrame.AddCaseRole(CaseRole.Accompanier, nfaccomp);
                    }
                    if (ft1 == "INSTRUMENT")
                    {
                        ParseNode pnInstrument = (ParseNode)n.Children[1];
                        NounFrame nfInstrument = new NounFrame(pnInstrument, parsetree);
                        AddNounFrame(nfInstrument);
                        currentVerbFrame.AddCaseRole(CaseRole.Instrument, nfInstrument);
                    }
                    if (ft1 == "AGENT")
                    {
                        ParseNode pnagent = (ParseNode)n.Children[1];
                        NounFrame nfagent = new NounFrame(pnagent, parsetree);
                        AddNounFrame(nfagent);
                        currentVerbFrame.AddCaseRole(CaseRole.Agent, nfagent);
                    }
                    if (ft1 == "SOURCE")
                    {
                        if (fromprep == true)
                        {

                            ParseNode sobjnode = (ParseNode)n.Children[1];
                            if (sobjnode.Children != null)
                            {
                                ParseNode npnode = (ParseNode)sobjnode.Children[0];
                                ParseNode nnode = (ParseNode)npnode.Children[0];
                                ParseNode grindnode = (ParseNode)nnode.Children[0];
                                if (nnode.Children != null)
                                {
                                    //CurrentNounFrame = new NounFrame(nnode, parseTree);
                                    //nfs.Add(CurrentNounFrame);
                                    //AddNounFrame(CurrentNounFrame);
                                    ParseNode pnSource = (ParseNode)nnode.Children[0];
                                    NounFrame nfSource = new NounFrame(pnSource, parsetree);
                                    AddNounFrame(nfSource);
                                    currentVerbFrame.AddCaseRole(CaseRole.Source, nfSource);
                                    //objlist = GetObjects(pnSource, parsetree, passive);
                                }
                            }
                        }
                        else
                        {
                            ParseNode pnSource = (ParseNode)n.Children[1];
                            ParseNode pnn1 = (ParseNode)pnSource.Children[0];
                            foreach (ParseNode nn in pnn1.Children)
                            {
                                if (nn.Goal == "N")
                                {
                                    NounFrame nfSource = new NounFrame(nn, parsetree);
                                    _n_f = nfSource;
                                    AddNounFrame(nfSource);
                                    currentVerbFrame.AddCaseRole(CaseRole.Source, nfSource);
                                }
                            }

                            foreach (ParseNode pp in pnn1.Children)
                            {
                                if (pp.Goal == "ADJ")
                                {
                                    // ParseNode pnSource1 = (ParseNode)pp.Children[0];
                                    NounFrame nfSource1 = new NounFrame(pp, parsetree);
                                    //AddNounFrame(nfSource1);
                                    string s = SentenceParser.GetWordString(parsetree, pp);
                                    pp.Text = s;
                                    currentAdjectives.Add(pp);
                                    _n_f.Adjective = currentAdjectives;
                                    //currentVerbFrame.AddCaseRole(CaseRole.Source, nfSource1);

                                }
                            }
                        }
                    }

                    if (ft1 == "DESTINATION")
                    {
                        ParseNode pndDestination = (ParseNode)n.Children[1];
                        if (pndDestination.Children != null)
                        {
                            ParseNode pnd = (ParseNode)pndDestination.Children[0];
                            if (pnd.Goal == "NP")
                            {
                                ParseNode pnnn = (ParseNode)pnd.Children[1];
                                NounFrame nfDestination = new NounFrame(pnnn, parsetree);
                                AddNounFrame(nfDestination);

                                currentVerbFrame.AddCaseRole(CaseRole.Destination, nfDestination);
                            }
                        }
                        else
                        {
                            NounFrame nfDestination = new NounFrame(pndDestination, parsetree);
                            AddNounFrame(nfDestination);

                            currentVerbFrame.AddCaseRole(CaseRole.Destination, nfDestination);
                        }
                    }
                    if (ft1 == "LOCATION")
                    {
                        if (k == false)
                        {
                            ParseNode pnLocation = (ParseNode)n.Children[1];
                            ParseNode NPn = (ParseNode)pnLocation.Children[0];
                            if (NPn.Children.Count == 1)
                            {
                                NounFrame nfLocation = new NounFrame(pnLocation, parsetree);
                                AddNounFrame(nfLocation);
                                currentVerbFrame.AddCaseRole(CaseRole.location, nfLocation);
                            }
                            else
                            {

                                foreach (ParseNode Nn in NPn.Children)
                                {
                                    if (Nn.Goal == "N")
                                    {
                                        NounFrame nfff = new NounFrame(Nn, parsetree);
                                        AddNounFrame(nfff);
                                        adj_noun = true;
                                        _n_f = nfff;

                                    }
                                }
                                foreach (ParseNode Nn in NPn.Children)
                                {
                                    if (Nn.Goal == "ADJ")
                                    {

                                        if (Nn.Children.Count != null)
                                        {
                                            ParseNode pnpn = (ParseNode)Nn.Children[0];
                                            ParseNode prep_phn = (ParseNode)pnpn.Children[1];
                                            ParseNode npN = (ParseNode)prep_phn.Children[0];
                                            ParseNode nNn = (ParseNode)npN.Children[0];

                                            NounFrame nfff = new NounFrame(nNn, parsetree);
                                            string s = SentenceParser.GetWordString(parsetree, nNn);
                                            nNn.Text = s;

                                            //AddNounFrame(nfff);
                                            currentAdjectives.Add(nNn);

                                            if (adj_noun == true)
                                            {
                                                _n_f.Adjective = currentAdjectives;
                                            }

                                        }



                                    }

                                }
                                currentVerbFrame.AddCaseRole(CaseRole.location, _n_f);

                            }

                        

                        }
                        
                    }
                }
                else if (n.Goal == "INF_PH")
                {
                    ParseNode pn = (ParseNode)n.Children[0];
                    string WordString = SentenceParser.GetWordString(parsetree, pn);
                    ft2 = prepositionOnto.DeduceFillerType(passive, WordString, ArgumentType.Verb);
                if (ft2 == "REASON")
                  {
                   ParseNode pnreason = (ParseNode)n.Children[1];
                   string s = SentenceParser.GetWordString(parsetree, pnreason);
                   VerbFrame vfreason = new VerbFrame(pnreason);
                   vfreason.VerbName = s;
                   AddVerbrame(vfreason);
                   currentVerbFrame.AddDomainRelation(DomainRelationType.Reason, vfreason);

                   
                  } 
                
                }
                if (n.Goal == "V")
                {
                    if (passive == true)
                    {
                        if (Sor == true)
                        {
                            string s = SentenceParser.GetWordString(parsetree, n);
                            if (s == "IS" || s == "ARE" || s == "WAS" || s == "WERE")
                            {
                                ParseNode node = (ParseNode)n.Parent;
                                ParseNode cmpn = (ParseNode)node.Children[1];
                                ParseNode adjn = (ParseNode)cmpn.Children[0];
                                ParseNode VPSPn = (ParseNode)adjn.Children[0];
                                if (VPSPn.Goal == "VPSP")
                                {
                                    FillRelations(parsetree, VPSPn);
                                    //string sss = SentenceParser.GetWordString(parsetree, VPSPn);
                                    //VerbFrame vfreason = new VerbFrame(VPSPn);
                                    //vfreason.VerbName = sss;
                                    //AddVerbrame(vfreason);
                                    //currentVerbFrame.AddDomainRelation(DomainRelationType.Reason, vfreason);
                                }
                            }
                        }
                        if (Cor == true)
                        {
                            string s = SentenceParser.GetWordString(parsetree, n);
                            if (s == "IS" || s == "ARE" || s == "WAS" || s == "WERE")
                            {
                                ParseNode node = (ParseNode)n.Parent;
                                ParseNode cmpn = (ParseNode)node.Children[1];
                                ParseNode adjn = (ParseNode)cmpn.Children[0];
                                ParseNode VPSPn = (ParseNode)adjn.Children[0];
                                if (VPSPn.Goal == "VPSP")
                                {
                                    if (Corname == "SO")
                                    {
                                        string sss = SentenceParser.GetWordString(parsetree, VPSPn);
                                        VerbFrame vfresult = new VerbFrame(VPSPn);
                                        vfresult.VerbName = sss;
                                        AddVerbrame(vfresult);
                                        currentVerbFrame.AddDomainRelation(DomainRelationType.ExpectedResult, vfresult);
                                    }
                                }
                            }
                        }


                    }


                    else
                    {
                        if (Sor == true)
                        {
                            FillRelations(parsetree, n);

                        }
                        if (Cor == true)//sara add it
                        {
                            if (Corname == "SO")
                            {
                                string s = SentenceParser.GetWordString(parsetree, n);
                                VerbFrame vfresult = new VerbFrame(n);
                                vfresult.VerbName = s;
                                AddVerbrame(vfresult);
                                currentVerbFrame.AddDomainRelation(DomainRelationType.ExpectedResult, vfresult);
                            }
                        }
                        //string s = SentenceParser.GetWordString(parsetree, n);
                        //VerbFrame vfreason = new VerbFrame(n);
                        //vfreason.VerbName = s;
                        //AddVerbrame(vfreason);
                        //currentVerbFrame.AddDomainRelation(DomainRelationType.Reason, vfreason);
                    }
                }
            

        }
        public void ModifyRoot(ArrayList path, ref ParseNode node)
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



    }
    
}
        #endregion 