using System;
using System.Collections.Generic;
using System.Text;

namespace GraphTest
{
	[Serializable]
    public class Edge
    {
        public Node Node1;
        public Node Node2;

        public Edge(Node n1, Node n2)
        {
            this.Node1 = n1;
            this.Node2 = n2;
        }
    }
}
