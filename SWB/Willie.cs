using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using SWB.StateManagement;
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
        public bool IsBaller = false;
        public bool IsSmall = true;
        public bool IsDead = false;
        public int PlayerIndex;
        public Rectangle Hitbox;
        public bool IsFalling;
        public bool MovingRight;
        public bool MovingLeft;
        public bool MaxJump;
        public bool IsJumping;
        public bool HasJump;
        public bool IsBraking;
        public bool IsWalking;
        public int HorizontalSpeed;
        public bool FieldGoal;
        public bool Finished;
        public bool ShowScore;

        public float Gravity;

        private Texture2D _texture;

        private Texture2D _smallTexture;
        private Texture2D _bigTexture;
        private float _maxSpeed;
        private float _maxJump;

        private double _animationTimer;
        private int _animationFrame;

        public bool IsWinner;

        public Color color;
        private int _scale;

        private int _offsetY;

        private double _hitCooldown;

        public bool GettingBig;
        public bool GettingSmall;

        public Willie (Texture2D smallTexture, Texture2D bigTexture, int playerIndex)
        {
            Position = new Vector2(64, 353);
            PrevPosition = Position;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            _smallTexture = smallTexture;
            _bigTexture = bigTexture;
            PlayerIndex = playerIndex;
            _maxSpeed = 4f;
            _maxJump = -6.6f;
            Gravity = 10f;
            color = Color.White;
            _texture = _smallTexture;
            _scale = 16;
            _offsetY = 0;
        }

        public void GetBig()
        {
            IsBig = true;
            IsBaller = false;
            _texture = _bigTexture;
            _scale = 32;
            _offsetY = 0;
            Position.Y -= 32;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 64);
            if (IsSmall)
            {
                IsSmall = false;
                GettingBig = true;
                _animationTimer = 0;
            }
        }

        public void GetBalls()
        {
            IsBaller = true;
            IsBig = false;
            IsSmall = false;
            _texture = _bigTexture;
            _scale = 32;
            _offsetY = 32;
            Position.Y -= 32;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 64);
            GettingBig = true;
            _animationTimer = 0;
        }

        public void GetSmall()
        {
            IsBig = false;
            IsSmall = true;
            _texture = _smallTexture;
            _scale = 16;
            _offsetY = 0;
            Position.Y += 32;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            GettingSmall = true;
        }

        public void GetHit()
        {
            if (IsBaller)
            {
                GetBig();
                _hitCooldown = 1;
            }
            else if (IsBig)
            {
                GetSmall();
                _hitCooldown = 1;
            }
            else if (_hitCooldown <= 0)
            {
                IsDead = true;
            }
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
            }
            if (IsWalking)
            {
                _maxSpeed = 4;
            }
            else
            {
                _maxSpeed = 5;
            }
            PrevPosition = Position;
            Position += Velocity;
            Hitbox.X = (int)Position.X;
            Hitbox.Y = (int)Position.Y;

            if (_hitCooldown > 0)
            {
                _hitCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_hitCooldown < 0.2)
                {
                    color = Color.White;
                }
                else if (_hitCooldown < 0.4)
                {
                    color = new Color(Color.White, 0);
                }
                else if (_hitCooldown < 0.8)
                {
                    color = Color.White;
                }
                else if (_hitCooldown < 1)
                {
                    color = new Color(Color.White, 0);
                }
            }
            else
            {
                color = Color.White;
            }
        }

        public void Draw (GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects direction = SpriteEffects.None;
            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (GettingBig)
            {
                if (_animationTimer > 0.3)
                {
                    GettingBig = false;
                }
                else if (_animationTimer >= 0.2)
                {
                    _animationFrame = 11;
                }
                else if (_animationTimer >= 0.1)
                {
                    _animationFrame = 10;
                }
                else if (_animationTimer > 0)
                {
                    _animationFrame = 11;
                }
            }
            else if (!IsDead)
            {
                if (MovingLeft)
                {
                    direction = SpriteEffects.FlipHorizontally;
                }
                if (IsJumping)
                {
                    _animationFrame = 5;
                }
                else if (IsBraking)
                {
                    _animationFrame = 4;
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
                _texture,
                Position,
                new Rectangle(_animationFrame * 16,  _offsetY, 16, _scale),
                color,
                0,
                new Vector2(0, 0),
                2f,
                direction,
                0
            );
        }
    }
}
