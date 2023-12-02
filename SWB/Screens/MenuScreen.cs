using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SWB.StateManagement;

namespace SWB.Screens
{
    // Base class for screens that contain a menu of options. The user can
    // move up and down to select an entry, or cancel to back out of the screen.
    public class MenuScreen : GameScreen
    {
        private ContentManager _content;
        private Tilemap _tilemapBackground;
        private Tilemap _tilemapForeground;
        private Willie _player1;

        private JayHawk[] _jayHawks;

        private InputAction _left;
        private InputAction _right;
        private InputAction _jump;
        private InputAction _jumpCharge;

        private float _offsetX;

        private double _deathTimer = 3;

        public MenuScreen()
        {
            
        }

        public override void Activate()
        {
            base.Activate();

            _jump = new InputAction(
                new[] { Keys.Space }, true);
            _jumpCharge = new InputAction(
                new[] { Keys.Space }, false);
            _left = new InputAction(
                new[] { Keys.Left }, false);
            _right = new InputAction(
                new[] { Keys.Right }, false);


            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _tilemapBackground = _content.Load<Tilemap>("level1_bg");
            _tilemapForeground = _content.Load<Tilemap>("level1_fg");

            _player1 = new Willie(_content.Load<Texture2D>("small_willie"), _content.Load<Texture2D>("big_willie"), 0);
            _jayHawks = new JayHawk[] {
                new JayHawk(new Vector2(736, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(1312, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(1696, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(1728, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(3104, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(3136, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(4000, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(4032, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(4064, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(4096, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(5568, 352), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(5600, 352), _content.Load<Texture2D>("enemies")),
            };
        }

        public void HandlePlayerInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            List<CollisionDirection> collision = _tilemapForeground.Update(gameTime, _player1.Hitbox);
            if (_player1.Position.X < 0)
            {
                _player1.Position.X = _player1.PrevPosition.X;
                _player1.Velocity.X = 0;
            }
            if (_player1.Position.X > 6528)
            {
                ScreenManager.Game.Exit();
            }
            if (_player1.Position.X > 6304)
            {
                _player1.IsWinner = true;
            }
            if (_player1.Position.Y > 448)
            {
                _player1.IsDead = true;
            }
            if (collision.Contains(CollisionDirection.CollisionLeft) || collision.Contains(CollisionDirection.CollisionRight))
            {
                _player1.Position.X = _player1.PrevPosition.X;
                _player1.Velocity.X = 0;
            }
            else
            {
                if (_right.Occurred(input, ControllingPlayer, out playerIndex))
                {
                    _player1.Velocity.X += 7 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _player1.MovingLeft = false;
                    _player1.MovingRight = true;
                }
                else if (_left.Occurred(input, ControllingPlayer, out playerIndex))
                {
                    _player1.Velocity.X -= 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _player1.MovingLeft = true;
                    _player1.MovingRight = false;
                }
                else
                {
                    if ((int)_player1.Velocity.X < 0)
                    {
                        _player1.Velocity.X += 3 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if ((int)_player1.Velocity.X > 0)
                    {
                        _player1.Velocity.X -= 3 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        _player1.Velocity.X = 0;
                    }
                }
            }
            if (collision.Contains(CollisionDirection.CollisionTop))
            {
                _player1.MaxJump = false;
                _player1.Velocity.Y = 0;
                _player1.IsFalling = false;
                _player1.IsJumping = false;
                _player1.Position.Y = _player1.PrevPosition.Y;
            }
            else
            {
                _player1.Velocity.Y += _player1.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (collision.Contains(CollisionDirection.CollisionTop) && _jump.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _player1.IsJumping = true;
                _player1.Velocity.Y = -4;
            }
            if (_jumpCharge.Occurred(input, ControllingPlayer, out playerIndex) && _player1.Velocity.Y < -1.5)
            {
                _player1.Gravity = 2f;
            }
            else
            {
                _player1.Gravity = 10f;
            }
            _player1.Update(gameTime);
        }

        public void HandleEnemyLogic(JayHawk jayhawk, GameTime gameTime)
        {
            if (!jayhawk.IsSquished)
            {
                List<CollisionDirection> worldCollision = _tilemapForeground.Update(gameTime, jayhawk.Hitbox);
                List<CollisionDirection> playerCollision = CollisionHelper.Collides(_player1.Hitbox, jayhawk.Hitbox);
                if (worldCollision.Contains(CollisionDirection.CollisionLeft) || worldCollision.Contains(CollisionDirection.CollisionRight))
                {
                    jayhawk.Velocity *= -1;
                }
                if (playerCollision.Count > 0)
                {
                    if (playerCollision.Contains(CollisionDirection.CollisionTop))
                    {
                        jayhawk.IsSquished = true;
                        _player1.Velocity.Y *= -1f;
                    }
                    else
                    {
                        _player1.IsDead = true;
                    }
                }
            }
            jayhawk.Update(gameTime);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (!_player1.IsDead)
            {
                HandlePlayerInput(gameTime, input);
                foreach(var jayHawk in _jayHawks)
                {
                    if (!jayHawk.IsDead)
                    {
                        HandleEnemyLogic(jayHawk, gameTime);
                    }
                }
            }
            else
            {
                _deathTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_deathTimer < 0)
                {
                    ScreenManager.Game.Exit();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float playerX;
            // Calculate our offset vector
            playerX = MathHelper.Clamp(_player1.Position.X, 300, 13600);
            _offsetX = 300 - playerX;

            Matrix transform;

            var spriteBatch = ScreenManager.SpriteBatch;
            transform = Matrix.CreateTranslation(_offsetX, 0, 0);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, transformMatrix: transform);

            if (_player1.IsWinner)
            {
                spriteBatch.DrawString(ScreenManager.BigFont, "You Win!", new Vector2(6528, 160), Color.White);
            }

            _tilemapBackground.Draw(gameTime, spriteBatch);
            _tilemapForeground.Draw(gameTime, spriteBatch);
            _player1.Draw(gameTime, spriteBatch);
            foreach(var jayHawk in _jayHawks)
            {
                jayHawk.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
