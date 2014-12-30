using System;
using System.Collections.Generic;
using System.Text;

namespace mmTMR
{
    public enum ArgumentType
        {
            Noun,
            Verb,
            Gerund
        }
    class PrepositionArgInfo
    {
        bool _passiveSentence;

        public bool PassiveSentence
        {
            get { return _passiveSentence; }
            set { _passiveSentence = value; }
        }
        string _preposition;

        public string Preposition
        {
            get { return _preposition; }
            set { _preposition = value; }
        }
        ArgumentType _argumentType;

        public ArgumentType ArgumentType
        {
            get { return _argumentType; }
            set { _argumentType = value; }
        }
        string _fillerType;

        public string FillerType
        {
            get { return _fillerType; }
            set { _fillerType = value; }
        }
        public PrepositionArgInfo(string preposition, ArgumentType argumentType, string fillerType)//:this(false,preposition, ArgumentType,argumentType,fillerType)
        {
            _passiveSentence = false;
            _preposition = preposition;
            _argumentType = argumentType;
            _fillerType = fillerType;
        }
        public PrepositionArgInfo(bool passiveSentence, string preposition, ArgumentType argumentType, string fillerType)
        {
            _passiveSentence = passiveSentence;
            _preposition = preposition;
            _argumentType = argumentType;
            _fillerType = fillerType;
        }    
     }
    class PrepositionOnto
    {
        List<PrepositionArgInfo> PrepositionArgInfoList = new List<PrepositionArgInfo>();

        public PrepositionOnto()
        {
            LoadPrepositionFillers();
        }

        public string DeduceFillerType(string preposition, ArgumentType arg)
        {
            return DeduceFillerType(false, preposition, arg);
        }
        public string DeduceFillerType(bool PassiveSentence,string preposition,ArgumentType arg)
        {

            foreach (PrepositionArgInfo pi in PrepositionArgInfoList)
            {

                if(pi.PassiveSentence ==  PassiveSentence&&
                    pi.Preposition == preposition&&                    
                    pi.ArgumentType == arg                   
                    )
                {
                    return pi.FillerType;
                }

            }
            return null;
        }


        public void LoadPrepositionFillers()
        {
            PrepositionArgInfoList.Add(new PrepositionArgInfo("TO", ArgumentType.Noun, "DESTINATION"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("TO", ArgumentType.Verb, "REASON"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("DUE_TO", ArgumentType.Verb, "REASON"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("BECAUSE_OF", ArgumentType.Noun, "REASON"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("BECAUSE_OF", ArgumentType.Gerund, "REASON"));
            
            PrepositionArgInfoList.Add(new PrepositionArgInfo("FROM", ArgumentType.Noun, "SOURCE"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("WITH", ArgumentType.Noun, "ACCOMPANION"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("FOR", ArgumentType.Gerund, "REASON"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("FOR", ArgumentType.Noun, "TIME"));
            
            PrepositionArgInfoList.Add(new PrepositionArgInfo("IN", ArgumentType.Noun, "LOCATION"));
            
            PrepositionArgInfoList.Add(new PrepositionArgInfo("INTO", ArgumentType.Noun, "LOCATION"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("AT", ArgumentType.Noun, "LOCATION"));
            
            PrepositionArgInfoList.Add(new PrepositionArgInfo(false,"BY", ArgumentType.Noun, "INSTRUMENT"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo(true,"BY", ArgumentType.Noun, "AGENT"));
            PrepositionArgInfoList.Add(new PrepositionArgInfo("BEFORE", ArgumentType.Noun, "NOUNBEFORE"));







        }
        //{
        //    this.PrepositionFiller = new Hashtable();
        //    ArrayList ar = new ArrayList();
        //    ar.Add(false);//Subject
        //    ar.Add("PART-OF-OBJECT");
        //    this.PrepositionFiller.Add("OF", ar);

        //    ar = new ArrayList();
        //    ar.Add(true);//Predecate
        //    ar.Add("DURATION");
        //    this.PrepositionFiller.Add("DURING", ar);

        //    ar = new ArrayList();
        //    ar.Add(true);
        //    ar.Add("PATH");
        //    this.PrepositionFiller.Add("THROUGHOUT", ar);

        //    ar = new ArrayList();
        //    ar.Add(true);//Predecate
        //    ar.Add("DESTINATION");
        //    this.PrepositionFiller.Add("TO", ar);

        //    ar = new ArrayList();
        //    ar.Add(true);//Predecate
        //    ar.Add("SOURCE");
        //    this.PrepositionFiller.Add("FROM", ar);

        //    ar = new ArrayList();
        //    ar.Add(true);//Predecate
        //    ar.Add("INSTRUMENT");
        //    this.PrepositionFiller.Add("THROUGH", ar);

        //    ar = new ArrayList();
        //    ar.Add(true);//Predecate
        //    ar.Add("DESTINATION");
        //    ar.Add(true);//Predecate
        //    ar.Add("LOCATION");
        //    this.PrepositionFiller.Add("INTO", ar);

        //    ar = new ArrayList();
        //    ar.Add(true);//Predecate
        //    ar.Add("LOCATION");
        //    this.PrepositionFiller.Add("IN", ar);

        //    ar = new ArrayList();
        //    ar.Add(true);//Predecate
        //    ar.Add("REASON");
        //    this.PrepositionFiller.Add("DUE_TO", ar);
        //}
    }
}
