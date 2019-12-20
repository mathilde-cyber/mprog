using System;
using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{
    class Reversi : Form
    {
        //declaraties
        int columns, rows, vakjex, vakjey;
        int aantalBlauw, aantalRood, aantalMogelijk;
        int clickx, clicky;
        
        bool beurt, help;
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
            rijTxtBox.Location = new Point(360, 20);
            rijTxtBox.Size = new Size(25, 25);
            rijLabel.Location = new Point(390, 20);
            rijLabel.Size = new Size(50, 25);
            rijLabel.Text = "Rijen";
            rijLabel.Font = new Font("Cambria", 10, FontStyle.Bold);

            //Opmaak textbox en label kolom
            kolomTxtBox.Location = new Point(360, 40);
            kolomTxtBox.Size = new Size(25, 25);
            kolomLabel.Location = new Point(390, 40);
            kolomLabel.Size = new Size(65, 25);
            kolomLabel.Text = "Kolommen";
            kolomLabel.Font = new Font("Cambria", 10, FontStyle.Bold);

            //opmaak en plaatsing tekst aan zet
            aanZetLabel.Location = new Point(50, 80);
            aanZetLabel.Font = new Font("Cambria", 15, FontStyle.Bold);
            aanZetLabel.Size = new Size(300, 25);
            aanZetLabel.Text = "Blauw is aan zet";

            //Opmaak speelbord
            bord.Size = new Size(501, 501); //+1 om de laatste zwarte lijn van bord ook te weergeven
            bord.Location = new Point(50, 150);

            //Opmaak tekst: Aantal Blauw
            labelAantalBl.Location = new Point(220, 20);
            labelAantalBl.Font = new Font("Cambria", 10, FontStyle.Bold);
            labelAantalBl.Size = new Size(120, 25);
            labelAantalBl.ForeColor = Color.Blue;

            //Opmaak tekst: Aantal Rood
            labelAantalRo.Location = new Point(220, 40);
            labelAantalRo.Font = new Font("Cambria", 10, FontStyle.Bold);
            labelAantalRo.Size = new Size(120, 20);
            labelAantalRo.ForeColor = Color.DarkRed;

            //beginwaarden
            beurt = true;
            help = false;
            kolomTxtBox.Text = "10"; //grootte beginveld
            rijTxtBox.Text = "10";

            //Array
            stand = new Vakje[rows, columns];

            //eventhandlers
            bord.Paint += TekenForm;
            nieuwSpelButton.MouseClick += Begin;
            helpButton.MouseClick += HelpFunctie;
            bord.MouseClick += LegSteen;

            Begin(null, null);

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

        void Begin(object o, MouseEventArgs mea)
        {
            try
            {
                int kolomtext = int.Parse(kolomTxtBox.Text);
                int rijtext = int.Parse(rijTxtBox.Text);

                if (int.Parse(kolomTxtBox.Text) > 3 && int.Parse(kolomTxtBox.Text) < 25)
                    if (int.Parse(rijTxtBox.Text) > 3 && int.Parse(rijTxtBox.Text) < 25)
                    {
                        columns = int.Parse(kolomTxtBox.Text); //max waarde instellen
                        rows = int.Parse(rijTxtBox.Text);
                    }
            }
            catch (FormatException e)
            { Console.WriteLine(e + "voer nummer in"); }

            
            
            stand = new Vakje[rows, columns]; //lege waardes in array zetten
            
            
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < rows; y++)
                    stand[x, y] = Vakje.Leeg;

            //stenen in het midden scherm
            stand[rows / 2 - 1, columns / 2] = Vakje.Blauw;
            stand[rows / 2, columns / 2] = Vakje.Rood;

            stand[rows / 2 - 1, columns / 2 - 1] = Vakje.Rood;
            stand[rows / 2, columns / 2 - 1] = Vakje.Blauw;

            ZieMogelijkheden(Vakje.Blauw);

            AantalStenen();
            bord.Invalidate();
        }


        void HelpFunctie(object o, MouseEventArgs mea)
        {
            help = !help;
            bord.Invalidate();
        }

        void LegSteen(object o, MouseEventArgs mea) //na klik wordt steen op goede plek geplaatst
        {
            clickx = mea.Location.X / vakjex; 
            clicky = mea.Location.Y / vakjey;

            //afwisselen acties afhankelijk van wie de beurt heeft

            if ((beurt == true) && (stand[clickx, clicky] == Vakje.Mogelijk)) //beurt aan blauw
            {
                stand[clickx, clicky] = Vakje.Blauw; //neergezette steen wordt blauw
                Insluiten(clickx, clicky, Vakje.Blauw); //ingesloten stenen worden blauw
                
                BeurtWissel();
                AantalStenen();
            }
            else if ((beurt == false) && stand[clickx, clicky] == Vakje.Mogelijk) //beurt aan rood
            {
                stand[clickx, clicky] = Vakje.Rood; //neergezette steen wordt rood
                Insluiten(clickx, clicky, Vakje.Rood); //ingesloten stenen worden rood

                BeurtWissel();
                AantalStenen();
            }

            bord.Invalidate();
        }

        void BeurtWissel()
        {
            if (beurt == true)
            {
                beurt = false;
                aanZetLabel.Text = "Rood is aan zet";
                ZieMogelijkheden(Vakje.Rood);
            }
            else
            {
                beurt = true;
                aanZetLabel.Text = "Blauw is aan zet";
                ZieMogelijkheden(Vakje.Blauw);
            }
        }

        void AantalStenen()
        {
            aantalBlauw = 0;
            aantalRood = 0;
            aantalMogelijk = 0;

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    if (stand[x, y] == Vakje.Blauw)
                        aantalBlauw++;

                    if (stand[x, y] == Vakje.Rood)
                        aantalRood++;

                    if (stand[x, y] == Vakje.Mogelijk)
                        aantalMogelijk++;
                }
            }

            labelAantalBl.Text = "Aantal Blauw: " + Convert.ToString(aantalBlauw);
            labelAantalRo.Text = "Aantal Rood: " + Convert.ToString(aantalRood);

            if (aantalMogelijk == 0)
                BeurtWissel();

            if ((aantalBlauw + aantalRood) == stand.Length)
                EindStand();

            bord.Invalidate();
        }

        void EindStand()
        {
            if (aantalBlauw < aantalRood)
                aanZetLabel.Text = "Rood heeft gewonnen!";


            else if (aantalRood < aantalBlauw)
                aanZetLabel.Text = "Blauw heeft gewonnen!";


            else if (aantalRood == aantalBlauw)
                aanZetLabel.Text = "Remise";
        }

        void TekenForm(object o, PaintEventArgs pea)
        {
            int x, y;

            vakjex = bord.Width / rows;
            vakjey = bord.Height / columns;
            Pen p = Pens.Black;
            Brush rood = Brushes.Red;
            Brush blauw = Brushes.Blue;

            // teken veld
            for (x = 0; x < rows; x++)
            {
                for (y = 0; y < columns; y++)
                {
                    pea.Graphics.DrawRectangle(p, vakjex * x, vakjey * y, vakjex, vakjey); //tekenen bord

                    if (stand[x, y] == Vakje.Rood)
                        pea.Graphics.FillEllipse(rood, vakjex * x, vakjey * y, vakjex, vakjey); //tekenen rode steen

                    if (stand[x, y] == Vakje.Blauw)
                        pea.Graphics.FillEllipse(blauw, vakjex * x, vakjey * y, vakjex, vakjey); //tekenen blauwe steen

                    if (help == true) //tekenen mogelijkheden
                        if (stand[x, y] == Vakje.Mogelijk)
                            pea.Graphics.DrawEllipse(p, (vakjex * x) + vakjex / 3, (vakjey * y) + vakjey / 3, 10, 10);
                }
            }
        }


        void ZieMogelijkheden(Vakje kleur)
        { /*deze methode zorgt dat de mogelijkheden uiteindelijk afgebeeld worden door
            de lege vakjes en mogelijk vakjes in de methode valideZet te gooien */

            for (int x = 0; x < rows ; x++) //stand.GetLength(0)
                for (int y = 0; y < columns; y++) //stand.GetLength(1)
                    if (stand[x, y] == Vakje.Leeg || stand[x, y] == Vakje.Mogelijk)
                        ValideZet(x, y, kleur);
        }

        void Insluiten(int x, int y, Vakje kleur)
        {
            //hier een methode schrijven om het insluiten te fixen
            //alle richtingen weer checken

            kleurVeranderMogelijk(x, y, kleur);
        }

        void kleurVeranderMogelijk(int x, int y, Vakje kleur)
        {   //deze methode checkt bij elke mogelijke richting welke stenen worden ingesloten en kleurt deze vervolgens

            Vakje andereKleur = Vakje.Leeg; //beginwaarde

            //zorgen dat voor beide kleuren de mogelijkheden gecheckt worden 
            if (kleur == Vakje.Rood)
                andereKleur = Vakje.Blauw;
            if (kleur == Vakje.Blauw)
                andereKleur = Vakje.Rood;

            //mogelijk checken horizontaal, verticaal en diagonaal
            if (RichtingCheck(x + 1, y, andereKleur, false, 1, 0)) //Horizontaal links naar rechts
                KleurVeranderenCheck(x + 1, y, andereKleur, kleur, false, 1, 0);

            if (RichtingCheck(x - 1, y, andereKleur, false, -1, 0)) //Horizontaal rechts naar links
                KleurVeranderenCheck(x - 1, y, andereKleur, kleur, false, -1, 0);

            if (RichtingCheck(x, y + 1, andereKleur, false, 0, 1)) //Verticaal boven naar beneden
                KleurVeranderenCheck(x, y + 1, andereKleur, kleur, false, 0, 1);

            if (RichtingCheck(x, y - 1, andereKleur, false, 0, -1)) //Verticaal beneden naar boven 
                KleurVeranderenCheck(x, y - 1, andereKleur, kleur, false, 0, -1);

            if (RichtingCheck(x + 1, y + 1, andereKleur, false, 1, 1)) //diagonaal rechts naar boven
                KleurVeranderenCheck(x + 1, y + 1, andereKleur, kleur, false, 1, 1);

            if (RichtingCheck(x - 1, y + 1, andereKleur, false, -1, 1)) //diagonaal links naar boven 
                KleurVeranderenCheck(x - 1, y + 1, andereKleur, kleur, false, -1, 1);

            if (RichtingCheck(x - 1, y - 1, andereKleur, false, -1, -1)) //diagonaal links naar beneden
                KleurVeranderenCheck(x - 1, y - 1, andereKleur, kleur, false, -1, -1);

            if (RichtingCheck(x + 1, y - 1, andereKleur, false, 1, -1)) //diagonaal  rechts naar beneden 
                KleurVeranderenCheck(x + 1, y - 1, andereKleur, kleur, false, 1, -1);
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

            if (x > rows - 1 || y > columns - 1 || x < 0 || y < 0) //geen niet bestaande vakjes checken
            {
                return false;
            }
            Vakje steen = stand[x, y];

            if (steen == kleur)
                return RichtingCheck(x + dx, y + dy, kleur, true, dx, dy); //als een steen van de ander op deze plek ligt

            if (steen == Vakje.Leeg || steen == Vakje.Mogelijk) // als er geen steen ligt is het geen valide zet of als het vakje al mogelijk is
                return false;

            else return gevonden; //eigen steen is weer gevonden
        }

        bool KleurVeranderenCheck(int x, int y, Vakje andereKleur, Vakje kleur, bool veranderbaar, int dx, int dy)
        {  /*in deze recursieve methode wordt gekeken of een kleur de ander kan insluiten
            gevonden om uiteindelijk de kleur te kunnen veranderen */

            if (x > rows - 1 || y > columns - 1 || x < 0 || y < 0) //geen niet bestaande vakjes
                return false;
            
            if (stand[x, y] == andereKleur)
            {
                stand[x, y] = kleur; //de ingesloten steen wordt veranderd in de andere kleur
                return KleurVeranderenCheck(x + dx, y + dy, andereKleur, kleur, true, dx, dy);//als een steen van de ander op deze plek ligt
            }

            if (stand[x, y] == Vakje.Leeg || stand[x, y] == Vakje.Mogelijk) // als er geen steen ligt is het geen valide zet of als het vakje al mogelijk is
                return false;

            else
                return veranderbaar; //eigen steen is weer gevonden
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