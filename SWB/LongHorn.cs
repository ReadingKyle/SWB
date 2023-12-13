using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWB
{
    public class LongHorn
    {
        public Vector2 Position;
        public bool IsSquished;
        public bool IsDead;
        public Vector2 Velocity;
        public bool SlideLeft;
        public bool SlideRight;

        public bool IsShelled;

        public double SlideTimer;

        private Texture2D _texture;

        public Rectangle Hitbox;

        private double _animationTimer;
        private int _animationFrame;

        private double _squishedTimer;

        private bool _isPeaking;

        public LongHorn(Vector2 position, Texture2D texture)
        {
            Position = position;
            _texture = texture;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 48);
            Velocity = new Vector2(-1, 0);
        }

        public void Update(GameTime gameTime)
        {
            if (SlideTimer > 0)
            {
                SlideTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (IsSquished)
            {
                if (!SlideLeft && !SlideRight)
                {
                    _squishedTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_squishedTimer > 8)
                    {
                        _isPeaking = false;
                        IsSquished = false;
                    }
                    else if (_squishedTimer > 6)
                    {
                        _isPeaking = true;
                    }
                }
                else
                {
                    _squishedTimer = 0;
                }
                if (SlideLeft)
                {
                    Velocity.X = -5;
                }
                else if (SlideRight)
                {
                    Velocity.X = 5;
                }
                else
                {
                    Velocity.X = 0;
                }
            }
            else
            {
                SlideLeft = false;
                SlideRight = false;
            }
            Position += Velocity;
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects direction = SpriteEffects.None;
            if (!IsDead)
            {
                if (IsShelled)
                {
                    direction = SpriteEffects.FlipVertically;
                    _animationFrame = 2;
                }
                else if (IsSquished)
                {
                    if (_isPeaking)
                    {
                        _animationFrame = 3;
                    }
                    else
                    {
                        _animationFrame = 2;
                    }
                }
                else
                {
                    if (Velocity.X > 0)
                    {
                        direction = SpriteEffects.FlipHorizontally;
                    }
                    _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_animationTimer > 0.5)
                    {
                        _animationFrame++;
                        if (_animationFrame > 1)
                        {
                            _animationFrame = 0;
                        }
                        _animationTimer = 0;
                    }
                }
                spriteBatch.Draw(
                _texture,
                Position,
                new Rectangle(_animationFrame * 16, 17, 16, 23),
                Color.White,
                0,
                new Vector2(0, 0),
                2f,
                direction,
                0
            );
            }
        }
    }
}
