using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace EmmySpel
{
    class Player : IPhysical, IDisposable
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
        private BulletHandler bulletHandler;
        /// <summary>
        /// In the case where the player isn't moving the player keeps shooting in the same direction as the last time
        /// </summary>
        private Vector2 lastBulletDirection;

        private InputManager inputManager;


        public Player(Texture2D texture, Vector2 position, float speed, float bulletSpeed, TimeSpan shootingCooldown, Texture2D bulletTexture, Point bulletSize, InputMode inputMode)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
            bulletHandler = new BulletHandler(bulletSpeed, shootingCooldown, bulletTexture, bulletSize);
            inputManager = new InputManager(inputMode);
        }

        public void Update(GameTime gameTime, GameWindow window, IPhysical[] otherObjects)
        {
            var keyState = Keyboard.GetState();
            var gamepadState = GamePad.GetState(PlayerIndex.One);

            Vector2 movementDirection = inputManager.GetMovementDirection();

            //because speed is measured in pixels per second we need to apply how much time has passed
            position += movementDirection * speed * (float)gameTime.ElapsedGameTime.TotalSeconds; 

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

            //Update the bullets and shooting
            if (inputManager.IsKeyDown(Keys.Space, Buttons.X))
            {
                Vector2 bulletDirection = movementDirection;

                //this wholse section is to make sure that the bullet fly in a consistent speed
                //ekvation time!
                //så längden/hypotenusan på vektoren ska vara 1
                //alltså x^2 + y^2 = c^2 = 1 där c är hypotenusan
                //om den är lägre än 1 behöver vi multiplicera den med ett tal så att den blir ett
                //kan kalla det talet a
                //det ger 1 = a * c^2 = a * (x^2 + y^2)
                //vi vet vad x och y är
                //det ger
                //1 / (x^2 + y^2) = a
                if (bulletDirection.Length() < 1 && (bulletDirection.X != 0 || bulletDirection.Y != 0))
                {
                    float xPow = bulletDirection.X * bulletDirection.X; //sparar värderna direkt så att vi slipper göra denna utäkning flera gånger
                    float yPow = bulletDirection.Y * bulletDirection.Y;
                    float scalar = 1 / (xPow + yPow); //räkna ut talet
                    float x = (float)Math.Sqrt(xPow * scalar) * Math.Sign(bulletDirection.X); //räkna ut x och y samt bibehåll rikting då xPow oxh yPow alltid är positiva
                    float y = (float)Math.Sqrt(yPow * scalar) * Math.Sign(bulletDirection.Y);
                    bulletDirection.X = x; //updatera bulletDirection
                    bulletDirection.Y = y;
                    //Det här är bara så att vi vet om någonting går fel
                    //Det skrivs ut till output delen av visual studio
                    //Ibland är längedn inte exakt 1 heller pga float aretmetik så därför kollar vi för värdet 0.999
                    System.Diagnostics.Debug.WriteLineIf(bulletDirection.Length() < 0.999, $"Warning! bullet speed lower than intended: {bulletDirection.Length()}");
                }

                //if the player isn't moving just shoot the same direction as the last time they shot
                if (bulletDirection.Length() == 0)
                {
                    bulletDirection = lastBulletDirection;
                }

                bulletHandler.Shoot(gameTime, new Vector2(position.X + (texture.Width / 2), position.Y + (texture.Height / 2)), bulletDirection);

                lastBulletDirection = bulletDirection;
            }

            bulletHandler.Update(gameTime, window, otherObjects);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
            bulletHandler.Draw(spriteBatch);
        }

        public Rectangle GetBounds() => new Rectangle(position.ToPoint(), texture.Bounds.Size);

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Player() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}