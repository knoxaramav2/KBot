using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBot.Util;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.VisualBasic;

namespace KBot.UI
{
    public enum Align
    {
        TL, TC, TR,
        CL, CC, CR,
        BL, BC, BR
    }

    public class Control
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch DrawCtx;
        public Rectangle Dim;

        protected Color Color;
        protected Texture2D Image;

        protected Control Parent { get; private set; }

        public Control(Control parent=null, Texture2D img=null, Color? clr=null, Point? pos=null, Point? size=null) {

            clr ??= Color.Gray;
            pos ??= new Point(0, 0);
            size ??= new Point(64, 64);

            Parent = parent;
            Graphics = Providers.Graphics;
            DrawCtx = Providers.DrawCtx;
            Dim = new();
            Image = img;
            Color = (Color)clr;
            Set(pos, size);
        }

        public void Set(Point? pos = null, Point? size = null)
        {
            if (size != null) { Dim.Size = (Point)size; }
            if (pos != null)
            {
                Dim.X = pos.Value.X;
                Dim.Y = pos.Value.Y;
            }
        }

        public virtual bool InBounds(Point xy)
        {
            return Dim.Contains(xy);
        }

        public virtual void SetBackground(Texture2D img=null, Color?clr=null)
        {
            if (img != null) { Image = img; }
            if (clr != null) { Color = (Color)clr; }
        }

        public virtual void Draw() {
            if (Image != null)
            {
                DrawCtx.Draw(Image, Dim, Color);
            }
        }

        public virtual void Update()
        {

        }

        public virtual void Config(Color? clr=null, Texture2D img=null)
        {
            Color = clr ?? Color;
            Image = img ?? Image;
        }

        protected Vector2 AlignChild(Vector2 obj, Align align)
        {
            Vector2 pos = new();
            var margin_w = (Dim.Width - obj.X) / 2;
            var margin_h = (Dim.Height - obj.Y) / 2;

            switch (align)
            {
                case Align.TL: pos = new Vector2(Dim.Left, Dim.Top); break;
                case Align.TC: pos = new Vector2(Dim.Center.X + margin_w, Dim.Top); break;
                case Align.TR: pos = new Vector2(Dim.Right - obj.X, Dim.Top); break;

                case Align.CL: pos = new Vector2(Dim.Left, Dim.Top + margin_h); break;
                case Align.CC: pos = new Vector2(Dim.Left + margin_w, Dim.Top + margin_h); break;
                case Align.CR: pos = new Vector2(Dim.Right - obj.X, Dim.Top + margin_h); break;

                case Align.BL: pos = new Vector2(Dim.Left, Dim.Bottom - obj.Y); break;
                case Align.BC: pos = new Vector2(Dim.Left + margin_w, Dim.Bottom - obj.Y); break;
                case Align.BR: pos = new Vector2(Dim.Right - obj.X, Dim.Bottom - obj.Y); break;

                default:
                    pos = new Vector2();
                    break;
            }

            return pos;
        }
    }

    public class Label : Control
    {
        public string Text { get; set; }
        private Align Align { get; set; }
        private SpriteFont Font { get; set; }
        private Color TextColor { get; set; }

        public Label(Control parent = null, SpriteFont font=null,
            Texture2D bgImg = null, Color? bgClr = null,
            Color? txtClr=null, Point? pos = null, Point? size = null, 
            string text="", Align align=Align.CC) 
            : base(parent, bgImg, bgClr, pos, size) {
            Text = text;
            Align = align;
            Font = font ?? Providers.Fonts.Get();
            TextColor = txtClr ?? Color.White;
        }

        public override void Draw()
        {
            base.Draw();

            var msr = Font.MeasureString(Text);
            Vector2 pos = AlignChild(msr, Align);
            DrawCtx.DrawString(Font, Text, pos, TextColor);
        }
    }

    public class Clickable : Control
    {
        public delegate void ClickCallback();
 
        private ClickCallback OnClick;
        private ClickCallback OnRelease;
        protected Texture2D PressedImg;
        protected Texture2D ReleasedImg;
        protected Color PressedColor;
        protected Color ReleasedColor;
        protected bool IsClicked;

        public Clickable(Control parent = null, 
            Texture2D baseImg = null, Color? baseClr = null, Texture2D pressImg = null, Color? pressClr = null,
            Point? pos = null, Point? size = null,
            ClickCallback clickCallback = null, ClickCallback releaseCallback = null) 
            : base(parent, baseImg, baseClr, pos, size)
        {
            ReleasedColor = Color;
            ReleasedImg = Image;
            PressedColor = pressClr ?? Color.DarkGray;
            PressedImg = pressImg ?? Image;

            SetCallback(clickCallback, releaseCallback);
            IsClicked = false;
        }

        public virtual void SetCallback(ClickCallback clickCallback=null, ClickCallback releaseCallback=null) {
            OnClick = clickCallback ?? OnClick;
            OnRelease = releaseCallback ?? OnRelease;
        }

        public virtual void Click()
        {
            IsClicked = true;
            Color = PressedColor;
            Image = PressedImg;
            OnClick?.Invoke();
        }

        public virtual void Release()
        {
            IsClicked = false;
            Color = ReleasedColor;
            Image = ReleasedImg;
            OnRelease?.Invoke();
        }
        
        public void Config(Color? pressClr=null, Texture2D pressImg=null, Color?relClr=null, Texture2D relImg=null)
        {
            base.Config(clr: relClr, img: relImg);
            if (pressClr !=  null) { PressedColor = (Color) pressClr; }
            if (pressImg != null) { PressedImg = pressImg; }
            if (relClr != null) { ReleasedColor = (Color)relClr; }
            if (relImg != null) { ReleasedImg = relImg; }
        }
    }

    public class Button : Clickable
    {
        Label Label;
        Texture2D ActiveImg;

        public Button(
            Control parent = null,
            Texture2D baseImg=null, Color? baseClr=null, Texture2D pressImg=null, Color? pressClr=null,
            Point? pos = null, Point? size = null, 
            ClickCallback clickCallback = null, ClickCallback releaseCallback = null, 
            string text="", Align align=Align.CC) 
            : base(parent, baseImg, baseClr, pressImg, pressClr, pos, size, clickCallback, releaseCallback)
        {
            ActiveImg = pressImg ?? Providers.Images.Get("Box1");
            Point dim = new((int)(Dim.Width * 0.9), (int)(Dim.Height * 0.9));
            var center = AlignChild(dim.ToVector2(), Align.CC);
            Label = new Label(align:align, size:dim, pos:center.ToPoint(), text:text, bgClr:pressClr, bgImg:pressImg);

            var box = Providers.Images.Get("Box1");
            baseImg ??= box;
            baseClr ??= Color.Gray;
            pressImg ??= box;
            pressClr ??= Color.DarkGray;

            Config(pressClr, pressImg, baseClr, baseImg);
        }

        public override void Draw()
        {
            base.Draw();
            Label.Draw();
        }

        public override void Update()
        {
            base.Update();
        }

        public void Click(bool clicked=false, bool shift = false, bool ctrl = false)
        {
            if (!clicked)
            {
                base.Release();
                return;
            }

            if (IsClicked) { return; }

            base.Click();
        }
    }

    public enum GeoTypes
    {
        GRID, COORD, PACK
    }

    public class Container : Control
    {
        GeoTypes Type;
        readonly protected List<Control> Items;
        
        public Container(GeoTypes Type) : base()
        {
            this.Type = Type;
            this.Items = new();
        }

        public virtual void Insert(Control item) {
            if (Items.Contains(item)) return;
            Items.Insert(0, item);
        }

        public override void Update()
        {
            foreach(var item in Items)
            {
                item.Update();
            }

            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            foreach (var item in Items)
            {
                item.Draw();
            }
        }
    }

    public enum MouseStates
    {
        PrevLMouse, CurrLMouse,
        PrevRMouse, CurrRMouse
    }

    public enum PressState
    {
        Click, Release, Hold, None
    }

    public struct CommonStateCheck
    {
        private bool[] MStates = { false, false, false, false }; 

        public CommonStateCheck()
        {
        }

        public void Update(MouseState mstate)
        {
            MStates[(int)MouseStates.PrevLMouse] = MStates[(int)MouseStates.CurrLMouse];
            MStates[(int)MouseStates.CurrLMouse] = mstate.LeftButton == ButtonState.Pressed;
            MStates[(int)MouseStates.PrevRMouse] = MStates[(int)MouseStates.CurrRMouse];
            MStates[(int)MouseStates.CurrRMouse] = mstate.RightButton == ButtonState.Pressed;
        }

        private PressState CompareState(bool prev, bool curr)
        {
            if (!prev && curr) { return PressState.Click; }
            if (prev && !curr) { return PressState.Release; }
            if (prev && curr) { return PressState.Hold; }
            return PressState.None;
        }

        public PressState CheckLMouse()
        {
            return CompareState(MStates[(int)MouseStates.PrevLMouse], MStates[(int)MouseStates.CurrLMouse]);
        }

        public PressState CheckRMouse()
        {
            return CompareState(MStates[(int)MouseStates.PrevRMouse], MStates[(int)MouseStates.CurrRMouse]);
        }
    }

    public class Menu : Container
    {
        public Control Current { get; set; } = null;

        private CommonStateCheck StateCheck { get; set; }

        public Menu(GeoTypes Type) : base(Type)
        {
            StateCheck = new();
        }

        public void Update(KeyboardState kbst, MouseState mst)
        {
            StateCheck.Update(mst);
            UpdateClick(mst);
            base.Update();
        }

        private void UpdateClick(MouseState mst)
        {
            var lmst = StateCheck.CheckLMouse();

            if (lmst == PressState.None) { return; }

            foreach(var item in Items)
            {
                if(!item.GetType().IsSubclassOf(typeof(Clickable))) { continue; }
                var curr = (Clickable)item;
                if (curr.InBounds(mst.Position))
                {
                    if ( Current != null && curr != Current)
                    {
                        if (Current.GetType().IsSubclassOf(typeof(Clickable))) 
                            { ((Clickable)Current).Release(); }
                    }
                    switch (lmst)
                    {
                        case PressState.Click: { curr.Click(); break; }
                        case PressState.Release: { curr.Release(); break; }
                        default: break;
                    }

                    Current = curr;
                }
            }
        }

        public virtual void OnType()
        {

        }
    }
}
