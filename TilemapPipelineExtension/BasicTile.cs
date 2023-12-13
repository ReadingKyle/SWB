using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TilemapPipelineExtension
{
    [ContentSerializerRuntimeType("SWB.Tile, SWB")]
    public class BasicTile
    {
        public Rectangle SourceRectangle;
        public Vector2 Position;
        public Rectangle InitialPosition;
        public Rectangle Bounds;
        public int ID;
        public int Index;
        public Vector2 Velocity;
        public bool Destroyed;
    }
}
