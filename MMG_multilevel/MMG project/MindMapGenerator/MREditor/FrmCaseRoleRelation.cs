using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mmTMR;

namespace MapperTool
{
	public partial class FrmCaseRoleRelation : Form
	{
		List<NounFrame> nouns;
		List<VerbFrame> verbs;
		public int NounIndex,VerbIndex;
		public CaseRole Relation;
		public FrmCaseRoleRelation(List<NounFrame> nounFrames, List<VerbFrame> verbFrames)
		{
			InitializeComponent();
			for (int i = 0; i < 10; i++)
				this.cmbCaseRoles.Items.Add(((CaseRole)i).ToString());
			this.nouns = nounFrames;
			this.verbs = verbFrames;
			for (int i = 0; i < this.nouns.Count; i++)
			{
				this.cmbNounFrames.Items.Add(this.nouns[i].Text);
			}
			for (int i = 0; i < this.verbs.Count; i++)
			{
				this.cmbVerbFrames.Items.Add(this.verbs[i].VerbName);
			}
		}
		public FrmCaseRoleRelation(List<NounFrame> nounFrames, List<VerbFrame> verbFrames, int nounIndex, int verbIndex): this(nounFrames, verbFrames)
		{
			this.cmbNounFrames.SelectedIndex = nounIndex;
			this.cmbVerbFrames.SelectedIndex = verbIndex;
			this.NounIndex = nounIndex;
			this.VerbIndex = verbIndex;
		}
		public FrmCaseRoleRelation(List<NounFrame> nounFrames, List<VerbFrame> verbFrames, int nounIndex, int verbIndex, CaseRole relation)
			: this(nounFrames, verbFrames,nounIndex,verbIndex)
		{
			this.Relation = relation;
			this.cmbCaseRoles.SelectedIndex = (int)relation;
		}
		private void FrmCaseRoleRelation_Load(object sender, EventArgs e)
		{
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			this.NounIndex = this.cmbNounFrames.SelectedIndex;
			this.VerbIndex = this.cmbVerbFrames.SelectedIndex;
			this.Relation = (CaseRole)this.cmbCaseRoles.SelectedIndex;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
