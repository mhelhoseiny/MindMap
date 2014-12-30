using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gif.Components;
using mmTMR;
using MindMapViewingManagement;

namespace MMG
{
    public partial class MultiLevelMMapForm : Form
    {
        List<pics> picsForms = new List<pics>();
        GoogleImageSearchSettings _settings;
        DrawingSizeMode _DrawSizeMode;
        public int Type;
        MindMapTMR _TopTMR;
        MindMapTMR _OriginalTMR;
        MultilevelGenerator.MultiLevel _ML;
        Form5 _parentForm;
        public MultiLevelMMapForm(Form5 parentForm,MindMapTMR TopTMR, MindMapTMR OriginalTMR, MultilevelGenerator.MultiLevel ML, GoogleImageSearchSettings settings, DrawingSizeMode drawSizeMode)
        {
            InitializeComponent();
            _parentForm = parentForm;
            _settings = settings;
            _DrawSizeMode = drawSizeMode;
            _TopTMR = TopTMR;
            _OriginalTMR = OriginalTMR;
            _ML = ML;
          

        }

        public List<List<PointF>> GETMLDrawingLayout()
        {
            List<List<PointF>> layout = new List<List<PointF>>();
            for (int i = 0; i < picsForms.Count; i++)
            {
                List<PointF> pts = new List<PointF>();
                for (int j = 0; j < picsForms[i].viewer.Entities.Count; j++)
                {
                    pts.Add(picsForms[i].viewer.Entities[j].Position); 
                }
                layout.Add(pts);
            }
            return layout;
        }

          public void LoadLayoutFrom(List<List<PointF>>  layout)
          {
            for (int i = 0; i < picsForms.Count; i++)
            {
                for (int j = 0; j < picsForms[i].viewer.Entities.Count; j++)
                {
                    picsForms[i].viewer.Entities[j].Position =  layout[i][j]; 
                }
                picsForms[i].Refresh();
            }
          }
        



        public void SaveCaseGIF(string path)
        {
            List<Image> images = GetImages();
            GenerateGIF(images, path);
 
        }

        private List<Image> GetImages()
        {
            List<Image> images = new List<Image>();
            for (int i = 0; i < picsForms.Count; i++)
            {
                var bm = new Bitmap(picsForms[i].Width, picsForms[i].Height);
                picsForms[i].DrawToBitmap(bm, new Rectangle(0, 0, picsForms[i].Width, picsForms[i].Height));
                images.Add(bm);
            }
            return images;
           
        }


         static void GenerateGIF(List<Image> images, string path)
        {
            AnimatedGifEncoder e = new AnimatedGifEncoder();
            e.Start(path);
            e.SetDelay(4000);
            //-1:no repeat,0:always repeat
            e.SetRepeat(0);
            for (int i = 0; i < images.Count; i++)
            {
                e.AddFrame(images[i]);
            }
            e.Finish();
 
        }

         private void setLayoutToAllToolStripMenuItem_Click(object sender, EventArgs e)
         {
             _parentForm.SetMLLyaoutToAll(this);
         }

         private void MultiLevelMMapForm_Load(object sender, EventArgs e)
         {
             pics pic = new pics(_TopTMR, _OriginalTMR, _ML, 0, picsForms, _settings, _DrawSizeMode);
             pic.Text = this.Text;
             pic.Show();

             for (int i = 0; i < picsForms.Count; i++)
             {
                 picsForms[i].MdiParent = this;
             }

         }




         internal void HandleOtherExpandions(Frame.Type type, int frameIndex, int formIndex)
         {
            for (int i = 0; i < _parentForm.MLForms.Count; i++)
			{
                if (_parentForm.MLForms[i] != this)
                {
                    pics PicForm = _parentForm.MLForms[i].picsForms[formIndex];
                    if (type == Frame.Type.Noun)
                        PicForm.HandleExpansion(PicForm.viewer._TMR.Nounframes[frameIndex], _parentForm.MLForms[i].picsForms);
                    else
                        PicForm.HandleExpansion(PicForm.viewer._TMR.VerbFrames[frameIndex], _parentForm.MLForms[i].picsForms);
                }
			}
         }
    }
}
