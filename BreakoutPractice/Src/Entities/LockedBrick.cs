using BreakoutPractice.Src.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src.Entities
{
    public class LockedBrick
    {
        public bool Active { get; set; }
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Sprite LockedBrickSprite { get; set; }
        public SoundEffect BrickHit2 { get; }

        public LockedBrick()
        {
            
        }
    }
}
