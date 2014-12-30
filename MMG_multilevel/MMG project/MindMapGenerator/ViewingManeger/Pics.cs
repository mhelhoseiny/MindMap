using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MindMapGenerator.Drawing_Management;
namespace MindMapViewingManagement
{
    public class Pics
    {
        public Image im;
        public Bitmap bt ;
        public Form PicForm;
        int NoOfEntities = 0;
        
        public Pics(Bitmap btm,Form f1)
        {
            bt = btm;
            PicForm = f1;
            begin();
        }
        MindMapGenerator.Drawing_Management.DrawingManager DM;
        public void begin()
        {
            mm_Image entity = new mm_Image(500, 300, bt);
            NoOfEntities++;
            mm_Image entity2 = new mm_Image(100, 100, bt);
            NoOfEntities++;
            mm_Image entity3 = new mm_Image(300, 100, bt);
            NoOfEntities++;
            mm_Image entity4 = new mm_Image(500, 100, bt);
            NoOfEntities++;
            mm_Image entity8 = new mm_Image(700, 100, bt);
            NoOfEntities++;
            mm_Image entity9 = new mm_Image(900, 100, bt);
            NoOfEntities++;

            mm_Image entity5 = new mm_Image(100, 600, bt);
            NoOfEntities++;
            mm_Image entity6 = new mm_Image(300, 600, bt);
            NoOfEntities++;
            mm_Image entity7 = new mm_Image(500, 600, bt);
            NoOfEntities++;
            mm_Image entity10 = new mm_Image(700, 600, bt);
            NoOfEntities++;
            mm_Image entity11 = new mm_Image(900, 600, bt);
            NoOfEntities++;

            //MM_Line line = new MM_Line(entity, entity2);
            //MM_Line line1 = new MM_Line(entity, entity3);
            //MM_Line line2 = new MM_Line(entity, entity4);
            //MM_Line line3 = new MM_Line(entity, entity5);
            //MM_Line line4 = new MM_Line(entity, entity6);
            //MM_Line line5 = new MM_Line(entity, entity7);
            //MM_Line line6 = new MM_Line(entity, entity8);
            //MM_Line line7 = new MM_Line(entity, entity9);
            //MM_Line line8 = new MM_Line(entity, entity10);
            //MM_Line line9 = new MM_Line(entity, entity11);

            MM_Arc line = new MM_Arc(entity, entity2);
            MM_Arc line1 = new MM_Arc(entity, entity3);
            MM_Arc line2 = new MM_Arc(entity, entity4);
            MM_Arc line3 = new MM_Arc(entity, entity5);
            MM_Arc line4 = new MM_Arc(entity, entity6);
            MM_Arc line5 = new MM_Arc(entity, entity7);
            MM_Arc line6 = new MM_Arc(entity, entity8);
            MM_Arc line7 = new MM_Arc(entity, entity9);
            MM_Arc line8 = new MM_Arc(entity, entity10);
            MM_Arc line9 = new MM_Arc(entity, entity11);
            DM = new DrawingManager(PicForm);
            
            DM.Add(entity);
            DM.Add(entity2);
            DM.Add(entity3);
            DM.Add(entity4);
            DM.Add(entity5);
            DM.Add(entity6);
            DM.Add(entity7);
            DM.Add(entity8);
            DM.Add(entity9);
            DM.Add(entity10);
            DM.Add(entity11);
            DM.Add(line);
            DM.Add(line1);
            DM.Add(line2);
            DM.Add(line3);
            DM.Add(line4); 
            DM.Add(line5);
            DM.Add(line6);
            DM.Add(line7);
            DM.Add(line8);
            DM.Add(line9);
        }
        public void calculatePositions()
        {
            int Wdis=(int)(PicForm.Width / NoOfEntities*200);
            int Hdis = (int)(PicForm.Height / NoOfEntities*200);
            
 
        }

    }
}
