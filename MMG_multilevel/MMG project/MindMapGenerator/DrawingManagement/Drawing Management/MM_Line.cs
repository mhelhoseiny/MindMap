using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MindMapGenerator.Drawing_Management
{
	[Serializable]
    public class MM_Line:IMM_Link
    {
        public MM_Line(IMM_Entity entity1,IMM_Entity entity2):base(entity1,entity2)
        { 
        }

        public override void Draw(System.Drawing.Graphics g)
        {
           Point point1 =_entity1.GetOuterPoint(_entity1.Position, _entity2.Position);
           Point point2 = _entity2.GetOuterPoint(_entity2.Position, _entity1.Position);
           if (DrawingHelperFunctions.Distance(_entity1.Position, point1) + DrawingHelperFunctions.Distance(_entity2.Position, point2) <=
               DrawingHelperFunctions.Distance(_entity1.Position, _entity2.Position)
               )
           {
               Pen pen = new Pen(Color.Blue);
               //pen.CustomEndCap = new AdjustableArrowCap(3, 3);
               //pen.StartCap = LineCap.NoAnchor;
               //pen.StartCap = LineCap.ArrowAnchor;
               //pen.EndCap = LineCap.RoundAnchor;
               g.DrawLine(pen, point1, point2);
           }
        }
    }
}
