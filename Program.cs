using System;
using System.Windows.Forms;
using System.Drawing;

namespace Reversi
{
    class Reversi : Form
    {
        //declaraties
        int rows, columns, vakje;
        int clickx, clicky;
        int rijparsetext, kolomparsetext;
        int aantalBlauw, aantalRood, aantalMogelijk;
        bool beurt, help;
        Button helpButton = new Button();
        Button nieuwSpelButton = new Button();
        Label aanZetLabel = new Label();
        Label kolomLabel = new Label();
        Label rijLabel = new Label();
        Label labelAantalBl = new Label();
        Label labelAantalRo = new Label();
        Panel bord = new Panel();
        Label popupLabel = new Label();
        TextBox rijTxtBox = new TextBox();
        TextBox kolomTxtBox = new TextBox();
        Vakjes[,] stand;
        enum Vakjes { Leeg, Blauw, Rood, Mogelijk };

        public Reversi() //constructor
        {
            //Opmaak scherm
            this.Size = new Size(600, 700);
            this.Text = "Reversi";
            this.BackColor = Color.DarkGray;

            //Opmaak help knop
            helpButton.Location = new Point(500, 20);
            helpButton.Size = new Size(40, 40);
            helpButton.Text = "Help";
            helpButton.BackColor = Color.AliceBlue;
            helpButton.Font = new Font("Cambria", 10, FontStyle.Bold);

            //Opmaak nieuw spel knop
            nieuwSpelButton.Location = new Point(50, 20);
            nieuwSpelButton.Size = new Size(80, 40);
            nieuwSpelButton.Text = "Nieuw spel";
            nieuwSpelButton.Font = new Font("Cambria", 10, FontStyle.Bold);
            nieuwSpelButton.BackColor = Color.AliceBlue;

            //Opmaak textbox en label rij
            rijTxtBox.Location = new Point(145, 20);
            rijTxtBox.Size = new Size(25, 25);
            rijLabel.Location = new Point(175, 20);
            rijLabel.Size = new Size(50, 25);
            rijLabel.Text = "Rijen";
            rijLabel.Font = new Font("Cambria", 10, FontStyle.Bold);

            //Opmaak textbox en label kolom
            kolomTxtBox.Location = new Point(145, 40);
            kolomTxtBox.Size = new Size(25, 25);
            kolomLabel.Location = new Point(175, 45);
            kolomLabel.Size = new Size(80, 20);
            kolomLabel.Text = "Kolommen";
            kolomLabel.Font = new Font("Cambria", 10, FontStyle.Bold);

            //Opmaak tekst wie aan zet is
            aanZetLabel.Location = new Point(350, 90);
            aanZetLabel.Font = new Font("Cambria", 15, FontStyle.Bold);
            aanZetLabel.Size = new Size(300, 25);
            aanZetLabel.Text = "Blauw is aan zet";

            //Opmaak speelbord
            bord.Size = new Size(501, 501); //+1 om de laatste zwarte lijn van bord ook te weergeven
            bord.Location = new Point(50, 140);

            //Opmaak tekst: Aantal Blauw
            labelAantalBl.Location = new Point(50, 85);
            labelAantalBl.Font = new Font("Cambria", 10, FontStyle.Bold);
            labelAantalBl.Size = new Size(120, 25);
            labelAantalBl.ForeColor = Color.Blue;

            //Opmaak tekst: Aantal Rood
            labelAantalRo.Location = new Point(50, 105);
            labelAantalRo.Font = new Font("Cambria", 10, FontStyle.Bold);
            labelAantalRo.Size = new Size(120, 20);
            labelAantalRo.ForeColor = Color.DarkRed;

            //Opmaak popup label (als verkeerde waarde in textbox ingevuld)
            popupLabel.Location = new Point(145, 65);
            popupLabel.Size = new Size(120, 20);
            popupLabel.ForeColor = Color.DarkRed;
            popupLabel.Text = "";

            //Beginwaarden
            beurt = true;
            help = false;
            kolomTxtBox.Text = "10"; //grootte beginveld
            rijTxtBox.Text = "10";
            columns = 10;
            rows = 10;
            //Array
            //stand = new Vakjes[rows, columns];

            //Eventhandlers
            bord.Paint += TekenForm;
            nieuwSpelButton.MouseClick += Begin;
            helpButton.MouseClick += HelpFunctie;
            bord.MouseClick += LegSteen;

            Begin(null, null); //start zonder waarden

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
            Controls.Add(popupLabel);
        }

        void Begin(object o, MouseEventArgs mea)
        {
            
            
            try
            {
                rijparsetext = int.Parse(rijTxtBox.Text);
                kolomparsetext = int.Parse(kolomTxtBox.Text);
                popupLabel.Text = "";

                if (rijparsetext > 3 && rijparsetext < 21) //max waarde instellen
                    if (kolomparsetext > 3 && kolomparsetext < 21)
                    {
                        rows = rijparsetext;
                        columns = kolomparsetext;
                    }
                else
                    {
                        kolomTxtBox.Text = "";
                        rijTxtBox.Text = "";
                        popupLabel.Text = "getal tussen 4 en 20!";
                    }
            }

            catch (FormatException) //vang error op, als er bijv. een letter wordt ingevuld.
            { 
              popupLabel.Text = "getal tussen 3 en 25!";
            }

            //de array aanmaken
            stand = new Vakjes[rows, columns]; 

            //lege waardes in array zetten
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                    stand[x, y] = Vakjes.Leeg; 

            //stenen in het midden scherm zetten
            stand[rows / 2 - 1, columns / 2] = Vakjes.Blauw;
            stand[rows / 2, columns / 2] = Vakjes.Rood;

            stand[rows / 2 - 1, columns / 2 - 1] = Vakjes.Rood;
            stand[rows / 2, columns / 2 - 1] = Vakjes.Blauw;

            ZieMogelijkheden(Vakjes.Blauw);
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
            clickx = mea.Location.X / vakje; 
            clicky = mea.Location.Y / vakje;

            //afwisselen acties afhankelijk van wie de beurt heeft

            if ((beurt == true) && (stand[clickx, clicky] == Vakjes.Mogelijk)) //beurt aan blauw
            {
                stand[clickx, clicky] = Vakjes.Blauw; //neergezette steen wordt blauw
                Insluiten(clickx, clicky, Vakjes.Blauw); //ingesloten stenen worden blauw
                
                BeurtWissel();
                AantalStenen();
            }
            else if ((beurt == false) && stand[clickx, clicky] == Vakjes.Mogelijk) //beurt aan rood
            {
                stand[clickx, clicky] = Vakjes.Rood; //neergezette steen wordt rood
                Insluiten(clickx, clicky, Vakjes.Rood); //ingesloten stenen worden rood

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
                ZieMogelijkheden(Vakjes.Rood);
            }
            else
            {
                beurt = true;
                aanZetLabel.Text = "Blauw is aan zet";
                ZieMogelijkheden(Vakjes.Blauw);
            }
        }

        void AantalStenen()
        {
            aantalBlauw = 0;
            aantalRood = 0;
            aantalMogelijk = 0;

            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                {
                    if (stand[x, y] == Vakjes.Blauw)
                        aantalBlauw++;

                    if (stand[x, y] == Vakjes.Rood)
                        aantalRood++;

                    if (stand[x, y] == Vakjes.Mogelijk)
                        aantalMogelijk++;
                }
            
            labelAantalBl.Text = "Aantal Blauw: " + Convert.ToString(aantalBlauw);
            labelAantalRo.Text = "Aantal Rood: " + Convert.ToString(aantalRood);

            if (aantalMogelijk == 0)
                BeurtWissel();

            if ((aantalBlauw + aantalRood) == stand.Length)
                EindStand();
            if ((aantalBlauw + aantalRood + 1) == stand.Length && aantalMogelijk == 0)
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
                aanZetLabel.Text = "Remise!";
        }

        void TekenForm(object o, PaintEventArgs pea)
        {
            //gebruik de grootste van rows/columns om de grootte van de vakjes te bepalen
            if (rows<columns) 
                vakje = bord.Width / columns;
            else
                vakje = bord.Height / rows;

            Pen p = Pens.Black;
            Brush rood = Brushes.DarkRed;
            Brush blauw = Brushes.Blue;

            // teken veld
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                {
                    pea.Graphics.DrawRectangle(p, vakje * x, vakje * y, vakje, vakje); //tekenen bord

                    if (stand[x, y] == Vakjes.Rood)
                        pea.Graphics.FillEllipse(rood, vakje * x, vakje * y, vakje - 1, vakje - 1); //tekenen rode steen

                    if (stand[x, y] == Vakjes.Blauw)
                        pea.Graphics.FillEllipse(blauw, vakje * x, vakje * y, vakje - 1, vakje - 1); //tekenen blauwe steen

                    if (help == true) //tekenen hulp / mogelijkheden
                        if (stand[x, y] == Vakjes.Mogelijk)
                            pea.Graphics.DrawEllipse(p, (vakje * x) + (vakje / 2 - 5), (vakje * y) + (vakje / 2 - 5), 10, 10);
                }
        }


        void ZieMogelijkheden(Vakjes kleur)
        { /*deze methode zorgt dat de mogelijkheden berekend worden door
            de lege en mogelijke vakjes aan de methode valideZet te geven */

            for (int x = 0; x < rows ; x++) 
                for (int y = 0; y < columns; y++) 
                    if (stand[x, y] == Vakjes.Leeg || stand[x, y] == Vakjes.Mogelijk)
                        ValideZet(x, y, kleur);
        }
        

        void Insluiten(int x, int y, Vakjes kleur)
        {   //deze methode checkt bij elke mogelijke richting welke stenen worden ingesloten en kleurt deze vervolgens

            Vakjes andereKleur = Vakjes.Leeg; //beginwaarde

            //zorgen dat voor beide kleuren de mogelijkheden gecheckt worden 
            if (kleur == Vakjes.Rood)
                andereKleur = Vakjes.Blauw;
            if (kleur == Vakjes.Blauw)
                andereKleur = Vakjes.Rood;

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


        void ValideZet(int x, int y, Vakjes kleur)
        {
            Vakjes andereKleur = Vakjes.Leeg; //beginwaarde

            //afhankelijk van wie aan de beurt is de mogelijkheden checken
            if (kleur == Vakjes.Rood)
                andereKleur = Vakjes.Blauw;
            if (kleur == Vakjes.Blauw)
                andereKleur = Vakjes.Rood;

            //mogelijkheden checken in alle richtingen
            if (RichtingCheck(x + 1, y, andereKleur, false, 1, 0)) //Horizontaal links naar rechts
                stand[x, y] = Vakjes.Mogelijk;

            else if (RichtingCheck(x - 1, y, andereKleur, false, -1, 0)) //Horizontaal rechts naar links
                stand[x, y] = Vakjes.Mogelijk;

            else if (RichtingCheck(x, y + 1, andereKleur, false, 0, 1)) //Verticaal boven naar beneden
                stand[x, y] = Vakjes.Mogelijk;

            else if (RichtingCheck(x, y - 1, andereKleur, false, 0, -1)) //Verticaal beneden naar boven
                stand[x, y] = Vakjes.Mogelijk;

            else if (RichtingCheck(x + 1, y + 1, andereKleur, false, 1, 1)) //diagonaal rechts naar boven
                stand[x, y] = Vakjes.Mogelijk;

            else if (RichtingCheck(x - 1, y + 1, andereKleur, false, -1, 1)) //diagonaal links naar boven
                stand[x, y] = Vakjes.Mogelijk;

            else if (RichtingCheck(x - 1, y - 1, andereKleur, false, -1, -1)) //diagonaal links naar beneden
                stand[x, y] = Vakjes.Mogelijk;

            else if (RichtingCheck(x + 1, y - 1, andereKleur, false, 1, -1)) //diagonaal  rechts naar beneden
                stand[x, y] = Vakjes.Mogelijk;

            else stand[x, y] = Vakjes.Leeg; //als in geen van de richtingen een mogelijkheid wordt gevonden blijft het vakje leeg

        }

        bool RichtingCheck(int x, int y, Vakjes kleur, bool gevonden, int dx, int dy)
        {  /*in deze recursieve methode wordt gekeken of een kleur de ander kan insluiten
            gevonden geeft daarbij aan dat een steen van de andere kleur is gevonden */

            if (x > rows - 1 || y > columns - 1 || x < 0 || y < 0) //geen niet bestaande vakjes checken
                return false;
            
            Vakjes steen = stand[x, y];

            if (steen == kleur)
                return RichtingCheck(x + dx, y + dy, kleur, true, dx, dy); //als een steen van de ander op deze plek ligt

            if (steen == Vakjes.Leeg || steen == Vakjes.Mogelijk) // als er geen steen ligt is het geen valide zet of als het vakje al mogelijk is
                return false;

            else return gevonden; //eigen steen is weer gevonden
        }

        bool KleurVeranderenCheck(int x, int y, Vakjes andereKleur, Vakjes kleur, bool veranderbaar, int dx, int dy)
        {  /*in deze recursieve methode wordt gekeken of een kleur de ander kan insluiten
            gevonden om uiteindelijk de kleur te kunnen veranderen */

            if (x > rows - 1 || y > columns - 1 || x < 0 || y < 0) //geen niet bestaande vakjes
                return false;
            
            if (stand[x, y] == andereKleur)
            {
                stand[x, y] = kleur; //de ingesloten steen wordt veranderd in de andere kleur
                return KleurVeranderenCheck(x + dx, y + dy, andereKleur, kleur, true, dx, dy);//als een steen van de ander op deze plek ligt
            }

            if (stand[x, y] == Vakjes.Leeg || stand[x, y] == Vakjes.Mogelijk) // als er geen steen ligt is het geen valide zet of als het vakje al mogelijk is
                return false;

            else return veranderbaar; //eigen steen is weer gevonden
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