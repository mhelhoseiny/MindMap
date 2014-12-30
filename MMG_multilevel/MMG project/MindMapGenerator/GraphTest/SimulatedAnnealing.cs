using System;
using System.Collections.Generic;
using System.Text;

namespace GraphTest
{
    class SimulatedAnnealing
    {
        double T = 40000000;
        double Lamda = 0.99;
        double Cost = double.MaxValue;
        double CurrentCost;
        double Lamda1 = 3, Lamda2 = 0, Lamda3 = 7, Lamda4 = 10000, Lamda5 = 50;
        double Radius = 10;
        public List<Edge> Edges = new List<Edge>();
        public List<Edge> MinimumEdges = new List<Edge>();
        public List<Node> Nodes = new List<Node>();
        public List<Node> MinimumNodes = new List<Node>();
        public double top = -400, bottom = 400, right = 400, left = -400;
        Random random = new Random();

        public SimulatedAnnealing(List<Node> nodes, List<Edge> edges)
        {
            this.Nodes = nodes;
            this.Edges = edges;
            CalculateCost();
            if (Cost > CurrentCost || (random.Next() < Math.Pow(Math.E, (Cost - CurrentCost / T))))
            {
                Cost = CurrentCost;
                MinimumEdges = Edges;
                MinimumNodes = Nodes;
            }
            PerformSimulatedAnnealing();
        }

        private void PerformSimulatedAnnealing()
        {
            while (T > 0.01)
            {
                int vertexID = random.Next(0, Nodes.Count - 1);
                int angle = random.Next(0, 360);
                Nodes[vertexID].Xposition += Radius * (Math.Cos(angle / 180 * Math.PI));
                Nodes[vertexID].Yposition += Radius * (Math.Sin(angle / 180 * Math.PI));
                Radius = Lamda * Radius;
                CalculateCost();
                if (Cost > CurrentCost)
                {
                    Cost = CurrentCost;
                    MinimumEdges = Edges;
                    MinimumNodes = Nodes;
                }
                T *= Lamda;
            }
            InitializeDimensions();
        }

        private void InitializeDimensions()
        {
            double YMaxValue = double.MinValue;
            double YMinValue = double.MaxValue;
            double XMaxValue = double.MinValue;
            double XMinValue = double.MaxValue;

            foreach (Node n in MinimumNodes)
            {
                if (n.Xposition < XMinValue)
                    XMinValue = n.Xposition;
                else if (n.Xposition > XMaxValue)
                    XMaxValue = n.Xposition;
                if (n.Yposition < YMinValue)
                    YMinValue = n.Yposition;
                else if (n.Yposition > YMaxValue)
                    YMaxValue = n.Yposition;
            }

            left = XMinValue - 20;
            right = XMaxValue + 20;
            top = YMinValue - 20;
            bottom = YMaxValue + 20;

        }
        private void CalculateCost()
        {
            CurrentCost = 0;
            double Distance;
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    if (i != j)
                    {
                        Distance = Math.Sqrt(Math.Pow(Nodes[i].Xposition - Nodes[j].Xposition, 2) + Math.Pow(Nodes[i].Yposition - Nodes[j].Yposition, 2));
                        CurrentCost += (Lamda1 / Distance);
                    }
                }
            }
            double Distancetop, Distancebottom, Distanceleft, Distanceright;
            foreach (Node N in Nodes)
            {
                Distancetop = Math.Abs(N.Yposition - top);
                Distancebottom = Math.Abs(N.Yposition - bottom);
                Distanceleft = Math.Abs(N.Xposition - left);
                Distanceright = Math.Abs(N.Xposition - right);

                CurrentCost += Lamda2 * (1 / (Distancetop * Distancetop) + 1 / (Distanceright * Distanceright) + 1 / (Distanceleft * Distanceleft) + 1 / (Distancebottom * Distancebottom));
            }
            double length;
            foreach (Edge E in Edges)
            {
                length = Math.Sqrt(Math.Pow(E.Node1.Xposition - E.Node2.Xposition, 2) + Math.Pow(E.Node1.Yposition - E.Node2.Yposition, 2));
                CurrentCost += Lamda3 * length * length;
            }

            int X = CalculateEdgeCrossings();

            CurrentCost += Lamda4 * X;

            //for (int i = 0; i < Nodes.Count; i++)
            //{
            //    foreach (Node N in Nodes[i].Neighbours)
            //    {
            //        CurrentCost += (Lamda5 / (Math.Pow(Nodes[i].Xposition - N.Xposition, 2) + Math.Pow(Nodes[i].Yposition - N.Yposition, 2)));
            //    }
            //}
        }
        private int CalculateEdgeCrossings()
        {
            double slope1;
            double slope2;
            double b1;
            double b2;
            double X;
            double Y;
            int Counter = 0;

            for (int i = 0; i < Edges.Count - 1; i++)
            {
                for (int j = i + 1; j < Edges.Count; j++)
                {


                    slope1 = getSlope(Edges[i]);
                    b1 = getB(Edges[i], slope1);
                    slope2 = getSlope(Edges[j]);
                    b2 = getB(Edges[j], slope2);
                    if (slope1 != slope2)
                    {
                        X = (b2 - b1) / (slope1 - slope2);
                        Y = slope2 * X + b2;
                        if (((Edges[i].Node1.Xposition - X) * (X - Edges[i].Node2.Xposition) >= 0) && ((Edges[i].Node1.Yposition - Y) * (Y - Edges[i].Node2.Yposition) >= 0) && ((Edges[j].Node1.Xposition - X) * (X - Edges[j].Node2.Xposition) >= 0) && ((Edges[j].Node1.Yposition - Y) * (Y - Edges[j].Node2.Yposition) >= 0))
                            Counter++;
                    }

                }
            }
            return Counter;
        }

        private double getB(Edge edge, double slope)
        {
            return (edge.Node1.Yposition - (slope * edge.Node1.Xposition));
        }

        private double getSlope(Edge edge)
        {
            double deltaY = Math.Abs(edge.Node1.Yposition - edge.Node2.Yposition);
            double deltaX = Math.Abs(edge.Node1.Xposition - edge.Node2.Xposition);

            return (deltaY / deltaX);
        }

        
    }
}
