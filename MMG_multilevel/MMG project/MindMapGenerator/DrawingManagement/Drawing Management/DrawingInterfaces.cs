using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
	[Serializable]
    public abstract class IMM_Entity
    {
        protected PointF _position;
        public virtual System.Drawing.PointF Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        public abstract void Draw(Graphics graphics);

        public abstract RectangleF Rectangle { get;}
        public abstract bool InArea(Point point);
        public abstract Point GetOuterPoint(PointF vector);
        public Point GetOuterPoint(PointF point1, PointF point2)
        {
            PointF point =  new PointF();
            point.X = point2.X - point1.X;
            point.Y = point2.Y - point1.Y;

            return GetOuterPoint(point);
        }
        public virtual void Move(Point point1, Point point2)
        {
            _position.X += point2.X - point1.X;
            _position.Y += point2.Y - point1.Y;
        }
    }
 
}
