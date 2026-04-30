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

        public Hud(int health, int score)
        {
            _health = health;
            _score = score;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawHealth(spriteBatch);
        }

        private void DrawHealth(SpriteBatch spriteBatch)
        {
            // TODO: 


        }
    }
}
