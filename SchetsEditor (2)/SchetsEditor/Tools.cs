using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{ 
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public class ObjectVorm
    {
        public string naam;
        public string tekst;
        public Point start;
        public Point eind;
        public Rectangle rect;
        public Color kleur;
        public int dikte;

        //volrechthoek en volcirkel
        public void Eigenschap(String n, Color k, Rectangle r)
        {
            naam = n;
            rect = r;
            kleur = k;
        }
        //cirkel en rechthoek
        public void Eigenschap(String n, Color k, Rectangle r, int d)
        {
            naam = n;
            rect = r;
            kleur = k;
            dikte = d;
        }
        //pen en lijn
        public void Eigenschap(String n, Color k, Point p1, Point p2, int d)
        {
            naam = n;
            start = p1;
            eind = p2;
            kleur = k;
            dikte = d;
        }
        //letters
        public void Eigenschap(String n, Color k, Point p1, string t)
        {
            naam = n;
            start = p1;
            tekst = t;
            kleur = k;
        }
        public void Rect(Rectangle xr) //? weg?
        {
            rect = xr;
        }

        public void Toevoeg(SchetsControl s) //figuren aan de lijst toevoegen
        {
            s.lijst.Add(this);
        }

        public void Haalweg(SchetsControl s, int i) //figuren uit de lijst verwijderen 
        {
            s.lijst.RemoveAt(i);
        }
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            startpunt = p;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {
            kwast = new SolidBrush(s.PenKleur);
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = new SizeF(30, 64);
                gr.DrawString(tekst, font, kwast, this.startpunt, StringFormat.GenericTypographic);
                startpunt.X += (int)sz.Width;

                var obj = new ObjectVorm();
                obj.Eigenschap(this.GetType().Name, s.PenKleur, new Point(startpunt.X, startpunt.Y), tekst);
                obj.Toevoeg(s);

                s.Invalidate();
            }
        }

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public ObjectVorm o = new ObjectVorm();

        public Rectangle Punten2Rechthoek(Point p1, Point p2)
        {
            Rectangle r = new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
                                , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y)));

            o.Rect(r);

            return r;
        }
        public static Pen MaakPen(Brush b, int dikte)
        {
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {
            s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);
        }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);

            //toevoegen eigenschappen figuur aan de lijst
            if (this.GetType().Name != "GumTool")
            {
                if (this.GetType().Name == "VolRechthoekTool" || this.GetType().Name == "VolCirkelTool")
                {
                    var obj = new ObjectVorm(); //nieuw object gemaakt anders wordt de andere overschreven

                    obj.Eigenschap(this.GetType().Name, s.PenKleur, Punten2Rechthoek(this.startpunt, p));
                    obj.Toevoeg(s);
                }
                else if (this.GetType().Name == "PenTool" || this.GetType().Name == "LijnTool")
                {
                    var obj = new ObjectVorm();
                    obj.Eigenschap(this.GetType().Name, s.PenKleur, this.startpunt, p, s.penDikte);
                    obj.Toevoeg(s);
                }

                else //rechthoektool en cirkeltool
                {
                    var obj = new ObjectVorm();
                    obj.Eigenschap(this.GetType().Name, s.PenKleur, Punten2Rechthoek(this.startpunt, p), s.penDikte);
                    obj.Toevoeg(s);
                }
            }
            else
            {
                GumTool g = new GumTool();
                g.Gum(p.X, p.Y, s);
            }

            s.Invalidate();
        }

        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(Graphics g, Point p1, Point p2);

        public virtual void Compleet(Graphics g, Point p1, Point p2)
        {
            this.Bezig(g, p1, p2);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "kader"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawRectangle(MaakPen(kwast, 3), Punten2Rechthoek(p1, p2));
        }
    }

    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {
            g.FillRectangle(kwast, Punten2Rechthoek(p1, p2));
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawLine(MaakPen(this.kwast, 3), p1, p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }


    public class CirkelTool : RechthoekTool
    {
        public override string ToString() { return "cirkel"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), Punten2Rechthoek(p1, p2));
        }
    }

    public class VolCirkelTool : RechthoekTool
    {
        public override string ToString() { return "vulCirkel"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, Punten2Rechthoek(p1, p2));
        }
    }


    public class GumTool : TweepuntTool
    {
        public override string ToString() { return "gum"; }

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
        }
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
        }

        public void Gum(int px, int py, SchetsControl s)
        {
            for (int i = 0; i < s.lijst.Count; i++)
            {
                if (s.lijst[i].naam == "LijnTool" || s.lijst[i].naam == "PenTool")
                {

                    double startX = s.lijst[i].start.X;
                    double startY = s.lijst[i].start.Y;
                    double eindX = s.lijst[i].eind.X;
                    double eindY = s.lijst[i].eind.Y;

                    double marge = 8;
                    double varY = eindY - startY;
                    double varX = eindX - startX;

                    //formule "Distance from a point to a line" van Wikipedia
                    //deze formule berekend de korte afstand van een punt naar een lijn

                    double afstand = Math.Abs(varY * px - varX * py + eindX * startY - eindY * startX)
                        / Math.Sqrt(Math.Pow(varY, 2) + Math.Pow(varX, 2));

                    if (afstand <= marge) //als de afstand van de muisklik binnen de marge valt wordt de lijn verwijderd
                    {
                        o.Haalweg(s, i);
                        break;
                    }
                }

                else if (s.lijst[i].naam == "TekstTool")
                {
                    if (px > s.lijst[i].start.X && px < s.lijst[i].start.X + 30 &&
                        py > s.lijst[i].start.Y && py < s.lijst[i].start.Y + 64)
                    {
                        o.Haalweg(s, i);
                        break;
                    }
                }
                else if (s.lijst[i].naam == "VolCirkelTool" || s.lijst[i].naam == "CirkelTool")
                {

                    int straalX = s.lijst[i].rect.Width / 2;
                    int straalY = s.lijst[i].rect.Height / 2;

                    int x = px - s.lijst[i].rect.X - straalX;  
                    int y = py - s.lijst[i].rect.Y - straalY;

                    //formule voor ellipse x^2/a^2 + y^2/b^2 = 1
                    //x is de klik x, a is de x-straal, zelfde geld voor y
                    //als de formule kleiner is dan 1, dan valt een punt binnen de ellipse 

                    double binnenCirkel = Math.Pow(x, 2) / Math.Pow(straalX, 2) + Math.Pow(y, 2) / Math.Pow(straalY, 2);

                    if (s.lijst[i].naam == "CirkelTool")
                    {
                        if (binnenCirkel > 0.90 && binnenCirkel < 1.10) // als er binnen een marge van 5 pixels wordt geklikt wordt gegumd
                        {
                            o.Haalweg(s, i);
                            break;
                        }
                    }
                    else if (s.lijst[i].naam == "VolCirkelTool")
                    {
                        if (binnenCirkel <= 1) //als er binnen de ellipse wordt geklikt wordt er gegumd
                        {
                            o.Haalweg(s, i);
                            break;
                        }
                    }
                }
                else if (s.lijst[i].naam == "RechthoekTool" || s.lijst[i].naam == "VolRechthoekTool")
                {
                    int rectX = s.lijst[i].rect.X;
                    int rectY = s.lijst[i].rect.Y;
                    int rectWidth = s.lijst[i].rect.Width;
                    int rectHeight = s.lijst[i].rect.Height;
                    int marge = 10;

                    //voorwaarde: de x en y van de muisklik vallen binnen het kader van de rechthoek
                    if (px > rectX && px < (rectWidth + rectX) && py > rectY
                        && py < (rectHeight + rectY))
                    {
                        if (s.lijst[i].naam == "VolRechthoekTool") //als er binnen de rechthoek wordt geklikt, wordt gegumd
                        {
                            o.Haalweg(s, i);
                            break;
                        }

                        else if (s.lijst[i].naam == "RechthoekTool")
                        {
                            if ((px > rectX && px < (rectWidth + rectX) && py > rectY //als er binnen een marge van de rand wordt geklikt, wordt gegumd
                                && py < (rectHeight + rectY) && !(px > rectX + marge
                                && px < (rectWidth + rectX) - marge && py > rectY + marge
                                && py < (rectHeight + rectY - marge))))                     
                            {
                                o.Haalweg(s, i);
                                break;
                            }
                        }
                    }
                }
            }
            s.Invalidate();
        }
    }
}
