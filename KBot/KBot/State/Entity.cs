using KBot.Components;
using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
        protected float Angle;
        protected float Vel;
        protected float RotVel;
        protected readonly float Acc;
        protected float Frc;
        protected readonly float RotAcc;
        protected float RotFrc;

        public PhysicsBody(Point pos, Point dim, float rot, float mvAcc, float mvFrc, float rotAcc, float rotFrc) 
        {
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
            ApplyFriction();
            Move();
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
            var x = Math.Sin(Angle) * Vel;
            var y = Math.Cos(Angle) * Vel;
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
            BBox = new Rectangle(BBox.Width / 2, BBox.Height / 2, sprite.Width, sprite.Height);
        }

        public virtual void Draw()
        {
            var pos = BBox.Center.ToVector2();
            var src = new Rectangle(0, 0, BBox.Width, BBox.Height);
            var org = new Vector2(BBox.Width/2, BBox.Height/2);

            DrawCtx.Draw(DrawInfo.Sprite, pos, src, Color.Yellow, Angle, org, 0.15f, SpriteEffects.None, 1f);
        }

        
    }

    public interface Controllable
    {
        public void ActionIO(KeyboardState kbst, MouseState mst);
    }

 
}
