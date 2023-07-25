using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.Aqua);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Comic Sans MS", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */
        double brojac = 0;
        Random r = new Random();
        SoundPlayer player;
        private int izbor;
        public int Izbor { get => izbor; set => izbor = value; }

        /* Initialization */
        Zmija zmija;
        Hrana hrana;
        Bonba bomba, bomba1, bomba2;

        private void SetupGame()
        {
            //1. setup stage
            SetStageTitle("Snake");
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\trava.jpg");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //2. add sprites
            if (this.Izbor == 0)
            {
                zmija = new Zmija("sprites\\sasa_t.png", 0, 0);
                hrana = new Hrana("sprites\\krafna.png", r.Next(0, GameOptions.RightEdge - 100), r.Next(0, GameOptions.DownEdge - 100));
                hrana.SetSize(10);
            }
            else if (this.Izbor == 1)
            {
                zmija = new Zmija("sprites\\divna_t.png", 0, 0);
                hrana = new Hrana("sprites\\greycat1.png", r.Next(0, GameOptions.RightEdge - 100), r.Next(0, GameOptions.DownEdge - 100));
                hrana.SetSize(10);
            }
            if (this.Izbor == 2)
            {
                zmija = new Zmija("sprites\\gordan_t.png", 0, 0);
                hrana = new Hrana("sprites\\kitten11.png", r.Next(0, GameOptions.RightEdge - 100), r.Next(0, GameOptions.DownEdge - 100));
                hrana.SetSize(26);
            }
            if (this.Izbor == 3)
            {
                zmija = new Zmija("sprites\\borka_t.png", 0, 0);
                hrana = new Hrana("sprites\\jabuka_t.png", r.Next(0, GameOptions.RightEdge - 100), r.Next(0, GameOptions.DownEdge - 100));
                hrana.SetSize(6);
            }
            zmija.SetSize(50);
            zmija.RotationStyle = "AllAround";
            Game.AddSprite(zmija);
            Game.AddSprite(hrana);
            bomba = new Bonba("sprites\\bonba.png", r.Next(0, GameOptions.RightEdge - 100), r.Next(100, GameOptions.DownEdge - 100));
            bomba.SetSize(10);
            Game.AddSprite(bomba);
            bomba1 = new Bonba("sprites\\bonba.png", r.Next(0, GameOptions.RightEdge - 100), r.Next(100, GameOptions.DownEdge - 100));
            bomba1.SetSize(10);
            Game.AddSprite(bomba1);
            bomba2 = new Bonba("sprites\\bonba.png", r.Next(0, GameOptions.RightEdge - 100), r.Next(100, GameOptions.DownEdge - 100));
            bomba2.SetSize(10);
            Game.AddSprite(bomba2);

            //3. scripts that start
            Game.StartScript(KretanjeZmije);
            Game.StartScript(Jedenje);
            Game.StartScript(Eksplodiranje);
        }

        /* Scripts */
        private int KretanjeZmije()
        {
            zmija.SetHeading(Sprite.DirectionsType.down);
            while (START)
            {
                zmija.MoveSteps(zmija.Brzina);
                if (zmija.TouchingEdge() || brojac<-5)
                {
                    player = new SoundPlayer("music\\game_over.wav");
                    player.Play();
                    ISPIS = "Game over. Imate : "+brojac+" bodova.";
                    zmija.SetVisible(false);
                    hrana.SetVisible(false);
                    bomba.SetVisible(false);
                    bomba1.SetVisible(false);
                    bomba2.SetVisible(false);
                    START = false;
                }
                if(START==false)
                {
                    GameOver go = new GameOver();
                    go.ShowDialog();
                }
                if(sensing.KeyPressed(Keys.Right))
                {
                    zmija.SetHeading(Sprite.DirectionsType.right);
                    zmija.MoveSteps(zmija.Brzina);
                }
                if (sensing.KeyPressed(Keys.Left))
                {
                    zmija.SetHeading(Sprite.DirectionsType.left);
                    zmija.MoveSteps(zmija.Brzina);
                }
                if (sensing.KeyPressed(Keys.Up))
                {
                    zmija.SetHeading(Sprite.DirectionsType.up);
                    zmija.MoveSteps(zmija.Brzina);

                }
                if (sensing.KeyPressed(Keys.Down))
                {
                    zmija.SetHeading(Sprite.DirectionsType.down);
                    zmija.MoveSteps(zmija.Brzina);
                }
                Wait(0.05);
            }
            return 0;
        }
        private int Jedenje()
        {
            while (START) //ili neki drugi uvjet
            {
                if(zmija.TouchingSprite(hrana))
                {
                    brojac += 1;
                    if (brojac%7==0)
                    {
                        player = new SoundPlayer("music\\burp.wav");
                        player.Play();
                    }
                    else
                    {
                        player = new SoundPlayer("music\\jedenje.wav");
                        player.Play();
                    }
                    hrana.SetVisible(false);
                    ISPIS = "Bodovi: "+brojac;
                    zmija.Brzina ++;
                    hrana.SetX(r.Next(0,GameOptions.RightEdge-hrana.Width));
                    hrana.SetY(r.Next(0, GameOptions.DownEdge - hrana.Heigth));
                    hrana.SetVisible(true);
                }
                Wait(0.1);
            }
            return 0;
        }
        private int Eksplodiranje()
        {
            while (START) //ili neki drugi uvjet
            {
                if (zmija.TouchingSprite(bomba))
                {
                    brojac -= 2;
                    if (brojac == 0 || brojac==-2)
                    {
                        player = new SoundPlayer("music\\scream.wav");
                        player.Play();
                    }
                    else
                    {
                        player = new SoundPlayer("music\\eksplozija.wav");
                        player.Play();
                    }
                    bomba.SetVisible(false);
                    ISPIS = "Bodovi: " + brojac;
                    bomba.SetX(r.Next(0, GameOptions.RightEdge - bomba.Width));
                    bomba.SetY(r.Next(0, GameOptions.DownEdge - bomba.Heigth));
                    bomba.SetVisible(true);
                }
                else if(zmija.TouchingSprite(bomba1))
                {
                    brojac -= 2;
                    if (brojac == 0 || brojac==-2)
                    {
                        player = new SoundPlayer("music\\scream.wav");
                        player.Play();
                    }
                    else
                    {
                        player = new SoundPlayer("music\\eksplozija.wav");
                        player.Play();
                    }
                    bomba1.SetVisible(false);
                    ISPIS = "Bodovi: " + brojac;
                    bomba1.SetX(r.Next(0, GameOptions.RightEdge - bomba1.Width));
                    bomba1.SetY(r.Next(0, GameOptions.DownEdge - bomba1.Heigth));
                    bomba1.SetVisible(true);
                }
                else if (zmija.TouchingSprite(bomba2))
                {
                    brojac -= 2;
                    if (brojac == 0 || brojac==-2)
                    {
                        player = new SoundPlayer("music\\scream.wav");
                        player.Play();
                    }
                    else
                    {
                        player = new SoundPlayer("music\\eksplozija.wav");
                        player.Play();
                    }
                    bomba2.SetVisible(false);
                    ISPIS = "Bodovi: " + brojac;
                    bomba2.SetX(r.Next(0, GameOptions.RightEdge - bomba2.Width));
                    bomba2.SetY(r.Next(0, GameOptions.DownEdge - bomba2.Heigth));
                    bomba2.SetVisible(true);
                }
                Wait(0.1);
            }
            return 0;
        }
        
        /* ------------ GAME CODE END ------------ */


    }
}
