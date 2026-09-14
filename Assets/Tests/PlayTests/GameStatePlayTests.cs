using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Pong;

namespace PlayTests
{
    public class GameStatePlayTests
    {
        private GameObject _gameObj;
        private GameManager _gameManager;
        private GameObject _ballObj;
        private Rigidbody2D _ballRb;
        private GameObject _leftPaddle;
        private GameObject _rightPaddle;

        [SetUp]
        public void SetUp()
        {
            _ballObj = new GameObject("Ball");
            _ballRb = _ballObj.AddComponent<Rigidbody2D>();
            _ballRb.gravityScale = 0f;

            _gameObj = new GameObject("GameManager");
            _gameManager = _gameObj.AddComponent<GameManager>();

            var ballField = typeof(GameManager).GetField("_ball",
                BindingFlags.NonPublic | BindingFlags.Instance);
            ballField.SetValue(_gameManager, _ballRb);

            GameManager.instance = _gameManager;
        }

        [TearDown]
        public void TearDown()
        {
            if (_gameObj != null)
                Object.Destroy(_gameObj);
            if (_ballObj != null)
                Object.Destroy(_ballObj);
            GameManager.instance = null;
        }

        [UnityTest]
        public IEnumerator GameManager_Start_InitializesInWaitingState()
        {
            yield return null;
            yield return null;

            Assert.That(_gameManager.IsWaitingToStart(), Is.True, "Game should start in WaitingToStart state");
        }

        [UnityTest]
        public IEnumerator GameManager_StartGame_TransitionsToPlaying()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            yield return null;

            Assert.That(_gameManager.IsPlaying(), Is.True, "Game should be in Playing state after StartGame");
        }

        [UnityTest]
        public IEnumerator GameManager_ScoreLeft_IncrementsLeftScore()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            _gameManager.ScorePoint(true);
            yield return null;

            Assert.That(_gameManager.ScoreLeft, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator GameManager_ScoreRight_IncrementsRightScore()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            _gameManager.ScorePoint(false);
            yield return null;

            Assert.That(_gameManager.ScoreRight, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator GameManager_ScoreNotCounted_WhenNotPlaying()
        {
            yield return null;
            yield return null;

            _gameManager.ScorePoint(true);
            yield return null;

            Assert.That(_gameManager.ScoreLeft, Is.EqualTo(0), "Score should not change when not playing");
        }

        [UnityTest]
        public IEnumerator GameManager_GameOver_WhenLeftReachesMaxScore()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            for (int i = 0; i < 5; i++)
                _gameManager.ScorePoint(true);
            yield return null;

            Assert.That(_gameManager.IsGameOver(), Is.True, "Game should be over when left reaches max score");
            Assert.That(_gameManager.Winner, Is.EqualTo(GameWinner.Left));
        }

        [UnityTest]
        public IEnumerator GameManager_GameOver_WhenRightReachesMaxScore()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            for (int i = 0; i < 5; i++)
                _gameManager.ScorePoint(false);
            yield return null;

            Assert.That(_gameManager.IsGameOver(), Is.True, "Game should be over when right reaches max score");
            Assert.That(_gameManager.Winner, Is.EqualTo(GameWinner.Right));
        }

        [UnityTest]
        public IEnumerator GameManager_ScoreAfterGameOver_IsIgnored()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            for (int i = 0; i < 5; i++)
                _gameManager.ScorePoint(true);

            int scoreBefore = _gameManager.ScoreLeft;
            _gameManager.ScorePoint(true);
            yield return null;

            Assert.That(_gameManager.ScoreLeft, Is.EqualTo(scoreBefore), "Score should not change after game over");
        }

        [UnityTest]
        public IEnumerator GameManager_Restart_ResetsState()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            for (int i = 0; i < 5; i++)
                _gameManager.ScorePoint(true);

            Assert.That(_gameManager.IsGameOver(), Is.True);

            _gameManager.RestartGame();
            yield return null;

            Assert.That(_gameManager.IsPlaying(), Is.True, "Game should be playing after restart");
            Assert.That(_gameManager.ScoreLeft, Is.EqualTo(0), "Left score should reset after restart");
            Assert.That(_gameManager.ScoreRight, Is.EqualTo(0), "Right score should reset after restart");
        }

        [UnityTest]
        public IEnumerator GameManager_AlternatingScores_NoEarlyGameOver()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            for (int i = 0; i < 4; i++)
            {
                _gameManager.ScorePoint(true);
                _gameManager.ScorePoint(false);
            }
            yield return null;

            Assert.That(_gameManager.IsGameOver(), Is.False, "Game should not be over with 4-4 score");
            Assert.That(_gameManager.ScoreLeft, Is.EqualTo(4));
            Assert.That(_gameManager.ScoreRight, Is.EqualTo(4));
        }

        [UnityTest]
        public IEnumerator GameManager_GameOver_PreventsFurtherScoring()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            for (int i = 0; i < 5; i++)
                _gameManager.ScorePoint(false);

            Assert.That(_gameManager.IsGameOver(), Is.True);

            _gameManager.ScorePoint(false);
            yield return null;

            Assert.That(_gameManager.ScoreRight, Is.EqualTo(5), "Score should remain at max after game over");
        }

        [UnityTest]
        public IEnumerator GameManager_BallVelocity_NonZero_AfterStart()
        {
            yield return null;
            yield return null;

            Assert.That(_ballRb.velocity.magnitude, Is.GreaterThan(0f), "Ball should have velocity after Start");
        }

        [UnityTest]
        public IEnumerator GameManager_BallVelocity_NonZero_AfterStartGame()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            yield return null;

            Assert.That(_ballRb.velocity.magnitude, Is.GreaterThan(0f), "Ball should have velocity after StartGame");
        }
    }
}
