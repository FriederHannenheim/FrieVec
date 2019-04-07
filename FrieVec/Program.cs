using Microsoft.VisualBasic;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace FrieVec
{
    class Program
    {
        Color selColor = Color.Black;
        public List<String> cmds;
        bool Drawing;
        Vector2i startPoint;
        String selected = "";
        String hovering = "";
        RenderWindow window;
        Image image;
        Image image2;
        int w, h;
        public void Run(uint W, uint H, String[] commands)
        {
            cmds = commands.ToList();
            w = (int)W;
            h = (int)H;
            window = new RenderWindow(new SFML.Window.VideoMode(W + 30, H), "FrieVec");
            image = new Image(W, H, Color.Black);
            Texture tex = new Texture(W, H);
            Sprite sprite = new Sprite();
            sprite.Position = new Vector2f(30, 0);

            image2 = new Image(W, H, Color.Transparent);
            Texture tex2 = new Texture(W, H);
            Sprite sprite2 = new Sprite();
            sprite2.Position = new Vector2f(30, 0);

            #region Line
            RectangleShape Slined = new RectangleShape(new Vector2f(30, 30));
            Slined.FillColor = new Color(200, 200, 200);
            RectangleShape Sline = new RectangleShape(new Vector2f(45, 3));
            Sline.Position = new Vector2f(0, -3);
            Sline.Rotation = 45;
            #endregion
            #region Fill
            RectangleShape Slaned = new RectangleShape(new Vector2f(30, 30));
            Slaned.FillColor = new Color(200, 200, 200);
            Slaned.Position = new Vector2f(0, 30);


            Texture Mussolini = new Texture(@"Assets/floodfill.png");
            Sprite Stalin = new Sprite();
            Stalin.Texture = Mussolini;
            Stalin.Position = new Vector2f(0, 30);
            Stalin.Scale = new Vector2f(0.7f, 0.7f);
            #endregion
            #region Colorpicker
            RectangleShape Cpf = new RectangleShape(new Vector2f(30, 30));
            Cpf.FillColor = new Color(200, 200, 200);
            Cpf.Position = new Vector2f(0, 60);
            Texture Cp = new Texture(@"Assets/cp.png");
            Sprite Cps = new Sprite();
            Cps.Texture = Cp;
            Cps.Position = new Vector2f(3.5f, 63.5f);
            Cps.Scale = new Vector2f(0.05f, 0.05f);
            #endregion

            RectangleShape Toolbar = new RectangleShape(new Vector2f(1000, 500));
            Toolbar.FillColor = new Color(230, 230, 230);
            Toolbar.Size = new Vector2f(30, (int)H);


            for (int i = 1; i < commands.Length; i++)
            {
                String[] ccommand = commands[i].Split(',');
                switch (ccommand[0])
                {
                    case "l":
                        DrawLine(int.Parse(ccommand[1]), int.Parse(ccommand[2]), int.Parse(ccommand[3]), int.Parse(ccommand[4]), new Color(Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6])), Convert.ToByte(int.Parse(ccommand[7]))));
                        break;
                    case "f":
                        Fill(int.Parse(ccommand[1]), int.Parse(ccommand[2]), new Color(Convert.ToByte(int.Parse(ccommand[3])), Convert.ToByte(int.Parse(ccommand[4])), Convert.ToByte(int.Parse(ccommand[5]))));
                        break;
                }
            }
            try
            {

            }
            catch
            {
                throw new Exception("Corrupted file");

            }

            window.MouseButtonPressed += Window_MouseButtonPressed;
            selected = "line";
            while (window.IsOpen)
            {
                image2 = new Image(W, H, Color.Transparent);
                Slaned.FillColor = new Color(200, 200, 200);
                Slined.FillColor = new Color(200, 200, 200);
                Cpf.FillColor = new Color(200, 200, 200);
                hovering = "";

                FloatRect rect = Slined.GetGlobalBounds();
                FloatRect rect2 = Slaned.GetGlobalBounds();
                FloatRect rect3 = Cpf.GetGlobalBounds();
                if (rect.Contains(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y))
                {
                    Slined.FillColor = new Color(160, 160, 160);
                    hovering = "line";
                }
                if (rect2.Contains(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y))
                {
                    Slaned.FillColor = new Color(160, 160, 160);
                    hovering = "fill";
                }
                if (rect3.Contains(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y))
                {
                    Cpf.FillColor = new Color(160, 160, 160);
                    hovering = "CP";
                }
                if (sprite.GetGlobalBounds().Contains(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y))
                {
                    hovering = "canvas";
                }
                if (selected != "fill")
                    Slined.FillColor = new Color(100, 100, 100);
                else if (selected != "line")
                    Slaned.FillColor = new Color(100, 100, 100);
                else if (selected != "CP")
                    Cpf.FillColor = new Color(100, 100, 100);
                if (Drawing)
                {
                    DrawLine(startPoint.X, startPoint.Y, SFML.Window.Mouse.GetPosition(window).X - 30, SFML.Window.Mouse.GetPosition(window).Y, selColor, "i2");
                }


                window.DispatchEvents();
                window.Closed += new EventHandler(OnClose);

                tex.Update(image);
                tex2.Update(image2);
                sprite.Texture = tex;
                sprite2.Texture = tex2;
                window.Clear();
                window.Draw(Toolbar);
                window.Draw(Slined);
                window.Draw(Sline);
                window.Draw(Slaned);
                window.Draw(Cpf);
                window.Draw(Cps);
                window.Draw(Stalin);
                window.Draw(sprite);
                window.Draw(sprite2);
                window.Display();
            }
        }

        private void Window_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button == SFML.Window.Mouse.Button.Left)
            {

                if (hovering != "canvas" && hovering != "CP")
                {
                    selected = hovering;
                }
                else
                {
                    if (hovering == "CP")
                    {
                        ColorDialog Dio = new ColorDialog();
                        Dio.AllowFullOpen = true;
                        Dio.ShowDialog();
                        selColor = new Color(Dio.Color.R, Dio.Color.G, Dio.Color.B);
                    }
                    else if (selected == "line" && !Drawing)
                    {
                        startPoint = SFML.Window.Mouse.GetPosition(window) + new Vector2i(-30, 0);
                        Drawing = true;
                    }
                    else if (selected == "line")
                    {
                        DrawLine(startPoint.X, startPoint.Y, SFML.Window.Mouse.GetPosition(window).X - 30, SFML.Window.Mouse.GetPosition(window).Y, selColor);
                        cmds.Add("l," + startPoint.X + "," + startPoint.Y + "," + (SFML.Window.Mouse.GetPosition(window).X - 30) + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                        Drawing = false;
                    }
                    else if (selected == "fill")
                    {
                        Fill(SFML.Window.Mouse.GetPosition(window).X - 30, SFML.Window.Mouse.GetPosition(window).Y, selColor);
                        cmds.Add("f," + (SFML.Window.Mouse.GetPosition(window).X - 30) + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                    }

                }
            }
            else if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                if (hovering == "canvas")
                {
                    if (selected == "line" && Drawing)
                    {
                        DrawLine(startPoint.X, startPoint.Y, SFML.Window.Mouse.GetPosition(window).X - 30, SFML.Window.Mouse.GetPosition(window).Y, selColor);
                        cmds.Add("l," + startPoint.X + "," + startPoint.Y + "," + (SFML.Window.Mouse.GetPosition(window).X - 30) + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                        startPoint = SFML.Window.Mouse.GetPosition(window) + new Vector2i(-30, 0);
                    }
                }
            }
            else if (e.Button == SFML.Window.Mouse.Button.Middle)
            {
                Drawing = false;
            }
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
                if (a.X < w && a.X > -1 && a.Y < h && a.Y > -1)
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
            if (x2 < 0 || x2 > w || y2 < 0 || y2 > h)
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
        void OnClose(object sender, EventArgs e)
        {

            window.Close();
        }
    }
    class Hey
    {
        static void Main(string[] args)
        {
            uint W;
            uint H;
            String[] commands = new String[1];
            String Filename = "";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            if (MessageBox.Show("Do you want to create a new File?", "", buttons: buttons) == System.Windows.Forms.DialogResult.Yes)
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
                    Console.Write(commands[0]);
                    t.Interrupt();
                }
            }
            else
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
            W = (uint)Int32.Parse(commands[0].Split(',')[0]);//read resolution
            H = (uint)Int32.Parse(commands[0].Split(',')[1]);

            Program p = new Program();
            p.Run(W, H, commands);
            for (int i = 0; i < p.cmds.Count - 1; i++)
            {
                p.cmds[i] += ";";
            }


            System.IO.File.WriteAllLines(Filename, p.cmds);
        }
    }
}
