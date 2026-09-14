using UnityEngine;

namespace Pong
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver
    }

    public enum GameWinner
    {
        None,
        Left,
        Right
    }

    public class GameHandler
    {
        private GameObject _paddlePrefab;
        public GameObject PaddlePrefab
        {
            get
            {
                if (_paddlePrefab == null)
                    _paddlePrefab = Resources.Load<GameObject>("Prefabs/Paddle");
                return _paddlePrefab;
            }
        }

        public int scoreLeft = 0;
        public int scoreRight = 0;
        public int maxScore = 5;

        public static float InitialBallSpeed = 10f;
        public static float BallSpeedIncrement = 0.5f;
        public static float MaxBallSpeed = 20f;

        public GameState State { get; private set; } = GameState.WaitingToStart;
        public GameWinner Winner { get; private set; } = GameWinner.None;

        public (GameObject, GameObject) CreatePaddles()
        {
            GameObject prefab = PaddlePrefab;

            GameObject left = prefab != null ? GameObject.Instantiate(prefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
            left.transform.position = new Vector2(-8, 0);
            var leftPm = left.GetComponent<PaddleManager>() ?? left.AddComponent<PaddleManager>();
            leftPm.isLeft = true;

            GameObject right = prefab != null ? GameObject.Instantiate(prefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
            right.transform.position = new Vector2(8, 0);
            var rightPm = right.GetComponent<PaddleManager>() ?? right.AddComponent<PaddleManager>();
            rightPm.isLeft = false;

            return (left, right);
        }

        public void InitializeBall(Rigidbody2D ball)
        {
            ball.transform.position = Vector2.zero;
            float angle = Random.Range(0, 15f) * Mathf.Deg2Rad;
            float r = Random.Range(0f, 1f);
            if (r < 0.25f)
                angle = 180f - angle;
            else if (r < 0.5f)
                angle += 180f;
            else if (r < 0.75f)
                angle = 360f - angle;
            float strength = InitialBallSpeed;
            ball.velocity = new Vector2(
                strength * Mathf.Cos(angle),
                strength * Mathf.Sin(angle));
        }

        public void ScorePoint(bool left)
        {
            if (State != GameState.Playing)
                return;

            if (left) scoreLeft++;
            else scoreRight++;

            if (scoreLeft >= maxScore)
            {
                Winner = GameWinner.Left;
                State = GameState.GameOver;
            }
            else if (scoreRight >= maxScore)
            {
                Winner = GameWinner.Right;
                State = GameState.GameOver;
            }
        }

        public void StartGame()
        {
            scoreLeft = 0;
            scoreRight = 0;
            Winner = GameWinner.None;
            State = GameState.Playing;
        }

        public void Restart()
        {
            scoreLeft = 0;
            scoreRight = 0;
            Winner = GameWinner.None;
            State = GameState.WaitingToStart;
        }

        public bool IsGameOver()
        {
            return State == GameState.GameOver;
        }

        public bool IsPlaying()
        {
            return State == GameState.Playing;
        }

        public bool IsWaitingToStart()
        {
            return State == GameState.WaitingToStart;
        }

        public GameWinner GetWinner()
        {
            return Winner;
        }

        public int GetTotalScore()
        {
            return scoreLeft + scoreRight;
        }
    }
}