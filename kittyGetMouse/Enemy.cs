using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace kittyGetMouse
{
    public class Enemy
    {
        float gameWidth = Game1.gameWidth;
        float gameHeight = Game1.gameHeight;

        public Texture2D mouseTexture;

        public Rectangle mouseRect;
        public Vector2 mousePosition;
        public float mouseSpeed;

        //movement variables
        Random random = new Random();
        float changeInPositionX;
        float changeInPositionY;
        float timeElapsed = 0.0f;

        public Enemy()
        {
            Reset();
        }

        public void Reset()
        {
            mouseRect = new Rectangle(0, 0, 8, 8);
            mousePosition = new Vector2(gameWidth / 2, (gameHeight / 2) + 40);
            mouseSpeed = 95f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
                spriteBatch.Draw(
                mouseTexture,
                mousePosition,
                mouseRect,
                Color.White,
                0f,
                new Vector2(mouseRect.Width / 2, mouseRect.Height / 2),
                2.0f,
                SpriteEffects.None,
                0f);
        }

        public void MouseMove(GameTime gameTime)
        {
            //make mouse move in a random direction at a set interval

            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed > 0.5f)
            {
                changeInPositionX = (float)random.Next(-1, 2);
                changeInPositionY = (float)random.Next(-1, 2);
                timeElapsed = 0;
            }

            mousePosition.X += mouseSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * changeInPositionX;
            mousePosition.Y += mouseSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds * changeInPositionY;

            if (changeInPositionX > 0)
                mouseRect = new Rectangle(24, 0, 8, 8);
            if (changeInPositionX < 0)
                mouseRect = new Rectangle(16, 0, 8, 8);
            if (changeInPositionY < 0)
                mouseRect = new Rectangle(8, 0, 8, 8);
            if (changeInPositionY > 0)
                mouseRect = new Rectangle(0, 0, 8, 8);

            //boundry control
            if (mousePosition.X > gameWidth - mouseRect.Width - 5)
                mousePosition.X = gameWidth - mouseRect.Width - 5;
            else if (mousePosition.X < mouseRect.Width + 5)
                mousePosition.X = mouseRect.Width + 5;

            if (mousePosition.Y > gameHeight - mouseRect.Height - 5)
                mousePosition.Y = gameHeight - mouseRect.Height - 5;
            else if (mousePosition.Y < mouseRect.Height + 10)
                mousePosition.Y = mouseRect.Height + 10;
        }
    }
}
