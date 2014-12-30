using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Wnlib;

namespace WordologyManager
{
    class WordSenses
    {
        ArrayList Senses = new ArrayList();



        public static ArrayList GetAllSenses(string word,string pos)
        {
            int senseCount = 0;
            Opt[] relatednessTypes = null;
            ArrayList senses = new ArrayList();
            MyWnLexicon.WSDWordInfo wordInfo = MyWnLexicon.Lexicon.FindWordInfo(word, true);
            Wnlib.PartsOfSpeech POS=Wnlib.PartsOfSpeech.Unknown;
            if (pos == "noun")
            {
                POS = PartsOfSpeech.Noun;
                senseCount = wordInfo.senseCounts[1];
            }
            if (pos == "verb")
            {
                POS = PartsOfSpeech.Verb;
                senseCount = wordInfo.senseCounts[2];
            }
            if (pos == "adj")
            {
                POS = PartsOfSpeech.Adj;
                senseCount = wordInfo.senseCounts[3];
            }
            if (pos == "adv")
            {
                POS = PartsOfSpeech.Adv;
                senseCount = wordInfo.senseCounts[4];
            }

            relatednessTypes = WordsMatching.Relatedness.GetRelatedness(POS);
            //senseCount = wordInfo.senseCounts[1];
            if (relatednessTypes == null) return null;
            for (int i = 0; i < senseCount; i++)
            {
                string gloss = GetRelatednessGlosses(word, i + 1, relatednessTypes);
                senses.Add(gloss);
            }

            return senses;
        }
        public static string GetRelatednessGlosses(string word, int senseNumber, Opt[] relatednessTypes)
        {
            

            for (int i = 0; i < relatednessTypes.Length; i++)
            {
                Opt relateness = relatednessTypes[i];
                Search se = new Search(word, true, relateness.pos, relateness.sch, senseNumber); //relateness.sch, senseNumber);//								
                if (se.senses != null && se.senses.Count > 0)
                {
                 string gloss = GetSynsetDefinition(se.senses[0]);
                 return gloss;    
                }
            }
            return "";
        }

        static string GetSynsetDefinition(SynSet sense)
        {
            if (sense == null) return null;
            string gloss = sense.defn;
            //			if (gloss.IndexOf(";") != -1)
            //				gloss=gloss.Substring(0, gloss.IndexOf(";")) ;
            foreach (Lexeme word in sense.words)
                gloss += " " + word.word;

            return gloss;
        }

       
    }
}
