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
using RetroReadingRacing.Screens;
using SharpDX.Direct2D1;
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
        private LongHorn[] _longHorns;
        private Ref[] _refs;
        private Powerup[] _powerups;

        private InputAction _left;
        private InputAction _right;
        private InputAction _jump;
        private InputAction _jumpCharge;
        private InputAction _sprint;
        private InputAction _throw;

        private float _offsetX;

        private double _deathTimer = 3;

        FireworkParticleSystem _fireworks;

        private Ball[] _balls;

        private TimeSpan _timer;

        private SoundEffectInstance _jumpSound;
        private SoundEffectInstance _breakSound;
        private SoundEffectInstance _stompSound;
        private SoundEffectInstance _coinSound;
        private SoundEffectInstance _powerUpSound;

        private Texture2D _bg;

        private double _leaveTimer = 5;

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
                new[] { Keys.Left, Keys.A }, false);
            _right = new InputAction(
                new[] { Keys.Right, Keys.D }, false);
            _sprint = new InputAction(
                new[] { Keys.LeftShift, Keys.RightShift }, false);
            _throw = new InputAction(
                new[] { Keys.LeftShift, Keys.RightShift }, true);


            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _tilemapBackground = _content.Load<Tilemap>("level1_bg");
            _tilemapForeground = _content.Load<Tilemap>("level1_fg");

            _player1 = new Willie(_content.Load<Texture2D>("small_willie"), _content.Load<Texture2D>("big_willie"), 0);
            _jayHawks = new JayHawk[] {
                new JayHawk(new Vector2(736, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(1312, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(1696, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(1728, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(3104, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(3136, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(4000, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(4032, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(4064, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(4096, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(5568, 353), _content.Load<Texture2D>("enemies")),
                new JayHawk(new Vector2(5600, 353), _content.Load<Texture2D>("enemies")),
            };
            _longHorns = new LongHorn[]
            {
                new LongHorn(new Vector2(3424, 339), _content.Load<Texture2D>("enemies")),
            };
            _refs = new Ref[]
            {
                new Ref(new Vector2(6272, 353), true, _content.Load<Texture2D>("enemies")),
                new Ref(new Vector2(6368, 353), false, _content.Load<Texture2D>("enemies")),
            };
            _powerups = new Powerup[]
            {
                new Powerup(new Vector2(512, 256), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(736, 256), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(704, 128), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(3008, 128), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(3008, 256), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(3392, 256), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(3488, 256), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(3584, 256), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(4128, 128), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(4160, 128), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(5440, 256), PowerupType.Points, _content.Load<Texture2D>("pickups"), false),
                new Powerup(new Vector2(672, 256), PowerupType.Big, _content.Load<Texture2D>("pickups"), true),
                new Powerup(new Vector2(3488, 128), PowerupType.Big, _content.Load<Texture2D>("pickups"), true),
                new Powerup(new Vector2(4160, 128), PowerupType.Big, _content.Load<Texture2D>("pickups"), true),
                new Powerup(new Vector2(2496, 256), PowerupType.Balls, _content.Load<Texture2D>("pickups"), true),
            };

            _fireworks = new FireworkParticleSystem(ScreenManager.Game, 100);
            ScreenManager.Game.Components.Add(_fireworks);

            _balls = new Ball[]
            {
                new Ball(new Vector2(0, 0), new Vector2(0, 0), _content.Load<Texture2D>("bball")),
                new Ball(new Vector2(0, 0), new Vector2(0, 0), _content.Load<Texture2D>("bball")),
                new Ball(new Vector2(0, 0), new Vector2(0, 0), _content.Load<Texture2D>("bball")),
                new Ball(new Vector2(0, 0), new Vector2(0, 0), _content.Load<Texture2D>("bball")),
                new Ball(new Vector2(0, 0), new Vector2(0, 0), _content.Load<Texture2D>("bball"))
            };

            SoundEffect jumpSound = _content.Load<SoundEffect>("jump (9)");
            SoundEffect breakSound = _content.Load<SoundEffect>("explosion (3)");
            SoundEffect stompSound = _content.Load<SoundEffect>("hitHurt");
            SoundEffect coinSound = _content.Load<SoundEffect>("pickupCoin (1)");
            SoundEffect powerUpSound = _content.Load<SoundEffect>("powerUp (2)");

            _jumpSound = jumpSound.CreateInstance();
            _breakSound = breakSound.CreateInstance();
            _stompSound = stompSound.CreateInstance();
            _coinSound = coinSound.CreateInstance();
            _powerUpSound = powerUpSound.CreateInstance();
            _jumpSound.Volume = 0.5f;
            _breakSound.Volume = 0.5f;
            _stompSound.Volume = 0.5f;
            _coinSound.Volume = 0.5f;
            _powerUpSound.Volume = 0.5f;

            _bg = _content.Load<Texture2D>("bg_mainmenu");
        }

        public void HandlePlayerInput(GameTime gameTime, InputState input)
        {
            if (!_player1.IsDead && !_player1.GettingBig)
            {
                PlayerIndex playerIndex;

                Dictionary<Tile, CollisionDirection> collision = _tilemapForeground.Update(gameTime, _player1.Hitbox, _player1.IsBig || _player1.IsBaller, _breakSound);

                if (_throw.Occurred(input, ControllingPlayer, out playerIndex) && _player1.IsBaller)
                {
                    foreach(Ball ball in _balls)
                    {
                        if (ball.IsDead)
                        {
                            ball.IsDead = false;
                            ball.HasHit = false;
                            ball.Position = _player1.Position;
                            ball.Velocity.X = _player1.MovingLeft ? -400 : 400;
                            break;
                        }
                    }
                }
                if (_sprint.Occurred(input, ControllingPlayer, out playerIndex))
                {
                    _player1.HorizontalSpeed = 10;
                    _player1.IsWalking = false;
                }
                else
                {
                    _player1.HorizontalSpeed = 4;
                    _player1.IsWalking = true;
                }

                if (_player1.Position.X < 0)
                {
                    _player1.Position.X = _player1.PrevPosition.X;
                    _player1.Velocity.X = 0;
                }
                if (_player1.Position.X > 6272)
                {
                    if (_player1.Position.Y < 192)
                    {
                        if (!_player1.FieldGoal)
                        {
                            ScreenManager.Score += 3;
                            _coinSound.Play();
                        }
                        _player1.FieldGoal = true;
                    }
                }
                if (_player1.Position.X > 6368)
                {
                    _player1.Finished = true;
                    foreach (var zebra in _refs)
                    {
                        zebra.FieldGoal = _player1.FieldGoal;
                        zebra.Finish = true;
                    }
                }
                if (_player1.Position.X > 6528)
                {
                    _player1.ShowScore = true;
                }
                if (_player1.Position.X > 6304)
                {
                    _player1.IsWinner = true;
                }
                if (_player1.Position.Y > 448)
                {
                    _player1.IsDead = true;
                    ScreenManager.Lives--;
                }
                if (collision.ContainsValue(CollisionDirection.CollisionBottom) && (collision.ContainsValue(CollisionDirection.CollisionLeft) || collision.ContainsValue(CollisionDirection.CollisionRight)))
                {
                    Tile bottomTile = null;
                    foreach (Tile tile in collision.Keys)
                    {
                        if (collision[tile] == CollisionDirection.CollisionBottom)
                        {
                            if (bottomTile == null || (tile.Position.Y < bottomTile.Position.Y))
                            {
                                bottomTile = tile;
                            }
                        }
                    }
                    foreach (Tile tile in collision.Keys)
                    {
                        if ((collision[tile] == CollisionDirection.CollisionLeft || collision[tile] == CollisionDirection.CollisionRight) && tile.Bounds.Y < bottomTile.Bounds.Y)
                        {
                            _player1.Position.X = _player1.PrevPosition.X;
                            _player1.Velocity.X = 0;
                        }
                    }
                }
                else if (collision.ContainsValue(CollisionDirection.CollisionLeft) || collision.ContainsValue(CollisionDirection.CollisionRight))
                {
                    _player1.Position.X = _player1.PrevPosition.X;
                    _player1.Velocity.X = 0;
                }
                else
                {
                    if (_right.Occurred(input, ControllingPlayer, out playerIndex) && _player1.Velocity.X >= 0)
                    {
                        _player1.Velocity.X += _player1.HorizontalSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        _player1.MovingLeft = false;
                        _player1.MovingRight = true;
                        _player1.IsBraking = false;
                    }
                    else if (_left.Occurred(input, ControllingPlayer, out playerIndex) && _player1.Velocity.X <= 0)
                    {
                        _player1.Velocity.X -= _player1.HorizontalSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        _player1.MovingLeft = true;
                        _player1.MovingRight = false;
                        _player1.IsBraking = false;
                    }
                    else
                    {
                        if ((int)_player1.Velocity.X < 0 && _right.Occurred(input, ControllingPlayer, out playerIndex))
                        {
                            _player1.Velocity.X += 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            _player1.IsBraking = true;
                            _player1.MovingLeft = false;
                            _player1.MovingRight = true;
                        }
                        else if ((int)_player1.Velocity.X > 0 && _left.Occurred(input, ControllingPlayer, out playerIndex))
                        {
                            _player1.Velocity.X -= 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            _player1.IsBraking = true;
                            _player1.MovingLeft = true;
                            _player1.MovingRight = false;
                        }
                        else if ((int)_player1.Velocity.X < 0)
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
                            _player1.IsBraking = false;
                        }
                    }
                }
                if (collision.ContainsValue(CollisionDirection.CollisionTop))
                {
                    _player1.Velocity.Y = 0;
                }
                if (collision.ContainsValue(CollisionDirection.CollisionBottom))
                {
                    foreach (Tile tile in collision.Keys)
                    {
                        if (collision[tile] == CollisionDirection.CollisionBottom && _tilemapForeground.Tiles[tile.Index - _tilemapForeground.MapWidth].ID == 0)
                        {
                            _player1.MaxJump = false;
                            _player1.Velocity.Y = 0;
                            _player1.IsFalling = false;
                            _player1.IsJumping = false;
                            float tileSize = 32.0f;
                            float snappedY = (float)Math.Round(_player1.Position.Y / tileSize) * tileSize;
                            _player1.Position.Y = snappedY + 1;
                            if (_jump.Occurred(input, ControllingPlayer, out playerIndex))
                            {
                                _jumpSound.Play();
                                _player1.IsJumping = true;
                                _player1.Velocity.Y = -6.5f;
                            }
                        }
                    }
                }
                else
                {
                    _player1.Velocity.Y += _player1.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (_jumpCharge.Occurred(input, ControllingPlayer, out playerIndex) && Math.Sign(_player1.Velocity.Y) < 0 && !_player1.MaxJump)
                {
                    _player1.Gravity = 10f;
                }
                else
                {
                    _player1.Gravity = 23f;
                }
            }
            if (!_player1.GettingBig)
            {
                _player1.Update(gameTime);
            }
        }

        public void HandleJayhawkLogic(JayHawk jayhawk, GameTime gameTime)
        {
            if (!jayhawk.IsSquished)
            {
                Dictionary<Tile, CollisionDirection> worldCollision = _tilemapForeground.Update(gameTime, jayhawk.Hitbox, false, _breakSound);
                CollisionDirection playerCollision = CollisionHelper.Collides(_player1.Hitbox, jayhawk.Hitbox);

                List<CollisionDirection> friendCollision = new List<CollisionDirection>();

                foreach (JayHawk friend in _jayHawks)
                {
                    if (jayhawk != friend)
                    {
                        CollisionDirection collision = CollisionHelper.Collides(jayhawk.Hitbox, friend.Hitbox);
                        if (collision != CollisionDirection.NoCollision)
                        {
                            friendCollision.Add(collision);
                        }
                    }
                }
                foreach (LongHorn friend in _longHorns)
                {
                    CollisionDirection collision = CollisionHelper.Collides(jayhawk.Hitbox, friend.Hitbox);
                    if (collision != CollisionDirection.NoCollision)
                    {
                        if (friend.IsSquished && (friend.SlideLeft || friend.SlideRight))
                        {
                            jayhawk.IsShelled = true;
                            jayhawk.Velocity.Y = -2;
                            _stompSound.Play();
                        }
                        else
                        {
                            friendCollision.Add(collision);
                        }
                    }
                }

                if (jayhawk.IsShelled)
                {
                    worldCollision.Clear();
                    playerCollision = CollisionDirection.NoCollision;
                    friendCollision.Clear();
                }

                if (worldCollision.ContainsValue(CollisionDirection.CollisionLeft) || worldCollision.ContainsValue(CollisionDirection.CollisionRight) || friendCollision.Contains(CollisionDirection.CollisionLeft) ||
                    friendCollision.Contains(CollisionDirection.CollisionRight))
                {
                    jayhawk.Velocity.X *= -1;
                }
                if (!worldCollision.ContainsValue(CollisionDirection.CollisionBottom))
                {
                    jayhawk.Velocity.Y += 20f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    jayhawk.Velocity.Y = 0;
                }
                if (playerCollision != CollisionDirection.NoCollision)
                {
                    if (playerCollision == CollisionDirection.CollisionBottom)
                    {
                        jayhawk.IsSquished = true;
                        _player1.Velocity.Y = -4;
                        _stompSound.Play();
                    }
                    else
                    {
                        _player1.Velocity.Y = -4;
                        _player1.GetHit();
                        if (_player1.IsDead)
                        {
                            ScreenManager.Lives--;
                        }
                    }
                }
                if (jayhawk.Position.Y > 448)
                {
                    jayhawk.IsDead = true;
                }
            }
            jayhawk.Update(gameTime);
        }

        public void HandleLongHornLogic(LongHorn longHorn, GameTime gameTime)
        {
            Dictionary<Tile, CollisionDirection> worldCollision = _tilemapForeground.Update(gameTime, longHorn.Hitbox, false, _breakSound);
            CollisionDirection playerCollision = CollisionHelper.Collides(_player1.Hitbox, longHorn.Hitbox);

            List<CollisionDirection> friendCollision = new List<CollisionDirection>();

            foreach (JayHawk friend in _jayHawks)
            {
                CollisionDirection collision = CollisionHelper.Collides(longHorn.Hitbox, friend.Hitbox);
                if (collision != CollisionDirection.NoCollision)
                {
                    friendCollision.Add(collision);
                }
            }
            foreach (LongHorn friend in _longHorns)
            {
                if (longHorn != friend)
                {
                    CollisionDirection collision = CollisionHelper.Collides(longHorn.Hitbox, friend.Hitbox);
                    if (collision != CollisionDirection.NoCollision)
                    {
                        if (friend.IsSquished && (friend.SlideLeft || friend.SlideRight))
                        {
                            longHorn.IsShelled = true;
                            longHorn.Velocity.Y = -2;
                            _stompSound.Play();
                        }
                        else
                        {
                            friendCollision.Add(collision);
                        }
                    }
                }
            }

            if (longHorn.IsShelled)
            {
                worldCollision.Clear();
                playerCollision = CollisionDirection.NoCollision;
                friendCollision.Clear();

                longHorn.Velocity.Y += 20 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (worldCollision.ContainsValue(CollisionDirection.CollisionBottom) && (worldCollision.ContainsValue(CollisionDirection.CollisionLeft) || worldCollision.ContainsValue(CollisionDirection.CollisionRight)))
            {
                Tile bottomTile = null;
                foreach (Tile tile in worldCollision.Keys)
                {
                    if (worldCollision[tile] == CollisionDirection.CollisionBottom)
                    {
                        if (bottomTile == null || (tile.Position.Y < bottomTile.Position.Y))
                        {
                            bottomTile = tile;
                        }
                    }
                }
                foreach (Tile tile in worldCollision.Keys)
                {
                    if ((worldCollision[tile] == CollisionDirection.CollisionLeft || worldCollision[tile] == CollisionDirection.CollisionRight) && tile.Bounds.Y < bottomTile.Bounds.Y)
                    {
                        if (longHorn.IsSquished && longHorn.SlideRight)
                        {
                            longHorn.SlideLeft = true;
                            longHorn.SlideRight = false;
                        }
                        else if (longHorn.IsSquished && longHorn.SlideLeft)
                        {
                            longHorn.SlideLeft = false;
                            longHorn.SlideRight = true;
                        }
                        else
                        {
                            longHorn.Velocity.X *= -1;
                        }
                    }
                }
            }
            else if (longHorn.IsSquished && (worldCollision.ContainsValue(CollisionDirection.CollisionLeft) || worldCollision.ContainsValue(CollisionDirection.CollisionRight)))
            {
                if (longHorn.IsSquished && longHorn.SlideRight)
                {
                    longHorn.SlideLeft = true;
                    longHorn.SlideRight = false;
                }
                else if (longHorn.IsSquished && longHorn.SlideLeft)
                {
                    longHorn.SlideLeft = false;
                    longHorn.SlideRight = true;
                }
            }
            if (friendCollision.Contains(CollisionDirection.CollisionLeft) ||
                    friendCollision.Contains(CollisionDirection.CollisionRight))
            {
                longHorn.Velocity.X *= -1;
            }
            if (worldCollision.ContainsValue(CollisionDirection.CollisionBottom))
            {
                longHorn.Velocity.Y = 0;
            }
            if (!longHorn.IsSquished)
            {
                if (!worldCollision.ContainsValue(CollisionDirection.CollisionBottom))
                {
                    longHorn.Velocity.X = -1;
                }
                if (playerCollision != CollisionDirection.NoCollision)
                {
                    if (playerCollision == CollisionDirection.CollisionBottom)
                    {
                        longHorn.IsSquished = true;
                        longHorn.SlideTimer = 1;
                        _player1.Velocity.Y = -5;
                        _stompSound.Play();
                    }
                    else
                    {
                        _player1.Velocity.Y = -4;
                        _player1.GetHit();
                        if (_player1.IsDead)
                        {
                            ScreenManager.Lives--;
                        }
                    }
                }
            }
            else
            {
                if (!worldCollision.ContainsValue(CollisionDirection.CollisionBottom))
                {
                    longHorn.Velocity.Y += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (playerCollision != CollisionDirection.NoCollision)
                {
                    if (playerCollision == CollisionDirection.CollisionLeft && longHorn.SlideLeft == false && longHorn.SlideRight == false && longHorn.SlideTimer <= 0)
                    {
                        longHorn.SlideRight = false;
                        longHorn.SlideLeft = true;
                        longHorn.SlideTimer = 1;
                    }
                    else if (playerCollision == CollisionDirection.CollisionRight && longHorn.SlideLeft == false && longHorn.SlideRight == false && longHorn.SlideTimer <= 0)
                    {
                        longHorn.SlideRight = true;
                        longHorn.SlideLeft = false;
                        longHorn.SlideTimer = 1;
                    }
                    else if (playerCollision == CollisionDirection.CollisionBottom)
                    {
                        if (longHorn.SlideRight || longHorn.SlideLeft)
                        {
                            _player1.Velocity.Y = -5;
                            longHorn.SlideRight = false;
                            longHorn.SlideLeft = false;
                        }
                        else
                        {
                            if (Math.Sign(_player1.Velocity.X) < 0)
                            {
                                longHorn.SlideRight = true;
                                longHorn.SlideLeft = false;
                            }
                            else
                            {
                                longHorn.SlideRight = false;
                                longHorn.SlideLeft = true;
                            }
                        }
                        longHorn.SlideTimer = 1;
                    }
                    else if ((playerCollision == CollisionDirection.CollisionRight || playerCollision == CollisionDirection.CollisionLeft) && (longHorn.SlideLeft == true || longHorn.SlideRight == true) && longHorn.SlideTimer <= 0)
                    {
                        _player1.GetHit();
                    }
                }
                if (longHorn.Position.Y > 448)
                {
                    longHorn.IsDead = true;
                }
            }
            longHorn.Update(gameTime);
        }

        public void HandlePowerupLogic(Powerup powerup,GameTime gameTime)
        {
            if (!powerup.IsPicked)
            {
                CollisionDirection playerCollision = CollisionHelper.Collides(_player1.Hitbox, powerup.Hitbox);

                if (playerCollision != CollisionDirection.NoCollision)
                {
                    if (playerCollision == CollisionDirection.CollisionTop && !powerup.IsReleased)
                    {
                        powerup.IsReleased = true;
                    }
                    else if (playerCollision == CollisionDirection.CollisionLeft || playerCollision == CollisionDirection.CollisionRight || playerCollision == CollisionDirection.CollisionBottom && powerup.IsActive)
                    {
                        if (powerup.Type == PowerupType.Big)
                        {
                            _player1.GetBig();
                            powerup.IsPicked = true;
                            _powerUpSound.Play();
                        }
                        else if (powerup.Type == PowerupType.Balls)
                        {
                            _player1.GetBalls();
                            powerup.IsPicked = true;
                            _powerUpSound.Play();
                        }
                    }
                }
                powerup.Update(gameTime);
                if (powerup.IsPicked && powerup.Type == PowerupType.Points)
                {
                    ScreenManager.Score += 90;
                    _coinSound.Play();
                }
            }
        }

        public void HandleBalls(GameTime gameTime, Ball ball)
        {
            ball.Update(gameTime);
            Dictionary<Tile, CollisionDirection> worldCollision = _tilemapForeground.Update(gameTime, ball.Hitbox, false, _breakSound);

            foreach (JayHawk chicken in _jayHawks)
            {
                CollisionDirection collision = CollisionHelper.Collides(ball.Hitbox, chicken.Hitbox);
                if (collision != CollisionDirection.NoCollision)
                {
                    chicken.IsShelled = true;
                    ball.HasHit = true;
                }
            }
            foreach (LongHorn cow in _longHorns)
            {
                CollisionDirection collision = CollisionHelper.Collides(ball.Hitbox, cow.Hitbox);
                if (collision != CollisionDirection.NoCollision)
                {
                    cow.IsShelled = true;
                    ball.HasHit = true;
                }
            }

            if (worldCollision.ContainsValue(CollisionDirection.CollisionBottom))
            {
                ball.Velocity.Y = 0;
            }
            else
            {
                ball.Velocity.Y += 10;
            }
            if (worldCollision.ContainsValue(CollisionDirection.CollisionBottom) && (worldCollision.ContainsValue(CollisionDirection.CollisionLeft) || worldCollision.ContainsValue(CollisionDirection.CollisionRight)))
            {
                Tile bottomTile = null;
                foreach (Tile tile in worldCollision.Keys)
                {
                    if (worldCollision[tile] == CollisionDirection.CollisionBottom)
                    {
                        if (bottomTile == null || (tile.Position.Y < bottomTile.Position.Y))
                        {
                            bottomTile = tile;
                        }
                    }
                }
                foreach (Tile tile in worldCollision.Keys)
                {
                    if (((worldCollision[tile] == CollisionDirection.CollisionLeft || worldCollision[tile] == CollisionDirection.CollisionRight) && tile.Bounds.Y < bottomTile.Bounds.Y))
                    {
                        ball.HasHit = true;
                        ball.Velocity.X = 0;
                        ball.Velocity.Y = 0;
                    }
                }
            }
            else if ((worldCollision.ContainsValue(CollisionDirection.CollisionLeft) || worldCollision.ContainsValue(CollisionDirection.CollisionRight)) || ball.Position.Y > 416) 
            {
                ball.HasHit = true;
                ball.Velocity.X = 0;
                ball.Velocity.Y = 0;
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            HandlePlayerInput(gameTime, input);
            if (!_player1.IsDead && !_player1.GettingBig)
            {
                foreach(var jayHawk in _jayHawks)
                {
                    if (!jayHawk.IsDead)
                    {
                        HandleJayhawkLogic(jayHawk, gameTime);
                    }
                }
                foreach (var longhorn in _longHorns)
                {
                    if (!longhorn.IsDead)
                    {
                        HandleLongHornLogic(longhorn, gameTime);
                    }
                }
                foreach (var powerup in _powerups)
                {
                    HandlePowerupLogic(powerup, gameTime);
                }
                foreach (var ball in _balls)
                {
                    if (!ball.IsDead)
                    {
                        HandleBalls(gameTime, ball);
                    }
                }
            }
            else
            {
                _player1.Velocity.X = 0;
                _player1.Velocity.Y += 15 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _deathTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_deathTimer < 0)
                {
                    if (ScreenManager.Lives > 0)
                    {
                        LoadingScreen.Load(ScreenManager, false, new MenuScreen());
                    }
                    else
                    {
                        LoadingScreen.Load(ScreenManager, true, new TitleScreen());
                    }
                }
            }
            if (!_player1.ShowScore)
            {
                _timer += gameTime.ElapsedGameTime;
            }
            else
            {
                _leaveTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_leaveTimer < 0)
                {
                    LoadingScreen.Load(ScreenManager, true, new TitleScreen());
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            if (!_player1.ShowScore)
            {
                float playerX;
                // Calculate our offset vector
                playerX = MathHelper.Clamp(_player1.Position.X, 300, 13600);
                _offsetX = 300 - playerX;

                Matrix transform;

                transform = Matrix.CreateTranslation(_offsetX, 0, 0);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, transformMatrix: transform);

                _tilemapBackground.Draw(gameTime, spriteBatch);

                foreach (var powerup in _powerups)
                {
                    powerup.Draw(gameTime, spriteBatch);
                }
                _tilemapForeground.Draw(gameTime, spriteBatch);
                _player1.Draw(gameTime, spriteBatch);
                foreach (var jayHawk in _jayHawks)
                {
                    jayHawk.Draw(gameTime, spriteBatch);
                }
                foreach (var longhorn in _longHorns)
                {
                    longhorn.Draw(gameTime, spriteBatch);
                }
                foreach (var zebra in _refs)
                {
                    zebra.Draw(gameTime, spriteBatch);
                }
                foreach (var ball in _balls)
                {
                    if (!ball.IsDead)
                    {
                        ball.Draw(gameTime, spriteBatch);
                    }
                }

                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                spriteBatch.Draw(
                _bg,
                new Vector2(0, 0),
                new Rectangle(0, 0, 256, 224),
                    Color.White,
                    0,
                    new Vector2(0, 0),
                    2f,
                    SpriteEffects.None,
                    0
                );
                spriteBatch.End();
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.DrawString(ScreenManager.Font, "SCORE: \n" + ScreenManager.Score.ToString(), new Vector2(25, 25), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "TIME: \n" + $"{_timer.Minutes:D2}:{_timer.Seconds:D2}", new Vector2(245, 25), Color.White);
            spriteBatch.DrawString(ScreenManager.Font, "LIVES: \n" + $"{ScreenManager.Lives}", new Vector2(425, 25), Color.White);
            spriteBatch.End();
        }
    }
}
