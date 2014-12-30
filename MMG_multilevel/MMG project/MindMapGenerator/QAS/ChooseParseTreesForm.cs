using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MMG;
using MindMapGenerator.Drawing_Management;
using DiscourseAnalysis;
using SyntacticAnalyzer;
using mmTMR;
using System.Collections;
using WnLexicon;


namespace MMG
{
    public partial class ChooseParseTreesForm : Form
    {
        ArrayList Indices;
        ArrayList GivenParseTrees;
        public ChooseParseTreesForm(ArrayList AllParseTrees, ArrayList IndexesList)
        {
            InitializeComponent();
            GivenParseTrees = new ArrayList(AllParseTrees);
            Indices = new ArrayList(IndexesList);
            //GivenParseTrees = AllParseTrees;
            //Indices = IndexesList;
            
        }

        public ArrayList ChosenParseTrees = new ArrayList();
        public List<int> NoOfSentences = new List<int>();
        List<int> NewIndices;// = new List<int>();
        private void ChooseParseTreesForm_Load(object sender, EventArgs e)
        {
            //datagrid[col,row]
            DataGridViewRow row = new DataGridViewRow();
            //DataGridCell cell = new DataGridCell();
            //display.....
            for (int i = 0; i < Indices.Count; i++)
            {
                if (!NoOfSentences.Contains((int)Indices[i]))
                {
                    NoOfSentences.Add((int)Indices[i]);

                }

            }
            int No = 0;
            NewIndices = new List<int>(ChosenParseTrees.Count);

           // for(int j=0;j<SentenceID.coun
            //int index = 0;
            int x = (int)Indices[0];
            NewIndices.Add(No);
            for (int i = 1; i < Indices.Count; i++)
            {
                if ((int)Indices[i] == x)
                {
                    NewIndices.Add(No);
                }
                else
                {
                    x = (int)Indices[i];
                    No++;
                    NewIndices.Add(No);
                }
                //if (!NewIndices.Contains((int)Indices[i]))
                //{
                //    No++;
                //    NewIndices.Add(No);
                //}
                //else
                //{
                //    NewIndices.Add(No);
                //}
                //int S1 = (int)Indices[i];
                //int S2 = (int)Indices[i + 1];

                //if (S1 == S2)
                //{
                //    //No++;
                //    NewIndices.Add(No);
                //    NewIndices.Add(No);

                //}

                //else
                //{
                //    //i--;
                //    No++;
                //    //NewIndices.Add(No);
                //}
                ////

                
            }
            //int num = NoOfSentences.Count;
            //for (int y = NoOfSentences.Count; y <= num; y++)
            //{
            //    NoOfSentences.Add(Convert.ToInt32('f'));
            //}
            //for (int x = 0; x < NoOfSentences.Count; x++)
            //{
            //    if (NoOfSentences[x] != x)
            //    {
            //        NoOfSentences[x] = x;
                    
            //    }
            //}
            //for (int y = 0; y < Indices.Count; y++)
            //{
            //    if(Indices.IndexOf(y)!=y)

            //}

           // ArrayList temp = new ArrayList(NoOfSentences[NoOfSentences.Count-2]);
            //DataGridViewRow row = new DataGridViewRow();
            //fill the datagrid 
            for (int i = 0; i < NoOfSentences.Count; i++)
            {
            //int b=0;
            //while(NoOfSentences[b]!=null)
            //{  
               // DataGridViewRow row = new DataGridViewRow();
                //if (NoOfSentences[b] == b)
                //{
                
                //    temp.Add(b);
                    //row.Cells.Add(i);
                    //dataGridView1.Rows.Add(
                    //row.Cells.Add(i);
                   //dataGridView1.Rows.Add(row);
                //if (NoOfSentences[i] != null && NoOfSentences[i]!=102)
                //{
                    dataGridView1[0, i].Value = i.ToString();
                    dataGridView1.Rows.Add();
                //}
                //else
                //{
                //    dataGridView1[0, i].Value = i.ToString();
                //    dataGridView1.Rows.Add();
                //}
                    

                //}
                //else
                //{

                //}
                //b++;
            }


            //for (int a = 0; a < temp.Count; a++)
            //{
            //    dataGridView1.Rows.Add(temp[a]);
 
            //}
   
        }


        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        List<int> ChosenPTIndex = new List<int>();
        //List<int> Count;
        private void btn_addToparsetrees_Click(object sender, EventArgs e)
        {
            //ChosenPTIndex = new List<int>(NoOfSentences.Count);

            for (int i = 0; i < NoOfSentences.Count; i++)
            {
                ChosenPTIndex.Add(int.Parse(dataGridView1[1, i].Value.ToString()));
            }



            //choose parsetrees
            //ChosenParseTrees = new ArrayList(NoOfSentences.Count);
            for (int i = 0; i < NoOfSentences.Count; i++)
            {
                int ChosenIndex = 0;
                //if (Indices.Contains(i))
                //{
                ChosenIndex = NewIndices.IndexOf(i);
                int count = int.Parse(dataGridView1[1, i].Value.ToString());
                ChosenParseTrees.Add(GivenParseTrees[ChosenIndex + count]);
                //}
            }


            MMG.Form1.SParseTrees = ChosenParseTrees;
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //MMG.Form1.SParseTrees = ChosenParseTrees;
        }
    }
}
