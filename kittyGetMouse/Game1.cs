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
        string gameMessageString;

        int gameScore;
        int level;
        int levelUpScore;
        int winningScore;

        string gameTimeString;
        int minutes;
        float seconds;
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
            _graphics.PreferredBackBufferWidth = 160; //change window size
            _graphics.PreferredBackBufferHeight = 144;
            _graphics.ApplyChanges();

            gameWidth = _graphics.PreferredBackBufferWidth;
            gameHeight = _graphics.PreferredBackBufferHeight;

            kitty = new Player();
            mouse = new Enemy();

            gameState = GameState.START;

            winningScore = 9;
            losingTime = 1;

            ResetStuff();

            base.Initialize();
        }        
        public void ResetStuff() //default values
        {
            gameScore = 0;
            level = 1;
            levelUpScore = 3;
            mouseStatus = true;
            deathTime = 100;
            enemyDeathTimer = deathTime;

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
                gameMessageString = "Press enter\r\nto Play!\r\nCatch the mice!";
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    gameState = GameState.PLAY;
            }

            if (gameState == GameState.PLAY)
            {
                kitty.KittyMove(gameTime);
                mouse.MouseMove(gameTime);

                //scoring            

                if (Keyboard.GetState().IsKeyDown(Keys.Up) ||
                    Keyboard.GetState().IsKeyDown(Keys.Down) ||
                    Keyboard.GetState().IsKeyDown(Keys.Left) ||
                    Keyboard.GetState().IsKeyDown(Keys.Right))
                { //only score when key is pressed
                    if ((Math.Abs(mouse.mousePosition.X - kitty.kittyPosition.X) < 10) && (Math.Abs(mouse.mousePosition.Y - kitty.kittyPosition.Y) < 10))
                    { //sprites origin are sufficiently close together
                        if (mouseStatus == true)
                        { //mouse must be visible to score
                            gameScore++;
                            mouseStatus = false; //stop drawing mouse (it will keep moving... i think)
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

                seconds += (float)gameTime.ElapsedGameTime.TotalSeconds; //convert to "totalTime" with function to convert to string todo

                if (seconds > 59)
                {
                    minutes++;
                    seconds = 0;
                }

                if (seconds > 10)
                {
                    gameTimeString = minutes + ":" + (int)seconds;
                } else
                {
                    gameTimeString = minutes + ":0" + (int)seconds;
                }

                if (minutes >= losingTime)
                {
                    gameState = GameState.LOSE;
                }
            }

            if (gameState == GameState.WIN)
            {
                gameMessageString = "You won!!\r\nPress enter\r\nto try again!";
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    gameState = GameState.PLAY;
                ResetStuff();
            }

            if (gameState == GameState.LOSE)
            {
                gameMessageString = "You lost!!\r\nPress enter\r\nto try again!";
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    gameState = GameState.PLAY;
                ResetStuff();
            }

            Trace.WriteLine(mouseStatus);
            base.Update(gameTime);
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

            if (mouseStatus == true)
            {
                _spriteBatch.Draw( //mouse
                    mouse.mouseTexture,
                    mouse.mousePosition,
                    mouse.mouseRect,
                    Color.White,
                    0f,
                    new Vector2(mouse.mouseRect.Width / 2, mouse.mouseRect.Height / 2),
                    2.0f,
                    SpriteEffects.None,
                    0f);
            }


            kitty.Draw(_spriteBatch);
            


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

            if (gameState == GameState.PLAY)
            {
                _spriteBatch.DrawString(scoreText, gameScore.ToString(), new Vector2(40, 8), Color.White); //score
                _spriteBatch.DrawString(scoreText, level.ToString(), new Vector2(78, 14), Color.White); //level
                _spriteBatch.DrawString(scoreText, gameTimeString, new Vector2(133, 8), Color.White); //time
            }

            Vector2 size = scoreText.MeasureString(gameMessageString);

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
                    0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
