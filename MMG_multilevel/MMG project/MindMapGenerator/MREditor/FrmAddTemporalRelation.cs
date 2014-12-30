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
    public partial class FrmAddTemporalRelation : Form
    {
        List<VerbFrame> verbs;
        public int VerbIndex1, VerbIndex2;
        public TemporalRelationType temporalRelation;
        public FrmAddTemporalRelation(List<VerbFrame> verbFrames)
        {
            //InitializeComponent();
            InitializeComponent();
            //for (int i = 0; i < 3; i++)
            //    this.c.Items.Add(((CaseRole)i).ToString());
            for (int i = 0; i < 3; i++)
            {
                this.cmbTemporalRelation.Items.Add(((TemporalRelationType)i).ToString());
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

        public FrmAddTemporalRelation(List<VerbFrame> verbFrames, int verbIndex1, int verbIndex2)
			: this(verbFrames)
		{
			this.cmbverbFrames1.SelectedIndex = verbIndex1;
			this.cmbVerbFrames2.SelectedIndex = verbIndex2;
			this.VerbIndex1 = verbIndex1;
			this.VerbIndex2 = verbIndex2;
		}

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.VerbIndex1 = this.cmbverbFrames1.SelectedIndex;
            this.VerbIndex2 = this.cmbVerbFrames2.SelectedIndex;
            this.temporalRelation = (TemporalRelationType)this.cmbTemporalRelation.SelectedIndex;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
