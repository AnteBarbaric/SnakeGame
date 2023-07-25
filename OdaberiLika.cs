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
    public partial class OdaberiLika : Form
    {
        public OdaberiLika()
        {
            InitializeComponent();
        }
        BGL bgl = new BGL();
        Pocetna p = new Pocetna();

        private void btnSasa_Click(object sender, EventArgs e)
        {
            bgl.Izbor = 0;
            button1.Visible = true;
        }

        private void btnDivna_Click(object sender, EventArgs e)
        {
            bgl.Izbor = 1;
            button1.Visible = true;
        }

        private void btnGordan_Click(object sender, EventArgs e)
        {
            bgl.Izbor = 2;
            button1.Visible = true;
        }

        private void btnBorka_Click(object sender, EventArgs e)
        {
            bgl.Izbor = 3;
            button1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            this.Hide();
            bgl.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            p.ShowDialog();
            this.Close();
        }
    }
}
