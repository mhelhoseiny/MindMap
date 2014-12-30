using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
    public class DrawingHelperFunctions
    {
        public static double Distance(Point point1, Point point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }
        public static double Distance(PointF point1, PointF point2)
        {
            return Math.Sqrt((point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y));
        }
    }
}
