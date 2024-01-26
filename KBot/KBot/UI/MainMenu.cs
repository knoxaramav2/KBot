using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KBot.UI
{
    public class MainMenu : Menu
    {
        readonly SpriteFont Font;
        private GameCtxState RetVal;

        protected override void InitComponents()
        {
            Button newBtn = new Button(text: "New Game",
                clickCallback: () => { System.Diagnostics.Debug.WriteLine("NEW"); RetVal = GameCtxState.NewGame; },
                releaseCallback: () => System.Diagnostics.Debug.WriteLine("RELEASE"));

            Button loadBtn = new Button(text: "Load Game",
                clickCallback: () => { System.Diagnostics.Debug.WriteLine("LOAD"); RetVal = GameCtxState.LoadGame; },
                releaseCallback: () => System.Diagnostics.Debug.WriteLine("RELEASE"));

            Button sttBtn = new Button(text: "Settings Game",
                clickCallback: () => { System.Diagnostics.Debug.WriteLine("SETTINGS"); RetVal = GameCtxState.Settings; },
                releaseCallback: () => System.Diagnostics.Debug.WriteLine("RELEASE"));

            Button exitBtn = new Button(text: "Exit",
                clickCallback: () => { System.Diagnostics.Debug.WriteLine("EXIT"); RetVal = GameCtxState.Exit; },
                releaseCallback: () => System.Diagnostics.Debug.WriteLine("RELEASE"));

            Insert(newBtn, new Point(0, 0));
            Insert(loadBtn, new Point(0, 1));
            Insert(sttBtn, new Point(0, 2));
            Insert(exitBtn, new Point(0, 3));

            Pack();
        }

        public MainMenu() : base(GeoTypes.GRID, margin:new Point(1,1)) {
            var size = Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
            RetVal = GameCtxState.NoChange;
            Move(new Point(0, 0), size);
            Font = Providers.Fonts.Get();
            InitComponents();
            Config(Color.Red, Providers.Sprites.Get("Box1"));
        }

        public override GameCtxState Update(KeyboardState kbst, MouseState mst)
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
