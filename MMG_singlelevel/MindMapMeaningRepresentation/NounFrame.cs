using System;
using System.Collections.Generic;
using System.Text;
using SyntacticAnalyzer;
using mmTMR;
using OntologyLibrary.OntoSem;

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
        time
        //Means,
        //Beneficiary,
        //purpose,
        //manner,
        //experiencer
        #endregion


    }
    public class NounFrame
    {
        public string IS_a;
        Concept _concept;
        ParseNode _parseNode;
        ParseTree _parsetree;
        string SearchText;

        public string SearchText1
        {
            get { return SearchText; }
            set { SearchText = value; }
        }
        public ParseTree Parsetree
        {
            get { return _parsetree; }
           
        }
        List<ParseNode> _Adjective;
        List<string> _Adj_FillerType = new List<string>();
        string _text=null;
        public Concept Concept
        {
            get { return _concept; }
            set { _concept = value; }
        }
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



        Dictionary<CaseRole, List<VerbFrame>> _associatedactions = new Dictionary<CaseRole, List<VerbFrame>>();

        public void AddCaseRole(CaseRole role,VerbFrame verbFrame)
        {
            if (_associatedactions.ContainsKey(role))
            {
                _associatedactions[role].Add(verbFrame);
            }
            else
            {
                List<VerbFrame> vframes = new List<VerbFrame>();
                vframes.Add(verbFrame);
                _associatedactions.Add(role,vframes);
            }
 
        }
        public NounFrame(ParseNode node,ParseTree pt)
        {
            _parseNode = node;
            _parsetree = pt;
        }
      
    }
}
