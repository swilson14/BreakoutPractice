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
        private SoundEffect _confirmSound;
        private List<Brick> _bricks;
        private Paddle _paddle;
        private int _health;
        private int _score;
        private int _level;
        private LevelMaker _levelMaker;
        private HighScore[] _highScores;

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
            _bricks = _levelMaker.CreateMap(1);
        }

        public override void Enter(params object[] args)
        {
            _bricks = _levelMaker.CreateMap(1);
            _highScores = (HighScore[])args[0];
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
                    _stateMachine.ChangeState("serve", _paddle, _bricks, _health, _score, _level);
                }
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
