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
        public int penDikte = 3; //beginwaarde pen dikte
        public List<ObjectVorm> lijst = new List<ObjectVorm>(); //aanmaken lijst met objecten

        public Color PenKleur
        {
            get { return penkleur; }
        }

        public int PenDikte
        {
            get { return penDikte; }
        }
        public Schets Schets
        {
            get { return schets; }
        }

        public SchetsControl()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.tekenLijst;

            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        public void tekenLijst(object o, PaintEventArgs pea) //Tekenen wat op de lijst staat
        {
            string naam;
            schets.NieuweBitmap(); //ervoor zorgen dat de nieuwe tekening geen oude elementen bevat door bitmap opnieuw te maken 

            for (int i = 0; i < lijst.Count; i++)
            {
                naam = lijst[i].naam;

                if (naam == "CirkelTool")
                    schets.TekenCirkel(pea.Graphics, new Pen(lijst[i].kleur, lijst[i].dikte), lijst[i].rect);

                if (naam == "VolCirkelTool")
                    schets.TekenCirkelVol(pea.Graphics, new SolidBrush(lijst[i].kleur), lijst[i].rect);

                if (naam == "RechthoekTool")
                    schets.TekenRecht(pea.Graphics, new Pen(lijst[i].kleur, lijst[i].dikte), lijst[i].rect);

                if (naam == "VolRechthoekTool")
                    schets.TekenRechtVol(pea.Graphics, new SolidBrush(lijst[i].kleur), lijst[i].rect);

                if (naam == "LijnTool")
                    schets.TekenLijn(pea.Graphics, new Pen(lijst[i].kleur, lijst[i].dikte), lijst[i].start, lijst[i].eind);

                if (naam == "PenTool")
                    schets.TekenLijn(pea.Graphics, new Pen(lijst[i].kleur, lijst[i].dikte), lijst[i].start, lijst[i].eind);

                if (naam == "TekstTool")
                    schets.TekenTekst(pea.Graphics, new SolidBrush(lijst[i].kleur), lijst[i].start, lijst[i].tekst);
            }
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
        public void Schoon(object o, EventArgs ea) //Alle getekende figuren wissen
        {
            lijst.Clear();
            schets.SchoonSchets();
            this.Invalidate();
        }

        public void Roteer(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
            
         }

        public void Undo(object o, EventArgs ea) //laatst getekende figuur wissen van de tekening
        {
            try { lijst.RemoveAt(lijst.Count - 1); this.Invalidate(); }
            catch { }
        }
       
        public void VeranderKleur(object obj, EventArgs ea) //verander kleur via menu in MDI-Child 
        {
            string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea) //verander kleur via MDI-Parent 
        {
            string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        public void VeranderDikte(object obj, EventArgs ea) //verander dikte via menu in Child MDI-Child 
        {
            penDikte = int.Parse(((ComboBox)obj).Text);
        }
        public void VeranderDikteViaMenu(object obj, EventArgs ea) //verander dikte via menu in MDI-Parent 
        {
            penDikte = int.Parse(((ToolStripMenuItem)obj).Text);
        }
    }
}
