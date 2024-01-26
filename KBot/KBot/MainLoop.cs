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

        private GameState _state;
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
            _state = GameState.GameLoop;
            _drawCtx = new SpriteBatch(GraphicsDevice);
            Providers.Init(_graphics, _drawCtx, Content);

            _ = Depots.Depots.Depot;

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
            var res = GameState.NoChange;

            switch (_state)
            {
                case GameState.MainMenu:
                    res = _mainMenu.Update(kbst, mst);
                    break;
                case GameState.NewGame:
                    res = _newMenu.Update(kbst, mst);
                    break;
                case GameState.LoadGame:
                    break;
                case GameState.GameLoop:
                    res = _gameLoop.Update(kbst, mst);
                    break;
                case GameState.Pause:
                    break;
                case GameState.Settings:
                    break;
                case GameState.Exit:
                    Exit();
                    break;
            }

            if (res != GameState.NoChange)
            {
                switch(res)
                {
                    case GameState.MainMenu:    { _mainMenu = new(); break; }
                    case GameState.NewGame:     { _newMenu = new(); break; }
                    case GameState.LoadGame:    { res = GameState.MainMenu; break; }
                    case GameState.GameLoop:    { _gameLoop = new(); break; }
                    case GameState.Pause:       { res = GameState.MainMenu; break; }
                    case GameState.Settings:    { res = GameState.MainMenu; break; }
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
                case GameState.MainMenu:
                    _mainMenu.Draw();
                    break;
                case GameState.NewGame:
                    _newMenu.Draw();
                    break;
                case GameState.LoadGame:
                    break;
                case GameState.GameLoop:
                    _gameLoop.Draw();
                    break;
                case GameState.Pause: 
                    break;
                
            }
                
            _drawCtx.End();

            base.Draw(gameTime);
        }
    }
}
