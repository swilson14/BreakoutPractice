using BreakoutPractice.Src.Entities;
using BreakoutPractice.Src.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src
{
    public class LevelMaker
    {
        enum LevelPattern
        {
            NONE,
            SINGLE_PYRAMID,
            MULTI_PYRAMID,
        }

        enum RowPattern 
        {
            SOLID,
            ALTERNATE,
            SKIP,
            NONE
        }

        private int _numRows;
        private int _numCols;
        private Random _random;
        private Brick _template;
        private ParticleEffect _particleEffect;
        private Sprite _lockedBrick;

        public LevelMaker(Random random, Brick template, ParticleEffect particleEffect, Sprite lockedBrick)
        {
            _random = random;
            _template = template;
            _particleEffect = particleEffect;
            _lockedBrick = lockedBrick;
        }

        public List<Brick> CreateMap(int level)
        {
            List<Brick> map = new List<Brick>();

            _numRows = _random.Next(1, 6);
            _numCols = _random.Next(7, 14);

            // Ensure number of columns is odd.
            if (_numCols % 2 == 0)
            {
                _numCols += 1;
            }

            // Highest tier and color based on level.
            int highestTier = Math.Min(3, (int)Math.Floor(level / 5.0));
            int highestColor = Math.Min(5, level % 5 + 3);

            for (int row = 1; row <= _numRows; row++)
            {
                bool skipPattern = false;
                if (_random.Next(2) == 1)
                {
                    skipPattern = true;
                }

                bool altPattern = false;
                if (_random.Next(2) == 1)
                {
                    altPattern = true;
                }

                int altColor1 = _random.Next(1, highestColor + 1);
                int altColor2 = _random.Next(1, highestColor + 1);
                
                int altTier1 = _random.Next(0, highestTier + 1);
                int altTier2 =  _random.Next(0, highestTier + 1);

                bool skipFlag = false;
                if (_random.Next(2) == 1)
                {
                    skipFlag = true;
                }

                bool altFlag = false;
                if (_random.Next(2) == 1)
                {
                    altFlag = true;
                }

                int solidColor = _random.Next(1, highestColor);
                int solidTier = _random.Next(0,highestTier);

                for (int col = 1; col <= _numCols; col++)
                {
                    if (skipPattern && skipFlag)
                    {
                        skipFlag = false;
                        continue;
                    }
                    else 
                    {
                        skipFlag = !skipFlag;
                    }

                    // Calculate x and y coordinates
                    int x = (col - 1) * 32 + 8 + (13 - _numCols) * 16;
                    int y = row * 16;

                    int tier = Constants.DEFAULT_BRICK_TIER;
                    int color = Constants.DEFAULT_BRICK_COLOR;
                    if (altPattern && altFlag)
                    {
                        color = altColor1;
                        tier = altTier1;
                        altFlag = !altFlag;
                    }
                    else
                    {
                        color = altColor2;
                        tier = altTier2;
                        altFlag = !altFlag;
                    }

                    if (!altPattern)
                    {
                        color = solidColor;
                        tier = solidTier;
                    }

                    Brick brick = new Brick(_template.SoundEffects, x, y, _template.BrickSprites, tier, color, _particleEffect);

                    map.Add(brick);
                }
            }

            if (map.Count == 0)
            {
                return this.CreateMap(level);
            }
            else
            {
                return map;
            }          
        }          
    }
}
