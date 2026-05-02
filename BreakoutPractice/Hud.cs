using BreakoutPractice.Src;
using BreakoutPractice.Src.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice
{
    public class Hud
    {
        private int _health;
        private int _score;
        private List<Sprite> _heartSprites;
        private SpriteFont _smallFont;

        public Hud(int health, int score, List<Sprite> heartSprites, SpriteFont smallFont)
        {
            _health = health;
            _score = score;
            _smallFont = smallFont;
        }

        public void Update(int health, int score)
        {
            _health = health;
            _score = score;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawHealth(spriteBatch);
            DrawScore(spriteBatch);
        }

        private void DrawHealth(SpriteBatch spriteBatch)
        {
            int healthX = Constants.VIRTUAL_WIDTH - 100;

            for (int i = 1; i < _health; i++)
            {
                _heartSprites[0].Draw(spriteBatch,new Vector2(healthX, 4));
                healthX += 11;
            }

            for (int i = 1; i < 3 - _health; i++)
            {
                _heartSprites[1].Draw(spriteBatch, new Vector2(healthX, 4));
                healthX = healthX += 11;
            }

        }

        private void DrawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_smallFont, $"Score: {_score}", new Vector2(50,5), Color.White);
        }

    }
}
