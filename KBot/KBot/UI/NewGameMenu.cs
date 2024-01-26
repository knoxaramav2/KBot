using KBot.State;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Linq;

namespace KBot.UI
{
    public class NewGameMenu : Menu
    {
        readonly SpriteFont Font;
        private GameCtxState RetVal;
        private string SaveName;
        private string TestVal;

        private void InitGameData()
        {
            Debug.WriteLine($">>>> {TestVal}");
            var gameData = GameState.State;
            SaveName = "Test.sav";
            gameData.SaveName = SaveName;
            gameData.Save();
        }

        protected override void InitComponents()
        {
            TextField nameField = new(text: "<Name>", 
                bgClr: Color.LightGray, fgClr: Color.Cyan,
                callback: (string nval) => { TestVal = nval; });
            Button startBtn = new(text: "Start",
                clickCallback: () => { Debug.WriteLine("START"); RetVal = GameCtxState.GameLoop; InitGameData(); },
                releaseCallback: () => Debug.WriteLine("RELEASE"));

            Button cancelBtn = new(text: "Cancel",
                clickCallback: () => { Debug.WriteLine("CANCEL"); RetVal = GameCtxState.MainMenu; },
                releaseCallback: () => Debug.WriteLine("RELEASE"));

            Insert(nameField, new Point(1, 0));
            Insert(startBtn, new Point(0, 1));
            Insert(cancelBtn, new Point(2, 1));

            Pack();
        }

        public NewGameMenu() : base(GeoTypes.GRID, margin: new Point(1, 1))
        {
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
