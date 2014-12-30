using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MindMapMeaningRepresentation;
using MindMapViewingManagement;
using mmTMR;

namespace MapperTool
{
    public partial class FrmNewTMR : Form
    {
        MindMapTMR OriginalTMR;
        MindMapTMR CurrentTMR;
        MultilevelGenerator.MultiLevel ML;

        MMViewManeger viewer;
        public FrmNewTMR(mmTMR.MindMapTMR TMR, mmTMR.MindMapTMR OriginalTMR, MultilevelGenerator.MultiLevel ML)
        {
            InitializeComponent();
            this.CurrentTMR = TMR;
            this.OriginalTMR = OriginalTMR;
            this.ML = ML;

            viewer = new MMViewManeger(this.panel1, this.CurrentTMR,false);
            viewer.Control = this.panel1;
            int MaximumY = viewer.getMaximumY();
            int MaximumX = viewer.getMaximumX();

            this.panel1.Height = MaximumY+40;
            this.panel1.Width = MaximumX+40;

            this.AutoScroll = true;


        }

        

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Frame f = viewer.getDoubleClickedFrame(e.X, e.Y);

            if (f != null)
            {
                MindMapTMR NewTMR = new MindMapTMR();

                if (f is NounFrame && this.ML.MainNounFrames.Contains((NounFrame)f) == false)
                {
                    List<VerbFrame> verbFrames = new List<VerbFrame>();
                    Dictionary<CaseRole, List<VerbFrame>> AssociatedActions = this.CurrentTMR.GetNounFrameAssociatedactions(this.CurrentTMR.Nounframes.IndexOf((NounFrame)f));
                    foreach (CaseRole cr in AssociatedActions.Keys)
                    {
                        foreach (VerbFrame vf in AssociatedActions[cr])
                        {
                            verbFrames.Add(vf);
                        }
                    }

                    foreach (VerbFrame vf in verbFrames)
                    {
                        VerbFrame original_vf = (VerbFrame)this.ML.NewFrame_OriginalFrame[vf];
                        NewTMR.VerbFrames.Add(original_vf);
                    }

                }
                else if (f is VerbFrame)
                {
                    NounFrame nf = null;

                    VerbFrame vf = (VerbFrame)f;
                    foreach (CaseRole cr in vf.CaseRoles.Keys)
                    {
                        foreach (NounFrame nf1 in vf.CaseRoles[cr])
                        {
                            foreach (NounFrame nf2 in ML.MainNounFrames)
                            {
                                if (nf1 == nf2)
                                {
                                    nf = nf1;
                                    NewTMR.Nounframes.Add(nf);
                                }
                            }
                        }
                    }
                    if (nf != null)
                    {
                        if (ML.MainNounFrames_VerbFrames[nf][vf.Concept].Count > 1)
                        {
                            foreach (VerbFrame vf2 in ML.MainNounFrames_VerbFrames[nf][vf.Concept])
                            {
                                NewTMR.VerbFrames.Add(vf2);
                            }
                        }
                        else
                        {
                            NewTMR.VerbFrames.Add(vf);
                        }
                    }

                }

                foreach (VerbFrame vf in NewTMR.VerbFrames)
                {
                    foreach (CaseRole cr in vf.CaseRoles.Keys)
                    {
                        foreach (NounFrame nf in vf.CaseRoles[cr])
                        {
                            if (NewTMR.Nounframes.Contains(nf) == false)
                                NewTMR.Nounframes.Add(nf);
                        }
                    }
                }

                FrmNewTMR newForm = new FrmNewTMR(NewTMR, this.OriginalTMR, ML);
                newForm.Text = f.Concept.Name;
                newForm.ShowDialog();
            }
        
        }

    }
}

