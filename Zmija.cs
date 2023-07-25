using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class Zmija:Sprite
    {
        private int brzina;
        public int Brzina { get => brzina; set => brzina = value; }
        public Zmija(string slika, int x, int y)
            :base(slika,300,0)
        {
            this.Brzina = 5;
        }
    }
    class Hrana:Sprite
    {
        public Hrana(string slika, int x, int y)
            :base(slika,x,y)
        {
            
        }
    }
    class Bonba : Sprite
    {
        public Bonba(string slika, int x, int y)
            : base(slika, x, y)
        {

        }
    }
}
