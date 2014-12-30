using System;
using System.Collections.Generic;
using System.Text;

namespace GraphTest
{
	[Serializable]
    class FloydWarshall
    {
        public List<Node> NodesList;
        public List<Edge> EdgesList;
        public double[,] Dij;
        private double[,] _wij;

        public double[,] Wij
        {
            get { return _wij; }
        }

        public FloydWarshall(List<Node> nodes, List<Edge> edges)
        {
            this.NodesList = nodes;
            this.EdgesList = edges;
            Initialize();
            Run();

        }
        public void Initialize()
        {
            Dij = new double[NodesList.Count, NodesList.Count];
            _wij = new double[NodesList.Count, NodesList.Count];
            
            for (int i = 0; i < NodesList.Count; i++)
            {
                for (int j = 0; j < NodesList.Count; j++)
                {
                    if (i == j)
                        _wij[i,j] = Dij[i, j] = 0;
                    else
                        _wij[i, j] = Dij[i, j] = double.MaxValue;
                }
            }
            foreach (Edge E in EdgesList)
            {
                _wij[E.Node1.index, E.Node2.index] = Dij[E.Node1.index, E.Node2.index] = Math.Sqrt(Math.Pow(E.Node1.Xposition - E.Node2.Xposition, 2) + Math.Pow(E.Node1.Yposition - E.Node2.Yposition, 2));

                _wij[E.Node2.index, E.Node1.index] = Dij[E.Node2.index, E.Node1.index] = Math.Sqrt(Math.Pow(E.Node1.Xposition - E.Node2.Xposition, 2) + Math.Pow(E.Node1.Yposition - E.Node2.Yposition, 2));
            }

        }
        public void Run()
        {
            
            for (int k = 0; k < NodesList.Count; k++)
            {
                double[,] NewDij = new double[NodesList.Count, NodesList.Count];
                for (int i = 0; i < NodesList.Count; i++)
                    for (int j = 0; j < NodesList.Count; j++)
                        //if (Dij[i, j] > Dij[i, k] + Dij[k, j])
                        //    Dij[i, j] = Dij[i, k] + Dij[k, j];
                        NewDij[i, j] = min(i, j, k);
                Dij = NewDij;
            }

        }
        private double min(int i, int j, int k )
        {
            double d_ij =Dij[i,j];
            double d_ik =Dij[i,k];
            double d_kj =Dij[k,j];
            if (d_ik == double.MaxValue || d_kj == double.MaxValue) return d_ij;
            else if (d_ij <= d_ik + d_kj)
                return d_ij;
            else
                return d_ik + d_kj;
        }
    }
}
