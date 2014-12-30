using System;
using System.Collections.Generic;
using System.Text;
using WordNetClasses;
using Wnlib;
using WnLexicon;
using WNBTBox;
using System.Collections;
using SyntacticAnalyzer;

namespace MMG
{
    class WSDReplace
    {
        public static bool IsHuman(string strSense)
        {
            bool blnIsHuman = false;
            Search se;
            SearchType sty;
            PartOfSpeech pos = null;
            if (strSense=="")
                return false;
            string[] strarr = strSense.Split('#');
            
            if (strarr.Length > 1)
            {
                switch (strarr[1])
                {
                    case "n":
                        pos = PartOfSpeech.of(PartsOfSpeech.Noun);
                        break;
                    default:
                        // there is an error
                        break;
                }
            }


            sty = new SearchType(true, "HYPERPTR");

            //se = new Search(strarr[0], true, pos, sty, int.Parse( strarr[2]));
           
            se = new Search(strarr[0], true, pos, sty, int.Parse(strarr[2]));

            SynSetList slist = se.senses;

            blnIsHuman = IsHumanHelper(slist);

            return blnIsHuman;
        }

        public static bool IsHumanHelper(SynSetList slist)
        {
            bool blnIsHuman = false;
            if (slist != null)
            {
                foreach (SynSet sset in slist)
                {
                    foreach (Lexeme lxm in sset.words)
                    {
                        if (lxm.word.ToUpper() == "PERSON" && lxm.wnsns == 1)
                        {
                            return true;
                        }
                    }
                    blnIsHuman = blnIsHuman | IsHumanHelper(sset.senses);
                    if (blnIsHuman)
                    {
                        return blnIsHuman;//If match found then do early exist from recursion
                    }
                }
            }
            return blnIsHuman;
        }

        // pass a tree and a node object to this function to get the actual text string that represents it
        //example: to get the word "he" from a sentence like this "He playes football" pass the whole parse tree 
        //object and the node which has "NNC" as its goal
        private static string GetWordString(ParseTree tree, ParseNode node)
        {
            string str = (string)tree.Words[node.Start];
            for (int i = node.Start + 1; i < node.End; i++)
                str += "_" + (string)tree.Words[i];
            return str;
        }

        //this methos discribes how to consume discourse
        private void UseDisCourse(object sender, System.EventArgs e)
        {
            Discourse d = new Discourse();
            ArrayList SParseTrees = null;
            d.Begin(SParseTrees);//Here pass the parse trees array list
            ArrayList DisClasses = d.DistinctClasses;
            //Note:-you may not understand this paragragh accurately if so do not 
            //care and read the example below
            //
            //this dictionary is an arraylist of array lists 
            //each word being refered to has an outer arraylist.the inner array list contains DiscourseEntery 
            //objects the first one represents the noun that is being refered to and the others represent the pronounsnoun nodes referencing
            //the original noun

            //Example:-
            //for sentences like this:
            //The boy plays football.He is happy.He is not crying.The Red planet is small.It is not soo far
            //in this example the DistinctClasses will contain two arraylist objects 1 For "The boy" 
            //and the other for the "Red Planet".
            //the First array list for the "The boy" will contain 3 Dictionary enteries
            //1.Dictionary entery that contain the tree node for the "The boy"
            //2.Dictionary entery that contain the tree node for the first "HE"
            //3.Dictionary entery that contain the tree node for the Second "HE"
            //
            //the First array list for the "The Red Planet" will contain 2 Dictionary enteries
            //1.Dictionary entery that contain the tree node for the "The Red Planet"
            //3.Dictionary entery that contain the tree node for the Second "it"
            //
            //Note:- that the noun being refered to is always the first Dictionary entery in its arraylist
            

            //this loop generates a string that contain every referenced noun and the pronouns that refer to it
            string actualword = "";
            foreach (ArrayList arr in DisClasses)
            {
                foreach (DiscourseEntry de in arr)
                {
                    // here we get the acual text string that represents the node 
                    string str = GetWordString((ParseTree)SParseTrees[de.TreeNum], de.Node) +
                        " (" + ((int)(de.TreeNum + 1)).ToString() + "." + ((int)(de.Node.Start + 1)).ToString() + ")";
                    actualword+= str + "\r\n";
                } 
                actualword+= "***************************************************\r\n";
            }
        }
    }



}
