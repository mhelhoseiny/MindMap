using System;
using System.Collections.Generic;
using System.Text;

namespace OurMindMapOntology
{
    public class DistanceInfo
    {
		public DistanceInfo(int distance, MindMapConcept parent, int c1ToParent, int c2ToParent, List<MindMapConcept> c1ToParentPath, List<MindMapConcept> c2ToParentPath)
        {
			this._distance = distance;
			this._parent = parent;
			this._c1ToParentDistance = c1ToParent;
			this._c2ToParentDistance = c2ToParent;
			this._c1ToParentPath = c1ToParentPath;
			this._c2ToParentPath = c2ToParentPath;
		}
        private int _distance;

        public int Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }
        private int _c1ToParentDistance;

        public int C1ToParentDistance
        {
            get { return _c1ToParentDistance; }
        }
        private int _c2ToParentDistance;

        public int C2ToParentDistance
        {
            get { return _c2ToParentDistance; }
        }
        private MindMapConcept _parent;

        public MindMapConcept Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
		private List<MindMapConcept> _c1ToParentPath;

		public List<MindMapConcept> C1ToParentPath
		{
			get
			{
				return _c1ToParentPath;
			}
			set
			{
				_c1ToParentPath = value;
			}
		}
		private List<MindMapConcept> _c2ToParentPath;

		public List<MindMapConcept> C2ToParentPath
		{
			get
			{
				return _c2ToParentPath;
			}
			set
			{
				_c2ToParentPath = value;
			}
		}
	}
}
