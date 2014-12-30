using System;
using System.Collections.Generic;
using System.Text;

namespace OntologyLibrary
{
    [Serializable]
    public class WordOlogy
    {
        public int ID=0;
        public string Word = "";
        public string Concept = "";
        public string Sense = "";
        public string SenseNo = "";
        public string Pos = "";

        public WordOlogy()
        { }
        public WordOlogy(int id,string word,string concept,string sense,string senseno,string pos)
        {
            ID = id;
            Word = word;
            Concept = concept;
            Sense = sense;
            SenseNo = senseno;
            Pos = pos;
        }
    }
}
