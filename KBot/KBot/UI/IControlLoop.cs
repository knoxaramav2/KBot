using KBot.Util;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.UI
{
    public interface IControlLoop
    {
        public GameCtxState Update(KeyboardState kbst, MouseState mst);
        public void Draw();
    }
}
