#define KARPIK_MOVEMENT_PLATFORMER2D
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Karpik.Movement.Platformer2D
{
    [RequireComponent(typeof(Movement2D))]
    public class SurfaceSlider : MonoBehaviour
    {
        private List<Collision2D> _collisions = new();

        private Vector2 _normal = default;
        private ContactPoint2D _contactPoint = default;
        private Transform _contactTransform = default;

        public OnSurfaceInfo Project(Vector2 forward, int maxAngle)
        {
            var angle = Vector2.Angle(forward, _normal);
            OnSurfaceInfo info = new()
            {
                Angle = angle % 90,
                Normal = _normal,
                HasContactWithSolidSurface = HasContactWithSolidSurface(),
                AngleStraightDown = Vector2.Angle(Vector2.up, _normal) % 90,
            };

            if (info.HasContactWithSolidSurface)
            {
                if (angle >= 90)
                {
                    MoveWithBigAngle(ref info, forward, maxAngle, angle);
                }
                else
                {
                    MoveWithLowAngle(ref info, forward, maxAngle, angle);
                }
            }
            else
            {
                info.Movement = forward;
            }

            return info;
        }

        public bool HasContactWithSolidSurface()
        {
            if (_contactTransform.IsUnityNull())
            {
                return false;
            }

            if (!_contactTransform.TryGetComponent(out Surface surface))
            {
                return false;
            }

            return surface.IsSolid;
        }

        private void MoveWithGoodPhysic(ref OnSurfaceInfo info, Vector2 forward)
        {
            info.Movement = forward - Vector2.Dot(forward, _normal) * _normal;
        }

        private void MoveWithBigAngle(ref OnSurfaceInfo info, Vector2 forward, int maxDegree, float angle)
        {
            if (angle < 90 + maxDegree)
            {
                MoveWithGoodPhysic(ref info, forward);
            }
            else
            {
                info.Movement = Vector2.zero;
            }
        }

        private void MoveWithLowAngle(ref OnSurfaceInfo info, Vector2 forward, int maxDegree, float angle)
        {
            MoveWithGoodPhysic(ref info, forward);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.collider.TryGetComponent<Surface>(out _))
            {
                if (!_collisions.Contains(collision))
                {
                    _collisions.Add(collision);
                }
                UpdateInfo();
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!collision.collider.TryGetComponent<Surface>(out _))
            {
                return;
            }
            _collisions.Remove(collision);
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            _contactPoint = default;
            _contactTransform = default;
            _normal = Vector2.zero;

            if (_collisions.Count > 0)
            {
                var surface = _collisions[^1];
                if (surface.contactCount == 0)
                {
                    return;
                }
                _contactPoint = surface.GetContact(0);
                _contactTransform = surface.transform;
                _normal = _contactPoint.normal;
            }
        }
    }
}