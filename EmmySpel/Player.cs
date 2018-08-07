using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EmmySpel
{
    class Player : IPhysical
    {
        private Texture2D texture;
        private Vector2 position;
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
            movement *= (float)gameTime.ElapsedGameTime.TotalSeconds;

            position += movement;

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

            foreach(var physicalObject in otherObjects)
            {
                if (Intersects(physicalObject))
                {
                    var otherBounds = physicalObject.GetBounds();
                    var overlap = Rectangle.Intersect(GetBounds(), otherBounds);
                    if (overlap.Width < overlap.Height)
                    {
                        position.X += overlap.X > otherBounds.Center.X ? overlap.Width : -overlap.Width;
                    }
                    else
                    {
                        position.Y += overlap.Y > otherBounds.Center.Y ? overlap.Height : -overlap.Height;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public Rectangle GetBounds() => new Rectangle(position.ToPoint(), texture.Bounds.Size);

        public bool Intersects(IPhysical other) => GetBounds().Intersects(other.GetBounds());
    }
}