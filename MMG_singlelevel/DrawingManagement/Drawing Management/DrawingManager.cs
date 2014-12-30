using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace MindMapGenerator.Drawing_Management
{
    public class DrawingManager
    {
        private Control _control;
        List<IMM_Entity> _entities = new List<IMM_Entity>();
        List<IMM_Link> _links = new List<IMM_Link>();
        public void Add(IMM_Entity entitiy)
        {
            _entities.Add(entitiy);
            _control.Refresh();
        }
        public void Add(IMM_Link link)
        {
            _links.Add(link);
            _control.Refresh();
        }

        public void Clear()
        {
            _links.Clear();
            _entities.Clear();
        }
        public Control Control
        {
            get { return _control; }
            set { _control = value; }
        } 

        public DrawingManager(Control control)
        {
            _control = control;
                        _control.Paint += new PaintEventHandler(_control_Paint);
            _control.MouseDown += new MouseEventHandler(_control_MouseDown);
            _control.MouseMove += new MouseEventHandler(_control_MouseMove);
            _control.MouseUp += new MouseEventHandler(_control_MouseUp);
        }

        Point _mousedownPoint =  new Point(-1,-1);
        bool _mousedown = false;
        IMM_Entity _movingentity=null;
      
        void _control_MouseUp(object sender, MouseEventArgs e)
        {
            _mousedown = false;
            _mousedownPoint = new Point(-1,-1);
            _lastmouseMovePt = new Point(-1, -1);
            _movingentity = null;
            _control.Refresh();
            //throw new Exception("The method or operation is not implemented.");
        }

        void _control_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mousedown)
                return;
            if (_movingentity != null&&_lastmouseMovePt.X!=-1)
                _movingentity.Move(_lastmouseMovePt, new Point(e.X, e.Y));
            _lastmouseMovePt = new Point(e.X, e.Y);
            _control.Refresh();
        }

        void _control_MouseDown(object sender, MouseEventArgs e)
        {
            _mousedownPoint = new Point(e.X, e.Y);
            _mousedown = true;
            foreach (IMM_Entity entity in _entities)
            {
                if (entity.InArea(_mousedownPoint))
                {
                    _movingentity = entity;
                    return;
 
                }
           }
           _movingentity = null;
           _control.Refresh();
        }

        Point _lastmouseMovePt = new Point(-1, -1);
        void _control_Paint(object sender, PaintEventArgs e)
        {
            foreach (IMM_Entity entity in _entities)
            {
                entity.Draw(e.Graphics);
            }
            foreach (IMM_Link link in _links)
            {
                link.Draw(e.Graphics);
            }
            //throw new Exception("The method or operation is not implemented.");
        }

       
    }
}
