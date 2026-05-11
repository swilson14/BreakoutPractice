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
using static System.Formats.Asn1.AsnWriter;

namespace BreakoutPractice.Src.States
{
    public class HighScoreState : State
    {
        private HighScoreList _highScores;
        private SoundEffect _wallHitSound;
        private SpriteFont _largeFont;
        private SpriteFont _mediumFont;
        private SpriteFont _smallFont;


        public HighScoreState(StateMachine stateMachine, Dictionary<string, SpriteFont> fonts, Dictionary<string, SoundEffect> soundEffects) : base(stateMachine)
        {
            _wallHitSound = soundEffects["wall_hit"];
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];
            _smallFont = fonts["small"];
        }

        public override void Enter(params object[] args)
        {
            _highScores = (HighScoreList)args[0];
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            // Escape/Quit Logic is no longer global
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
            {
                _wallHitSound.Play();
                _stateMachine.ChangeState("start", _highScores);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            string HighScoresTitle = "High Scores";
            float HighScoreTitleWidth = _largeFont.MeasureString(HighScoresTitle).X;
            spriteBatch.DrawString(_largeFont, HighScoresTitle, new Vector2((Constants.VIRTUAL_WIDTH - HighScoreTitleWidth) / 2, 20), Color.White);

            // Iterate over all high scores.
            for (int i = 0; i < 10; i++)
            {
                string name = _highScores.HighScores[i].Name;
                string score = _highScores.HighScores[i].Score.ToString();
                int scoreNumber = i + 1;
                string scoreNumberText = $"{scoreNumber}.";
                float scoreNumberWidth = _mediumFont.MeasureString(scoreNumberText).X;

                spriteBatch.DrawString(
                    _mediumFont,
                    scoreNumberText,
                    new Vector2(Constants.VIRTUAL_WIDTH / 4, 60 + scoreNumber + scoreNumber * 13),
                    Color.White
                    );

                spriteBatch.DrawString(
                    _mediumFont,
                    name,
                    new Vector2(Constants.VIRTUAL_WIDTH / 4 + 38, 60 + scoreNumber + scoreNumber * 13), // right aligned??
                    Color.White
                    );

                spriteBatch.DrawString(
                    _mediumFont,
                    score,
                    new Vector2(Constants.VIRTUAL_WIDTH / 2, 60 + scoreNumber + scoreNumber * 13), // right aligned??
                    Color.White
                    );

            }

            string pressEscapeText = "Press Escape to return to the main menu!";
            float pressEscapeWidth = _smallFont.MeasureString(pressEscapeText).X;
            spriteBatch.DrawString(
                _smallFont,
                pressEscapeText,
                new Vector2((Constants.VIRTUAL_WIDTH - pressEscapeWidth) / 2, Constants.VIRTUAL_HEIGHT - 18),
                Color.White
                );
        }
    }
}
