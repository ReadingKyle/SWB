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
        public static List<CollisionDirection> Collides(Rectangle a, Rectangle b)
        {
            List<CollisionDirection> result = new List<CollisionDirection>();
            if (!(a.Right < b.Left || a.Left > b.Right ||
                    a.Top > b.Bottom || a.Bottom < b.Top))
            {
                if ((a.Right > b.Left && a.Left < b.Right) && a.Top < b.Bottom)
                {
                    result.Add(CollisionDirection.CollisionTop);
                }
                else if ((a.Right > b.Left && a.Left < b.Right) && a.Bottom > b.Top)
                {
                    result.Add(CollisionDirection.CollisionBottom);
                }
                if ((a.Top < b.Bottom && a.Bottom-2 > b.Top) && a.Right > b.Left)
                {
                    result.Add(CollisionDirection.CollisionRight);
                }
                else if ((a.Top < b.Bottom && a.Bottom-2 > b.Top) && a.Left < b.Right)
                {
                    result.Add(CollisionDirection.CollisionLeft);
                }
            }
            return result;
        }
    }
}
