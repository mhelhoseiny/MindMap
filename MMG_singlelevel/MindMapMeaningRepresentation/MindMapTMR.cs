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

namespace mmTMR
{

    public class MindMapTMR
    {
        //public Hashtable PrepositionFiller;
        //public Hashtable WordToConcept;
        //public Hashtable ADJAndADVFiller;

        //public Hashtable Concepts = new Hashtable();


        //public Ontology Onto;
        public OntologyLibrary.OntoSem.Ontology Onto_mo2a;
        public WordOlogy Wordology;
        public ArrayList ArrWordology;
        public string _ontologyDirectoryPath =
                @"F:\Mohamed ELhoseiny\Research\English2MindMap\MMG2008\Ontology\Formatted OntoSem";
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

        List<NounFrame> _nounframes = new List<NounFrame>();

        public List<NounFrame> Nounframes
        {
            get { return _nounframes; }
        }
        List<VerbFrame> _verbFrames = new List<VerbFrame>();

        public List<VerbFrame> VerbFrames
        {
            get { return _verbFrames; }
        }
        
        public MindMapTMR(ArrayList parseTrees, ArrayList discourse)
        {
            _parseTrees = parseTrees;
            _discourse = discourse;

        }
        VerbFrame currentVerbFrame = null;
        
    
        private List<NounFrame> currentsubjects=new List<NounFrame>();
        private List<NounFrame> currentObjects = new List<NounFrame>();
        

        public void BuildSentenceTMR(ParseTree parsetree)
        {
            BuildNodeTMR(parsetree, parsetree.Root, -1);
        }

        public void BuildTMR()
        {
            GetWordologyList();
            int i  = 1;
            foreach (ParseTree ptree in _parseTrees)
            {
                BuildSentenceTMR(ptree);
                i++;
            }
            MapMeaning();
            
        }
        public void LoadOntology()
        {
            if (Onto_mo2a == null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                System.IO.FileStream fs = new System.IO.FileStream(
                    _ontologyDirectoryPath + @"\Ontology.bin", FileMode.Open);
                Onto_mo2a = (OntologyLibrary.OntoSem.Ontology)bf.Deserialize(fs);
                Onto_mo2a.OntologyPath = _ontologyDirectoryPath;
                fs.Close();
            }

        }
        private void MapMeaning()
        {
            LoadOntology();
            foreach (NounFrame Nf in Nounframes)
            {
                ParseNode pn = Nf.ParseNode;
                string pn_text = Nf.Text;
                
                Nf.Adjective_fillerType = new List<string>();
                if (Nf.Adjective == null)
                {
                    continue;
                }
                for (int i = 0; i < Nf.Adjective.Count; i++)
                {
                    Nf.Adjective_fillerType.Add("");
                }
                foreach (WordOlogy w in ArrWordology)
                {

                    if (pn.Sense == w.Sense && w.Pos == "noun" && w.Word == pn_text.ToLower())
                    {
                        //FileStream allConceptsFile = new FileStream(_ontologyDirectoryPath + @"\AllConcepts.txt", FileMode.Open);
                        //StreamReader allConceptsFileReader = new StreamReader(allConceptsFile);
                        string concept = w.Concept;
                        string Conceptpath = _ontologyDirectoryPath + @"\" + concept[0] + @"\" + concept + ".txt";
                        OntologyLibrary.OntoSem.Concept C = (OntologyLibrary.OntoSem.Concept)Onto_mo2a[concept];

                        Nf.Concept = C;
                        OntologyLibrary.OntoSem.PropertiesDictionary pd = Nf.Concept.FullProperties;


                        foreach (Property de in pd.Properties)
                        {

                            OntologyLibrary.OntoSem.FillerList OF = de.Fillers;
                            foreach (OntologyLibrary.OntoSem.Filler Ofiller in OF)
                            {

                                if (de.Name == "IS-A")
                                {
                                    string is_a = Ofiller.ConceptFiller.Name;
                                    Nf.IS_a = is_a;
                                }

                                for (int i = 0; i < Nf.Adjective.Count;i++ )
                                {
                                    ParseNode pnn = Nf.Adjective[i];

                                    if (pnn.Text == Ofiller.ScalarFiller)
                                    {
                                        Nf.Adjective_fillerType[i] = de.Name.ToString();
                                    }
                                    
                                }




                            }

                        }




                        Property maplexProperty = C.FullProperties["FILLER"];





                    }
                }

            }

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
                        else if (verb_pp.Goal == "NP" && verb_pp.Children.Count>=2)
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

               case "SS":


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
                                                   VerbFrames.Add(currentVerbFrame);
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
                                                       VerbFrames.Add(currentVerbFrame);
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
                                                       VerbFrames.Add(currentVerbFrame);
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
                                                  VerbFrames.Add(currentVerbFrame);
                                              }
                                              else if (VINFn.Goal == "VING")
                                              {
                                                  currentVerbFrame = new VerbFrame(VINFn);
                                                  verbname = SentenceParser.GetWordString(parsetree, VINFn);
                                                  currentVerbFrame.VerbName = verbname;
                                                  VerbFrames.Add(currentVerbFrame);
                                              }
                                             


                                           }
                                           else
                                           {
                                               currentVerbFrame = new VerbFrame(n);
                                               verbname = SentenceParser.GetWordString(parsetree, n);
                                               currentVerbFrame.VerbName = verbname;
                                               VerbFrames.Add(currentVerbFrame);

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
                                                   VerbFrames.Add(currentVerbFrame);
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
                                                   VerbFrames.Add(currentVerbFrame);
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
                                           VerbFrames.Add(currentVerbFrame);
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
                                           VerbFrames.Add(currentVerbFrame);

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
                                               VerbFrames.Add(currentVerbFrame);
                                           }
                                           
                                       }
                                       else if (pnode.Goal == "VINF")//3yzen ne3rf zy 2lle fo2eha walla l2 :D:D:D
                                       {
                                           currentVerbFrame = new VerbFrame(pnode);
                                           verbname = SentenceParser.GetWordString(parsetree, pnode);
                                           currentVerbFrame.VerbName = verbname;
                                           VerbFrames.Add(currentVerbFrame);
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

                                           currentVerbFrame.Adverb = CurrentAdverbs;
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

                                   currentVerbFrame.Adverb = CurrentAdverbs;

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
                                Nounframes.Add(CurrentNounFrame);
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
                                //Nounframes.Add(n1);
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
                            Nounframes.Add(CurrentNounFrame);
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
                                        //Nounframes.Add(nj);


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
                                //Nounframes.Add(nj);
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
                                Nounframes.Add(CurrentNounFrame);
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
                                        //Nounframes.Add(n1);


                                    }
                                }
                            }
                            else if (p2.Goal == "ADJ")
                            {
                                string s = SentenceParser.GetWordString(parseTree, p2);
                                p2.Text = s;
                                currentAdjectives.Add(p2);
                                //NounFrame n1 = new NounFrame(p2, parseTree);
                                //Nounframes.Add(n1);

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
                        Nounframes.Add(CurrentNounFrame);
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
                               //Nounframes.Add(nj);


                           }
                        }
                    }
                    else if (p2.Goal == "ADJ")
                    {
                        string s = SentenceParser.GetWordString(parseTree, p2);
                        p2.Text = s;
                        currentAdjectives.Add(p2);
                        //NounFrame nj = new NounFrame(p2, parseTree);
                        //Nounframes.Add(nj);
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
                VerbFrames.Add(vfafter);
                currentVerbFrame.AddTemporalRelation(TemporalRelationType.Before, vfafter);
                //currentVerbFrame.AddTemporalRelation(TemporalRelationType.Before, currentVerbFrame);
            }
            if (Sorname == "SO_THAT")
            {
                string sss = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfreason = new VerbFrame(pnode);
                vfreason.VerbName = sss;
                VerbFrames.Add(vfreason);
                currentVerbFrame.AddDomainRelation(DomainRelationType.Reason, vfreason);
            }
            if (Sorname == "BECAUSE")
            {
                string sss = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfreason = new VerbFrame(pnode);
                vfreason.VerbName = sss;
                VerbFrames.Add(vfreason);
                currentVerbFrame.AddDomainRelation(DomainRelationType.Reason, vfreason);
            }
            if (Sorname == "THAT" && WordStringafter == "AFTER")
            {
                string s11 = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfafter = new VerbFrame(pnode);
                vfafter.VerbName = s11;
                VerbFrames.Add(vfafter);
                currentVerbFrame.AddTemporalRelation(TemporalRelationType.After, vfafter);
                // currentVerbFrame.AddTemporalRelation(TemporalRelationType.Before, currentVerbFrame);
            }
            if (Sorname == "THAT" && WordStringbefore == "BEFORE")
            {
                string s22 = SentenceParser.GetWordString(parsetree, pnode);
                VerbFrame vfbefore = new VerbFrame(pnode);
                vfbefore.VerbName = s22;
                VerbFrames.Add(vfbefore);
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
                Nounframes.Add(nfSource);
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
                        Nounframes.Add(_n_f);
                        currentVerbFrame.AddCaseRole(CaseRole.Source, nfSource1);
                        
                        
                    }
                }
                foreach (ParseNode pp in npnode.Children)
                {
                    if (pp.Goal == "ADJ")
                    {
                        // ParseNode pnSource1 = (ParseNode)pp.Children[0];
                        NounFrame nfSource1 = new NounFrame(pp, parsetree);
                        //Nounframes.Add(nfSource1);
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
                            //Nounframes.Add(nftime);
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
                                Nounframes.Add(nfbefore);
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
                                Nounframes.Add(nfreason);
                                currentVerbFrame.AddDomainRelation_n(DomainRelationType.Reason, nfreason);
                            }
                            else
                            {
                                ParseNode VINGN = (ParseNode)Nn.Children[0];
                                if (VINGN.Goal == "VING")
                                {
                                    NounFrame nfreason = new NounFrame(Nn, parsetree);
                                    Nounframes.Add(nfreason);
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
                                Nounframes.Add(nfreason);
                                currentVerbFrame.AddDomainRelation_n(DomainRelationType.Reason, nfreason);

                            }
                        }

                    }
                    
                    if (ft1 == "ACCOMPANION")
                    {
                        ParseNode pnaccomp = (ParseNode)n.Children[1];
                        NounFrame nfaccomp = new NounFrame(pnaccomp, parsetree);
                        Nounframes.Add(nfaccomp);
                        currentVerbFrame.AddCaseRole(CaseRole.Accompanier, nfaccomp);
                    }
                    if (ft1 == "INSTRUMENT")
                    {
                        ParseNode pnInstrument = (ParseNode)n.Children[1];
                        NounFrame nfInstrument = new NounFrame(pnInstrument, parsetree);
                        Nounframes.Add(nfInstrument);
                        currentVerbFrame.AddCaseRole(CaseRole.Instrument, nfInstrument);
                    }
                    if (ft1 == "AGENT")
                    {
                        ParseNode pnagent = (ParseNode)n.Children[1];
                        NounFrame nfagent = new NounFrame(pnagent, parsetree);
                        Nounframes.Add(nfagent);
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
                                    //Nounframes.Add(CurrentNounFrame);
                                    ParseNode pnSource = (ParseNode)nnode.Children[0];
                                    NounFrame nfSource = new NounFrame(pnSource, parsetree);
                                    Nounframes.Add(nfSource);
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
                                    Nounframes.Add(nfSource);
                                    currentVerbFrame.AddCaseRole(CaseRole.Source, nfSource);
                                }
                            }

                            foreach (ParseNode pp in pnn1.Children)
                            {
                                if (pp.Goal == "ADJ")
                                {
                                    // ParseNode pnSource1 = (ParseNode)pp.Children[0];
                                    NounFrame nfSource1 = new NounFrame(pp, parsetree);
                                    //Nounframes.Add(nfSource1);
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
                                Nounframes.Add(nfDestination);

                                currentVerbFrame.AddCaseRole(CaseRole.Destination, nfDestination);
                            }
                        }
                        else
                        {
                            NounFrame nfDestination = new NounFrame(pndDestination, parsetree);
                            Nounframes.Add(nfDestination);

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
                                Nounframes.Add(nfLocation);
                                currentVerbFrame.AddCaseRole(CaseRole.location, nfLocation);
                            }
                            else
                            {

                                foreach (ParseNode Nn in NPn.Children)
                                {
                                    if (Nn.Goal == "N")
                                    {
                                        NounFrame nfff = new NounFrame(Nn, parsetree);
                                        Nounframes.Add(nfff);
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

                                            //Nounframes.Add(nfff);
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
                   VerbFrames.Add(vfreason);
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
                                    //VerbFrames.Add(vfreason);
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
                                        VerbFrames.Add(vfresult);
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
                                VerbFrames.Add(vfresult);
                                currentVerbFrame.AddDomainRelation(DomainRelationType.ExpectedResult, vfresult);
                            }
                        }
                        //string s = SentenceParser.GetWordString(parsetree, n);
                        //VerbFrame vfreason = new VerbFrame(n);
                        //vfreason.VerbName = s;
                        //VerbFrames.Add(vfreason);
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
