using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MindMapViewingManagement;
using MindMapGenerator.Drawing_Management;

namespace MMG
{
    [Serializable]
    public partial class Form6 : Form
    {
        public int Type;
        public MMViewManeger mmvm;
        public Form5 parentform;
        public Form6(Form5 Parentform)
        {
            parentform = Parentform;
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        private void stopLayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mmvm != null)
                if (mmvm.LayoutiingThread.ThreadState != System.Threading.ThreadState.Stopped&& 
                mmvm.LayoutiingThread.ThreadState != System.Threading.ThreadState.Aborted )
                {
                    mmvm.LayoutiingThread.Abort();
                }

        }

        private void adjustNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void rerunLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mmvm != null)
                mmvm.BuildAutomaticLayoutedGraph();
        }

        private void initLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mmvm != null)
                mmvm.BuildAutomaticLayoutedGraph(true);
        }

        private void setLayoutToAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parentform.SetFormLayoutToAll(this);
        }

        private void seAlltLocationsFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<List<PointF>> pts =  parentform.LoadLocationsFrom(openFileDialog1.FileName);

                mmvm.LoadLocationFrom(pts[0]);
            }
            
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            int size = 0;
          
            txtSize.Text = mmvm.Links.Count.ToString();

        }
    }
}
