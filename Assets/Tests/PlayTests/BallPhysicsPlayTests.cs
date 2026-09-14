using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Pong;

namespace PlayTests
{
    public class BallPhysicsPlayTests
    {
        private GameObject _ballObj;
        private BallManager _ball;
        private Rigidbody2D _rb;

        [SetUp]
        public void SetUp()
        {
            _ballObj = new GameObject("TestBall");
            _rb = _ballObj.AddComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _ball = _ballObj.AddComponent<BallManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_ballObj != null)
                Object.Destroy(_ballObj);
        }

        [UnityTest]
        public IEnumerator Ball_Physics_MovesAfterLaunch()
        {
            Vector2 startPos = _ballObj.transform.position;
            _ball.Launch(Vector2.right);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Vector2 endPos = _ballObj.transform.position;
            Assert.That(endPos.x, Is.GreaterThan(startPos.x), "Ball should have moved right");
        }

        [UnityTest]
        public IEnumerator Ball_Physics_VelocityMatchesRigidbody()
        {
            _ball.Launch(new Vector2(1f, 0f));
            yield return new WaitForFixedUpdate();

            Assert.That(_rb.velocity.magnitude, Is.GreaterThan(0f), "Rigidbody should have velocity");
        }

        [UnityTest]
        public IEnumerator Ball_Physics_OnWallHit_ReversesYDirection()
        {
            _ball.Launch(new Vector2(1f, 1f));
            yield return new WaitForFixedUpdate();

            float velYBefore = _rb.velocity.y;
            _ball.OnWallHit();
            yield return new WaitForFixedUpdate();

            Assert.That(_ball.Velocity.y, Is.EqualTo(-velYBefore).Within(0.01f));
        }

        [UnityTest]
        public IEnumerator Ball_Physics_OnPaddleHit_SpeedIncreasesInPhysics()
        {
            _ball.Launch(Vector2.right);
            yield return new WaitForFixedUpdate();

            float speedBefore = _rb.velocity.magnitude;
            _ball.OnPaddleHit();
            yield return new WaitForFixedUpdate();

            float speedAfter = _rb.velocity.magnitude;
            Assert.That(speedAfter, Is.GreaterThan(speedBefore), "Speed should increase after paddle hit");
        }

        [UnityTest]
        public IEnumerator Ball_Physics_OnWallHit_XVelocityPreserved()
        {
            _ball.Launch(new Vector2(3f, 2f));
            yield return new WaitForFixedUpdate();

            float velXBefore = _rb.velocity.x;
            _ball.OnWallHit();
            yield return new WaitForFixedUpdate();

            Assert.That(_ball.Velocity.x, Is.EqualTo(velXBefore).Within(0.01f));
        }

        [UnityTest]
        public IEnumerator Ball_Physics_MovesLeft_WhenLaunchedLeft()
        {
            Vector2 startPos = _ballObj.transform.position;
            _ball.Launch(Vector2.left);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Vector2 endPos = _ballObj.transform.position;
            Assert.That(endPos.x, Is.LessThan(startPos.x), "Ball should have moved left");
        }

        [UnityTest]
        public IEnumerator Ball_Physics_MovesUp_WhenLaunchedUp()
        {
            Vector2 startPos = _ballObj.transform.position;
            _ball.Launch(Vector2.up);
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Vector2 endPos = _ballObj.transform.position;
            Assert.That(endPos.y, Is.GreaterThan(startPos.y), "Ball should have moved up");
        }
    }
}
