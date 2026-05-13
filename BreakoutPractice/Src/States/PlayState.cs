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

namespace BreakoutPractice.Src.States
{
    public class PlayState : State
    {
        private Paddle _paddle;
        private List<Ball> _balls;
        private SoundEffect _pauseSound;
        private SoundEffect _paddleHit;
        private SoundEffect _hurtSound;
        private SoundEffect _victorySound;
        private SoundEffect _wallHit;
        private SpriteFont _largeFont;
        private bool _paused;
        private List<Brick> _bricks;
        private int _health;
        private int _score;
        private int _level;
        private Hud _hud;
        private Random _random;
        private HighScoreList _highScores;
        private Sprite _extraBallSprite;
        private ExtraBallPowerup _extraBallPowerup;
        private List<Sprite> _ballSprites;

        public PlayState(StateMachine stateMachine, Dictionary<string,SoundEffect> gameSounds, SpriteFont largeFont, Random random, Hud hud, Sprite extraBallSprite, List<Sprite> ballSprites) : base(stateMachine)
        {
            _pauseSound = gameSounds["pause"];
            _largeFont = largeFont;
            _paddleHit = gameSounds["paddle_hit"];
            _hurtSound = gameSounds["hurt"];
            _victorySound = gameSounds["victory"];
            _wallHit = gameSounds["wall_hit"];
            _hud = hud;
            _random = random;
            _extraBallSprite = extraBallSprite;
        }

        public override void Enter(params object[] args)
        {
            _paddle = (Paddle)args[0];
            _bricks = (List<Brick>)args[1];
            _health = (int)args[2];
            _score = (int)args[3];
            _balls = (List<Ball>)args[4];
            _level = (int)args[5];
            _highScores = (HighScoreList)args[6];

            int ballDx = _random.Next(-200, 201);
            int ballDy = _random.Next(-60, -51);

            _balls[0].Velocity = new Vector2(ballDx, ballDy);
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

            foreach (var ball in _balls)
            {
                ball.Update(gameTime);
            }

            _hud.Update(_health, _score);

            if (_extraBallPowerup != null)
            {
                _extraBallPowerup.Update(gameTime);
                if (_extraBallPowerup.Position.Y > Constants.VIRTUAL_HEIGHT + _extraBallPowerup.Sprite.Height)
                {
                    _extraBallPowerup = null;
                }

                if (_extraBallPowerup.Collides(new Rectangle((int)_paddle.X, (int)_paddle.Y, _paddle.Width, _paddle.Height)))
                {
                    int skin1 = _random.Next(7);
                    int skin2 = _random.Next(7);
                                     
                    _balls.Add(new Ball(skin1, _wallHit, _ballSprites));
                    _balls.Add(new Ball(skin2, _wallHit, _ballSprites));
                }
            }

            foreach (var ball in _balls)
            {

                if (ball.Collides(new Rectangle((int)_paddle.X, (int)_paddle.Y, _paddle.Width, _paddle.Height)))
                {
                    ball.Y = _paddle.Y - 8;
                    ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);

                    bool paddleMovingLeft = _paddle.Velocity.X < 0;
                    bool paddleMovingRight = _paddle.Velocity.X > 0;
                    float paddleCenter = _paddle.X + _paddle.Width / 2;

                    int startingBounceDx = 50;
                    int bounceAngleMultiplier = 8;

                    // If ball hits the paddle on left side while it's moving left.
                    if (ball.X < paddleCenter && paddleMovingLeft)
                    {
                        float ballOffset = paddleCenter - ball.X;
                        ball.Velocity = new Vector2(-startingBounceDx - bounceAngleMultiplier * ballOffset, ball.Velocity.Y);
                    }
                    // If ball hits the paddle on right side while it's moving right.
                    else if (ball.X > paddleCenter && paddleMovingRight)
                    {
                        float ballOffset = ball.X - paddleCenter;
                        ball.Velocity = new Vector2(startingBounceDx + bounceAngleMultiplier * ballOffset, ball.Velocity.Y);
                    }

                    _paddleHit.Play();
                }

                foreach (Brick brick in _bricks)
                {
                    if (brick.Active && ball.Collides(new Rectangle((int)brick.X, (int)brick.Y, brick.Width, brick.Height)))
                    {
                        _score += (brick.Tier * 200 + brick.Color * 25);

                        brick.BrickHit();

                        if (CheckVictory())
                        {
                            _victorySound.Play();
                            _stateMachine.ChangeState("victory", _level, _paddle, _health, _score, ball, _highScores);
                        }

                        int ballRadius = (int)(ball.Width / 2f);
                        float brickWidth = brick.Width;
                        float brickHeight = brick.Height;
                        Vector2 brickCenter = new Vector2(brick.X + brickWidth / 2, brick.Y + brickHeight / 2);
                        Vector2 ballCenter = new Vector2(ball.X + ballRadius, ball.Y + ballRadius);
                        Vector2 collisionOffset = brickCenter - ballCenter;

                        // Depth
                        Vector2 collisionDepth = new Vector2(brickWidth / 2 + ballRadius - Math.Abs(collisionOffset.X),
                                                             brickHeight / 2 + ballRadius - Math.Abs(collisionOffset.Y));
                        if (collisionDepth.X < collisionDepth.Y)
                        {
                            ball.Velocity = new Vector2(-ball.Velocity.X, ball.Velocity.Y);
                            if (collisionOffset.X > 0)
                            {
                                ball.X -= collisionDepth.X;
                            }
                            else
                            {
                                ball.X += collisionDepth.X;
                            }
                        }
                        else
                        {
                            ball.Velocity = new Vector2(ball.Velocity.X, -ball.Velocity.Y);
                            if (collisionOffset.Y > 0)
                            {
                                ball.Y -= collisionDepth.Y;
                            }
                            else
                            {
                                ball.Y += collisionDepth.Y;
                            }
                        }

                        ball.Velocity = new Vector2(ball.Velocity.X, (float)(ball.Velocity.Y * 1.02));

                        if (_extraBallPowerup == null && _balls.Count < 2)
                        {
                            if (_random.Next(4) > 2)
                            {
                                _extraBallPowerup = new ExtraBallPowerup(_extraBallSprite, new Vector2(brick.X, brick.Y));
                            }
                        }
                        break;
                    }
                }


                if (ball.Y >= Constants.VIRTUAL_HEIGHT)
                {
                    ball.Active = false;
                }

                _balls.RemoveAll(b => b.Active == false);

                if (_balls.Count == 0)
                {
                    _health -= 1;
                    _hurtSound.Play();

                    if (_health <= 0)
                    {
                        _stateMachine.ChangeState("game_over", _score, _highScores);
                    }
                    else
                    {
                        _stateMachine.ChangeState("serve", _paddle, _bricks, _health, _score, _level, _highScores);
                    }
                }
            }

            // Escape/Quit Logic is no longer global
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();
        }

        private bool CheckVictory()
        {
            if (_bricks.Any(b => b.Active))
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

            foreach (var ball in _balls)
            {
                ball.Draw(spriteBatch);
            }

            _hud.Draw(spriteBatch);

            if (_extraBallPowerup != null)
            {
                _extraBallPowerup.Draw(spriteBatch);
            }

            if (_paused)
            {
                string pauseText = "PAUSED";
                float pauseTextWidth = _largeFont.MeasureString(pauseText).X;
                spriteBatch.DrawString(_largeFont, pauseText, new Vector2(pauseTextWidth, Constants.VIRTUAL_HEIGHT - 16), Color.White);
            }
        }
    }
}
