using KBot.Components;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

    public class PhysicsBody
    {
        protected Rectangle BBox;
        protected double X;
        protected double Y;
        protected float Angle;
        protected float Vel;
        protected float RotVel;
        protected readonly float Acc;
        protected float Frc;
        protected readonly float RotAcc;
        protected float RotFrc;

        public PhysicsBody(
            Point pos, Point dim, 
            float rot, float mvAcc, float mvFrc, float rotAcc, float rotFrc) 
        {
            X = pos.X;
            Y = pos.Y;
            BBox = new(pos, dim);
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
            X -= Math.Sin(Angle) * Vel;
            Y += Math.Cos(Angle) * Vel;

            BBox.X = (int)X;
            BBox.Y = (int)Y;
        }
    
        public virtual void Move(Point dest) { }

        public virtual void SetLoc(Point center)
        {
            X = center.X;
            Y = center.Y;
            BBox.X = (int)X;
            BBox.Y = (int)Y;
        }
    }

    public class Entity : PhysicsBody, IDrawable
    {
        protected GraphicsDeviceManager Graphics;
        protected SpriteBatch DrawCtx;
        protected Rectangle Window;

        protected DrawableInfo DrawInfo;

        DrawableInfo IDrawable.DrawInfo { get; set; }

        public Entity(Point loc, Point size, float rot, Texture2D sprite, Color clr,
            float mvAcc, float mvFrc, float rocAcc, float rotFrc) 
            : base(loc, size, rot, mvAcc, mvFrc, rocAcc, rotFrc)
        {
            Graphics = Providers.Graphics;
            DrawCtx = Providers.DrawCtx;
            Window = Graphics.GraphicsDevice.PresentationParameters.Bounds;
            DrawInfo = new(sprite, clr, size.X/sprite.Width);
        }

        public virtual void Draw()
        {
            var pos = BBox.Center.ToVector2();
            var src = new Rectangle(0, 0, DrawInfo.Sprite.Width, DrawInfo.Sprite.Height);
            var org = new Vector2(BBox.Width/2, BBox.Height/2);
            var scale = new Vector2(BBox.Width / DrawInfo.Sprite.Width, BBox.Height / DrawInfo.Sprite.Height);

            DrawCtx.Draw(
                DrawInfo.Sprite,
                pos, src, 
                DrawInfo.Color, Angle, 
                org, scale, 
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
            Point loc, Point size, float rot, Texture2D sprite, Color clr, 
            float mvAcc, float mvFrc, float rocAcc, float rotFrc) 
            : base(loc, size, rot, sprite, clr, mvAcc, mvFrc, rocAcc, rotFrc)
        {
        }

        public virtual void ActionIO(KeyboardState kbst, MouseState mst) { }
    }

    public class Player : Controllable
    {
        public Player() : base(
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
        List<Component> Part;

        public ComponentEntity(
            Point loc, Point size, float rot, Texture2D sprite, Color clr, 
            float mvAcc, float mvFrc, float rocAcc, float rotFrc) : base(loc, size, rot, sprite, clr, mvAcc, mvFrc, rocAcc, rotFrc)
        {
        }

        public static ComponentEntity EntityFromBot(Bot bot)
        {
            return null;
        }
    }

    public class BotEntity : Entity, IMachineControllable
    {
        ComponentEntity Base;

        public BotEntity(Point loc, Point size, 
            float rot, Texture2D sprite, Color clr, 
            float mvAcc, float mvFrc, float rocAcc, float rotFrc) : base(loc, size, rot, sprite, clr, mvAcc, mvFrc, rocAcc, rotFrc)
        {
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
