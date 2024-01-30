using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.UI
{
    internal class StageMenu : Menu, IControlLoop
    {
        readonly GameCtxState RetVal;

        public StageMenu() 
            : base(GeoTypes.ALIGN, margin:new Point(1,1))
        {
            var size = Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
            RetVal = GameCtxState.NoChange;
            Move(new Point(0, 0), size);
            InitComponents();
        }

        protected override void InitComponents()
        {
            var botsBtn = new Button(this, text:"View Bots");
            var statsBtn = new Button(this, text: "Stats");
            var launchBtn = new Button(this, text: "Launch");

            Insert(statsBtn, new Point(0, 0));
            Insert(botsBtn, new Point(0, 2));
            Insert(launchBtn, new Point(1, 1));

            base.InitComponents();
        }

        public override GameCtxState Update(KeyboardState kbst, MouseState mst)
        {
            base.Update();
            return RetVal;
        }
    }
}
