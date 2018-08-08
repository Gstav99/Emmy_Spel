﻿using Microsoft.Xna.Framework;
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
        private float bulletSpeed;
        private TimeSpan lastBulletShot;
        /// <summary>
        /// Is a double since TimeSpan uses doubles and not floats
        /// </summary>
        private TimeSpan shootingCooldown;
        private Texture2D bulletTexture;
        private Point bulletSize;

        private List<Bullet> bullets;

        public Player(Texture2D texture, Vector2 position, float speed, float bulletSpeed, TimeSpan shootingCooldown, Texture2D bulletTexture, Point bulletSize)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
            this.bulletSpeed = bulletSpeed;
            this.shootingCooldown = shootingCooldown;
            this.bulletTexture = bulletTexture;
            this.bulletSize = bulletSize;
            lastBulletShot = new TimeSpan(0);
            bullets = new List<Bullet>();
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

            //Update the bullets and shooting
            if (keyState.IsKeyDown(Keys.Space) && gameTime.TotalGameTime - lastBulletShot >= shootingCooldown)
            {
                var bulletVelocity = new Vector2(Math.Sign(movement.X) * bulletSpeed, Math.Sign(movement.Y) * bulletSpeed);
                if(bulletVelocity.X == 0 && bulletVelocity.Y == 0)
                {
                    bulletVelocity.X = 1 * bulletSpeed;
                }

                bullets.Add(new Bullet(bulletTexture, new Vector2(position.X + (texture.Width / 2), position.Y + (texture.Height / 2)), bulletVelocity, bulletSize));
                  lastBulletShot = gameTime.TotalGameTime;
            }

            foreach(var bullet in bullets)
            {
                 bullet.Update(gameTime, window);
            }
            bullets.RemoveAll(b => !b.Enabled);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
            foreach(Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
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