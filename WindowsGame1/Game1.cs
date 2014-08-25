namespace Star
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Net;
    using Microsoft.Xna.Framework.Storage;
    using Star.Util;
    using Star.Util.Log;
    using Star.Util.Debug;


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        private Color backgroundColour = Color.Black;

        private const int initialRocks = 3;
        
        private Texture2D rockTexture;
        private Texture2D starTexture;
        private Texture2D hearthTexture;
        private Texture2D background;

        private Sprite hearth;
        private Sprite star;
        private List<Sprite> rocks = new List<Sprite>();

        private SpriteFont scoreFont;
        private int score;

        private readonly Random rand = new Random();
        private CornerStack cornerStack;
        private readonly ILogger logger = ILoggerFactory.getLogger(typeof(Game1));
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        private Cue soundLoop;

        private int lastTickCount;

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = System.TimeSpan.FromMilliseconds(100);   
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.audioEngine = new AudioEngine("Content\\Audio\\MyGameAudio.xgs");
            this.waveBank = new WaveBank(this.audioEngine, "Content\\Audio\\Wave Bank.xwb");
            this.soundBank = new SoundBank(this.audioEngine, "Content\\Audio\\Sound Bank.xsb");
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.soundLoop = this.soundBank.GetCue("125bpm-Minimal--HHats-Snare");

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            
            this.graphics.PreferredBackBufferWidth = 500;
            this.graphics.PreferredBackBufferHeight = 500;
            this.graphics.ApplyChanges();

            this.cornerStack = new CornerStack(new Rectangle(0, 0, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight));
            
            this.background = Content.Load<Texture2D>("gcenter");
            this.starTexture = Content.Load<Texture2D>("star");
            this.hearthTexture = Content.Load<Texture2D>("hearth");
            this.rockTexture = Content.Load<Texture2D>("rock");

            this.scoreFont = Content.Load<SpriteFont>("starfont");

            Init();
        }

        private void SetNextRandomPosition(Sprite aSprite)
        {            
            do
            {
                aSprite.Position = new Vector2(this.rand.Next((int) this.graphics.PreferredBackBufferWidth - (int) aSprite.Size.X), this.rand.Next(this.graphics.PreferredBackBufferHeight - (int) aSprite.Size.Y));
            } while (this.star.CollidesWith(aSprite));            
        }
        

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.rockTexture.Dispose();
            this.starTexture.Dispose();
            this.hearthTexture.Dispose();

            this.soundBank.Dispose();
            this.waveBank.Dispose();
            this.audioEngine.Dispose();

            this.background.Dispose();       
            this.spriteBatch.Dispose();            
            this.scoreFont = null;
        }

        private void Init()
        {
            logger.Debug("Initializing game...");
            this.lastTickCount = System.Environment.TickCount;

            Queue<Vector2> corners = this.cornerStack.GetRandomCornerStack();

            this.star = new Sprite(this.starTexture, Vector2.Zero, new Vector2(35f, 35f), new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight));
            this.star.Position = corners.Dequeue();
            logger.Debug("...setting star to corner: {0}", star.Position);
            this.star.Velocity = Vector2.Zero;            

            this.hearth = new Sprite(this.hearthTexture, new Vector2(this.graphics.PreferredBackBufferWidth / 2, this.graphics.PreferredBackBufferHeight / 2), new Vector2(23f, 22f), new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight));

            this.rocks.Clear();
            for (int i = 0; i < initialRocks; i++)
            {
                Sprite newRock = createRock(corners.Dequeue());
               
                logger.Debug("...setting rock to corner: {0}", newRock.Position);

                this.rocks.Add(newRock);
            }

            this.score = 0;

            logger.Debug("...finished initializing.");

            if (this.soundLoop.IsPaused)
            {
                this.soundLoop.Resume();
            }
            else
            {
                this.soundLoop.Play();
            }            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {            
            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }            

            if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                || Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                
                foreach (Sprite rock in this.rocks)
                {
                    rock.Velocity *= -1;
                    rock.RandomMovement();
                }
            }            
            
            this.star.Move();

            foreach (Sprite rock in this.rocks)
            {
                rock.RandomMovement();

                if (rock.CollidesWith(this.hearth))
                {
                    logger.Debug("Rock {0} collides with hearth", rock.GetHashCode());
                    rock.Velocity *= -1;                 
                }

                if (rock.CollidesWith(this.star))
                {
                    logger.Debug("Rock {0} collides with star", rock.GetHashCode());
                    this.soundLoop.Pause();
                    StartVibrating();
                    System.Threading.Thread.Sleep(1000);
                    StopVibrating();
                    Init();
                    return;
                }

                foreach (Sprite otherrock in this.rocks)
                {
                    if (rock != otherrock && rock.CollidesWith(otherrock))
                    {
                        this.soundBank.PlayCue("FOLEY TECHNOLOGY SPRING DVD PLAYER SPRING VIBRATE LONG 01");
                        logger.Debug("Rock {0} collides with rock {1}", rock.GetHashCode(), otherrock.GetHashCode());
                        rock.Velocity *= -1;                                                                       
                        otherrock.Velocity *= -1;
                        otherrock.RandomMovement();
                        rock.RandomMovement();
                    }                    
                }
            }


            if (this.star.CollidesWith(this.hearth))
            {
                logger.Debug("Star collides with hearth...");

                this.soundBank.PlayCue("digital_feedback");

                StartVibrating();
                this.score++;

                if (0 == this.score % 5)
                {
                    this.soundBank.PlayCue("hyperspace_activate");

                    Sprite newRock = createRock(this.cornerStack.GetCorner(CornerStack.POSITION.LOWER_RIGHT));                    
                    this.rocks.Add(newRock);
                    logger.Debug("...must add new rock at {0}", newRock.Position);
                }

                SetNextRandomPosition(this.hearth);
            }
            else
            {
                StopVibrating();
            }

            if (System.Environment.TickCount - this.lastTickCount > 10000)
            {
                this.soundBank.PlayCue("hyperspace_activate");

                Sprite newRock = createRock(this.cornerStack.GetCorner(CornerStack.POSITION.LOWER_RIGHT));
                this.rocks.Add(newRock);
                logger.Debug("...must add new rock at {0}", newRock.Position);

                this.lastTickCount = System.Environment.TickCount;
            }

            base.Update(gameTime);
        }

        private Sprite createRock(Vector2 position)
        {
            Assert.FailIfNull(position, "position");
            return new Sprite(this.rockTexture, 
                                        position, 
                                        new Vector2(30f, 30f), 
                                        new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight));            
        }


        private void StopVibrating()
        {
            this.backgroundColour = Color.Black;
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            }
        }

        private void StartVibrating()
        {
            this.backgroundColour = Color.Red;
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                GamePad.SetVibration(PlayerIndex.One, 0.5f, 0.5f);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(this.backgroundColour);

            this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            this.spriteBatch.Draw(this.background, Vector2.Zero, Color.White);

            this.spriteBatch.DrawString(this.scoreFont, "Herzchen: " + this.score, new Vector2(15, 15), Color.Black);
            this.spriteBatch.DrawString(this.scoreFont, "Herzchen: " + this.score, new Vector2(16, 16), Color.White);            

            this.spriteBatch.Draw(this.star.Texture, this.star.Position, Color.White);
            this.spriteBatch.Draw(this.hearth.Texture, this.hearth.Position, Color.White);

            foreach (Sprite rock in this.rocks)
            {
                this.spriteBatch.Draw(this.rockTexture, rock.Position, Color.White);
            }
            
            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
