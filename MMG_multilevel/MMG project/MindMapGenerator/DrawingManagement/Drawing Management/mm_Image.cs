using System;
using System.Collections.Generic;
using System.Text;
using MindMapGenerator.Drawing_Management;
using System.Drawing;
namespace GoogleImageDrawingEntity
{
	[Serializable]
    public class mm_Image : MindMapGenerator.Drawing_Management.MM_Rectangle
    {
        
        public Bitmap bitmap;
        public mm_Image(int x,int y,Bitmap bm):base(x,y,bm.Width,bm.Height)
        {
            bitmap = bm;
        }
        public override void Draw(Graphics graphics)
        {
            graphics.DrawImage(bitmap, _rectangle.Location);
        }
    }
}