using KBot.Components;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KBot.State
{
    public struct DrawableInfo
    {
        public Texture2D Sprite;
        public Color Color;
        public float Scale;

        public DrawableInfo(Texture2D sprite, Color color, float scale)
        {
            Sprite = sprite;
            Color = color;
            Scale = scale;
        }
    }

    interface IDrawable
    {
        DrawableInfo DrawInfo { get; set; }
    }

    public class Anchor
    {
        public double X;
        public double Y;

        public Anchor(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Anchor CenterOf(int w, int h)
        {
            return new Anchor(X + w / 2, Y + h / 2);
        }

        public Anchor TopLeft(int w, int h)
        {
            return new Anchor(X - w / 2, Y - h / 2);
        }

        public Anchor(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public Anchor(Vector2 v)
        {
            X = v.X;
            Y = v.Y;
        }

        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        public Vector2 ToVector2()
        {
            return new Vector2((float)X, (float)Y);
        }

        public static Anchor operator +(Anchor lval, Anchor rval) => new(lval.X + rval.X, lval.Y + rval.Y);
        public static Anchor operator -(Anchor lval, Anchor rval) => new(lval.X - rval.X, lval.Y - rval.Y);
    }

    public class PhysicsBody
    {
        protected PhysicsBody ParentBody;
        internal Anchor Pos { get; set; }
        protected Rectangle BBox;
        protected float Angle;
        protected float Vel;
        protected float RotVel;
        protected readonly float Acc;
        protected float Frc;
        protected readonly float RotAcc;
        protected float RotFrc;

        public PhysicsBody(
            PhysicsBody parent,
            Point offset, Point dim, 
            float rot, float mvAcc, float mvFrc, float rotAcc, float rotFrc) 
        {
            ParentBody = parent;
            Pos = new Anchor(offset);
            BBox = new Rectangle(Pos.TopLeft(dim.X, dim.Y).ToPoint(), dim);
            
            Angle = rot;
            Vel = 0;
            RotVel = 0;
            Acc = mvAcc;
            Frc = mvFrc;
            RotAcc = rotAcc;
            RotFrc = rotFrc;
        }

        public virtual void UpdateMotion()
        {
            Move();
            ApplyFriction();
        }

        protected virtual void ApplyFriction()
        {
            RotVel /= RotFrc;
            Vel /= Frc;
        }

        public virtual void Move(float prcAcc = 0f, float prcRotAcc = 0f)
        {
            Vel += UMath.Constain(Acc * prcAcc, -Acc, Acc);
            RotVel += UMath.Constain(RotAcc * prcRotAcc, -RotAcc, RotAcc);

            Angle += RotVel;
            Pos.X -= Math.Sin(Angle) * Vel;
            Pos.Y += Math.Cos(Angle) * Vel;

            BBox.X = (int)Pos.X;
            BBox.Y = (int)Pos.Y;
        }
    
        public virtual void Move(Point dest) { }

        public virtual void SetPos(Point center, float angle)
        {
            Pos.X = center.X;
            Pos.Y = center.Y;
            BBox.X = (int)Pos.X +BBox.Width/2;
            BBox.Y = (int)Pos.Y +BBox.Height/2;
            Angle = angle;
        }
    
        public Anchor AbsPos()
        {
            Anchor offset = ParentBody == null ? new(0, 0) : ParentBody.AbsPos();
            return Pos + offset;
        }
    }

    public class Entity : PhysicsBody, IDrawable
    {
        protected Entity Parent;
        protected GraphicsDeviceManager Graphics;
        protected SpriteBatch DrawCtx;
        protected Rectangle Window;
        protected DrawableInfo DrawInfo;
        private static readonly Vector2 ZeroOrigin = new Vector2(0,0);
        protected bool NoRender;

        DrawableInfo IDrawable.DrawInfo { get; set; }

        public Entity(Entity parent, Point offset, Point size, float rot, 
            Texture2D sprite, Color clr,
            float mvAcc, float mvFrc, float rocAcc, float rotFrc,
            bool norender=false) 
            : base(parent, offset, size, rot, mvAcc, mvFrc, rocAcc, rotFrc)
        {
            Parent = parent;
            Graphics = Providers.Graphics;
            DrawCtx = Providers.DrawCtx;
            Window = Graphics.GraphicsDevice.PresentationParameters.Bounds;
            if (!norender) { DrawInfo = new(sprite, clr, size.X / sprite.Width); }
            NoRender = norender;
        }

        public virtual void Draw()
        {
            if (NoRender) 
                { return; }
            //var pos = BBox.Center.ToVector2();
            var pos = AbsPos().TopLeft(BBox.Width, BBox.Height).ToVector2();
            var src = new Rectangle(0, 0, DrawInfo.Sprite.Width, DrawInfo.Sprite.Height);
            var org = new Vector2(BBox.Width/2, BBox.Height/2);
            var scale = (float)Math.Sqrt((double)(BBox.Width * BBox.Height) / (double)(DrawInfo.Sprite.Width * DrawInfo.Sprite.Height));
            Debug.WriteLine($"{((ComponentEntity)this).ID}: POS={pos} | SRC={src} | ORG={org} SCALE={scale}");

            var whiteRectangle = new Texture2D(Graphics.GraphicsDevice, 1, 1);
            whiteRectangle.SetData<Color>(new Color[] { Color.White });

            if (ParentBody == null)
            {
                DrawCtx.Draw(
                whiteRectangle,
                pos, src,
                Color.Red * .5f, Angle,
                ZeroOrigin, scale,
                SpriteEffects.None, 1f);
            }

            DrawCtx.Draw(
                DrawInfo.Sprite,
                pos, src,
                DrawInfo.Color*1f, Angle,
                ZeroOrigin, scale,
                SpriteEffects.None, 1f);
        }

        public virtual void Update() { UpdateMotion(); }
    }

    public interface IControllable
    {
        public void ActionIO(KeyboardState kbst, MouseState mst);
    }

    public class Controllable : Entity, IControllable
    {
        public Controllable(
            Entity parent,
            Point offset, Point size, float rot, Texture2D sprite, Color clr, 
            float mvAcc, float mvFrc, float rocAcc, float rotFrc,
            bool norender=false) 
            : base(parent, offset, size, rot, sprite, clr, mvAcc, mvFrc, rocAcc, rotFrc, 
                  norender)
        {
        }

        public virtual void ActionIO(KeyboardState kbst, MouseState mst) { }
    }

    public class Player : Controllable
    {
        public Player() : base(
            null,
            UIO.ScreenCenter, new Point(64, 64), 0, Providers.Sprites.Get("HatDev"), 
            Color.Red, 0.5f, 1.8f, 0.05f, 1.8f)
        {
            
        }

        public override void Update()
        {
            Debug.WriteLine($"Pos {BBox.Center} Angle: {Angle:0.##} Rot: {RotVel:0.##} : Vel {Vel:0.##}");
            base.Update();
        }

        public override void ActionIO(KeyboardState kbst, MouseState mst)
        {
            var acc = 0.0f;
            var rot = 0.0f;

            if (kbst.IsKeyDown(Keys.W)) { acc -= 1f; }
            if (kbst.IsKeyDown(Keys.S)) { acc += 1f; }
            if (kbst.IsKeyDown(Keys.A)) { rot -= 1f; }
            if (kbst.IsKeyDown(Keys.D)) { rot += 1f; }

            Move(acc, rot);
        }

        public override void Draw()
        {
            base.Draw();
        }
    }

    public interface IMachineControllable
    {
        public void Update();
    }

    public class ComponentEntity : Entity
    {
        public List<ComponentEntity> Parts;
        public string ID;
        protected int Health;
        private readonly string[] AllowRender = {"DEVMOBO", "DEVPOWER", "DEVCPU", "DEVMEM", "DEVCHASSIS", "DEVGUN" };
        
        //protected Point Dim;

        public ComponentEntity(ComponentEntity parent, Component cmp, Point offset) : base(
            parent,
            offset, cmp.Size, 0, cmp.Sprite,
            Color.White, 0.5f, 1.8f, 0.05f, 1.8f)
        {
            Parts = new();
            Health = cmp.Health;
            ID = cmp.ID;
            //if (ID != "DEVMOBO") { NoRender = true; }

            NoRender = !AllowRender.Any(x => ID.Equals(x, StringComparison.OrdinalIgnoreCase));

            Debug.WriteLine($"CMP: {ID}");
            var subcomps = cmp.SubComponents;
            foreach (var slot in subcomps) {
                if (slot.Part == null) { continue; }
                var cePart = new ComponentEntity(this, slot.Part, slot.Offset);
                Debug.WriteLine($"ADD: {parent?.ID}.{ID}.{cePart.ID}");
                Parts.Add(cePart);
            }
        }

        public override void Draw()
        {
            base.Draw();
            foreach(var  part in Parts) { part.Draw(); }
        }

        public override void SetPos(Point center, float angle)
        {
            base.SetPos(center, angle);
            foreach(var part in Parts) { part.SetPos(center, angle); }
        }
    }

    public class BotEntity : Controllable
    {
        public ComponentEntity Base;

        public BotEntity(Bot bot, Point loc) : base(null,
            new Point(0, 0), new Point(64, 64), 0, null,
            Color.White, 0.5f, 1.8f, 0.05f, 1.8f, norender:true)
        {
            Base = new(null, bot.Base, loc);
        }

        public override void Update()
        {
            Base.SetPos(BBox.Center, Angle);
            base.Update();
        }

        public override void Draw()
        {
            Base.Draw();
        }

        public override void ActionIO(KeyboardState kbst, MouseState mst)
        {
            var acc = 0.0f;
            var rot = 0.0f;

            if (kbst.IsKeyDown(Keys.W)) { acc -= 1f; }
            if (kbst.IsKeyDown(Keys.S)) { acc += 1f; }
            if (kbst.IsKeyDown(Keys.A)) { rot -= 1f; }
            if (kbst.IsKeyDown(Keys.D)) { rot += 1f; }

            Move(acc, rot);
        }

    }
}
