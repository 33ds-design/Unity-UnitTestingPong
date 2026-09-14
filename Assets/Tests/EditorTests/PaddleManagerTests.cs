using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Pong;

namespace EditorTests
{
    public class PaddleManagerTests
    {
        private GameObject _paddleObj;
        private PaddleManager _paddle;

        [SetUp]
        public void SetUp()
        {
            _paddleObj = new GameObject("TestPaddle");
            _paddle = _paddleObj.AddComponent<PaddleManager>();
            _paddle.isLeft = true;
            PaddleManager.ResetSpeed();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_paddleObj);
            PaddleManager.ResetSpeed();
        }

        [Test]
        public void Paddle_IsLeft_DefaultTrue_AfterSetUp()
        {
            Assert.That(_paddle.isLeft, Is.True);
        }

        [Test]
        public void Paddle_IsAI_DefaultFalse()
        {
            Assert.That(_paddle.isAI, Is.False);
        }

        [Test]
        public void Paddle_Speed_StaticProperty_HasValue()
        {
            Assert.That(PaddleManager.Speed, Is.EqualTo(0.25f));
        }

        [Test]
        public void Paddle_AIReactionDelay_StaticProperty_HasValue()
        {
            Assert.That(PaddleManager.AIReactionDelay, Is.GreaterThan(0f));
        }

        [Test]
        public void Paddle_SetSpeed_ChangesStaticSpeed()
        {
            PaddleManager.SetSpeed(0.5f);
            Assert.That(PaddleManager.Speed, Is.EqualTo(0.5f));
        }

        [Test]
        public void Paddle_ResetSpeed_RestoresDefault()
        {
            PaddleManager.SetSpeed(1.0f);
            PaddleManager.ResetSpeed();
            Assert.That(PaddleManager.Speed, Is.EqualTo(0.25f));
        }

        [Test]
        public void Paddle_GetY_ReturnsInitialZero()
        {
            Assert.That(_paddle.GetY(), Is.EqualTo(0f));
        }

        [UnityTest]
        public IEnumerator Paddle_MoveUp_IncreasesY()
        {
            float startY = _paddle.GetY();
            _paddle.MoveUp();
            yield return null;
            Assert.That(_paddle.GetY(), Is.GreaterThan(startY));
        }

        [UnityTest]
        public IEnumerator Paddle_MoveDown_DecreasesY()
        {
            float startY = _paddle.GetY();
            _paddle.MoveDown();
            yield return null;
            Assert.That(_paddle.GetY(), Is.LessThan(startY));
        }

        [UnityTest]
        public IEnumerator Paddle_MoveUp_ClampedAtTopBound()
        {
            for (int i = 0; i < 100; i++)
                _paddle.MoveUp();
            yield return null;
            Assert.That(_paddle.GetY(), Is.LessThanOrEqualTo(4f));
        }

        [UnityTest]
        public IEnumerator Paddle_MoveDown_ClampedAtBottomBound()
        {
            for (int i = 0; i < 100; i++)
                _paddle.MoveDown();
            yield return null;
            Assert.That(_paddle.GetY(), Is.GreaterThanOrEqualTo(-4f));
        }

        [UnityTest]
        public IEnumerator Paddle_IsAtTopBound_True_WhenAtMaxY()
        {
            for (int i = 0; i < 100; i++)
                _paddle.MoveUp();
            yield return null;
            Assert.That(_paddle.IsAtTopBound(), Is.True);
        }

        [UnityTest]
        public IEnumerator Paddle_IsAtBottomBound_True_WhenAtMinY()
        {
            for (int i = 0; i < 100; i++)
                _paddle.MoveDown();
            yield return null;
            Assert.That(_paddle.IsAtBottomBound(), Is.True);
        }

        [UnityTest]
        public IEnumerator Paddle_IsAtTopBound_False_WhenNotAtMaxY()
        {
            yield return null;
            Assert.That(_paddle.IsAtTopBound(), Is.False);
        }

        [UnityTest]
        public IEnumerator Paddle_IsAtBottomBound_False_WhenNotAtMinY()
        {
            yield return null;
            Assert.That(_paddle.IsAtBottomBound(), Is.False);
        }
    }
}
