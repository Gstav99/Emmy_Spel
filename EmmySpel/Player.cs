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

        public InputMode InputMode { get; set; }


        public Player(Texture2D texture, Vector2 position, float speed, float bulletSpeed, TimeSpan shootingCooldown, Texture2D bulletTexture, Point bulletSize, InputMode inputMode)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
            InputMode = inputMode;
            bulletHandler = new BulletHandler(bulletSpeed, shootingCooldown, bulletTexture, bulletSize);
        }

        public void Update(GameTime gameTime, GameWindow window, IPhysical[] otherObjects)
        {
            var keyState = Keyboard.GetState();
            var gamepadState = GamePad.GetState(PlayerIndex.One);

            Vector2 movementDirection = GetMovementDirection();

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
            if (InputMode == InputMode.KeyBoard ? keyState.IsKeyDown(Keys.Space) : gamepadState.IsButtonDown(Buttons.X))
            {
                Vector2 bulletDirection = movementDirection;

                //this wholse section is to make sure that the bullet fly in a consistent speed
                if (bulletDirection.Length() < 1 && (bulletDirection.X != 0 || bulletDirection.Y != 0))
                {
                    float xPow = bulletDirection.X * bulletDirection.X;
                    float yPow = bulletDirection.Y * bulletDirection.Y;
                    float scalar = 1 / (xPow + yPow);
                    float x = (float)Math.Sqrt(xPow * scalar) * Math.Sign(bulletDirection.X);
                    float y = (float)Math.Sqrt(yPow * scalar) * Math.Sign(bulletDirection.Y);
                    bulletDirection.X = x;
                    bulletDirection.Y = y;
                    System.Diagnostics.Debug.WriteLineIf(bulletDirection.Length() < 0.999, $"Warning! bullet speed lower than intended: {bulletDirection.Length()}");
                }

                if (bulletDirection.Length() == 0)
                {
                    bulletDirection = lastBulletDirection;
                }

                bulletHandler.Shoot(gameTime, new Vector2(position.X + (texture.Width / 2), position.Y + (texture.Height / 2)), bulletDirection);

                lastBulletDirection = bulletDirection;
            }

            bulletHandler.Update(gameTime, window, otherObjects);
        }

        private Vector2 GetMovementDirection()
        {
            switch (InputMode)
            {
                case InputMode.KeyBoard:
                    return GetKeyBoardDirection();

                case InputMode.Gamepad:
                    return GetGamepadDirection();

                default:
                    throw new ArgumentException("unkown input mode");
            }
        }

        private Vector2 GetGamepadDirection()
        {
            var gamepadState = GamePad.GetState(PlayerIndex.One);
            var direction = gamepadState.ThumbSticks.Left;
            direction.Y = -direction.Y;
            return direction;
        }

        private Vector2 GetKeyBoardDirection()
        {
            KeyboardState keyState = Keyboard.GetState();

            Vector2 movement = Vector2.Zero;

            if (keyState.IsKeyDown(Keys.W))
            {
                movement.Y -= 1;
            }
            if (keyState.IsKeyDown(Keys.S))
            {
                movement.Y += 1;
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                movement.X -= 1;
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                movement.X += 1;
            }
            return movement;
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