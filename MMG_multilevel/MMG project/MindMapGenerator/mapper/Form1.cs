using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace OntologyLibrary
{
    public partial class Form1 : Form
    {
      

        public Form1()
        {
            InitializeComponent();
        }
 

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Wnlib.WNCommon.path = "C:\\Program Files\\WordNet\\2.1\\dict\\";
            OntologyMapperGenerator omg = new OntologyMapperGenerator(@"..\..\..\Ontology\Formatted OntoSem");
            omg.ConstructMapping();
            
        }
    }
}