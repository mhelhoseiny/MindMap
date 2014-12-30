using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using WordsMatching;
using WordNetClasses;
using Wnlib;
using OntologyLibrary;
using System.Runtime.Serialization;



namespace WordologyManager
{
    public partial class FrmEnrich : Form
    {
        public FrmEnrich()
        {
            InitializeComponent();
            Wnlib.WNCommon.path = "C:\\Program Files (x86)\\WordNet\\2.1\\dict\\";
            LoadOntologyConcepts();
            GetWordologyList();
        }
        ArrayList ArrWordology;
        string _wordologyDirectoryPath ="";
        string _ConceptsPath=@"AllConcepts.txt";
        ArrayList ArrConcepts = new ArrayList();

        private void button1_Click(object sender, EventArgs e)
        {
            
            AddMappingPair();
            
        }


        private void RunMapping()
        {
            Wnlib.WNCommon.path = "C:\\Program Files (x86)\\WordNet\\2.1\\dict\\";
            OntologyMapperGenerator omg = new OntologyMapperGenerator(@"..\..\..\Ontology\Formatted OntoSem");
            omg.ConstructMapping();
        }

        private void AddMappingPair()
        {
            WordOlogy wordology = new WordOlogy();
            wordology.Concept = comboBoxConcepts.Text;
            wordology.ID = ArrWordology.Count + 1;
            wordology.Pos = comboBoxPos.Text;
			wordology.Word = textBoxWord.Text;
            string sense = lstSenses.SelectedItem.ToString();
            string[] strsplt = sense.Split('*');
             sense = strsplt[1].Substring(2);
             wordology.Sense = sense;
            ArrWordology.Add(wordology);
            lstSenses.Items.Remove(lstSenses.SelectedItem);
            listBox2.Items.Add("* " + sense);
        }
        private void GetWordologyList()
        {

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

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(_wordologyDirectoryPath + @"\wordology.txt", FileMode.Create);
            bf.Serialize(fs, ArrWordology);
            fs.Close();
        }
        private void buttonMeaning_Click(object sender, EventArgs e)
        {
            lstSenses.Items.Clear();
            ArrayList senses =  WordSenses.GetAllSenses(textBoxWord.Text,comboBoxPos.SelectedItem.ToString());
            foreach (string str in senses)
            {
                lstSenses.Items.Add("* " + str);
            }
            

        }
        private void LoadOntologyConcepts()
        {
            string concept="";
            FileStream allConceptsFile = new FileStream(_ConceptsPath , FileMode.Open);
            StreamReader allConceptsFileReader = new StreamReader(allConceptsFile);
            while ((concept = allConceptsFileReader.ReadLine()) != null)
            {
                ArrConcepts.Add(concept);
                comboBoxConcepts.Items.Add(concept);
            }
        }

        
        private void GetConceptMappedSenses(string selectedConcept)
        {
            listBox2.Items.Clear();
            foreach (WordOlogy wo in ArrWordology)
            {
                if (wo.Concept == selectedConcept)
                {
                    string item = "* " +wo.Word+"-"+wo.Pos+":"+ wo.Sense;
					//if (!listBox2.Items.Contains(item))
					//{
					    listBox2.Items.Add(item);
					//}
                    //listBox2.Items.Add(wo.Word + "\t" +":"+ wo.Sense);
                }
            }
            
        }

        private void buttonConceptSenses_Click(object sender, EventArgs e)
        {

            GetConceptMappedSenses(comboBoxConcepts.Text);
        }

        private void actionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void regenerateMapperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you sure tht you want to reconstruct Automatic Mapper.This will take several minutes (i.e. in hours) dependin on your machine","Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {

                RunMapping();
            }
        }

        private void testMapperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Test t  =new Test();
            t.Show();
        }

        private void countOfMappingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(ArrWordology.Count.ToString(), "Count of Mappings", MessageBoxButtons.OK);
        }

        private void btnGetConcepts_Click(object sender, EventArgs e)
        
		{
			if (lstSenses.SelectedItem != null)
			{
				string senseLine = this.lstSenses.SelectedItem.ToString();
				int senseEndIndex = senseLine.LastIndexOf(' ');
				string sense = senseLine.Substring(2, senseEndIndex - 2);
				string s = "";
				foreach (WordOlogy wo in ArrWordology)
				{
					if (wo.Sense == sense)
					{
						s += wo.Concept + "\n";
					}
				}
				if(s=="")
					MessageBox.Show("No concepts mapped to this sense");
				else
					MessageBox.Show(s);
			}
			else
			{
				MessageBox.Show("select a sense","error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
        }
		List<WordOlogy> lstWordology;
		private void btnUniqueMap_Click(object sender, EventArgs e)
		{
			lstWordology = new List<WordOlogy>();
			ComparerWordology comparer=new ComparerWordology();
			ArrWordology.Sort(comparer);
			for (int i = 0; i < ArrWordology.Count; i++)
			{
				if (i==0||comparer.Compare(ArrWordology[i],ArrWordology[i-1])!=0)
				{
					lstWordology.Add((WordOlogy)ArrWordology[i]);
				}
			}
			IFormatter formatter = new BinaryFormatter();
			FileStream file = new FileStream("UniqueWordology.bin", FileMode.Create, FileAccess.Write);
			formatter.Serialize(file,lstWordology);
			file.Close();
			int unUniqueSenses = 0;
			for (int i = 0; i < lstWordology.Count; i++)
			{
				if (i != 0 && lstWordology[i].Concept.CompareTo(lstWordology[i - 1].Concept) == 0)
				{
					unUniqueSenses++;
					MessageBox.Show(lstWordology[i].Concept);
				}
			}
			MessageBox.Show("unique concepts count = " + lstWordology.Count.ToString()+"\nconcepts has more than one sense = "+unUniqueSenses+"number of concepts = "+this.comboBoxConcepts.Items.Count);
		}


		class ComparerWordology:IComparer
		{

			#region IComparer Members

			public int Compare(object x, object y)
			{
				WordOlogy w1 = (WordOlogy)x, w2 = (WordOlogy)y;
				int cconcept = w1.Concept.CompareTo(w2.Concept),csense=w1.Sense.CompareTo(w2.Sense);
				if (cconcept==0)
					return csense;
				return cconcept;
			}

			#endregion
		}
        
      
    }
}