using BreakoutPractice.Input;
using BreakoutPractice.Src.Entities;
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
using static System.Formats.Asn1.AsnWriter;

namespace BreakoutPractice.Src.States
{
    public class PaddleSelectState : State
    {
        private HighScoreList _highScores;
        private int _currentPaddle;
        private List<Sprite> _paddleSprites;

        // Sound effects
        private SoundEffect _noSelectSound;
        private SoundEffect _selectSound;
        private SoundEffect _confirmSound;

        // Fonts
        private SpriteFont _largeFont;
        private SpriteFont _mediumFont;
        private SpriteFont _smallFont;

        // Graphics
        private Sprite _leftArrowSprite;
        private Sprite _rightArrowSprite;

        // Just passing through.
        private Paddle _paddle;
        private List<Brick> _bricks;
        private int _health;
        private int _score;
        private int _level;

        public PaddleSelectState(StateMachine stateMachine, Dictionary<string,SoundEffect> soundEffects, Dictionary<string, SpriteFont> fonts,
            List<Sprite> arrowSprites, List<Sprite> paddleSprites) : base(stateMachine)
        {
            _currentPaddle = 1;

            // Sound Effects
            _noSelectSound = soundEffects["no_select"];
            _selectSound = soundEffects["select"];
            _confirmSound = soundEffects["confirm"];

            // Fonts
            _largeFont = fonts["large"];
            _mediumFont = fonts["medium"];
            _smallFont = fonts["small"];

            // Graphics
            _leftArrowSprite = arrowSprites[0];
            _rightArrowSprite = arrowSprites[1];
            _paddleSprites = paddleSprites;
        }

        public override void Enter(params object[] args)
        {
            _paddle = (Paddle)args[0];
            _bricks = (List<Brick>)args[1];
            _health = (int)args[2];
            _score = (int)args[3];
            _level = (int)args[4];
            _highScores = (HighScoreList)args[5];   
        }

        public override void Update(GameTime gameTime, KeyboardInfo keyboard)
        {
            if (keyboard.WasKeyJustPressed(Keys.Left)) 
            {
                if (_currentPaddle == 1)
                {
                    _noSelectSound.Play();
                }
                else
                {
                    _selectSound.Play();
                    _currentPaddle--;
                }
            }
            else if (keyboard.WasKeyJustPressed(Keys.Right))
            {
                if (_currentPaddle == 4)
                {
                    _noSelectSound.Play();
                }
                else 
                {
                    _selectSound.Play();
                    _currentPaddle++;
                }
            }

            if (keyboard.WasKeyJustPressed(Keys.Enter))
            {
                _confirmSound.Play();
                _paddle.ChangeSkin(_currentPaddle);
                _stateMachine.ChangeState("serve", _paddle, _bricks, _health, _score, _level, _highScores);
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw left arrow
            Color leftColor = Color.White;
            if (_currentPaddle == 1)
            {
                leftColor = new Color(40f / 255f, 40f / 255f, 40f / 255f, 128f / 255f);
            }
            _leftArrowSprite.Color = leftColor;
            _leftArrowSprite.Draw(spriteBatch, new Vector2(Constants.VIRTUAL_WIDTH / 4 - 24, Constants.VIRTUAL_HEIGHT - Constants.VIRTUAL_HEIGHT / 3));


            // Draw right arrow
            Color rightColor = Color.White;
            if (_currentPaddle == 4)
            {
                rightColor = new Color(40f / 255f, 40f / 255f, 40f / 255f, 128f / 255f);
            }
            _rightArrowSprite.Color = rightColor;
            _rightArrowSprite.Draw(spriteBatch, new Vector2(Constants.VIRTUAL_WIDTH - Constants.VIRTUAL_WIDTH / 4, Constants.VIRTUAL_HEIGHT - Constants.VIRTUAL_HEIGHT / 3));

            // Draw paddle
            int spriteIndex = (int)2 + 4 * (_currentPaddle - 1);
            spriteIndex = spriteIndex - 1;
            Sprite paddleSprite = _paddleSprites[spriteIndex];
            paddleSprite.Draw(spriteBatch, new Vector2(Constants.VIRTUAL_WIDTH / 2 - 32, Constants.VIRTUAL_HEIGHT - Constants.VIRTUAL_HEIGHT / 3));

        }

        public override void DrawText(SpriteBatch spriteBatch, int viewportWidth, int viewportHeight)
        {
            string selectPaddleText = "Select your paddle with left and right!";
            float selectPaddleWidth = _mediumFont.MeasureString(selectPaddleText).X;

            spriteBatch.DrawString(_mediumFont, selectPaddleText,
                new Vector2((viewportWidth - selectPaddleWidth) / 2, viewportHeight / 4),
                Color.White);

            string pressEnterText = "(Press Enter to continue!)";
            float pressEnterWidth = _smallFont.MeasureString(pressEnterText).X;

            spriteBatch.DrawString(_smallFont, pressEnterText,
                new Vector2((viewportWidth - pressEnterWidth) / 2, viewportHeight / 3),
                Color.White);
        }


    }
}
