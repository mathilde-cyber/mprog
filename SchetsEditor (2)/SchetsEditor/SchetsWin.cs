using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Linq;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size(this.ClientSize.Width - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        public void klikToolMenu(object obj, EventArgs ea)  //waren private
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        public void klikToolButton(object obj, EventArgs ea) //
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        public SchetsWin()
        {
            ISchetsTool[] deTools = { new PenTool()
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    , new CirkelTool()
                                    , new VolCirkelTool()
                                    };

            List<string> deKleuren = new List<string>();
            foreach (PropertyInfo p in typeof(Color).GetProperties())
            {
                if (p.PropertyType.FullName == "System.Drawing.Color")
                    deKleuren.Add(p.Name);
            }

            int[] Diktes = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 20 };

            this.ClientSize = new Size(700, 520);
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.BackColor = Color.White;               //!!

            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
            {
                vast = true;
                huidigeTool.MuisVast(schetscontrol, mea.Location);
            };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                    huidigeTool.MuisDrag(schetscontrol, mea.Location);
            };
            schetscontrol.MouseUp += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                    huidigeTool.MuisLos(schetscontrol, mea.Location);
                vast = false;
            };
            schetscontrol.KeyPress += (object o, KeyPressEventArgs kpea) =>
            {
                huidigeTool.Letter(schetscontrol, kpea.KeyChar);
            };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = true;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren, Diktes);          //!
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren, Diktes);       //!
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        private void maakFileMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Openen", null, this.openen);        //!!
            menu.DropDownItems.Add("Opslaan", null, this.opslaan);            //!!

            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {
                ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(List<string> kleuren, int[] diktes) //!!!
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon);
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer);
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            ToolStripMenuItem submenu1 = new ToolStripMenuItem("Kies dikte");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            foreach (int i in diktes)
                submenu1.DropDownItems.Add(i.ToString(), null, schetscontrol.VeranderDikteViaMenu);
            menu.DropDownItems.Add(submenu);
            menu.DropDownItems.Add(submenu1);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 25 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons(List<string> kleuren, int[] diktes)
        {
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);

            Button b; Label l, ld; ComboBox cbbK, cbbD;
            Button undo, c;
            b = new Button();
            b.Text = "Clear";
            b.Location = new Point(0, 0);
            b.Click += schetscontrol.Schoon;

            paneel.Controls.Add(b);

            c = new Button();
            c.Text = "Rotate";
            c.Location = new Point(80, 0);
            c.Click += schetscontrol.Roteer;
            paneel.Controls.Add(c);

            l = new Label();
            l.Text = "Penkleur:";
            l.Location = new Point(220, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);

            ld = new Label();
            ld.Text = "Pendikte:";
            ld.Location = new Point(400, 3);
            ld.AutoSize = true;
            paneel.Controls.Add(ld);

            undo = new Button();
            undo.Text = "Undo";
            undo.Location = new Point(165, 0);
            undo.Size = new Size(40, 23);
            undo.Click += schetscontrol.Undo;
            paneel.Controls.Add(undo);

            //combobox Kleur
            cbbK = new ComboBox(); cbbK.Location = new Point(270, 0);
            cbbK.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbK.SelectedValueChanged += schetscontrol.VeranderKleur;

            foreach (string k in kleuren)
                cbbK.Items.Add(k);

            cbbK.SelectedIndex = 8;
            paneel.Controls.Add(cbbK);

            //combobox Dikte
            cbbD = new ComboBox(); cbbD.Location = new Point(450, 0);
            cbbD.Size = new Size(40, 20);
            cbbD.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbD.SelectedValueChanged += schetscontrol.VeranderDikte;

            foreach (int d in diktes)
                cbbD.Items.Add(d);

            cbbD.SelectedIndex = 2;
            paneel.Controls.Add(cbbD);
        }

        
        public void opslaan(object o, EventArgs e)
        {
            SaveFileDialog opslaanfile = new SaveFileDialog();
            opslaanfile.Filter = "Tekstfiles|*.txt|Alle files|*.*";
            opslaanfile.Title = "Schets opslaan als...";

            if (opslaanfile.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(opslaanfile.FileName);

                foreach (string s in maakStringlijst(schetscontrol.lijst))
                { sw.Write(s); }

                sw.Close();
            }
        }

        public void openen(object sender, EventArgs e)
        {
            List<string> filelijst = new List<string>();
            OpenFileDialog openfile = new OpenFileDialog();
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openfile.FileName);
                string filetekst = sr.ReadToEnd();

                filelijst = filetekst.Split('\n').ToList();

                sr.Close();

                schetscontrol.lijst = maakObjectLijst(filelijst);
            }
            schetscontrol.Invalidate();
        }

        public List<string> maakStringlijst(List<ObjectVorm> l)
        {
            List<string> stringlijst = new List<string>();
            string naam, kleur, rect, start, eind, tekst, dikte;

            for (int i = 0; i < schetscontrol.lijst.Count; i++)
            {
                naam = schetscontrol.lijst[i].naam;

                //kleur ToString geeft dit terug: "Color [Black]", dit moet ingekort worden
                //naar slechts de kleur "Black"

                kleur = schetscontrol.lijst[i].kleur.ToString();
                string[] kleurarray = kleur.Split(' ');
                kleur = kleurarray[1].Substring(1, kleurarray[1].Length - 2);

                rect = schetscontrol.lijst[i].rect.Location.X.ToString() + " "
                    + schetscontrol.lijst[i].rect.Location.Y.ToString() + " " +
                    schetscontrol.lijst[i].rect.Size.Width.ToString() + " " +
                    schetscontrol.lijst[i].rect.Size.Height.ToString();

                start = schetscontrol.lijst[i].start.X.ToString() + " " +
                    schetscontrol.lijst[i].start.Y.ToString();

                eind = schetscontrol.lijst[i].eind.X.ToString() + " " +
                    schetscontrol.lijst[i].eind.Y.ToString();

                tekst = schetscontrol.lijst[i].tekst;

                dikte = schetscontrol.lijst[i].dikte.ToString();

                if (naam == "VolRechthoekTool" || naam == "RechthoekTool" ||
                    naam == "CirkelTool" || naam == "VolCirkelTool")
                    stringlijst.Add(naam + " " + kleur + " " + rect + " " + dikte + '\n');
                else if (naam == "PenTool" || naam == "LijnTool")
                    stringlijst.Add(naam + " " + kleur + " " + start + " " + eind + " " + dikte + '\n');
                else if (naam == "TekstTool")
                    stringlijst.Add(naam + " " + kleur + " " + start + " " + tekst + '\n');
            }
            return stringlijst;
        }

        public List<ObjectVorm> maakObjectLijst(List<string> stringl)
        {
            List<ObjectVorm> objectlijst = new List<ObjectVorm>();

            foreach (string s in stringl)
            {
                String[] sArray = s.Split(' ');

                if (sArray[0] == "VolRechthoekTool" || sArray[0] == "VolCirkelTool")
                {
                    var obj = new ObjectVorm();

                    obj.Eigenschap(sArray[0], Color.FromName(sArray[1]),
                    new Rectangle(new Point(int.Parse(sArray[2]), int.Parse(sArray[3])),
                    new Size(int.Parse(sArray[4]), int.Parse(sArray[5]))));

                    objectlijst.Add(obj);
                }
                else if (sArray[0] == "RechthoekTool" || sArray[0] == "CirkelTool")
                {
                    var obj = new ObjectVorm();

                    obj.Eigenschap(sArray[0], Color.FromName(sArray[1]),
                    new Rectangle(new Point(int.Parse(sArray[2]), int.Parse(sArray[3])),
                    new Size(int.Parse(sArray[4]), int.Parse(sArray[5]))), int.Parse(sArray[6]));

                    objectlijst.Add(obj);
                }

                else if (sArray[0] == "PenTool" || sArray[0] == "LijnTool")
                {
                    var obj = new ObjectVorm();

                    obj.Eigenschap(sArray[0], Color.FromName(sArray[1]),
                    new Point(int.Parse(sArray[2]), int.Parse(sArray[3])),
                    new Point(int.Parse(sArray[4]), int.Parse(sArray[5])), int.Parse(sArray[6]));

                    objectlijst.Add(obj);
                }

                else if (sArray[0] == "TekstTool")
                {
                    var obj = new ObjectVorm();

                    obj.Eigenschap(sArray[0], Color.FromName(sArray[1]),
                    new Point(int.Parse(sArray[2]), int.Parse(sArray[3])),
                    sArray[4]);

                    objectlijst.Add(obj);
                }
            }
            return objectlijst;
        }
    }
}