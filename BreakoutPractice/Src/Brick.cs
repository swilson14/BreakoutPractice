using BreakoutPractice.Src.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src
{
   

    public class Brick
    {      
        public bool Active { get; set; }
        public float X { get; set; }
        public float Y { get; set; }      
        public int Width { get; set;  }
        public int Height { get; set; }

        public Sprite brickSprite { get; set; }
        public List<Sprite> BrickSprites { get; }

        public int Tier { get; set; }
        public int Color { get; set; }
   

        public SoundEffect BrickHit2 { get; }

        public Brick(SoundEffect brickHit, float x, float y, List<Sprite> brickSprites, int tier, int color)
        {
            BrickSprites = brickSprites;
            Tier = tier;
            Color = color;
            BrickHit2 = brickHit;
            X = x;
            Y = y;
            Width = 32;
            Height = 16;
            Active = true;
            
            int spriteIndex = ((Color - 1) * 4) + Tier;
            brickSprite = BrickSprites[spriteIndex];
        }

        public void BrickHit()
        {
            BrickHit2.Play();
            Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                brickSprite.Draw(spriteBatch, new Vector2(X,Y));
            }
        }
    }
}
