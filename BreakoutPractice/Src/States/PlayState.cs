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
        private Ball _ball;
        private SoundEffect _pauseSound;
        private SoundEffect _paddleHit;
        private SoundEffect _hurtSound;
        private SoundEffect _victorySound;
        private SpriteFont _largeFont;
        private bool _paused;
        private List<Brick> _bricks;
        private int _health;
        private int _score;
        private int _level;
        private Hud _hud;
        private Random _random;

        public PlayState(StateMachine stateMachine, Dictionary<string,SoundEffect> gameSounds, SpriteFont largeFont, Random random, Hud hud) : base(stateMachine)
        {
            _pauseSound = gameSounds["pause"];
            _largeFont = largeFont;
            _paddleHit = gameSounds["paddle_hit"];
            _hurtSound = gameSounds["hurt"];
            _victorySound = gameSounds["victory"];
            _hud = hud;
            _random = random;
        }

        public override void Enter(params object[] args)
        {
            _paddle = (Paddle)args[0];
            _bricks = (List<Brick>)args[1];
            _health = (int)args[2];
            _score = (int)args[3];
            _ball = (Ball)args[4];
            _level = (int)args[5];

            int ballDx = _random.Next(-200, 201);
            int ballDy = _random.Next(-60, -51);
            _ball.Velocity = new Vector2(ballDx, ballDy);
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
            _ball.Update(gameTime);
            _hud.Update(_health, _score);

            if (_ball.Collides(new Rectangle((int)_paddle.X, (int)_paddle.Y, _paddle.Width, _paddle.Height)))
            {
                _ball.Y = _paddle.Y - 8;
                _ball.Velocity = new Vector2(_ball.Velocity.X, -_ball.Velocity.Y);

                bool paddleMovingLeft = _paddle.Velocity.X < 0;
                bool paddleMovingRight = _paddle.Velocity.X > 0;
                float paddleCenter = _paddle.X + _paddle.Width / 2;

                int startingBounceDx = 50;
                int bounceAngleMultiplier = 8;

                // If ball hits the paddle on left side while it's moving left.
                if (_ball.X < paddleCenter && paddleMovingLeft)
                {
                    float ballOffset = paddleCenter - _ball.X;
                    _ball.Velocity = new Vector2(-startingBounceDx - bounceAngleMultiplier * ballOffset, _ball.Velocity.Y);
                }
                // If ball hits the paddle on right side while it's moving right.
                else if (_ball.X > paddleCenter && paddleMovingRight)
                {
                    float ballOffset = _ball.X - paddleCenter;
                    _ball.Velocity = new Vector2(startingBounceDx + bounceAngleMultiplier * ballOffset,_ball.Velocity.Y);
                }

                _paddleHit.Play();
            }

            foreach (Brick brick in _bricks)
            {
                if (brick.Active && _ball.Collides(new Rectangle((int)brick.X, (int)brick.Y, brick.Width, brick.Height)))
                {

                    _score += (brick.Tier * 200 + brick.Color * 25);

                    brick.BrickHit();

                    if (CheckVictory())
                    {
                        _victorySound.Play();
                        _stateMachine.ChangeState("victory", _paddle, _health, _score, _ball);
                    }



                    int ballRadius = (int)(_ball.Width / 2f);
                    float brickWidth = brick.Width;
                    float brickHeight = brick.Height;
                    Vector2 brickCenter = new Vector2(brick.X + brickWidth / 2, brick.Y + brickHeight / 2);
                    Vector2 ballCenter = new Vector2(_ball.X + ballRadius, _ball.Y + ballRadius);
                    Vector2 collisionOffset = brickCenter - ballCenter;

                    // Depth
                    Vector2 collisionDepth = new Vector2(brickWidth / 2 + ballRadius - Math.Abs(collisionOffset.X),
                                                         brickHeight / 2 + ballRadius - Math.Abs(collisionOffset.Y));
                    if (collisionDepth.X < collisionDepth.Y)
                    {
                        _ball.Velocity = new Vector2(-_ball.Velocity.X, _ball.Velocity.Y);
                        if (collisionOffset.X > 0)
                        {
                            _ball.X -= collisionDepth.X;
                        }
                        else 
                        {
                            _ball.X += collisionDepth.X;
                        }
                    }
                    else
                    {
                        _ball.Velocity = new Vector2(_ball.Velocity.X, -_ball.Velocity.Y);
                        if (collisionOffset.Y > 0)
                        {
                            _ball.Y -= collisionDepth.Y;
                        }
                        else
                        {
                            _ball.Y += collisionDepth.Y;
                        }
                    }
                    _ball.Velocity = new Vector2(_ball.Velocity.X, (float)(_ball.Velocity.Y * 1.02));
                    break;
                }
            }

            if (_ball.Y >= Constants.VIRTUAL_HEIGHT)
            {
                _health -= 1;
                _hurtSound.Play();

                if (_health <= 0)
                {
                    _stateMachine.ChangeState("GameOver",_score);
                }
                else 
                {
                    _stateMachine.ChangeState("serve", _paddle, _bricks, _health, _score);
                }
            }

            // Escape/Quit Logic is no longer global
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();
        }

        private bool CheckVictory()
        {
            if (_bricks.Select(b => b.Active).Any())
            {
                return false;
            }

            return true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Brick brick in _bricks)
            {
                brick.Draw(spriteBatch);
            }

            _paddle.Draw(spriteBatch);
            _ball.Draw(spriteBatch);
            _hud.Draw(spriteBatch);

            if (_paused)
            {
                string pauseText = "PAUSED";
                float pauseTextWidth = _largeFont.MeasureString(pauseText).X;
                spriteBatch.DrawString(_largeFont, pauseText, new Vector2(pauseTextWidth, Constants.VIRTUAL_HEIGHT - 16), Color.White);
            }
        }
    }
}
