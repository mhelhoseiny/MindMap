using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MindMapGenerator.Drawing_Management;
namespace MMG
{
    
    public partial class pics : Form
    {
        public pics()
        {
            InitializeComponent();
        }

       
        /*
        public Image im;
        public Bitmap bt;

        public pics(Bitmap btm)
        {
            bt = btm;
            begin();
        }
        MindMapGenerator.Drawing_Management.DrawingManager DM;
        public void begin()
        {
            mm_Image entity = new mm_Image(20, 20, bt);
            mm_Image entity2 = new mm_Image(100, 100, bt);
            MM_Line line = new MM_Line(entity, entity2);
            DM = new DrawingManager(this);
            DM.Add(entity);
            DM.Add(entity2);
            DM.Add(line);


        }
         */ 
    }
     
}