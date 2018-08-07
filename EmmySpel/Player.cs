using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EmmySpel
{
    class Player : IPhysical
    {
        /// <summary>
        /// The texture representing the player
        /// </summary>
        private Texture2D texture;
        private Vector2 position;
        /// <summary>
        /// Speed in pixels per second
        /// </summary>
        private float speed;

        public Player(Texture2D texture, Vector2 position, float speed)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
        }

        public void Update(GameTime gameTime, GameWindow window, IPhysical[] otherObjects)
        {
            var keyState = Keyboard.GetState();

            Vector2 movement = Vector2.Zero;

            if (keyState.IsKeyDown(Keys.W))
            {
                movement.Y -= speed;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                movement.Y += speed;
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                movement.X -= speed;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                movement.X += speed;
            }
            movement *= (float)gameTime.ElapsedGameTime.TotalSeconds; //because speed is measured in pixels per second we need to apply how much time has passed

            position += movement; //move the player

            //Check so that the player is within the screen
            if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.Y < 0)
            {
                position.Y = 0;
            }
            if (position.X + texture.Width > window.ClientBounds.Width)
            {
                position.X = window.ClientBounds.Width - texture.Width;
            }
            if (position.Y + texture.Height > window.ClientBounds.Height)
            {
                position.Y = window.ClientBounds.Height - texture.Height;
            }

            //Check if the player has collided with any other object and if so try to separate the player from the object
            var playerBounds = GetBounds();
            foreach (var physicalObject in otherObjects)
            {
                if (playerBounds.Intersects(physicalObject.GetBounds()))
                {
                    var otherBounds = physicalObject.GetBounds();
                    var overlap = Rectangle.Intersect(playerBounds, otherBounds);
                    if (overlap.Width < overlap.Height)
                    {
                        position.X += overlap.X > otherBounds.Center.X ? overlap.Width : -overlap.Width;
                    }
                    else
                    {
                        position.Y += overlap.Y > otherBounds.Center.Y ? overlap.Height : -overlap.Height;
                    }
                    playerBounds = GetBounds(); //sinve the player has been moved, we need to update their bounds
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public Rectangle GetBounds() => new Rectangle(position.ToPoint(), texture.Bounds.Size);
    }
}