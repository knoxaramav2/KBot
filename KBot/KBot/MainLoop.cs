using KBot.UI;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        enum GameState { MainMenu, Pause, Normal }

        public MainLoop()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            System.Diagnostics.Debug.WriteLine("STARTING...");
            // TODO: Add your initialization logic here
            _state = GameState.MainMenu;
            _drawCtx = new SpriteBatch(GraphicsDevice);
            Providers.Init(_graphics, _drawCtx, Content);
            _mainMenu = new();

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

            switch (_state)
            {
                case GameState.MainMenu:
                    _mainMenu.Update(kbst, mst);
                    break;
                case GameState.Pause: break;
                case GameState.Normal: break;
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
                case GameState.Pause: break;
                case GameState.Normal: break;
            }
                
            //_drawCtx.Draw(shell, new Rectangle(0, 0, 64, 64), 
            //    new Rectangle(0, 0, shell.Width, shell.Height), Color.White);
            _drawCtx.End();

            base.Draw(gameTime);
        }
    }
}
