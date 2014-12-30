using System;
using System.Collections.Generic;
using System.Text;

namespace OntologyConceptsEditor
{
    [Serializable]
    public class Relation
    {
        public RelationType type;
        /// <summary>
        /// not used but may be used later with fuzzy
        /// </summary>
        private double degree;
        public Vertex from;
        public Vertex to;

        public Relation(Vertex vertexFrom, Vertex vertexTo, RelationType typeOfRel)
        {
            this.from = vertexFrom;
            this.to = vertexTo;
            this.type = typeOfRel;
        }
    }
}
