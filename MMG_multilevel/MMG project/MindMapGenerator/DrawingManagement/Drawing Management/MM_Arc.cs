using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
	[Serializable]
    public class MM_Arc:IMM_Link
    {
        public MM_Arc(IMM_Entity entity1,IMM_Entity entity2):base(entity1,entity2)
        { 
        }

        public override void Draw(System.Drawing.Graphics g)
        {
            
            
            Point point1 = _entity1.GetOuterPoint(_entity1.Position, _entity2.Position);
            Point point2 = _entity2.GetOuterPoint(_entity2.Position, _entity1.Position);
            
            int w=Math.Abs(point1.X-point2.X);
            int h=Math.Abs(point1.Y-point2.Y);
            Size size=new Size(w,h);
            Rectangle R = new Rectangle(point1, size);

            if (DrawingHelperFunctions.Distance(_entity1.Position, point1) + DrawingHelperFunctions.Distance(_entity2.Position, point2) <=
                DrawingHelperFunctions.Distance(_entity1.Position, _entity2.Position)
                )

                g.DrawArc(new System.Drawing.Pen(Color.Blue), R, (float)Math.PI/5, (float)Math.PI / 6);
                //g.DrawArc(new System.Drawing.Pen(Color.Blue), R, 0, 30);
        }
    }
}
