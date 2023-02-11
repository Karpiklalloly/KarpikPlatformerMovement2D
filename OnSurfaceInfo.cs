using UnityEngine;

namespace Karpik.Movement.Platformer2D
{
    public struct OnSurfaceInfo
    {
        public float Angle { get; set; }
        public Vector2 Normal { get; set; }
        public Vector2 Movement { get; set; }
        public bool HasContactWithSolidSurface { get; set; }
        public float AngleStraightDown { get; set; }

        public bool IsOnSolidSurface()
        {
            if (HasContactWithSolidSurface)
            {
                return StayOnSurface();
            }
            return false;
        }

        private bool StayOnSurface()
        {
            return Normal.y > 0;
        }
    }
}