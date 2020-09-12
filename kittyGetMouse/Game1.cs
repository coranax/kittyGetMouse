using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace kittyGetMouse
{
    public class Game1 : Game
    {
        enum GameState { START, PLAY, WIN, LOSE };
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
        int losingTime;

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

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true; //this is the mouse pointer, dummy
            _graphics.PreferredBackBufferWidth = 160; //change window size
            _graphics.PreferredBackBufferHeight = 144;
            _graphics.ApplyChanges();

            gameWidth = _graphics.PreferredBackBufferWidth;
            gameHeight = _graphics.PreferredBackBufferHeight;

            kitty = new Player();
            mouse = new Enemy();

            gameState = GameState.START;

            winningScore = 3; //9 testing fixme
            losingTime = 3; //1 testing fixme

            ResetStuff();

            base.Initialize();
        }        
        public void ResetStuff() //default values
        {
            gameScore = 0;
            level = 1;
            levelUpScore = 1; //3 testing fixme
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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (gameState == GameState.START)
            {
                messageRect = new Rectangle(0, 0, 140, 53);
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
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

                if (seconds > losingTime) //testing minute fixme
                {
                    gameState = GameState.LOSE;
                }
            }

            if (gameState == GameState.WIN)
            {
                messageRect = new Rectangle(140, 0, 140, 53);
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    gameState = GameState.PLAY;
                ResetStuff();
            }

            if (gameState == GameState.LOSE)
            {
                messageRect = new Rectangle(0, 53, 140, 53);
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    gameState = GameState.PLAY;
                ResetStuff();
            }            
            base.Update(gameTime);
        }

        public void TotalTime(GameTime gameTime)
        {
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            minutes = (int)timeElapsed / 60;
            seconds = (int)timeElapsed % 60;

            gameTimeString = minutes + ":" + seconds.ToString("00");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

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
                _spriteBatch.DrawString(scoreText, gameScore.ToString(), new Vector2(40, 8), Color.White); //score
                _spriteBatch.DrawString(scoreText, level.ToString(), new Vector2(78, 14), Color.White); //level
                _spriteBatch.DrawString(scoreText, gameTimeString, new Vector2(133, 8), Color.White); //time
            }

            if (gameState != GameState.PLAY) //message box
            {
                _spriteBatch.Draw(
                    messageBox,
                    new Vector2(gameWidth / 2, gameHeight / 2 + 25),
                    messageRect,
                    Color.White,
                    0f,
                    new Vector2(messageRect.Width / 2, messageRect.Height / 2),
                    1.0f,
                    SpriteEffects.None,
                    0f);
            }

            /*Vector2 size = scoreText.MeasureString(gameMessageString);
            
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

            base.Draw(gameTime);
        }
    }
}

