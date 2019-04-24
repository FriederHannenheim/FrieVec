using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace FrieVec
{
    class Imageutillity
    {
        public uint W, H;
        public static void DrawBezier(Vector2f st,Vector2f en,Vector2f p1,Vector2f p2,Color color,ref Image img)
        {
            int L = (int)((st - p1 + en - p2).X + (st - p1 + en - p2).X)*3;
            foreach (Vector2f duuuuuu in CalcCubicBezier(st,en,p1,p2,L))
            {
                img.SetPixel((uint)duuuuuu.X, (uint)duuuuuu.Y, color);
            }
        }
        static List<Vector2f> CalcCubicBezier(
        Vector2f start,
        Vector2f end,
        Vector2f startControl,
        Vector2f endControl,
        int numSegments)
        {
            List<Vector2f> ret = new List<Vector2f>();
            if (numSegments < 1) // Any points at all?
                return ret;

            ret.Add(start); // First point is fixed
            float p = 1.0f / numSegments;
            float q = p;
            for (int i = 1; i < numSegments; i++, p += q) // Generate all between
                ret.Add(p * p * p * (end + 3.0f * (startControl - endControl) - start) +
                              3.0f * p * p * (start - 2.0f * startControl + endControl) +
                              3.0f * p * (startControl - start) + start);
            ret.Add(end); // Last point is fixed
            return ret;
        }
        public void Fill(int x, int y, Color color, ref Image image)
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
        public void DrawLine(int x1, int y1, int x2, int y2, Color color, ref Image image)
        {
            if (x2 < 0 || x2 > W || y2 < 0 || y2 > H)
                return;
            int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
            int dy = Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {

                image.SetPixel((uint)x1, (uint)y1, color);
                if (x1 == x2 && y1 == y2) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x1 += sx; }
                if (e2 < dy) { err += dx; y1 += sy; }
            }
        }
        public void DrawText(int x, int y, String text, float size, Color color, ref Text curtxt, ref List<Drawable> Lüscht, float Rotation)
        {
            if (curtxt == null)
            {
                curtxt = new Text();
                Lüscht.Add(curtxt);
                curtxt.DisplayedString = text;
                curtxt.Font = new SFML.Graphics.Font(Form1.SolutionLoc + "/Assets/arial.ttf");
                curtxt.FillColor = color;
            }

            curtxt.Rotation = Rotation;
            curtxt.Position = new Vector2f(x, y);
            curtxt.Scale = new Vector2f(size, size);


        }
        public void DrawCircle(int x_centro, int y_centro, uint r, Color color, ref Image img)
        {
            uint x_centre = (uint)x_centro;
            uint y_centre = (uint)y_centro;
            IntRect Racks = new IntRect(0, 0, (int)W, (int)H);

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
    }
}
