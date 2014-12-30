using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mmTMR;
using System.IO;
using OntologyLibrary.OntoSem;
using MindMapViewingManagement;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MindMapMeaningRepresentation;
using MultilevelGenerator;
using WordsMatching;
using SyntacticAnalyzer;
using OurMindMapOntology;


namespace MapperTool
{
	public partial class frmMREditor : Form
	{
		public frmMREditor()
		{
			InitializeComponent();
		}
		public MindMapTMR tmr;
		//DirectRelationBasedTMRWeighter weighter;
		MMViewManeger viewer;
		private void Form1_Load(object sender, EventArgs e)
		{
			tmr = new MindMapTMR();
			tmr.Nounframes = new List<NounFrame>();
			tmr.VerbFrames = new List<VerbFrame>();
			viewer = new MMViewManeger(this.pnlDrawing, tmr,false);
			//weighter = new DirectRelationBasedTMRWeighter(tmr);
			/*
			for (int i = 0; i < 3; i++)
			{
				this.cmbDomainRelations.Items.Add(((DomainRelationType)i).ToString());
			}
			//tmr.Nounframes[0].*/
/*			
            StreamReader sr = new StreamReader(@"Formatted OntoSem\Get.txt");
			string str = null;
			int x = 0;
            List<string> items = new List<string>();
            while ((str = sr.ReadLine()) != null)
            {
                //if (++x < 2000)
                {
                    if (str == "")
                        continue;
                    items.Add(str);
                    //cmbConceptNames.Items.Add(str);
                    //cmbFirstConcept.Items.Add(str);
                    //cmbSecondConcept.Items.Add(str);
                }
            }

            string[] itemsArr = items.ToArray();
			//cmbConceptNames.Items.AddRange(items);
			//cmbFirstConcept.Items.AddRange(items);
			//cmbSecondConcept.Items.AddRange(items);
			sr.Close();
            cmbNounConcepts.Items.AddRange(itemsArr);
            cmbVerbConcepts.Items.AddRange(itemsArr);
			//TODO: make loading ontology from resource file from resource file*/
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void btnAddNounFrame_Click(object sender, EventArgs e)
		{
			FrmAddEditFrame form = new FrmAddEditFrame("Add noun frame");
			if (form.ShowDialog()==DialogResult.OK)
			{
				this.tmr.Nounframes.Add(form.nounResult);
				UpdateCmbNouns();
				this.viewer.AddNounFrameEntity(form.nounResult, new Point(0, 0));
			}
		}

		private void UpdateCmbNouns()
		{
			this.cmbNounFrames.Items.Clear();
			for (int i = 0; i < this.tmr.Nounframes.Count; i++)
				this.cmbNounFrames.Items.Add(tmr.Nounframes[i].Text);
			this.cmbNounFrames.SelectedIndex = this.cmbNounFrames.Items.Count - 1;
		}

		private void UpdateCmbVerbs()
		{
			this.cmbVerbFrame.Items.Clear();
			for (int i = 0; i < this.tmr.VerbFrames.Count; i++)
				this.cmbVerbFrame.Items.Add(tmr.VerbFrames[i].VerbName);
			this.cmbVerbFrame.SelectedIndex = this.cmbVerbFrame.Items.Count - 1;
		}

		private void btnAddCaseRole_Click(object sender, EventArgs e)
		{
			FrmCaseRoleRelation form = new FrmCaseRoleRelation(tmr.Nounframes, tmr.VerbFrames);
			if (form.ShowDialog()==DialogResult.OK)
			{
				NounFrame currentNoun = tmr.Nounframes[form.NounIndex];
				CaseRole c = form.Relation;
				int currentVerbIndex = form.VerbIndex;
				if (!this.tmr.VerbFrames[currentVerbIndex].CaseRoles.ContainsKey(c) || !this.tmr.VerbFrames[currentVerbIndex].CaseRoles[c].Contains(currentNoun))
				{
					this.tmr.VerbFrames[currentVerbIndex].AddCaseRole(c, currentNoun);
					this.viewer.UpdateRelations();
				}
			}
		}

		private void btnAddDomainRelation_Click(object sender, EventArgs e)
		{
			FrmDomainRelation form = new FrmDomainRelation(tmr.VerbFrames);
			if (form.ShowDialog() == DialogResult.OK)
			{
				int v1Index=form.VerbIndex1;
				DomainRelationType d = form.domainRelation;
				int v2Index = form.VerbIndex2;
				if (!this.tmr.VerbFrames[v1Index].DomainRelations.ContainsKey(d) || !this.tmr.VerbFrames[v1Index].DomainRelations[d].Contains(tmr.VerbFrames[v2Index]))
				{
					this.tmr.VerbFrames[v1Index].AddDomainRelation(d, tmr.VerbFrames[v2Index]);
					this.viewer.UpdateRelations();
				}
			}
		}

		private void btnEditNounFrame_Click(object sender, EventArgs e)
		{
			if (this.cmbNounFrames.SelectedIndex!=-1)
			{
				FrmAddEditFrame form = new FrmAddEditFrame(this.tmr.Nounframes[this.cmbNounFrames.SelectedIndex]);
				if (form.ShowDialog()==DialogResult.OK)
				{
					this.tmr.Nounframes[this.cmbNounFrames.SelectedIndex] = form.nounResult;
					UpdateCmbNouns();
				}
			}
		}

		private void btnDeleteNounFrame_Click(object sender, EventArgs e)
		{
			if (this.cmbNounFrames.SelectedIndex != -1)
			{
				if (MessageBox.Show("Are you sure you want to delete?")==DialogResult.OK)
				{
					//this.viewer.DicNounFrame.Remove(this.tmr.Nounframes[this.cmbNounFrames.SelectedIndex]);
					this.tmr.Nounframes.RemoveAt(this.cmbNounFrames.SelectedIndex);
					int index=this.cmbNounFrames.SelectedIndex;
					UpdateCmbNouns();
					if (this.tmr.Nounframes.Count == index)
					{
						this.cmbNounFrames.SelectedIndex = index - 1;
					}
					else
						this.cmbNounFrames.SelectedIndex = index;
				}
			}

		}

		private void btnAddVerbFrame_Click(object sender, EventArgs e)
		{
			FrmAddEditFrame form = new FrmAddEditFrame("Add verb frame");
			if (form.ShowDialog() == DialogResult.OK)
			{
				this.tmr.VerbFrames.Add(form.verbResult);
				UpdateCmbVerbs();
				this.viewer.AddVerbFrameEntity(form.verbResult, new Point(0, 0));
			}

		}

		private void btnEditVerbFrame_Click(object sender, EventArgs e)
		{
			if (this.cmbVerbFrame.SelectedIndex != -1)
			{
				FrmAddEditFrame form = new FrmAddEditFrame(this.tmr.VerbFrames[this.cmbVerbFrame.SelectedIndex]);
				if (form.ShowDialog() == DialogResult.OK)
				{
					this.tmr.VerbFrames[this.cmbVerbFrame.SelectedIndex] = form.verbResult;
					UpdateCmbNouns();
				}
			}

		}

		private void btnDeleteVerbFrame_Click(object sender, EventArgs e)
		{
			if (this.cmbVerbFrame.SelectedIndex != -1)
			{
				if (MessageBox.Show("Are you sure you want to delete?") == DialogResult.OK)
				{
					this.tmr.VerbFrames.RemoveAt(this.cmbVerbFrame.SelectedIndex);
					int index = this.cmbVerbFrame.SelectedIndex;
					UpdateCmbVerbs();
					if (this.tmr.VerbFrames.Count == index)
					{
						this.cmbVerbFrame.SelectedIndex = index - 1;
					}
					else
						this.cmbVerbFrame.SelectedIndex = index;
				}
			}

		}
		void Display()
		{
			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Display();
		}
		private void btnSave_Click(object sender, EventArgs e)
		{
			if (this.saveFileDialog1.ShowDialog()==DialogResult.OK)
			{
				IFormatter formatter = new BinaryFormatter();
				Stream file=saveFileDialog1.OpenFile();
				formatter.Serialize(file, viewer);
				file.Close();
			}
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			if (this.openFileDialog1.ShowDialog()==DialogResult.OK)
			{
				viewer.Clear();
				IFormatter formatter = new BinaryFormatter();
				Stream file = openFileDialog1.OpenFile();
				viewer = (MMViewManeger)formatter.Deserialize(file);
				viewer.Control=this.pnlDrawing;
				tmr = viewer._TMR;
				this.UpdateCmbNouns();
				this.UpdateCmbVerbs();
				file.Close();
			}
		}

		private void cmbNounFrames_SelectedIndexChanged(object sender, EventArgs e)
		{
			//if (this.cmbNounFrames.SelectedIndex!=-1)
			//    this.lblNounWeight.Text = weighter.WeighNounFrame(this.cmbNounFrames.SelectedIndex).ToString();
		}

		private void cmbVerbFrame_SelectedIndexChanged(object sender, EventArgs e)
		{
			//if (this.cmbVerbFrame.SelectedIndex != -1)
			//    this.lblVerbWeight.Text = weighter.WeighVerbFrame(this.cmbVerbFrame.SelectedIndex).ToString();
		}

		private void btnCalculate_Click(object sender, EventArgs e)
		{
			new FrmCalculateWeights(tmr).Show();
		}

        private void button_addTemporalRelation_Click(object sender, EventArgs e)
        {
            FrmAddTemporalRelation form = new FrmAddTemporalRelation(tmr.VerbFrames);
            if (form.ShowDialog() == DialogResult.OK)
            {
                int v1Index = form.VerbIndex1;
                TemporalRelationType d = form.temporalRelation;
                int v2Index = form.VerbIndex2;
                if (!this.tmr.VerbFrames[v1Index].TemporalRelations.ContainsKey(d) || !this.tmr.VerbFrames[v1Index].TemporalRelations[d].Contains(tmr.VerbFrames[v2Index]))
                {
                    this.tmr.VerbFrames[v1Index].AddTemporalRelation(d, tmr.VerbFrames[v2Index]);
                    this.viewer.UpdateRelations();
                }
            }
        }

        MultiLevel ML;
        MindMapTMR MLTMR;

        private void button_Multilevel_Click(object sender, EventArgs e)
        {
            
            ML = new MultiLevel(this.tmr);
            MLTMR = ML.Run();

            FrmNewTMR newForm = new FrmNewTMR(MLTMR, this.tmr, ML);

            newForm.ShowDialog();
        }

        private void pnlDrawing_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
	}
}
