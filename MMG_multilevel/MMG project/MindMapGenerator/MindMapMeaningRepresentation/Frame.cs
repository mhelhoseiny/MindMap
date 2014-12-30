using System;
using System.Collections.Generic;
using System.Text;
using OurMindMapOntology;

namespace mmTMR
{
    [Serializable]
    public class Frame
    {
       // public Frame(VerbFrame )

        MindMapConcept _concept;

        public MindMapConcept Concept
        {
            get { return _concept; }
            set { _concept = value; }
        }

        int _S_ID = 0;

        public int Sentence_ID
        {
            get { return _S_ID; }
            set { _S_ID = value; }
        }

        Type _frameType;

        public Type FrameType
        {
            get { return _frameType; }
            set { _frameType = value; }
        }

       

        public enum Type
        {
            Verb,
            Noun
        };



            
    }
}
