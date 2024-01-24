using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.UI
{
    public class MainMenu : Menu
    {
        readonly SpriteFont Font;

        private void InitComponents()
        {
            Button testBtn = new Button(text:"Test", 
                clickCallback: () => System.Diagnostics.Debug.WriteLine("CLICK"),
                releaseCallback: () => System.Diagnostics.Debug.WriteLine("RELEASE"));

            Insert(testBtn);
        }

        public MainMenu() : base(GeoTypes.PACK) {
            Font = Providers.Fonts.Get();
            InitComponents();
        }

        public override void Draw()
        {
            //DrawCtx.DrawString(Font, "Test value",
            //    new Vector2(100, 100), Color.Black);
            base.Draw();
        }
    }
}
