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

namespace MapperTool
{
	public partial class frmMREditor : Form
	{
		public frmMREditor()
		{
			InitializeComponent();
		}
		List<NounFrame> nouns;
		Ontology onto;
		List<VerbFrame> verbs;
		MMViewManeger viewer;
		private void Form1_Load(object sender, EventArgs e)
		{
			nouns = new List<NounFrame>();
			verbs = new List<VerbFrame>();
			MindMapTMR tmr = new MindMapTMR();
			tmr.Nounframes = nouns;
			tmr.VerbFrames = verbs;
			viewer = new MMViewManeger(this.pnlDrawing, tmr);
			/*
			for (int i = 0; i < 3; i++)
			{
				this.cmbDomainRelations.Items.Add(((DomainRelationType)i).ToString());
			}
			//nouns[0].*/
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
			onto = new Ontology(@"Formatted OntoSem");
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void btnAddNounFrame_Click(object sender, EventArgs e)
		{
			FrmAddEditFrame form = new FrmAddEditFrame("Add noun frame");
			if (form.ShowDialog()==DialogResult.OK)
			{
				this.nouns.Add(form.nounResult);
				UpdateCmbNouns();
				this.viewer.AddNounFrameEntity(form.nounResult, new Point(0, 0));
			}
		}

		private void UpdateCmbNouns()
		{
			this.cmbNounFrames.Items.Clear();
			for (int i = 0; i < this.nouns.Count; i++)
				this.cmbNounFrames.Items.Add(nouns[i].Text);
			this.cmbNounFrames.SelectedIndex = this.cmbNounFrames.Items.Count - 1;
		}

		private void UpdateCmbVerbs()
		{
			this.cmbVerbFrame.Items.Clear();
			for (int i = 0; i < this.verbs.Count; i++)
				this.cmbVerbFrame.Items.Add(verbs[i].VerbName);
			this.cmbVerbFrame.SelectedIndex = this.cmbVerbFrame.Items.Count - 1;
		}

		private void btnAddCaseRole_Click(object sender, EventArgs e)
		{
			FrmCaseRoleRelation form = new FrmCaseRoleRelation(nouns, verbs);
			if (form.ShowDialog()==DialogResult.OK)
			{
				NounFrame currentNoun = nouns[form.NounIndex];
				CaseRole c = form.Relation;
				int currentVerbIndex = form.VerbIndex;
				if (!this.verbs[currentVerbIndex].CaseRoles.ContainsKey(c) || !this.verbs[currentVerbIndex].CaseRoles[c].Contains(currentNoun))
				{
					this.verbs[currentVerbIndex].AddCaseRole(c, currentNoun);

					this.viewer.UpdateRelations();
				}
			}
		}

		private void btnAddDomainRelation_Click(object sender, EventArgs e)
		{
			FrmVerbRelation form = new FrmVerbRelation(verbs);
			if (form.ShowDialog() == DialogResult.OK)
			{
				int v1Index=form.VerbIndex1;
				DomainRelationType d = form.domainRelation;
				int v2Index = form.VerbIndex2;
				if (!this.verbs[v1Index].DomainRelations.ContainsKey(d) || !this.verbs[v1Index].DomainRelations[d].Contains(verbs[v2Index]))
				{
					this.verbs[v1Index].AddDomainRelation(d, verbs[v2Index]);
					this.viewer.UpdateRelations();
				}
			}
		}

		private void btnEditNounFrame_Click(object sender, EventArgs e)
		{
			if (this.cmbNounFrames.SelectedIndex!=-1)
			{
				FrmAddEditFrame form = new FrmAddEditFrame(this.nouns[this.cmbNounFrames.SelectedIndex]);
				if (form.ShowDialog()==DialogResult.OK)
				{
					this.nouns[this.cmbNounFrames.SelectedIndex] = form.nounResult;
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
					//this.viewer.DicNounFrame.Remove(this.nouns[this.cmbNounFrames.SelectedIndex]);
					this.nouns.RemoveAt(this.cmbNounFrames.SelectedIndex);
					int index=this.cmbNounFrames.SelectedIndex;
					UpdateCmbNouns();
					if (this.nouns.Count == index)
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
				this.verbs.Add(form.verbResult);
				UpdateCmbVerbs();
				this.viewer.AddVerbFrameEntity(form.verbResult, new Point(0, 0));
			}

		}

		private void btnEditVerbFrame_Click(object sender, EventArgs e)
		{
			if (this.cmbVerbFrame.SelectedIndex != -1)
			{
				FrmAddEditFrame form = new FrmAddEditFrame(this.verbs[this.cmbVerbFrame.SelectedIndex]);
				if (form.ShowDialog() == DialogResult.OK)
				{
					this.verbs[this.cmbVerbFrame.SelectedIndex] = form.verbResult;
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
					this.verbs.RemoveAt(this.cmbVerbFrame.SelectedIndex);
					int index = this.cmbVerbFrame.SelectedIndex;
					UpdateCmbVerbs();
					if (this.verbs.Count == index)
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
		[Serializable]
		struct DataToSave
		{
			public List<NounFrame> nouns;
			public List<VerbFrame> verbs;
		}
		private void btnSave_Click(object sender, EventArgs e)
		{
			if (this.saveFileDialog1.ShowDialog()==DialogResult.OK)
			{
				DataToSave obj = new DataToSave();
				obj.nouns = nouns;
				obj.verbs = verbs;
				IFormatter formatter = new BinaryFormatter();
				Stream file=saveFileDialog1.OpenFile();
				formatter.Serialize(file, obj);
				file.Close();
			}
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			if (this.openFileDialog1.ShowDialog()==DialogResult.OK)
			{
				IFormatter formatter = new BinaryFormatter();
				Stream file = openFileDialog1.OpenFile();
				DataToSave obj = (DataToSave)formatter.Deserialize(file);
				this.verbs = obj.verbs;
				nouns = obj.nouns;
				viewer.Clear();
			}
		}

	}
}
