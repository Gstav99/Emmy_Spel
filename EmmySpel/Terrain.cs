using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EmmySpel
{
    class Terrain : IPhysical
    {
        private Texture2D texture;
        private Point size;
        private Vector2 position;

        public Terrain(Texture2D texture, Point size, Vector2 position)
        {
            this.texture = texture;
            this.size = size;
            this.position = position;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle(position.ToPoint(), size), Color.White);
        }

        public Rectangle GetBounds() => new Rectangle(position.ToPoint(), size);
    }
}
