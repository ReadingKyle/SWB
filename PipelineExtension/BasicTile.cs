using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipelineExtension
{
    [ContentSerializerRuntimeType("SWB.Tile, SWB")]
    public class BasicTile
    {
        public Rectangle SourceRectangle;
        public Rectangle PositionRectangle;
        public Rectangle[] Bounds;
        public int ID;
    }
}
