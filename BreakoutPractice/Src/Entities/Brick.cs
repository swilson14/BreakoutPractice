using BreakoutPractice.Src.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Particles.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src.Entities
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

        public SoundEffect BrickHit1 { get; }
        public SoundEffect BrickHit2 { get; }

        public Dictionary<string, SoundEffect> SoundEffects { get;  }

        private ParticleEffect _particleEffect;
        private Dictionary<string, Vector3> _hslColors;

        private bool _isLockedBrick;
        private Sprite _lockedBrickSprite;


        public Brick(Dictionary<string, SoundEffect> soundEffects, float x, float y, List<Sprite> brickSprites, int tier, int color, ParticleEffect particleEffect, Sprite lockedBrickSprite)
        {
            SoundEffects = soundEffects;
            BrickSprites = brickSprites;
            Tier = tier;
            Color = color;
            BrickHit1 = SoundEffects["brick_hit_1"];
            BrickHit2 = SoundEffects["brick_hit_2"];
            X = x;
            Y = y;
            Width = 32;
            Height = 16;
            Active = true;
            _particleEffect = particleEffect;

            _hslColors = new Dictionary<string, Vector3>
            {
                {"blue", new Vector3(218f, 1.0f, 0.7f)},
                {"green", new Vector3(95f, 0.6f, 0.47f)},
                {"red", new Vector3(354f, 0.6f, 0.6f)},
                {"purple", new Vector3(319f, 0.5f, 0.7f)},
                {"gold", new Vector3(57f, 1.0f, 0.6f)}
            };
            _isLockedBrick = false;
            _lockedBrickSprite = lockedBrickSprite;
        }

        public void BrickHit()
        {          
            BrickHit2.Play();

            Vector3 currentHslColor = new Vector3();

            // blue
            switch (Color)
            {
                case 1:
                    currentHslColor = _hslColors["blue"];
                    break;
                case 2:
                    currentHslColor = _hslColors["green"];
                    break;
                case 3:
                    currentHslColor = _hslColors["red"];
                    break;
                case 4:
                    currentHslColor = _hslColors["purple"];
                    break;
                case 5:
                    currentHslColor = _hslColors["gold"];
                    break;
            }

            _particleEffect.Emitters[0].Parameters.Color = new ParticleColorParameter(currentHslColor);
            _particleEffect.Position = new Vector2(X,Y);
            _particleEffect.Trigger();

            if (Tier > 0)
            {
                if (Color == 1)
                {
                    Tier -= 1;
                    Color = 5;
                }
                else
                {
                    Color -= 1;
                }
            }
            else
            {
                if (Color == 1)
                {
                    Active = false;
                }
                else
                {
                    Color -= 1;
                }
            }
            
            if (Active == false)
            {
                BrickHit1.Play();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int spriteIndex = ((Color - 1) * 4) + Tier;
            brickSprite = BrickSprites[spriteIndex];

            if (Active && _isLockedBrick)
            {
                brickSprite = _lockedBrickSprite;
                brickSprite.Draw(spriteBatch, new Vector2(X,Y));
            }
            else if (Active)
            {
                brickSprite.Draw(spriteBatch, new Vector2(X, Y));
            }
        }
    }
}
