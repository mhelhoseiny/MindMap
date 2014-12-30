using System;
using System.Collections.Generic;
using System.Text;
using MindMapGenerator.Drawing_Management;
using mmTMR;
using GoogleImageDrawingEntity;
using System.Drawing;
using SyntacticAnalyzer;
using System.Windows.Forms;
using WordsMatching;
using Google.API.Search;
using ViewingManeger;
using MindMapMeaningRepresentation;


namespace MindMapViewingManagement
{
    public static class FrameExtensions
    {
        public static List<Frame> GetNeighbors(this NounFrame nf,MindMapTMR tmr )
        {
            List<Frame> frames = new List<Frame>();
            Dictionary<CaseRole, List<VerbFrame>> Associatedactions = tmr.GetNounFrameAssociatedactions(tmr.Nounframes.IndexOf(nf));
            //Array caseRoleArr = Enum.GetValues(typeof(CaseRole));

            int len = Enum.GetNames(typeof(CaseRole)).Length;

            for (int i = 0; i < Enum.GetNames(typeof(CaseRole)).Length; i++)
            {
                //Enum.GetValues(CaseRole)
                if (Associatedactions.ContainsKey((CaseRole)i))
                {
                    List<VerbFrame> ListofVF = Associatedactions[(CaseRole)i];
                    foreach(VerbFrame vf in ListofVF)
                        frames.Add(vf);
                }
                if (nf.Ownerof.ContainsKey((CaseRole)i))
                {
                    foreach (NounFrame nfi in nf.Ownerof[(CaseRole)i])
                        frames.Add(nfi);
                }
            }
            
            return frames;

        }

        public static List<Frame> GetNeighbors(this VerbFrame vf, MindMapTMR tmr)
        {
            List<Frame> frames = new List<Frame>();
            int verbFrameIndex = tmr.VerbFrames.IndexOf(vf);
            double weight = 0;

            foreach (CaseRole cr in tmr.VerbFrames[verbFrameIndex].CaseRoles.Keys)
            {
                List<NounFrame> nfs = tmr.VerbFrames[verbFrameIndex].CaseRoles[cr];
                foreach (NounFrame NF in nfs)
                {
                    frames.Add(NF);
                }
            }

            foreach (TemporalRelationType trt in tmr.VerbFrames[verbFrameIndex].TemporalRelations.Keys)
            {
                List<VerbFrame> vfs = tmr.VerbFrames[verbFrameIndex].TemporalRelations[trt];
                foreach (VerbFrame VF in vfs)
                {
                    //weight += temporalRelationWeights[trt];
                    frames.Add(VF);
                }
            }

            foreach (DomainRelationType drt in tmr.VerbFrames[verbFrameIndex].DomainRelations.Keys)
            {
                List<VerbFrame> vfs = tmr.VerbFrames[verbFrameIndex].DomainRelations[drt];
                foreach (VerbFrame VF in vfs)
                {
                    //weight += domainRelationWeights[drt];
                    frames.Add(VF);
                }
            }

            foreach (TemporalRelationType trt in tmr.VerbFrames[verbFrameIndex].TemporalRelations_n.Keys)
            {
                List<NounFrame> nfs = tmr.VerbFrames[verbFrameIndex].TemporalRelations_n[trt];
                foreach (NounFrame NF in nfs)
                {
                    frames.Add(NF);
                }
            }

            foreach (DomainRelationType drt in tmr.VerbFrames[verbFrameIndex].DomainRelations_n.Keys)
            {
                List<NounFrame> nfs = tmr.VerbFrames[verbFrameIndex].DomainRelations_n[drt];
                foreach (NounFrame NF in nfs)
                {
                    frames.Add(NF);
                }
            }
            return frames;
        }
    }

    [Serializable]
    public class NounFrameEntity : MM_RectangleWithText
    {
        NounFrame _nounFrame;
        Bitmap _bitmap = null;
        public PictureBox picbox = new PictureBox();
        NewGoogleSearch GoogleImSearch = new NewGoogleSearch();
        public DrawingSizeMode _SizeMode;
        MindMapTMR _tmr;
        ConceptCmbination _conceptComb;
        string _conceptCombText = "";
        public NounFrameEntity(int x, int y, NounFrame nounFrame, MindMapTMR tmr, NewGoogleSearch googleImSearch, DrawingSizeMode sizeMode, ConceptCmbination conceptCombination)
            : base(0, 0, 70, 40, "", "")
        {
            _tmr = tmr;
            _SizeMode = sizeMode;
            GoogleImSearch = googleImSearch;
            _conceptComb = conceptCombination;
            _nounFrame = nounFrame;
            string Text = "";

            //if (_nounFrame.Adjective != null)
            //{
            //    foreach (ParseNode adj in _nounFrame.Adjective)
            //        Text += (adj.Text + " ");

            //}

            if (_nounFrame.AdjectivesInfo != null)
            {

                foreach (MyWordInfo mwi in _nounFrame.AdjectivesInfo)
                {
                    Text += (mwi.Word + " ");

                }
            }

            Text += (_nounFrame.Text);

            _conceptCombText = GetConceptCombinationText();
            _nounFrame.SearchText1 = Text;


            //string _wordologyDirectoryPath = Application.ExecutablePath;
            //int index = _wordologyDirectoryPath.LastIndexOf("\\");
            //_wordologyDirectoryPath = _wordologyDirectoryPath.Substring(0, index);

            if (IsGoogleImage())
            {
                ///////////////// habal
                string strpath = "";
                //if (_nounFrame.Text == "agricultural".ToUpper())
                //    strpath = _wordologyDirectoryPath+@"\pics\agricultural.jpg";
                //else if (_nounFrame.Text == "queen".ToUpper())
                //    strpath = _wordologyDirectoryPath+@"\pics\queen.jpg";
                //else if (_nounFrame.Text == "shakespeare".ToUpper())
                //    strpath =_wordologyDirectoryPath+@"\pics\shakespeare.jpg";
                //else if (_nounFrame.Text == "living".ToUpper())
                //    strpath = _wordologyDirectoryPath+@"\pics\coins.jpg";
                //else if (_nounFrame.Text == "writer".ToUpper())
                //    strpath = _wordologyDirectoryPath+@"\pics\writer.jpg";

                ////////////////////////////

                if (strpath != "")
                    _bitmap = new Bitmap(strpath);
                else
                {
                    //_bitmap = GoogleSearch.GetImage(_nounFrame.SearchText1);


                    IList<IImageResult> Results;
                    if (sizeMode == DrawingSizeMode.Normal)
                    {
                        Results = GoogleImSearch.Search2(_nounFrame.SearchText1+" "+_conceptCombText);
                    }
                    else
                    {
                        ImageSize imsize = GetAutoSize();
                        Results = GoogleImSearch.Search2(_nounFrame.SearchText1+" "+_conceptCombText, imsize);

                    }
                    // if (Results.Count > 500000 && Results.Count < 15000000)
                    double R;
                    double.TryParse(_nounFrame.SearchText1, out R);

                    if (Results.Count >= 1 && R == 0)
                    {
                        GoogleImSearch.LoadImageFromUrl(Results[0].TbImage.Url, picbox);
                        _bitmap = new Bitmap(picbox.Image);
                        _rectangle = new Rectangle(x, y, _bitmap.Width, _bitmap.Height);
                        _position = new PointF(x + _bitmap.Width / 2, y + _bitmap.Height / 2);
                    }

                }
                /////////////////////////
                //_rectangle = new Rectangle(x, y, _bitmap.Width, _bitmap.Height);
                //_position = new PointF(x + _bitmap.Width / 2, y + _bitmap.Height / 2);
            }
        }

        private string GetConceptCombinationText()
        {
            if (_conceptComb == ConceptCmbination.None)
                return "";
            Frame maxConcept = null;
            double RelWeight = -1;
            DirectRelationBasedTMRWeighter2 weighter = new DirectRelationBasedTMRWeighter2(_tmr);
            List<double> NFWeights = weighter.GetNounFrameWeights();
            List<Frame> visited = new List<Frame>();
            double frameWeight = NFWeights[_tmr.Nounframes.IndexOf(this._nounFrame)];
            GetConceptCombinationText(this._nounFrame,2,0, NFWeights, frameWeight , ref visited, ref maxConcept, ref  RelWeight);
            if (RelWeight > 3 && maxConcept != null)
                return ((NounFrame)maxConcept).Text;
            else
                return "";
        }

        private void GetConceptCombinationText(Frame currConcept, int maxLevel, int currLevel, List<double> nfWeights, double frameWeight, ref List<Frame> visitedNodes, ref Frame maxConcept, ref double RelWeight)
        {
            visitedNodes.Add(currConcept);
            if (currConcept is NounFrame)
            {
                NounFrame nf = (NounFrame)currConcept;
                if (currLevel != maxLevel)
                {
                    List<Frame> frames = nf.GetNeighbors(_tmr);
                    for (int i = 0; i < frames.Count; i++)
                    {
                        if (!visitedNodes.Contains(frames[i]))
                        {
                            GetConceptCombinationText(frames[i], maxLevel, currLevel + 1, nfWeights, frameWeight, ref visitedNodes, ref  maxConcept, ref RelWeight);
                        }
                    }
                }
                else
                {
                    double currRelWeight = nfWeights[_tmr.Nounframes.IndexOf(nf)] / frameWeight;
                    if (maxConcept == null)
                    {
                        RelWeight = currRelWeight;
                        maxConcept = currConcept;
                    }
                    else
                    {
                        if (currRelWeight > RelWeight)
                        {
                            RelWeight = currRelWeight;
                            maxConcept = currConcept;
                        }
                    }
                }


            }
            else if (currConcept is VerbFrame)
            {
                if (currLevel != maxLevel)
                {
                    VerbFrame vf = (VerbFrame)currConcept;
                    List<Frame> frames = vf.GetNeighbors(_tmr);
                    for (int i = 0; i < frames.Count; i++)
                    {
                        if (!visitedNodes.Contains(frames[i]))
                        {
                            GetConceptCombinationText(frames[i], maxLevel, currLevel + 1, nfWeights, frameWeight, ref visitedNodes, ref  maxConcept, ref RelWeight);
                        }
                    }
                }
            }




        }

        private ImageSize GetAutoSize()
        {
            int links = 0;
            foreach (CaseRole cr in _nounFrame.Ownerof.Keys)
            {
                links += _nounFrame.Ownerof[cr].Count;
            }
            foreach (VerbFrame VF in _tmr.VerbFrames)
            {
                foreach (CaseRole cr in VF.CaseRoles.Keys)
                {
                    if (VF.CaseRoles[cr].Contains(_nounFrame))
                        links++;
                }

                foreach (DomainRelationType dr in VF.DomainRelations_n.Keys)
                {
                    if (VF.DomainRelations_n[dr].Contains(_nounFrame))
                        links++;
                }

                foreach (TemporalRelationType tr in VF.TemporalRelations_n.Keys)
                {
                    if (VF.TemporalRelations_n[tr].Contains(_nounFrame))
                        links++;
                }


            }
            if (links >= 7)
                return ImageSize.Medium;
            else
                return ImageSize.Small;
        }

        private bool IsGoogleImage()
        {
            //return false;
            //if (_nounFrame.ParseNode.Children != null)
            //{
            //    ParseNode pn = (ParseNode)_nounFrame.ParseNode.Children[0];
            //    if (pn.Goal != "PRO_N")
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        if (_nounFrame.Text == "Shakespeare")
            //        {
            //            return true;
            //        }
            //        return false;
            //    }

            //}

            return true;
            //return false;
        }



        public override void Draw(System.Drawing.Graphics graphics)
        {
            //string Text = "";

            //if (_nounFrame.Adjective != null)
            //{
            //    foreach (ParseNode adj in _nounFrame.Adjective)
            //        Text += (adj.Text + " ");

            //}
            //Text += (_nounFrame.Text);

            if (_bitmap == null)
            {
                PointF point = new Point();
                point.X = this.Position.X - 40;
                point.Y = this.Position.Y + 30;

                base.Draw(graphics);
                //graphics.DrawImage(_bitmap, _rectangle.Location);
                graphics.DrawString(_nounFrame.SearchText1, new Font(FontFamily.GenericSansSerif, 14), new System.Drawing.SolidBrush(Color.Black), point);

            }
            else
            {
                int h = _bitmap.Height / 2;
                int w = _bitmap.Width / 2;
                PointF point = new Point();
                point.X = this.Position.X - w;
                point.Y = this.Position.Y + h;
                graphics.DrawImage(_bitmap, _rectangle.Location);
                graphics.DrawString(_nounFrame.SearchText1, new Font(FontFamily.GenericSansSerif, 14), new System.Drawing.SolidBrush(Color.Black), point);
            }
        }
    

    }
}

   

       

