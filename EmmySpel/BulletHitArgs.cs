namespace EmmySpel
{
    public class BulletHitArgs
    {
        public BulletHitArgs(Bullet bullet, IPhysical objectHit)
        {
            Bullet = bullet;
            ObjectHit = objectHit;
        }

        public Bullet Bullet { get; set; }
        public IPhysical ObjectHit { get; set; }
    }
}