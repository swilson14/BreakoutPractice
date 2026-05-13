using BreakoutPractice.Src.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src.Entities
{
    public class ExtraBallPowerup
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Sprite Sprite { get; set; }

        private float _speed = 10f;

        public ExtraBallPowerup(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
            Velocity = new Vector2(0,1);
        }

        public bool Collides(Rectangle target)
        {
            if (Position.X > target.X + target.Width || target.X > Position.X + Sprite.Width)
            {
                return false;
            }

            if (Position.Y > target.Y + target.Height || target.Y > Position.Y + Sprite.Height)
            {
                return false;
            }

            return true;
        }

        public void Update(GameTime gameTime)
        {
            float Y = (float)(Position.Y + Velocity.Y * _speed* gameTime.ElapsedGameTime.TotalSeconds);
            Position = new Vector2(Position.X, Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}
