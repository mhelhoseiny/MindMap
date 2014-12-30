using System;
using System.Collections.Generic;
using System.Text;
using SyntacticAnalyzer;
using mmTMR;
using OntologyLibrary.OntoSem;

namespace mmTMR
{
    
    public enum TemporalRelationType
    {
        After,
        Before,
        Concurrent
    }
    public enum DomainRelationType
    {
        ExpectedResult,
        UnExpectedResult,
        Reason
    }
    

     //DONE
    //#region CircumstantialRoles
     //   //location
     //   //time

     //   #endregion
    #region Aspect
    public enum Phase
    {
        Unknown,
        Begin,
        End,
        Continue
    }
   
    public enum IterationType
    {
        Unknown, 
        Single,
       Multilple
    }
    public class Iteration
    {
        private IterationType _iterationType = IterationType.Unknown;

        public IterationType IterationType
        {
            get { return _iterationType; }
            set { _iterationType = value; }
        }
        private int _count = 0;

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }
    }
    public enum DurationType
    {
       Unknown, 
       Momentary,
       Prolonged
    }
    public class Duration
    {
        private string _actionTime;

        public string ActionTime
        {
            get { return _actionTime; }
            set { _actionTime = value; }
        }
        private DurationType _durationType = DurationType.Unknown;
        
        public DurationType DurationType
        {
            get { return _durationType; }
            set { _durationType = value; }
        }
        
        private TimeSpan _duration = new TimeSpan();

        public TimeSpan Duration1
        {
            get { return _duration; }
            set { _duration = value; }
        }
    }
    public enum Telicity
    {
        Unknown,
        Completed,
        Incompleted,
        
    }
    public class Aspect
    {
        Phase _phase;

        public Phase Phase
        {
            get { return _phase; }
            set { _phase = value; }
        }
        Iteration _iteration;

        public Iteration Iteration
        {
            get { return _iteration; }
            set { _iteration = value; }
        }
        Duration _duration;

        public Duration Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        Telicity _telicity;//i.e. Completeness

        public Telicity Telicity
        {
            get { return _telicity; }
            set { _telicity = value; }
        }
        public Aspect()
        {
            _phase = Phase.Unknown;
            _iteration = new Iteration();
            _duration = new Duration();
            _telicity = Telicity.Unknown;
        }
       
    }
        //contains phase,iteration,duration,telicity //done or undone
    #endregion
    // a class that contains an actions and Related Frames
    //Related frames contains 
    //Verb Frames: Temporal Relations (After ,before,concurrent), Domain Relations(Reasons,Resulrs).
    //Noun Frame as Fillers : Agent,Theme,Cotheme,Accompanion,....,etc
    public class VerbFrame
    {
        //public status st;
        //public ArrayList CorFrames = new ArrayList();
        //public bool Positive;
        //public bool IsConcept = false;
        //public bool IsPRP = false;
        public ParseNode _parseNode;
        public string VerbName;
        public List<ParseNode>  _Adverb;
        public Concept Predicate;
        Dictionary<TemporalRelationType, List<VerbFrame>> _temporalRelations = new Dictionary<TemporalRelationType, List<VerbFrame>>();

        public Dictionary<TemporalRelationType, List<VerbFrame>> TemporalRelations
        {
            get { return _temporalRelations; }
            set { _temporalRelations = value; }
        }
        Dictionary<TemporalRelationType, List<NounFrame>> _temporalRelations_n = new Dictionary<TemporalRelationType, List<NounFrame>>();

        public Dictionary<TemporalRelationType, List<NounFrame>> TemporalRelations_n
        {
            get { return _temporalRelations_n; }
            set { _temporalRelations_n = value; }
        }
        Dictionary<DomainRelationType, List<VerbFrame>> _domainRelations = new Dictionary<DomainRelationType, List<VerbFrame>>();
        Dictionary<DomainRelationType, List<NounFrame>> _domainRelations_n = new Dictionary<DomainRelationType, List<NounFrame>>();


        public Dictionary<DomainRelationType, List<VerbFrame>> DomainRelations
        {
            get { return _domainRelations; }

        }
        public Dictionary<DomainRelationType, List<NounFrame>> DomainRelations_n
        {
            get { return _domainRelations_n; }

        }

        Dictionary<CaseRole, List<NounFrame>> _caseRoles = new Dictionary<CaseRole, List<NounFrame>>();

        public Dictionary<CaseRole, List<NounFrame>> CaseRoles
        {
            get { return _caseRoles; }
          
        }
        Aspect _aspect = new Aspect();
        public VerbFrame(ParseNode node)
        {
            _parseNode = node;
        }
        public List<ParseNode> Adverb
        {
            get { return _Adverb; }
            set { _Adverb = value; }
        }
        public Aspect Aspect
        {
            get { return _aspect; }
            set { _aspect = value; }
        }
        string _time = "UNKNOWN";

        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public void AddDomainRelation(DomainRelationType domainRelationType,VerbFrame frame)
         {
             if (_domainRelations.ContainsKey(domainRelationType))
             {
                 _domainRelations[domainRelationType].Add(frame);
             }
             else
             {
                 List<VerbFrame>  verbframelst =  new List<VerbFrame>();
                 verbframelst.Add(frame);
                 _domainRelations.Add(domainRelationType, verbframelst);
             }

         }
        public void AddDomainRelation_n(DomainRelationType domainRelationType, NounFrame frame)
        {
            if(_domainRelations_n.ContainsKey(domainRelationType))
            {
                _domainRelations_n[domainRelationType].Add(frame);
            }
            
            else
            {
                List<NounFrame> nounframelst = new List<NounFrame>();
                nounframelst.Add(frame);
               _domainRelations_n.Add(domainRelationType, nounframelst);
            }
        }
        public List<VerbFrame> GetTemporalRelation(TemporalRelationType temporalRelationType)
        {
            return _temporalRelations[temporalRelationType];
        }

        public List<VerbFrame> GetDomainRelation(DomainRelationType domainRelationType)
        {
            return _domainRelations[domainRelationType];
        }
        public List<NounFrame> GetCaseRole(CaseRole caseRole)
        {
            return _caseRoles[caseRole];
        }
        public void AddTemporalRelation(TemporalRelationType temporalRelationType, VerbFrame frame)
        {
            if (_temporalRelations.ContainsKey(temporalRelationType))
            {
                _temporalRelations[temporalRelationType].Add(frame);
            }
            else
            {
                List<VerbFrame> verbframelst = new List<VerbFrame>();
                verbframelst.Add(frame);
                _temporalRelations.Add(temporalRelationType, verbframelst);
            }

        }
        public void AddTemporalRelation_n(TemporalRelationType temporalRelationType, NounFrame frame)
        {
            if (_temporalRelations_n.ContainsKey(temporalRelationType))
            {
                _temporalRelations_n[temporalRelationType].Add(frame);
            }
            else
            {
                List<NounFrame> nounframelst = new List<NounFrame>();
                nounframelst.Add(frame);
                _temporalRelations_n.Add(temporalRelationType, nounframelst);
            }

        }
        public void AddCaseRole(CaseRole caseRole, NounFrame frame)
        {
            if (_caseRoles.ContainsKey(caseRole))
            {
                _caseRoles[caseRole].Add(frame);
            }
            else
            {
                List<NounFrame> verbframelst = new List<NounFrame>();
                verbframelst.Add(frame);
                _caseRoles.Add(caseRole, verbframelst);
            }

        }
        
        
    }
}
