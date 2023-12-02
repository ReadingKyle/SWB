using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWB
{
    public class Willie
    {
        public Vector2 Position;
        public Vector2 PrevPosition;
        public Vector2 Velocity;
        public bool IsBig = false;
        public bool IsDead = false;
        public int PlayerIndex;
        public Rectangle Hitbox;
        public bool IsFalling;
        public bool MovingRight;
        public bool MovingLeft;
        public bool MaxJump;
        public bool IsJumping;

        public float Gravity;

        private Texture2D _smallTexture;
        private Texture2D _bigTexture;
        private float _maxSpeed;
        private float _maxJump;

        private double _animationTimer;
        private int _animationFrame;

        public bool IsWinner;

        public Willie (Texture2D smallTexture, Texture2D bigTexture, int playerIndex)
        {
            Position = new Vector2(100, 352);
            PrevPosition = Position;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            _smallTexture = smallTexture;
            _bigTexture = bigTexture;
            PlayerIndex = playerIndex;
            _maxSpeed = 4f;
            _maxJump = -5f;
            Gravity = 10f;
        }

        public void Update(GameTime gameTime)
        {
            if (Math.Abs(Velocity.X) >= _maxSpeed)
            {
                Velocity.X = Math.Sign(Velocity.X) * _maxSpeed;
            }
            if (Velocity.Y <= _maxJump)
            {
                MaxJump = true;
                IsJumping = false;
            }
            PrevPosition = Position;
            Position += Velocity;
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;
        }

        public void Draw (GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects direction = SpriteEffects.None;
            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (!IsBig)
            {
                if (!IsDead)
                {
                    if (MovingLeft)
                    {
                        direction = SpriteEffects.FlipHorizontally;
                    }
                    if (IsJumping)
                    {
                        _animationFrame = 5;
                    }
                    else if (Math.Abs(Velocity.X) > 0)
                    {
                        if (_animationTimer > 0.05)
                        {
                            _animationFrame++;
                            if (_animationFrame > 3)
                            {
                                _animationFrame = 1;
                            }
                            _animationTimer = 0;
                        }
                    }
                    else
                    {
                        _animationFrame = 0;
                    }
                }
                else
                {
                    _animationFrame = 6;
                }
                spriteBatch.Draw(
                    _smallTexture,
                    Position,
                    new Rectangle(_animationFrame * 16, PlayerIndex == 0 ? 0 : 16, 16, 16),
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
