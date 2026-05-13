using BreakoutPractice.Input;
using BreakoutPractice.Src.Entities;
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
        private SoundEffect _confirmSound;
        private List<Brick> _bricks;
        private Paddle _paddle;
        private int _health;
        private int _score;
        private int _level;
        private LevelMaker _levelMaker;
        private HighScoreList _highScores;

        public StartState(StateMachine stateMachine, Dictionary<string,SoundEffect> gameSounds, Dictionary<string,SpriteFont> fonts, LevelMaker levelMaker, Paddle paddle, int health, int score) : base(stateMachine)
        {
            highlighted = 1;
            _paddleHitSound = gameSounds["paddle_hit"];
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];
            _confirmSound = gameSounds["confirm"];
            _paddle = paddle;
            _health = health;
            _score = score;
            _level = 1;
            _levelMaker = levelMaker;
            _bricks = _levelMaker.CreateMap(_level);
        }

        public override void Enter(params object[] args)
        {
            _bricks = _levelMaker.CreateMap(_level);
            _highScores = (HighScoreList)args[0];
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

            if (keyboard.WasKeyJustPressed(Keys.Enter)) 
            {
                _confirmSound.Play();

                if (highlighted == 1)
                {
                    _stateMachine.ChangeState("paddle_select", _paddle, _bricks, _health, _score, _level, _highScores);
                }
                else 
                {
                    _stateMachine.ChangeState("high_scores", _highScores);
                }
            }

            // Escape/Quit Logic is no longer global
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

        public override void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight)
        {
            string titleText = "BREAKOUT";
            float titleWidth = _largeFont.MeasureString(titleText).X;

            spriteBatch.DrawString(_largeFont, titleText, new Vector2((viewportWidth - titleWidth) / 2, viewportHeight / 3), Color.White);

            string startText = "START";
            float startWidth = _mediumFont.MeasureString(startText).X;
            Color startColor = Color.White;
            if (highlighted == 1)
            {
                startColor = new Color(103, 255, 255, 255);
            }

            spriteBatch.DrawString(_mediumFont, startText, new Vector2((viewportWidth - startWidth) / 2, viewportHeight / 2 + 210), startColor);

            string highScoreText = "HIGH SCORES";
            float highScoreWidth = _mediumFont.MeasureString(highScoreText).X;
            Color highScoreColor = Color.White;
            if (highlighted == 2)
            {
                highScoreColor = new Color(103, 255, 255, 255);
            }

            spriteBatch.DrawString(_mediumFont, highScoreText, new Vector2((viewportWidth - highScoreWidth) / 2, viewportHeight / 2 + 270), highScoreColor);
        }

    }
}
