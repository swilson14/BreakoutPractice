using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src
{
    public class LevelMaker
    {
        private int _numRows;
        private int _numCols;
        public Random RNG;
        public Brick _template;


        public LevelMaker(Random random, Brick template)
        {
            RNG = random;
            _template = template;
        }

        public List<Brick> CreateMap()
        {
            List<Brick> map = new List<Brick>();
            _numRows = RNG.Next(1, 6);
            _numCols = RNG.Next(7, 13);

            for (int row = 0; row < _numRows; row++)
            {
                for (int col = 0; col < _numCols; col++)
                {
                    // Calculate x and y coordinates
                    int x = (col - 1) * 32 + 8 + (13 - _numCols) * 16;
                    int y = row * 16;
                    map.Add(new Brick(_template.BrickHit2, x, y, _template.BrickSprites));
                }
            }
            return map;
        }
           
    }
}
