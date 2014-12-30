using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace GraphTest
{
    class CompareClass :IComparer<Node>
    {
        #region IComparer Members
        public CompareClass()
        { 
        }

        public int Compare(Node n1, Node n2)
        {

            if (n1.energy < n2.energy)
                return 1;
            else if (n1.energy == n2.energy)
                return 0;
            else
                return -1;
        }

        #endregion
    }
}
