using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EmmySpel
{
    class Bullet
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Point size;

        public bool Enabled { get; set; }

        public Bullet(Texture2D texture, Vector2 position, Vector2 velocity, Point size)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.size = size;
            Enabled = true;
        }

        public void Update(GameTime gameTime, GameWindow window)
        {
            if (!Enabled)
            {
                return;
            }

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (position.X < 0 || 
                position.X + size.X > window.ClientBounds.Width ||
                position.Y < 0 ||
                position.Y + size.Y > window.ClientBounds.Height)
            {
                Enabled = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Enabled)
            {
                spriteBatch.Draw(texture, new Rectangle(position.ToPoint(), size), Color.White);
            }
        }
    }
}
