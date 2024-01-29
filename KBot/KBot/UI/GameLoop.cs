using KBot.State;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KBot.UI
{
    internal class GameLoop : IControlLoop
    {
        private ControlSystem CSystem;
        private GameState State;

        private GameCtxState RetVal;

        public GameLoop() {
            CSystem = new();
            State = GameState.State;
            RetVal = GameCtxState.NoChange;
        }

        public GameCtxState Update(KeyboardState kbst, MouseState mst)
        {
            CSystem.Update(kbst, mst);
            return RetVal;
        }

        public void Draw()
        {
            CSystem.Draw();
        }
    }
}
