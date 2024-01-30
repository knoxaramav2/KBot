using KBot.Depots;
using KBot.State;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace KBot.UI
{
    internal class DevBox : IControlLoop
    {
        BotEntity ebot;
        GameCtxState RetVal;
        SpriteBatch DrawCtx;
        Texture2D CenterX;
        Rectangle Center;

        public DevBox() {
            Debug.WriteLine("SHOW DEVBOX");
            var bot = FabDepot.Depots.Get("DEVBOT");
            ebot = new BotEntity(bot, UIO.ScreenCenter);
            RetVal = GameCtxState.NoChange;
            DrawCtx = Providers.DrawCtx;
            CenterX = Providers.Sprites.Get("Cross");
            Center = new Rectangle(0, 0, 50, 50).SetCenter(UIO.ScreenCenter);
        }

        public void Draw()
        {
            DrawCtx.Draw(CenterX, Center, Color.Red);

            ebot.Draw();
            
            //ebot.Draw();
        }

        public GameCtxState Update(KeyboardState kbst, MouseState mst)
        {
            ebot.ActionIO(kbst, mst);
            ebot.Update();
            if (kbst.IsKeyDown(Keys.Escape)) { RetVal = GameCtxState.MainMenu; }

            return RetVal;
        }
    }
}
