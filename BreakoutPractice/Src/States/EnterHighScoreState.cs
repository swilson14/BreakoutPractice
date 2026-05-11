using BreakoutPractice.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src.States
{
    public class EnterHighScoreState : State
    {
        private HighScoreList _highScores;
        private int _score;
        private int _scoreIndex;
        private SpriteFont _largeFont;
        private SpriteFont _mediumFont;
        private SpriteFont _smallFont;
        private char[] _chars = { 'A', 'A', 'A' };
        private int _highlightedChar;
        private SoundEffect _selectSound;

        public EnterHighScoreState(StateMachine stateMachine, Dictionary<string, SpriteFont> fonts, Dictionary<string,SoundEffect> soundEffects) : base(stateMachine)
        {
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];
            _smallFont = fonts["small"];
            _selectSound = soundEffects["select"];
            _highlightedChar = 0;
        }

        public override void Enter(params object[] args)
        {
            _highScores = (HighScoreList)args[0];
            _score = (int)args[1];
            _scoreIndex = (int)args[2];
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (keyboard.WasKeyJustPressed(Keys.Enter))
            {
                string name = new string(_chars);

                for (int i = 8; i >= _scoreIndex; i--)
                {
                    _highScores.HighScores[i + 1] = new HighScore
                    {
                        Name = _highScores.HighScores[i].Name,
                        Score = _highScores.HighScores[i].Score,
                    };

                };

                _highScores.HighScores[_scoreIndex] = new HighScore
                {
                    Name = name,
                    Score = _score,
                };

                // Write to file
                string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string gameFolder = Path.Combine(dataPath, "breakout");
                string dataFile = Path.Combine(gameFolder, "breakout.dat");
                string scoreString = "";

                for (int i = 0; i < 10; i++)
                {
                    scoreString += _highScores.HighScores[i].Name + "\n";
                    scoreString += _highScores.HighScores[i].Score + "\n";
                }

                File.WriteAllText(dataFile, scoreString);
                _stateMachine.ChangeState("high_scores",_highScores);
            }
            
            if (keyboard.WasKeyJustPressed(Keys.Left) && _highlightedChar > 0)
            {
                _highlightedChar--;
                _selectSound.Play();
            }
            else if (keyboard.WasKeyJustPressed(Keys.Right) && _highlightedChar < 2)
            {
                _highlightedChar++;
                _selectSound.Play();
            }

            if (keyboard.WasKeyJustPressed(Keys.Up)) 
            {
                _chars[_highlightedChar]++;
                if (_chars[_highlightedChar] > 90)
                {
                    _chars[_highlightedChar] = 'A';
                }
            }
            else if (keyboard.WasKeyJustPressed(Keys.Down))
            {
                if (_chars[_highlightedChar] < 65)
                {
                    _chars[_highlightedChar] = 'Z';
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            string yourScoreText = $"Your score: {_score}";
            float yourScoreWidth = _mediumFont.MeasureString(yourScoreText).X;
            spriteBatch.DrawString(_mediumFont, yourScoreText, new Vector2((Constants.VIRTUAL_WIDTH - yourScoreWidth) / 2, 30), Color.White);
            
            Color char1Color = Color.White;
            Color char2Color = Color.White;
            Color char3Color = Color.White;

            if (_highlightedChar == 0)
            {
                char1Color = new Color(103, 255, 255, 255);
            }
            else if (_highlightedChar == 1)
            {
                char2Color = new Color(103, 255, 255, 255);
            }
            else if (_highlightedChar == 2)
            {
                char3Color = new Color(103, 255, 255, 255);
            }

            spriteBatch.DrawString(_largeFont, _chars[0].ToString(), 
                new Vector2(Constants.VIRTUAL_WIDTH / 2 - 28, Constants.VIRTUAL_HEIGHT / 2),
                char1Color
                );

            spriteBatch.DrawString(_largeFont, _chars[1].ToString(),
                new Vector2(Constants.VIRTUAL_WIDTH / 2 - 6, Constants.VIRTUAL_HEIGHT / 2),
                char2Color
                );

            spriteBatch.DrawString(_largeFont, _chars[2].ToString(),
                new Vector2(Constants.VIRTUAL_WIDTH / 2 + 20, Constants.VIRTUAL_HEIGHT / 2),
                char3Color
                );

            string pressEnterText = "Press Enter to confirm!";
            float pressEnterWidth = _smallFont.MeasureString(pressEnterText).X;

            spriteBatch.DrawString(_smallFont, pressEnterText,
                new Vector2(Constants.VIRTUAL_WIDTH - pressEnterWidth / 2, Constants.VIRTUAL_HEIGHT - 18),
                Color.White);

        }
    }
}
