using UnityEngine;

namespace Karpik.Movement.Platformer2D
{
    [RequireComponent(typeof(Collider2D))]
    public class Surface : MonoBehaviour
    {
        [SerializeField] private bool _solid;

        public bool IsSolid => _solid;

        private Collider2D _collider;

        private void Start()
        {
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = !_solid;
        }

    }
}