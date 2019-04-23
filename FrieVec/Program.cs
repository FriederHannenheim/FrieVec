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
        float SizeFactor = 2;
        public static String SolutionLoc = "C:\\Users\\Chef\\Desktop\\FrieVec\\";


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
            float Dpc = (this.DeviceDpi / 2.54f)*SizeFactor;

            Panel Panel = new Panel();
            Panel.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            Controls.Add(Panel);

            Button LineButton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1*Dpc)), new System.Drawing.Point(0, 0), System.Drawing.Image.FromFile(SolutionLoc + "Assets/Line.png"), System.Drawing.Color.FromArgb(30, 30, 30));
            Panel.Controls.Add(LineButton);
            LineButton.Click += new EventHandler(sLine);

            Button FillButton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)),new System.Drawing.Point(0,(int)(1 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "Assets/floodfill.png"), System.Drawing.Color.FromArgb(30, 30, 30),ImageLayout.Center);
            Panel.Controls.Add(FillButton);
            FillButton.Click += new EventHandler(sFill);

            Button ColorSelectButton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), new System.Drawing.Point(0,(int)(4 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "Assets/cp.png"), System.Drawing.Color.FromArgb(30, 30, 30));
            Panel.Controls.Add(ColorSelectButton);
            ColorSelectButton.Click += new EventHandler(CP);

            Button Circlebutton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), new System.Drawing.Point(0,(int)(2 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "Assets/Circle.png"), System.Drawing.Color.FromArgb(30, 30, 30),ImageLayout.Stretch);
            Panel.Controls.Add(Circlebutton);
            Circlebutton.Click += new EventHandler(sCircle);

            Button TextButton = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)), new System.Drawing.Point(0,(int)(3 * Dpc)), System.Drawing.Image.FromFile(SolutionLoc + "Assets/Input.png"), System.Drawing.Color.FromArgb(30, 30, 30));
            Panel.Controls.Add(TextButton);
            TextButton.Click += TextButton_Click;

            Button CloseB = CreateButton.NewButton(new System.Drawing.Size((int)(1 * Dpc), (int)(1 * Dpc)),System.Drawing.Point.Empty,"X",System.Drawing.Color.FromArgb(30,30,30));
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
            this.Size = this.SizeFromClientSize(new System.Drawing.Size((int)(W+Dpc),(int)H));
            Imageutils.H = H;
            for (int i = 1; i < cmds.Count; i++)
            {
                String[] ccommand = cmds[i].Split(',');
                switch (ccommand[0])
                {
                    case "l":
                        Imageutils.DrawLine(int.Parse(ccommand[1]), int.Parse(ccommand[2]), int.Parse(ccommand[3]), int.Parse(ccommand[4]), new Color(Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6])), Convert.ToByte(int.Parse(ccommand[7]))),ref image);
                        break;
                    case "f":
                        Imageutils.Fill(int.Parse(ccommand[1]), int.Parse(ccommand[2]), new Color(Convert.ToByte(int.Parse(ccommand[3])), Convert.ToByte(int.Parse(ccommand[4])), Convert.ToByte(int.Parse(ccommand[5]))),ref image);
                        break;
                    case "c":
                        Imageutils.DrawCircle(int.Parse(ccommand[1]), int.Parse(ccommand[2]), uint.Parse(ccommand[3]), new Color(Convert.ToByte(int.Parse(ccommand[4])), Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6]))),ref image);
                        break;
                    case "t":
                        Text t = null;
                        Rotation = float.Parse(ccommand[8],CultureInfo.InvariantCulture);
                        Imageutils.DrawText(int.Parse(ccommand[1]), int.Parse(ccommand[2]), ccommand[3], float.Parse(ccommand[4], CultureInfo.InvariantCulture), new Color(Convert.ToByte(int.Parse(ccommand[5])), Convert.ToByte(int.Parse(ccommand[6])), Convert.ToByte(int.Parse(ccommand[7]))),ref t,ref Lüscht,Rotation);
                        break;
                }
            }
            Rotation = 0;
            Click += Form1_Click;
            while (Visible)
            {
                CloseB.Location = new System.Drawing.Point(0, this.ClientSize.Height -(int) Dpc);
                Panel.Size = new System.Drawing.Size((int)Dpc, this.ClientSize.Height);
                rendersurface.Location = new System.Drawing.Point((int)((int)(Dpc +(this.ClientSize.Width)- W)*0.5f), (int)(((this.ClientSize.Height) - H) * 0.5f));
                image2 = new Image(W, H, Color.Transparent);
                if (drawing)
                {
                    if (selected == "line")
                        Imageutils.DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor, ref image2);
                    if (selected == "circle")
                        Imageutils.DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor, ref image2);
                    if (selected == "txt")
                        Imageutils.DrawText(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, DrawingStr, radius*0.1f, selColor,ref curtxt,ref Lüscht,Rotation);
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
            Imageutils.DrawText(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, DrawingStr, radius, selColor,ref t,ref Lüscht,Rotation);
            drawing = true;
            window.SetMouseCursorVisible(false);
            selected = "txt";
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            if (drawing && selected == "circle")
            {
                Imageutils.DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor,ref image);
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

            p.Run();
            if (p.save == false)
                return;
            for (int i = 0; i < p.cmds.Count - 1; i++)
            {
                p.cmds[i] += ";";
            }


            System.IO.File.WriteAllText(p.Filename,StringCipher.Encrypt(string.Join("",p.cmds), "T322ewfWa"));

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
                    Imageutils.Fill(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor,ref image);
                    cmds.Add("f," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                }
                else if (drawing && selected == "line")
                {
                    drawing = false;
                    Imageutils.DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor,ref image);
                    cmds.Add("l," + startpos.X + "," + startpos.Y + "," + SFML.Window.Mouse.GetPosition(window).X + "," + SFML.Window.Mouse.GetPosition(window).Y + "," + selColor.R + "," + selColor.G + "," + selColor.B);
                }
                else if (selected == "line")
                {
                    startpos = SFML.Window.Mouse.GetPosition(window);
                    drawing = true;
                }
                else if (drawing && selected == "circle")
                {
                    Imageutils.DrawCircle(SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, (uint)radius, selColor,ref image);
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
                        Imageutils.DrawLine(startpos.X, startpos.Y, SFML.Window.Mouse.GetPosition(window).X, SFML.Window.Mouse.GetPosition(window).Y, selColor,ref image);

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
            if (Filename == "")
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

    }

}



