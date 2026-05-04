using BreakoutPractice.Input;
using BreakoutPractice.Src.Graphics;
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
    public class ServeState : State
    {
        private Paddle _paddle;
        private List<Brick> _bricks;
        private int _health;
        private int _score;
        private Ball _ball;
        private Random _random;
        private List<Sprite> _ballSprites;
        private SoundEffect _wallHit;
        private Hud _hud;
        private int _level;
        private SpriteFont _largeFont;
        private SpriteFont _mediumFont;

        public ServeState(StateMachine stateMachine, List<Sprite> ballSprites, SoundEffect wallHit, Random random, Hud hud, Dictionary<string,SpriteFont> fonts) : base(stateMachine)
        {
            _random = random;
            _ballSprites = ballSprites;
            _wallHit = wallHit;
            _hud = hud;
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];
        }

        public override void Enter(params object[] args)
        {
            _paddle = (Paddle)args[0];
            _bricks = (List<Brick>)args[1];
            _health = (int)args[2];
            _score = (int)args[3];
            _level = (int)args[4];

            int skin = _random.Next(7);
            _ball = new Ball(skin, _wallHit, _ballSprites);
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            _paddle.Update(gameTime, keyboard);
            _hud.Update(_health, _score);
            // ball follows the player
            _ball.X = _paddle.X + (_paddle.Width / 2);
            _ball.Y = _paddle.Y - 8;

            if (keyboard.WasKeyJustPressed(Keys.Enter))
            {
                _stateMachine.ChangeState("play", _paddle, _bricks, _health, _score, _ball, _level);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _paddle.Draw(spriteBatch);
            _ball.Draw(spriteBatch);

            foreach (Brick brick in _bricks)
            {
                brick.Draw(spriteBatch);
            }

            _hud.Draw(spriteBatch);

            string completeText = $"Level {_level}";
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