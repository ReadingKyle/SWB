using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SWB.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWB
{
    public enum PowerupType
    {
        Big,
        Balls,
        Points
    }
    public class Powerup
    {
        public Vector2 InitialPosition;
        public Vector2 Position;
        public PowerupType Type;
        public Vector2 Velocity;
        public bool IsReleased;
        public bool IsActive;
        public bool Persist;
        public bool IsPicked;
        public Rectangle Hitbox;

        public double timer;

        private Texture2D _texture;

        public Powerup(Vector2 position, PowerupType type, Texture2D texture, bool persist)
        {
            InitialPosition = position;
            Position = position;
            Type = type;
            Velocity.Y = 30;
            _texture = texture;
            timer = 0.2;
            Persist = persist;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }

        public void Update(GameTime gameTime)
        {
            if (IsReleased)
            {
                if (timer > 0 && Persist)
                {
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (Persist && Position.Y > InitialPosition.Y - 32)
                {
                    Position.Y -= Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    IsActive = true;
                }
                else if (!Persist && Position.Y > InitialPosition.Y - 48)
                {
                    Position.Y -= Velocity.Y * 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    IsActive = true;
                    timer = 0.5;
                }
                else if (!Persist && Position.Y <= InitialPosition.Y - 48)
                {
                    if (timer <= 0)
                    {
                        IsPicked = true;
                    }
                    else
                    {
                        timer -= gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsPicked && IsActive)
            {
                spriteBatch.Draw(
                    _texture,
                    Position,
                    new Rectangle(16 * (int)Type, 0, 16, 16),
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
}
