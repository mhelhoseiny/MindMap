using System;
using System.Collections.Generic;
using System.Text;

namespace OntologyConceptsEditor
{
    [Serializable]
    public class Vertex
    {
        public string Name;
        public List<Relation> Edges;
        public bool isVisited;
        public Relation Previous;

        public Vertex()
        {
            isVisited = false;
            this.Previous = null;
            this.Edges = new List<Relation>();
        }

        public Vertex(string name):this()
        {
            this.Name = name;
        }
    }
}
