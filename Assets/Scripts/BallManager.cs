using UnityEngine;

namespace Pong
{
    public class BallManager : MonoBehaviour
    {
        public static float InitialSpeed = 10f;
        public static float SpeedIncrement = 0.5f;
        public static float MaxSpeed = 20f;

        private Rigidbody2D _rb;
        private float _currentSpeed;
        private int _hitCount;
        private Vector2 _velocity;

        public float CurrentSpeed => _currentSpeed;
        public int HitCount => _hitCount;
        public Vector2 Velocity => _velocity;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 direction)
        {
            _currentSpeed = InitialSpeed;
            _hitCount = 0;
            _velocity = direction.normalized * _currentSpeed;
            if (_rb != null) _rb.velocity = _velocity;
        }

        public void OnPaddleHit()
        {
            _hitCount++;
            _currentSpeed = Mathf.Min(_currentSpeed + SpeedIncrement, MaxSpeed);
            Vector2 dir = _velocity.normalized;
            _velocity = dir * _currentSpeed;
            if (_rb != null) _rb.velocity = _velocity;
        }

        public void OnWallHit()
        {
            _velocity.y = -_velocity.y;
            if (_rb != null) _rb.velocity = _velocity;
        }

        public Vector2 GetDirection()
        {
            return _velocity.normalized;
        }

        public bool IsMovingLeft()
        {
            return _velocity.x < 0;
        }

        public bool IsMovingRight()
        {
            return _velocity.x > 0;
        }

        public bool IsMovingUp()
        {
            return _velocity.y > 0;
        }

        public bool IsMovingDown()
        {
            return _velocity.y < 0;
        }

        public bool IsAtMaxSpeed()
        {
            return Mathf.Approximately(_currentSpeed, MaxSpeed);
        }
    }
}