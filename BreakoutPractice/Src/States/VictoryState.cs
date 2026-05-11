using BreakoutPractice.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakoutPractice.Src.States
{
    public class VictoryState : State
    {
        private int _level;
        private int _score;
        private Paddle _paddle;
        private int _health;
        private Ball _ball;
        private Hud _hud;
        private LevelMaker _levelMaker;
        private SpriteFont _largeFont;
        private SpriteFont _mediumFont;
        private HighScoreList _highScores;

        public VictoryState(StateMachine stateMachine, Hud hud, LevelMaker levelMaker, Dictionary<string, SpriteFont> fonts) : base(stateMachine)
        {
            _hud = hud;
            _levelMaker = levelMaker;
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];
        }

        public override void Enter(params object[] args)
        {
            _level = (int)args[0];     
            _paddle = (Paddle)args[1];
            _health = (int)args[2];
            _score = (int)args[3];
            _ball = (Ball)args[4];
            _highScores = (HighScoreList)args[5];
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            _paddle.Update(gameTime, keyboard);
            _hud.Update(_health, _score);
            _ball.X = _paddle.X + (_paddle.Width / 2);
            _ball.Y = _paddle.Y - 8;

            if (keyboard.WasKeyJustPressed(Keys.Enter))
            {
                _stateMachine.ChangeState("serve", _paddle, _levelMaker.CreateMap(_level + 1), _health, _score, _level + 1, _highScores);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _paddle.Draw(spriteBatch);
            _ball.Draw(spriteBatch);
            _hud.Draw(spriteBatch);

            string completeText = $"Level {_level} complete!";
            float completeTextWidth = _largeFont.MeasureString(completeText).X;

            spriteBatch.DrawString(_largeFont, completeText, 
                new Vector2((Constants.VIRTUAL_WIDTH - completeTextWidth) / 2, Constants.VIRTUAL_HEIGHT / 4),
                Color.White);

            string instructionsText = "Press Enter to serve!";
            float instructionsWidth = _mediumFont.MeasureString(instructionsText).X;

            spriteBatch.DrawString(_mediumFont, instructionsText,
                new Vector2((Constants.VIRTUAL_WIDTH - instructionsWidth) / 2, Constants.VIRTUAL_HEIGHT / 2),
                Color.White);
        }
    }
}
