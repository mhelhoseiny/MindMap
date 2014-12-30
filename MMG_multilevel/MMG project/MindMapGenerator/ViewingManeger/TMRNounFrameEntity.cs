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
    class TMRNounFrameEntity : MM_RectangleWithText
    {
        NounFrame _nounFrame;
        Bitmap _bitmap=null;
        
        
        public PictureBox picbox = new PictureBox();
        NewGoogleSearch GoogleImSearch = new NewGoogleSearch();
        public TMRNounFrameEntity(int x, int y, NounFrame nounFrame, NewGoogleSearch googleImSearch)
            : base(0, 0, 70, 40, nounFrame.Text,"")
            //:base(0,0,70,40,nounFrame.Text)
        {
            GoogleImSearch = googleImSearch;
            _nounFrame = nounFrame;

            string Text = "";
           
            

            ////////////////////////////
            if (_nounFrame.AdjectivesInfo != null)
            {                

                foreach (MyWordInfo mwi in _nounFrame.AdjectivesInfo)
                {
                    Text += mwi.Word;
                    
                }
            }

            Text += (_nounFrame.Text);
            ///////////////////////////

            _nounFrame.SearchText1 = Text;
            
            Text2 = nounFrame.IS_a;
            if (IsGoogleImage())
            {
                //_bitmap = GoogleSearch.GetImage(_nounFrame.SearchText1);

                
                IList<IImageResult> Results = GoogleImSearch.Search2(_nounFrame.SearchText1);

                if (Results.Count >= 1)
                {
                    GoogleImSearch.LoadImageFromUrl(Results[0].TbImage.Url, picbox);
                    _bitmap = new Bitmap(picbox.Image);

                    _rectangle = new Rectangle(x, y, _bitmap.Width, _bitmap.Height);
                    _position = new PointF(x + _bitmap.Width / 2, y + _bitmap.Height / 2);
                }
            }
        }
        private bool IsGoogleImage()
        {
            return true;
        }
        public override void Draw(System.Drawing.Graphics graphics)
        {
            if (_bitmap == null)
            {
                base.Draw(graphics);
            }
            else
            {
                int h = _bitmap.Height / 2;
                int w = _bitmap.Width / 2;
                PointF point = new Point();
                point.X = this.Position.X - w;
                point.Y = this.Position.Y + h;
                graphics.DrawImage(_bitmap, _rectangle.Location);
                graphics.DrawString(_nounFrame.SearchText1, new Font(FontFamily.GenericSansSerif, 20), new System.Drawing.SolidBrush(Color.Black), point);
            }
        }
    }

       
}
