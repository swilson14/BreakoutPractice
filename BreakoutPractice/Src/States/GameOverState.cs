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
        private HighScoreList _highScores;
        private SoundEffect _highScoreSound;

        private Game _game;

        public GameOverState(StateMachine stateMachine, Dictionary<string, SpriteFont> fonts, Dictionary<string, SoundEffect> soundEffects, Game game) : base(stateMachine)
        {
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];
            _highScoreSound = soundEffects["high_score"];
            _game = game;
        }

        public override void Enter(params object[] args)
        {
            _score = (int)args[0];
            _highScores = (HighScoreList)args[1];
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (keyboard.WasKeyJustPressed(Keys.Enter)) 
            {
                bool isHighScore = false;

                int scoreIndex = 10;

                for (int i = 9; i >= 0; i--)
                {
                    int currentHighScore = _highScores.HighScores[i].Score;
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
                _game.Exit();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

        public override void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight)
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
                new Vector2((viewportWidth - gameOverWidth) / 2, viewportHeight / 3),
                Color.White);

            spriteBatch.DrawString(
                _mediumFont,
                finalScoreText,
                new Vector2((viewportWidth - finalScoreWidth) / 2, viewportHeight / 2),
                Color.White);

            spriteBatch.DrawString(
                _mediumFont,
                pressEnterText,
                new Vector2((viewportWidth - pressEnterWidth) / 2, viewportHeight - viewportHeight / 4),
                Color.White);
        }
    }
}