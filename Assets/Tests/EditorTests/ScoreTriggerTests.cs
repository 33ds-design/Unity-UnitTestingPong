using NUnit.Framework;
using UnityEngine;

using Pong;

namespace EditorTests
{
    public class ScoreTriggerTests
    {
        private GameObject _triggerObj;
        private ScoreTrigger _trigger;
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

            _triggerObj = new GameObject("TestScoreTrigger");
            _triggerObj.AddComponent<BoxCollider2D>().isTrigger = true;
            _trigger = _triggerObj.AddComponent<ScoreTrigger>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_triggerObj);
            Object.DestroyImmediate(_gmObj);
            Object.DestroyImmediate(_ballObj);
            GameManager.instance = null;
        }

        [Test]
        public void ScoreTrigger_IsLeft_DefaultFalse()
        {
            Assert.That(_trigger.isLeft, Is.False);
        }

        [Test]
        public void ScoreTrigger_SetIsLeft_True()
        {
            _trigger.isLeft = true;
            Assert.That(_trigger.isLeft, Is.True);
        }

        [Test]
        public void ScoreTrigger_OnTriggerEnter_ScoresForOpponent_WhenLeftTrigger()
        {
            _gm.StartGame();
            _trigger.isLeft = true;
            GameObject colObj = new GameObject("Collider");
            colObj.AddComponent<BoxCollider2D>();
            var col = colObj.GetComponent<Collider2D>();

            var method = typeof(ScoreTrigger).GetMethod("OnTriggerEnter2D",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(_trigger, new object[] { col });

            Assert.That(_gm.ScoreRight, Is.EqualTo(1));
            Object.DestroyImmediate(colObj);
        }

        [Test]
        public void ScoreTrigger_OnTriggerEnter_ScoresForOpponent_WhenRightTrigger()
        {
            _gm.StartGame();
            _trigger.isLeft = false;
            GameObject colObj = new GameObject("Collider");
            colObj.AddComponent<BoxCollider2D>();
            var col = colObj.GetComponent<Collider2D>();

            var method = typeof(ScoreTrigger).GetMethod("OnTriggerEnter2D",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(_trigger, new object[] { col });

            Assert.That(_gm.ScoreLeft, Is.EqualTo(1));
            Object.DestroyImmediate(colObj);
        }
    }
}
