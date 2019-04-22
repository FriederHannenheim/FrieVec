using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FrieVec
{
    public static class CreateButton
    {
        public static Button NewButton(Size s,Point pos,Image icon,Color back,ImageLayout layout = ImageLayout.Stretch)
        {
            Button but = new Button();
            but.Size = s;
            but.Location = pos;
            but.BackgroundImage = icon;
            but.BackgroundImageLayout = layout;
            but.BackColor = back;
            return but;
        }   
        public static Button NewButton(Size s, Point pos, String text,Color back)
        {
            Button but = new Button();


            return but;
        }
    }
}
