using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace DemoScenes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
          
            DoubleBuffered = true;
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Start();
            timer.Tick += Timer_Tick;
            Paint += Form1_Paint;
            init();

           b.theBMP = bmp;
           b.theFB = fb;
           b.theGBMP = gBmp;
           b.theGForm = gForm;
           b.theWidth = fWidth;
           b.theHeight = fHeight;
           b.theMarginHeight = _marginHeight;
           b.theMarginWidth = _marginWidth;


            b.init();
        }
        Bobs b = new Bobs();

        private int _marginWidth = 25;
        private int _marginHeight = 25;
        private Bitmap bmp;
        private Graphics gForm;
        private Graphics gBmp;
        private int fHeight, fWidth;
        private int midWidth, midHeight;
        private Random rand;
        private FastBitmap fb;
        const double PI = 3.14159;
        DrawScreen cDraw;
        bool doit = false;
        private void init()
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gForm = pictureBox1.CreateGraphics();
            gBmp = Graphics.FromImage(bmp);
            rand = new Random();
            fHeight = bmp.Height;
            fWidth = bmp.Width;
            midWidth = fWidth / 2;
            midHeight = fHeight / 2;

            cDraw = new DrawScreen();

            initForm(cDraw);
            cDraw.theWidth = fWidth;
            cDraw.theHeight = fHeight;
            cDraw.theMarginWidth = cDraw.theMarginHeight = 15;
        }
        private void initForm<T>(T obj) where T : BaseGraphics, new()
        {
            obj.theBMP = bmp;
            obj.theFB = fb;
            obj.theGBMP = gBmp;
            obj.theGForm = gForm;
            obj.theWidth = fWidth;
            obj.theHeight = fHeight;
            obj.theMarginHeight = _marginHeight;
            obj.theMarginWidth = _marginWidth;

        }
        private void Form1_Paint(object? sender, PaintEventArgs e)
        {
            var gr = e.Graphics;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            b.doIt("");

            Invalidate();
        }
    }

    public class DrawScreen : BaseGraphics
    {

        private int _viewerWidth;

        public int theViewerWidth
        {
            get { return _viewerWidth; }
            set { _viewerWidth = value; }
        }

        private int _viewerHeight;

        public int theViewerHeight
        {
            get { return _viewerHeight; }
            set { _viewerHeight = value; }
        }


        FastBitmap fb;


        public DrawScreen()
        {
            
            Bobs.drawIt += new Bobs.drawHandler(drawScreen);
            Bobs.startIt += new Bobs.startHandler(startFB);
            Bobs.endIt += new Bobs.endHandler(stopFB);
            Bobs.updateIt += new Bobs.updateHandler(drawFB);            

        }

        public FastBitmap startFB()
        {
            fb = new FastBitmap(theBMP);
            return fb;
        }

        public void stopFB()
        {
            fb.Release();
        }

        public void drawFB(int x, int y, Color co)
        {
            bool inWidth = x > theMarginWidth && x < theWidth - theMarginWidth;
            bool inHeight = y > theMarginHeight && y < theHeight - theMarginHeight;

            if (inWidth && inHeight)
                fb.SetPixel(x, y, co);
        }

        public void drawScreen()
        {
            try
            {
                Pen thePen = new Pen(Color.White);

                int x1 = theMarginWidth;
                int x2 = theWidth - theMarginWidth;
                int y1 = theMarginHeight;
                int y2 = theHeight - theMarginHeight;

                theGBMP.DrawLine(thePen, x1, y1, x2, y1);
                theGBMP.DrawLine(thePen, x1, y2, x2, y2);
                theGBMP.DrawLine(thePen, x1, y1, x1, y2);
                theGBMP.DrawLine(thePen, x2, y1, x2, y2);

                theGForm.DrawImage(theBMP, 0, 0, theWidth, theHeight);
            }
            catch (Exception)
            {
                //throw;
            }
        }

    }
}
