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
    public class NewGameMenu : Menu, IControlLoop
    {
        readonly SpriteFont Font;
        private GameCtxState RetVal;

        private bool ValidValues = false;
        private string SaveName;
        private string PlayerName;

        private bool Validate()
        {
            ValidValues = !string.IsNullOrEmpty(SaveName) && !string.IsNullOrEmpty(PlayerName);
            return ValidValues;
        }

        private void InitGameData()
        {
            Debug.WriteLine($">>>> {PlayerName}");
            Debug.WriteLine($">>>> {SaveName}");

            if (!Validate())
            {

                return;
            }

            var gameData = GameState.NewGame(SaveName, PlayerName);
            gameData.Save();
            RetVal = GameCtxState.HomeScreen;
        }

        protected override void InitComponents()
        {
            Label gameLbl = new Label(text:"Save Name: ");
            TextField gameNameField = new(placeholder: "<Name>", 
                bgClr: Color.LightGray, fgClr: Color.Cyan,
                callback: (string nval) => { SaveName = nval; });
            Label playerLbl = new Label(text: "Player Name: ");
            TextField playerNameField = new(placeholder: "<Name>",
                bgClr: Color.LightGray, fgClr: Color.Cyan,
                callback: (string nval) => { PlayerName = nval; });
            
            Button startBtn = new(text: "Start",
                clickCallback: () => { Debug.WriteLine("START"); InitGameData(); },
                releaseCallback: () => Debug.WriteLine("RELEASE"));
            Button cancelBtn = new(text: "Cancel",
                clickCallback: () => { Debug.WriteLine("CANCEL"); RetVal = GameCtxState.MainMenu; },
                releaseCallback: () => Debug.WriteLine("RELEASE"));

            Insert(gameLbl, new Point(0, 0));
            Insert(gameNameField, new Point(1, 0));
            Insert(playerLbl, new Point(0, 1));
            Insert(playerNameField, new Point(1, 1));
            Insert(startBtn, new Point(0, 2));
            Insert(cancelBtn, new Point(2, 2));

            base.InitComponents();
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
