using KBot.State;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.UI
{
    public class LoadMenu : Menu, IControlLoop
    {
        GameCtxState RetVal;
        string SelectPath;

        private void SelectFile(string path)
        {
            SelectPath = path;
        }

        private void LoadFile()
        {
            if (string.IsNullOrEmpty(SelectPath)) { return; }
            GameState.State = GameState.Load(SelectPath);
            RetVal = GameCtxState.HomeScreen;
        }

        protected override void InitComponents()
        {
            var path = UFile.SavesDir;
            var gameFiles = Directory.GetFiles(path, "*.sav");

            int y = 0;
            for (; y < gameFiles.Length; ++y)
            {
                var file = gameFiles[y];
                var btn = new Button(this,
                    text: Path.GetFileNameWithoutExtension(file),
                    clickCallback: () => SelectFile(file),
                    align: Align.CC
                    );
                Insert(btn, new Point(1, y), Align.CC);
            }

            var loadBtn = new Button(text:"Load", clickCallback: () => { LoadFile(); });
            var cancelButton = new Button(text:"Cancel",
                clickCallback: () => RetVal = GameCtxState.MainMenu);

            //++y;
            Insert(loadBtn, new Point(0, y));
            Insert(cancelButton, new Point(2, y));

            base.InitComponents();
        }

        public LoadMenu() : base(GeoTypes.GRID, margin:new Point(1,1))
        {
            var size = Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
            RetVal = GameCtxState.NoChange;
            SelectPath = string.Empty;
            Move(new Point(0, 0), size);
            InitComponents();
            Config(Color.Green, Providers.Sprites.Get("Box1"));
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
