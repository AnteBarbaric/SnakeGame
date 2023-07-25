using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace OTTER
{
    public partial class Pocetna : Form
    {
        public Pocetna()
        {
            InitializeComponent();
        }
        SoundPlayer player = new SoundPlayer("music\\game_start.wav");

        private void Pocetna_Load(object sender, EventArgs e)
        {
            player.Play();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OdaberiLika ol = new OdaberiLika();
            ol.ShowDialog();
            this.Close();
        }
    }
}
