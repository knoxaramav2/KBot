using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.Util
{
    public static class UIO
    {
        public static Point ScreenDim => Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
        public static Point ScreenCenter => Providers.Graphics.GraphicsDevice.Viewport.Bounds.Center;
    }
}
