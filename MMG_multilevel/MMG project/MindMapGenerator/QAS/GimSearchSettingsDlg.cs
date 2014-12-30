using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MindMapViewingManagement;

namespace MMG
{
    public partial class GimSearchSettingsDlg : Form
    {
        public GoogleImageSearchSettings GImSearchSettings;
        public GimSearchSettingsDlg(GoogleImageSearchSettings gImSearchSettings)
        {
            GImSearchSettings = gImSearchSettings;
            InitializeComponent();
            cmb_Colorization.SelectedIndex = cmb_Colorization.Items.IndexOf(gImSearchSettings.Coloriztion.ToString());
            cmbImType.SelectedIndex = cmbImType.Items.IndexOf(gImSearchSettings.Imtype.ToString());
            cmbImSize.SelectedIndex = cmbImSize.Items.IndexOf(gImSearchSettings.ImSize.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GImSearchSettings.Count = (int)numericUpDown1.Value;
            GImSearchSettings.Imtype = cmbImType.Text;
            GImSearchSettings.ImSize = cmbImSize.Text;
            GImSearchSettings.Coloriztion = cmb_Colorization.Text;

        }
    }
}
