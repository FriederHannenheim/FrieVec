using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using System.Text.RegularExpressions;

namespace FrieVec
{
    class Program
    {
        String selected = "";
        String hovering = "";
        RenderWindow window;
        Image image;
        int w, h;
        bool pressed;
        public void Run(uint W, uint H, String[] commands)
        {
            
            w = (int)W;
            h = (int)H;
            window = new RenderWindow(new SFML.Window.VideoMode(W+30, H), "FrieVec");
            image = new Image(W, H, Color.Black);
            Texture tex = new Texture(W, H);
            Sprite sprite = new Sprite();
            sprite.Position = new Vector2f(30, 0);
            
            RectangleShape Slined = new RectangleShape(new Vector2f(30,30));
            Slined.FillColor = new Color(200, 200, 200);
            RectangleShape Sline = new RectangleShape(new Vector2f(45,3));
            Sline.Position = new Vector2f(0, -3);
            Sline.Rotation = 45;

            RectangleShape Slaned = new RectangleShape(new Vector2f(30, 30));
            Slaned.FillColor = new Color(200, 200, 200);
            Slaned.Position = new Vector2f(0, 30);

            Texture Mussolini = new Texture(@"Assets/floodfill.png");
            Sprite Stalin = new Sprite();
            Stalin.Texture = Mussolini;
            Stalin.Position = new Vector2f(0,30);
            Stalin.Scale = new Vector2f(0.7f, 0.7f);

            RectangleShape Toolbar = new RectangleShape(new Vector2f(1000, 500));
            Toolbar.FillColor = new Color(230, 230, 230);
            Toolbar.Size = new Vector2f(30,(int) H);
            

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

            while (window.IsOpen)
            {
                Slaned.FillColor = new Color(200, 200, 200);
                Slined.FillColor = new Color(200, 200, 200);
                hovering = "";
                
                FloatRect rect = Slined.GetGlobalBounds();
                FloatRect rect2 = Slaned.GetGlobalBounds();
                if (rect.Contains(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y))
                {
                    Slined.FillColor = new Color(160,160,160);
                    hovering = "line";
                }
                if (rect2.Contains(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y))
                {
                    Slaned.FillColor = new Color(160, 160, 160);
                    hovering = "fill";
                }
                if(sprite.GetGlobalBounds().Contains(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y)))
                {
                    hovering = "canvas";
                }
                if (selected == "line")
                    Slined.FillColor = new Color(100, 100, 100);
                else if (selected == "fill")
                    Slaned.FillColor = new Color(100, 100, 100);



                window.DispatchEvents();
                window.Closed += new EventHandler(OnClose);

                tex.Update(image);
                sprite.Texture = tex;
                window.Clear();
                window.Draw(Toolbar);
                window.Draw(Slined);
                window.Draw(Sline);
                window.Draw(Slaned);
                window.Draw(Stalin);
                window.Draw(sprite);
                window.Display();
                pressed = false;
            }
        }

        private void Window_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button == SFML.Window.Mouse.Button.Left)
            {
                selected = hovering;
            }
        }

        private void Fill(int x, int y, Color color)
        {
            Stack<Vector2i> pixels = new Stack<Vector2i>();
            Color target = image.GetPixel((uint)x, (uint)y);
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
        private void DrawLine(int x1, int y1, int x2, int y2, Color color)
        {
            uint x, y;
            int i, step;
            int dx = (x2 - x1);
            int dy = (y2 - y1);
            if (Math.Abs(dx) >= Math.Abs(dy))
                step = Math.Abs(dx);
            else
                step = Math.Abs(dy);
            dx = dx / step;
            dy = dy / step;
            x = (uint)x1;
            y = (uint)y1;
            i = 1;
            while (i <= step)
            {
                image.SetPixel(x, y, color);
                x = x + (uint)dx;
                y = y + (uint)dy;
                i = i + 1;
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
            String[] commands;

            using (StreamReader fileStream = new StreamReader("D:/C#/FrieVec/FrieVec/bin/Debug/Haus.frvc"))//Read from file
            {
                String FullText = fileStream.ReadToEnd();
                FullText = Regex.Replace(FullText,@"\t|\n|\r","");
                commands = FullText.Split(';');
            }
            Console.Write(commands[0]);
            Console.Write(commands[1]);
            Console.Write(commands[2]);
            Console.Write(commands[3]);
            try
            {
                W = (uint)Int32.Parse(commands[0].Split(',')[0]);//read resolution
                H = (uint)Int32.Parse(commands[0].Split(',')[1]);
            }
            catch
            {
                throw new Exception("bad resolution");
            }
            Program p = new Program();
            p.Run(W, H, commands);
        }
    }
}
