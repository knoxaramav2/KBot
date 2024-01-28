using KBot.State;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KBot.UI
{
    enum HomeMenuType
    {
        NoChange,
        HomeScreen, BuildScreen, CompetitionScreen,
        InventoryScreen,

        //Exit scope
        World, Exit
    }

    public class HomeScreen
    {
        HomeMenuType Type;
        GameCtxState RetVal;

        readonly HomeScreenMenu MainScreen;
        readonly BuildScreenMenu BuildScreen;
        readonly CompetitionScreenMenu CompetitionScreen;
        readonly InventoryScreenMenu InventoryScreen;

        public HomeScreen()
        {
            Type = HomeMenuType.HomeScreen;
            RetVal = GameCtxState.NoChange;

            MainScreen = new();
            BuildScreen = new();
            CompetitionScreen = new();
            InventoryScreen = new();
        }

        public GameCtxState Update(KeyboardState kbst, MouseState mst)
        {
            var res = HomeMenuType.NoChange;
            switch (Type)
            {
                case HomeMenuType.HomeScreen:
                    res = MainScreen.Update(kbst, mst); break;
                case HomeMenuType.BuildScreen:
                    res = BuildScreen.Update(kbst, mst); break;
                case HomeMenuType.CompetitionScreen:
                    res = CompetitionScreen.Update(kbst, mst); break;
                case HomeMenuType.InventoryScreen:
                    res = InventoryScreen.Update(kbst, mst); break;
                case HomeMenuType.Exit:
                    res = HomeMenuType.Exit;
                    break;
            }

            if (res != HomeMenuType.NoChange)
            {
                switch (res)
                {
                    case HomeMenuType.Exit: RetVal = GameCtxState.MainMenu; break;
                    case HomeMenuType.World: RetVal = GameCtxState.GameLoop; break;
                    default: Type = res; break;
                }
            }

            return RetVal;
        }

        public void Draw()
        {
            switch (Type)
            {
                case HomeMenuType.HomeScreen:
                    MainScreen.Draw(); break;
                case HomeMenuType.BuildScreen:
                    BuildScreen.Draw(); break;
                case HomeMenuType.CompetitionScreen:
                    CompetitionScreen.Draw(); break;
                case HomeMenuType.InventoryScreen:
                    InventoryScreen.Draw(); break;
            }
        }
    }

    class HomeScreenMenu : Menu
    {
        private HomeMenuType RetVal { get; set; }

        private void LaunchWorld()
        {
            GameState.State.Avatar = new Player();
            RetVal = HomeMenuType.World;
        }

        protected override void InitComponents()
        {
            var nameLbl = new Label(this, text: GameState.State.Player.Name);

            var invBtn = new Button(this, text:"Inventory");
            var buildBtn = new Button(this, text: "Build Bot");
            var compBtn = new Button(this, text: "Competition");
            var worldBtn = new Button(this, text: "World", clickCallback: () => LaunchWorld());
            var exit = new Button(this, text: "Exit",
                clickCallback: () => { RetVal = HomeMenuType.Exit; }
                );

            Insert(nameLbl, new Point(0, 0), Align.CC);
            Insert(invBtn, new Point(0, 1), Align.CC);
            Insert(buildBtn, new Point(1, 1), Align.CC);
            Insert(compBtn, new Point(0, 2), Align.CC);
            Insert(worldBtn, new Point(1, 2), Align.CC);
            Insert(worldBtn, new Point(0, 3), Align.CC);

            base.InitComponents();
        }

        public HomeScreenMenu():base(GeoTypes.GRID, margin:new Point(1,1))
        {
            var size = UIO.ScreenDim;
            RetVal = HomeMenuType.NoChange;
            Move(new Point(0, 0), size);
            InitComponents();
            Config(Color.Magenta, Providers.Sprites.Get("Box1"));
        }

        public new HomeMenuType Update(KeyboardState kbst, MouseState mst)
        {
            base.Update(kbst, mst);
            return RetVal;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }

    class BuildScreenMenu : Menu
    {
        readonly HomeMenuType RetVal;

        protected override void InitComponents()
        {
            base.InitComponents();
        }

        public BuildScreenMenu() : base(GeoTypes.GRID, margin: new Point(1, 1))
        {
            var size = Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
            RetVal = HomeMenuType.NoChange;
            Move(new Point(0, 0), size);
            InitComponents();
            Config(Color.ForestGreen, Providers.Sprites.Get("Box1"));
        }

        public new HomeMenuType Update(KeyboardState kbst, MouseState mst)
        {
            base.Update(kbst, mst);
            return RetVal;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }

    class CompetitionScreenMenu : Menu
    {
        readonly HomeMenuType RetVal;

        protected override void InitComponents()
        {
            base.InitComponents();
        }

        public CompetitionScreenMenu() : base(GeoTypes.GRID, margin: new Point(1, 1))
        {
            var size = Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
            RetVal = HomeMenuType.NoChange;
            Move(new Point(0, 0), size);
            InitComponents();
            Config(Color.Coral, Providers.Sprites.Get("Box1"));
        }

        public new HomeMenuType Update(KeyboardState kbst, MouseState mst)
        {
            base.Update(kbst, mst);
            return RetVal;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }

    class InventoryScreenMenu : Menu
    {
        readonly HomeMenuType RetVal;

        protected override void InitComponents()
        {
            base.InitComponents();
        }

        public InventoryScreenMenu() : base(GeoTypes.GRID, margin: new Point(1, 1))
        {
            var size = Providers.Graphics.GraphicsDevice.Viewport.Bounds.Size;
            RetVal = HomeMenuType.NoChange;
            Move(new Point(0, 0), size);
            InitComponents();
            Config(Color.Aqua, Providers.Sprites.Get("Box1"));
        }

        public new HomeMenuType Update(KeyboardState kbst, MouseState mst)
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
