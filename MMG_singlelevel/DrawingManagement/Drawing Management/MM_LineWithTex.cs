using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
    public class MM_LineWithText:MM_Line
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

       
        public MM_LineWithText(IMM_Entity entity1, IMM_Entity entity2, string text)
            : base(entity1,entity2)
        {
            _text = text;
        }
        public override void Draw(System.Drawing.Graphics g)
        {
            base.Draw(g);
            Point point1 = _entity1.GetOuterPoint(_entity1.Position, _entity2.Position);
            Point point2 = _entity2.GetOuterPoint(_entity2.Position, _entity1.Position);

            PointF point =  new Point();
            point.X = (point1.X + point2.X)/2;
            point.Y = (point1.Y + point2.Y)/2;
            
            g.DrawString(_text, new Font(FontFamily.GenericSansSerif, 20), new System.Drawing.SolidBrush(Color.Black), point);
            


        }
    }
}
