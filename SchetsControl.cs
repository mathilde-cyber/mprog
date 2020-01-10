using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class SchetsControl : UserControl
    {
        private Schets schets;
        private Color penkleur;

        /*public void SchetsVanList(PaintEventArgs pea) //weet niet of dit zo moet. tekenen wat op de lijst staat
        {

            for (int i = 0; i < lijst.Count; i++)
            {
                string naam = lijst[i].Naam;
                if (naam == "Cirkel")
                    Graphics.DrawEllipse();

                if (naam == "VulCirkelTool")
                    Graphics.FillEllipse();

                if (naam == "Rechthoek")
                    Graphics.DrawEllipse();

                if (naam == "VolRechthoek")
                    Graphics.FillRectangle();

                if (naam == "Lijn")
                    Graphics.DrawLine();


            }
        }
        */

        public List<ObjectVorm> lijst = new List<ObjectVorm>();

        public Color PenKleur
        {
            get { return penkleur; }
        }
        public Schets Schets
        {
            get { return schets; }
        }

        //eigen
        public void VoegToe(ObjectVorm a)
        {
            lijst.Add(a);

            foreach (var st in lijst)
                Console.WriteLine(st);
        }

        public SchetsControl()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);

        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);

        }
        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }
        public Graphics MaakBitmapGraphics()
        {
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public void Schoon(object o, EventArgs ea)
        {
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
        }
        public void VeranderKleur(object obj, EventArgs ea)
        {
            string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
    }
}
