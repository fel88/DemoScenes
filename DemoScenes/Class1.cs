using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace DemoScenes
{
    class Bobs : BaseGraphics, iDemo
    {
        #region mad props

        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        int dlen, dwid;
        int[,] d;
        int blen, bwid;
        int[,] b;
        int centerx, centery;
        double[] ypath;
        double[] xpath;
        double count = 0;

        public struct bl
        {
            public double x, y;
        }

        private bl[] balls;

        const int NUMBER_OF_BOBS = 14;

        struct cols
        {
            public int r, g, b;
        }

        private cols[] colors;

        private const double Whorls = 16.0;
        private const double rad = 3.14159 / 180;
        double mult1;
        int l, m, n, o;

        #endregion

        public void init()
        {
            loadTexture(ResourceHelper.ReadResourceBitmap("ball4"), out dlen, out dwid, out d);
            loadTexture(ResourceHelper.ReadResourceBitmap("check"), out blen, out bwid, out b);

            mult1 = .06;
            centerx = theWidth >> 1;
            centery = theHeight >> 1;

            double rad;

            xpath = new double[512];
            ypath = new double[512];

            for (int i = 0; i < 512; i++)
            {
                rad = (i * 0.703125) * 0.0174532;
                xpath[i] = Math.Sin(rad) * centerx + centerx;
                xpath[i] = Math.Cos(rad * 2) * centerx / 2 + centerx / 2 + 120;
                ypath[i] = (centery) - (int)((Math.Cos(rad * 2) * Math.Sin(rad)) * 120) - 20;
            }


            balls = new bl[NUMBER_OF_BOBS];

            l = m = n = o = 0;
            for (int index = 0; index < NUMBER_OF_BOBS; index++)
            {
                balls[index].x = xpath[l & 511];
                balls[index].y = xpath[m & 511];
                l += 20;
                m += 20;
            }


            colors = new cols[256];

            for (int i = 0; i < 256; i++)
            {
                double red = 1 + Math.Cos(i * Math.PI / 128);
                double grn = 1 + Math.Cos((i - 85) * Math.PI / 128);
                double blu = 1 + Math.Cos((i + 85) * Math.PI / 128);
                colors[i].r = (int)(red * 127) % 256;
                colors[i].g = (int)(grn * 127) % 256;
                colors[i].b = (int)(blu * 127) % 256;
            }

        }


        public void doIt(string msg)
        {
            theGBMP.Clear(Color.Black);
            theFB = startIt();


            #region ripples

            for (int X = 0; X < theWidth; X++)
            {
                double xm = X * mult1;
                double yAdj = Math.Sin(xm + count) * 5;

                for (int Y = 0; Y < theHeight; Y++)
                {
                    double ym = Y * mult1 + count;
                    double xAdj = Math.Sin(ym) * 5;
                    double xval = Math.Cos(xm + ym) * 5;

                    int sx = (int)(X + xval - xAdj);
                    int sy = (int)(Y + xval - yAdj);

                    if (sx > theWidth || sy > theHeight) continue;

                    Color c = Color.FromArgb(X ^ Y);

                    updateIt(sx, sy, c);
                }
            }

            count += 0.3;

            #endregion


            #region bobs

            l = n;
            m = o;
            double scale = .66;
            for (int k = 0; k < NUMBER_OF_BOBS; k++)
            {
                balls[k].x = xpath[l & 511];
                balls[k].y = ypath[m & 511];
                drawBall(scale, balls[k].x, balls[k].y);
                scale += .01;
                l += 20;
                m += 20;
            }

            n += 1;
            o += 2;
            n &= 511;
            o &= 511;

            #endregion


            endIt();
            drawIt();

        }


        private void drawBall(double scale, double ix, double iy)
        {
            for (int i = 0; i < dlen; i++)
            {
                for (int j = 0; j < dwid; j++)
                {
                    int xx = d[i, j];
                    if (xx == 999) continue;

                    updateIt((int)((i + ix) * scale), (int)((j + iy) * scale), Color.FromArgb(xx));

                }
            }
        }

    }
    interface iDemo
    {
        void init();
        void doIt(string msg);
    }

    public abstract class BaseGraphics
    {

        private int _marginWidth;

        public int theMarginWidth
        {
            get { return _marginWidth; }
            set { _marginWidth = value; }
        }

        private int _marginHeight;

        public int theMarginHeight
        {
            get { return _marginHeight; }
            set { _marginHeight = value; }
        }

        private FastBitmap _fb;

        public FastBitmap theFB
        {
            get { return _fb; }
            set { _fb = value; }
        }

        private Bitmap _bmp;

        public Bitmap theBMP
        {
            get { return _bmp; }
            set { _bmp = value; }
        }

        private Graphics _gBmp;

        public Graphics theGBMP
        {
            get { return _gBmp; }
            set { _gBmp = value; }
        }

        private Graphics gForm;

        public Graphics theGForm
        {
            get { return gForm; }
            set { gForm = value; }
        }

        private int _width;

        public int theWidth
        {
            get { return _width; }
            set { _width = value; }
        }
        private int _height;

        public int theHeight
        {
            get { return _height; }
            set { _height = value; }
        }

        private bool _fadeOut;

        public bool theFadeOut
        {
            get { return _fadeOut; }
            set { _fadeOut = value; }
        }


        public bool fadeIn = true;
        public double fival = 0;
        public double fimover = .05;


        public struct colr
        {
            public int r;
            public int g;
            public int b;
        }

        public void shadePalette(ref colr[] colors)
        {
            for (int i = 0; i < 64; i++)
            {
                colors[i].r = 0;
                colors[i].g = 0;
                colors[i].b = 4 * i;

                colors[64 + i].r = 0;
                colors[64 + i].g = i << 2;
                colors[64 + i].b = 128 - (i << 1);
            }

            for (int i = 128; i < 256; i++)
            {
                colors[i].r = i;
                colors[i].g = i;
                colors[i].b = 255;
            }
        }

        public void standardPalette(ref colr[] colors)
        {
            for (int i = 0; i < 256; i++)
            {
                double red = 1 + Math.Cos(i * Math.PI / 128);
                double grn = 1 + Math.Cos((i - 85) * Math.PI / 128);
                double blu = 1 + Math.Cos((i + 85) * Math.PI / 128);
                colors[i].r = (int)(red * 127) % 256;
                colors[i].g = (int)(grn * 127) % 256;
                colors[i].b = (int)(blu * 127) % 256;
            }
        }

        public void voxelPalette(ref colr[] colors)
        {
            for (int i = 0; i < 256; i++)
            {
                colors[i].r = i >> 1;
                colors[i].g = i >> 1;
                colors[i].b = i;
            }
        }



        public double d2r(double degrees)
        {
            double conversion = 0.1745327; // --> 3.14159 / 180.0;
            return degrees * conversion;
        }

        public void calc_3d(int x, int y, int z, int midx, int midy, double cr1,
          double sr1, out int sx, out int sy, int CameraPosition, int Distance)
        {
            double x1, z1, y1, x2, y2, z2;

            x1 = cr1 * x - sr1 * z;
            z1 = sr1 * x + cr1 * z;

            x2 = cr1 * x1 + sr1 * y;

            y1 = cr1 * y - sr1 * x1;
            y2 = sr1 * z1 + cr1 * y1;

            z2 = cr1 * z1 - sr1 * y1;

            z2 -= CameraPosition;

            sx = Convert.ToInt32((Distance * x2 / z2) + midx);
            sy = Convert.ToInt32((Distance * y2 / z2) + midy);
        }


        private double GetAngle(double X, double Y)
        {
            double GetAngle = 0;
            if (X < 0)
            {
                GetAngle = Math.Atan(Y / X) + Math.PI;
            }
            else if (X > 0)
                GetAngle = Math.Atan(Y / X);
            else
              if (Y < 0)
                GetAngle = 1.5 * Math.PI;
            else
                GetAngle = 0.5 * Math.PI;

            return GetAngle;
        }

        public void loadTexture(Bitmap bmp, out int len, out int wid, out Color[,] c)
        {
            len = bmp.Width; wid = bmp.Height;

            c = new Color[bmp.Width, bmp.Height];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    c[i, j] = bmp.GetPixel(i, j);
                }
            }
        }

        public void loadTexture(Bitmap bmp, out int len, out int wid, out int[,] c)
        {
            len = bmp.Width; wid = bmp.Height;

            c = new int[bmp.Width, bmp.Height];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color r = bmp.GetPixel(i, j);
                    if (r.G == 0 && r.B == 0 && r.R == 0)
                        c[i, j] = 999;
                    else
                    {
                        c[i, j] = r.ToArgb();
                    }
                }
            }
        }

        public void rotate(int x, int y, double angle, out int sx, out int sy)
        {
            //if (x < theWidth / 2) x = -x;
            //if (x > theWidth / 2) x = x/2;

            //x = theWidth / 2 + x;

            //if (y < theHeight / 2) y = -y;
            //if (y > theHeight / 2) y = y / 2;

            //y = theHeight / 2 + y;

            sx = (int)(x * Math.Cos(d2r(angle)) - y * Math.Sin(d2r(angle)));
            sy = (int)(x * Math.Sin(d2r(angle)) + y * Math.Cos(d2r(angle)));
        }

        public void circle(int xCtr, int yCtr, int radius, Color c)
        {
            int x, y, d;

            x = 0;
            y = radius;
            d = 2 * (1 - radius);


            while (y >= 0)
            {
                _fb.SetPixel(xCtr + x, yCtr + y, c);
                _fb.SetPixel(xCtr + x, yCtr - y, c);
                _fb.SetPixel(xCtr - x, yCtr + y, c);
                _fb.SetPixel(xCtr - x, yCtr - y, c);
                if (d + y > 0)
                {
                    y = y - 1;
                    d = d - (2 * y * (_width / _height)) - 1;
                }
                if (x > d)
                {
                    x = x + 1;
                    d = d + (2 * x) + 1;
                }
            }
        }
    }
    public static class ResourceHelper
    {
        public static Bitmap ReadResourceBitmap(string resourceName)
        {
            var assembly = Assembly.GetCallingAssembly();
            var fr1 = assembly.GetManifestResourceNames().First(z => z.Contains(resourceName));

            
            using (Stream stream = assembly.GetManifestResourceStream(fr1))                        
                return (Bitmap)Bitmap.FromStream(stream);                            
        }

        public static string ReadResourceTxt(string resourceName)
        {
            var assembly = Assembly.GetCallingAssembly();
            var fr1 = assembly.GetManifestResourceNames().First(z => z.Contains(resourceName));

            using (Stream stream = assembly.GetManifestResourceStream(fr1))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}

