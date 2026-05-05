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
    public class GameOverState : State
    {
        private int _score;
        private SpriteFont _largeFont;
        private SpriteFont _mediumFont;
        private HighScore[] _highScores;
        private SoundEffect _highScoreSound;

        public GameOverState(StateMachine stateMachine, Dictionary<string, SpriteFont> fonts, Dictionary<string, SoundEffect> soundEffects) : base(stateMachine)
        {
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];
            _highScoreSound = soundEffects["high_score"];
        }

        public override void Enter(params object[] args)
        {
            _score = (int)args[0];
            _highScores = (HighScore[])args[1];
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (keyboard.WasKeyJustPressed(Keys.Enter)) 
            {
                bool isHighScore = false;

                int scoreIndex = 11;

                for (int i = 9; i >= 0; i--)
                {
                    int currentHighScore = _highScores[i].Score;
                    if (_score > currentHighScore)
                    {
                        scoreIndex = i;
                        isHighScore = true;
                    }
                }

                if (isHighScore)
                {
                    _highScoreSound.Play();
                    _stateMachine.ChangeState("enter_high_score", _highScores, _score, scoreIndex);
                }
                else
                {
                    _stateMachine.ChangeState("start", _highScores);
                }
            }

            // Escape/Quit Logic is no longer global
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            string gameOverText = "GAME OVER";
            float gameOverWidth = _largeFont.MeasureString(gameOverText).X;

            string finalScoreText = $"Final Score: {_score}";
            float finalScoreWidth = _mediumFont.MeasureString(finalScoreText).X;

            string pressEnterText = "Press Enter!";
            float pressEnterWidth = _mediumFont.MeasureString(pressEnterText).X;

            spriteBatch.DrawString(
                _largeFont, 
                gameOverText, 
                new Vector2((Constants.VIRTUAL_WIDTH - gameOverWidth) / 2, Constants.VIRTUAL_HEIGHT / 3),
                Color.White);

            spriteBatch.DrawString(
                _mediumFont,
                finalScoreText,
                new Vector2((Constants.VIRTUAL_WIDTH - finalScoreWidth) / 2, Constants.VIRTUAL_HEIGHT / 2),
                Color.White);

            spriteBatch.DrawString(
                _mediumFont,
                pressEnterText,
                new Vector2((Constants.VIRTUAL_WIDTH - pressEnterWidth) / 2, Constants.VIRTUAL_HEIGHT - Constants.VIRTUAL_HEIGHT / 4),
                Color.White);
        }
    }
}