using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OntologyLibrary.OntoSem;

namespace MapperTool
{
	public class LoadConcepts
	{
		public static string[] LoadGetFile()
		{
			StreamReader sr = new StreamReader(@"Formatted OntoSem\Get.txt");
			string str = null;
			int x = 0;
			List<string> items = new List<string>();
			while ((str = sr.ReadLine()) != null)
			{
				if (str == "")
					continue;
				items.Add(str);
			}

			string[] itemsArr = items.ToArray();
			sr.Close();
			return itemsArr;
		}
		private static Ontology _ontology;
		static LoadConcepts()
		{
			_ontology = new Ontology("Formatted OntoSem");
		}
		public static Ontology Ontology
		{
			get
			{
				return LoadConcepts._ontology;
			}
		}
	}
}
