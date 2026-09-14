using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

using Pong;

namespace EditorTests
{
    public class GameManagerTests
    {
        private GameObject _gmObj;
        private GameManager _gm;
        private GameObject _ballObj;
        private Rigidbody2D _ball;

        [SetUp]
        public void SetUp()
        {
            _gmObj = new GameObject("TestGameManager");
            _gm = _gmObj.AddComponent<GameManager>();

            _ballObj = new GameObject("TestBall");
            _ball = _ballObj.AddComponent<Rigidbody2D>();

            var ballField = typeof(GameManager).GetField("_ball",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ballField.SetValue(_gm, _ball);

            var funcsField = typeof(GameManager).GetField("_funcs",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            funcsField.SetValue(_gm, new GameHandler());

            GameManager.instance = _gm;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gmObj);
            Object.DestroyImmediate(_ballObj);
            GameManager.instance = null;
        }

        [Test]
        public void GameManager_InitialState_IsWaitingToStart()
        {
            Assert.That(_gm.CurrentState, Is.EqualTo(GameState.WaitingToStart));
        }

        [Test]
        public void GameManager_IsWaitingToStart_True_Initially()
        {
            Assert.That(_gm.IsWaitingToStart(), Is.True);
        }

        [Test]
        public void GameManager_IsPlaying_False_Initially()
        {
            Assert.That(_gm.IsPlaying(), Is.False);
        }

        [Test]
        public void GameManager_IsGameOver_False_Initially()
        {
            Assert.That(_gm.IsGameOver(), Is.False);
        }

        [Test]
        public void GameManager_ScorePoint_WhenNotPlaying_DoesNothing()
        {
            int scoreBefore = _gm.ScoreLeft;
            _gm.ScorePoint(true);
            Assert.That(_gm.ScoreLeft, Is.EqualTo(scoreBefore));
        }

        [Test]
        public void GameManager_AfterStartGame_IsPlaying()
        {
            _gm.StartGame();
            Assert.That(_gm.IsPlaying(), Is.True);
        }

        [Test]
        public void GameManager_AfterStartGame_ScoresAreZero()
        {
            _gm.StartGame();
            Assert.That(_gm.ScoreLeft, Is.EqualTo(0));
            Assert.That(_gm.ScoreRight, Is.EqualTo(0));
        }

        [Test]
        public void GameManager_ScorePoint_Left_IncrementsScoreLeft()
        {
            _gm.StartGame();
            _gm.ScorePoint(true);
            Assert.That(_gm.ScoreLeft, Is.EqualTo(1));
            Assert.That(_gm.ScoreRight, Is.EqualTo(0));
        }

        [Test]
        public void GameManager_ScorePoint_Right_IncrementsScoreRight()
        {
            _gm.StartGame();
            _gm.ScorePoint(false);
            Assert.That(_gm.ScoreRight, Is.EqualTo(1));
        }

        [Test]
        public void GameManager_RestartGame_ResetsToPlaying()
        {
            _gm.StartGame();
            _gm.ScorePoint(true);
            _gm.RestartGame();
            Assert.That(_gm.IsPlaying(), Is.True);
            Assert.That(_gm.ScoreLeft, Is.EqualTo(0));
        }
    }
}
