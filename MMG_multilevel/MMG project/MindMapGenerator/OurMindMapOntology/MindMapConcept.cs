using System;
using System.Collections.Generic;
using System.Text;
using WordsMatching;
using OwlDotNetApi;

namespace OurMindMapOntology
{
    [Serializable]
    public class MindMapConcept
    {
		private string _parentConceptName;

		public string ParentConceptName
		{
			get
			{
				return _parentConceptName;
			}
			set
			{
				_parentConceptName = value;
			}
		}
        private string _name;
        private string _defintion;
        private List<MyWordInfo> _maplex;
        private MindMapConcept _parent;
        private Dictionary<string, List<string>> _NonVisualProperties;
        private List<MindMapConcept> _disjoints;
        private Dictionary<string, List<string>> _VisualProperties;
		//TODO: choose the name of the file
		//TODO: handle the parent of the concept.
		//TODO: handle the properties of the concept.
		//TODO: handle the visual properties of the concept.
		//public MindMapConcept(string conceptName, string fileName)
		//{
		//    OntologyReaderHelper helper = new OntologyReaderHelper(fileName);
		//    IOwlLiteral conceptLiteral = helper.GetConcept(conceptName);
		//    this.Defintion = helper.GetDefinition(conceptLiteral);
		//    this.Maplex = helper.GetMaplex(conceptLiteral);
		//    this.Name = conceptName;
		//    this._parentConceptName = helper.GetParent(conceptLiteral).ID;
		//    //this.pa
		//}
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> VisualProperties
        {
            get { return _VisualProperties; }
            set { _VisualProperties = value; }
        }

        public List<MindMapConcept> Disjoints
        {
            get { return _disjoints; }
            set { _disjoints = value; }
        }

		public Dictionary<string, List<string>> NonVisualProperties
        {
            get { return _NonVisualProperties; }
            set { _NonVisualProperties = value; }
        }

        public MindMapConcept Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        //TODO:select the sense and sense number with the mapped words
        public List<MyWordInfo> Maplex
        {
            get { return _maplex; }
            set { _maplex = value; }
        }

        public string Defintion
        {
            get { return _defintion; }
            set { _defintion = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Text
        {
            get { return Name.Split('^')[0]; }

        }
        /// <summary>
        ///    
        /// </summary>
        /// <returns>
        /// Returns Distance Info That Contains -->
		/// int Distance between C1 & C2
        /// Distance = c1ToParent + c2ToParent
		/// Return MindMapConcept parent --> The first Parent they have
		/// Return string path --> The Path between the two Concepts
		/// Return int c1ToParent --> The Distance between C1 & Parent of the two
		/// Return int c2ToParent --> The Distance between C2 & Parent of the two
        /// </returns>
        public static DistanceInfo Distance(MindMapConcept c1, MindMapConcept c2)
        {
            if (c1 == c2)
            {
                return new DistanceInfo(0, c1, 0, 0, new List<MindMapConcept>(), new List<MindMapConcept>());
            }
            List<MindMapConcept> c1Parents = new List<MindMapConcept>(), c2Parents = new List<MindMapConcept>();
            c1Parents.Add(c1);
            while (c1.Parent != null)
            {
                c1 = c1.Parent;
                c1Parents.Add(c1);
            }
            c2Parents.Add(c2);
            while (c2.Parent != null)
            {
                c2 = c2.Parent;
                c2Parents.Add(c2);
            }
            while (c1Parents[c1Parents.Count - 1] == c2Parents[c2Parents.Count - 1])
            {
                c1Parents.RemoveAt(c1Parents.Count - 1);
                c2Parents.RemoveAt(c2Parents.Count - 1);
                if (c1Parents.Count == 0)
                    return new DistanceInfo(c2Parents.Count, c2Parents[c2Parents.Count - 1].Parent, 0, c2Parents.Count, c1Parents, c2Parents);
                else if (c2Parents.Count == 0)
                    break;
            }
            return new DistanceInfo(c2Parents.Count + c1Parents.Count, c1Parents[c1Parents.Count - 1].Parent, c1Parents.Count, c2Parents.Count, c1Parents, c2Parents);
		}
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			return ((MindMapConcept)(obj)).Name == Name;
		}
		public static bool operator ==(MindMapConcept c1,MindMapConcept c2)
		{
			if ((object)c1 == null)
				if ((object)c2 == null)
					return true;
				else
					return false;
			return c1.Equals(c2);
		}
		public static bool operator !=(MindMapConcept c1, MindMapConcept c2)
		{
			return !(c1 == c2);
		}
		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
		public override string ToString()
		{
			return "Name="+Name+",Parent="+ParentConceptName+",Maplex="+Maplex[0].Word+" ... ";
		}
    }
}
