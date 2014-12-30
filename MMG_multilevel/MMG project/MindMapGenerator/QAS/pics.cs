using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MindMapGenerator.Drawing_Management;
using MindMapViewingManagement;
using MindMapMeaningRepresentation;
using DiscourseAnalysis;
using SyntacticAnalyzer;
using mmTMR;
using GoogleImageDrawingEntity;
using OntologyLibrary.OntoSem;

namespace MMG
{
    
    public partial class pics : Form
    {
        public MMViewManeger viewer;
        public MindMapTMR CurrentTMR;
        MindMapTMR OriginalTMR;
        MultilevelGenerator.MultiLevel ML;
        List<pics> picForms;
        public string descText = "";

        int _level;
        GoogleImageSearchSettings _settings;
        DrawingSizeMode _DrawSizeMode;
        public pics(MindMapTMR CurrentTMR, MindMapTMR OriginalTMR, MultilevelGenerator.MultiLevel ML, int level, List<pics> PicForms):this(CurrentTMR,OriginalTMR, ML,level,PicForms,new GoogleImageSearchSettings(), DrawingSizeMode.Normal)
        {
 
        }
        public pics(MindMapTMR CurrentTMR, MindMapTMR OriginalTMR, MultilevelGenerator.MultiLevel ML, int level, List<pics> PicForms, GoogleImageSearchSettings settings, DrawingSizeMode drawSizeMode)
        {
            _level = level;
            _settings = settings;
            _DrawSizeMode = drawSizeMode;
            PicForms.Add(this);
            picForms = PicForms;
            InitializeComponent();
            this.CurrentTMR = CurrentTMR;
            this.OriginalTMR = OriginalTMR;
            this.ML = ML;
            this.Height = 700;
            this.Width = 1000;
            this.Text = "Mind Map";

            viewer = new MMViewManeger(this.panel1, this.CurrentTMR,false,settings,_DrawSizeMode);
            viewer.Control = this.panel1;
            int MaximumY = viewer.getMaximumY();
            int MaximumX = viewer.getMaximumX();

            this.panel1.Height = MaximumY + 40;
            this.panel1.Width = MaximumX + 40;

            this.AutoScroll = true;
           
        }
        

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Frame f = viewer.getDoubleClickedFrame(e.X, e.Y);

            //try
            {
                HandleExpansion(f, picForms);
                int frameIndex;
                if (f is NounFrame)
                {
                    frameIndex = CurrentTMR.Nounframes.IndexOf((NounFrame)f);
                    ((MultiLevelMMapForm)this.MdiParent).HandleOtherExpandions(Frame.Type.Noun, frameIndex, picForms.IndexOf(this));

                }
                else
                {
                    frameIndex = CurrentTMR.VerbFrames.IndexOf((VerbFrame)f);
                    ((MultiLevelMMapForm)this.MdiParent).HandleOtherExpandions(Frame.Type.Verb, frameIndex, picForms.IndexOf(this));

                }
            }
            //catch (Exception E)
           //{
           //     MessageBox.Show(E.Message);
           // }

           
        
        }

        public void HandleExpansion(Frame f, List<pics> Picforms)
        {
            bool Flag = false;
            if (f != null)
            {

                MindMapTMR NewTMR = new MindMapTMR();

                if (f is NounFrame)
                {
                    List<Frame> Frames = new List<Frame>();
                    Frames.Add(f);


                    foreach (Frame frame in this.ML.VerbFrames_NounFrames.Keys)
                    {
                        foreach (Dictionary<OurMindMapOntology.MindMapConcept, List<Frame>> dictionary in this.ML.VerbFrames_NounFrames[frame])
                        {
                            foreach (OurMindMapOntology.MindMapConcept c in dictionary.Keys)
                            {
                                if (c == f.Concept)
                                {
                                    Frames.Clear();
                                    Frames = dictionary[c];
                                    if (Frames.Count > 1)
                                        Flag = true;
                                }
                            }
                        }
                    }

                    foreach (Frame _f in Frames)
                    {
                        NounFrame nounframe = (NounFrame)_f;
                        if (NewTMR.Nounframes.Contains(nounframe) == false)
                            NewTMR.Nounframes.Add(nounframe);
                        Dictionary<CaseRole, List<VerbFrame>> AssociatedActions = this.OriginalTMR.GetNounFrameAssociatedactions(this.OriginalTMR.Nounframes.IndexOf((NounFrame)_f));
                        foreach (CaseRole cr in AssociatedActions.Keys)
                        {
                            foreach (VerbFrame vf in AssociatedActions[cr])
                            {
                                if (NewTMR.VerbFrames.Contains(vf) == false)
                                    NewTMR.VerbFrames.Add(vf);
                            }
                        }

                        foreach (CaseRole cr in nounframe.Ownerof.Keys)
                        {
                            for (int i = 0; i < nounframe.Ownerof[cr].Count; i++)
                            {
                                if (NewTMR.Nounframes.Contains(nounframe.Ownerof[cr][i]) == false)
                                {
                                    NewTMR.Nounframes.Add(nounframe.Ownerof[cr][i]);
                                }
                            }
                        }

                        foreach (NounFrame nf in this.OriginalTMR.Nounframes)
                        {
                            foreach (CaseRole cr in nf.Ownerof.Keys)
                            {
                                foreach (NounFrame nf2 in nf.Ownerof[cr])
                                {
                                    if (nf2 == nounframe && NewTMR.Nounframes.Contains(nf) == false)
                                        NewTMR.Nounframes.Add(nf);
                                }
                            }
                        }
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
                            Flag = true;
                            foreach (VerbFrame vf2 in ML.MainNounFrames_VerbFrames[nf][vf.Concept])
                            {
                                NewTMR.VerbFrames.Add(vf2);
                                //NewTMR.VerbFrames.Add((VerbFrame)ML.NewFrame_OriginalFrame[(Frame)vf2]);
                            }
                        }
                        else
                        {
                            NewTMR.VerbFrames.Add((VerbFrame)ML.NewFrame_OriginalFrame[(Frame)vf]);
                        }
                    }

                }

                for (int k = 0; k < NewTMR.VerbFrames.Count;k++)
                {
                    VerbFrame vf = NewTMR.VerbFrames[k];
                    foreach (DomainRelationType drt in vf.DomainRelations.Keys)
                    {
                        for (int i = 0; i < vf.DomainRelations[drt].Count; i++)
                        {
                            if (NewTMR.VerbFrames.Contains(vf.DomainRelations[drt][i]) == false)
                            {
                                NewTMR.VerbFrames.Add(vf.DomainRelations[drt][i]);
                            }
                        }
                    }
                }

                for (int k = 0; k < NewTMR.VerbFrames.Count; k++)
                {
                    VerbFrame vf = NewTMR.VerbFrames[k];
                    foreach (TemporalRelationType trt in vf.TemporalRelations.Keys)
                    {
                        for (int i = 0; i < vf.TemporalRelations[trt].Count; i++)
                        {
                            if (NewTMR.VerbFrames.Contains(vf.TemporalRelations[trt][i]) == false)
                            {
                                NewTMR.VerbFrames.Add(vf.TemporalRelations[trt][i]);
                            }
                        }
                    }
                }

                foreach (VerbFrame vf in NewTMR.VerbFrames)
                {
                    foreach (DomainRelationType drt in vf.DomainRelations_n.Keys)
                    {
                        for (int i = 0; i < vf.DomainRelations_n[drt].Count; i++)
                        {
                            if (NewTMR.Nounframes.Contains(vf.DomainRelations_n[drt][i]) == false)
                            {
                                NewTMR.Nounframes.Add(vf.DomainRelations_n[drt][i]);
                            }
                        }
                    }
                }

                foreach (VerbFrame vf in NewTMR.VerbFrames)
                {
                    foreach (TemporalRelationType trt in vf.TemporalRelations_n.Keys)
                    {
                        for (int i = 0; i < vf.TemporalRelations_n[trt].Count; i++)
                        {
                            if (NewTMR.Nounframes.Contains(vf.TemporalRelations_n[trt][i]) == false)
                            {
                                NewTMR.Nounframes.Add(vf.TemporalRelations_n[trt][i]);
                            }
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

                List<NounFrame> nfs = new List<NounFrame>();
                if (Flag == false)
                {
                    foreach (NounFrame nf in NewTMR.Nounframes)
                    {
                        foreach (CaseRole cr in nf.Ownerof.Keys)
                        {
                            foreach (NounFrame nf2 in nf.Ownerof[cr])
                            {
                                if (NewTMR.Nounframes.Contains(nf2) == false)
                                {
                                    nfs.Add(nf2);
                                }
                            }
                        }
                    }
                }

                foreach (NounFrame nf in nfs)
                {
                    if (NewTMR.Nounframes.Contains(nf) == false)
                        NewTMR.Nounframes.Add(nf);
                }
                pics newForm;

                if (Flag == true)
                {
                    newForm = new pics(NewTMR, this.OriginalTMR, ML, _level + 1, Picforms, _settings, _DrawSizeMode);
                }
                else
                {
                    if (NewTMR.VerbFrames.Count != 0 || NewTMR.VerbFrames.Count != 0)
                    {
                        MultilevelGenerator.MultiLevel NewML = new MultilevelGenerator.MultiLevel(NewTMR);
                        MindMapTMR NewNewTMR = NewML.Run();
                        newForm = new pics(NewNewTMR, this.OriginalTMR, NewML, _level + 1, Picforms, _settings, _DrawSizeMode);

                    }
                    else
                        return;
                }
                if (f.Concept != null)
                    newForm.descText = f.Concept.Text;
                else if (f is NounFrame)
                    newForm.descText = ((NounFrame)f).Text;
                else
                    newForm.descText = ((VerbFrame)f).VerbName;


                newForm.Text = this.Text;
                newForm.MdiParent = this.MdiParent;
                newForm.Show();
            }
        }

        private void pics_Load(object sender, EventArgs e)
        {
            label1.Text = "Level " + _level + (descText==""? "": "( "+descText+" )");
            if (_level == 0)
            {
                for (int i = 0; i < CurrentTMR.VerbFrames.Count; i++)
                {
                    HandleExpansion(CurrentTMR.VerbFrames[i], picForms);
                }
            }
            else if (_level == 1)
            {
                
            }
        }

        
    }
     
}