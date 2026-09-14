using UnityEngine;
using UnityEngine.UI;

namespace Pong
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField] private Rigidbody2D _ball;
        [SerializeField] private Text _scoreLeftText;
        [SerializeField] private Text _scoreRightText;

        private GameHandler _funcs;

        public GameState CurrentState => _funcs != null ? _funcs.State : GameState.WaitingToStart;
        public GameWinner Winner => _funcs != null ? _funcs.Winner : GameWinner.None;
        public int ScoreLeft => _funcs != null ? _funcs.scoreLeft : 0;
        public int ScoreRight => _funcs != null ? _funcs.scoreRight : 0;

        private void Start()
        {
            instance = this;
            _funcs = new GameHandler();
            _funcs.CreatePaddles();
            _funcs.InitializeBall(_ball);
        }

        public void StartGame()
        {
            _funcs.StartGame();
            UpdateScoreUI();
            _funcs.InitializeBall(_ball);
        }

        public void ScorePoint(bool left)
        {
            if (!_funcs.IsPlaying())
                return;

            _funcs.ScorePoint(left);
            UpdateScoreUI();

            if (_funcs.IsGameOver())
            {
                Debug.Log("Game Over! Winner: " + _funcs.GetWinner());
            }
            else
            {
                _funcs.InitializeBall(_ball);
            }
        }

        public void RestartGame()
        {
            _funcs.Restart();
            UpdateScoreUI();
            _funcs.InitializeBall(_ball);
            _funcs.StartGame();
            UpdateScoreUI();
            _funcs.InitializeBall(_ball);
        }

        public bool IsPlaying()
        {
            return _funcs != null && _funcs.IsPlaying();
        }

        public bool IsGameOver()
        {
            return _funcs != null && _funcs.IsGameOver();
        }

        public bool IsWaitingToStart()
        {
            return _funcs != null && _funcs.IsWaitingToStart();
        }

        private void UpdateScoreUI()
        {
            if (_scoreLeftText != null)
                _scoreLeftText.text = _funcs.scoreLeft.ToString();
            if (_scoreRightText != null)
                _scoreRightText.text = _funcs.scoreRight.ToString();
        }
    }
}