using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WsprInspector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Text = $"WSPR Inspector v{WsprSharp.Version.VersionString}";

            Recalculate();
        }

        private void Recalculate()
        {
            var wspr = new WsprSharp.WsprTransmission(tbCallsign.Text, tbLocation.Text, double.Parse(cbPower.Text));

            if (wspr.IsValid == false)
            {
                tbMessage.Text = "invalid input";
                rtbLevels.Text = "invalid input";
                return;
            }

            tbMessage.Text = wspr.MessageString;
            rtbLevels.Text = wspr.LevelsString;
            pictureBox1.Image = WsprImage.MakeSpectrogram(wspr.Levels);
        }

        private void tbCallsign_TextChanged(object sender, EventArgs e) => Recalculate();
        private void tbLocation_TextChanged(object sender, EventArgs e) => Recalculate();
        private void cbPower_SelectedIndexChanged(object sender, EventArgs e) => Recalculate();
    }
}
