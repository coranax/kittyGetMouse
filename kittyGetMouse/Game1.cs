using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

//testing git... 1 2 3 

namespace kittyGetMouse
{
    public class Game1 : Game
    {
        public enum GameState { START, PLAY, WIN, LOSE };

        int gameScore; //todo initialize
        int winningScore;
        int level;
        int levelUpScore;

        //todo game time, losing time, etc

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

            GameState gameState = GameState.PLAY; //todo ??

            gameScore = 0;
            mouseStatus = true;
            deathTime = 100;
            enemyDeathTimer = deathTime;


            base.Initialize();
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
            } else if (enemyDeathTimer == 0)
            {
                enemyDeathTimer = deathTime;
                mouseStatus = true;
            }

            //win condition

            if (gameScore == levelUpScore) //todo but not winning score
            {
                //level++;
                //mouse.mouseSpeed++; //todo
            }


            base.Update(gameTime);
        }

        public void GameWin()
        {
            //display text todo
            ResetStuff();
        }

        public void GameLose()
        {
            //display text todo
            ResetStuff();
        }

        public void ResetStuff()
        {
            gameScore = 0;
            mouseStatus = true;
            deathTime = 100;
            enemyDeathTimer = deathTime;
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

            _spriteBatch.DrawString(scoreText, gameScore.ToString(), new Vector2(40, 8), Color.White); //font

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
