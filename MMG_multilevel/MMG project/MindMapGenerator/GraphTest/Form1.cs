using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace GraphTest
{
    public partial class GraphForm : Form
    {
        List<Node> Nodes = new List<Node>();
        List<Edge> Edges = new List<Edge>();
        Mode AllocationMode;
        BindingSource bindingSource1 = new BindingSource();
        BindingSource bindingSource2 = new BindingSource();
        double Xposition, Yposition;
        int index=0;
        
        public GraphForm()
        {
            InitializeComponent();
        }

        private void GraphForm_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void button_InsertNode_Click(object sender, EventArgs e)
        {
             Xposition = int.Parse(this.textBox_X.Text);
             Yposition = int.Parse(this.textBox_Y.Text);
             Nodes.Add(new Node((double)Xposition, (double)Yposition, index));
             index++;
             bindingSource1.ResetBindings(true);
             bindingSource2.ResetBindings(true);
             panel2.Invalidate();
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

        private void button_InsertEdge_Click(object sender, EventArgs e)
        {
            Node Nodea = (Node)comboBox_Node1.SelectedItem;
            Node Nodeb = (Node)comboBox_Node2.SelectedItem;
            Edges.Add(new Edge((Node)comboBox_Node1.SelectedItem, (Node)comboBox_Node2.SelectedItem));
            Nodea.Neighbours.Add(Nodeb);
            Nodeb.Neighbours.Add(Nodea);

            panel2.Invalidate();
        }

        private void GraphForm_Load(object sender, EventArgs e)
        {
            bindingSource1.DataSource = this.Nodes;
            bindingSource2.DataSource = this.Nodes;
            this.comboBox_Node1.DataSource = this.bindingSource1;
            this.comboBox_Node1.DisplayMember = "Index";
            this.comboBox_Node2.DataSource = this.bindingSource2;
            this.comboBox_Node2.DisplayMember = "Index";
           
        }

        private void button_FloydW_Click(object sender, EventArgs e)
        {
            FloydWarshall FW = new FloydWarshall(Nodes, Edges);
            FWMatrix FWMatrixForm = new FWMatrix(FW.Dij, Nodes.Count);
            FWMatrixForm.ShowDialog();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            Pen P = new Pen(Color.Blue, 2);
            Pen P2 = new Pen(Color.Black, 2);
            lock (Nodes)
            {
                foreach (Node n in Nodes)
                {
                    G.DrawRectangle(P, (float)n.Xposition - 10, (float)n.Yposition - 10, 20, 20);
                    G.DrawString(n.index.ToString(), this.Font, new SolidBrush(Color.Blue), (float)n.Xposition - 5, (float)n.Yposition - 5);
                }
                foreach (Edge E in Edges)
                {
                    G.DrawLine(P2, (float)E.Node1.Xposition, (float)E.Node1.Yposition, (float)E.Node2.Xposition, (float)E.Node2.Yposition);
                    int distance = Convert.ToInt32(Math.Sqrt(Math.Pow(E.Node1.Xposition - E.Node2.Xposition, 2) + Math.Pow(E.Node1.Yposition - E.Node2.Yposition, 2)));
                    double xs = E.Node1.Xposition + E.Node2.Xposition;
                    double ys = E.Node1.Yposition + E.Node2.Yposition;

                    G.DrawString(distance.ToString(), this.Font, new SolidBrush(Color.Blue), (float)xs / 2, (float)ys / 2);
                }
            }
        }

        Node Node1;
        Node Node2;
        bool MouseDownFlag = false;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownFlag = true;
            if (radioButton_insertEdge.Checked == true)
            {
                Node1 = getNode((double)e.X, (double)e.Y);
            }
            else if (radioButton_Move.Checked == true)
            {
                Node1 = getNode((double)e.X, (double)e.Y);
            }
        }

        private Node getNode(double x, double y)
        {
            Node N = new Node(0,0,-1);
            foreach (Node n in Nodes)
            {
                if (x <= n.Xposition + 10 && x >= n.Xposition - 10 && y <= n.Yposition + 10 && y >= n.Yposition-10)
                {
                    N = n;
                    break;
                }
            }
            return N;
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            MouseDownFlag = false;
            if (radioButton_insertNode.Checked == true)
            {
                Nodes.Add(new Node((double)e.X, (double)e.Y, index));
                index++;
                bindingSource1.ResetBindings(true);
                bindingSource2.ResetBindings(true);
                panel2.Invalidate();
            }
            else if (radioButton_insertEdge.Checked == true)
            {
                Node2 = getNode((double)e.X, (double)e.Y);
                if (Node1.index != -1 && Node2.index != -1)
                {
                    Edges.Add(new Edge(Node1, Node2));
                    Node1.Neighbours.Add(Node2);
                    Node2.Neighbours.Add(Node1);
                }
                panel2.Invalidate();
            }
            else
            {
                Node1.Xposition = (double)e.X;
                Node1.Yposition = (double)e.Y;
                bindingSource1.ResetBindings(true);
                bindingSource2.ResetBindings(true);
                panel2.Invalidate();
            }
        }
        private void SpringRun()
        {
            SpringModel SM = new SpringModel(Nodes, Edges, double.Parse(txt_desired.Text), double.Parse(txt_desiredEdgeLength.Text), AllocationMode);

            if (singleStepModel != null)
            {
                singleStepModel.NodePositionChanged -= new NodePositionChangedEventHandler(singleStepModel_NodePositionChanged);
                singleStepModel = null;
            }

            SM.NodePositionChanged += new NodePositionChangedEventHandler(singleStepModel_NodePositionChanged);
            SM.Run();
            SM.NodePositionChanged -= new NodePositionChangedEventHandler(singleStepModel_NodePositionChanged);

            
            //List<double> nodesEnergy = SM.GetNodesEnergy();

            //string s = "";
            //int i = 0;
            //foreach (double en in nodesEnergy)
            //{
            //    s += "Node " + i + " : " + en + "\n";
            //    i++;
            //}
            //s += "i = " + SM.i.ToString();
            //MessageBox.Show(s);
            //panel2.Invalidate();
        }
        private void button_spring_Click(object sender, EventArgs e)
        {
            if (radioButton_Random.Checked == true)
                AllocationMode = Mode.RANDOM;
            else if (radioButton_UserDefined.Checked == true)
                AllocationMode = Mode.USERDEFINED;
            else
                AllocationMode = Mode.CIRCULAR;

            Thread th = new Thread(new ThreadStart(SpringRun));
            th.Start();
           
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseDownFlag == true)
            {
                if (radioButton_Move.Checked == true)
                {
                    Node1.Xposition = (double)e.X;
                    Node1.Yposition = (double)e.Y;
                    if (singleStepModel != null)
                    {
                        singleStepModel.NodePositionChanged -= new NodePositionChangedEventHandler(singleStepModel_NodePositionChanged);

                        singleStepModel = null;
                    }
                    panel2.Invalidate();
                }
            }
        }

        private void button_annrealing_Click(object sender, EventArgs e)
        {
            SimulatedAnnealing S = new SimulatedAnnealing(this.Nodes, this.Edges);
            Nodes = S.MinimumNodes;
            Edges = S.MinimumEdges;
            panel2.Invalidate();
        }
        SpringModel singleStepModel = null;
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton_Random.Checked == true)
                AllocationMode = Mode.RANDOM;

            else if (radioButton_UserDefined.Checked == true)
                AllocationMode = Mode.USERDEFINED;
            else
                AllocationMode = Mode.CIRCULAR;

            try
            {
                if (singleStepModel == null)
                {
                    singleStepModel = new SpringModel(Nodes, Edges, double.Parse(txt_desired.Text), double.Parse(txt_desiredEdgeLength.Text),AllocationMode); 
                    singleStepModel.InitializeSpringModel();
                    singleStepModel.NodePositionChanged += new NodePositionChangedEventHandler(singleStepModel_NodePositionChanged);
                }
                singleStepModel.ChildSingleStep();
            }
            catch (Exception)
            {
                MessageBox.Show("Input Error");
                singleStepModel = null;
                return;
            }
            lock (Nodes)
            {
                this.Nodes = singleStepModel.Nodes;
                this.Edges = singleStepModel.Edges;
            }
            panel2.Invalidate();
            MessageBox.Show(singleStepModel.MaximumEnergyNode.index + " : " + singleStepModel.MaximumEnergyNode.energy);
            
        }

        void singleStepModel_NodePositionChanged(int nodeIndex, double dx, double dy)
        {
            panel2.Invalidate();
        }

        private void txt_desired_TextChanged(object sender, EventArgs e)
        {
            singleStepModel = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AlignNodes();
            panel2.Invalidate();

        }

        private void SpringNStep()
        {
            try
            {
                if (singleStepModel == null)
                {
                    singleStepModel = new SpringModel(Nodes, Edges, double.Parse(txt_desired.Text), double.Parse(txt_desiredEdgeLength.Text),AllocationMode); 
                    singleStepModel.InitializeSpringModel();
                    singleStepModel.NodePositionChanged += new NodePositionChangedEventHandler(singleStepModel_NodePositionChanged);
                }
                for (int i = 0; i < int.Parse(txt_N.Text); i++)
                    singleStepModel.ChildSingleStep();
                SpringModelReportStatus smrs = new SpringModelReportStatus(singleStepModel);
                smrs.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Input Error");
                singleStepModel = null;
                return;
            }
            this.Nodes = singleStepModel.Nodes;
            this.Edges = singleStepModel.Edges;
            panel2.Invalidate();
 
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (radioButton_Random.Checked == true)
                AllocationMode = Mode.RANDOM;
            else if (radioButton_UserDefined.Checked == true)
                AllocationMode = Mode.USERDEFINED;
            else
                AllocationMode = Mode.CIRCULAR;

            Thread th =  new Thread(new ThreadStart(SpringNStep));
            th.Start();
            //while (th.ThreadState == ThreadState.Running) ;
            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        

        
    }
}