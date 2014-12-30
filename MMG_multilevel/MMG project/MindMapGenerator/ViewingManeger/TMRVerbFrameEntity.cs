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
	[Serializable]
    class TMRVerbFrameEntity : MM_RectangleWithText
    {
        VerbFrame _verbFrame;
        Bitmap _bitmap=null;
         public TMRVerbFrameEntity(int x, int y, VerbFrame verbFrame)
             : base(0, 0, 70, 40, verbFrame.VerbName,"")
        
        {
            _verbFrame = verbFrame;
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
                if (_verbFrame.Adverb != null)
                {
                    foreach (string adv in _verbFrame._Adverb)
                        Text += (adv + " ");
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
