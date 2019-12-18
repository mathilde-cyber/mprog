using System;
using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{
    class Reversi : Form
    {
        //declaraties
        int columns, rows;
        Button helpButton = new Button();
        Button nieuwSpelButton = new Button();

        Label aanZetLabel = new Label();
        Label kolomLabel = new Label();
        Label rijLabel = new Label();
        Label labelAantalBl = new Label();
        Label labelAantalRo = new Label();
        

        Panel bord = new Panel();
        TextBox rijTxtBox = new TextBox();
        TextBox kolomTxtBox = new TextBox();
        Vakje[,] stand;
        bool beurt;
        bool help;
       
       
       
        enum Vakje { Leeg, Blauw, Rood, Mogelijk };

        public Reversi() //constructor
        {  
            //Opmaak scherm
            this.Size = new Size(600, 700);
            this.Text = "Reversi";
            this.BackColor = Color.DarkGray;

            //Opmaak help knop
            helpButton.Location = new Point(50, 20);
            helpButton.Size = new Size(60, 40);
            helpButton.Text = "Help";
            helpButton.BackColor = Color.AliceBlue;
            helpButton.Font = new Font("Cambria", 10, FontStyle.Bold);

            //Opmaak nieuw spel knop
            nieuwSpelButton.Location = new Point(125, 20);
            nieuwSpelButton.Size = new Size(80, 40);
            nieuwSpelButton.Text = "Nieuw spel";
            nieuwSpelButton.Font = new Font("Cambria", 10, FontStyle.Bold);
            nieuwSpelButton.BackColor = Color.AliceBlue;

            //Opmaak txtbox en label rij
            rijTxtBox.Location = new Point(400, 20);
            rijTxtBox.Size = new Size(25, 25);
            rijLabel.Location = new Point(435, 20);
            rijLabel.Size = new Size(50, 25);
            rijLabel.Text = "Rij";
            rijLabel.Font = new Font("Cambria", 10, FontStyle.Bold);

            //Opmaak textbox en label kolom
            kolomTxtBox.Location = new Point(400, 55);
            kolomTxtBox.Size = new Size(25, 25);
            kolomLabel.Location = new Point(435, 60);
            kolomLabel.Size = new Size(50, 25);
            kolomLabel.Text = "Kolom";
            kolomLabel.Font = new Font("Cambria", 10, FontStyle.Bold);

            //opmaak en plaatsing tekst aan zet
            aanZetLabel.Location = new Point(50, 80);
            aanZetLabel.Font = new Font("Cambria", 15, FontStyle.Bold);
            aanZetLabel.Size = new Size(200, 25);
            aanZetLabel.Text = "Blauw is aan zet";

            //Opmaak speelbord
            bord.Size = new Size(501, 501); //+1 om de laatste zwarte lijn van bord ook te weergeven
            bord.Location = new Point(50, 150);

            //Opmaak tekst: Aantal Blauw
            labelAantalBl.Location = new Point(220, 20);
            labelAantalBl.Font = new Font("Cambria", 10, FontStyle.Bold);
            labelAantalBl.Size = new Size(150, 25);
            labelAantalBl.ForeColor = Color.Blue;
            
            
            //Opmaak tekst: Aantal Rood
            labelAantalRo.Location = new Point(220, 50);
            labelAantalRo.Font = new Font("Cambria", 10, FontStyle.Bold);
            labelAantalRo.Size = new Size(150, 25);
            labelAantalRo.ForeColor = Color.Red;
            

            //beginwaarden
            beurt = true;
            help = false;
            
            

            //Array
            stand = new Vakje[rows, columns];

            //eventhandlers
            bord.Paint += TekenForm;
            nieuwSpelButton.MouseClick += begin;
            helpButton.MouseClick += HelpFunctie;
            bord.MouseClick += legSteen;

            //beginscherm, grootte beginveld
            kolomTxtBox.Text = "10";
            rijTxtBox.Text = "10";
            labelAantalBl.Text = "Aantal Blauw: 2 ";
            labelAantalRo.Text = "Aantal Rood: 2 ";

            begin(null, null);

            //toevoegen
            Controls.Add(bord);
            Controls.Add(nieuwSpelButton);
            Controls.Add(helpButton);
            Controls.Add(aanZetLabel);
            Controls.Add(rijTxtBox);
            Controls.Add(kolomTxtBox);
            Controls.Add(rijLabel);
            Controls.Add(kolomLabel);
            Controls.Add(labelAantalBl);
            Controls.Add(labelAantalRo);

        }


        void begin(object o, MouseEventArgs mea)
        {

            columns = int.Parse(kolomTxtBox.Text); //max waarde instellen
            rows = int.Parse(rijTxtBox.Text);

            stand = new Vakje[rows, columns]; //lege waardes in array zetten
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    stand[x, y] = Vakje.Leeg;
                }
            }

            //stenen in het midden scherm
            stand[rows / 2 - 1, columns / 2] = Vakje.Blauw;
            stand[rows / 2, columns / 2] = Vakje.Rood;

            stand[rows / 2 - 1, columns / 2 - 1] = Vakje.Rood;
            stand[rows / 2, columns / 2 - 1] = Vakje.Blauw;

            zieMogelijkheden(Vakje.Blauw);


            labelAantalBl.Text = "Aantal blauw: 2 ";
            labelAantalRo.Text = "Aantal Rood: 2 ";

            bord.Invalidate();


            

        }





        void HelpFunctie(object o, MouseEventArgs mea)
        {
            
            help = !help;
            bord.Invalidate();
           
        }


        void legSteen(object o, MouseEventArgs mea) //na klik wordt steen op goede plek geplaatst
        {
            int clickx;
            int clicky;

            clickx = mea.Location.X / 50; // variabele maken?
            clicky = mea.Location.Y / 50;

            //afwisselen steenkleur afhankelijk van wie de beurt heeft
            
            if ((beurt == true) && (stand[clickx,clicky] == Vakje.Mogelijk))
            {
                stand[clickx, clicky] = Vakje.Blauw; //steen wordt rood
                Insluiten(clickx, clicky, Vakje.Blauw);
                beurt = false;
                zieMogelijkheden(Vakje.Rood);
                

                
                aanZetLabel.Text = "Rood is aan zet";  
                AantalStenen();

            }
            else if ((beurt == false) && stand[clickx, clicky] == Vakje.Mogelijk)
            {
                stand[clickx, clicky] = Vakje.Rood; //steen wordt blauw
                Insluiten(clickx, clicky, Vakje.Rood);
                beurt = true;
                zieMogelijkheden(Vakje.Blauw);
                


                aanZetLabel.Text = "Blauw is aan zet";
                AantalStenen();

            }



            bord.Invalidate();
        }

        void AantalStenen() 
        {
            
            int aantalBlauw = 0;
            int aantalRood = 0;

            for (int x = 0; x < stand.GetLength(0); x++)
            {
                for (int y = 0; y < stand.GetLength(1); y++)
                {
                    if (stand[x,y] == Vakje.Blauw) 
                        aantalBlauw++;

                    if (stand[x, y] == Vakje.Rood)
                        aantalRood++;

                }
            }


            labelAantalBl.Text = "Aantal Blauw: " + Convert.ToString(aantalBlauw);
            labelAantalRo.Text = "Aantal Rood: " + Convert.ToString(aantalRood);

            bord.Invalidate();


        }


        void TekenForm(object o, PaintEventArgs pea)
        {
            int x;
            int y;

            // teken veld 
            Pen p = Pens.Black;
            for (x = 0; x < rows; x++)
            {
                for (y = 0; y < columns; y++)
                {
                    pea.Graphics.DrawRectangle(p, 50 * x, 50 * y, 50, 50); //tekenen bord

                    if (stand[x, y] == Vakje.Rood)
                    {
                        Brush b = Brushes.Red;
                        pea.Graphics.FillEllipse(b, x * 50, y * 50, 50, 50); //tekenen rode steen
                    }

                    if (stand[x, y] == Vakje.Blauw)
                    {
                        Brush b2 = Brushes.Blue;
                        pea.Graphics.FillEllipse(b2, x * 50, y * 50, 50, 50); //tekenen blauwe steen
                    }


                    if (help == true)
                        if (stand[x, y] == Vakje.Mogelijk)
                        {
                        
                            pea.Graphics.DrawEllipse(p, (x * 50)+ 20 , (y * 50) + 20, 10, 10); //tekenen mogelijkheden

                        }
                }
            }
        }




        


        void zieMogelijkheden(Vakje kleur)
        { /*deze methode zorgt dat de mogelijkheden uiteindelijk afgebeeld worden door 
            de lege vakjes en mogelijk vakjes in de methode valideZet te gooien */

            for (int x = 0; x < stand.GetLength(0); x++)
            {
                for (int y = 0; y < stand.GetLength(1); y++)
                {

                    if (stand[x, y] == Vakje.Leeg || stand[x,y] == Vakje.Mogelijk)
                        ValideZet(x, y, kleur);

                }

            }

        }



        void Insluiten(int x, int y, Vakje kleur)
        {
            //hier een methode schrijven om het insluiten te fixen 
            //alle richtingen weer checken 
            //

            

            Vakje andereKleur = Vakje.Leeg; //beginwaarde

            //zorgen dat voor beide kleuren de mogelijkheden gecheckt worden 
            if (kleur == Vakje.Rood)
                andereKleur = Vakje.Blauw;
            if (kleur == Vakje.Blauw)
                andereKleur = Vakje.Rood;

            //
            if (KleurVeranderenCheck(x + 1, y, andereKleur, false, 1, 0)) //Horizontaal links naar rechts
                stand[x + 1, y] = kleur;


            else if (KleurVeranderenCheck(x - 1, y, andereKleur, false, -1, 0)) //Horizontaal rechts naar links
                stand[x - 1, y] = kleur;


            else if (KleurVeranderenCheck(x, y + 1, andereKleur, false, 0, 1)) //Verticaal boven naar beneden
                stand[x, y + 1] = kleur;


            else if (KleurVeranderenCheck(x, y - 1, andereKleur, false, 0, -1)) //Verticaal beneden naar boven 
                stand[x, y - 1] = kleur;


            else if (KleurVeranderenCheck(x + 1, y + 1, andereKleur, false, 1, 1)) //diagonaal rechts naar boven
                stand[x + 1, y + 1] = kleur;


            else if (KleurVeranderenCheck(x - 1, y + 1, andereKleur, false, -1, 1)) //diagonaal links naar boven 
                stand[x - 1, y + 1] = kleur;

            else if (KleurVeranderenCheck(x - 1, y - 1, andereKleur, false, -1, -1)) //diagonaal links naar beneden
                stand[x - 1, y - 1] = kleur;

            else if (KleurVeranderenCheck(x + 1, y - 1, andereKleur, false, 1, -1)) //diagonaal  rechts naar beneden 
                stand[x + 1, y - 1] = kleur;


        }



        void ValideZet(int x, int y, Vakje kleur)

        {
            Vakje andereKleur = Vakje.Leeg; //beginwaarde

            //zorgen dat voor beide kleuren de mogelijkheden gecheckt worden 
            if (kleur == Vakje.Rood)
                andereKleur = Vakje.Blauw;
            if (kleur == Vakje.Blauw)
                andereKleur = Vakje.Rood;

            //mogelijk checken horizontaal, verticaal en diagonaal
            if (RichtingCheck(x + 1, y, andereKleur, false, 1, 0)) //Horizontaal links naar rechts
                stand[x, y] = Vakje.Mogelijk;

            else if (RichtingCheck(x - 1, y, andereKleur, false, -1, 0)) //Horizontaal rechts naar links
                stand[x, y] = Vakje.Mogelijk;

            else if (RichtingCheck(x, y + 1, andereKleur, false, 0, 1)) //Verticaal boven naar beneden
                stand[x, y] = Vakje.Mogelijk;

            else if (RichtingCheck(x, y - 1, andereKleur, false, 0, -1)) //Verticaal beneden naar boven 
                stand[x, y] = Vakje.Mogelijk;

            else if (RichtingCheck(x + 1, y + 1, andereKleur, false, 1, 1)) //diagonaal rechts naar boven
                stand[x, y] = Vakje.Mogelijk;

            else if (RichtingCheck(x - 1, y + 1, andereKleur, false, -1, 1)) //diagonaal links naar boven 
                stand[x, y] = Vakje.Mogelijk;

            else if (RichtingCheck(x - 1, y - 1, andereKleur, false, -1, -1)) //diagonaal links naar beneden
                stand[x, y] = Vakje.Mogelijk;

            else if (RichtingCheck(x + 1, y - 1, andereKleur, false, 1, -1)) //diagonaal  rechts naar beneden 
                stand[x, y] = Vakje.Mogelijk;

            else stand[x, y] = Vakje.Leeg; //als in geen van de richtingen een mogelijkheid wordt gevonden blijft het vakje leeg

        }


       
        bool RichtingCheck(int x, int y, Vakje kleur, bool gevonden, int dx, int dy)
        {  /*in deze recursieve methode wordt gekeken of een kleur de ander kan insluiten
            gevonden geeft daarbij aan dat een steen van de andere kleur is gevonden */

            if (x > stand.GetLength(0) - 1 || y > stand.GetLength(1) - 1 || x < 0 || y < 0) //geen niet bestaande vakjes
            {
                return false;
            }
            Vakje steen = stand[x, y];

            if (steen == kleur)
                return RichtingCheck(x + dx, y + dy, kleur, true, dx, dy); //als een steen van de ander op deze plek ligt

            else if (steen == Vakje.Leeg || steen == Vakje.Mogelijk) // als er geen steen ligt is het geen valide zet of als het vakje al mogelijk is
                return false;

            else return gevonden; //eigen steen is weer gevonden 

        }

        bool KleurVeranderenCheck(int x, int y, Vakje kleur, bool gevonden, int dx, int dy)
        {  /*in deze recursieve methode wordt gekeken of een kleur de ander kan insluiten
            gevonden om uiteindelijk de kleur te kunnen veranderen */

      

            if (x > stand.GetLength(0) - 1 || y > stand.GetLength(1) - 1 || x < 0 || y < 0) //geen niet bestaande vakjes
            {
                return false;
            }
            Vakje steen = stand[x, y];

            if (steen == kleur)
                return KleurVeranderenCheck(x + dx, y + dy, kleur, true, dx, dy);//als een steen van de ander op deze plek ligt
            //iets veranderen om te checken of de kleur veranderd kan worden 


            else if (steen == Vakje.Leeg || steen == Vakje.Mogelijk) // als er geen steen ligt is het geen valide zet of als het vakje al mogelijk is
                return false;

            else return gevonden; //eigen steen is weer gevonden 

        }

    }


    class MainClass
    {
        public static void Main()
        {
            Application.Run(new Reversi());
        }
    }
}
