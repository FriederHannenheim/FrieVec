using Microsoft.VisualBasic;
using SFML.Graphics;
using SFML.System;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

/* todo:
-Curves
-Encryption random string
-Pixels
*/

namespace FrieVec
{
    public class Form1 : Form
    {
        public bool save = true;
        bool StrgPressed;
        float Rotation;
        public static String SolutionLoc = "D:\\C#\\FrieVec\\";
        String Filename = "";
        bool drawing;
        int radius = 30;
        String DrawingStr = "";
        Text curtxt;
        String selected;
        Vector2i startpos;
        Image image;
        Image image2;
        Color selColor = Color.White;
        uint W, H;
        RenderWindow window;
        List<String> cmds;
        String[] commands = new String[1];
        Sprite sprite2;
        Sprite sprite;
        List<Drawable> Lüscht = new List<Drawable>();
        public Form1()
        {

        }
        public void Run()
        {
            Panel Panel = new Panel();

            Controls.Add(Panel);

            Button LineButton = new Button();
            LineButton.Size = new System.Drawing.Size(30, 30);
            LineButton.Location = new System.Drawing.Point(0, 0);
            LineButton.Image = System.Drawing.Image.FromFile(SolutionLoc + "Assets/Line.png");
            LineButton.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Panel.Controls.Add(LineButton);
            LineButton.Click += new EventHandler(sLine);

            Button FillButton = new Button();
            FillButton.Size = new System.Drawing.Size(30, 30);
            FillButton.Location = new System.Drawing.Point(0, 30);
            FillButton.Image = System.Drawing.Image.FromFile(SolutionLoc + "Assets/floodfill.png");
            FillButton.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Panel.Controls.Add(FillButton);
            FillButton.Click += new EventHandler(sFill);

            Button ColorSelectButton = new Button();
            ColorSelectButton.Size = new System.Drawing.Size(30, 30);
            ColorSelectButton.Location = new System.Drawing.Point(0, 150);
            ColorSelectButton.Image = System.Drawing.Image.FromFile(SolutionLoc + "Assets/cp.png");
            ColorSelectButton.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Panel.Controls.Add(ColorSelectButton);
            ColorSelectButton.Click += new EventHandler(CP);

            Button Circlebutton = new Button();
            Circlebutton.Size = new System.Drawing.Size(30, 30);
            Circlebutton.Location = new System.Drawing.Point(0, 60);
            Circlebutton.BackgroundImage = System.Drawing.Image.FromFile(SolutionLoc + "Assets/Circle.png");
            Circlebutton.BackgroundImageLayout = ImageLayout.Stretch;
            Circlebutton.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Panel.Controls.Add(Circlebutton);
            Circlebutton.Click += new EventHandler(sCircle);

            Button TextButton = new Button();
            TextButton.Size = new System.Drawing.Size(30, 30);
            TextButton.Location = new System.Drawing.Point(0, 90);
            TextButton.BackgroundImage = System.Drawing.Image.FromFile(SolutionLoc + "Assets/Input.png");
            TextButton.BackgroundImageLayout = ImageLayout.Stretch;
            TextButton.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            Panel.Controls.Add(TextButton);
            TextButton.Click += TextButton_Click;

            Button CloseB = new Button();
            CloseB.Size = new System.Drawing.Size(30, 30);
            Panel.Controls.Add(CloseB);
            CloseB.Text = "X";
            CloseB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            CloseB.ForeColor = System.Drawing.Color.Red;
            CloseB.Font = new System.Drawing.Font(CloseB.Font.Name, CloseB.Font.Size, System.Drawing.FontStyle.Bold);
            CloseB.Click += CloseB_Click;

            DrawingSurface rendersurface = new DrawingSurface();
            rendersurface.Size = new System.Drawing.Size((int)W, (int)H);
            Size = new System.Drawing.Size((int)W+46, (int)H+40);
            Controls.Add(rendersurface);
            Panel.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            image = new Image((uint)W, (uint)H, Color.Black);
            Texture tex = new Texture((uint)W, (uint)H);
            sprite = new Sprite();

            image2 = new Image(W, H, Color.Transparent);
            Texture tex2 = new Texture(W, H);
            sprite2 = new Sprite();
            window = new RenderWindow(rendersurface.Handle);
            window.MouseButtonPressed += Window_MouseButtonPressed;
            window.MouseWheelScrolled += Window_MouseWheelScrolled;

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
                        DrawCircle(int.Parse(ccommand[1]), int.Parse(ccommand[2]), uint.Parse(ccommand[3]), new Color(Convert.ToByte(int.Parse(ccommand[4])), Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6]))));
                        break;
                    case "t":
                        Text t = null;
                        Rotation = float.Parse(ccommand[8],CultureInfo.InvariantCulture);
                        DrawText(int.Parse(ccommand[1]), int.Parse(ccommand[2]), ccommand[3], float.Parse(ccommand[4], CultureInfo.InvariantCulture), new Color(Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6])), Convert.ToByte(int.Parse(ccommand[7]))),ref t);
                        break;
                }
            }
            Rotation = 0;

            Click += Form1_Click;
            while (Visible)
            {
                CloseB.Location = new System.Drawing.Point(0, Size.Height - 70);
                Panel.Size = new System.Drawing.Size(30, Height);
                rendersurface.Location = new System.Drawing.Point((int)(30 +( (Size.Width -46)- W)*0.5f), (int)(((Size.Height - 40) - W) * 0.5f));
                image2 = new Image(W, H, Color.Transparent);
                if (drawing)
                {
                    if (selected == "line")
                        DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor, "i2");
                    if (selected == "circle")
                        DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor, "i2");
                    if (selected == "txt")
                        DrawText(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, DrawingStr, radius*0.1f, selColor,ref curtxt);
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
                foreach (Drawable item in Lüscht)
                {
                    window.Draw(item);
                }
                window.Display();
                StrgPressed = false;
                if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.LControl))
                {
                    StrgPressed = true;
                }
            }
        }

        private void CloseB_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Save changes?", "Close", MessageBoxButtons.YesNoCancel);
            if(dialog == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("FrieVec.exe");
                Application.Exit();
            }
            if(dialog == DialogResult.No)
            {
                System.Diagnostics.Process.Start("FrieVec.exe");
                save = false;
                Application.Exit();
            }
        }

        private void TextButton_Click(object sender, EventArgs e)
        {
            DrawingStr = Interaction.InputBox("Text", "Text");
            Text t = new Text();
            DrawText(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, DrawingStr, radius, selColor,ref t);
            drawing = true;
            window.SetMouseCursorVisible(false);
            selected = "txt";
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (drawing && selected == "circle")
            {
                DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor);
                cmds.Add("c," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + radius + "," + selColor.R + "," + selColor.G + "," + selColor.B);
            }
        }
        private void Window_MouseWheelScrolled(object sender, SFML.Window.MouseWheelScrollEventArgs e)
        {
            if(StrgPressed)
            {
                Rotation += e.Delta*3;
            }else
            {
                radius += (int)((e.Delta * radius) * 0.1f);
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
            button5.Image = System.Drawing.Image.FromFile(SolutionLoc + "Assets/Folder.png");
            button5.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            p.Controls.Add(button5);
            button5.Click += new EventHandler(p.LoadFile);

            Button button6 = new Button();
            button6.Size = new System.Drawing.Size(30, 30);
            button6.Location = new System.Drawing.Point(0, 30);
            button6.BackgroundImage = System.Drawing.Image.FromFile(SolutionLoc + "Assets/new.png");
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
            if (p.save == false)
                return;
            for (int i = 0; i < p.cmds.Count - 1; i++)
            {
                p.cmds[i] += ";";
            }


            System.IO.File.WriteAllText(p.Filename,StringCipher.Encrypt(string.Join("",p.cmds), "T322ewfWa"));

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
        void DrawText(int x, int y, String text,float size,Color color,ref Text curtxt)
        {
            if (curtxt == null)
            {
                curtxt = new Text();
                Lüscht.Add(curtxt);
                curtxt.DisplayedString = text;
                curtxt.Font = new SFML.Graphics.Font(SolutionLoc+"/Assets/arial.ttf");
                curtxt.FillColor = color;
            }

            curtxt.Rotation = Rotation;
            curtxt.Position = new Vector2f(x,y);
            curtxt.Scale = new Vector2f(size, size);


        }
        void DrawCircle(int x_centro,int y_centro, uint r, Color color, String iage = "i1")
        {
            uint x_centre = (uint)x_centro;
            uint y_centre = (uint)y_centro;
            IntRect Racks = new IntRect(0, 0, (int)W, (int)H);
            //if you're not me, please don't look at this code 
            ref Image img = ref image;
            if (iage == "i2")
                img = ref image2;

            uint x = r, y = 0;
            if (Racks.Contains((int)(x + x_centre), (int)(y + y_centre)))
                img.SetPixel(x + x_centre, y + y_centre, color);

            if (r > 0)
            {
                if (Racks.Contains((int)(x + x_centre), (int)(-y + y_centre)))
                    img.SetPixel(x + x_centre, (uint)-y + y_centre, color);
                if (Racks.Contains((int)(y + x_centre), (int)(x + y_centre)))
                    img.SetPixel(y + x_centre, x + y_centre, color);
                if (Racks.Contains((int)(-y + x_centre), (int)(x + y_centre)))
                    img.SetPixel((uint)-y + x_centre, x + y_centre, color);
            }

            int P = 1 - (int)r;
            while (x > y)
            {
                y++;


                if (P <= 0)
                    P = (int)(P + 2 * y + 1);


                else
                {
                    x--;
                    P = (int)(P + 2 * y - 2 * x + 1);
                }


                if (x < y)
                    break;
                if (Racks.Contains((int)(x + x_centre), (int)(y + y_centre)))
                    img.SetPixel(x + x_centre, y + y_centre, color);
                if (Racks.Contains((int)(-x + x_centre), (int)(y + y_centre)))
                    img.SetPixel((uint)-x + x_centre, y + y_centre, color);
                if (Racks.Contains((int)(x + x_centre), (int)(-y + y_centre)))
                    img.SetPixel(x + x_centre, (uint)-y + y_centre, color);
                if (Racks.Contains((int)(-x + x_centre), (int)(-y + y_centre)))
                    img.SetPixel((uint)-x + x_centre, (uint)-y + y_centre, color);

                if (x != y)
                {
                    if (Racks.Contains((int)(y + x_centre), (int)(x + y_centre)))
                        img.SetPixel(y + x_centre, x + y_centre, color);
                    if (Racks.Contains((int)(-y + x_centre), (int)(x + y_centre)))
                        img.SetPixel((uint)-y + x_centre, x + y_centre, color);
                    if (Racks.Contains((int)(y + x_centre), (int)(-x + y_centre)))
                        img.SetPixel(y + x_centre, (uint)-x + y_centre, color);
                    if (Racks.Contains((int)(-y + x_centre), (int)(-x + y_centre)))
                        img.SetPixel((uint)-y + x_centre, (uint)-x + y_centre, color);
                }
            }
            if (Racks.Contains((int)(x_centre), (int)(y_centre - r)))
                img.SetPixel(x_centre, (uint)(y_centre - r), color);
            if (Racks.Contains((int)(x_centre - (uint)r), (int)(y_centre)))
                img.SetPixel(x_centre - (uint)r, y_centre, color);


        }
        private void sLine(object sender, EventArgs e)
        {
            selected = "line";
            drawing = false;
        }
        private void sFill(object sender, EventArgs e)
        {
            selected = "fill";
            drawing = false;
        }
        private void sCircle(object sender, EventArgs e)
        {
            selected = "circle";
            drawing = true;
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
                else if (drawing && selected == "line")
                {
                    drawing = false;
                    DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor);
                    cmds.Add("l," + startpos.X + "," + startpos.Y + "," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                }
                else if (selected == "line")
                {
                    startpos = SFML.Window.Mouse.GetPosition(window);
                    drawing = true;
                }
                else if (drawing && selected == "circle")
                {
                    DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor);
                    cmds.Add("c," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + radius + "," + selColor.R + "," + selColor.G + "," + selColor.B);

                }if(drawing && selected == "txt")
                {
                    cmds.Add("t," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + DrawingStr + "," + (radius * 0.1f).ToString(CultureInfo.InvariantCulture) + "," + selColor.R + "," + selColor.G + "," + selColor.B + "," + Rotation.ToString(CultureInfo.InvariantCulture));
                    drawing = false;
                    curtxt = null;
                    window.SetMouseCursorVisible(true);
                }
            }
            if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                if (drawing)
                {
                    if (selected == "line")
                    {
                        DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor);

                        cmds.Add("l," + startpos.X + "," + startpos.Y + "," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                        startpos = SFML.Window.Mouse.GetPosition(window);
                    }
                    if (selected == "txt")
                    {
                        cmds.Add("t," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + DrawingStr + "," + (radius * 0.1f).ToString(CultureInfo.InvariantCulture) + "," + selColor.R + "," + selColor.G + "," + selColor.B+","+Rotation.ToString(CultureInfo.InvariantCulture));
                        curtxt = null;
                    }

                }
            }
            if (e.Button == SFML.Window.Mouse.Button.Middle)
            {
                drawing = false;
            }
        }
        private void LoadFile(object sender, EventArgs e)
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
            if (Filename == null)
                return;
            using (StreamReader fileStream = new StreamReader(Filename))//Read from file
            {
                String FullText = fileStream.ReadToEnd();
                try
                {
                    FullText = StringCipher.Decrypt(FullText,"T322ewfWa");
                }
                catch
                {

                }
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
                if (saveFileDialog1.FileName == "")
                    return;
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
    public static class StringCipher
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}



