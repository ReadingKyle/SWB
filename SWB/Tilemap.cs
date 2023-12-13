using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace SWB
{
    public class Tilemap
    {
        /// <summary>
        /// The map width
        /// </summary>
        public int MapWidth { get; init; }

        /// <summary>
        /// The map height
        /// </summary>
        public int MapHeight { get; init; }

        /// <summary>
        /// The width of a tile in the map
        /// </summary>
        public int TileWidth { get; init; }

        /// <summary>
        /// The height of a tile in the map
        /// </summary>
        public int TileHeight { get; init; }

        /// <summary>
        /// The texture containing the tiles
        /// </summary>
        public Texture2D TilesetTexture { get; init; }

        /// <summary>
        /// An array of source rectangles corresponding to
        /// tile positions in the texture
        /// </summary>
        public Tile[] Tiles { get; init; }

        public Rectangle[] TileSet { get; init; }


        /// <summary>
        /// Checks to see if a sprite has collided with another sprite
        /// </summary>
        /// <param name="other">other sprite</param>
        /// <returns></returns>
        public CollisionDirection CollidesWith(Rectangle player, Rectangle tile)
        {
            return CollisionHelper.Collides(player, tile);
        }

        /// <summary>
        /// The map data - an array of indices to the Tile array
        /// </summary>
        public int[] TileIndices { get; init; }

        public Dictionary<Tile, CollisionDirection> Update(GameTime gameTime, Rectangle playerHitbox, bool canDestroy, SoundEffectInstance breakSound)
        {
            Dictionary<Tile, CollisionDirection> result = new Dictionary<Tile, CollisionDirection>();
            foreach (var tile in Tiles)
            {
                if (tile.Position.Y < 416)
                {
                    CollisionDirection direction = CollidesWith(playerHitbox, tile.Bounds);
                    if (direction != CollisionDirection.NoCollision)
                    {
                        result[tile] = direction;
                    }
                    if (direction == CollisionDirection.CollisionTop && (tile.ID == 7 || tile.ID == 5) && tile.Velocity.Y == 0)
                    {
                        if (canDestroy && tile.ID == 7)
                        {
                            tile.Velocity.Y = -100;
                            tile.Destroyed = true;
                            breakSound.Play();
                        }
                        else
                        {
                            tile.Velocity.Y = -16.5f;
                        }
                    }
                    tile.Update(gameTime);
                }
            }
            return result;
        }

        /// <summary>
        /// Draws the tilemap. Assumes that spriteBatch.Begin() has been called.
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">a spritebatch to draw with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*for(int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    // Indices start at 1, so shift by 1 for array coordinates
                    int index = TileIndices[y * MapWidth + x] - 1;

                    // Index of -1 (shifted from 0) should not be drawn
                    if (index == -1) continue;

                    // Draw the current tile
                    spriteBatch.Draw(
                        TilesetTexture,
                        new Rectangle(
                            x * TileWidth,
                            y * TileHeight,
                            TileWidth,
                            TileHeight
                            ),
                        TileSet[index],
                        Color.White
                    );
                }
            }*/
            foreach (Tile tile in Tiles)
            {
                Rectangle source = new Rectangle
                (
                    (int)tile.Position.X,
                    (int)tile.Position.Y,
                    tile.InitialPosition.Width,
                    tile.InitialPosition.Height
                );
                int index = tile.ID - 1;
                if (index == -1) continue;

                spriteBatch.Draw(
                    TilesetTexture,
                    source,
                    TileSet[index],
                    Color.White
                );
            }
        }
    }
}
