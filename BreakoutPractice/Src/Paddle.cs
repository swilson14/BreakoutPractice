using BreakoutPractice.Input;
using BreakoutPractice.Src.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src
{
    public enum PaddleSize
    {
        smallest = 1,
        medium = 2,
        large = 3,
        huge = 4,
    }

    public class Paddle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float dX { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Skin { get; set;  }        
        public PaddleSize Size { get; set; }

        public Sprite PaddleSprite { get; set;  }

        public Paddle(List<Sprite> paddleSprites)
        {
            X = Constants.VIRTUAL_WIDTH / 2 - 32;
            Y = Constants.VIRTUAL_HEIGHT - 32;
            Width = 64;
            Height = 16;
            Skin = 1;
            Size = PaddleSize.medium;
            int spriteIndex = (int)Size + 4 * (Skin - 1);
            spriteIndex = spriteIndex - 1;
            PaddleSprite = paddleSprites[spriteIndex];
        }

        public void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            {
                dX = -Constants.PADDLE_SPEED;
            }
            else if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            {
                dX = Constants.PADDLE_SPEED;
            }
            else 
            { 
                dX = 0; 
            }

            // Prevent paddle from moving off screen
            if (dX < 0)
            {
                X = Math.Max(0, X + (float)(dX * gameTime.ElapsedGameTime.TotalSeconds));
            }
            else 
            {
                X = Math.Min(Constants.VIRTUAL_WIDTH - Width, X + (float)(dX * gameTime.ElapsedGameTime.TotalSeconds));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PaddleSprite.Draw(spriteBatch, new Vector2(X, Y));
        }

    }
}
