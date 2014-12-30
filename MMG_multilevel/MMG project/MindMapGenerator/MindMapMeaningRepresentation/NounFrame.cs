using System;
using System.Collections.Generic;
using System.Text;
using SyntacticAnalyzer;
using mmTMR;
using OntologyLibrary.OntoSem;
using OurMindMapOntology;
using WordsMatching;




namespace mmTMR
{
    public enum CaseRole
    {
        #region CaseRoles
        Agent,
        Theme,
        Cotheme,
        Accompanier,
        Instrument,
        Source,
        Destination,
        Path,
        location,
        time,
        Means,
        OwnerOf,
        Beneficiary,
        purpose,
        //manner,
        //experiencer
        unknown,
        Action,
        Possession,
        example,////////
        among,
        reason,
        use,
        focus,
        Against,
        Under,
        About,
        On
        #endregion

        
    }
	[Serializable]
    public class NounFrame : Frame
    {
        public string IS_a;
       //MindMapConcept _concept;
       List<MyWordInfo> _adjectivesInfo = new List<MyWordInfo>();

       public List<MyWordInfo> AdjectivesInfo
       {
           get { return _adjectivesInfo; }
           set { _adjectivesInfo = value; }
       }



       List<NounFrame> _addedto = new List<NounFrame>();

       public List<NounFrame> Addedto
       {
           get { return _addedto; }
           set { _addedto = value; }
       }
       //private List<NounFrame> _ownerof = new List<NounFrame>();

       //public List<NounFrame> Ownerof
       //{
       //    get { return _ownerof; }
       //    set { _ownerof = value; }
       //}

       

        ParseNode _parseNode;
        ParseTree _parsetree;
        string SearchText;

        //int _S_ID = 0;
        bool _containsA = false;

        public bool ContainsA
        {
            get { return _containsA; }
            set { _containsA = value; }
        }



        //public int Sentence_ID
        //{
        //    get { return _S_ID; }
        //    set { _S_ID = value; }
        //}

        public NounFrame(string text,MindMapConcept c)
        {
            this.FrameType = Type.Noun;
            this._text = text;
            this.Concept = c;
        }
        public NounFrame(ParseNode node)
        {
            this._parseNode = node;
            this._Adv_descriptive = new List<string>();
        }

        public string SearchText1
        {
            get { return SearchText; }
            set { SearchText = value; }
        }
        public ParseTree Parsetree
        {
            get { return _parsetree; } 
        }

        //save el adjectives elli beterga3li list of nounframe

        List<NounFrame> _testAdj = new List<NounFrame>();

        public List<NounFrame> TestAdj
        {
            get { return _testAdj; }
            set { _testAdj = value; }
        }


        List<ParseNode> _Adjective=new List<ParseNode>();
        List<string> _Adjectives = new List<string>();

        public List<string> Adjectives
        {
            get { return _Adjectives; }
            set { _Adjectives = value; }
        }
        //marwa
        List<string> _Adv_descriptive= new List<string>();

        public List<string> Adv_descriptive
        {
            get { return _Adv_descriptive; }
            set { _Adv_descriptive = value; }
        }
        List<NounFrame> _ppj = new List<NounFrame>();

        public List<NounFrame> Ppj
        {
            get { return _ppj; }
            set { _ppj = value; }
        }

        List<string> _Adj_FillerType = new List<string>();
        string _text=null;
        //public MindMapConcept Concept
        //{
        //    get { return _concept; }
        //    set { _concept = value; }
        //}
        public ParseNode ParseNode
        {
            get { return _parseNode; }
            set { _parseNode = value; }
        }
        public string Text
        {
            get {
                if (_text == null)
                    _text = SentenceParser.GetWordString(_parsetree, _parseNode);

                return _text;
            }
        }
        public List<ParseNode> Adjective
        {
            get { return _Adjective; }
            set { _Adjective = value; }
        }

        
        public List<string> Adjective_fillerType
        {
            get { return _Adj_FillerType; }
            set { _Adj_FillerType = value; }
        }



		//Dictionary<CaseRole, List<VerbFrame>> _associatedactions = new Dictionary<CaseRole, List<VerbFrame>>();
        Dictionary<CaseRole, List<NounFrame>> _ownerof = new Dictionary<CaseRole, List<NounFrame>>();

        public Dictionary<CaseRole, List<NounFrame>> Ownerof
        {
            get { return _ownerof; }
           // set { _ownerof = value; }
        }

        //public Dictionary<CaseRole, List<VerbFrame>> Associatedactions
        //{
        //    get { return _associatedactions; }
        //}

        //public void AddCaseRole(CaseRole role,VerbFrame verbFrame)
        //{
        //    if (_associatedactions.ContainsKey(role))
        //    {
        //        _associatedactions[role].Add(verbFrame);
        //    }
        //    else
        //    {
        //        List<VerbFrame> vframes = new List<VerbFrame>();
        //        vframes.Add(verbFrame);
        //        _associatedactions.Add(role,vframes);
        //    }
 
        //}
        public void AddCaseRolenouns(CaseRole role, NounFrame nounframe)
        {
            if (_ownerof.ContainsKey(role))
            {
                _ownerof[role].Add(nounframe);
            }
            else
            {
                List<NounFrame> nframes = new List<NounFrame>();
                nframes.Add(nounframe);
                _ownerof.Add(role, nframes);
            }
 
        }
        public NounFrame(ParseNode node,ParseTree pt)
        {
            _parseNode = node;
            _parsetree = pt;
        }
      
    }
}
