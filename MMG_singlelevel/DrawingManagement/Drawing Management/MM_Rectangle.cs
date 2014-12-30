using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
    public class MM_Rectangle : IMM_Entity
    {
        protected Rectangle _rectangle;
        public MM_Rectangle(int x,int y,int width,int height)
        {
            _rectangle = new Rectangle(x, y, width, height);
            _position = new PointF(x+width/2,y+height/2);
        }
        public override void Draw(System.Drawing.Graphics graphics)
        {
                        graphics.DrawRectangle(new Pen(Color.BlueViolet),_rectangle);
            
        }
        public override PointF Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                _rectangle.Location = new Point((int)(_position.X - _rectangle.Width / 2),
           (int)(_position.Y - _rectangle.Height / 2)); 
            }
        }

        public override System.Drawing.RectangleF Rectangle
        {
            get { return  new RectangleF(_rectangle.X,_rectangle.Y,_rectangle.Width,_rectangle.Height); }
        }

        public override void Move(Point point1, Point point2)
        {
            base.Move(point1, point2);
            _rectangle.Location = new Point( (int)(_position.X - _rectangle.Width / 2),
           (int)(_position.Y - _rectangle.Height / 2)); 
        
        }

        public override bool InArea(System.Drawing.Point point)
        {
            if (point.X >= _rectangle.Left && point.X <= _rectangle.Right && point.Y >= _rectangle.Top && point.Y <= _rectangle.Bottom)
               return true;

            return false;
        }

        public override System.Drawing.Point GetOuterPoint(System.Drawing.PointF vector)
        {
            double angle1 = Math.Atan2(_rectangle.Height / 2, _rectangle.Width / 2);
            double angle2 = Math.PI/2- angle1;
            double diameterby2 = Math.Sqrt(_rectangle.Width * _rectangle.Width+_rectangle.Height*_rectangle.Height)/2;
            angle1 *= 2;
            angle2 *= 2;
            double angle = Math.Atan2(vector.Y, vector.X);
            Point pt = new Point();
            if (angle >= -Math.PI && angle <= -Math.PI+angle1 / 2)
            {
                pt.X = _rectangle.Left;
                double nangle = Math.PI + angle;
                double shifty = _rectangle.Width / 2 * Math.Tan(nangle);
                pt.Y = (int)(_position.Y - shifty);
            }
            else if (angle >= -Math.PI + angle1 / 2 && angle <= -Math.PI/2)
            {
                
                pt.Y = _rectangle.Top;
                double nangle = -(angle+Math.PI/2);
                double shiftx = _rectangle.Height / 2 * Math.Tan(nangle);
                pt.X = (int)(_position.X - shiftx);
            }
            else if (angle >= -Math.PI / 2 && angle <= -angle1 / 2)
            {
                pt.Y = _rectangle.Top;
                double nangle = angle + Math.PI / 2;
                double shiftx = _rectangle.Height / 2 * Math.Tan(nangle);
                pt.X = (int)(_position.X + shiftx);
            }
            else if (angle >= -angle1/2 && angle <= 0)
            {
                pt.X = _rectangle.Right;
                double nangle = - angle;
                double shifty = _rectangle.Width / 2 * Math.Tan(nangle);
                pt.Y = (int)(_position.Y - shifty);
            }
            else if (angle >= 0 && angle <= angle1/2)
            {
                pt.X = _rectangle.Right;
                double nangle = angle;
                double shifty = _rectangle.Width / 2 * Math.Tan(nangle);
                pt.Y = (int)(_position.Y + shifty);
            }
            else if (angle >= angle1/2  && angle <= Math.PI/2)
            {
                pt.Y = _rectangle.Bottom;
                double nangle = Math.PI / 2-angle ;
                double shiftx = _rectangle.Height / 2 * Math.Tan(nangle);
                pt.X = (int)(_position.X +shiftx);
            }
            else if (angle >= Math.PI / 2 && angle <= Math.PI / 2 +angle2/2)
            {
                pt.Y = _rectangle.Bottom;
                double nangle =  angle-Math.PI/2;
                double shiftx = _rectangle.Height / 2 * Math.Tan(nangle);
                pt.X = (int)(_position.X - shiftx);
            }
            else //if (angle >= angle1/2 + angle2  && angle <= Math.PI)
            {
                pt.X = _rectangle.Left;
                double nangle = Math.PI- angle;
                double shifty = _rectangle.Width / 2 * Math.Tan(nangle);
                pt.Y = (int)(_position.Y + shifty);
            }
            return pt;
            //throw new Exception("The method or operation is not implemented.");
        }
    }
}
