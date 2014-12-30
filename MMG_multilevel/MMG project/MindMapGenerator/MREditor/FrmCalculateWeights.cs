using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mmTMR;
using MindMapMeaningRepresentation;

namespace MapperTool
{
	public partial class FrmCalculateWeights : Form
	{
		public FrmCalculateWeights(MindMapTMR tmr)
		{
			InitializeComponent();
			DirectRelationBasedTMRWeighter2 weighter = new DirectRelationBasedTMRWeighter2(tmr);
			List<double> nounsWeight = weighter.GetNounFrameWeights();
			List<double> verbsWeight = weighter.GetVerbFrameWeights();
            this.dataGridView1.Columns[3].ValueType = typeof(double);
			for (int i = 0; i < tmr.Nounframes.Count; i++)
			{
				int rowIndex = this.dataGridView1.Rows.Add();
				this.dataGridView1[0, rowIndex].Value = tmr.Nounframes[i].Text;
                this.dataGridView1[1, rowIndex].Value = "noun";
                this.dataGridView1[2, rowIndex].Value = tmr.Nounframes[i].Concept.Name;
				this.dataGridView1[3, rowIndex].Value = nounsWeight[i];
			}
            this.dataGridView2.Columns[3].ValueType = typeof(double);
			for (int i = 0; i < tmr.VerbFrames.Count; i++)
			{
                
				int rowIndex = this.dataGridView2.Rows.Add();
				this.dataGridView2[0, rowIndex].Value = tmr.VerbFrames[i].VerbName;
                this.dataGridView2[1, rowIndex].Value = "verb";
                this.dataGridView2[2, rowIndex].Value = tmr.VerbFrames[i].Predicate.Name;
				this.dataGridView2[3, rowIndex].Value = verbsWeight[i];
			}
            //this.dataGridView1.Columns[3].ValueType = typeof(double);
			this.dataGridView1.Sort(this.dataGridView1.Columns[3], ListSortDirection.Descending);
            this.dataGridView2.Sort(this.dataGridView2.Columns[3], ListSortDirection.Descending);
		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		
	}
}
