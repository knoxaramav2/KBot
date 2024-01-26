using KBot.Factories;
using KBot.State;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KBot.UI
{
    internal class GameLoop
    {
        private BotFactory Factory;

        public GameLoop() {
            var winSz = Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
            var center = new Point(winSz.X/2, winSz.Y/2);

            Factory = new BotFactory();
        }

        public GameCtxState Update(KeyboardState kbst, MouseState mst)
        {
            
            //Bot.ActionIO(kbst, mst);

            return GameCtxState.NoChange;
        }

        public void Draw()
        {
            //Bot.Draw();
        }
    }
}
