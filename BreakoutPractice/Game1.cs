using BreakoutPractice.Input;
using BreakoutPractice.Src;
using BreakoutPractice.Src.Graphics;
using BreakoutPractice.Src.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace BreakoutPractice
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private BoxingViewportAdapter _viewportAdapter;
        private OrthographicCamera _camera;
        private StateMachine _stateMachine;
        private KeyboardInfo _keyboard;
        private FrameCounter _frameCounter;
        private LevelMaker _levelMaker;

        // Fonts
        private SpriteFont _smallFont;
        private SpriteFont _mediumFont;
        private SpriteFont _largeFont;

        // Graphics
        private Texture2D _background;
        private Texture2D _main;
        private Texture2D _arrows;
        private Texture2D _hearts;
        private Texture2D _particle;
        private TextureAtlas _atlas;
        private List<Sprite> _paddleSprites = new List<Sprite>();
        private List<Sprite> _ballSprites = new List<Sprite>();
        private List<Sprite> _brickSprites = new List<Sprite>();

        // Audio
        private SoundEffect _paddleHitSound;
        private SoundEffect _scoreSound;
        private SoundEffect _wallHitSound;
        private SoundEffect _confirmSound;
        private SoundEffect _selectSound;
        private SoundEffect _noSelectSound;
        private SoundEffect _brickHitSound1;
        private SoundEffect _brickHitSound2;
        private SoundEffect _hurtSound;
        private SoundEffect _victorySound;
        private SoundEffect _recoverSound;
        private SoundEffect _highScoreSound;
        private SoundEffect _pauseSound;
        private Song _music;
        private Paddle _paddle;
        private Ball _ball;
        private Random random;
        private Brick _brickTemplate;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Window Size
            _graphics.PreferredBackBufferWidth = Constants.WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = Constants.WINDOW_HEIGHT;
            _graphics.ApplyChanges();

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, Constants.VIRTUAL_WIDTH, Constants.VIRTUAL_HEIGHT);
            _camera = new OrthographicCamera(_viewportAdapter);

            _keyboard = new KeyboardInfo();
            _frameCounter = new FrameCounter();

            _paddle = new Paddle(_paddleSprites);
            
            random = new Random();
            int ballSkin = random.Next(7);

            _ball = new Ball(ballSkin, _wallHitSound, _ballSprites);

            _brickTemplate = new Brick(_brickHitSound2, 0, 0, _brickSprites);

            _levelMaker = new LevelMaker(random, _brickTemplate);

            _stateMachine = new StateMachine();

            Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>
            {
                {"paddle_hit", _paddleHitSound},
                {"score", _scoreSound},
                {"wall_hit", _wallHitSound},
                {"confirm", _confirmSound},
                {"select", _selectSound},
                {"no_select", _noSelectSound},
                {"brick_hit_1", _brickHitSound1},
                {"brick_hit_2", _brickHitSound2},
                {"hurt", _hurtSound},
                {"victory", _victorySound},
                {"recover", _recoverSound},
                {"high_score", _highScoreSound},
                {"pause", _pauseSound},
            };

    
            _stateMachine.States = new Dictionary<string, State>
            {
                { "start", new StartState(_stateMachine, soundEffects, _largeFont, _mediumFont)},
                { "play", new PlayState(_stateMachine, _paddle, soundEffects, _largeFont, _ball, random, _levelMaker)},
            };

            _stateMachine.ChangeState("start");
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Fonts
            _smallFont = Content.Load<SpriteFont>("Fonts/small");
            _mediumFont = Content.Load<SpriteFont>("Fonts/medium");
            _largeFont = Content.Load<SpriteFont>("Fonts/large");

            // Graphics
            _background = Content.Load<Texture2D>("Graphics/background");
            _main = Content.Load<Texture2D>("Graphics/breakout");
            _arrows = Content.Load<Texture2D>("Graphics/arrows");
            _hearts = Content.Load<Texture2D>("Graphics/hearts");
            _particle = Content.Load<Texture2D>("Graphics/particle");

            // TextureAtlas
            _atlas = new TextureAtlas(_main);
            PopulatePaddleSprites();
            PopulateBallSprites();
            PopulateBrickSprites();

            // Audio
            // Sound Effects
            _paddleHitSound = Content.Load<SoundEffect>("Sounds/paddle_hit");
            _scoreSound = Content.Load<SoundEffect>("Sounds/score");
            _wallHitSound = Content.Load<SoundEffect>("Sounds/wall_hit");
            _confirmSound = Content.Load<SoundEffect>("Sounds/confirm");
            _selectSound = Content.Load<SoundEffect>("Sounds/select");
            _noSelectSound = Content.Load<SoundEffect>("Sounds/no-select");
            _brickHitSound1 = Content.Load<SoundEffect>("Sounds/brick-hit-1");
            _brickHitSound2 = Content.Load<SoundEffect>("Sounds/brick-hit-2");
            _hurtSound = Content.Load<SoundEffect>("Sounds/hurt");
            _victorySound = Content.Load<SoundEffect>("Sounds/victory");
            _recoverSound = Content.Load<SoundEffect>("Sounds/recover");
            _highScoreSound = Content.Load<SoundEffect>("Sounds/high_score");
            _pauseSound = Content.Load<SoundEffect>("Sounds/pause");
            // Song
            _music = Content.Load<Song>("Sounds/music");
        }

        private void PopulateBrickSprites()
        {
            int x = 0;
            int y = 0;
            int count = 1;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    _atlas.AddRegion($"brick-{count}", x, y, 32, 16);
                    _brickSprites.Add(_atlas.CreateSprite($"brick-{count}"));
                    x += 32;
                    count++;
                }
                y+= 16;
            }

            for (int j = 0; j < 3; j++)
            {
                _atlas.AddRegion($"brick-{count}", x, y, 32, 16);
                _brickSprites.Add(_atlas.CreateSprite($"brick-{count}"));
                x += 32;
                count++;
            }
        }

        private void PopulateBallSprites()
        {
            int x = 96;
            int y = 48;
            int count = 1;
            for (int i = 0; i < 4; i++)
            {
                _atlas.AddRegion($"ball-{count}", x, y, 8, 8);
                _ballSprites.Add(_atlas.CreateSprite($"ball-{count}"));
                x += 8;
                count++;
            }

            x = 96;
            y = 56;

            for (int i = 0; i < 3; i++)
            {
                _atlas.AddRegion($"ball-{count}", x, y, 8, 8);
                _ballSprites.Add(_atlas.CreateSprite($"ball-{count}"));
                x += 8;
                count++;
            }
        }

        private void PopulatePaddleSprites()
        {
            int x = 0;
            int y = 64;
            int count = 1;
            // Paddle Sprites
            for (int i = 0; i < 4; i++)
            {
                _atlas.AddRegion($"smallestPaddle-{count}", x, y, 32, 16);
                _atlas.AddRegion($"mediumPaddle-{count}", x + 32, y, 64, 16);
                _atlas.AddRegion($"largePaddle-{count}", x + 96, y, 96, 16);
                _atlas.AddRegion($"hugePaddle-{count}", x, y + 16, 128, 16);

                _paddleSprites.Add(_atlas.CreateSprite($"smallestPaddle-{count}"));
                _paddleSprites.Add(_atlas.CreateSprite($"mediumPaddle-{count}"));
                _paddleSprites.Add(_atlas.CreateSprite($"largePaddle-{count}"));
                _paddleSprites.Add(_atlas.CreateSprite($"hugePaddle-{count}"));

                x = 0;
                y += 32;
                count++;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            _stateMachine.Update(gameTime, _keyboard);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Get the camera's transformation matrix
            Matrix transformMatrix = _camera.GetViewMatrix();

            int backgroundWidth = _background.Width;
            int backgroundHeight = _background.Height;

            // Calulate FPS
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            string fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            // Apply the transformation to the sprite batch
            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
            // Draw here.
            _spriteBatch.Draw(_background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero,
                new Vector2(Constants.VIRTUAL_WIDTH / (backgroundWidth - 1.0f), Constants.VIRTUAL_HEIGHT / (backgroundHeight - 1.0f)), // scale
                SpriteEffects.None, 0.0f);

            // Draw FPS
            _spriteBatch.DrawString(_smallFont, fps, new Vector2(5), Color.White);
            _stateMachine.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
