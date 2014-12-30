using System;
using System.Collections.Generic;
using System.Text;
using MindMapGenerator.Drawing_Management;
using System.Windows;
using System.Windows.Forms;
using mmTMR;
using System.Drawing;
using SyntacticAnalyzer;
using GraphTest;
using WordsMatching;
using Google.API.Search;
using ViewingManeger;
using MindMapMeaningRepresentation;
using MindMapViewingManagement;
using System.Threading;

namespace MindMapViewingManagement
{
    [Serializable]
    public class GoogleImageSearchSettings
    {
        public int Count { get; set; }
        public ImageSize ImSize { get; set; }
        public Colorization Coloriztion { get; set; }
        public ImageType Imtype { get; set; }
        public string FileTypeStr { get; set; }
        public GoogleImageSearchSettings(int count, ImageSize imSize, Colorization colorization, ImageType imType, string filetype)
        {
            Count = count;
            ImSize = imSize;
            Coloriztion = colorization;
            Imtype = imType;
            FileTypeStr = filetype;
        }

        public GoogleImageSearchSettings(int count, ImageSize imSize, Colorization colorization, ImageType imType)
            : this(count, imSize, colorization, imType, "jpg")
        {

        }


        public GoogleImageSearchSettings(string imSize, string colorization, string imType, string filetype)
            : this(1, imSize, colorization, imType, filetype)
        {
        }

        public GoogleImageSearchSettings(string imSize, string colorization, string imType)
            : this(1, imSize, colorization, imType, "jpg")
        {

        }
        public GoogleImageSearchSettings()
            : this("Small", "All", "All", "jpg")
        { }
    }




    public enum DrawingSizeMode
    {
        Normal,
        AutoSize
    }

    public enum ConceptCmbination
    {
        None,
        Combined2

    }
    [Serializable]
    public class MMViewManeger : DrawingManager
    {

        public MindMapTMR _TMR;
        private ConceptCmbination _conceptCombination;

        public ConceptCmbination ConceptCombination
        {
            get { return _conceptCombination; }
            set { _conceptCombination = value; }
        }
        public bool ShowEdgeText
        {
            get;
            set;
        }

        NewGoogleSearch GImSearch { get; set; }
        Dictionary<VerbFrame, VerbFrameEntity> _dicVerbFrame = new Dictionary<VerbFrame, VerbFrameEntity>();
        Dictionary<NounFrame, NounFrameEntity> _dicNounFrame = new Dictionary<NounFrame, NounFrameEntity>();
        //Dictionary<VerbFrame, MM_RectangleWithText> _dicVerbFrameTimeEntity = new Dictionary<VerbFrame, MM_RectangleWithText>();
        List<Node> _nodes = new List<Node>();
        List<MM_RectangleWithText> otherEntities = new List<MM_RectangleWithText>();
        List<GraphTest.Edge> _edges = new List<GraphTest.Edge>();
        SpringModel _spModel;
        private Size imageSize = new Size(150, 150);
        public PictureBox picbox = new PictureBox();

        public int getMaximumY()
        {
            int MaximumY = 0;
            foreach (VerbFrame vf in _dicVerbFrame.Keys)
            {
                if (_dicVerbFrame[vf].Position.Y > MaximumY)
                {
                    MaximumY = Convert.ToInt32(_dicVerbFrame[vf].Position.Y);
                }
            }

            foreach (NounFrame nf in _dicNounFrame.Keys)
            {
                if (_dicNounFrame[nf].Position.Y > MaximumY)
                {
                    MaximumY = Convert.ToInt32(_dicNounFrame[nf].Position.Y);
                }
            }

            return MaximumY;
        }

        public int getMaximumX()
        {
            int MaximumX = 0;
            foreach (VerbFrame vf in _dicVerbFrame.Keys)
            {
                if (_dicVerbFrame[vf].Position.X > MaximumX)
                {
                    MaximumX = Convert.ToInt32(_dicVerbFrame[vf].Position.X);
                }
            }

            foreach (NounFrame nf in _dicNounFrame.Keys)
            {
                if (_dicNounFrame[nf].Position.X > MaximumX)
                {
                    MaximumX = Convert.ToInt32(_dicNounFrame[nf].Position.X);
                }
            }

            return MaximumX;
        }

        public Frame getDoubleClickedFrame(int X, int Y)
        {
            Frame result = null;
            Point p = new Point(X, Y);

            foreach (VerbFrame vf in _dicVerbFrame.Keys)
            {
                if (_dicVerbFrame[vf].InArea(p) == true)
                {
                    result = vf;
                    break;
                }
            }

            foreach (NounFrame nf in _dicNounFrame.Keys)
            {
                if (_dicNounFrame[nf].InArea(p) == true)
                {
                    result = nf;
                    break;
                }
            }
            return result;
        }

        private void AddLinks()
        {
            foreach (VerbFrame VF in _TMR.VerbFrames)
            {
                VerbFrameEntity VF_entity = _dicVerbFrame[VF];
                foreach (CaseRole cr in VF.CaseRoles.Keys)
                {
                    List<NounFrame> nfs = VF.CaseRoles[cr];
                    foreach (NounFrame NF in nfs)
                    {
                        NounFrameEntity nfe = _dicNounFrame[NF];
                        Add(new MM_LineWithText(VF_entity, nfe, cr.ToString()));
                        //Add(new MM_Line(VF_entity, nfe));

                    }
                }
            }
            foreach (VerbFrame VF in _TMR.VerbFrames)
            {
                VerbFrameEntity VF_entity = _dicVerbFrame[VF];
                foreach (DomainRelationType drt in VF.DomainRelations.Keys)
                {
                    List<VerbFrame> vfl = VF.DomainRelations[drt];
                    foreach (VerbFrame vf in vfl)
                    {
                        VerbFrameEntity vfe = _dicVerbFrame[vf];
                        Add(new MM_LineWithText(VF_entity, vfe, drt.ToString()));
                        //Add(new MM_Line(VF_entity, vfe));
                    }
                }

                foreach (DomainRelationType drt in VF.DomainRelations_n.Keys)
                {
                    List<NounFrame> nfl = VF.DomainRelations_n[drt];
                    foreach (NounFrame nf in nfl)
                    {
                        NounFrameEntity nfe = _dicNounFrame[nf];
                        Add(new MM_LineWithText(VF_entity, nfe, drt.ToString()));
                        //Add(new MM_Line(VF_entity, nfe));
                    }
                }

                foreach (TemporalRelationType trt in VF.TemporalRelations.Keys)
                {
                    List<VerbFrame> vfl = VF.TemporalRelations[trt];
                    foreach (VerbFrame vf in vfl)
                    {
                        VerbFrameEntity vfe = _dicVerbFrame[vf];
                        Add(new MM_LineWithText(VF_entity, vfe, trt.ToString()));
                        //Add(new MM_Line(VF_entity, vfe));
                    }


                }
                foreach (TemporalRelationType trt in VF.TemporalRelations_n.Keys)
                {
                    List<NounFrame> nfl = VF.TemporalRelations_n[trt];
                    foreach (NounFrame nf in nfl)
                    {
                        NounFrameEntity nfe = _dicNounFrame[nf];
                        Add(new MM_LineWithText(VF_entity, nfe, trt.ToString()));
                        //Add(new MM_Line(VF_entity, nfe));
                    }


                }
                //aspects
                if (VF.Aspect.Duration.ActionTime != null)
                {
                    string time = VF.Aspect.Duration.ActionTime;
                    VerbFrameEntity vfe = _dicVerbFrame[VF];
                    MM_RectangleWithText timeEntity = new MM_RectangleWithText(0, 0, 30, 25, time, "");
                    Add(timeEntity);
                    Add(new MM_LineWithText(VF_entity, timeEntity, "time"));
                    //Add(new MM_Line(VF_entity, timeEntity));
                }

            }
        }
        public void UpdateRelations()
        {
            this.ClearLinks();
            AddLinks();
        }
        public DrawingSizeMode SizeMode;
        public bool initOnly;

        public void LoadLocationFrom(List<PointF> positions)
        {
            for (int i = 0; i < this.Entities.Count; i++)
            {
                this.Entities[i].Position = positions[i];
            }
            this.Control.Invalidate();
 
        }
        public MMViewManeger(Control cntrl, MindMapTMR TMR, bool showedgeText, GoogleImageSearchSettings gImSearchSettings, DrawingSizeMode sizeMode)
            : base(cntrl)
        {
            _conceptCombination = ConceptCmbination.None;
            SizeMode = sizeMode;
            GImSearch = new NewGoogleSearch(gImSearchSettings);
            _TMR = TMR;
            ShowEdgeText = showedgeText;
            FilterMeaningRepresentationForDrawing();
            BuildAutomaticLayoutedGraph(true);

            //4each 3al noun frames w n create entities w n defha w 2a3redha 
            //verb frames 
        }
        public MMViewManeger(Control cntrl, MindMapTMR TMR, bool showedgeText, GoogleImageSearchSettings gImSearchSettings)
            : this(cntrl, TMR, showedgeText, gImSearchSettings, DrawingSizeMode.Normal)
        {

        }
        public MMViewManeger(Control cntrl, MindMapTMR TMR, bool showedgeText)
            : this(cntrl, TMR, showedgeText, new MindMapViewingManagement.GoogleImageSearchSettings(), DrawingSizeMode.Normal)
        {

        }


        void FilterMeaningRepresentationForDrawing()
        {
            for (int i = 0; i < _TMR.Nounframes.Count; i++)
            {
                NounFrame nf = _TMR.Nounframes[i];
                if (nf.Ownerof.ContainsKey(CaseRole.Possession))
                {
                    List<NounFrame> Pnfs = nf.Ownerof[CaseRole.Possession];
                    for (int j = 0; j < Pnfs.Count; j++)
                    {
                        NounFrame nfj = Pnfs[j];
                        if (_TMR.HasNoRelationsExceptFrom(Pnfs[j], nf))
                        {
                            MyWordInfo wi = new MyWordInfo();
                            wi.Word = nfj.Text;
                            wi.Sense = nfj.ParseNode.SenseNo;
                            //TODO : assign part of speach here
                            //wi.Pos = 
                            bool found = false;
                            foreach (MyWordInfo winfo in nf.AdjectivesInfo)
                            {
                                if (wi.Word == winfo.Word )
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if(!found)
                                nf.AdjectivesInfo.Add(wi);
                            int index = _TMR.Nounframes.IndexOf(nfj);
                           
                            Pnfs.RemoveAt(j);
                            j--;
                            if (index != -1)
                            {
                                _TMR.Nounframes.RemoveAt(index);
                                if (index < i)
                                    i--;
                            }
                            
                         

                        }

                    }
                }

            }

        }
        public void AddNounFrameEntity(NounFrame nounFrame, Point point)
        {
            NounFrameEntity nfe = new NounFrameEntity(point.X, point.Y, nounFrame,_TMR, GImSearch, SizeMode,_conceptCombination);
            this.Add(nfe);
            _dicNounFrame.Add(nounFrame, nfe);
            Control.Invalidate();
        }

        public void AddVerbFrameEntity(VerbFrame verbFrame, Point point)
        {
            VerbFrameEntity VF_entity = new VerbFrameEntity(point.X, point.Y, verbFrame, _TMR, GImSearch, SizeMode);
            Add(VF_entity);
            _dicVerbFrame.Add(verbFrame, VF_entity);
            Control.Invalidate();
        }
        public void BuildAutomaticLayoutedGraph()
        {
            BuildAutomaticLayoutedGraph(false);
        }
        public void BuildAutomaticLayoutedGraph(bool initOnly)
        {
            ClearData();
            int index = 0;
            foreach (NounFrame NF in _TMR.Nounframes)
            {
                NounFrameEntity nfe = new NounFrameEntity(0, 0, NF, _TMR, GImSearch, SizeMode,_conceptCombination);
                this.Add(nfe);
                _dicNounFrame.Add(NF, nfe);
                _nodes.Add(new Node(0, 0, index));
                index++;
            }

            foreach (NounFrame NF in _TMR.Nounframes)
            {
                foreach (CaseRole cr in NF.Ownerof.Keys)
                {
                    foreach (NounFrame NF2 in NF.Ownerof[cr])
                    {
                        if (_dicNounFrame.ContainsKey(NF2) == true)
                        {
                            NounFrameEntity nfe2;
                            nfe2 = _dicNounFrame[NF2];
                            if (ShowEdgeText)
                                Add(new MM_LineWithText(_dicNounFrame[NF], nfe2, cr.ToString()));
                            else
                                Add(new MM_LineWithText(_dicNounFrame[NF], nfe2, ""));
                            _edges.Add(new GraphTest.Edge(_nodes[this._TMR.Nounframes.IndexOf(NF)], _nodes[this._TMR.Nounframes.IndexOf(NF2)]));
                            _nodes[this._TMR.Nounframes.IndexOf(NF)].Neighbours.Add(_nodes[this._TMR.Nounframes.IndexOf(NF2)]);
                            _nodes[this._TMR.Nounframes.IndexOf(NF2)].Neighbours.Add(_nodes[this._TMR.Nounframes.IndexOf(NF)]);
                        }
                    }
                }
            }

            foreach (VerbFrame VF in _TMR.VerbFrames)
            {


                VerbFrameEntity VF_entity = new VerbFrameEntity(0, 0, VF, _TMR, GImSearch, SizeMode);
                Add(VF_entity);
                _dicVerbFrame.Add(VF, VF_entity);
                _nodes.Add(new Node(0, 0, index));
                //if (VF.Aspect.Duration.ActionTime != null)
                //{
                //    index++;
                //    _nodes.Add(new Node(0, 0, index));
                //}


                foreach (CaseRole cr in VF.CaseRoles.Keys)
                {
                   
                    List<NounFrame> nfs = VF.CaseRoles[cr];
                    foreach (NounFrame NF in nfs)
                    {
                        if (_dicNounFrame.ContainsKey(NF) == true)
                        {
                            NounFrameEntity nfe = _dicNounFrame[NF];
                            if (cr == CaseRole.Agent)
                            {
                                if (ShowEdgeText)
                                    Add(new MM_LineWithText(nfe, VF_entity, cr.ToString()));
                                else
                                    Add(new MM_LineWithText(nfe,VF_entity, ""));
                            }
                            else
                            {
                                if (ShowEdgeText)
                                    Add(new MM_LineWithText(VF_entity, nfe, cr.ToString()));
                                else
                                    Add(new MM_LineWithText(VF_entity, nfe, ""));
                            }
                            //Add(new MM_Line(VF_entity, nfe));
                            _edges.Add(new GraphTest.Edge(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count], _nodes[this._TMR.Nounframes.IndexOf(NF)]));
                            _nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[this._TMR.Nounframes.IndexOf(NF)]);
                            _nodes[this._TMR.Nounframes.IndexOf(NF)].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count]);
                        }
                    }
                }
                index++;
            }
            foreach (VerbFrame VF in _TMR.VerbFrames)
            {
                VerbFrameEntity VF_entity = _dicVerbFrame[VF];
                foreach (DomainRelationType drt in VF.DomainRelations.Keys)
                {
                    List<VerbFrame> vfl = VF.DomainRelations[drt];
                    foreach (VerbFrame vf in vfl)
                    {
                        if (_dicVerbFrame.ContainsKey(vf) == true)
                        {
                            VerbFrameEntity vfe = _dicVerbFrame[vf];
                            if (ShowEdgeText)
                                Add(new MM_LineWithText(VF_entity, vfe, drt.ToString()));
                            else
                                Add(new MM_LineWithText(VF_entity, vfe, ""));
                            //Add(new MM_Line(VF_entity, vfe));
                            _edges.Add(new GraphTest.Edge(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count], _nodes[this._TMR.VerbFrames.IndexOf(vf) + this._TMR.Nounframes.Count]));
                            _nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(vf) + this._TMR.Nounframes.Count]);
                            _nodes[this._TMR.VerbFrames.IndexOf(vf) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count]);
                        }
                    }
                }

                foreach (DomainRelationType drt in VF.DomainRelations_n.Keys)
                {
                    List<NounFrame> nfl = VF.DomainRelations_n[drt];
                    foreach (NounFrame nf in nfl)
                    {
                        if (_dicNounFrame.ContainsKey(nf) == true)
                        {
                            NounFrameEntity nfe = _dicNounFrame[nf];
                            if (ShowEdgeText)
                                Add(new MM_LineWithText(VF_entity, nfe, drt.ToString()));
                            else
                                Add(new MM_LineWithText(VF_entity, nfe, ""));

                            //Add(new MM_Line(VF_entity, nfe));
                            _edges.Add(new GraphTest.Edge(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count], _nodes[this._TMR.Nounframes.IndexOf(nf)]));
                            _nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[this._TMR.Nounframes.IndexOf(nf)]);
                            _nodes[this._TMR.Nounframes.IndexOf(nf)].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count]);
                        }
                    }
                }

                foreach (TemporalRelationType trt in VF.TemporalRelations.Keys)
                {
                    List<VerbFrame> vfl = VF.TemporalRelations[trt];
                    foreach (VerbFrame vf in vfl)
                    {
                        if (_dicVerbFrame.ContainsKey(vf) == true)
                        {
                            VerbFrameEntity vfe = _dicVerbFrame[vf];
                            if (ShowEdgeText)
                                Add(new MM_LineWithText(VF_entity, vfe, trt.ToString()));
                            else
                                Add(new MM_LineWithText(VF_entity, vfe, ""));

                            //Add(new MM_Line(VF_entity, vfe));
                            _edges.Add(new GraphTest.Edge(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count], _nodes[this._TMR.VerbFrames.IndexOf(vf) + this._TMR.Nounframes.Count]));
                            _nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(vf) + this._TMR.Nounframes.Count]);
                            _nodes[this._TMR.VerbFrames.IndexOf(vf) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count]);
                        }
                    }
                }

                foreach (TemporalRelationType trt in VF.TemporalRelations_n.Keys)
                {
                    List<NounFrame> nfl = VF.TemporalRelations_n[trt];
                    foreach (NounFrame nf in nfl)
                    {
                        if (_dicNounFrame.ContainsKey(nf) == true)
                        {
                            NounFrameEntity nfe = _dicNounFrame[nf];
                            if (ShowEdgeText)
                                Add(new MM_LineWithText(VF_entity, nfe, trt.ToString()));
                            else
                                Add(new MM_LineWithText(VF_entity, nfe, ""));

                            //Add(new MM_Line(VF_entity, nfe));
                            _edges.Add(new GraphTest.Edge(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count], _nodes[this._TMR.Nounframes.IndexOf(nf)]));
                            _nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[this._TMR.Nounframes.IndexOf(nf)]);
                            _nodes[this._TMR.Nounframes.IndexOf(nf)].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count]);
                        }
                    }
                }
                //aspects
                if (VF.Aspect.Duration.ActionTime != null)
                {
                    string time = VF.Aspect.Duration.ActionTime;
                    VerbFrameEntity vfe = _dicVerbFrame[VF];
                    MM_RectangleWithText timeEntity = new MM_RectangleWithText(0, 0, 30, 25, time, "");
                    Add(timeEntity);
                    otherEntities.Add(timeEntity);
                    _nodes.Add(new Node(0, 0, _nodes.Count));
                    if (ShowEdgeText)
                        Add(new MM_LineWithText(VF_entity, timeEntity, "Time"));
                    else
                        Add(new MM_LineWithText(VF_entity, timeEntity, ""));

                    //Add(new MM_Line(VF_entity, timeEntity));
                    _edges.Add(new GraphTest.Edge(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count], _nodes[_nodes.Count - 1]));
                    _nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[_nodes.Count - 1]);
                    _nodes[_nodes.Count - 1].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count]);

                }

                //adverbs
                //if (VF.AdverbsInfo != null)
                //{
                //    foreach (MyWordInfo mwi in VF.AdverbsInfo)
                //    {
                //        MM_RectangleWithText entity = new MM_RectangleWithText(0, 0, 30, 25, mwi.Word, "");
                //        Add(entity);
                //        otherEntities.Add(entity);
                //        _nodes.Add(new Node(0, 0, _nodes.Count));
                //        Add(new MM_LineWithText(VF_entity, entity, "Adverb"));
                //        _edges.Add(new GraphTest.Edge(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count], _nodes[_nodes.Count - 1]));
                //        _nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count].Neighbours.Add(_nodes[_nodes.Count - 1]);
                //        _nodes[_nodes.Count - 1].Neighbours.Add(_nodes[this._TMR.VerbFrames.IndexOf(VF) + this._TMR.Nounframes.Count]);

                //    }
                //}

                //adjectives

                //foreach (NounFrame nounframe in _TMR.Nounframes)
                //{
                //    if (nounframe.AdjectivesInfo != null)
                //    {
                //        foreach (MyWordInfo mwi in nounframe.AdjectivesInfo)
                //        {
                //            MM_RectangleWithText entity = new MM_RectangleWithText(0, 0, 30, 25, mwi.Word, "");
                //            Add(entity);
                //            otherEntities.Add(entity);
                //            _nodes.Add(new Node(0, 0, _nodes.Count));
                //            Add(new MM_LineWithText(VF_entity, entity, "Adjective"));
                //            _edges.Add(new GraphTest.Edge(_nodes[this._TMR.Nounframes.IndexOf(nounframe)], _nodes[_nodes.Count - 1]));
                //            _nodes[this._TMR.Nounframes.IndexOf(nounframe)].Neighbours.Add(_nodes[_nodes.Count - 1]);
                //            _nodes[_nodes.Count - 1].Neighbours.Add(_nodes[this._TMR.Nounframes.IndexOf(nounframe)]);   
                //        }                       
                //    }
                //}
            }


            _spModel = new SpringModel(_nodes, _edges, 800, 200, Mode.CIRCULAR);
            //this._spModel._getOriginDistance = new GetOriginDistance(getOriginDistance);
            this._spModel.StepDoneEvent+=new SingleStepUpdateEventHandler(_spModel_StepDoneEvent);
            if (LayoutiingThread != null)
                if (LayoutiingThread.ThreadState != ThreadState.Stopped)
                    LayoutiingThread.Abort();
            if (initOnly)
            {
                _spModel.InitializeSpringModel();
                _spModel.AdjustNodes(new Rectangle(0, 0, 800, 600));
                _spModel_StepDoneEvent();
            }
            else
            {
                LayoutiingThread = new System.Threading.Thread(new System.Threading.ThreadStart(_spModel.Run));
                LayoutiingThread.Start();
            }
            
            
        }

        private void ClearData()
        {
            this._nodes.Clear();
            this._edges.Clear();
            this._dicVerbFrame.Clear();
            this._dicNounFrame.Clear();
            this.otherEntities.Clear();
            this.Entities.Clear();
            this.Links.Clear();
        }
        public Thread LayoutiingThread;

        void _spModel_StepDoneEvent()
        {
            int i = 0;
            foreach (NounFrame NF in this._TMR.Nounframes)
            {
                NounFrameEntity nfe = this._dicNounFrame[NF];
                nfe.Position = new PointF((float)_nodes[i].Xposition, (float)_nodes[i].Yposition);
                i++;
            }
            foreach (VerbFrame VF in this._TMR.VerbFrames)
            {
                VerbFrameEntity vfe = this._dicVerbFrame[VF];
                vfe.Position = new PointF((float)_nodes[i].Xposition, (float)_nodes[i].Yposition);
                //if (VF.Aspect.Duration.ActionTime != null)
                //{
                //    i++;
                //    MM_RectangleWithText timeEntity = _dicVerbFrameTimeEntity[VF];
                //    timeEntity.Position = new PointF((float)_nodes[i].Xposition, (float)_nodes[i].Yposition);
                //}
                i++;
            }
            foreach (MM_RectangleWithText Entity in otherEntities)
            {
                Entity.Position = new PointF((float)_nodes[i].Xposition, (float)_nodes[i].Yposition);
                i++;
            }
            this.Control.Invalidate();
        }

        public double getOriginDistance(int i, int j)
        {
            double result = 0;
            if (i < this._TMR.Nounframes.Count)
            {
                result += (Math.Sqrt(Math.Pow(_dicNounFrame[this._TMR.Nounframes[i]].Rectangle.Height, 2) + Math.Pow(_dicNounFrame[this._TMR.Nounframes[i]].Rectangle.Width, 2))) / 2;
            }
            else if (i < this._TMR.VerbFrames.Count)
            {
                result += (Math.Sqrt(Math.Pow(_dicVerbFrame[this._TMR.VerbFrames[i]].Rectangle.Height, 2) + Math.Pow(_dicVerbFrame[this._TMR.VerbFrames[i]].Rectangle.Width, 2))) / 2;
            }
            else
            {
                result += 19.5;
            }
            if (j < this._TMR.Nounframes.Count)
            {
                result += (Math.Sqrt(Math.Pow(_dicNounFrame[this._TMR.Nounframes[j]].Rectangle.Height, 2) + Math.Pow(_dicNounFrame[this._TMR.Nounframes[j]].Rectangle.Width, 2))) / 2;
            }
            else if (j < this._TMR.VerbFrames.Count)
            {
                result += (Math.Sqrt(Math.Pow(_dicVerbFrame[this._TMR.VerbFrames[j]].Rectangle.Height, 2) + Math.Pow(_dicVerbFrame[this._TMR.VerbFrames[j]].Rectangle.Width, 2))) / 2;
            }
            else
            {
                result += 19.5;
            }
            return result;
        }



        public void AssignLayoutFrom(MMViewManeger mMViewManeger)
        {
            for (int i = 0; i < mMViewManeger.Entities.Count; i++)
            {
                Entities[i].Position = mMViewManeger.Entities[i].Position;
            }
            this.Control.Invalidate();
        }
    }

}