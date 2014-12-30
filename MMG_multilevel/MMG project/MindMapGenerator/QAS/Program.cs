using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MMG
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
           Control.CheckForIllegalCrossThreadCalls = false;
           Application.Run(new Form5());
            //Application.Run(new TMRDemo());
            
        }

    }
}
