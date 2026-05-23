using BreakoutPractice.Input;
using BreakoutPractice.Src;
using BreakoutPractice.Src.Entities;
using BreakoutPractice.Src.Graphics;
using BreakoutPractice.Src.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;

using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Data;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;


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
        private Random _random;

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
        private TextureAtlas _heartsAtlas;
        private TextureAtlas _arrowAtlas;
        private List<Sprite> _paddleSprites = new List<Sprite>();
        private List<Sprite> _ballSprites = new List<Sprite>();
        private List<Sprite> _brickSprites = new List<Sprite>();
        private List<Sprite> _heartSprites = new List<Sprite>();
        private List<Sprite> _arrowSprites = new List<Sprite>();
        private ParticleEffect _particleEffect;
        private Sprite _extraBallSprite;
        private Sprite _lockedBrickSprite;
        private Sprite _keyPowerupSprite;

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

        // Game Objects
        private Paddle _paddle;
        private Brick _brickTemplate;
        private Hud _hud;

        // Game stats
        private int _score;
        private int _health;
        private HighScoreList _highScores;

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
            
            _random = new Random();

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

            Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>
            {
                { "small", _smallFont },
                { "medium", _mediumFont },
                { "large", _largeFont },
            };

            _brickTemplate = new Brick(soundEffects, 0, 0, _brickSprites, Constants.DEFAULT_BRICK_TIER, Constants.DEFAULT_BRICK_COLOR, _particleEffect, _lockedBrickSprite);

            _levelMaker = new LevelMaker(_random, _brickTemplate, _particleEffect, _lockedBrickSprite);

            _health = 3;
            _score = 0;

            _hud = new Hud(_health, _score, _heartSprites, _smallFont);

            _stateMachine = new StateMachine();
            _highScores = new HighScoreList();
            _highScores.HighScores = new HighScore[10];
            LoadHighScores();

            _stateMachine.States = new Dictionary<string, State>
            {
                { "start", new StartState(_stateMachine, soundEffects, fonts, _levelMaker, _paddle, _health, _score)},
                { "play", new PlayState(_stateMachine, soundEffects, _largeFont, _random, _hud, _extraBallSprite, _ballSprites,_keyPowerupSprite)},
                { "serve", new ServeState(_stateMachine, _ballSprites, _wallHitSound, _random, _hud, fonts)},
                { "game_over", new GameOverState(_stateMachine, fonts, soundEffects)},
                { "victory", new VictoryState(_stateMachine, _hud, _levelMaker, fonts) },
                { "high_scores", new HighScoreState(_stateMachine, fonts, soundEffects) },
                { "enter_high_score", new EnterHighScoreState(_stateMachine, fonts, soundEffects)},
                { "paddle_select", new PaddleSelectState(_stateMachine, soundEffects, fonts, _arrowSprites, _paddleSprites) }
            };

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_music);

            _stateMachine.ChangeState("start", _highScores);           
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

            CreateParticleEffects();

            // TextureAtlas
            _atlas = new TextureAtlas(_main);
            _heartsAtlas = new TextureAtlas(_hearts);
            _arrowAtlas = new TextureAtlas(_arrows);
            PopulatePaddleSprites();
            PopulateBallSprites();
            PopulateBrickSprites();
            PopulateHeartSprites();
            PopulateArrowSprites();
            PopulatePowerupSprites();


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

        private void PopulatePowerupSprites()
        {
            _atlas.AddRegion("extraBalls",128,192,16,16);
            _extraBallSprite = _atlas.CreateSprite("extraBalls");

            _atlas.AddRegion("key",144,192,16,16);
            _keyPowerupSprite = _atlas.CreateSprite("key");
        }

        private void PopulateArrowSprites()
        {
            int tileWidth = 24;
            int tileHeight = 24;

            float sheetWidth = _arrowAtlas.Texture.Width / tileWidth;
            float sheetHeight = _arrowAtlas.Texture.Height / tileHeight;

            int count = 1;
            for (int i = 0; i < sheetHeight; i++)
            {
                for (int j = 0; j < sheetWidth; j++)
                {
                    _arrowAtlas.AddRegion($"arrow-{count}", j * tileWidth, i * tileHeight, tileWidth, tileHeight);
                    _arrowSprites.Add(_arrowAtlas.CreateSprite($"arrow-{count}"));
                    count++;
                }
            }
        }

        private void CreateParticleEffects()
        {
            Vector2 viewportCenter = GraphicsDevice.Viewport.Bounds.Center.ToVector2();

            _particleEffect = new ParticleEffect("Explosion")
            {
                Position = viewportCenter,

                // Disable auto trigger - we'll trigger manually
                AutoTrigger = false,
            };

            ParticleEmitter emitter = new ParticleEmitter(2000)
            {
                Name = "Explosion Emitter",
                LifeSpan = 0.5f,
                TextureRegion = new MonoGame.Extended.Graphics.Texture2DRegion(_particle),

                // Change the profile to make particles explode outward
                Profile = Profile.Circle(15, CircleRadiation.Out),

                Parameters = new ParticleReleaseParameters
                {
                    Quantity = new ParticleInt32Parameter(64),

                    // Increase random speed to 100-200
                    Speed = new ParticleFloatParameter(-15f, 15f),

                    Color = new ParticleColorParameter(new Vector3(0.0f, 1.0f, 0.6f)),
                    Scale = new ParticleVector2Parameter(new Vector2(1.0f, 1.0f))
                }
            };

            emitter.Modifiers.Add(new LinearGravityModifier
            {
                // Change gravity to move down
                Direction = Vector2.UnitY,

                // Strength increased to 200
                Strength = 200f
            });

            emitter.Modifiers.Add(new AgeModifier
            {
                Interpolators =
        {
            new OpacityInterpolator
            {
                StartValue = 1.0f,
                EndValue = 0.0f
            }
        }
            });

            // Add drag modifier for air resistance
            emitter.Modifiers.Add(new DragModifier
            {
                Density = 0.5f,
                DragCoefficient = 0.3f
            });

            _particleEffect.Emitters.Add(emitter);
        }

        private void PopulateHeartSprites()
        {
            int tileWidth = 10;
            int tileHeight = 9;

            float sheetWidth = _heartsAtlas.Texture.Width / tileWidth;
            float sheetHeight = _heartsAtlas.Texture.Height / tileHeight;

            int count = 1;
            for (int i = 0; i < sheetHeight; i++)
            {
                for (int j = 0; j < sheetWidth; j++)
                {
                    _heartsAtlas.AddRegion($"heart-{count}", j * tileWidth, i * tileHeight, tileWidth, tileHeight);
                    _heartSprites.Add(_heartsAtlas.CreateSprite($"heart-{count}"));
                    count++;
                }
            }
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
                x = 0;
                y+= 16;
            }

            x = 0;

            for (int j = 0; j < 3; j++)
            {
                _atlas.AddRegion($"brick-{count}", x, y, 32, 16);
                _brickSprites.Add(_atlas.CreateSprite($"brick-{count}"));
                x += 32;
                count++;
            }

            // Locked brick
            x = 160;
            y = 48;

            _atlas.AddRegion($"brick-locked", x, y, 32, 16);
            _lockedBrickSprite = _atlas.CreateSprite("brick-locked");
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

            _particleEffect.Update(gameTime);

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
            string fps = string.Format("FPS: {0}", Math.Round(_frameCounter.AverageFramesPerSecond));

            // Apply the transformation to the sprite batch
            _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            // Draw Images.
            _spriteBatch.Draw(_background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero,
                new Vector2(Constants.VIRTUAL_WIDTH / (backgroundWidth - 1.0f), Constants.VIRTUAL_HEIGHT / (backgroundHeight - 1.0f)), // scale
                SpriteEffects.None, 0.0f);

            _spriteBatch.Draw(_particleEffect);
            _stateMachine.Draw(_spriteBatch);
            _spriteBatch.End();

            // Draw Text
            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
            _spriteBatch.DrawString(_smallFont, fps, new Vector2(15), Color.White);
            _stateMachine.DrawText(_spriteBatch, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void LoadHighScores()
        {
            string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gameFolder = Path.Combine(dataPath, "breakout");
            string dataFile = Path.Combine(gameFolder, "breakout.dat");
            Directory.CreateDirectory(gameFolder);

            if (!File.Exists(dataFile))
            {
                string scoreText = "";

                for (int i = 10; i >= 1; i--) 
                {
                    scoreText +=  "AAA\n";
                    scoreText += $"{i * 1000}\n";
                }

                File.WriteAllText(dataFile, scoreText);
            }

            bool name = true;
            int count = 0;

            for (int i = 0; i < _highScores.HighScores.Length; i++) 
            {
                _highScores.HighScores[i] = new HighScore();
            }
           
            foreach (var line in File.ReadAllLines(dataFile))
            {
                if (name)
                {
                    _highScores.HighScores[count].Name = line.Substring(0, 3);
                }
                else
                {
                    _highScores.HighScores[count].Score = int.Parse(line);
                    count++;
                }
                name = !name;
            }
        }
    }
}
