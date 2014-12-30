using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mmTMR;
using OurMindMapOntology;

namespace MapperTool
{
	public partial class FrmAddEditFrame : Form
	{
        MindMapOntology _ontology;
		public NounFrame nounResult;
		public VerbFrame verbResult;
		public FrmAddEditFrame()
		{
            _ontology = new MindMapOntology("1.owl");
			InitializeComponent();
            LoadComboBox();
			//this.cmbConcepts.Items.AddRange(LoadConcepts.LoadGetFile());
			nounResult = null;
			verbResult = null;
		}

        private void LoadComboBox()
        {
            List<string> conceptStrings = new List<string>();
            foreach (string str in _ontology.Concepts.Keys)
            {
                conceptStrings.Add(_ontology.Concepts[str].Name);
            }
            conceptStrings.Sort();
            this.cmbConcepts.Items.AddRange(conceptStrings.ToArray());
            
        }
		public FrmAddEditFrame(NounFrame noun):this()
		{
			this.txtFrameText.Text = noun.Text;
            
			this.cmbConcepts.SelectedItem = noun.Concept;
			this.Text = "Edit " + noun.Text;
			nounResult = noun;
		}
		public FrmAddEditFrame(string formName):this()
		{
			this.Text = formName;
		}
		public FrmAddEditFrame(VerbFrame verb)
		{
			this.txtFrameText.Text = verb.VerbName;
			this.cmbConcepts.SelectedItem = verb.Predicate.Name;
			this.Text = "Edit " + verb.VerbName;
			verbResult = verb;
		}
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
            MindMapConcept c = this._ontology.Concepts[this.cmbConcepts.SelectedItem.ToString()];
			this.nounResult = new NounFrame(this.txtFrameText.Text, c);
			this.verbResult = new VerbFrame(this.txtFrameText.Text, c);
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void FrmAddEditFrame_Load(object sender, EventArgs e)
		{

		}
	}
}
