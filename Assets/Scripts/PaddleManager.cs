using UnityEngine;

namespace Pong
{
    public class PaddleManager : MonoBehaviour
    {
        private static float _speed = 0.25f;
        private static float _aiReactionDelay = 0.15f;

        public bool isLeft;
        public bool isAI = false;
        public Transform ballTarget;

        private KeyCode _upKey;
        private KeyCode _downKey;
        private float _aiTimer;

        public static float Speed => _speed;
        public static float AIReactionDelay => _aiReactionDelay;

        private void Start()
        {
            _upKey = isLeft ? KeyCode.W : KeyCode.UpArrow;
            _downKey = isLeft ? KeyCode.S : KeyCode.DownArrow;
        }

        void Update()
        {
            if (isAI)
            {
                UpdateAI();
            }
            else
            {
                if (Input.GetKey(_upKey))
                    MoveUp();
                else if (Input.GetKey(_downKey))
                    MoveDown();
            }
        }

        private void UpdateAI()
        {
            if (ballTarget == null)
                return;

            _aiTimer += Time.deltaTime;
            if (_aiTimer < _aiReactionDelay)
                return;

            _aiTimer = 0f;

            float ballY = ballTarget.position.y;
            float paddleY = transform.position.y;
            float diff = ballY - paddleY;

            if (diff > 0.1f)
                MoveUp();
            else if (diff < -0.1f)
                MoveDown();
        }

        public void MoveUp()
        {
            transform.Translate(Vector2.up * _speed);
            if (transform.position.y > 4f)
            {
                Vector3 p = transform.position;
                p.y = 4f;
                transform.position = p;
            }
        }

        public void MoveDown()
        {
            transform.Translate(-Vector2.up * _speed);
            if (transform.position.y < -4f)
            {
                Vector3 p = transform.position;
                p.y = -4f;
                transform.position = p;
            }
        }

        public float GetY()
        {
            return transform.position.y;
        }

        public bool IsAtTopBound()
        {
            return Mathf.Approximately(transform.position.y, 4f);
        }

        public bool IsAtBottomBound()
        {
            return Mathf.Approximately(transform.position.y, -4f);
        }

        public static void SetSpeed(float newSpeed)
        {
            _speed = newSpeed;
        }

        public static void ResetSpeed()
        {
            _speed = 0.25f;
        }
    }
}