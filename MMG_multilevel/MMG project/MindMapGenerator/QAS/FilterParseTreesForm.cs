using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SyntacticAnalyzer;

namespace MMG
{
    public partial class FilterParseTreesForm : Form
    {
        public List<List<int>> ParseTresIndices = new List<List<int>>();
        public List<int> SelectedParseTrees = new List<int>();


        void FindParseTree(int index, out int ParseTreeInd, out int subIndex)
        {
            for (int i = 0; i < ParseTresIndices.Count; i++)
            {
                for (int j = 0; j < ParseTresIndices[i].Count; j++)
                {
                    if (ParseTresIndices[i][j] == index)
                    {
                        ParseTreeInd = i;
                        subIndex = j;
                        return;
                    }
                }
            }
            ParseTreeInd = -1;
            subIndex = -1;
            return;
        }
        ArrayList _parseTrees;
        public FilterParseTreesForm(ArrayList SParseTrees,List<int> selcetPTrees)
        {
            InitializeComponent();
            _parseTrees = SParseTrees;
            SelectedParseTrees = selcetPTrees;
            int k = 0;
            
            List<int> cTreeIndices = new List<int>();
            
            for (int i = 0; i < SParseTrees.Count; i++)
            {
                ParseTree cPtree = (ParseTree)SParseTrees[i];
                int rowId = dataGridView1.Rows.Add();
                dataGridView1.Rows[rowId].Cells[0].Value = AppendStrings(cPtree.Words);
                dataGridView1.Rows[rowId].Cells[1].Value = AppendStrings(SentenceParser.GetPOSString(cPtree));
                dataGridView1.Rows[rowId].Cells[2].Value = k.ToString();
                if (i + 1 == SParseTrees.Count)
                {
                    cTreeIndices.Add(i);
                    ParseTresIndices.Add(cTreeIndices);
                    int rowId2 = dataGridView2.Rows.Add();
                    dataGridView2.Rows[rowId2].Cells[0].Value = dataGridView1.Rows[rowId].Cells[0].Value;
                    dataGridView2.Rows[rowId2].Cells[1].Value = k.ToString();
                    if (SelectedParseTrees != null && rowId2<= SelectedParseTrees.Count - 1)
                    {
                        dataGridView2.Rows[rowId2].Cells[1].Value = SelectedParseTrees[rowId2].ToString();

                    }
                }
                else
                {

                    ParseTree nPtree = (ParseTree)SParseTrees[i + 1];
                    if (cPtree.Words.Count != nPtree.Words.Count)
                    {

                      
                        cTreeIndices.Add(i);
                        ParseTresIndices.Add(cTreeIndices);
                        cTreeIndices = new List<int>();
                        int rowId2 = dataGridView2.Rows.Add();
                        dataGridView2.Rows[rowId2].Cells[0].Value = dataGridView1.Rows[rowId].Cells[0].Value;
                        dataGridView2.Rows[rowId2].Cells[1].Value = k.ToString();
                        if (SelectedParseTrees != null && rowId2 <= SelectedParseTrees.Count - 1)
                        {
                            dataGridView2.Rows[rowId2].Cells[1].Value = SelectedParseTrees[rowId2].ToString();

                        }
                        k = -1;
                    }
                    else
                    {
                        bool match = true;
                        for (int j = 0; j < cPtree.Words.Count; j++)
                        {
                            if (cPtree.Words[j] != nPtree.Words[j])
                            {
                               
                                cTreeIndices.Add(i);
                                ParseTresIndices.Add(cTreeIndices);
                                int rowId2 = dataGridView2.Rows.Add();
                                dataGridView2.Rows[rowId2].Cells[0].Value = dataGridView1.Rows[rowId].Cells[0].Value;
                                
                                dataGridView2.Rows[rowId2].Cells[1].Value = k.ToString();
                                if (SelectedParseTrees != null && rowId2 <= SelectedParseTrees.Count - 1)
                                {
                                    dataGridView2.Rows[rowId2].Cells[1].Value = SelectedParseTrees[rowId2].ToString();
                                    
                                }
                                cTreeIndices = new List<int>();
                                match = false;
                                k = -1;
                                break;
                            }
                        }
                        if (match)
                        {
                            cTreeIndices.Add(i);
                        }

                    }
                }
                k++;
                

            }
        }
    
        private static string AppendStrings(ArrayList strsArr)
        {
            string str ="";
            foreach(string strI in  strsArr)
            {
                str += strI + " ";

            }
            return str;
        }

        private static string AppendStrings(List<string> strsArr)
        {
            string str = "";
            foreach (string strI in strsArr)
            {
                str += strI+" ";

            }
            return str;
        }

        public ArrayList NewParseTrees
        {
            get;
            private set;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewParseTrees = new ArrayList();
            SelectedParseTrees = new List<int>();
            for(int i = 0;i< dataGridView2.Rows.Count;i++)
            {
                try
                {
                    int k = int.Parse((string)dataGridView2.Rows[i].Cells[1].Value);
                    NewParseTrees.Add(_parseTrees[ParseTresIndices[i][k]]);
                    SelectedParseTrees.Add(k);
                }
                catch (Exception) { }
            }
            this.Close();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                int ptreeIndex, subindex;
                FindParseTree(index, out ptreeIndex, out subindex);
                if (ptreeIndex != -1)
                {
                    int rowId2 = ptreeIndex;
                    dataGridView2.Rows[rowId2].Cells[1].Value = subindex.ToString();
                  
                }
            }
        }
        }
}
   

       



