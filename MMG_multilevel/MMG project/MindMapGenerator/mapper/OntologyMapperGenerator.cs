using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OntologyLibrary.OntoSem;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using WordNetClasses;
using Wnlib;
using WordsMatching;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
namespace OntologyLibrary
{
    class OntologyMapperGenerator
    {
        private string _ontologyDirectoryPath;
        private int conceptcounter = 0;
        
        ArrayList wordologyArr = new ArrayList();
       
        public string OntologyDirectoryPath
        {
            get { return _ontologyDirectoryPath; }
        }
        
        public OntologyMapperGenerator(string ontologyDirectoryPath)
        {
            _ontologyDirectoryPath = ontologyDirectoryPath;
        }
        private Ontology Onto = null;
        private int ID = -1;
        private int CannotGetSenseExeption = 0;
       
        void LoadOntology()
        {
            if (Onto == null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                System.IO.FileStream fs = new System.IO.FileStream(
                    _ontologyDirectoryPath + @"\Ontology.bin", FileMode.Open);
                Onto = (Ontology)bf.Deserialize(fs);
                fs.Close();
            }
           
        }
        public static void GenerateOntologyBinFile()
        {

            string _ontologyDirectoryPath =
                @"..\..\..\Ontology\Formatted OntoSem\";
            Ontology onto = new Ontology(_ontologyDirectoryPath);
            StreamReader sr = new StreamReader(onto.OntologyPath + "\\Get.txt");
            string str = null;
            while ((str = sr.ReadLine()) != null)
            {
                if (str == "")
                    continue;
                onto.LoadConcept(str);
            }
            sr.Close();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(
                onto.OntologyPath + "\\Ontology.bin", FileMode.Create);
            bf.Serialize(fs, onto);
            fs.Close();
            //MessageBox.Show("Finished");
        }
        
        public void ConstructMapping()
        {

            string concept="";
            string word="";
            string senseNo="";
            string Sense="";
            string Pos="";
            
            
            
            LoadOntology();
            FileStream allConceptsFile = new FileStream(_ontologyDirectoryPath + @"\AllConcepts.txt", FileMode.Open);
            StreamReader allConceptsFileReader = new StreamReader(allConceptsFile);

            string _wordologyDirectoryPath =
           @"..\..\..\wordology\";

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(
               _wordologyDirectoryPath + "\\wordology.txt", FileMode.Create);
            int indxWatcherconceptCounter = 0;
            int NoMapLexConcepts = 0;
            int AllSensesMapped = 0;
            
            while ((concept = allConceptsFileReader.ReadLine()) != null)
            {
                indxWatcherconceptCounter++;
                string Conceptpath = _ontologyDirectoryPath + @"\" + concept[0] + @"\" + concept;
                Concept C = (Concept)Onto[concept];
                Property maplexProperty = C.FullProperties["ENGLISH1"];

                if (maplexProperty != null)
                {
                    MapConceptsWithMapLex(concept,maplexProperty);
                }
                else
                {
                    NoMapLexConcepts++;
                    MapConceptsWithOutMapLex(concept);
                }
               
            }//end while
            allConceptsFileReader.Close();
            allConceptsFile.Close();
            bf.Serialize(fs, wordologyArr);
            fs.Close();
            MessageBox.Show("no map-lex concepts number = "+NoMapLexConcepts.ToString());
            MessageBox.Show("can't getsense pos number = " + CannotGetSenseExeption.ToString());
            MessageBox.Show(conceptcounter.ToString());
            
        }

        private void MapConceptsWithOutMapLex(string concept)
        {

            Wnlib.Index index;
            Wnlib.PartOfSpeech p;
            Search se;
            WordOlogy WO=new WordOlogy();
            int NoOfSensesSucceeded = 0;

            try
            {
                index = Wnlib.Index.lookup(concept.ToLower(), PartOfSpeech.of(PartsOfSpeech.Noun));
                if (index != null)
                {
                    WO.Pos = "noun";
                    Opt[] relatedness = WordsMatching.Relatedness.GetRelatedness(PartsOfSpeech.Noun);
                    foreach (Opt o in relatedness)
                    {
                        for (int senseNumber = 0; senseNumber < index.sense_cnt; senseNumber++)
                        {
                            se = new Search(concept, true, PartOfSpeech.of("noun"), o.sch, senseNumber);
                            SynSet sense = new SynSet(index, senseNumber, se);
                            WO.Concept = concept;
                            WO.Word = concept;
                            WO.Sense = sense.defn;
                            WO.ID = ID;
                            ID++;
                            NoOfSensesSucceeded++;
                            //AllSensesMapped++;
                            //bf.Serialize(fs, "\n" + WO);
                            wordologyArr.Add(WO);
                            
                        }
                    }
                }


            }
            catch
            { }
            try
            {

                index = Wnlib.Index.lookup(concept.ToLower(), PartOfSpeech.of(PartsOfSpeech.Verb));
                if (index != null)
                {

                    WO.Pos = "verb";
                    Opt[] relatedness = WordsMatching.Relatedness.GetRelatedness(PartsOfSpeech.Verb);
                    foreach (Opt o in relatedness)
                    {
                        for (int senseNumber = 0; senseNumber < index.sense_cnt; senseNumber++)
                        {
                            se = new Search(concept, true, PartOfSpeech.of("verb"), o.sch, senseNumber);
                            SynSet sense = new SynSet(index, senseNumber, se);
                            WO.Sense = sense.defn;
                            WO.Concept = concept;
                            WO.Word = concept;
                            WO.ID = ID;
                            ID++;
                            NoOfSensesSucceeded++;
//                            AllSensesMapped++;
                            //bf.Serialize(fs, "\n" + WO);
                            wordologyArr.Add(WO);
                        }
                    }
                }


            }
            catch
            { }
            try
            {

                index = Wnlib.Index.lookup(concept.ToLower(), PartOfSpeech.of(PartsOfSpeech.Adj));
                if (index != null)
                {

                    WO.Pos = "adj";
                    Opt[] relatedness = WordsMatching.Relatedness.GetRelatedness(PartsOfSpeech.Adj);
                    foreach (Opt o in relatedness)
                    {
                        for (int senseNumber = 0; senseNumber < index.sense_cnt; senseNumber++)
                        {
                            se = new Search(concept, true, PartOfSpeech.of("adj"), o.sch, senseNumber);
                            SynSet sense = new SynSet(index, senseNumber, se);
                            WO.Sense = sense.defn;
                            WO.Concept = concept;
                            WO.Word = concept;
                            WO.ID = ID;
                            ID++;
                            NoOfSensesSucceeded++;
                            //AllSensesMapped++;
                            //bf.Serialize(fs, "\n" + WO);
                            wordologyArr.Add(WO);
                        }
                    }
                }


            }
            catch
            { }
            try
            {

                index = Wnlib.Index.lookup(concept.ToLower(), PartOfSpeech.of(PartsOfSpeech.Adv));
                if (index != null)
                {

                    WO.Pos = "adv";
                    Opt[] relatedness = WordsMatching.Relatedness.GetRelatedness(PartsOfSpeech.Noun);
                    foreach (Opt o in relatedness)
                    {
                        for (int senseNumber = 0; senseNumber < index.sense_cnt; senseNumber++)
                        {
                            se = new Search(concept, true, PartOfSpeech.of("adv"), o.sch, senseNumber);
                            SynSet sense = new SynSet(index, senseNumber, se);
                            WO.Sense = sense.defn;
                            WO.Concept = concept;
                            WO.Word = concept;
                            WO.ID = ID;
                            ID++;
                            NoOfSensesSucceeded++;
                            //AllSensesMapped++;
                            //bf.Serialize(fs, "\n" + WO);
                            wordologyArr.Add(WO);
                        }
                    }
                }


            }
            catch
            { }


            if (NoOfSensesSucceeded != 0)
                conceptcounter++;
 

        }

        private void MapConceptsWithMapLex(string concept, Property maplexProperty)
        {
            MyWordInfo mwi ;
            WordOlogy WO=new WordOlogy();
            
            List<MyWordInfo> maplexsenses = new List<MyWordInfo>();
            int NoOfSensesSucceeded = 0;
            for (int i = 0; i < maplexProperty.Fillers.Count; i++)
            {
                string tmp = maplexProperty.Fillers[i].ScalarFiller;
                char[] charr = new char[] { '-', '_' };
                string[] splt = tmp.Split(charr);
                //there r fillers with no type & a-bomb masalan 

                if (splt.Length > 1)
                {
                    mwi = new MyWordInfo();
                    for (int k = 0; k < splt.Length - 2; k++)
                    {
                        mwi.Word += splt[k] + " ";
                    }
                    mwi.Word += splt[splt.Length - 2];
                    if (splt[splt.Length - 1].Length == 2)
                    {
                        if (splt[splt.Length - 1][0] == 'v')
                        {
                            mwi.Pos = Wnlib.PartsOfSpeech.Verb;
                        }
                        else if (splt[splt.Length - 1][0] == 'n')
                        {
                            mwi.Pos = Wnlib.PartsOfSpeech.Noun;
                        }
                        else if (splt[splt.Length - 1][0] == 'a')
                        {
                            mwi.Pos = Wnlib.PartsOfSpeech.Adj;
                        }
                        else if (splt[splt.Length - 1][0] == 'r')
                        {
                            mwi.Pos = Wnlib.PartsOfSpeech.Adv;
                        }
                        else
                        {
                            mwi.Pos = Wnlib.PartsOfSpeech.Unknown;
                        }
                    }
                    else
                    {
                        mwi.Pos = Wnlib.PartsOfSpeech.Unknown;
                        mwi.Word += " " + splt[splt.Length - 1];
                    }
                    if (i == 0 || (maplexsenses.Count > 0 && (mwi.Word != maplexsenses[maplexsenses.Count - 1].Word || mwi.Pos != maplexsenses[maplexsenses.Count - 1].Pos)))
                    {
                        maplexsenses.Add(mwi);

                    }

                }
                //ne loop 3al ontology kolaha
            }


            if (maplexsenses.Count > 0)
            {
                MyWordInfo[] maplexArray = new MyWordInfo[maplexsenses.Count];
                for (int j = 0; j < maplexsenses.Count; j++)
                {
                    maplexArray[j] = maplexsenses[j];
                }
                WordSenseDisambiguator wsd = new WordSenseDisambiguator();
                MyWordInfo[] res = new MyWordInfo[maplexArray.Length];
                res = wsd.Disambiguate(maplexArray);
                int i = 0;

                foreach (MyWordInfo wi in res)
                {
                    string tmp = maplexProperty.Fillers[i].ScalarFiller;
                    char[] charr = new char[] { '-', '_' };
                    string[] splt = tmp.Split(charr);

                    if (splt.Length > 1 && splt[splt.Length - 1].Length == 2)
                    {
                        WO.SenseNo = splt[splt.Length - 1];
                    }
                    else
                    {
                        // "sense doesn't have POS";
                    }

                    Wnlib.PartOfSpeech p = Wnlib.PartOfSpeech.of((Wnlib.PartsOfSpeech)wi.Pos);

                    try
                    {
                        Wnlib.Index index = Wnlib.Index.lookup(wi.Word.ToLower(), p);
                        SynSet sense = new SynSet(index, res[i].Sense, null);
                        WO.Sense = sense.defn;
                       // AllSensesMapped++;
                        NoOfSensesSucceeded++;
                        try
                        {
                            WO.Pos = p.name;
                        }
                        catch
                        {
                            WO.Pos = wi.Pos.ToString();
                        }
                        ID++;
                        WO.Word = wi.Word;
                        WO.ID = ID;
                        WO.Concept = concept;
                        wordologyArr.Add(WO);
                        
                    }
                    catch
                    {
                    };
                    if (NoOfSensesSucceeded == 0)
                    {
                        CannotGetSenseExeption++;

                    }
                    i++;
                    //bf.Serialize(fs, "\n" + WO);
                }
                conceptcounter++;
            }
        }

       


        
    }
  
  


}
