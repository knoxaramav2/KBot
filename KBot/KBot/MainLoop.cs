using KBot.UI;
using KBot.Util;
using KBot.Depots;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace KBot
{
    public class MainLoop : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _drawCtx;
        private SpriteFont _font;
        private Texture2D shell;

        private GameCtxState _state;
        private MainMenu _mainMenu;
        private NewGameMenu _newMenu;
        private GameLoop _gameLoop;

        public MainLoop()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Debug.WriteLine("STARTING...");
            _state = GameCtxState.MainMenu;
            _drawCtx = new SpriteBatch(GraphicsDevice);
            Providers.Init(_graphics, _drawCtx, Content);

            //Init depots
            _ = Depots.ComponentDepot.Depots;
            _ = Depots.FabDepot.Depots;

            _mainMenu = new();
            _gameLoop = new();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            shell = Content.Load<Texture2D>("DevShell1");
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
                    res = _mainMenu.Update(kbst, mst);
                    break;
                case GameCtxState.NewGame:
                    res = _newMenu.Update(kbst, mst);
                    break;
                case GameCtxState.LoadGame:
                    break;
                case GameCtxState.GameLoop:
                    res = _gameLoop.Update(kbst, mst);
                    break;
                case GameCtxState.Pause:
                    break;
                case GameCtxState.Settings:
                    break;
                case GameCtxState.Exit:
                    Exit();
                    break;
            }

            if (res != GameCtxState.NoChange)
            {
                switch(res)
                {
                    case GameCtxState.MainMenu:    { _mainMenu = new(); break; }
                    case GameCtxState.NewGame:     { _newMenu = new(); break; }
                    case GameCtxState.LoadGame:    { res = GameCtxState.MainMenu; break; }
                    case GameCtxState.GameLoop:    { _gameLoop = new(); break; }
                    case GameCtxState.Pause:       { res = GameCtxState.MainMenu; break; }
                    case GameCtxState.Settings:    { res = GameCtxState.MainMenu; break; }
                    default:
                        Debug.WriteLine($">>> {res}");
                        break;
                }

                _state = res;
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
                    _mainMenu.Draw();
                    break;
                case GameCtxState.NewGame:
                    _newMenu.Draw();
                    break;
                case GameCtxState.LoadGame:
                    break;
                case GameCtxState.GameLoop:
                    _gameLoop.Draw();
                    break;
                case GameCtxState.Pause: 
                    break;
                
            }
                
            _drawCtx.End();

            base.Draw(gameTime);
        }
    }
}
