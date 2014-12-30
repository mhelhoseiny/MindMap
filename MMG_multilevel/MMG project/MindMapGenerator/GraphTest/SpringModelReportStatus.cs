using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GraphTest
{
    public partial class SpringModelReportStatus : Form
    {
        SpringModel _spModel;
        public SpringModelReportStatus(SpringModel spModel)
        {
            _spModel = spModel;
            InitializeComponent();
        }

        private void SpringModelReportStatus_Load(object sender, EventArgs e)
        {
            if (_spModel == null)
            {
                this.Close();
                return;            
            }
            int Count =_spModel.Nodes.Count;
            txt_Report.AppendText("Lij\n=======================================================\n");
            txt_Report.AppendText(" \t  ");
            
            for (int j = 0; j < Count; j++)
            {
                txt_Report.AppendText(j.ToString()+"\t");
            }
            txt_Report.AppendText("\n");
            
            for (int i = 0; i < Count; i++)
            {
                txt_Report.AppendText(i.ToString() + "\t: ");
            
                for (int j = 0; j < Count; j++)
                {
                    txt_Report.AppendText(Math.Round(_spModel.Lij(i, j), 3) + "\t");
                }
                txt_Report.AppendText("\n");
            }

            txt_Report.AppendText("\nKij\n=======================================================\n");

            txt_Report.AppendText(" \t  ");
            for (int j = 0; j < Count; j++)
            {
               
                txt_Report.AppendText(j.ToString() + "\t");
            }
            txt_Report.AppendText("\n");
            for (int i = 0; i < Count; i++)
            {
                txt_Report.AppendText(i.ToString() + "\t: ");

                for (int j = 0; j < Count; j++)
                {
                    txt_Report.AppendText(Math.Round(_spModel.Kij(i, j), 3) + "\t");
                }
                txt_Report.AppendText("\n");
            }

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            for (int i = 0; i < _spModel.Nodes.Count; i++)
            {
                if (_spModel.Nodes[i].Xposition > maxX)
                {
                    maxX = _spModel.Nodes[i].Xposition;
                }
                if (_spModel.Nodes[i].Yposition > maxY)
                {
                    maxY = _spModel.Nodes[i].Yposition;
                }
                if (_spModel.Nodes[i].Xposition < minX)
                {
                    minX = _spModel.Nodes[i].Xposition;
                }
                if (_spModel.Nodes[i].Yposition < minY)
                {
                    minY = _spModel.Nodes[i].Yposition;
                }
            }
            txt_Report.AppendText("\nBounding Rectangle is defined by\n");
            txt_Report.AppendText("minX  = " + minX + ",maxX = " + maxX + ",minY  = " + minY + ",maxY = " + maxY+"\n");
            txt_Report.AppendText("\n");
            //txt_Report.AppendText("Energy\n=============================================");
            //List<double> energy = _spModel.GetNodesEnergy();
            //for (int i = 0; i < Count; i++)
            //{
            //    txt_Report.AppendText("\n");
            //    txt_Report.AppendText("Energy(" + i.ToString() + ")");
            //    txt_Report.AppendText(energy[i].ToString());
            //}
        }
    }
}
