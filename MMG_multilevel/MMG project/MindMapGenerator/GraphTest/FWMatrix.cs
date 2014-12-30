using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GraphTest
{
    public partial class FWMatrix : Form
    {
        double[,] Dij;
        int Count;
        
        public FWMatrix(double[,] dij, int count)
        {
            this.Dij = dij;
            this.Count = count;
            InitializeComponent();
        }

        private void FWMatrix_Load(object sender, EventArgs e)
        {
            
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    richTextBox1.AppendText(Math.Round(Dij[i, j],3) + "\t");
                }
                richTextBox1.AppendText("\n");
            }
            
        }
    }
}