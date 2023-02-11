using UnityEngine;

namespace Karpik.Movement.Platformer2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SurfaceSlider))]
    public class Movement2D : MonoBehaviour
    {
        [SerializeField][Min(0f)] private float _gravityScale;

        [Header("Movement")]
        [SerializeField] private float _speed;
        [Tooltip("Max platform angle to go over it")]
        [SerializeField]
        [Range(0f, 90f)]
        private int _maxAngleToMove;

        [Header("Jump")]
        [SerializeField] private float _jumpForce;

        public float Speed => _speed;
        public int MaxAngleToMove => _maxAngleToMove;
        public float JumpForce => _jumpForce;
        public bool CanJump => _jumpState == JumpState.OnJumpSurface;
        public bool IsOnGround => _jumpState == JumpState.OnJumpSurface;

        private Rigidbody2D _rigidbody;
        private SurfaceSlider _surfaceSlider;

        private Vector2 _movementDirection;

        private float _jumpForceCurrent;
        private JumpState _jumpState;
        private float _timeInFlight;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _surfaceSlider = GetComponent<SurfaceSlider>();

            _rigidbody.gravityScale = 0;
            _jumpState = JumpState.None;
        }

        /// <summary>
        /// Move RigidBody2D in FixedUpdate
        /// </summary>
        /// <param name="direction">Direction to move</param>
        public void Move(Vector2 direction)
        {
            _timeInFlight += Time.fixedDeltaTime;
            direction.Normalize();

            var surfaceInfo = _surfaceSlider.Project(new Vector2(direction.x, 0), _maxAngleToMove);
            var standartMovement = FindStandartMovement(surfaceInfo.Movement);
            _movementDirection.x = standartMovement.x;

            if (surfaceInfo.IsOnSolidSurface())
            {
                _jumpState = JumpState.OnJumpSurface;
            }
            else
            {
                _jumpState = JumpState.None;
            }
            float gravity = FindGravityImpact();
            float jump = FindJumpImpact();

            _movementDirection.y += gravity;
            _movementDirection.y += jump;

            if (surfaceInfo.Angle > _maxAngleToMove)
            {
                MakeJumpZero();
            }

            _rigidbody.MovePosition(_rigidbody.position + _movementDirection);
        }

        public void Jump()
        {
            if (!CanJump)
            {
                return;
            }
            _jumpForceCurrent = _jumpForce / 100;
            _timeInFlight = 0;
        }

        private float FindJumpImpact()
        {
            _jumpForceCurrent += (_gravityScale * Time.fixedDeltaTime * _timeInFlight * Vector2.down).y;
            return Mathf.Clamp(_jumpForceCurrent, 0, float.MaxValue);
        }

        private float FindGravityImpact()
        {
            if (IsOnGround)
            {
                MakeJumpZero();
                return Vector2.zero.y;
            }
            else
            {
                return (_gravityScale * Time.fixedDeltaTime * _timeInFlight * Vector2.down).y;
            }
        }

        private Vector2 FindStandartMovement(Vector2 direction)
        {
            return direction * (_speed * Time.fixedDeltaTime);
        }

        private void MakeJumpZero()
        {
            _timeInFlight = 0;
            _movementDirection.y = 0;
        }

        private enum JumpState
        {
            None,
            OnJumpSurface,
        }
    }
}