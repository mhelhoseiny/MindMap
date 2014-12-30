using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
    public class MM_Line:IMM_Link
    {
        public MM_Line(IMM_Entity entity1,IMM_Entity entity2):base(entity1,entity2)
        { 
        }

        public override void Draw(System.Drawing.Graphics g)
        {
           Point point1 =_entity1.GetOuterPoint(_entity1.Position, _entity2.Position);
           Point point2 = _entity2.GetOuterPoint(_entity2.Position, _entity1.Position);
           if (DrawingHelperFunctions.Distance(_entity1.Position, point1) + DrawingHelperFunctions.Distance(_entity2.Position, point2)<=
               DrawingHelperFunctions.Distance(_entity1.Position, _entity2.Position)
               )
                g.DrawLine(new System.Drawing.Pen(Color.Blue),point1 ,point2);
        }
    }
}
