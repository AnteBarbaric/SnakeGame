using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    public partial class GameOver : Form
    {
        public GameOver()
        {
            InitializeComponent();
        }
        Pocetna p = new Pocetna();
        BGL bgl = new BGL();
        private void btnNo_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
