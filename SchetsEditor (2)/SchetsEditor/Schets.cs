using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;
        Bitmap nieuw;
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
        }
        public Graphics BitmapGraphics
        {
            get
            {
                return Graphics.FromImage(bitmap);
            }
        }
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                bitmap = nieuw;
            }
        }

        public void NieuweBitmap() //voorkomen dat de bitmap nog tekeningen bevat door een nieuwe bitmap te maken 
        {
            nieuw = new Bitmap(bitmap.Size.Width, bitmap.Size.Height);
            bitmap = nieuw;
        }

        public void TekenRecht(Graphics gr, Pen p, Rectangle r) //tekenen rechthoek
        {
            gr.DrawImage(bitmap, 0, 0);
            gr.DrawRectangle(p, r);
        }

        public void TekenRechtVol(Graphics gr, Brush b, Rectangle r) //tekenen gevulde rechthoek
        {
            gr.DrawImage(bitmap, 0, 0);
            gr.FillRectangle(b, r);
        }

        public void TekenLijn(Graphics gr, Pen p, Point p1, Point p2) //tekenen lijn
        {
            gr.DrawImage(bitmap, 0, 0);
            gr.DrawLine(p, p1, p2);
        }
        public void TekenPen(Graphics gr, Pen p, Point p1, Point p2) //tekeken pen
        {
            gr.DrawImage(bitmap, 0, 0);
            gr.DrawLine(p, p1, p2);
        }

        public void TekenCirkel(Graphics gr, Pen p, Rectangle r) //tekenen cirkel
        {
            gr.DrawImage(bitmap, 0, 0);
            gr.DrawEllipse(p, r);

        }

        public void TekenCirkelVol(Graphics gr, Brush b, Rectangle r) //tekenen gevulde cirkel
        {
            gr.DrawImage(bitmap, 0, 0);
            gr.FillEllipse(b, r);
        }

        public void TekenTekst(Graphics gr, Brush b, Point p1, String t) //tekenen tekst
        {
            gr.DrawImage(bitmap, 0, 0);
            gr.DrawString(t, new Font("Tahoma", 40), b, p1.X, p1.Y);
        }

        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }


       /* public void Weg()
        {
            Graphics g = Graphics.FromImage(bitmap);
            Color c = Color.White;
            g.Clear(c);
        }
        */
        

        public void SchoonSchets() //scherm weer wit maken
        {
            Graphics gr = Graphics.FromImage(bitmap);
            Color c = Color.White;
            gr.Clear(c);
        }
    }
}
