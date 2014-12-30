using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
	[Serializable]
    public class MM_RectangleWithText:MM_Rectangle
    {


         protected string _text;
         protected string _text2;

         public string Text2
         {
             get { return _text2; }
             set { _text2 = value; }
         }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        


        public MM_RectangleWithText(int x, int y, int width, int height, string text,string text2)
            : base(x,y,width,height)
        {
           
            _text = text;
            _text2 = text2;
        }
        public override void Draw(System.Drawing.Graphics graphics)
        {
            PointF point = new PointF();
            //point.X = this.Position.X - 40;
            //point.Y = this.Position.Y + 20;
            point.X = this.Position.X - 10;
            point.Y = this.Position.Y + 10;


            PointF point2 = new PointF();
            //point2.X = this.Position.X - 40;
            //point2.Y = this.Position.Y -50;
            point2.X = this.Position.X - 10;
            point2.Y = this.Position.Y + 10;


            base.Draw(graphics);
            graphics.DrawString(_text, new Font(FontFamily.GenericSansSerif, 15), new System.Drawing.SolidBrush(Color.Black), point);//this.Position);
            if(_text2!="")
                graphics.DrawString(_text2, new Font(FontFamily.GenericSansSerif, 15), new System.Drawing.SolidBrush(Color.Red), point2);//this.Position);
        }
       

    }
}
