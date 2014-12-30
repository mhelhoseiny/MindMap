using System;
using System.Collections.Generic;
using System.Text;

namespace GraphTest
{
	[Serializable]
    public class Node
    {
        public double Xposition;
        public double Yposition;
        public int index;
        public double energy;
        public List<Node> Neighbours;


        public Node(double X, double Y, int _index)
        {
            this.Xposition = X;
            this.Yposition = Y;
            this.index = _index;
            this.Neighbours = new List<Node>();
        }

        public int Index
        {
            get
            {
                return index;
            }
        }
    }
}
