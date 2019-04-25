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
-Better main menu
*/

namespace FrieVec
{
    public class Form1 : Form
    {
        float SizeFactor = 1.5f;
        public static String SolutionLoc = "C:\\Users\\Chef\\Desktop\\FrieVec\\";


        bool bezierFin;
        Vector2f startb, endb, p1b, p2b;
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
        Imageutillity Imageutils = new Imageutillity();
        public bool save = true;
        bool StrgPressed;
        float Rotation;
        public Form1()
        {

        }
        public void Run()
        {
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            float Dpc = (this.DeviceDpi / 2.54f) * SizeFactor;

            Panel Panel = new Panel();
            Panel.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            Controls.Add(Panel);

            Button LineButton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), new System.Drawing.Point(0, 0), System.Drawing.Image.FromFile(SolutionLoc + "Assets/Line.png"), System.Drawing.Color.FromArgb(30, 30, 30));
            Panel.Controls.Add(LineButton);
            LineButton.Click += (sender, e) => Select(sender, e, "line");

            Button FillButton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), new System.Drawing.Point(0, (int)(1 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "Assets/floodfill.png"), System.Drawing.Color.FromArgb(30, 30, 30), ImageLayout.Center);
            Panel.Controls.Add(FillButton);
            FillButton.Click += (sender, e) => Select(sender, e, "fill");

            Button Circlebutton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), new System.Drawing.Point(0, (int)(2 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "Assets/Circle.png"), System.Drawing.Color.FromArgb(30, 30, 30), ImageLayout.Stretch);
            Panel.Controls.Add(Circlebutton);
            Circlebutton.Click += (sender, e) => Select(sender, e, "circle");

            Button TextButton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), new System.Drawing.Point(0, (int)(3 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "Assets/Input.png"), System.Drawing.Color.FromArgb(30, 30, 30));
            Panel.Controls.Add(TextButton);
            TextButton.Click += TextButton_Click;

            Button BezierB = CreateButton.NewButton(new System.Drawing.Size((int)(Dpc), (int)Dpc), new System.Drawing.Point(0, (int)(4 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "/Assets/Bezier.png"), System.Drawing.Color.FromArgb(30, 30, 30));
            Panel.Controls.Add(BezierB);
            BezierB.Click += (sender, e) => Select(sender, e, "bezier");

            Button ColorSelectButton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), new System.Drawing.Point(0, (int)(5 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "Assets/cp.png"), System.Drawing.Color.FromArgb(30, 30, 30));
            Panel.Controls.Add(ColorSelectButton);
            ColorSelectButton.Click += new EventHandler(CP);

            Button CloseB = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), System.Drawing.Point.Empty, "X", System.Drawing.Color.FromArgb(30, 30, 30));
            Panel.Controls.Add(CloseB);
            CloseB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            CloseB.ForeColor = System.Drawing.Color.Red;
            CloseB.Click += CloseB_Click;

            DrawingSurface rendersurface = new DrawingSurface();
            rendersurface.Size = new System.Drawing.Size((int)W, (int)H);
            Controls.Add(rendersurface);
            window = new RenderWindow(rendersurface.Handle);
            window.MouseButtonPressed += Window_MouseButtonPressed;
            window.MouseWheelScrolled += Window_MouseWheelScrolled;

            image = new Image((uint)W, (uint)H, Color.Black);
            Texture tex = new Texture((uint)W, (uint)H);
            sprite = new Sprite();

            image2 = new Image(W, H, Color.Transparent);
            Texture tex2 = new Texture(W, H);
            sprite2 = new Sprite();



            Imageutils.W = W;
            this.Size = this.SizeFromClientSize(new System.Drawing.Size((int)(W + Dpc), (int)H));
            Imageutils.H = H;
            for (int i = 1; i < cmds.Count; i++)
            {

                String[] ccommand = cmds[i].Split(',');
                Console.WriteLine(ccommand[0]);
                switch (ccommand[0])
                {
                    case "l":
                        Imageutils.DrawLine(int.Parse(ccommand[1]), int.Parse(ccommand[2]), int.Parse(ccommand[3]), int.Parse(ccommand[4]), new Color(Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6])), Convert.ToByte(int.Parse(ccommand[7]))), ref image);
                        break;
                    case "f":
                        Imageutils.Fill(int.Parse(ccommand[1]), int.Parse(ccommand[2]), new Color(Convert.ToByte(int.Parse(ccommand[3])), Convert.ToByte(int.Parse(ccommand[4])), Convert.ToByte(int.Parse(ccommand[5]))), ref image);
                        break;
                    case "c":
                        Imageutils.DrawCircle(int.Parse(ccommand[1]), int.Parse(ccommand[2]), uint.Parse(ccommand[3]), new Color(Convert.ToByte(int.Parse(ccommand[4])), Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6]))), ref image);
                        break;
                    case "t":
                        Text t = null;
                        Rotation = float.Parse(ccommand[8], CultureInfo.InvariantCulture);
                        Imageutils.DrawText(int.Parse(ccommand[1]), int.Parse(ccommand[2]), ccommand[3], float.Parse(ccommand[4], CultureInfo.InvariantCulture), new Color(Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6])), Convert.ToByte(int.Parse(ccommand[7]))), ref t, ref Lüscht, Rotation);
                        break;
                    case "b":
                        Imageutils.DrawBezier(
                            new Vector2f(int.Parse(ccommand[1]), int.Parse(ccommand[2])),
                            new Vector2f(int.Parse(ccommand[3]), int.Parse(ccommand[4])),
                            new Vector2f(int.Parse(ccommand[5]), int.Parse(ccommand[6])),
                            new Vector2f(int.Parse(ccommand[7]), int.Parse(ccommand[8])),
                            new Color(Convert.ToByte(int.Parse(ccommand[9])), Convert.ToByte(int.Parse(ccommand[10])), Convert.ToByte(int.Parse(ccommand[11]))),
                            ref image);
                        break;
                }
            }
            Imageutils.DrawBezier(new Vector2f(100, 100), new Vector2f(900, 900), new Vector2f(300, 150), new Vector2f(100, 50), Color.White, ref image);
            Rotation = 0;
            Click += Form1_Click;
            while (Visible)
            {
                CloseB.Location = new System.Drawing.Point(0, this.ClientSize.Height - (int)Dpc);
                Panel.Size = new System.Drawing.Size((int)Dpc, this.ClientSize.Height);
                rendersurface.Location = new System.Drawing.Point((int)((int)(Dpc + (this.ClientSize.Width) - W) * 0.5f), (int)(((this.ClientSize.Height) - H) * 0.5f));
                image2 = new Image(W, H, Color.Transparent);
                if (drawing)
                {
                    if (selected == "line")
                        Imageutils.DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor, ref image2);
                    if (selected == "circle")
                        Imageutils.DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor, ref image2);
                    if (selected == "txt")
                        Imageutils.DrawText(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, DrawingStr, radius * 0.1f, selColor, ref curtxt, ref Lüscht, Rotation);
                    if (selected == "bezier" && !bezierFin)
                    {
                        Imageutils.DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)(Dpc * 0.5f), Color.White, ref image2);
                        Imageutils.DrawCircle((int)startb.X,(int)startb.Y, (uint)(Dpc * 0.5f), Color.White, ref image2);
                        Imageutils.DrawLine((int)startb.X, (int)startb.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor, ref image2);
                    }
                    if (selected == "bezier" && bezierFin)
                    {
                        Imageutils.DrawCircle((int)endb.X,(int)endb.Y, (uint)(Dpc * 0.5f), Color.White, ref image2);
                        Imageutils.DrawCircle((int)startb.X, (int)startb.Y, (uint)(Dpc * 0.5f), Color.White, ref image2);
                        Imageutils.DrawCircle((int)p1b.X, (int)p1b.Y, (uint)(Dpc * 0.5f), Color.White, ref image2);
                        Imageutils.DrawCircle((int)p2b.X, (int)p2b.Y, (uint)(Dpc * 0.5f), Color.White, ref image2);
                        Imageutils.DrawBezier(startb,endb,p1b,p2b, selColor, ref image2);
                    }
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
            if (dialog == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("FrieVec.exe");
                Application.Exit();
            }
            if (dialog == DialogResult.No)
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
            Imageutils.DrawText(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, DrawingStr, radius, selColor, ref t, ref Lüscht, Rotation);
            drawing = true;
            window.SetMouseCursorVisible(false);
            selected = "txt";
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (drawing && selected == "circle")
            {
                Imageutils.DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor, ref image);
                cmds.Add("c," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + radius + "," + selColor.R + "," + selColor.G + "," + selColor.B);
            }
        }
        private void Window_MouseWheelScrolled(object sender, SFML.Window.MouseWheelScrollEventArgs e)
        {
            if (StrgPressed)
            {
                Rotation += e.Delta * 3;
            }
            else
            {
                radius += (int)((e.Delta * radius) * 0.1f);
            }

        }
        [STAThread]
        static void Main()
        {
            Form1 p = new Form1();
            uint W = 200;
            uint H = 100;
            float Dpc = (p.DeviceDpi / 2.54f) * p.SizeFactor;

            p.ClientSize = new System.Drawing.Size((int)W, (int)H);

            Button button5 = new Button();
            button5.BackgroundImage = System.Drawing.Image.FromFile(SolutionLoc + "/Assets/Folder.png");
            button5.Size = new System.Drawing.Size(100, 100);
            button5.BackgroundImageLayout = ImageLayout.Stretch;
            button5.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            button5.Anchor = AnchorStyles.None;
            p.Controls.Add(button5);
            button5.Click += new EventHandler(p.LoadFile);

            Button button6 = new Button();
            button6.BackgroundImage = System.Drawing.Image.FromFile(SolutionLoc + "/Assets/new.png");
            button6.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            button6.Left = 100;
            button6.Size = new System.Drawing.Size(100, 100);
            button6.BackgroundImageLayout = ImageLayout.Zoom;
            button6.Anchor = AnchorStyles.None;
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

            p.Controls.Remove(button5);
            p.Controls.Remove(button6);


            W = (uint)Int32.Parse(p.commands[0].Split(',')[0]);//read resolution
            H = (uint)Int32.Parse(p.commands[0].Split(',')[1]);
            p.Text = p.Filename.Split("\\".ToCharArray())[p.Filename.Split("\\".ToCharArray()).Count() - 1];


            p.cmds = p.commands.ToList();
            p.W = W;
            p.H = H;

            p.Run();
            if (p.save == false)
                return;
            for (int i = 0; i < p.cmds.Count; i++)
            {
                p.cmds[i] += ";\n";
            }


            System.IO.File.WriteAllText(p.Filename,/*StringCipher.Encrypt(*/string.Join("", p.cmds)/*, "T322ewfWa")*/);

        }

        private void Select(object sender, EventArgs e, String sel)
        {
            selected = sel;
            drawing = false;
            if (sel == "circle")
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
                    Imageutils.Fill(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor, ref image);
                    cmds.Add("f," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                }
                else if (drawing && selected == "line")
                {
                    drawing = false;
                    Imageutils.DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor, ref image);
                    cmds.Add("l," + startpos.X + "," + startpos.Y + "," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                }
                else if (selected == "line")
                {
                    startpos = SFML.Window.Mouse.GetPosition(window);
                    drawing = true;
                }
                else if (drawing && selected == "circle")
                {
                    Imageutils.DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor, ref image);
                    cmds.Add("c," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + radius + "," + selColor.R + "," + selColor.G + "," + selColor.B);

                }
                if (drawing && selected == "txt")
                {
                    cmds.Add("t," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + DrawingStr + "," + (radius * 0.1f).ToString(CultureInfo.InvariantCulture) + "," + selColor.R + "," + selColor.G + "," + selColor.B + "," + Rotation.ToString(CultureInfo.InvariantCulture));
                    drawing = false;
                    curtxt = null;
                    window.SetMouseCursorVisible(true);
                }
                if(selected == "bezier" && !drawing)
                {
                    drawing = true;
                    bezierFin = false;
                    startb = (Vector2f)SFML.Window.Mouse.GetPosition(window);
                }
                if (selected == "bezier" && !bezierFin && drawing)
                {
                    bezierFin = true;
                    endb = (Vector2f)SFML.Window.Mouse.GetPosition(window);
                    p1b = (endb + startb) * 0.25f;
                    p2b = (endb + startb) * 0.75f;
                }
                
            }
            if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                if (drawing)
                {
                    if (selected == "line")
                    {
                        Imageutils.DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor, ref image);

                        cmds.Add("l," + startpos.X + "," + startpos.Y + "," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                        startpos = SFML.Window.Mouse.GetPosition(window);
                    }
                    if (selected == "txt")
                    {
                        cmds.Add("t," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + DrawingStr + "," + (radius * 0.1f).ToString(CultureInfo.InvariantCulture) + "," + selColor.R + "," + selColor.G + "," + selColor.B + "," + Rotation.ToString(CultureInfo.InvariantCulture));
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
            if (Filename == "")
                return;
            using (StreamReader fileStream = new StreamReader(Filename))//Read from file
            {
                String FullText = fileStream.ReadToEnd();
                try
                {
                    FullText = StringCipher.Decrypt(FullText, "T322ewfWa");
                }
                catch
                {

                }

                commands = FullText.Split(';');
                for (int i = 0; i < commands.Count(); i++)

                {
                    commands[i] = Regex.Replace(commands[i], "\n", "");
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

    }

}



