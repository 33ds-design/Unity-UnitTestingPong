using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Pong;

namespace PlayTests
{
    public class ScoreTriggerPlayTests
    {
        private GameObject _gmObj;
        private GameManager _gameManager;
        private GameObject _ballObj;
        private Rigidbody2D _ballRb;
        private GameObject _triggerObj;
        private ScoreTrigger _scoreTrigger;
        private BoxCollider2D _triggerCol;

        [SetUp]
        public void SetUp()
        {
            _ballObj = new GameObject("Ball");
            _ballRb = _ballObj.AddComponent<Rigidbody2D>();
            _ballRb.gravityScale = 0f;
            var ballCol = _ballObj.AddComponent<CircleCollider2D>();
            ballCol.radius = 0.5f;

            _gmObj = new GameObject("GameManager");
            _gameManager = _gmObj.AddComponent<GameManager>();
            var ballField = typeof(GameManager).GetField("_ball",
                BindingFlags.NonPublic | BindingFlags.Instance);
            ballField.SetValue(_gameManager, _ballRb);
            GameManager.instance = _gameManager;

            _triggerObj = new GameObject("ScoreTrigger");
            _scoreTrigger = _triggerObj.AddComponent<ScoreTrigger>();
            _triggerCol = _triggerObj.AddComponent<BoxCollider2D>();
            _triggerCol.isTrigger = true;
            _triggerObj.transform.position = new Vector2(10f, 0f);
        }

        [TearDown]
        public void TearDown()
        {
            if (_gmObj != null)
                Object.Destroy(_gmObj);
            if (_ballObj != null)
                Object.Destroy(_ballObj);
            if (_triggerObj != null)
                Object.Destroy(_triggerObj);
            GameManager.instance = null;
        }

        [UnityTest]
        public IEnumerator ScoreTrigger_LeftTrigger_ScoresRight()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            yield return null;

            _scoreTrigger.isLeft = true;
            _ballObj.transform.position = _triggerObj.transform.position;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            var method = typeof(ScoreTrigger).GetMethod("OnTriggerEnter2D",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Collider2D col = _ballObj.GetComponent<Collider2D>();
            method.Invoke(_scoreTrigger, new object[] { col });
            yield return null;

            Assert.That(_gameManager.ScoreRight, Is.EqualTo(1), "Right should score when ball hits left trigger");
        }

        [UnityTest]
        public IEnumerator ScoreTrigger_RightTrigger_ScoresLeft()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            yield return null;

            _scoreTrigger.isLeft = false;
            _ballObj.transform.position = _triggerObj.transform.position;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            var method = typeof(ScoreTrigger).GetMethod("OnTriggerEnter2D",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Collider2D col = _ballObj.GetComponent<Collider2D>();
            method.Invoke(_scoreTrigger, new object[] { col });
            yield return null;

            Assert.That(_gameManager.ScoreLeft, Is.EqualTo(1), "Left should score when ball hits right trigger");
        }

        [UnityTest]
        public IEnumerator ScoreTrigger_ScoreNotCounted_WhenGameNotPlaying()
        {
            yield return null;
            yield return null;

            _scoreTrigger.isLeft = false;
            var method = typeof(ScoreTrigger).GetMethod("OnTriggerEnter2D",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Collider2D col = _ballObj.GetComponent<Collider2D>();
            method.Invoke(_scoreTrigger, new object[] { col });
            yield return null;

            Assert.That(_gameManager.ScoreLeft, Is.EqualTo(0), "Score should not change when game not playing");
        }

        [UnityTest]
        public IEnumerator ScoreTrigger_MultipleScores_AccumulateCorrectly()
        {
            yield return null;
            yield return null;

            _gameManager.StartGame();
            yield return null;

            _scoreTrigger.isLeft = true;
            var method = typeof(ScoreTrigger).GetMethod("OnTriggerEnter2D",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Collider2D col = _ballObj.GetComponent<Collider2D>();

            for (int i = 0; i < 3; i++)
            {
                method.Invoke(_scoreTrigger, new object[] { col });
                yield return null;
            }

            Assert.That(_gameManager.ScoreRight, Is.EqualTo(3), "Right score should be 3 after 3 triggers");
        }
    }
}
