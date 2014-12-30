using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
	[Serializable]
    public class MM_Circle:IMM_Entity
    {
        
        private float _radious;

        public float Radious
        {
            get { return _radious; }
            set { _radious = value; }
        }
        public MM_Circle(PointF position,float radious)
        {
            _position = position;
            _radious = radious;
 
        }

        #region IMM_Entity Members

        

        public override void Draw(System.Drawing.Graphics graphics)
        {
            graphics.DrawEllipse(new Pen(Color.Red), Rectangle);    
        }

        public override  System.Drawing.RectangleF Rectangle
        {
            get
            {
                PointF p = new PointF();
                p.X = _position.X - _radious;
                p.Y = _position.Y - _radious;
                return new RectangleF(p, new SizeF(2 * _radious, 2 * _radious));
            }
        }

        #endregion

        #region IMM_Entity Members


        public override bool InArea(Point point)
        {
            if ((point.X - _position.X) * (point.X - _position.X) + (point.Y - _position.Y) * (point.Y - _position.Y) <= _radious * _radious)
            {
                return true;
            }
            return false;
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public override Point GetOuterPoint(PointF vector)
        {
            double angle = Math.Atan2(vector.Y, vector.X);

            Point P = new Point();
            P.X =(int)  (_position.X+_radious*Math.Cos(angle));
            P.Y = (int ) (_position.Y + _radious * Math.Sin(angle));

            return P;

        }
    }
}
