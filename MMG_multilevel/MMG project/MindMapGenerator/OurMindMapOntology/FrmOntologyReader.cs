using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OwlDotNetApi;
using WordsMatching;
using Wnlib;

namespace OurMindMapOntology
{
    public partial class FrmOntologyReader : Form
    {
        public FrmOntologyReader()
        {
            InitializeComponent();
        }
		MindMapOntology helper;
		private void btnLoad_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog()==DialogResult.OK)
			{
				helper = new MindMapOntology(openFileDialog1.FileName);
				//MindMapConcept c1 = helper.Concepts["READ"];
				//MindMapConcept c2 = helper.Concepts["GOLF"];
				//DistanceInfo info = MindMapConcept.Distance(c1, c2);
				//MessageBox.Show(info.Distance.ToString());
				foreach (string conceptName in helper.Concepts.Keys)
				{
					this.lboxChildren.Items.Add(conceptName);
				}
				MyWordInfo word = new MyWordInfo("accomplishment", PartsOfSpeech.Noun);
				word.Sense = 1;
				MindMapConcept concept = MindMapMapper.GetConcept(word, helper);
				if(concept==null)
					MessageBox.Show("Test");
				else
					MessageBox.Show(concept.Name);
			}
		}

		private void btnGetParent_Click(object sender, EventArgs e)
		{
			this.lblParent.Text = helper.Concepts[this.txtConceptName.Text].ParentConceptName;
		}

		private void btnGetChildren_Click(object sender, EventArgs e)
		{
			this.lboxChildren.Items.Clear();
			List<MindMapConcept> children = helper.Concepts[txtConceptName.Text].Disjoints;
			foreach (MindMapConcept concept in children)
			{
				this.lboxChildren.Items.Add(concept.Name);
			}
		}

		private void btnDefinition_Click(object sender, EventArgs e)
		{
			MindMapConcept concept = helper.Concepts["CAR"];
			List<string> names = helper.SetPropertiesCombinations("c:\\", concept);
			this.lblParent.Text = names.Count.ToString();
			//this.lblParent.Text = helper.GetDefinition(this.txtConceptName.Text);
		}

		private void btnMaplex_Click(object sender, EventArgs e)
		{
			List<MyWordInfo> maplexes = helper.Concepts[this.txtConceptName.Text].Maplex;
			for (int i = 0; i < maplexes.Count; i++)
			{
				this.lboxChildren.Items.Add(maplexes[i].Word + "\t" + maplexes[i].Pos + "\t" + maplexes[i].Sense);
			}
		}
    }
}
