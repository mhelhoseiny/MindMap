using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OntologyLibrary;
using OntologyLibrary.OntoSem;
using System.IO;
using System.Security.Policy;
using System.Collections;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OntologyConceptsEditor
{
    public partial class frmOntologyEditor : Form
    {
        public frmOntologyEditor()
        {
            InitializeComponent();
        }
        Dictionary<string, Vertex> Graph;
        void FillGraph()
        {
            //Graph = new Dictionary<string, Vertex>();//
            //onto = new Ontology(OntologyConceptsEditor.projectData.path);
            //StreamReader sr = new StreamReader(OntologyConceptsEditor.projectData.path + @"\Get.txt");
            //string str = null;
            //int x = 0;
            //while ((str = sr.ReadLine()) != null)
            //{
            //    //if (++x < 1000)
            //    {
            //        if (str == "")
            //            continue;
            //        onto.LoadConcept(str);
            //        this.Graph.Add(str, new Vertex(str));
            //        //cmbConceptNames.Items.Add(str);
            //        //cmbFirstConcept.Items.Add(str);
            //        //cmbSecondConcept.Items.Add(str);
            //    }
            //}
            //sr.Close();
            foreach (Concept c in onto)
            {
                PropertiesDictionary pd = c.FullProperties;
                int n = 0;
                foreach (Property de in pd.Properties)
                {

                    OntologyLibrary.OntoSem.FillerList OF = de.Fillers;
                    List<TreeNode> ch = new List<TreeNode>();
                    foreach (Filler Ofiller in OF)
                    {
                        if (Ofiller.ConceptFiller != null)
                        {
                            if (de.Name == "SUBCLASSES")
                            {
                                //Graph[c.Name].Edges = new List<Relation>();
                                //Graph[Ofiller.ConceptFiller.Name].Edges = new List<Relation>();
                                Graph[c.Name].Edges.Add(new Relation(Graph[c.Name], Graph[Ofiller.ConceptFiller.Name], RelationType.SubClass));
                                Graph[Ofiller.ConceptFiller.Name].Edges.Add(new Relation(Graph[Ofiller.ConceptFiller.Name], Graph[c.Name], RelationType.InheritedFrom));
                            }
                        }
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            onto = new Ontology(OntologyConceptsEditor.projectData.path);
        }
        Ontology onto;
        void  Breadth(Vertex start, Vertex end)
        {
            if (start==end)
            {
                return;
            }
            foreach (KeyValuePair<string,Vertex> pair in Graph)
            {
                pair.Value.Previous = null;
                pair.Value.isVisited = false;
            }
            Queue<Vertex> current = new Queue<Vertex>();
            current.Enqueue(start);
            start.isVisited = true;
            while (current.Count!=0)
            {
                Vertex v = current.Dequeue();
                foreach (Relation r in v.Edges)
                {
                    if (v!=start&&v.Previous.type == RelationType.SubClass && r.type == RelationType.InheritedFrom)
                    {
                        continue;
                    }
                    if (r.to.isVisited)
                    {
                        continue;
                    }
                    r.to.isVisited = true;
                    r.to.Previous = r;
                    current.Enqueue(r.to);
                    if (r.to == end)
                    {
                        return;
                    }
                }
            }
            if (end!=null)
            {
                MessageBox.Show("No path");
            }
            else
            {
                this.treeView2.Nodes.Clear();
                foreach (KeyValuePair<string,Vertex> pair in Graph)
                {
                    if (pair.Value.isVisited==false)
                    {
                        treeView2.Nodes.Add(pair.Value.Name);
                    }
                }
            }
        }
        private void FillCombo()
        {
            Graph = new Dictionary<string, Vertex>();
            //onto = new Ontology(OntologyConceptsEditor.projectData.path);
            StreamReader sr = new StreamReader(OntologyConceptsEditor.projectData.path + @"\Get.txt");
            string str = null;
            int x = 0;
            string[] items = new string[8715];
            while ((str = sr.ReadLine()) != null)
            {
                //if (++x < 200)
                {
                    if (str == "")
                        continue;
                    onto.LoadConcept(str);
                    items[x++] = str;
                    this.Graph.Add(str, new Vertex(str));
                }
            }
            cmbConceptNames.Items.AddRange(items);
            cmbFirstConcept.Items.AddRange(items);
            cmbSecondConcept.Items.AddRange(items);
            sr.Close();
        }

        private void btnLoadCombo_Click(object sender, EventArgs e)
        {
            FillCombo();
            FillGraph();
            btnFillTreeView.Enabled = true;
            //MessageBox.Show(GC.GetTotalMemory(true).ToString());
        }

        private void cmbConceptNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            string zzz = cmbConceptNames.Text;
            GetData(zzz);
        }

        private void GetData(string zzz)
        {
            richTextBox1.Text = "";
            onto.LoadConcept(zzz);
            Concept c = onto[zzz];
            PropertiesDictionary pd = c.FullProperties;

            foreach (Property de in pd.Properties)
            {
                OntologyLibrary.OntoSem.FillerList OF = de.Fillers;
                foreach (Filler Ofiller in OF)
                {
                    if (Ofiller.ConceptFiller != null)
                    {
                        richTextBox1.Text += de.Name + "   =   " + Ofiller.ConceptFiller.Name + "\n";

                    }
                }

            }
        }

        List<TreeNode> nodes = new List<TreeNode>();
        //List<string> lst = new List<string>();
        Queue<string> lst = new Queue<string>();
        private void btnFillTree_Click(object sender, EventArgs e)
        {

            nodes.Add(new TreeNode("ALL"));
            lst.Enqueue("ALL");
            int n = lst.Count;
            int index = 0;
            for (int i = 0; i < n; i++)
            {
                string str = lst.Dequeue();
                n--;
                Concept c = onto[str];

                PropertiesDictionary pd = c.FullProperties;

                foreach (Property de in pd.Properties)
                {
                    OntologyLibrary.OntoSem.FillerList OF = de.Fillers;

                    foreach (Filler Ofiller in OF)
                    {

                        if (de.Name == "IS-A" && Ofiller.ConceptFiller != null)
                        {
                            TreeNode tn = new TreeNode(Ofiller.ConceptFiller.Name);
                            int k = 0;
                            foreach (TreeNode _n in nodes)
                            {
                                if (_n.Text == tn.Text)
                                {
                                    index = k;//nodes.IndexOf(tn);
                                    goto l;
                                }
                                k++;
                            }
                            //if (nodes.Contains(tn.Text))
                            //{

                            //}
                        }
                    l:
                        if (de.Name == "SUBCLASSES" && Ofiller.ConceptFiller != null)
                        {

                            //all.Nodes.Add(Ofiller.ConceptFiller.Name);
                            nodes.Add(new TreeNode(Ofiller.ConceptFiller.Name));
                            nodes[index].Nodes.Add(new TreeNode(Ofiller.ConceptFiller.Name));
                            lst.Enqueue(Ofiller.ConceptFiller.Name);
                            n++;
                            //treeView1.Nodes.Add(nodes[nodes.Count - 1]);
                            //richTextBox1.Text += de.Name + "   =   " + Ofiller.ConceptFiller.Name + "\n";
                            //treeView1.Nodes.Add(new TreeNode(

                        }
                    }

                }
            }
            //lb1:
            treeView1.Nodes.Add(nodes[0]);
            #region
            //foreach (Concept c in onto)
            //{
            //    PropertiesDictionary pd=c.FullProperties;

            //foreach (Property de in pd.Properties)
            //{
            //    OntologyLibrary.OntoSem.FillerList OF = de.Fillers;
            //    foreach (Filler Ofiller in OF)
            //    {
            //        if (de.Name == "SUBCLASSES" && Ofiller.ConceptFiller!=null)
            //        {
            //            //all.Nodes.Add(Ofiller.ConceptFiller.Name);
            //            nodes.Add(new TreeNode(Ofiller.ConceptFiller.Name));
            //            //richTextBox1.Text += de.Name + "   =   " + Ofiller.ConceptFiller.Name + "\n";
            //            //treeView1.Nodes.Add(new TreeNode(
            //        }
            //    }

            //}
            //foreach (Property de in pd.Properties)
            //{
            //    OntologyLibrary.OntoSem.FillerList OF = de.Fillers;
            //    foreach (Filler Ofiller in OF)
            //    {
            //        if (de.Name == "IS-A" && Ofiller.ConceptFiller!=null)
            //        {
            //            TreeNode tn=new TreeNode(Ofiller.ConceptFiller.Name);
            //            if(nodes.Contains(tn))
            //            {
            //            int index = nodes.IndexOf(tn);
            //            nodes[index].Nodes.Add(c.Name);
            //            }
            //        }
            //    }

            //}
            //foreach (TreeNode  node in nodes)
            //{
            //    if(!treeView1.Nodes.Contains(node))
            //    treeView1.Nodes.Add(node);
            //}
            //}
            #endregion
        }

        //Dictionary<TreeNode, List<TreeNode>> all = new Dictionary<TreeNode, List<TreeNode>>();
        Dictionary<string, TreeNode> all = new Dictionary<string, TreeNode>();
        Dictionary<string, TreeNode> distance = new Dictionary<string, TreeNode>();
        List<TreeNode> childs = new List<TreeNode>();

        public void Fill(TreeNode t)
        {
            Concept c = onto[t.Text];
            PropertiesDictionary pd = c.FullProperties;
            foreach (Property de in pd.Properties)
            {

                OntologyLibrary.OntoSem.FillerList OF = de.Fillers;
                List<TreeNode> ch = new List<TreeNode>();
                foreach (Filler Ofiller in OF)
                {
                    if (de.Name == "SUBCLASSES" && Ofiller.ConceptFiller != null)
                    {
                        t.Nodes.Add(new TreeNode(Ofiller.ConceptFiller.Name));
                    }
                }
            }
            for (int i = 0; i < t.Nodes.Count; i++)
            {
                Fill(t.Nodes[i]);
            }
        }
        TreeNode parent_node;
        private void btnFillTreeView_Click(object sender, EventArgs e)
        {
            parent_node = new TreeNode("ALL");
            //all.Add["ALL"];
            Fill(parent_node);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(parent_node);
            btnGetPath.Enabled = true;
            /////////////////////////////////////////////////////////////////
            #region
            //int x = 0;

            //List<string> list = new List<string>();
            //list.Add("ALL");
            ////int IsA = 2;
            //for(int i=0;i<list.Count;i++)
            //{

            //    Concept c = onto[list[i]];
            //    if (c == null)
            //    {
            //        string _s = list[i];
            //        _s = _s.Substring(0, _s.Length - 1);
            //        c = onto[_s];
            //    }

            //    //if (++x < 5000)
            //    {
            //        PropertiesDictionary pd = c.FullProperties;
            //        if (!all.ContainsKey(c.Name))
            //        {
            //            all.Add(c.Name, new TreeNode(c.Name));
            //            all[c.Name].Text = c.Name;//
            //        }
            //        foreach (Property de in pd.Properties)
            //        {

            //            OntologyLibrary.OntoSem.FillerList OF = de.Fillers;
            //            List<TreeNode> ch = new List<TreeNode>();
            //            foreach (Filler Ofiller in OF)
            //            {

            //                #region
            //                //if (de.Name == "IS-A" && Ofiller.ConceptFiller != null)
            //                //{
            //                //    try
            //                //    {
            //                //        List<TreeNode> ln = new List<TreeNode>();
            //                //        ln.Add(new TreeNode(c.Name));
            //                //        all.Add(new TreeNode(Ofiller.ConceptFiller.Name), ln);

            //                //    }
            //                //    catch (Exception) { }
            //                //    //if (!all.ContainsKey(new TreeNode(Ofiller.ConceptFiller.Name)))
            //                //    //{
            //                //    //    //ch.Add(new TreeNode(Ofiller.ConceptFiller.Name));
            //                //    //    List<TreeNode> ln = new List<TreeNode>();
            //                //    //    ln.Add(new TreeNode(c.Name));
            //                //    //    all.Add(new TreeNode(Ofiller.ConceptFiller.Name), ln);
            //                //    //    //childs.Add(new TreeNode(Ofiller.ConceptFiller.Name));
            //                //    //}
            //                //    //else
            //                //    //{
            //                //    //    all[new TreeNode(c.Name)].Add(new TreeNode(Ofiller.ConceptFiller.Name));
            //                //    //}
            //                //}

            //                //if (de.Name == "IS-A" && Ofiller.ConceptFiller != null)
            //                //{
            //                //    try
            //                //    {
            //                //        all.Add(Ofiller.ConceptFiller.Name, new TreeNode(Ofiller.ConceptFiller.Name));
            //                //        all.Add(c.Name, new TreeNode(c.Name));
            //                //        all[Ofiller.ConceptFiller.Name].Nodes.Add(all[c.Name]);                   
            //                //    }
            //                //    catch (Exception)
            //                //    {
            //                //        try
            //                //        {
            //                //            all.Add(c.Name, new TreeNode(c.Name));
            //                //            all[Ofiller.ConceptFiller.Name].Nodes.Add(all[c.Name]);                                        
            //                //        }
            //                //        catch (Exception) { }
            //                //    }
            //                //}
            //                //else
            //                #endregion
            //                //if (de.Name == "IS-A" && Ofiller.ConceptFiller != null)
            //                //{
            //                //    string _parent = all[c.Name].Previous.Name;
            //                //    if (_parent != null && _parent != "")
            //                //    {
            //                //        all[_parent].Nodes.Add(all[c.Name]);
            //                //    }
            //                //}
            //                if (de.Name == "SUBCLASSES" && Ofiller.ConceptFiller != null)
            //                {
            //                    if (!list.Contains(Ofiller.ConceptFiller.Name) && !all.ContainsKey(Ofiller.ConceptFiller.Name))
            //                    {
            //                        list.Add(Ofiller.ConceptFiller.Name);
            //                    }
            //                        /////////
            //                    else
            //                    {
            //                        for (int counter = 0; counter < 10; counter++)
            //                        {
            //                            if (!list.Contains(Ofiller.ConceptFiller.Name + counter) && !all.ContainsKey(Ofiller.ConceptFiller.Name + counter))
            //                            {
            //                                list.Add(Ofiller.ConceptFiller.Name + counter);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    /////////////
            //                    if(!all.ContainsKey(c.Name))
            //                    {
            //                        all.Add(c.Name, new TreeNode(c.Name));
            //                        all[c.Name].Text = c.Name;//
            //                    }
            //                    if (!all.ContainsKey(Ofiller.ConceptFiller.Name))
            //                    {
            //                        all.Add(Ofiller.ConceptFiller.Name, new TreeNode(Ofiller.ConceptFiller.Name));
            //                        all[Ofiller.ConceptFiller.Name].Text = Ofiller.ConceptFiller.Name;
            //                    }
            //                        //
            //                    else
            //                    {
            //                        for (int counter = 0; counter < 10; counter++)
            //                        {
            //                            if (!all.ContainsKey(Ofiller.ConceptFiller.Name))
            //                            {
            //                                all.Add(Ofiller.ConceptFiller.Name + counter, new TreeNode(Ofiller.ConceptFiller.Name));
            //                                all[Ofiller.ConceptFiller.Name + counter].Text = Ofiller.ConceptFiller.Name;
            //                            }
            //                        }
            //                    }
            //                    //
            //                    if (!all[c.Name].Nodes.Contains(all[Ofiller.ConceptFiller.Name]))
            //                    {
            //                        all[Ofiller.ConceptFiller.Name].Text = Ofiller.ConceptFiller.Name;
            //                        all[c.Name].Nodes.Add(all[Ofiller.ConceptFiller.Name]);
            //                    }

            //                }
            //            }
            //        }
            //        list.RemoveAt(i);
            //        i--;
            //}
            //}

            //treeView1.Nodes.Clear();
            //treeView1.Nodes.Add(all["ALL"]);

            //btnGetPath.Enabled = true;
            #endregion
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            txtFullPath.Text = treeView1.SelectedNode.FullPath;
            GetData(treeView1.SelectedNode.Text);
        }
        private void btnGetPath_Click(object sender, EventArgs e)
        {
            if (this.cmbSecondConcept.Text==this.cmbFirstConcept.Text)
            {
                MessageBox.Show("Same Node");
            }
            Breadth(Graph[cmbFirstConcept.Text], Graph[cmbSecondConcept.Text]);
            Vertex v = Graph[cmbSecondConcept.Text];
            string path = "";
//            List<string> leftPath, RightPath;
            Stack<string> leftPath=new Stack<string>();
            Queue<string> RightPath = new Queue<string>();
            char _path = 'L';
            string parentNode = "";
            while (v!=null&& v.Previous!=null)
            {
                if (_path=='L')
                {
                    if (v.Previous.type==RelationType.SubClass)
                    {
                        leftPath.Push(v.Name);
                    }
                    else
                    {
                        _path = 'R';
                        parentNode = v.Name;
                    }
                }
                else
                {
                    RightPath.Enqueue(v.Name);
                }
                path = "/" + v.Name + path;
                v = v.Previous.from;
            }
            path = this.cmbFirstConcept.Text + path;
            RightPath.Enqueue(this.cmbFirstConcept.Text);
            MessageBox.Show(path);
            TreeNode tree = new TreeNode(parentNode);
            TreeNode current = tree;
            while (leftPath.Count!=0)
            {
                string nextNode = leftPath.Pop();
                current.Nodes.Add(nextNode);
                current = current.Nodes[0];
            }
            current = tree;
            while (RightPath.Count!=0)
            {
                TreeNode nextNode = new TreeNode(RightPath.Dequeue());
                current.Nodes.Add(nextNode);
                current = nextNode;
            }
            treeView2.Nodes.Clear();
            treeView2.Nodes.Add(tree);
            /////////////////////////////////////////////////////////////////
            //distance.Clear();
            //string FirstPath = all[cmbFirstConcept.Text].FullPath;
            //string SecondPath = all[cmbSecondConcept.Text].FullPath;

            //string[] firstparts = FirstPath.Split('/');
            //string[] secondparts = SecondPath.Split('/');

            //int k = firstparts.Length;
            //if (secondparts.Length < k)
            //    k = secondparts.Length;
            //int i = 0;
            //for ( i = 0; i < k; i++)
            //{
            //    if (firstparts[i] != secondparts[i])
            //        break;
            //}
            //int path = firstparts.Length - i + secondparts.Length - i;
            //string str = "";
            //for (int j = firstparts.Length-1; j >= firstparts.Length - i; j--)
            //{
            //    str += firstparts[j] + "-->";
            //}
            //for (int j = secondparts.Length-i; j < secondparts.Length; j++)
            //{
            //    str += secondparts[j] + "-->";
            //}
            //MessageBox.Show(path.ToString());
            //MessageBox.Show(str);
            ////////
            //string n = "";
            //for (i = 0; i < k; i++)
            //{
            //    if (firstparts[i] != secondparts[i])
            //        break;
            //    else
            //        n = firstparts[i];
            //}
            //distance.Add(n, new TreeNode(n));
            //string IsA = n;
            //for (int j =  i; j < firstparts.Length; j++)
            //{
            //    distance.Add(firstparts[j], new TreeNode(firstparts[j]));
            //    distance[IsA].Nodes.Add(distance[firstparts[j]]);
            //    IsA = firstparts[j];
            //}
            //IsA = n;
            //for (int j =  i; j < secondparts.Length; j++)
            //{
            //    distance.Add(secondparts[j], new TreeNode(secondparts[j]));
            //    distance[IsA].Nodes.Add(distance[secondparts[j]]);
            //    IsA = secondparts[j];
            //}
            //treeView2.Nodes.Clear();
            //treeView2.Nodes.Add(distance[n]);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            IFormatter formatter=new BinaryFormatter();
            FileStream fs = new FileStream(OntologyConceptsEditor.projectData.path+"\\Graph.bin",FileMode.Create,FileAccess.Write);
            formatter.Serialize(fs, Graph);
            fs.Close();
            ///////////////////////////
            fs = new FileStream(OntologyConceptsEditor.projectData.path + "\\Tree.bin", FileMode.Create, FileAccess.Write);
            formatter.Serialize(fs, parent_node);
            fs.Close();

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(OntologyConceptsEditor.projectData.path + @"\Get.txt");
            string str = null;
            int x = 0;
            List<string> items = new List<string>();
            while ((str = sr.ReadLine()) != null)
            {
                //if (++x < 2000)
                {
                    if (str == "")
                        continue;
                    items.Add( str);
                    //cmbConceptNames.Items.Add(str);
                    //cmbFirstConcept.Items.Add(str);
                    //cmbSecondConcept.Items.Add(str);
                }
            }

            string[] itemsArr = items.ToArray();
            cmbConceptNames.Items.AddRange(itemsArr);
            cmbFirstConcept.Items.AddRange(itemsArr);
            cmbSecondConcept.Items.AddRange(itemsArr);
            sr.Close();
            //MessageBox.Show("fill combo completed");
            /////////////////////////////////////////////////////*/
            IFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(OntologyConceptsEditor.projectData.path + "\\Graph.bin", FileMode.Open, FileAccess.Read);
            Graph = (Dictionary<string,Vertex>)formatter.Deserialize(fs);
            fs.Close();
            //MessageBox.Show("Graph loaded");
            ///////////////////////////
            fs = new FileStream(OntologyConceptsEditor.projectData.path + "\\Tree.bin", FileMode.Open, FileAccess.Read);
            parent_node=(TreeNode)formatter.Deserialize(fs);
            fs.Close();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(parent_node);
            btnGetPath.Enabled = true;
            //MessageBox.Show("Tree loaded");
            //MessageBox.Show(GC.GetTotalMemory(true).ToString());
        }

        private void btnErrors_Click(object sender, EventArgs e)
        {
            this.Breadth(Graph["ALL"], null);
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            txtFullPath.Text = treeView2.SelectedNode.FullPath;
            GetData(treeView2.SelectedNode.Text);
        }
    }
}
