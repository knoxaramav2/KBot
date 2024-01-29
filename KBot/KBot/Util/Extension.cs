using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KBot.Util
{
    public static class ExtendRect
    {
        public static Rectangle SetCenter(this Rectangle rect, Point center) 
        {
            rect.X = center.X - rect.Width/2;
            rect.Y = center.Y - rect.Height/2;
            return rect;
        }
    }
}
