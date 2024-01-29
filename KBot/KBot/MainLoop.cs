using KBot.UI;
using KBot.Util;
using KBot.Depots;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using KBot.State;

namespace KBot
{
    public class MainLoop : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _drawCtx;
        private GameCtxState _state;
        private IControlLoop _currMenu;

        readonly GameCtxState StartState = GameCtxState.DevBox;

        public MainLoop(string[] args)
        {
            Config.Init(args);
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Debug.WriteLine("STARTING...");
            _drawCtx = new SpriteBatch(GraphicsDevice);
            Providers.Init(_graphics, _drawCtx, Content);
            _state = StartState;
            SetStateControl(_state);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //ComponentDepot.Depots.Load();
            //FabDepot.Depots.Load();
        }

        protected void SetStateControl(GameCtxState newState)
        {
            switch (newState)
            {
                case GameCtxState.MainMenu: { _currMenu = new MainMenu(); break; }
                case GameCtxState.NewGame: { _currMenu = new NewGameMenu(); break; }
                case GameCtxState.LoadGame: { _currMenu = new LoadMenu(); break; }
                case GameCtxState.GameLoop: { _currMenu = new GameLoop(); break; }
                case GameCtxState.HomeScreen: { _currMenu = new HomeScreen(); break; }
                case GameCtxState.Pause: { _currMenu = new MainMenu(); break; }
                case GameCtxState.DevBox: { _currMenu = new DevBox(); break; }
                default:
                    Debug.WriteLine($">>> {newState}");
                    break;
            }

            _state = newState;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kbst = Keyboard.GetState();
            var mst = Mouse.GetState();
            var res = GameCtxState.NoChange;

            switch (_state)
            {
                case GameCtxState.MainMenu:
                case GameCtxState.NewGame:
                case GameCtxState.LoadGame:
                case GameCtxState.Pause:
                case GameCtxState.Settings:
                case GameCtxState.HomeScreen:
                case GameCtxState.GameLoop:
                case GameCtxState.DevBox:
                    res = _currMenu.Update(kbst, mst);
                    break;
                case GameCtxState.Exit:
                    Exit();
                    break;
            }

            if (res != GameCtxState.NoChange)
            {
                SetStateControl(res);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _drawCtx.Begin();

            switch (_state)
            {
                case GameCtxState.MainMenu:
                case GameCtxState.NewGame:
                case GameCtxState.LoadGame:
                case GameCtxState.Pause:
                case GameCtxState.Settings:
                case GameCtxState.HomeScreen:
                case GameCtxState.GameLoop:
                case GameCtxState.DevBox:
                    _currMenu.Draw();
                    break;
            }
                
            _drawCtx.End();

            base.Draw(gameTime);
        }
    }
}
