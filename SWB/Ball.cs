using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWB
{
    public class Ball
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsDead;
        public bool HasHit;
        private Texture2D _texture;
        public Rectangle Hitbox;

        private double _animationTimer;
        private int _animationFrame;

        public Ball(Vector2 position, Vector2 velocity, Texture2D texture)
        {
            Position = position;
            Velocity = velocity;
            _texture = texture;
            IsDead = true;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            _animationFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (HasHit)
            {
                if (_animationFrame < 2)
                {
                    _animationFrame = 2;
                }
                if (_animationTimer > 0.05)
                {
                    _animationFrame += 1;
                    if (_animationFrame >= 5)
                    {
                        HasHit = false;
                        IsDead = true;
                        _animationFrame = 0;
                    }
                    _animationTimer = 0;
                }
            }
            else
            {
                if (_animationTimer > 0.05)
                {
                    _animationFrame += 1;
                    if (_animationFrame > 2)
                    {
                        _animationFrame = 0;
                    }
                    _animationTimer = 0;
                }
            }

            spriteBatch.Draw(
            _texture,
            Position,
            new Rectangle(_animationFrame * 16, 0, 16, 16),
                Color.White,
                0,
                new Vector2(0, 0),
                2f,
                SpriteEffects.None,
                0
            );
        }
    }
}
