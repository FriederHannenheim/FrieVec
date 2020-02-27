using SFML.Graphics;
using SFML.System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using SFML.Window;
using Microsoft.VisualBasic;
namespace FrieVec_remaster
{
    public partial class Form1 : Form
    {
        public DrawingCurrent current;              //Currently selected drawable
        public Color currentColor = Color.White;
        private RenderWindow window;
        private string[] commands;
        private List<Drawable> sprites = new List<Drawable>();
        DrawingSurface DrawS;
        private string Selected;
        private Font font;
        public Form1()
        {
            InitializeComponent();
            font = new Font(Properties.Resources.aileron_bold);
            //Initialize drawingsurface
            DrawS = new DrawingSurface();
            DrawS.Location = new System.Drawing.Point(0, toolStrip1.Height);
            DrawS.Height = Height - toolStrip1.Height;
            DrawS.Width = Width;
            Controls.Add(DrawS);
        }

        public void Main()
        {
            window = new RenderWindow(DrawS.Handle);
            window.MouseButtonPressed += new System.EventHandler<MouseButtonEventArgs>(window_mousedown);
            window.MouseWheelScrolled += new System.EventHandler<MouseWheelScrollEventArgs>(window_scroll);

            //Read commands from file
            using (StreamReader filestream = new StreamReader("test.frvc"))
            {
                commands = filestream.ReadToEnd().Split(';');
            }
            //Parse commands and add them to sprites
            for (int i = 0; i < commands.Length; i++)
            {
                string[] subCommands = commands[i].Split(',');
                sprites.Add(Drawables.parseCommand(subCommands));

            }
            //Remove empty objects
            sprites.RemoveAll(item => item == null);
            while (Visible)
            {
                Application.DoEvents();
                window.DispatchEvents();
                window.Clear();
                Update();

                //Draw Here
                for (int i = 0; i < sprites.Count; i++)
                {
                    window.Draw(sprites[i]);
                }
                if (current.drawable != null)
                    window.Draw(current.drawable);
                //Stop Drawing

                window.Display();

            }
        }
        //Update every frame
        private new void Update()
        {
            if (current.drawable != null)
            {
                if (current.drawable.GetType() == typeof(Line))//Set end of the line to mouse position
                {
                    Line l = current.drawable as Line;
                    l.end = (Vector2f)Mouse.GetPosition(window);
                    l.Update();
                    current.drawable = l;
                }
                else if (current.drawable.GetType() == typeof(CircleShape))//Set the position of the circle
                {
                    CircleShape c = current.drawable as CircleShape;
                    c.Position = (Vector2f)Mouse.GetPosition(window) - new Vector2f(c.Radius, c.Radius);

                    current.drawable = c;
                }
                else if (current.drawable.GetType() == typeof(Text))
                {
                    Text t = current.drawable as Text;
                    t.Position = (Vector2f)Mouse.GetPosition(window);
                    
                    current.drawable = t;
                }
                else if (current.drawable.GetType() == typeof(BezierCurve))//If no handle is selected set the end position and handles
                {
                    BezierCurve b = current.drawable as BezierCurve;
                    switch (b.handling) //If a handle is selected set the position
                    {
                        case "n":
                            return;
                        case "s":
                            b.starthandle = (Vector2f)Mouse.GetPosition(window);
                            b.Update();
                            current.drawable = b;
                            return;
                        case "e":
                            b.endhandle = (Vector2f)Mouse.GetPosition(window);
                            b.Update();
                            current.drawable = b;
                            return;
                    }
                    b.end = (Vector2f)Mouse.GetPosition(window);
                    b.starthandle = new Vector2f((b.end.X - b.start.X) * 0.25f + b.start.X, (b.end.Y - b.start.Y) * 0.25f + b.start.Y);
                    b.endhandle = new Vector2f((b.end.X - b.start.X) * 0.75f + b.start.X, (b.end.Y - b.start.Y) * 0.75f + b.start.Y);
                    b.Update();
                    current.drawable = b;
                }

            }

        }
        private void window_scroll(object sender, MouseWheelScrollEventArgs e)
        {
            if (current.drawable != null)
            {
                if (current.drawable.GetType() == typeof(CircleShape))
                {
                    CircleShape c = current.drawable as CircleShape;
                    c.Radius += e.Delta;
                    current.drawable = c;
                }
                if (current.drawable.GetType() == typeof(Text))
                {
                    SFML.Graphics.Text t = new Text((current.drawable as Text).DisplayedString, new Font(Properties.Resources.aileron_bold));
                    t.CharacterSize = (current.drawable as Text).CharacterSize + (uint)(e.Delta*1.5f);
                    t.FillColor = currentColor;
                    current.drawable = t;
                }
            }
        }
        private void window_mousedown(object sender, MouseButtonEventArgs e)
        {
            if (current.drawable == null || e.Button == Mouse.Button.Right)
            {
                
                if (e.Button == Mouse.Button.Right)
                    sprites.Add(current.drawable);
                
                switch (Selected)
                {
                    case "line":
                        current = new DrawingCurrent();
                        current.drawable = new Line((Vector2f)Mouse.GetPosition(window), (Vector2f)Mouse.GetPosition(window), currentColor);
                        break;
                    case "circle":
                        float radius = 50;
                        if (current.drawable != null)
                            if (current.drawable.GetType() == typeof(CircleShape))
                                radius = (current.drawable as CircleShape).Radius;

                        current = new DrawingCurrent();
                        CircleShape c = new CircleShape(radius);
                        c.Position = (Vector2f)Mouse.GetPosition(window) - new Vector2f(c.Radius, c.Radius);
                        c.OutlineColor = currentColor;
                        c.OutlineThickness = 1;
                        c.FillColor = Color.Transparent;
                        current.drawable = c;
                        break;
                    case "text":
                        
                        string s = "";
                        if (e.Button == Mouse.Button.Right && current.drawable.GetType() == typeof(Text))
                            s = (current.drawable as Text).DisplayedString;
                        else
                            s = Interaction.InputBox("Text", "Text");
                        SFML.Graphics.Text t = new Text(s, new Font(Properties.Resources.aileron_bold));
                        t.CharacterSize = 50;
                        t.Position = new Vector2f(50,55);
                        t.FillColor = currentColor;
                        
                        current.drawable = t;
                        break;
                    case "bezier":
                        current = new DrawingCurrent();
                        current.drawable = new BezierCurve((Vector2f)Mouse.GetPosition(window), (Vector2f)Mouse.GetPosition(window), (Vector2f)Mouse.GetPosition(window), (Vector2f)Mouse.GetPosition(window), currentColor);

                        break;
                }


            }
            else
            {
                if (current.drawable.GetType() == typeof(BezierCurve))
                {

                    BezierCurve b = current.drawable as BezierCurve;
                    if (b.handling == "n")
                    {
                        if (Drawables.CircleContains((Vector2i)b.starthandle, 12, Mouse.GetPosition(window)))
                        {
                            b.handling = "s";
                        }
                        else if (Drawables.CircleContains((Vector2i)b.endhandle, 12, Mouse.GetPosition(window)))
                        {
                            b.handling = "e";
                        }
                        else
                        {
                            b.handling = "";
                            current.drawable = b;
                            sprites.Add(current.drawable);
                            current.drawable = null;
                            return;
                        }


                    }
                    else
                        b.handling = "n";
                    current.drawable = b;
                    return;
                }
                sprites.Add(current.drawable);
                current.drawable = null;
            }


        }
        private void btn_Click(object sender, System.EventArgs e)
        {
            current.drawable = null;
            if (btnLine.Equals(sender as ToolStripButton))
                Selected = "line";
            if (btnCircle.Equals(sender as ToolStripButton))
                Selected = "circle";
            if (btnText.Equals(sender as ToolStripButton))
                Selected = "text";
            if (btnBezier.Equals(sender as ToolStripButton))
                Selected = "bezier";
            if (btnColor.Equals(sender as ToolStripButton))
            {
                ColorDialog c = new ColorDialog();
                c.AnyColor = true;
                c.ShowDialog();

                currentColor = new Color(c.Color.R, c.Color.G, c.Color.B); 
            }
                
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string Coms = "";
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].GetType() == typeof(Line)) {
                    Line l = sprites[i] as Line;
                    Coms += string.Format("l,{0},{1},{2},{3},{4},{5},{6};",
                        l.start.X,
                        l.start.Y,
                        l.end.X,
                        l.end.Y,
                        l.color.R,
                        l.color.G,
                        l.color.B);
                }
                else if (sprites[i].GetType() == typeof(CircleShape)) {
                    CircleShape c = sprites[i] as CircleShape;
                    Coms += string.Format("c,{0},{1},{2},{3},{4},{5};",
                        c.Position.X+c.Radius,
                        c.Position.Y+c.Radius,
                        c.Radius,
                        c.OutlineColor.R,
                        c.OutlineColor.G,
                        c.OutlineColor.B);
                }
                else if(sprites[i].GetType() == typeof(Text)) {
                    Text t = sprites[i]  as Text;
                    Coms += string.Format("t,{0},{1},{2},{3},{4},{5},{6};",
                        t.DisplayedString,
                        t.Position.X,
                        t.Position.Y,
                        t.CharacterSize,
                        t.FillColor.R,
                        t.FillColor.G,
                        t.FillColor.B);
                }
                else if(sprites[i].GetType() == typeof(BezierCurve)) {
                    BezierCurve b = sprites[i] as BezierCurve;
                    Coms += string.Format("b,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10};",
                        b.start.X,b.start.Y,
                        b.end.X,b.end.Y,
                        b.starthandle.X,b.starthandle.Y,
                        b.endhandle.X,b.endhandle.Y,
                        b.color.R,b.color.G,b.color.B);
                }
                File.WriteAllText("test.frvc", Coms);
            }
        }
    }
    public struct DrawingCurrent
    {
        public Drawable drawable;
        public DrawingCurrent(Drawable _drawable)
        {
            drawable = _drawable;
        }
        public static bool operator ==(DrawingCurrent c1, object c2)
        {
            return c2 == null;
        }
        public static bool operator !=(DrawingCurrent c1, object c2)
        {
            return c2 != null;
        }
    }
}

