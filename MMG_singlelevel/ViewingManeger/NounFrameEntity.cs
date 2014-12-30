using System;
using System.Collections.Generic;
using System.Text;
using MindMapGenerator.Drawing_Management;
using mmTMR;
using GoogleImageDrawingEntity;
using System.Drawing;
using SyntacticAnalyzer;
using System.Windows.Forms;

namespace MindMapViewingManagement
{
    class NounFrameEntity : MM_RectangleWithText
    {
        NounFrame _nounFrame;
        Bitmap _bitmap;
        public NounFrameEntity(int x, int y, NounFrame nounFrame)
            :base(0,0,70,40,nounFrame.Text,"")
        {

            _nounFrame = nounFrame;
            string Text = "";
            if (_nounFrame.Adjective != null)
            {
                foreach (ParseNode adj in _nounFrame.Adjective)
                    Text += (adj.Text + " ");

            }
            Text += (_nounFrame.Text);
            _nounFrame.SearchText1 = Text;
           
            string _wordologyDirectoryPath = Application.ExecutablePath;
            int index = _wordologyDirectoryPath.LastIndexOf("\\");
            _wordologyDirectoryPath = _wordologyDirectoryPath.Substring(0, index);

            if (IsGoogleImage())
            {
                ///////////////// habal
                string strpath = "";
                if (_nounFrame.Text == "agricultural".ToUpper())
                    strpath = _wordologyDirectoryPath+@"\pics\agricultural.jpg";
                else if (_nounFrame.Text == "queen".ToUpper())
                    strpath = _wordologyDirectoryPath+@"\pics\queen.jpg";
                else if (_nounFrame.Text == "shakespeare".ToUpper())
                    strpath =_wordologyDirectoryPath+@"\pics\shakespeare.jpg";
                else if (_nounFrame.Text == "living".ToUpper())
                    strpath = _wordologyDirectoryPath+@"\pics\coins.jpg";
                else if (_nounFrame.Text == "writer".ToUpper())
                    strpath = _wordologyDirectoryPath+@"\pics\writer.jpg";
                
                ////////////////////////////

                if (strpath != "")
                    _bitmap = new Bitmap(strpath);
                else
                {
                    _bitmap = GoogleSearch.GetImage(_nounFrame.SearchText1);
                }
                    /////////////////////////
                _rectangle = new Rectangle(x, y, _bitmap.Width, _bitmap.Height);
                _position = new PointF(x + _bitmap.Width / 2, y + _bitmap.Height / 2);
            }
        }

        private bool IsGoogleImage()
        {
            if (_nounFrame.ParseNode.Children != null)
            {
                ParseNode pn = (ParseNode)_nounFrame.ParseNode.Children[0];
                if (pn.Goal != "PRO_N")
                {
                    return true;
                }
                else
                {
                    if (_nounFrame.Text == "Shakespeare")
                    {
                        return true;
                    }
                    return false;
                }

            }

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
                base.Draw(graphics);
            }
            else
            {
                int h=_bitmap.Height/2;
                int w=_bitmap.Width/2;
                PointF point = new Point();
                point.X = this.Position.X-w;
                point.Y = this.Position.Y+h;
                graphics.DrawImage(_bitmap, _rectangle.Location);
                graphics.DrawString(_nounFrame.SearchText1, new Font(FontFamily.GenericSansSerif, 20), new System.Drawing.SolidBrush(Color.Black), point);
            }
        }
    }

       
}
