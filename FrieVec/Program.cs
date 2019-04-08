﻿using Microsoft.VisualBasic;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

/* todo:
-Curves
-Text
-Pixels
*/
namespace FrieVec
{
    public class Form1 : Form
    {
        String Filename = "";
        bool drawing;
        String selected;
        Vector2i startpos;
        Image image;
        Image image2;
        Color selColor = Color.White;
        uint W, H;
        RenderWindow window;
        List<String> cmds;
        String[] commands = new String[1];
        public Form1()
        {

        }
        public void Run()
        {
            Button button1 = new Button();
            button1.Size = new System.Drawing.Size(30, 30);
            button1.Location = new System.Drawing.Point(0, 0);
            button1.Image = System.Drawing.Image.FromFile(@"Assets/Line.png");
            button1.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Controls.Add(button1);
            button1.Click += new EventHandler(sLine);

            Button button2 = new Button();
            button2.Size = new System.Drawing.Size(30, 30);
            button2.Location = new System.Drawing.Point(0, 30);
            button2.Image = System.Drawing.Image.FromFile(@"Assets/floodfill.png");
            button2.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Controls.Add(button2);
            button2.Click += new EventHandler(sFill);

            Button button3 = new Button();
            button3.Size = new System.Drawing.Size(30, 30);
            button3.Location = new System.Drawing.Point(0, 60);
            button3.Image = System.Drawing.Image.FromFile(@"Assets/cp.png");
            button3.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Controls.Add(button3);
            button3.Click += new EventHandler(CP);

            DrawingSurface rendersurface = new DrawingSurface();
            rendersurface.Size = new System.Drawing.Size((int)W, (int)H);
            rendersurface.Location = new System.Drawing.Point(30, 0);
            Controls.Add(rendersurface);

            image = new Image((uint)W, (uint)H, Color.Black);
            Texture tex = new Texture((uint)W, (uint)H);
            Sprite sprite = new Sprite();

            image2 = new Image(W, H, Color.Transparent);
            Texture tex2 = new Texture(W, H);
            Sprite sprite2 = new Sprite();

            for (int i = 1; i < cmds.Count; i++)
            {
                String[] ccommand = cmds[i].Split(',');
                switch (ccommand[0])
                {
                    case "l":
                        DrawLine(int.Parse(ccommand[1]), int.Parse(ccommand[2]), int.Parse(ccommand[3]), int.Parse(ccommand[4]), new Color(Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6])), Convert.ToByte(int.Parse(ccommand[7]))));
                        break;
                    case "f":
                        Fill(int.Parse(ccommand[1]), int.Parse(ccommand[2]), new Color(Convert.ToByte(int.Parse(ccommand[3])), Convert.ToByte(int.Parse(ccommand[4])), Convert.ToByte(int.Parse(ccommand[5]))));
                        break;
                    case "c":
                        DrawCircle(int.Parse(ccommand[1]), int.Parse(ccommand[2]),int.Parse(ccommand[3]),new Color(Convert.ToByte(int.Parse(ccommand[4])), Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6]))));
                        break;
                }
            }

            window = new RenderWindow(rendersurface.Handle);
            window.MouseButtonPressed += Window_MouseButtonPressed;
            Console.WriteLine("Hoi2");
            while (Visible)
            {
                image2 = new Image(W, H, Color.Transparent);
                if (drawing)
                {
                    DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor, "i2");
                }
                Application.DoEvents();
                window.DispatchEvents(); // handle SFML events - NOTE this is still required when SFML is hosted in another window
                tex.Update(image);
                tex2.Update(image2);
                sprite.Texture = tex;
                sprite2.Texture = tex2;
                window.Clear(); // clear our SFML RenderWindow
                window.Draw(sprite);
                window.Draw(sprite2);
                window.Display();                         // display what SFML has drawn to the screen
            }
        }



        [STAThread]
        static void Main()
        {
            Form1 p = new Form1();
            uint W = 400;
            uint H = 400;

            #region veryBadcode
            p.Size = new System.Drawing.Size((int)W + 30, (int)H);
            Button button5 = new Button();
            button5.Size = new System.Drawing.Size(30, 30);
            button5.Location = new System.Drawing.Point(0, 0);
            button5.Image = System.Drawing.Image.FromFile(@"Assets/Folder.png");
            button5.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            p.Controls.Add(button5);
            button5.Click += new EventHandler(p.Load);

            Button button6 = new Button();
            button6.Size = new System.Drawing.Size(30, 30);
            button6.Location = new System.Drawing.Point(0, 30);
            button6.BackgroundImage = System.Drawing.Image.FromFile(@"Assets/new.png");
            button6.BackgroundImageLayout = ImageLayout.Zoom;
            button6.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            p.Controls.Add(button6);
            button6.Click += new EventHandler(p.New);
            p.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);


            p.Show();

            while (p.Filename == "" && p.Visible)
            {
                Application.DoEvents();
            }
            if (!p.Visible)
            {
                return;
            }
            #endregion
            p.Controls.Remove(button5);
            p.Controls.Remove(button6);

            W = (uint)Int32.Parse(p.commands[0].Split(',')[0]);//read resolution
            H = (uint)Int32.Parse(p.commands[0].Split(',')[1]);
            p.Text = p.Filename.Split("\\".ToCharArray())[p.Filename.Split("\\".ToCharArray()).Count() - 1];


            p.cmds = p.commands.ToList();
            p.W = W;
            p.H = H;
            p.Size = new System.Drawing.Size((int)W + 30, (int)H);


            p.Run();
            for (int i = 0; i < p.cmds.Count - 1; i++)
            {
                p.cmds[i] += ";";
            }


            System.IO.File.WriteAllLines(p.Filename, p.cmds);

        }
        private void Fill(int x, int y, Color color)
        {
            Stack<Vector2i> pixels = new Stack<Vector2i>();
            Color target = image.GetPixel((uint)x, (uint)y);
            if (color == target)
                return;
            pixels.Push(new Vector2i(x, y));
            while (pixels.Count > 0)
            {
                Vector2i a = pixels.Pop();
                if (a.X < W && a.X > -1 && a.Y < H && a.Y > -1)
                {
                    if (image.GetPixel((uint)a.X, (uint)a.Y) == target)
                    {
                        image.SetPixel((uint)a.X, (uint)a.Y, color);
                        pixels.Push(new Vector2i(a.X - 1, a.Y));
                        pixels.Push(new Vector2i(a.X + 1, a.Y));
                        pixels.Push(new Vector2i(a.X, a.Y - 1));
                        pixels.Push(new Vector2i(a.X, a.Y + 1));
                    }
                }
            }
        }
        private void DrawLine(int x1, int y1, int x2, int y2, Color color, string iage = "i1")
        {
            if (x2 < 0 || x2 > W || y2 < 0 || y2 > H)
                return;
            int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
            int dy = Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                if (iage == "i1")
                    image.SetPixel((uint)x1, (uint)y1, color);
                if (iage == "i2")
                    image2.SetPixel((uint)x1, (uint)y1, color);
                if (x1 == x2 && y1 == y2) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x1 += sx; }
                if (e2 < dy) { err += dx; y1 += sy; }
            }
        }
        void DrawCircle(int xc, int yc, int r, Color color)
        {
            int x = 0, y = r;
            int d = 3 - 2 * r;
            DRW(xc, yc, x, y,color);
            while (y >= x)
            {

                x++;


                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else
                    d = d + 4 * x + 6;
                DRW(xc, yc, x, y,color);
            }
        }
        void DRW(int xc, int yc, int x, int y,Color Kola)
        {
            image.SetPixel((uint)(xc + x),(uint) (yc + y), selColor);
            image.SetPixel((uint)(xc - x), (uint)(yc + y), selColor);
            image.SetPixel((uint)(xc + x), (uint)(yc - y), selColor);
            image.SetPixel((uint)(xc - x), (uint)(yc - y), selColor);
            image.SetPixel((uint)(xc + y), (uint)(yc + x), selColor);
            image.SetPixel((uint)(xc - y), (uint)(yc + x), selColor);
            image.SetPixel((uint)(xc + y), (uint)(yc - x), selColor);
            image.SetPixel((uint)(xc - y), (uint)(yc - x), selColor);
        }
        private void sLine(object sender, EventArgs e)
        {
            selected = "line";
        }
        private void sFill(object sender, EventArgs e)
        {
            selected = "fill";
        }
        private void CP(object sender, EventArgs e)
        {
            ColorDialog Dio = new ColorDialog();
            Dio.AllowFullOpen = true;
            Dio.ShowDialog();
            selColor = new Color(Dio.Color.R, Dio.Color.G, Dio.Color.B);
        }
        private void Window_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button == SFML.Window.Mouse.Button.Left)
            {
                if (selected == "fill")
                {
                    Fill(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor);
                    cmds.Add("f," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                }
                else if (drawing)
                {
                    drawing = false;
                    DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor);
                    cmds.Add("l," + startpos.X + "," + startpos.Y + "," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                }
                else
                {
                    startpos = SFML.Window.Mouse.GetPosition(window);
                    drawing = true;
                }
            }
            if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                if (drawing)
                {
                    DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor);

                    cmds.Add("l," + startpos.X + "," + startpos.Y + "," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                    startpos = SFML.Window.Mouse.GetPosition(window);
                }
            }
            if (e.Button == SFML.Window.Mouse.Button.Middle)
            {
                drawing = false;
            }
        }
        private void Load(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "FrieVec Image (*.frvc)|*.frvc";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            var t = new Thread((ThreadStart)(() =>
            {
                openFileDialog.ShowDialog();
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
            Filename = openFileDialog.FileName;
            t.Interrupt();
            using (StreamReader fileStream = new StreamReader(Filename))//Read from file
            {
                String FullText = fileStream.ReadToEnd();
                FullText = Regex.Replace(FullText, @"\t|\n|\r", "");
                commands = FullText.Split(';');
                if (commands.Length < 2)
                {

                }
            }
        }
        private void New(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
            {
                saveFileDialog1.Filter = "FrieVec Image|*.frvc";
                saveFileDialog1.Title = "Create an Image File";
                var t = new Thread((ThreadStart)(() =>
                {
                    saveFileDialog1.ShowDialog();
                }));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
                FileStream dulf = System.IO.File.Create(saveFileDialog1.FileName);
                dulf.Close();
                Filename = saveFileDialog1.FileName;
                commands[0] = Interaction.InputBox("Input resolution", "New File", "500,500");
                t.Interrupt();
            }
        }
    }
    public class DrawingSurface : System.Windows.Forms.Control
    {
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            // don't call base.OnPaint(e) to prevent forground painting
            // base.OnPaint(e);
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
            // don't call base.OnPaintBackground(e) to prevent background painting
            //base.OnPaintBackground(pevent);
        }
    }

}



