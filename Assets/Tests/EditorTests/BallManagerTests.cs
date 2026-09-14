using NUnit.Framework;
using UnityEngine;

using Pong;

namespace EditorTests
{
    public class BallManagerTests
    {
        private GameObject _ballObj;
        private BallManager _ball;
        private Rigidbody2D _rb;

        [SetUp]
        public void SetUp()
        {
            _ballObj = new GameObject("TestBall");
            _rb = _ballObj.AddComponent<Rigidbody2D>();
            _ball = _ballObj.AddComponent<BallManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_ballObj);
        }

        [Test]
        public void Ball_AfterLaunch_HasNonZeroVelocity()
        {
            _ball.Launch(Vector2.right);
            Assert.That(_ball.Velocity, Is.Not.EqualTo(Vector2.zero));
        }

        [Test]
        public void Ball_AfterLaunch_InitialSpeedIsCorrect()
        {
            _ball.Launch(Vector2.right);
            Assert.That(_ball.CurrentSpeed, Is.EqualTo(BallManager.InitialSpeed));
        }

        [Test]
        public void Ball_AfterLaunch_HitCountIsZero()
        {
            _ball.Launch(Vector2.right);
            Assert.That(_ball.HitCount, Is.EqualTo(0));
        }

        [Test]
        public void Ball_SpeedIncrements_AfterPaddleHit()
        {
            _ball.Launch(Vector2.right);
            float speedBefore = _ball.CurrentSpeed;
            _ball.OnPaddleHit();
            Assert.That(_ball.CurrentSpeed, Is.EqualTo(speedBefore + BallManager.SpeedIncrement));
        }

        [Test]
        public void Ball_HitCount_IncrementsAfterEachPaddleHit()
        {
            _ball.Launch(Vector2.right);
            _ball.OnPaddleHit();
            _ball.OnPaddleHit();
            _ball.OnPaddleHit();
            Assert.That(_ball.HitCount, Is.EqualTo(3));
        }

        [Test]
        public void Ball_Speed_CappedAtMaxSpeed()
        {
            _ball.Launch(Vector2.right);
            int maxHits = Mathf.CeilToInt((BallManager.MaxSpeed - BallManager.InitialSpeed) / BallManager.SpeedIncrement) + 5;
            for (int i = 0; i < maxHits; i++)
                _ball.OnPaddleHit();
            Assert.That(_ball.CurrentSpeed, Is.LessThanOrEqualTo(BallManager.MaxSpeed));
        }

        [Test]
        public void Ball_IsAtMaxSpeed_True_WhenCapped()
        {
            _ball.Launch(Vector2.right);
            int maxHits = Mathf.CeilToInt((BallManager.MaxSpeed - BallManager.InitialSpeed) / BallManager.SpeedIncrement) + 5;
            for (int i = 0; i < maxHits; i++)
                _ball.OnPaddleHit();
            Assert.That(_ball.IsAtMaxSpeed(), Is.True);
        }

        [Test]
        public void Ball_OnWallHit_ReversesYVelocity()
        {
            _ball.Launch(new Vector2(1f, 1f));
            float velYBefore = _ball.Velocity.y;
            _ball.OnWallHit();
            Assert.That(_ball.Velocity.y, Is.EqualTo(-velYBefore).Within(0.001f));
        }

        [Test]
        public void Ball_OnWallHit_PreservesXVelocity()
        {
            _ball.Launch(new Vector2(3f, 2f));
            float velXBefore = _ball.Velocity.x;
            _ball.OnWallHit();
            Assert.That(_ball.Velocity.x, Is.EqualTo(velXBefore).Within(0.001f));
        }

        [Test]
        public void Ball_IsMovingLeft_True_WhenVelocityXNegative()
        {
            _ball.Launch(Vector2.left);
            Assert.That(_ball.IsMovingLeft(), Is.True);
        }

        [Test]
        public void Ball_IsMovingRight_True_WhenVelocityXPositive()
        {
            _ball.Launch(Vector2.right);
            Assert.That(_ball.IsMovingRight(), Is.True);
        }

        [Test]
        public void Ball_IsMovingUp_True_WhenVelocityYPositive()
        {
            _ball.Launch(Vector2.up);
            Assert.That(_ball.IsMovingUp(), Is.True);
        }

        [Test]
        public void Ball_IsMovingDown_True_WhenVelocityYNegative()
        {
            _ball.Launch(Vector2.down);
            Assert.That(_ball.IsMovingDown(), Is.True);
        }

        [Test]
        public void Ball_GetDirection_ReturnsNormalizedVelocity()
        {
            _ball.Launch(new Vector2(3f, 4f));
            Vector2 dir = _ball.GetDirection();
            Assert.That(dir.magnitude, Is.EqualTo(1f).Within(0.001f));
        }
    }
}
