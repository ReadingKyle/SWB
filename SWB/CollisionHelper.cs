using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWB
{
    public enum CollisionDirection
    {
        NoCollision,
        CollisionTop,
        CollisionBottom,
        CollisionLeft,
        CollisionRight
    }
    public static class CollisionHelper
    {
        /// <summary>
        /// Detects a collision between two BoundingRectangles
        /// </summary>
        /// <param name="a">The first rectangle</param>
        /// <param name="b">The second rectangle</param>
        /// <returns>true for collision, false otherwise</returns>
        public static CollisionDirection Collides(Rectangle a, Rectangle b)
        {
            CollisionDirection result = CollisionDirection.NoCollision;

            if (a.Intersects(b))
            {
                int overlapX = Math.Min(a.Right, b.Right) - Math.Max(a.Left, b.Left);
                int overlapY = Math.Min(a.Bottom, b.Bottom) - Math.Max(a.Top, b.Top);

                if (overlapX >= overlapY)
                {
                    if (a.Top < b.Top)
                        result = CollisionDirection.CollisionBottom;
                    else
                        result = CollisionDirection.CollisionTop;
                }
                else
                {
                    if (a.Left < b.Left)
                        result = CollisionDirection.CollisionRight;
                    else
                        result = CollisionDirection.CollisionLeft;
                }
            }

            return result;
        }

    }
}
