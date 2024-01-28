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
        private SpriteFont _font;
        private Texture2D shell;

        private GameCtxState _state;
        private Menu _currMenu;
        //private MainMenu _mainMenu;
        //private NewGameMenu _newMenu;
        private GameLoop _gameLoop;
        //private GameState _gameState;
        private HomeScreen _homeScreen;

        public MainLoop()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Debug.WriteLine("STARTING...");
            _drawCtx = new SpriteBatch(GraphicsDevice);
            Providers.Init(_graphics, _drawCtx, Content);

            _state = GameCtxState.LoadGame;
            _currMenu = new LoadMenu();

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
                case GameCtxState.NewGame:
                case GameCtxState.LoadGame:
                case GameCtxState.Pause:
                case GameCtxState.Settings:
                    res = _currMenu.Update(kbst, mst);
                    break;

                case GameCtxState.HomeScreen:
                    res = _homeScreen.Update(kbst, mst);
                    break;
                case GameCtxState.GameLoop:
                    res = _gameLoop.Update(kbst, mst);
                    break;
                case GameCtxState.Exit:
                    Exit();
                    break;
            }

            if (res != GameCtxState.NoChange)
            {
                switch(res)
                {
                    case GameCtxState.MainMenu: { _currMenu = new MainMenu(); break; }
                    case GameCtxState.NewGame:  { _currMenu = new NewGameMenu(); break; }
                    case GameCtxState.LoadGame: { _currMenu = new LoadMenu(); break; }
                    case GameCtxState.GameLoop: { _gameLoop = new GameLoop(); break; }
                    case GameCtxState.HomeScreen: { _homeScreen = new HomeScreen(); break; }
                    case GameCtxState.Pause:    { _currMenu = new MainMenu(); break; }
                    case GameCtxState.Settings: { _currMenu = new MainMenu(); break; }
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
                case GameCtxState.NewGame:
                case GameCtxState.LoadGame:
                case GameCtxState.Pause:
                case GameCtxState.Settings:
                    _currMenu.Draw();
                    break;

                case GameCtxState.HomeScreen:
                    _homeScreen.Draw();
                    break;
                case GameCtxState.GameLoop:
                    _gameLoop.Draw();
                    break;
            }
                
            _drawCtx.End();

            base.Draw(gameTime);
        }
    }
}
