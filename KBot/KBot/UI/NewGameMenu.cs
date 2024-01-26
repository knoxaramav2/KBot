using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KBot.UI
{
    public class NewGameMenu : Menu
    {
        readonly SpriteFont Font;
        private GameState RetVal;


        private void InitComponents()
        {
            Button startBtn = new Button(text: "Start",
                clickCallback: () => { System.Diagnostics.Debug.WriteLine("START"); RetVal = GameState.GameLoop; },
                releaseCallback: () => System.Diagnostics.Debug.WriteLine("RELEASE"));

            Button cancelBtn = new Button(text: "Cancel",
                clickCallback: () => { System.Diagnostics.Debug.WriteLine("CANCEL"); RetVal = GameState.MainMenu; },
                releaseCallback: () => System.Diagnostics.Debug.WriteLine("RELEASE"));

            Insert(startBtn, new Point(0, 0));
            Insert(cancelBtn, new Point(1, 0));

            Pack();
        }

        public NewGameMenu() : base(GeoTypes.GRID, margin: new Point(1, 1))
        {
            var size = Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
            RetVal = GameState.NoChange;
            Move(new Point(0, 0), size);
            Font = Providers.Fonts.Get();
            InitComponents();
            Config(Color.Red, Providers.Sprites.Get("Box1"));
        }

        public override GameState Update(KeyboardState kbst, MouseState mst)
        {
            base.Update(kbst, mst);
            return RetVal;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
