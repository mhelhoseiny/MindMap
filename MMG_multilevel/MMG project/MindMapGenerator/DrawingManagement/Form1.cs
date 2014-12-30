using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MindMapGenerator.Drawing_Management;

namespace MindMapGenerator
{
    public partial class Form1 : Form
    {
        Drawing_Management.DrawingManager DrawManager;
        public Form1()
        {
            InitializeComponent();
            DrawManager = new Drawing_Management.DrawingManager(this);
            //MM_Circle circle1 = new MM_Circle(new PointF(100, 100), 50);
            //MM_Circle circle2 = new MM_Circle(new PointF(120, 150), 60);
            //DrawManager.Add(circle1);
            //DrawManager.Add(circle2);
            //DrawManager.Add(new MM_Line(circle1, circle2));
            MM_Rectangle rect1 = new MM_Rectangle(100, 100, 20,50);
            MM_Rectangle rect2 = new MM_Rectangle(120, 150, 70,30);
            DrawManager.Add(rect1);
            DrawManager.Add(rect2);
            DrawManager.Add(new MM_Line(rect1, rect2));
            
        }
        

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }
    
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {

                MM_Circle c = new MM_Circle(new PointF(float.Parse(txt_X.Text), float.Parse(txt_Y.Text)), float.Parse(txt_radious.Text));
                DrawManager.Add(c);
            }
            catch
            {
                MessageBox.Show("Input error");
            }
        }
    }
}