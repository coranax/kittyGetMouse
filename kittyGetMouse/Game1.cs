﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace kittyGetMouse
{
    public class Game1 : Game
    {
        enum GameState { TITLE, START, PLAY, WIN, LOSE };
        GameState gameState;
        //string gameMessageString;
        Texture2D messageBox;
        Rectangle messageRect;

        int gameScore;
        int level;
        int levelUpScore;
        int winningScore;

        string gameTimeString;
        float minutes;
        float seconds;
        float timeElapsed;
        float losingTime;
        string losingTimeString;

        bool mouseStatus; //mouse spawn parameters
        int deathTime;
        int enemyDeathTimer;

        Player kitty;
        Enemy mouse;

        Texture2D backGround;
        Texture2D foreGround;
        SpriteFont scoreText;

        public static float gameWidth; //easier variables
        public static float gameHeight;
        float defWidth;
        float defHeight;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public RenderTarget2D target; //scaling and window size
        float proportion;
        int scale;

        KeyboardState newState, oldState; //key pressed

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            scale = 3; //overall game scaling, change this or use +/- key in game

            defWidth = 160; //do not change this (default size)
            defHeight = 144;

            gameWidth = defWidth;
            gameHeight = defHeight;

            kitty = new Player();
            mouse = new Enemy();

            IsMouseVisible = true; //this is the mouse pointer, dummy

            gameState = GameState.TITLE;

            winningScore = 9; //win and lose variables
            losingTime = 1f; //minutes
            losingTimeString = losingTime + ":00"; //change this manually if losing time should be seconds

            base.Initialize();

            SetScale();

            gameWidth = _graphics.PreferredBackBufferWidth;
            gameHeight = _graphics.PreferredBackBufferHeight;

            ResetStuff();
        }

        public void ResetStuff() //default values
        {
            gameScore = 0;
            level = 1;
            levelUpScore = 3;
            mouseStatus = true;
            deathTime = 100;
            enemyDeathTimer = deathTime;

            timeElapsed = 0;
            gameTimeString = "0:00";
            minutes = 0;
            seconds = 0;

            kitty.Reset();
            mouse.Reset();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backGround = Content.Load<Texture2D>("woodTile");
            foreGround = Content.Load<Texture2D>("foreground");
            messageBox = Content.Load<Texture2D>("message");

            mouse.mouseTexture = Content.Load<Texture2D>("mouse");
            kitty.kittyTexture = Content.Load<Texture2D>("kitty");

            scoreText = Content.Load<SpriteFont>("gamefont");

            target = new RenderTarget2D(GraphicsDevice, (int)defWidth, (int)defHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            newState = Keyboard.GetState(); //for KeyPressed()

            if (gameState == GameState.TITLE)
            {
                messageRect = new Rectangle(160, 144, 160, 144);
                if (KeyPressed(Keys.Enter))
                    gameState = GameState.START;
            }

            if (gameState == GameState.START)
            {
                messageRect = new Rectangle(0, 0, 160, 144);
                if (KeyPressed(Keys.Enter))
                    gameState = GameState.PLAY;
            }

            if (gameState == GameState.PLAY)
            {
                kitty.KittyMove(gameTime);
                mouse.MouseMove(gameTime);
                TotalTime(gameTime);

                //scoring
                if (Keyboard.GetState().IsKeyDown(Keys.Up) ||
                    Keyboard.GetState().IsKeyDown(Keys.Down) ||
                    Keyboard.GetState().IsKeyDown(Keys.Left) ||
                    Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    //movement key is pressed
                    if ((Math.Abs(mouse.mousePosition.X - kitty.kittyPosition.X) < 10) && (Math.Abs(mouse.mousePosition.Y - kitty.kittyPosition.Y) < 10))
                    {
                        //player and enemy sprite origins are sufficiently close together
                        if (mouseStatus == true)
                        {
                            //enemy mouse is visible
                            gameScore++;
                            mouseStatus = false; //stop drawing mouse (it will keep moving) and start countdown enemyDeathTimer
                        }
                    }
                }

                if (mouseStatus == false && enemyDeathTimer > 0) //countdown to "respawn"
                {
                    enemyDeathTimer--;
                }
                else if (enemyDeathTimer == 0)
                {
                    enemyDeathTimer = deathTime;
                    mouseStatus = true;
                }

                //win condition
                if ((gameScore == levelUpScore) && (gameScore < winningScore))
                {
                    level++;
                    mouse.mouseSpeed *= 1.5f;
                    levelUpScore += levelUpScore;
                }

                if (gameScore == winningScore)
                {
                    gameState = GameState.WIN;
                }

                //lose condition
                if (minutes >= losingTime)
                {
                    gameState = GameState.LOSE;
                }
            }

            Trace.WriteLine("losing time " + losingTime + "  and  minutes " + minutes);

            if (gameState == GameState.WIN)
            {
                messageRect = new Rectangle(160, 0, 160, 144);
                if (KeyPressed(Keys.Enter))
                    gameState = GameState.PLAY;
                ResetStuff();
            }

            if (gameState == GameState.LOSE)
            {
                messageRect = new Rectangle(0, 144, 160, 144);
                if (KeyPressed(Keys.Enter))
                    gameState = GameState.PLAY;
                ResetStuff();
            }

            if (KeyPressed(Keys.OemPlus) && scale < 7) //increase scale
            {
                scale++;
                SetScale();
            }

            if (KeyPressed(Keys.OemMinus) && scale > 1) //decrease scale
            {
                scale--;
                SetScale();
            }

            oldState = newState;  //for KeyPressed()
            base.Update(gameTime);
        }

        public bool KeyPressed(Keys key) //logs only one press of a key at a time
        {
            if (newState.IsKeyUp(key) && oldState.IsKeyDown(key))
            {
                oldState = newState;
                return true;
            }
            return false;
        }

        public void SetScale() //change game size
        {
            _graphics.PreferredBackBufferWidth = (int)defWidth * scale;
            _graphics.PreferredBackBufferHeight = (int)defHeight * scale;
            _graphics.ApplyChanges();
        }

        public void TotalTime(GameTime gameTime) //timer
        {
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            minutes = (int)timeElapsed / 60;
            seconds = (int)timeElapsed % 60;

            gameTimeString = minutes + ":" + seconds.ToString("00");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            proportion = 1f / (defHeight / _graphics.GraphicsDevice.Viewport.Height); //keep game elements in proportion
            GraphicsDevice.SetRenderTarget(target);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

            Color textColor = new Color(15, 56, 15);

            for (int i = 0; i <= gameWidth; i += 32) //background
            {
                for (int j = 0; j <= gameHeight; j += 32)
                {
                    _spriteBatch.Draw(
                        backGround,
                        new Vector2(i, j),
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1.0f,
                        SpriteEffects.None,
                        0f);
                }
            }

            if (mouseStatus == true) //enemy mouse
            {
                mouse.Draw(_spriteBatch);
            }

            kitty.Draw(_spriteBatch); //player kitty

            _spriteBatch.Draw( //foreground
                foreGround,
                Vector2.Zero,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0f);

            if (gameState == GameState.PLAY) //game info
            {
                _spriteBatch.DrawString(scoreText, gameScore.ToString(), new Vector2(40, 8), textColor); //score
                _spriteBatch.DrawString(scoreText, level.ToString(), new Vector2(78, 14), textColor); //level
                _spriteBatch.DrawString(scoreText, gameTimeString, new Vector2(133, 8), textColor); //time
            }

            if (gameState != GameState.PLAY) //message box
            {
                _spriteBatch.Draw(
                    messageBox,
                    new Vector2(defWidth / 2, defHeight / 2),
                    messageRect,
                    Color.White,
                    0f,
                    new Vector2(messageRect.Width / 2, messageRect.Height / 2),
                    1.0f,
                    SpriteEffects.None,
                    0f);
            }
            
            if (gameState == GameState.START) //text winning score and losing time
            {
                _spriteBatch.DrawString(scoreText, winningScore.ToString(), new Vector2(81, 72), textColor, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(scoreText, losingTimeString, new Vector2(77, 88), textColor, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            }

            /*Vector2 size = scoreText.MeasureString(gameMessageString); //might reuse this later
            
            if (gameState != GameState.PLAY) //start/win/lose msg
                _spriteBatch.DrawString(
                    scoreText, 
                    gameMessageString, 
                    new Vector2(gameWidth/2, gameHeight/2), 
                    Color.White,
                    0f,
                    size/2,
                    2,
                    SpriteEffects.None,
                    0f);*/

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null); //more scaling and proportion stuff
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            _spriteBatch.Draw(target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, proportion, SpriteEffects.None, 0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
} //ggez