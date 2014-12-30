using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MindMapGenerator.Drawing_Management
{
	[Serializable]
    public abstract class IMM_Link
    {
        protected IMM_Entity _entity1,_entity2;

        public IMM_Entity Entity2
        {
          get { return _entity2; }
          set { _entity2 = value; }
        }

        public IMM_Entity Entity1
        {
          get { return _entity1; }
          set { _entity1 = value; }
        }
        public IMM_Link(IMM_Entity entity1,IMM_Entity entity2)
        {
            _entity1 = entity1;
            _entity2 = entity2;
        }

        public abstract void Draw(Graphics g);

        
    }
}
