using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GraphTest
{
    public delegate void NodePositionChangedEventHandler(int nodeIndex, double dx, double dy);
    public delegate void SingleStepUpdateEventHandler();
    public delegate double GetOriginDistance(int i, int j);



    public enum Mode
    {
        USERDEFINED, RANDOM, CIRCULAR
    };
	[Serializable]
    public class SpringModel
    {
        public List<Node> Nodes = new List<Node>();
        public List<Edge> Edges = new List<Edge>();
        public List<Node> MinimumEnergyNodes;
        double CurrentSystemEnergy = 0;
        double PreviousSystemEnergy = 0;
        double MinimumSystemEnery = double.MaxValue;
        Mode InitialAllocation;
        int minX, maxX, minY, maxY;
        int number_of_random_iterations;
        Random random;
        public GetOriginDistance _getOriginDistance;
        public event NodePositionChangedEventHandler NodePositionChanged;
        public event SingleStepUpdateEventHandler StepDoneEvent;

        public double Lij(int i,int j)
        {
            return lij[i, j];
        }
        public double Kij(int i, int j)
        {
            return kij[i, j];
        }

        double[,] lij;
        double[,] kij;
        double Lnode;
        public double Epson = 0.00000001;
        public int MaximumNumberOfIterations = 10000;
        double DesiredLength;
        int NodesCount;

        FloydWarshall FW;
        double _desiredEdgeLength;

        public void AdjustNodes(Rectangle rect)
        {
            RectangleF r =  GetGraphRectangle();
            foreach (Node node in Nodes)
            {
                node.Xposition = node.Xposition - r.X+50;
                node.Yposition = node.Yposition - r.Y+50;
            }
            
            foreach (Node node in Nodes)
            {
                
                    if(node.Xposition<rect.X|| node.Xposition>=rect.X+rect.Width)
                    {
                        double dist1 = Math.Abs(node.Xposition-rect.X);
                        double dist2 = Math.Abs(node.Xposition-rect.X-rect.Width);
                        if(dist1<dist2)
                            node.Xposition = rect.X;
                        else
                            node.Xposition = rect.X+rect.Width;
                    }
                    if(node.Yposition<rect.Y|| node.Yposition>=rect.Y+rect.Height)
                    {
                        double dist1 = Math.Abs(node.Yposition-rect.Y);
                        double dist2 = Math.Abs(node.Yposition-rect.Y-rect.Height);
                        if(dist1<dist2)
                            node.Yposition = rect.Y;
                        else
                            node.Yposition = rect.Y+rect.Height;
                    }
                

            }
        }
        double DefaultPairDistance(int i, int j)
        {
            return 0;
 
        }

        private RectangleF GetGraphRectangle()
        {
            double minX = double.MaxValue, minY = double.MaxValue, maxX = double.MinValue, maxY = double.MinValue;
            foreach (Node node in Nodes)
            {
                if (node.Xposition < minX)
                    minX = node.Xposition;
                if (node.Xposition > maxX)
                    maxX = node.Xposition;
                if (node.Yposition < minY)
                    minY = node.Yposition;
                if (node.Yposition > maxY)
                    maxY = node.Yposition;

            }
            return new RectangleF((float)minX, (float)minY, (float)(maxX - minX + 1), (float)(maxY - minY + 1));
        }
        public SpringModel(List<Node> nodes, List<Edge> edges,double desiredLength,double desiredEdgeLength, Mode allocationMode)
        {
            this.Nodes = nodes;
            this.Edges = edges;
            this.Lnode = desiredLength;
            _desiredEdgeLength = desiredEdgeLength;
            this.NodesCount = Nodes.Count;
            _getOriginDistance = DefaultPairDistance;
            if (allocationMode == Mode.RANDOM)
            {
                setRandomMode(0, 800, 0, 800, 20);
            }
            else if (allocationMode == Mode.USERDEFINED)
            {
                setUserDefinedMode();
            }
            else if (allocationMode == Mode.CIRCULAR)
            {
                setCircularMode();
            }
        }

        private void setCircularMode()
        {
            InitialAllocation = Mode.CIRCULAR;
        }

        public Node MaximumEnergyNode;
        
        public bool ChildSingleStep()
        {
            i++;
            if (MaximumEnergyNode.energy > Epson)
            {
                lock (Nodes)
                {
                    double Dx = getDeltaX(MaximumEnergyNode);
                    double Dy = getDeltaY(MaximumEnergyNode);
                    GetDxDy(MaximumEnergyNode, out Dx, out Dy);
                    MaximumEnergyNode.Xposition += Dx;
                    MaximumEnergyNode.Yposition += Dy;
                    if (NodePositionChanged != null)
                    {
                        NodePositionChanged(MaximumEnergyNode.index, Dx, Dy);
                    }
                    updateFloydWarshall();

                    getEnergy(MaximumEnergyNode);
                }
                if (StepDoneEvent != null)
                    StepDoneEvent();
                
                return true;
            }
            else
            {
                lock (Nodes)
                {
                    Nodes[MaximumEnergyNode.index] = MaximumEnergyNode;
                }
                MaximumEnergyNode = getMaximumEnergyNode();
                if (StepDoneEvent != null)
                    StepDoneEvent();
                return false;
            }

        }
        public int i = 0;

        public bool SingleStep()
        {
            CurrentSystemEnergy = getSystemEnergy();
            bool stillRunning = (Math.Abs(CurrentSystemEnergy - PreviousSystemEnergy) > 0.000000000001);

            if (i > MaximumNumberOfIterations || stillRunning == false)
                return false;

            while (ChildSingleStep())
            {
                if (i > MaximumNumberOfIterations)
                    break;
            }

            PreviousSystemEnergy = CurrentSystemEnergy;
           
            return true;

        }
        
        public void InitializeSpringModel()
        {
            NodesCount = Nodes.Count;
            lij = new double[NodesCount, NodesCount];
            kij = new double[NodesCount, NodesCount];
            if (InitialAllocation == Mode.RANDOM)
            {
                randomizePositions();
            }
            else if (InitialAllocation == Mode.CIRCULAR)
            {
                circulizePositions();
            }
            updateFloydWarshall();
            MaximumEnergyNode = getMaximumEnergyNode(); 
        }

        public void Run()
        {
            if (InitialAllocation == Mode.USERDEFINED)
            {
                SingleRun();
            }

            else if (InitialAllocation == Mode.RANDOM)
            {
                for (int j = 0; j < number_of_random_iterations; j++)
                {
                    SingleRun();
                    if (CurrentSystemEnergy < MinimumSystemEnery)
                    {
                        MinimumSystemEnery = CurrentSystemEnergy;
                        MinimumEnergyNodes.Clear();
                        foreach (Node n in Nodes)
                        {
                            Node newNode = new Node(n.Xposition, n.Yposition, n.index);
                            newNode.energy = n.energy;
                            MinimumEnergyNodes.Add(newNode);
                        }
                    }
                }
                lock (Nodes)
                {
                    for (int m = 0; m < NodesCount; m++)
                    {
                        Nodes[m].Xposition = MinimumEnergyNodes[m].Xposition;
                        Nodes[m].Yposition = MinimumEnergyNodes[m].Yposition;
                        Nodes[m].energy = MinimumEnergyNodes[m].energy;
                        Nodes[m].index = MinimumEnergyNodes[m].index;
                    }
                }
            }
            else if (InitialAllocation == Mode.CIRCULAR)
            {

                SingleRun();
            }
            this.AlignNodes();
        }

        
        private void circulizePositions()
        {
            bool[] marked = new bool[NodesCount];
            List<Node> nodes_to_allocate = new List<Node>();

            for (int i = 0; i < NodesCount; i++)
            {
                marked[i] = false;
            }

            Node node = getMaximumNeighboursNode();
            marked[node.index] = true;
            node.Xposition = 500;
            node.Yposition = 500;
            nodes_to_allocate.Add(node);

            while (nodes_to_allocate.Count >0)
            {
                double Theta = 0;
                foreach (Node n in nodes_to_allocate[0].Neighbours)
                    {
                        if (marked[n.index] == false)
                        {
                            double desiredIJ = _desiredEdgeLength + _getOriginDistance(nodes_to_allocate[0].index, n.index);
                            n.Xposition = nodes_to_allocate[0].Xposition + desiredIJ * Math.Cos((Theta / 180) * Math.PI);
                            n.Yposition = nodes_to_allocate[0].Yposition + desiredIJ * Math.Sin((Theta / 180) * Math.PI);
                            Theta += (360 / nodes_to_allocate[0].Neighbours.Count);
                            if (nodes_to_allocate.Contains(n) == false)
                            {
                                nodes_to_allocate.Add(n);
                                marked[n.index] = true;
                            }
                        }
                    }
                nodes_to_allocate.Remove(nodes_to_allocate[0]);
            }

            
        }

        private Node getMaximumNeighboursNode()
        {
            int max = 0;
            Node maxNode = null;
            foreach (Node n in Nodes)
            {
                if (n.Neighbours.Count > max)
                {
                    max = n.Neighbours.Count;
                    maxNode = n;
                }
            }
            return maxNode;
        }

        public void SingleRun()
        {
            InitializeSpringModel();
            i = 0;
            while (SingleStep()) ;
        }
        

        private Node getMaximumEnergyNode()
        {
            double maximumEnergy = 0;
            Node MaximumEnergyNode = null;
            for (int k = 0; k < NodesCount; k++)
            {
                getEnergy(Nodes[k]);
                if (Nodes[k].energy >= maximumEnergy)
                {
                    maximumEnergy = Nodes[k].energy;
                    MaximumEnergyNode = Nodes[k];
                }
            }
            return MaximumEnergyNode;
        }

        private double getSystemEnergy()
        {
            double E = 0;
            for (int i = 0; i < Nodes.Count - 1; i++)
            {
                for (int j = i + 1; j < Nodes.Count; j++)
                {
                    lij[i, j] = DesiredLength * FW.Dij[i, j];
                    kij[i, j] = 1 / (FW.Dij[i, j] * FW.Dij[i, j]);
                    double DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                    double DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                    E += 0.5 * kij[i, j] * (Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2) + Math.Pow(lij[i, j], 2) - (2 * lij[i, j] * Math.Sqrt(Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2))));
                }
            }
            return E;

        }
    
        private void GetDxDy(Node node,out double dx,out double dy)
        {
            double PartialY = GetPartialDiffY(node);
            
            double PartialY2 = get2PartialDiffY(node);
            double PartialXY = getPartialDiffXY(node);
            double PartialX = GetPartialDiffX(node);
            double PartialX2 = get2PartialDiffX(node);
            double deltaY = (-PartialY + (PartialX * PartialXY / PartialX2)) / (PartialY2 - ((PartialXY * PartialXY) / PartialX2));        

            
            double deltaX = (-PartialX - (PartialXY * deltaY)) / PartialX2;

            dx = deltaX;
            dy = deltaY;
        
        }

        private double getDeltaX(Node node)
        {
            
            double PartialX = GetPartialDiffX(node);
            
            double PartialX2 = get2PartialDiffX(node);
            double PartialXY = getPartialDiffXY(node);
            double deltaY = getDeltaY(node);

            double deltaX = (-PartialX - (PartialXY * deltaY)) / PartialX2;
            
            return deltaX;
        }

        private double getDeltaY(Node node)
        {
            
            double PartialY = GetPartialDiffY(node);
            double PartialY2 = get2PartialDiffY(node);
            double PartialXY = getPartialDiffXY(node);
            double PartialX = GetPartialDiffX(node);
            double PartialX2 = get2PartialDiffX(node);
            double deltaY = (-PartialY + (PartialX * PartialXY / PartialX2)) / (PartialY2 - ((PartialXY * PartialXY) / PartialX2));
            return deltaY;
        }

        private void updateFloydWarshall()
        {
            FW = new FloydWarshall(Nodes, Edges);
            double MaximumShortestLength = getMaximumShortestLength();
            DesiredLength = Lnode / MaximumShortestLength;
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = 0; j < Nodes.Count; j++)
                {
                    if(i!=j)
                    {
                        //double dij = System.Math.Max(1, FW.Dij[i, j]);
                        //FW.Dij[i, j]= System.Math.Max(1, FW.Dij[i, j]);
                        
                        double dij = FW.Dij[i, j];
                        if (FW.Wij[i, j] == double.MaxValue)
                        {
                            lij[i, j] = DesiredLength * dij;
                        }
                        else
                        {
                            lij[i, j] = _desiredEdgeLength;
                            if (_getOriginDistance != null)          //not sure about this...
                            {
                                lij[i, j] += _getOriginDistance(i, j);
                            }
                            
                        }
                        kij[i, j] = 1e6 /(dij * dij);
                        //if (kij[i, j] < 1)
                        //    kij[i, j] = 1;
                    }
                }
            }
            
        }

        public List<double> GetNodesEnergy()
        {
            List<double> nodesEnergy = new List<double>();
            foreach(Node n in Nodes)
            {
                getEnergy(n);
                nodesEnergy.Add(n.energy);
            }

            return nodesEnergy;
        }

        private void getEnergy(Node n)
        {
            //double MaximumShortestLength = getMaximumShortestLength();
            //DesiredLength = right / MaximumShortestLength;
            double PartialX = GetPartialDiffX(n);
            double PartialY = GetPartialDiffY(n);
            n.energy = Math.Sqrt(Math.Pow(PartialX, 2) + Math.Pow(PartialY, 2));
        }

        private double getMaximumShortestLength()
        {
            double Maximum = 0;
            for (int i = 0; i < NodesCount - 1; i++)
            {
                for (int j = i + 1; j < NodesCount; j++)
                {
                    if (FW.Dij[i, j] > Maximum)
                        Maximum = FW.Dij[i, j];
                }
            }
            return Maximum;
        }

        private void setRandomMode(int minX, int maxX, int minY, int maxY, int number_of_iterations)
        {
            InitialAllocation = Mode.RANDOM;
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
            this.number_of_random_iterations = number_of_iterations;
            random = new Random();
            MinimumEnergyNodes = new List<Node>();
        }

        private void setUserDefinedMode()
        {
            InitialAllocation = Mode.USERDEFINED;
        }

        private void randomizePositions()
        {
            lock (Nodes)
            {
                for (int i = 0; i < NodesCount; i++)
                {
                    Nodes[i].Xposition = random.Next(minX, maxX);
                    Nodes[i].Yposition = random.Next(minY, maxY);
                }
            }
        }

        private void AlignNodes()
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            foreach (Node n in Nodes)
            {
                if (n.Xposition < minX)
                    minX = n.Xposition;
                if (n.Yposition < minY)
                    minY = n.Yposition;
            }

            foreach (Node n in Nodes)
            {
                n.Xposition -= minX;
                n.Yposition -= minY;
            }
        }

        #region Equations
        private double GetPartialDiffX(Node m)
        {
            double DiffX = 0, DiffY = 0;
            int i = m.index, j = m.index + 1;
            double dummy = 0;



            while (j < Nodes.Count)
            {
                
                DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                dummy += 0.5 * kij[i, j] * (2 * Nodes[i].Xposition - 2 * Nodes[j].Xposition - (lij[i, j] * (2 * Nodes[i].Xposition - 2 * Nodes[j].Xposition) / Math.Sqrt(Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2))));
                j++;
            }

            j = m.index;
            i = 0;
            while (i < Nodes.Count - 1)
            {
                if (i != -1 && j > i)
                {
                    
                    DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                    DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                    double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                    dummy += 0.5 * kij[i, j] * (-2 * Nodes[i].Xposition + 2 * Nodes[j].Xposition - (lij[i, j] * (-2 * Nodes[i].Xposition + 2 * Nodes[j].Xposition) / Math.Sqrt(Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2))));

                }
                i++;


            }

            return dummy;

        }


        private double GetPartialDiffY(Node m)
        {
            double DiffX = 0, DiffY = 0;
            int i = m.index, j = m.index + 1;
            double dummy = 0;
            while (j < Nodes.Count)
            {
                
                DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                dummy += 0.5 * kij[i, j] * (2 * Nodes[i].Yposition - 2 * Nodes[j].Yposition - (lij[i, j] * (2 * Nodes[i].Yposition - 2 * Nodes[j].Yposition) / Math.Sqrt(Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2))));
                j++;
            }

            j = m.index;
            i = 0;
            while (i < Nodes.Count - 1)
            {
                if (i != -1 && j > i)
                {
                    
                    DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                    DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                    double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                    dummy += 0.5 * kij[i, j] * (-2 * Nodes[i].Yposition + 2 * Nodes[j].Yposition - (lij[i, j] * (-2 * Nodes[i].Yposition + 2 * Nodes[j].Yposition) / Math.Sqrt(Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2))));

                }
                i++;

            }

            return dummy;

        }
        private double get2PartialDiffX(Node m)
        {
            double DiffX = 0, DiffY = 0;
            int i = m.index, j = m.index + 1;
            double dummy = 0;
            while (j < Nodes.Count)
            {
                
                DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                double val=0.5 * kij[i, j] * 
                    (2 + 
                    (0.5 * lij[i, j] * Math.Pow(2 * Nodes[i].Xposition - 2 * Nodes[j].Xposition, 2) / Math.Pow(SumOfSquares, 1.5)) 
                    - (2 * lij[i, j] / Math.Sqrt(SumOfSquares)));
                dummy += val;
                j++;
            }

            j = m.index;
            i = 0;
            while (i < Nodes.Count - 1)
            {
                if (i != -1 && j > i)
                {
                    
                    DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                    DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                    double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                    double val = 0.5 * kij[i, j] * 
                        (2 
                        + (0.5 * lij[i, j] * Math.Pow(-2 * Nodes[i].Xposition + 2 * Nodes[j].Xposition, 2) / Math.Pow(SumOfSquares, 1.5)) 
                        - (2 * lij[i, j] / Math.Sqrt(SumOfSquares)));
                    dummy += val;
                }
                i++;
            }

            return dummy;
        }
        private double get2PartialDiffY(Node m)
        {
            double DiffX = 0, DiffY = 0;
            int i = m.index, j = m.index + 1;
            double dummy = 0;
            while (j < Nodes.Count)
            {
                
                DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                dummy += 0.5 * kij[i, j] * 
                    (2 
                    + (0.5 * lij[i, j] * Math.Pow(2 * Nodes[i].Yposition - 2 * Nodes[j].Yposition, 2) / Math.Pow(SumOfSquares, 1.5)) 
                    - (2 * lij[i, j] / Math.Sqrt(SumOfSquares))
                    );
                j++;
            }

            j = m.index;
            i = 0;
            while (i < Nodes.Count - 1)
            {
                if (i != -1 && j > i)
                {
                    
                    DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                    DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                    double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                    dummy += 0.5 * kij[i, j] * 
                        (2 
                        + (0.5 * lij[i, j] * Math.Pow(-2 * Nodes[i].Yposition + 2 * Nodes[j].Yposition, 2) / Math.Pow(SumOfSquares, 1.5)) 
                        - (2 * lij[i, j] / Math.Sqrt(SumOfSquares)));

                }
                i++;
            }




            return dummy;
        }
        private double getPartialDiffXY(Node m)
        {
            double DiffX = 0, DiffY = 0;
            int i = m.index, j = m.index + 1;
            double dummy = 0;
            while (j < Nodes.Count)
            {
                
                DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                double var = (0.25 * kij[i, j] * lij[i, j] * (2 * Nodes[i].Xposition - 2 * Nodes[j].Xposition) * (2 * Nodes[i].Yposition - 2 * Nodes[j].Yposition)) / 
                    Math.Pow(SumOfSquares, 1.5);
                dummy += var;
                j++;
            }

            j = m.index;
            i = 0;
            while (i < Nodes.Count - 1)
            {
                if (i != -1 && j > i)
                {
                    
                    DiffX = Nodes[i].Xposition - Nodes[j].Xposition;
                    DiffY = Nodes[i].Yposition - Nodes[j].Yposition;
                    double SumOfSquares = Math.Pow(DiffX, 2) + Math.Pow(DiffY, 2);
                    double var = (0.25 * kij[i, j] * lij[i, j] * 
                        (-2 * Nodes[i].Xposition + 2 * Nodes[j].Xposition) * 
                        (-2 * Nodes[i].Yposition + 2 * Nodes[j].Yposition)) 
                        / Math.Pow(SumOfSquares, 1.5);
                    dummy += var;
                }
                i++;
            }

            return dummy;
        }
        #endregion
    }
}
