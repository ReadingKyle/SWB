using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWB
{
    public class JayHawk
    {
        public Vector2 Position;
        public bool IsSquished;
        public bool IsDead;
        public Vector2 Velocity;

        private Texture2D _texture;

        public Rectangle Hitbox;

        private double _animationTimer;
        private int _animationFrame;

        private double _squishedTimer;

        public JayHawk(Vector2 position, Texture2D texture) 
        {
            Position = position;
            _texture = texture;
            Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);
            Velocity = new Vector2(-1, 0);
        }

        public void Update(GameTime gameTime)
        {
            if (IsSquished)
            {
                _squishedTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_squishedTimer > 0.5)
                {
                    IsDead = true;
                }
            }
            else
            {
                Position += Velocity;
                Hitbox.X = (int)Position.X;
                Hitbox.Y = (int)Position.Y;
            }
        }

        public void Draw (GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsDead)
            {
                if (IsSquished)
                {
                    _animationFrame = 2;
                }
                else
                {
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
                new Rectangle(_animationFrame * 16, 40, 16, 16),
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
