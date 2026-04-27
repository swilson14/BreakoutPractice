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
    public class PlayState : State
    {
        private Paddle _paddle;
        private SoundEffect _pauseSound;
        private SpriteFont _largeFont;
        private bool _paused;

        public PlayState(StateMachine stateMachine, Paddle paddle, SoundEffect pauseSound, SpriteFont largeFont) : base(stateMachine)
        {
            _paddle = paddle;
            _pauseSound = pauseSound;
            _largeFont = largeFont;
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (_paused)
            {
                if (keyboard.WasKeyJustPressed(Keys.Space))
                {
                    _paused = false;
                    _pauseSound.Play();
                }
                else 
                {
                    return;
                }
            }
            else if (keyboard.WasKeyJustPressed(Keys.Space))
            {
                _paused = true;
                _pauseSound.Play();
                return;
            }

            _paddle.Update(gameTime, keyboard);

            // Escape/Quit Logic is no longer global
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _paddle.Draw(spriteBatch);
            if (_paused)
            {
                string pauseText = "PAUSED";
                float pauseTextWidth = _largeFont.MeasureString(pauseText).X;
                spriteBatch.DrawString(_largeFont, pauseText, new Vector2(pauseTextWidth, Constants.VIRTUAL_HEIGHT - 16), Color.White);
            }

        }
    }
}
