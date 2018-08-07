using Microsoft.Xna.Framework;

namespace EmmySpel
{
    interface IPhysical
    {
        Rectangle GetBounds();

        bool Intersects(IPhysical other);
    }
}
