using BreakoutPractice.Src.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src
{
    public class Ball
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 Velocity { get; set; }

        public int Skin { get; set; }

        private SoundEffect _wallHit;

        private Sprite ballSprite;

        public Ball(int skin, SoundEffect wallHit, List<Sprite> ballSprites)
        {
            Width = 8;
            Height = 8;
            Velocity = Vector2.Zero;
            Skin = skin;
            _wallHit = wallHit;
            ballSprite = ballSprites[Skin];
        }

        public bool Collides(Rectangle target)
        {
            if (X > target.X + target.Width || target.X > X + Width)
            {
                return false;
            }

            if (Y > target.Y  || target.Y > Y + Height)
            {
                return false;
            }

            return true;
        }

        public void Reset()
        {
            X = Constants.VIRTUAL_WIDTH / 2 - 2;
            Y = Constants.VIRTUAL_HEIGHT / 2 - 2;
            Velocity = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            X += (float)(Velocity.X * gameTime.ElapsedGameTime.TotalSeconds);
            Y += (float)(Velocity.Y * gameTime.ElapsedGameTime.TotalSeconds);

            if (X <= 0)
            {
                X = 0;
                Velocity = new Vector2(-Velocity.X,Velocity.Y);
                _wallHit.Play();
            }

            if (X >= Constants.VIRTUAL_WIDTH - 8)
            {
                X = Constants.VIRTUAL_WIDTH - 8;
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
                _wallHit.Play();
            }

            if (Y <= 0)
            {
                Y = 0;
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
                _wallHit.Play();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ballSprite.Draw(spriteBatch, new Vector2(X, Y));
        }
    }
}
