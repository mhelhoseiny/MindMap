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
	public partial class FrmVerbRelation : Form
	{
		List<VerbFrame> verbs;
		public int VerbIndex1,VerbIndex2;
		public DomainRelationType domainRelation;
		public FrmVerbRelation(List<VerbFrame> verbFrames)
		{
			InitializeComponent();
			//for (int i = 0; i < 3; i++)
			//    this.c.Items.Add(((CaseRole)i).ToString());
			for (int i = 0; i < 3; i++)
			{
				this.cmbDomainRelation.Items.Add(((DomainRelationType)i).ToString());
			}

			this.verbs = verbFrames;
			for (int i = 0; i < this.verbs.Count; i++)
			{
				this.cmbverbFrames1.Items.Add(this.verbs[i].VerbName);
			}
			for (int i = 0; i < this.verbs.Count; i++)
			{
				this.cmbVerbFrames2.Items.Add(this.verbs[i].VerbName);
			}
		}
		public FrmVerbRelation(List<VerbFrame> verbFrames, int verbIndex1, int verbIndex2)
			: this(verbFrames)
		{
			this.cmbverbFrames1.SelectedIndex = verbIndex1;
			this.cmbVerbFrames2.SelectedIndex = verbIndex2;
			this.VerbIndex1 = verbIndex1;
			this.VerbIndex2 = verbIndex2;
		}
		//public FrmVerbRelation(List<NounFrame> nounFrames, List<VerbFrame> verbFrames, int nounIndex, int verbIndex, CaseRole relation)
		//    : this(nounFrames, verbFrames,nounIndex,verbIndex)
		//{
		//    this.Relation = relation;
		//    this.cmbDomainRelation.SelectedIndex = (int)relation;
		//}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			this.VerbIndex1 = this.cmbverbFrames1.SelectedIndex;
			this.VerbIndex2 = this.cmbVerbFrames2.SelectedIndex;
			this.domainRelation = (DomainRelationType)this.cmbDomainRelation.SelectedIndex;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
