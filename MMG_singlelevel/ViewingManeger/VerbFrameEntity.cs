using System;
using System.Collections.Generic;
using System.Text;
using MindMapGenerator.Drawing_Management;
using mmTMR;
using GoogleImageDrawingEntity;
using System.Drawing;
using SyntacticAnalyzer;

namespace MindMapViewingManagement
{
    class VerbFrameEntity : MM_RectangleWithText
    {
        VerbFrame _verbFrame;
        Bitmap _bitmap;
         public VerbFrameEntity(int x, int y, VerbFrame verbFrame)
             : base(0, 0, 70, 40, verbFrame.VerbName,"")
        
        {
            _verbFrame = verbFrame;
            if (IsGoogleImage())
            {
                _bitmap = GoogleSearch.GetImage(verbFrame.VerbName);
                _rectangle = new Rectangle(x, y, _bitmap.Width, _bitmap.Height);
                _position = new PointF(x + _bitmap.Width / 2, y + _bitmap.Height / 2);
            }
            
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
                if (_verbFrame.Adverb != null)
                {
                    foreach (ParseNode adv in _verbFrame._Adverb)
                        Text += (adv.Text + " ");
                }
                Text += (_verbFrame.VerbName);

                base.Draw(graphics);
            //    graphics.DrawString(Text, new Font(FontFamily.GenericSansSerif, 20), new System.Drawing.SolidBrush(Color.Black), point);
            }
            else
            {
                graphics.DrawImage(_bitmap, _rectangle.Location);

            }
        }


    }
}
