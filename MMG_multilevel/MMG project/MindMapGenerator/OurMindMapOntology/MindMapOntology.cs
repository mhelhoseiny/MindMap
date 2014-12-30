using System;
using System.Collections.Generic;
using System.Text;
using OwlDotNetApi;
using WnLexicon;
using WordsMatching;
using Wnlib;
using System.Collections;
using System.IO;

namespace OurMindMapOntology
{
    [Serializable]
	public class MindMapOntology
	{
		const string eSubClass = "http://www.w3.org/2000/01/rdf-schema#subClassOf";
		const string eLabel = "http://www.w3.org/2000/01/rdf-schema#label";
		const string eDefinition = "http://www.semanticweb.org/ontologies/2009/3/OntologyAll.owl#DEFINITION";
		const string eMaplex = "http://www.semanticweb.org/ontologies/2009/3/OntologyAll.owl#MAPLEX";
		const string eAnnotation = "http://www.semanticweb.org/ontologies/2009/3/OntologyAll.owl#";
		Dictionary<string, string> annotations = new Dictionary<string, string>();
		private Dictionary<string, MindMapConcept> _concepts;

		public Dictionary<string, MindMapConcept> Concepts
		{
			get
			{
				return _concepts;
			}
			set
			{
				_concepts = value;
			}
		}
		public List<List<string>> SetAllPropertiesCombinations(string path)
		{
			List<List<string>> result = new List<List<string>>();
			foreach (KeyValuePair<string,MindMapConcept> pair in Concepts)
			{
				result.Add(SetPropertiesCombinations(path, pair.Value));
			}
			return result;
		}
		public List<string> SetPropertiesCombinations(string path, MindMapConcept concept)
		{
			if (!Directory.Exists(concept.Name))
				Directory.CreateDirectory(concept.Name);
			List<string> visualProperties = new List<string>(concept.VisualProperties.Keys);
			return CreateCombination(path, concept, 0, new List<string>(), visualProperties);
		}
		List<string> CreateCombination(string path, MindMapConcept concept, int PropertyIndex,List<string>previous,List<string>visualProperties)
		{
			if (PropertyIndex == visualProperties.Count)
			{
				if(previous.Count==0)
					return new List<string>(new string[]{concept.Name});
				List<string> adjOrder = new List<string>(previous);
				adjOrder.Sort();
				string fileName = adjOrder[0];
				for (int i = 1; i < adjOrder.Count; i++)
				{
					fileName += " " + adjOrder[i];
				}
				return new List<string>(new string[] { fileName });
			}
			else
			{
				List<string> result = new List<string>();
				result.AddRange(CreateCombination(path, concept, PropertyIndex + 1, previous,visualProperties));
				for (int i = 0; i < concept.VisualProperties[visualProperties[PropertyIndex]].Count; i++)
				{
					previous.Add(concept.VisualProperties[visualProperties[PropertyIndex]][i]);
					result.AddRange(CreateCombination(path, concept, PropertyIndex + 1, previous, visualProperties));
					previous.RemoveAt(previous.Count - 1);
				}
				return result;
			}
		}
		public MindMapOntology(string fileName)
		{
			Concepts = new Dictionary<string, MindMapConcept>();
			IOwlGraph graph;
			IOwlParser parser = new OwlXmlParser();
			object o=parser.ParseOwl(fileName);
			graph = (IOwlGraph)o;
			foreach (DictionaryEntry entry in graph.Nodes)
			{
				IOwlNode node = (IOwlNode)entry.Value;
				if (node.GetType()==typeof(OwlAnnotationProperty)&&node.ID!=eMaplex&&node.ID!=eDefinition)
					annotations.Add(node.ID,((string)entry.Key).Substring(eAnnotation.Length));
			}
			foreach (DictionaryEntry entry in graph.Literals)
			{
				IOwlLiteral literal = (IOwlLiteral)entry.Value;
				if (literal.ParentEdges[0].ParentNode.ChildEdges[eLabel, 0]==null||((IOwlLiteral)(literal.ParentEdges[0].ParentNode.ChildEdges[eLabel, 0].ChildNode)) != literal)
					continue;
				MindMapConcept concept = new MindMapConcept();
				concept.Name = literal.ID;
				concept.Defintion = GetDefinition(literal);
				concept.VisualProperties = GetVisualFeatures(literal);
				concept.NonVisualProperties = GetNonVisualFeatures(literal);
				concept.Maplex = GetMaplex(literal);
				if (concept.Name.ToUpper() != "THING" && GetParent(literal)!=null)
					concept.ParentConceptName = GetParent(literal).ID;
				else
					concept.ParentConceptName = "";
				Concepts.Add(concept.Name, concept);
                //Concepts.Add(concept.Name.Split('^')[0], concept);

			}
			foreach (KeyValuePair<string, MindMapConcept> concept in Concepts)
			{
				if (concept.Value.Name.ToUpper() != "THING"&&concept.Value.ParentConceptName!=null&&concept.Value.ParentConceptName!="")
					try
					{
						concept.Value.Parent = Concepts[concept.Value.ParentConceptName];

					}
					catch (Exception)
					{
						
						concept.Value.Parent = null;
					}
				else
					concept.Value.Parent = null;
			}
		}
		string GetDefinition(IOwlLiteral conceptLiteral)
		{
			try
			{
				OwlNode nodeClass = (OwlNode)conceptLiteral.ParentEdges[0].ParentNode;
				return nodeClass.ChildEdges[eDefinition, 0].ChildNode.ID.Split('^')[0];

			}
			catch (Exception)
			{
				return "";
			}
		}
		string GetDefinition(string conceptName)
		{
			return GetDefinition(GetConcept(conceptName));
		}
		OwlLiteral GetParent(IOwlLiteral conceptLiteral)
		{
			try
			{
				OwlNode nodeClass = (OwlNode)conceptLiteral.ParentEdges[0].ParentNode;
				OwlNode parentClass = (OwlNode)nodeClass.ChildEdges[eSubClass, 0].ChildNode;
				return (OwlLiteral)parentClass.ChildEdges[eLabel, 0].ChildNode;

			}
			catch (Exception)
			{
				return null;
			}
		}
		OwlLiteral GetParent(string conceptName)
		{
			return GetParent(GetConcept(conceptName));
		}
		OwlLiteral GetConcept(string conceptName)
		{
			//TODO: repair this function
			IOwlGraph graph = null;
			return (OwlLiteral)graph.Literals[conceptName];
		}
		List<OwlLiteral> GetChildren(string conceptName)
		{
			return GetChildren(GetConcept(conceptName));
		}
		Dictionary<string, List<string>> GetVisualFeatures(IOwlLiteral literal)
		{
			Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
			OwlNode nodeClass = (OwlNode)literal.ParentEdges[0].ParentNode;

			foreach (IOwlEdge edge in nodeClass.ChildEdges)
			{
				string edgeMap = edge.ID;
				if (annotations.ContainsKey(edgeMap))
				{
					string featureName = edgeMap.Substring(eAnnotation.Length);
					if (featureName[0] != '_')
					{
						if (!result.ContainsKey(featureName))
							result.Add(featureName, new List<string>());
						List<string> featuresValue = result[featureName];
						string[] values = edge.ChildNode.ID.Split('\n');
						featuresValue.AddRange(values);
					}
				}
			}
			return result;
		}
		Dictionary<string, List<string>> GetNonVisualFeatures(IOwlLiteral literal)
		{
			Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
			OwlNode nodeClass = (OwlNode)literal.ParentEdges[0].ParentNode;

			foreach (IOwlEdge edge in nodeClass.ChildEdges)
			{
				string edgeMap = edge.ID;
				if (annotations.ContainsKey(edgeMap))
				{
					string featureName = edgeMap.Substring(eAnnotation.Length);
					if (featureName[0] == '_')
					{
						featureName = featureName.Substring(1);
						if (!result.ContainsKey(featureName))
							result.Add(featureName, new List<string>());
						List<string> featuresValue = result[featureName];
						string[] values = edge.ChildNode.ID.Split('\n');
						featuresValue.AddRange(values);
					}
				}
			}
			return result;
		}
		List<MyWordInfo> GetMaplex(IOwlLiteral conceptLiteral)
		{
			try
			{
				OwlNode nodeClass = (OwlNode)conceptLiteral.ParentEdges[0].ParentNode;
				string Maplex = nodeClass.ChildEdges[eMaplex, 0].ChildNode.ID.Split('^')[0];
				string[] allMaplexes = Maplex.Split('\n');
				List<MyWordInfo> result = new List<MyWordInfo>();
				for (int i = 0; i < allMaplexes.Length; i++)
				{
					MyWordInfo word = new MyWordInfo();
					string[] parts = allMaplexes[i].Split('-');
					word.Word = parts[0];

					//TODO: handle the adjective and adverb
					switch (parts[1][0])
					{
						case 'n':
						case 'N':
							word.Pos = PartsOfSpeech.Noun;
							break;
						case 'v':
						case 'V':
							word.Pos = PartsOfSpeech.Verb;
							break;
						case 'a':
						case 'A':
							if (parts[1][2] == 'j' || parts[1][2] == 'J')
								word.Pos = PartsOfSpeech.Adj;
							else
								word.Pos = PartsOfSpeech.Adv;
							break;
						default:
							word.Pos = PartsOfSpeech.Unknown;
							break;
					}
					int index = 1;
					for (; parts[1][index] < '0' || parts[1][index] > '9'; index++)
						;
					string senseNumber = parts[1].Substring(index);
					word.Sense = int.Parse(senseNumber);
					result.Add(word);
				}
				return result;

			}
			catch (Exception)
			{
				return new List<MyWordInfo>();
			}
		}
		List<MyWordInfo> GetMaplex(string conceptName)
		{
			return GetMaplex(GetConcept(conceptName));
		}
		List<OwlLiteral> GetChildren(IOwlLiteral conceptLiteral)
		{
			try
			{
				List<OwlLiteral> result = new List<OwlLiteral>();
				OwlNode nodeClass = (OwlNode)conceptLiteral.ParentEdges[0].ParentNode;
				IOwlEdgeCollection edges = nodeClass.ParentEdges;
				foreach (IOwlEdge edge in edges)
				{
					OwlNode childClass = (OwlNode)edge.ParentNode;
					result.Add((OwlLiteral)childClass.ChildEdges[eLabel, 0].ChildNode);
				}
				return result;

			}
			catch (Exception)
			{
				return new List<OwlLiteral>();
			}
		}
		//public List<IOwlLiteral>GetChildren(string 
	}
}
