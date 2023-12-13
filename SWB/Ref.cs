using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWB
{
    public class Ref
    {
        public Vector2 Position;
        public bool FieldGoal;
        public bool Finish;

        private Texture2D _texture;
        private bool _leftRef;

        private int _animationFrame;
        private double _animationTimer;

        public Ref(Vector2 position, bool leftRef, Texture2D texture)
        {
            Position = position;
            _leftRef = leftRef;
            _texture = texture;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects direction = _leftRef ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (Finish)
            {
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 2)
                {
                    _animationFrame = FieldGoal ? 2 : 3;
                }
                else if (_animationTimer > 1)
                {
                    _animationFrame = 0;
                }
                else if (_animationTimer > 0.5)
                {
                    _animationFrame = 1;
                }
                else
                {
                    _animationFrame = 0;
                }
            }
            else
            {
                _animationFrame = 0;
            }

            spriteBatch.Draw(
                _texture,
                Position,
                new Rectangle(_animationFrame * 16, 0, 16, 16),
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
