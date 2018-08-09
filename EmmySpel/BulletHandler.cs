using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace EmmySpel
{
    class BulletHandler
    {
        private float bulletSpeed;
        private TimeSpan lastBulletShot;
        /// <summary>
        /// Is a double since TimeSpan uses doubles and not floats
        /// </summary>
        private TimeSpan shootingCooldown;
        private Texture2D bulletTexture;
        private Point bulletSize;
        private List<Bullet> bullets;

        public event EventHandler<BulletHitArgs> Bullethit = delegate { };

        public BulletHandler(float bulletSpeed, TimeSpan shootingCooldown, Texture2D bulletTexture, Point bulletSize)
        {
            this.bulletSpeed = bulletSpeed;
            this.shootingCooldown = shootingCooldown;
            this.bulletTexture = bulletTexture;
            this.bulletSize = bulletSize;
            //so that bullets can be shot as soon as the bullethandler is initialized
            lastBulletShot = -shootingCooldown;
            bullets = new List<Bullet>();
        }

        public void Update(GameTime gameTime, GameWindow window, IPhysical[] otherObjects)
        {
            foreach (var bullet in bullets)
            {
                bullet.Update(gameTime, window);
                foreach(var gameObject in otherObjects)
                {
                    if (bullet.GetBounds().Intersects(gameObject.GetBounds()))
                    {
                        Bullethit.Invoke(this, new BulletHitArgs(bullet, gameObject));
                    }
                }
            }
            bullets.RemoveAll(b => !b.Enabled);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }

        public bool Shoot(GameTime gameTime, Vector2 position, Vector2 direction)
        {
            if (gameTime.TotalGameTime - lastBulletShot < shootingCooldown)
            {
                return false;
            }

            var bulletVelocity = direction * bulletSpeed;
            if (bulletVelocity.X == 0 && bulletVelocity.Y == 0)
            {
                bulletVelocity.X = 1 * bulletSpeed;
            }

            bullets.Add(new Bullet(bulletTexture, position, bulletVelocity, bulletSize));
            lastBulletShot = gameTime.TotalGameTime;
            return true;
        }
    }
}
