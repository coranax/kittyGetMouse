using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace kittyGetMouse
{
    public class Game1 : Game
    {

        int gameScore;

        Texture2D kittyTexture; // kitty stuff
        Vector2 kittyPosition;
        float kittySpeed;
        Rectangle kittyRect;

        Texture2D mouseTexture; // mouse stuff
        Vector2 mousePosition;
        float mouseSpeed;
        Rectangle mouseRect;
        //bool mouseStatus; // true, mouse is alive

        Texture2D backGround; // background stuff

        Texture2D foreGround; // foreground stuff

        float gameWidth; // easier variables
        float gameHeight;

        Random random = new Random();
        float changeInPositionX;
        float changeInPositionY;
        float timeElapsed = 0.0f;

        SpriteFont scoreText;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            kittyRect = new Rectangle(0, 0, 12, 16); // for choosing which sprite from the set to use
            mouseRect = new Rectangle(0, 0, 8, 8);

        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 160; // change window size
            _graphics.PreferredBackBufferHeight = 144;
            _graphics.ApplyChanges();

            gameWidth = _graphics.PreferredBackBufferWidth; // easier name to work with
            gameHeight = _graphics.PreferredBackBufferHeight;

            kittyPosition = new Vector2(gameWidth / 2, gameHeight / 2);
            kittySpeed = 100f;

            mousePosition = new Vector2(20, 20);
            mouseSpeed = 95f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backGround = Content.Load<Texture2D>("woodTile");
            kittyTexture = Content.Load<Texture2D>("kitty");
            mouseTexture = Content.Load<Texture2D>("mouse");
            foreGround = Content.Load<Texture2D>("foreground");

            scoreText = Content.Load<SpriteFont>("gamefont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up))
            {
                kittyPosition.Y -= kittySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                kittyRect = new Rectangle(12, 0, kittyRect.Width, kittyRect.Height);
            }

            if (kstate.IsKeyDown(Keys.Down))
            {
                kittyPosition.Y += kittySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                kittyRect = new Rectangle(0, 0, kittyRect.Width, kittyRect.Height);
            }

            if (kstate.IsKeyDown(Keys.Left))
            {
                kittyPosition.X -= kittySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                kittyRect = new Rectangle(24, 0, kittyRect.Width, kittyRect.Height);
            }

            if (kstate.IsKeyDown(Keys.Right))
            {
                kittyPosition.X += kittySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                kittyRect = new Rectangle(36, 0, kittyRect.Width, kittyRect.Height);
            }

            if (kittyPosition.X > gameWidth - kittyRect.Width / 2)
                kittyPosition.X = gameWidth - kittyRect.Width / 2;
            else if (kittyPosition.X < kittyRect.Width / 2)
                kittyPosition.X = kittyRect.Width / 2;

            if (kittyPosition.Y > gameHeight - kittyRect.Height / 2)
                kittyPosition.Y = gameHeight - kittyRect.Height / 2;
            else if (kittyPosition.Y < kittyRect.Height / 2)
                kittyPosition.Y = kittyRect.Height / 2;


            if (mousePosition.X > gameWidth - mouseRect.Width / 2)
                mousePosition.X = gameWidth - mouseRect.Width / 2;
            else if (mousePosition.X < mouseRect.Width / 2)
                mousePosition.X = mouseRect.Width / 2;

            if (mousePosition.Y > gameHeight - mouseRect.Height / 2)
                mousePosition.Y = gameHeight - mouseRect.Height / 2;
            else if (mousePosition.Y < mouseRect.Height / 2)
                mousePosition.Y = mouseRect.Height / 2;

            // logic to make mouse move in a random direction.
            // mouse moves at set interval, maybe change to randomInterval?

            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (timeElapsed > 0.5f)
            {
                changeInPositionX = (float)random.Next(-1, 2);
                changeInPositionY = (float)random.Next(-1, 2);
                timeElapsed = 0;
            }

            mousePosition.X += mouseSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * changeInPositionX;
            mousePosition.Y += mouseSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * changeInPositionY;

            if (Math.Abs(mousePosition.X - kittyPosition.X) < 10)
            {
                if (Math.Abs(mousePosition.Y - kittyPosition.Y) < 10)
                {
                    gameScore++;
                    //mouseStatus = false;

                    //don't want the score to incriment unless the player is moving
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);


            for (int i = 0; i <= gameWidth; i += 32)
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

            //if (mouseStatus == true)
            // {
            _spriteBatch.Draw(
                mouseTexture,
                mousePosition,
                mouseRect,
                Color.White,
                0f,
                new Vector2(mouseRect.Width / 2, mouseRect.Height / 2),
                2.0f,
                SpriteEffects.None,
                0f);
            // }


            _spriteBatch.Draw(
                kittyTexture, // texture
                kittyPosition, // position on screen
                kittyRect, // where in the sprite sheet to take sprite from, source rectangle
                Color.White, // color
                0f, // rotation
                new Vector2(kittyRect.Width / 2, kittyRect.Height / 2), // center point of sprite, origin point
                2.0f, //scale
                SpriteEffects.None, //effects
                0f); //layer depth


            _spriteBatch.Draw(
                foreGround,
                Vector2.Zero,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0f);

            _spriteBatch.DrawString(scoreText, gameScore.ToString(), new Vector2(40, 8), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
