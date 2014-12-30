using System;
using System.Collections.Generic;
using System.Text;
using Wnlib;
using WnLexicon;
using WordsMatching;
namespace OurMindMapOntology
{
    public class MindMapMapper
    {
        public static MindMapConcept GetConcept(MyWordInfo word,MindMapOntology ontology)
        {
			foreach (KeyValuePair<string,MindMapConcept> pair in ontology.Concepts)
			{
				for (int i = 0; i < pair.Value.Maplex.Count; i++)
				{
					MyWordInfo maplex = pair.Value.Maplex[i];
					if (maplex.Pos==word.Pos&&maplex.Word.ToUpper()==word.Word.ToUpper()&&maplex.Sense==word.Sense)
						return pair.Value;
				}
			}
			return null;
        }

    }
}
