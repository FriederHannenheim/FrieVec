using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
namespace FrieVec_remaster
{
    static class Drawables
    {
        public static bool CircleContains(Vector2i position, int radius, Vector2i checkpos)
        {
            int dx = Math.Abs(checkpos.X - position.X);
            int dy = Math.Abs(checkpos.Y - position.Y);

            if (dx > radius)
                return false;
            if (dy > radius)
                return false;
            if (dx + dy <= radius)
                return true;
            return false;
        }
        public static Sprite Line(Vector2i end,Color color)
        {
            Image img = new Image((uint)end.X,(uint)end.Y);
            int dx = Math.Abs(end.X), sx = 1;
            int dy = Math.Abs(end.Y), sy = 1;
            int x1=0, y1 = 0;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {

                img.SetPixel((uint)x1, (uint)y1, color);
                if (x1 == end.X && y1 == end.Y) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x1 += sx; }
                if (e2 < dy) { err += dx; y1 += sy; }
            }
            return new Sprite(new Texture(img));
            
        }
        public static Drawable parseCommand(string[] subCommands)
        {
            switch (subCommands[0])
            {
                case "l":
                    Line s = new Line(
                        new Vector2f(int.Parse(subCommands[1]), int.Parse(subCommands[2])),
                        new Vector2f(int.Parse(subCommands[3]), int.Parse(subCommands[4])),
                        new Color(
                            byte.Parse(subCommands[5]),
                            byte.Parse(subCommands[6]),
                            byte.Parse(subCommands[7])
                        )
                    );
                    s.Update();
                    return s;
                case "c":
                    CircleShape c = new CircleShape(int.Parse(subCommands[3]));
                    c.Position = new Vector2f(int.Parse(subCommands[1]) - int.Parse(subCommands[3]), int.Parse(subCommands[2]) - int.Parse(subCommands[3]));
                    c.OutlineColor = new Color(byte.Parse(subCommands[4]), byte.Parse(subCommands[5]), byte.Parse(subCommands[6]));
                    c.OutlineThickness = 1;
                    c.FillColor = Color.Transparent;
                    string[] addentum = subCommands.Skip(6).ToArray();
                    byte Opacity = 255;
                    foreach (string a in addentum)
                    {
                        string[] subaddentum = a.Split(':');
                        switch (subaddentum[0])
                        {
                            case "f":
                                c.FillColor = new Color(byte.Parse(subaddentum[1]), byte.Parse(subaddentum[2]), byte.Parse(subaddentum[3]));
                                break;
                            case "o":
                                Opacity = byte.Parse(subaddentum[1]);
                                break;
                        }
                    }
                    if (Opacity != 255)
                    {
                        Color oc = new Color(c.OutlineColor);
                        oc.A = (byte)Opacity;
                        c.OutlineColor = oc;
                        if (c.FillColor.A != 0)
                        {
                            Color fc = new Color(c.FillColor);
                            fc.A = Opacity;
                            c.FillColor = fc;
                        }
                    }
                    return c;
                case "t":
                    Font font = new Font(Properties.Resources.aileron_bold);
                    SFML.Graphics.Text text = new Text(subCommands[1], font, uint.Parse(subCommands[4]));
                    text.Position = new Vector2f(int.Parse(subCommands[2]), int.Parse(subCommands[3]));
                    text.FillColor = new Color(byte.Parse(subCommands[5]), byte.Parse(subCommands[6]), byte.Parse(subCommands[7]));
                    
                    return text;
                case "b":
                    BezierCurve b = new BezierCurve(new Vector2f(int.Parse(subCommands[1]), int.Parse(subCommands[2])), new Vector2f(int.Parse(subCommands[3]), int.Parse(subCommands[4])), new Vector2f(int.Parse(subCommands[5]), int.Parse(subCommands[6])), new Vector2f(int.Parse(subCommands[7]), int.Parse(subCommands[8])), new Color(byte.Parse(subCommands[9]), byte.Parse(subCommands[10]), byte.Parse(subCommands[11])));
                    b.Update();
                    return b;
            }

            return null;
        }
    }
    class BezierCurve: Drawable
    {
        public Color color;
        public Vector2f start;
        public Vector2f end;
        public Vector2f starthandle;
        public Vector2f endhandle;
        public string handling = "";
        List<Vertex> vertices;


        public BezierCurve(Vector2f _start,Vector2f _end,Vector2f _starthandle,Vector2f _endhandle,Color _color)
        {
            start = _start;
            end = _end;
            starthandle = _starthandle;
            endhandle = _endhandle;
            color = _color;
        }
        public void Update()
        {
            int L = (int)Math.Sqrt(Math.Pow(start.X + starthandle.X, 2) + Math.Pow(start.Y + starthandle.Y, 2));
            L += (int)Math.Sqrt(Math.Pow(starthandle.X + endhandle.X, 2) + Math.Pow(starthandle.Y + endhandle.Y, 2));
            L += (int)Math.Sqrt(Math.Pow(endhandle.X + end.X, 2) + Math.Pow(endhandle.Y + end.Y, 2));
            L += (int)Math.Sqrt(Math.Pow(start.X + end.X, 2) + Math.Pow(start.Y + end.Y, 2));
            vertices = new List<Vertex>();
            if (L < 1) // Any points at all?
                return;

            vertices.Add(new Vertex(start,color)); // First point is fixed
            float p = 1.0f / L;
            float q = p;
            for (int i = 1; i < L; i++, p += q) // Generate all between
                vertices.Add(new Vertex(p * p * p * (end + 3.0f * (starthandle - endhandle) - start) +
                              3.0f * p * p * (start - 2.0f * starthandle + endhandle) +
                              3.0f * p * (starthandle - start) + start,color));
            vertices.Add(new Vertex(end,color)); // Last point is fixed

        }
        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(vertices.ToArray(),PrimitiveType.Points,states);
            if(handling != "")
            {
                CircleShape s = new CircleShape(10);
                CircleShape e = new CircleShape(10);
                s.Position = starthandle - new Vector2f(10, 10);
                e.Position = endhandle - new Vector2f(10, 10);
                s.FillColor = Color.Transparent;
                s.OutlineThickness = 1;
                e.FillColor = Color.Transparent;
                e.OutlineThickness = 1;
                target.Draw(s);
                target.Draw(e);
            }
        }


    }
    class Line : Drawable
    {
        public Vector2f start;
        public Vector2f end;
        public Color color;
        Vertex[] vertices = new Vertex[2];
        public void Update()
        {
            vertices[0] = new Vertex(start, color);
            vertices[1] = new Vertex(end, color);
        }
        public Line(Vector2f _start,Vector2f _end,Color _color)
        {
            start = _start;
            end = _end;
            color = _color;

        }
        public void Draw(RenderTarget target,RenderStates states)
        {

            target.Draw(vertices.ToArray(), PrimitiveType.Lines, states);
        }
    }
}
