using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace kittyGetMouse
{
    public class Player
    {
        float gameWidth = Game1.gameWidth;
        float gameHeight = Game1.gameHeight;

        public Texture2D kittyTexture;
        public Rectangle kittyRect;
        public Vector2 kittyPosition;

        public float kittySpeed;

        public Player()
        {
            Reset();
        }

        public void Reset()
        {
            kittyRect = new Rectangle(0, 0, 12, 16);
            kittyPosition = new Vector2(gameWidth / 2, gameHeight / 2 - 20);
            kittySpeed = 100f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
            kittyTexture,
            kittyPosition,
            kittyRect,
            Color.White,
            0f,
            new Vector2(kittyRect.Width / 2, kittyRect.Height / 2),
            2.0f,
            SpriteEffects.None,
            0f);
        }

        public void KittyMove(GameTime gameTime)
        {
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

            //boundry control
            if (kittyPosition.X > gameWidth - kittyRect.Width)
                kittyPosition.X = gameWidth - kittyRect.Width;
            else if (kittyPosition.X < kittyRect.Width)
                kittyPosition.X = kittyRect.Width;

            if (kittyPosition.Y > gameHeight - kittyRect.Height)
                kittyPosition.Y = gameHeight - kittyRect.Height;
            else if (kittyPosition.Y < kittyRect.Height)
                kittyPosition.Y = kittyRect.Height;
        }
    }
}
