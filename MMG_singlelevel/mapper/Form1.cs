using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

namespace OntologyLibrary
{
    public partial class Form1 : Form
    {
      

        public Form1()
        {
            InitializeComponent();
        }

        ArrayList ArrWordology;
        int correct=0;
        int incorrect=0;
        private void Form1_Load(object sender, EventArgs e)
        {
            //Wnlib.WNCommon.path = "C:\\Program Files (x86)\\WordNet\\2.1\\dict\\";
            //OntologyMapperGenerator omg = new OntologyMapperGenerator(@"..\..\..\Ontology\Formatted OntoSem");
            //omg.ConstructMapping();
            GetWordologyList();
            test();
        }
        private void GetWordologyList()
        {
            string _wordologyDirectoryPath =
              @"wordology\";

            _wordologyDirectoryPath = Application.ExecutablePath;
            int index = _wordologyDirectoryPath.LastIndexOf("\\");
            _wordologyDirectoryPath = _wordologyDirectoryPath.Substring(0, index);
            ArrayList strreader = new ArrayList();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(
                _wordologyDirectoryPath + @"\wordology.txt", FileMode.Open);
            StreamReader SR = new StreamReader(fs);
            ArrayList arr = new ArrayList();
            ArrWordology = (ArrayList)bf.Deserialize(fs);
            fs.Close();
        }

        private void test()
        {
            int id;
            string sense="";
            string word="";
            string concept = "";
            Random r = new Random();
            WordOlogy wdgy = new WordOlogy();
            for (int i = 0; i < 100; i++)
            {
                id=r.Next(0,ArrWordology.Count);
                wdgy = (WordOlogy)ArrWordology[id];
                sense = wdgy.Sense;
                word = wdgy.Word;
                concept = wdgy.Concept;
                label1.Text = concept;
                textBox1.Text = word+" : "+sense;
                string str=concept +" : "+ word + " : " + sense;
                listBox1.Items.Add(str);
                MessageBoxButtons msgbtn = MessageBoxButtons.YesNo;
                DialogResult result;
                result= MessageBox.Show(str,"",msgbtn);

                if (result == DialogResult.Yes)
                {
                    correct++;
                }
                if (result == DialogResult.No)
                {
                    incorrect++;
                }

            }
            MessageBox.Show(correct.ToString());
            MessageBox.Show(incorrect.ToString());

        }

        private void button1_Click(object sender, EventArgs e)
        {    
        //    Wnlib.WNCommon.path = "C:\\Program Files (x86)\\WordNet\\2.1\\dict\\";
        //    OntologyMapperGenerator omg = new OntologyMapperGenerator(@"..\..\..\Ontology\Formatted OntoSem");
        //    omg.ConstructMapping();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}