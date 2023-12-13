using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWB
{
    public class Tile
    {
        public Rectangle SourceRectangle;
        public Vector2 Position;
        public Rectangle InitialPosition;
        public Rectangle Bounds;
        public int ID;
        public int Index;
        public Vector2 Velocity;
        public bool Destroyed;

        public void Update(GameTime gameTime)
        {
            if (Destroyed)
            {
                Bounds = new Rectangle(0, 0, 0, 0);
                Velocity.Y += 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                if (Position.Y > InitialPosition.Y - 25 && Math.Abs(Velocity.Y) > 0)
                {
                    Position.Y += (float)Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Bounds.Y = (int)Position.Y;
                }
                else if (Position.Y <= InitialPosition.Y - 25 && Velocity.Y < 0)
                {
                    Velocity.Y *= -1;
                }
                else if (Position.Y < InitialPosition.Y && Velocity.Y > 0)
                {
                    Position.Y += (float)Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Bounds.Y = (int)Position.Y;
                }
                if (Position.Y >= InitialPosition.Y && Velocity.Y > 0)
                {
                    Velocity.Y = 0;
                    Position.Y = InitialPosition.Y;
                    Bounds.Y = (int)Position.Y;
                }
            }
            if (ID == 5 && gameTime.ElapsedGameTime.TotalSeconds % 2 == 0)
            {
                SourceRectangle.X -= 16;
                if (SourceRectangle.X < 48)
                {
                    SourceRectangle.X = 80;
                }
            }
        }
    }
}
