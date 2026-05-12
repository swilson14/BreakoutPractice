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
        public Vector2 Velocity { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Skin { get; set;  }        
        public PaddleSize Size { get; set; }
        public Sprite PaddleSprite { get; set; }

        private List<Sprite> _paddleSprites;

        public Paddle(List<Sprite> paddleSprites)
        {
            X = Constants.VIRTUAL_WIDTH / 2 - 32;
            Y = Constants.VIRTUAL_HEIGHT - 32;
            Width = 64;
            Height = 16;
            Skin = 1;
            Size = PaddleSize.medium;
            _paddleSprites = paddleSprites;
            int spriteIndex = (int)Size + 4 * (Skin - 1);
            spriteIndex = spriteIndex - 1;
            PaddleSprite = _paddleSprites[spriteIndex];
        }

        public void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            {
                Velocity = new Vector2(-Constants.PADDLE_SPEED, 0);
            }
            else if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            {
                Velocity = new Vector2(Constants.PADDLE_SPEED, 0);
            }
            else
            {
                Velocity = Vector2.Zero;
            }

            // Prevent paddle from moving off screen
            if (Velocity.X < 0)
            {
                X = Math.Max(0, X + (float)(Velocity.X * gameTime.ElapsedGameTime.TotalSeconds));
            }
            else
            {
                X = Math.Min(Constants.VIRTUAL_WIDTH - Width, X + (float)(Velocity.X * gameTime.ElapsedGameTime.TotalSeconds));
            }

            // Move along Y axis.
            if (Velocity.Y < 0)
            {
                Y = Math.Max(Constants.VIRTUAL_HEIGHT - 75, Y + (float)(Velocity.Y * gameTime.ElapsedGameTime.TotalSeconds));
            }
            else
            {
                Y = Math.Min(Constants.VIRTUAL_HEIGHT - Height, Y + (float)(Velocity.Y * gameTime.ElapsedGameTime.TotalSeconds));
            }

        }

        public void ChangeSkin(int skin)
        {
            Skin = skin;
            int spriteIndex = (int)Size + 4 * (Skin - 1);
            spriteIndex = spriteIndex - 1;
            PaddleSprite = _paddleSprites[spriteIndex];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PaddleSprite.Draw(spriteBatch, new Vector2(X, Y));
        }

    }
}
