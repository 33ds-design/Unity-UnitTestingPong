using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Pong;

namespace PlayTests
{
    public class PaddleMovementPlayTests
    {
        private GameObject _paddleObj;
        private PaddleManager _paddle;

        [SetUp]
        public void SetUp()
        {
            _paddleObj = new GameObject("TestPaddle");
            _paddle = _paddleObj.AddComponent<PaddleManager>();
            PaddleManager.ResetSpeed();
        }

        [TearDown]
        public void TearDown()
        {
            if (_paddleObj != null)
                Object.Destroy(_paddleObj);
            PaddleManager.ResetSpeed();
        }

        [UnityTest]
        public IEnumerator Paddle_MoveUp_PositionIncreases()
        {
            float yBefore = _paddle.transform.position.y;
            _paddle.MoveUp();
            yield return null;

            Assert.That(_paddle.transform.position.y, Is.GreaterThan(yBefore));
        }

        [UnityTest]
        public IEnumerator Paddle_MoveDown_PositionDecreases()
        {
            float yBefore = _paddle.transform.position.y;
            _paddle.MoveDown();
            yield return null;

            Assert.That(_paddle.transform.position.y, Is.LessThan(yBefore));
        }

        [UnityTest]
        public IEnumerator Paddle_MoveUp_RespectsTopBound()
        {
            for (int i = 0; i < 100; i++)
                _paddle.MoveUp();
            yield return null;

            Assert.That(_paddle.transform.position.y, Is.LessThanOrEqualTo(4.01f));
            Assert.That(_paddle.IsAtTopBound(), Is.True);
        }

        [UnityTest]
        public IEnumerator Paddle_MoveDown_RespectsBottomBound()
        {
            for (int i = 0; i < 100; i++)
                _paddle.MoveDown();
            yield return null;

            Assert.That(_paddle.transform.position.y, Is.GreaterThanOrEqualTo(-4.01f));
            Assert.That(_paddle.IsAtBottomBound(), Is.True);
        }

        [UnityTest]
        public IEnumerator Paddle_SetSpeed_ChangesMovementDistance()
        {
            PaddleManager.SetSpeed(0.5f);
            float yBefore = _paddle.transform.position.y;
            _paddle.MoveUp();
            yield return null;

            float diff = _paddle.transform.position.y - yBefore;
            Assert.That(diff, Is.EqualTo(0.5f).Within(0.001f));
        }

        [UnityTest]
        public IEnumerator Paddle_InitialY_IsZero()
        {
            yield return null;
            Assert.That(_paddle.GetY(), Is.EqualTo(0f).Within(0.001f));
        }

        [UnityTest]
        public IEnumerator Paddle_MoveUpDown_Y_CancelsOut()
        {
            float yBefore = _paddle.transform.position.y;
            _paddle.MoveUp();
            _paddle.MoveDown();
            yield return null;

            Assert.That(_paddle.transform.position.y, Is.EqualTo(yBefore).Within(0.001f));
        }

        [UnityTest]
        public IEnumerator Paddle_AI_MovesTowardBall_WhenBallAbove()
        {
            _paddle.isAI = true;
            GameObject ballObj = new GameObject("AIBall");
            ballObj.transform.position = new Vector2(0f, 3f);
            _paddle.ballTarget = ballObj.transform;

            float yBefore = _paddle.transform.position.y;

            var updateAIMethod = typeof(PaddleManager).GetMethod("UpdateAI",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            for (int i = 0; i < 20; i++)
                updateAIMethod.Invoke(_paddle, null);
            yield return null;

            Assert.That(_paddle.transform.position.y, Is.GreaterThan(yBefore), "AI paddle should move up toward ball");
            Object.Destroy(ballObj);
        }

        [UnityTest]
        public IEnumerator Paddle_AI_MovesTowardBall_WhenBallBelow()
        {
            _paddle.isAI = true;
            GameObject ballObj = new GameObject("AIBall");
            ballObj.transform.position = new Vector2(0f, -3f);
            _paddle.ballTarget = ballObj.transform;

            float yBefore = _paddle.transform.position.y;

            var updateAIMethod = typeof(PaddleManager).GetMethod("UpdateAI",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            for (int i = 0; i < 20; i++)
                updateAIMethod.Invoke(_paddle, null);
            yield return null;

            Assert.That(_paddle.transform.position.y, Is.LessThan(yBefore), "AI paddle should move down toward ball");
            Object.Destroy(ballObj);
        }

        [UnityTest]
        public IEnumerator Paddle_AI_DoesNotMove_WhenNoBallTarget()
        {
            _paddle.isAI = true;
            _paddle.ballTarget = null;
            float yBefore = _paddle.transform.position.y;

            for (int i = 0; i < 10; i++)
                yield return null;

            Assert.That(_paddle.transform.position.y, Is.EqualTo(yBefore).Within(0.001f));
        }
    }
}
