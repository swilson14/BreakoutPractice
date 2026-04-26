using BreakoutPractice.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src.States
{
    public class StartState : State
    {
        private int highlighted;
        private SoundEffect _paddleHitSound;
        private SpriteFont _largeFont;
        private SpriteFont _mediumFont;

        public StartState(StateMachine stateMachine, SoundEffect paddleHitSound, SpriteFont largeFont, SpriteFont mediumFont) : base(stateMachine)
        {
            highlighted = 1;
            _paddleHitSound = paddleHitSound;
            _largeFont = largeFont;
            _mediumFont = mediumFont;
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (keyboard.WasKeyJustPressed(Keys.Up) || keyboard.WasKeyJustPressed(Keys.Down))
            {
                if (highlighted == 1)
                {
                    highlighted = 2;
                }
                else if (highlighted == 2)
                {
                    highlighted = 1;
                }
                _paddleHitSound.Play();
            }

            // Escape/Quit Logic is no longer global
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            string titleText = "BREAKOUT";
            float titleWidth = _largeFont.MeasureString(titleText).X;

            spriteBatch.DrawString(_largeFont,titleText,new Vector2((Constants.VIRTUAL_WIDTH - titleWidth) / 2 , Constants.VIRTUAL_HEIGHT / 3), Color.White);

            string startText = "START";
            float startWidth = _mediumFont.MeasureString(startText).X;
            Color startColor = Color.White;
            if (highlighted == 1)
            {
                startColor = new Color(103,255,255,255);
            }

            spriteBatch.DrawString(_mediumFont, startText, new Vector2((Constants.VIRTUAL_WIDTH - startWidth) / 2, Constants.VIRTUAL_HEIGHT / 2 + 70), startColor);

            string highScoreText = "HIGH SCORES";
            float highScoreWidth = _mediumFont.MeasureString(highScoreText).X;
            Color highScoreColor = Color.White;
            if (highlighted == 2)
            {
                highScoreColor = new Color(103, 255, 255, 255);
            }

            spriteBatch.DrawString(_mediumFont, highScoreText, new Vector2((Constants.VIRTUAL_WIDTH - highScoreWidth) / 2, Constants.VIRTUAL_HEIGHT / 2 + 90), highScoreColor);
        }

    }
}
