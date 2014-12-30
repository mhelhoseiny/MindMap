using System;
using System.Collections.Generic;
using System.Text;
using MindMapGenerator.Drawing_Management;
using mmTMR;
using GoogleImageDrawingEntity;
using System.Drawing;
using SyntacticAnalyzer;
using WordsMatching;
using Google.API.Search;
using ViewingManeger;
using System.Windows.Forms;

namespace MindMapViewingManagement
{
	[Serializable]
    class VerbFrameEntity : MM_RectangleWithText
    {
        VerbFrame _verbFrame;
        Bitmap _bitmap=null;
        NewGoogleSearch GoogleImSearch {get;set;}
        public PictureBox picbox = new PictureBox();
        public DrawingSizeMode _SizeMode;
        MindMapTMR _tmr;
         public VerbFrameEntity(int x, int y, VerbFrame verbFrame,MindMapTMR tmr,NewGoogleSearch googleImSearch,DrawingSizeMode SizeMode )
             : base(0, 0, 70, 40, verbFrame.VerbName,"")
        {
            _tmr = tmr;
            _SizeMode = SizeMode;
            GoogleImSearch = googleImSearch;
            string Text = "";
            _verbFrame = verbFrame;
            if (_verbFrame.AdverbsInfo != null)
            {
                foreach (MyWordInfo mwi in _verbFrame.AdverbsInfo)
                {
                    Text += mwi.Word;

                }
            }
            Text += (_verbFrame.VerbName);
          
            _text = Text;
            if (IsGoogleImage())
            {
                //_bitmap = GoogleSearch.GetImage(verbFrame.VerbName);
                //_rectangle = new Rectangle(x, y, _bitmap.Width, _bitmap.Height);
                //_position = new PointF(x + _bitmap.Width / 2, y + _bitmap.Height / 2);
                


                    
                    IList<IImageResult> Results;
                    if (_SizeMode == DrawingSizeMode.Normal)
                    {
                        Results = GoogleImSearch.Search2(_text);
                    }
                    else
                    {
                        ImageSize imsize = GetAutoSize();
                        Results = GoogleImSearch.Search2(_text, imsize);

                    }
                    if (Results.Count >= 1)
                    {
                        googleImSearch.LoadImageFromUrl(Results[0].TbImage.Url, picbox);
                        _bitmap = new Bitmap(picbox.Image);
                        _rectangle = new Rectangle(x, y, _bitmap.Width, _bitmap.Height);
                        _position = new PointF(x + _bitmap.Width / 2, y + _bitmap.Height / 2);
                    }
                
            }
            
        }

         private ImageSize GetAutoSize()
         {
             int links = 0;
             
            
        
             foreach (TemporalRelationType tr in _verbFrame.TemporalRelations.Keys)
             {
                 if (_verbFrame.TemporalRelations[tr].Contains(_verbFrame))
                     links++;
             }
             foreach (DomainRelationType dr in _verbFrame.DomainRelations.Keys)
             {
                 if (_verbFrame.DomainRelations[dr].Contains(_verbFrame))
                     links++;
             }

             foreach (VerbFrame VF in _tmr.VerbFrames)
             {
                 foreach (DomainRelationType dr in VF.DomainRelations.Keys)
                 {
                     if (VF.DomainRelations[dr].Contains(_verbFrame))
                         links++;
                 }

                 foreach (TemporalRelationType tr in VF.TemporalRelations.Keys)
                 {
                     if (VF.TemporalRelations[tr].Contains(_verbFrame))
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
            return false;
        }
        public override void Draw(System.Drawing.Graphics graphics)
        {
            if (_bitmap == null)
            {
                PointF point = new Point();
                point.X = this.Position.X - 40;
                point.Y = this.Position.Y + 30;

                string Text = "";
                //if (_verbFrame.Adverb != null)
                //{
                //    foreach (ParseNode adv in _verbFrame._Adverb)
                //        Text += (adv.Text + " ");
                //}
                //if (_verbFrame.Adverb != null)
                //{
                //    foreach (string adv in _verbFrame._Adverb)
                //        Text += (adv + " ");
                //}
                if (_verbFrame.VerbNegation)
                    Text += "NOT "; 
                if (_verbFrame.AdverbsInfo != null)
                {
                    foreach (MyWordInfo mwi in _verbFrame.AdverbsInfo)
                    {
                        if(mwi.Word!="NOT")
                            Text += (mwi.Word + " ");

                    }
                }
                Text += (_verbFrame.VerbName);
                if (_verbFrame.VerbName == "KILLED" && _verbFrame.Passive && _verbFrame.Transitive)
                    Text += " BY";
                _text = Text;
                base.Draw(graphics);
               // graphics.DrawString(Text, new Font(FontFamily.GenericSansSerif, 20), new System.Drawing.SolidBrush(Color.Black), point);
            }
            else
            {
                int h = _bitmap.Height / 2;
                int w = _bitmap.Width / 2;
                PointF point = new Point();
                point.X = this.Position.X - w;
                point.Y = this.Position.Y + h;
                graphics.DrawImage(_bitmap, _rectangle.Location);
                graphics.DrawString(Text, new Font(FontFamily.GenericSansSerif, 14), new System.Drawing.SolidBrush(Color.Black), point);
            }
        }


    }
}
