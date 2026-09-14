using NUnit.Framework;

using Pong;

namespace EditorTests
{
    public class GameHandlerTests
    {
        private GameHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _handler = new GameHandler();
        }

        [Test]
        public void GameHandler_InitialState_IsWaitingToStart()
        {
            Assert.That(_handler.State, Is.EqualTo(GameState.WaitingToStart));
        }

        [Test]
        public void GameHandler_InitialWinner_IsNone()
        {
            Assert.That(_handler.Winner, Is.EqualTo(GameWinner.None));
        }

        [Test]
        public void GameHandler_InitialScores_AreZero()
        {
            Assert.That(_handler.scoreLeft, Is.EqualTo(0));
            Assert.That(_handler.scoreRight, Is.EqualTo(0));
        }

        [Test]
        public void GameHandler_AfterStartGame_StateIsPlaying()
        {
            _handler.StartGame();
            Assert.That(_handler.State, Is.EqualTo(GameState.Playing));
        }

        [Test]
        public void GameHandler_AfterStartGame_ScoresAreZero()
        {
            _handler.scoreLeft = 3;
            _handler.scoreRight = 2;
            _handler.StartGame();
            Assert.That(_handler.scoreLeft, Is.EqualTo(0));
            Assert.That(_handler.scoreRight, Is.EqualTo(0));
        }

        [Test]
        public void GameHandler_AfterStartGame_WinnerIsNone()
        {
            _handler.StartGame();
            Assert.That(_handler.Winner, Is.EqualTo(GameWinner.None));
        }

        [Test]
        public void GameHandler_ScorePoint_Left_IncrementsScoreLeft()
        {
            _handler.StartGame();
            _handler.ScorePoint(true);
            Assert.That(_handler.scoreLeft, Is.EqualTo(1));
            Assert.That(_handler.scoreRight, Is.EqualTo(0));
        }

        [Test]
        public void GameHandler_ScorePoint_Right_IncrementsScoreRight()
        {
            _handler.StartGame();
            _handler.ScorePoint(false);
            Assert.That(_handler.scoreLeft, Is.EqualTo(0));
            Assert.That(_handler.scoreRight, Is.EqualTo(1));
        }

        [Test]
        public void GameHandler_ScorePoint_WhenNotPlaying_DoesNothing()
        {
            _handler.ScorePoint(true);
            Assert.That(_handler.scoreLeft, Is.EqualTo(0));
        }

        [Test]
        public void GameHandler_ScorePoint_WhenGameOver_DoesNothing()
        {
            _handler.StartGame();
            _handler.scoreLeft = _handler.maxScore - 1;
            _handler.ScorePoint(true);
            Assert.That(_handler.State, Is.EqualTo(GameState.GameOver));
            _handler.ScorePoint(false);
            Assert.That(_handler.scoreRight, Is.EqualTo(0));
        }

        [Test]
        public void GameHandler_WhenLeftScoreReachesMax_GameOver()
        {
            _handler.StartGame();
            _handler.maxScore = 3;
            _handler.scoreLeft = 2;
            _handler.ScorePoint(true);
            Assert.That(_handler.State, Is.EqualTo(GameState.GameOver));
        }

        [Test]
        public void GameHandler_WhenRightScoreReachesMax_GameOver()
        {
            _handler.StartGame();
            _handler.maxScore = 3;
            _handler.scoreRight = 2;
            _handler.ScorePoint(false);
            Assert.That(_handler.State, Is.EqualTo(GameState.GameOver));
        }

        [Test]
        public void GameHandler_WhenLeftWins_WinnerIsLeft()
        {
            _handler.StartGame();
            _handler.maxScore = 1;
            _handler.ScorePoint(true);
            Assert.That(_handler.Winner, Is.EqualTo(GameWinner.Left));
        }

        [Test]
        public void GameHandler_WhenRightWins_WinnerIsRight()
        {
            _handler.StartGame();
            _handler.maxScore = 1;
            _handler.ScorePoint(false);
            Assert.That(_handler.Winner, Is.EqualTo(GameWinner.Right));
        }

        [Test]
        public void GameHandler_Restart_ResetsStateToWaitingToStart()
        {
            _handler.StartGame();
            _handler.Restart();
            Assert.That(_handler.State, Is.EqualTo(GameState.WaitingToStart));
        }

        [Test]
        public void GameHandler_Restart_ResetsScores()
        {
            _handler.StartGame();
            _handler.scoreLeft = 5;
            _handler.scoreRight = 3;
            _handler.Restart();
            Assert.That(_handler.scoreLeft, Is.EqualTo(0));
            Assert.That(_handler.scoreRight, Is.EqualTo(0));
        }

        [Test]
        public void GameHandler_Restart_ResetsWinner()
        {
            _handler.StartGame();
            _handler.maxScore = 1;
            _handler.ScorePoint(true);
            _handler.Restart();
            Assert.That(_handler.Winner, Is.EqualTo(GameWinner.None));
        }

        [Test]
        public void GameHandler_IsPlaying_True_WhenPlaying()
        {
            _handler.StartGame();
            Assert.That(_handler.IsPlaying(), Is.True);
        }

        [Test]
        public void GameHandler_IsPlaying_False_WhenWaitingToStart()
        {
            Assert.That(_handler.IsPlaying(), Is.False);
        }

        [Test]
        public void GameHandler_IsGameOver_True_WhenGameOver()
        {
            _handler.StartGame();
            _handler.maxScore = 1;
            _handler.ScorePoint(true);
            Assert.That(_handler.IsGameOver(), Is.True);
        }

        [Test]
        public void GameHandler_IsGameOver_False_WhenPlaying()
        {
            _handler.StartGame();
            Assert.That(_handler.IsGameOver(), Is.False);
        }

        [Test]
        public void GameHandler_IsWaitingToStart_True_Initially()
        {
            Assert.That(_handler.IsWaitingToStart(), Is.True);
        }

        [Test]
        public void GameHandler_IsWaitingToStart_False_AfterStartGame()
        {
            _handler.StartGame();
            Assert.That(_handler.IsWaitingToStart(), Is.False);
        }

        [Test]
        public void GameHandler_GetWinner_ReturnsNone_WhenNoWinner()
        {
            Assert.That(_handler.GetWinner(), Is.EqualTo(GameWinner.None));
        }

        [TestCase(0, 0, 0)]
        [TestCase(1, 0, 1)]
        [TestCase(0, 1, 1)]
        [TestCase(3, 2, 5)]
        [TestCase(5, 5, 10)]
        public void GameHandler_GetTotalScore_ReturnsCorrectSum(int left, int right, int expected)
        {
            _handler.scoreLeft = left;
            _handler.scoreRight = right;
            Assert.That(_handler.GetTotalScore(), Is.EqualTo(expected));
        }

        [TestCase(1, 0, ExpectedResult = 1)]
        [TestCase(0, 1, ExpectedResult = 1)]
        [TestCase(2, 3, ExpectedResult = 5)]
        [TestCase(4, 6, ExpectedResult = 10)]
        public int GameHandler_Parameterized_ScoreSum(int left, int right)
        {
            _handler.scoreLeft = left;
            _handler.scoreRight = right;
            return _handler.GetTotalScore();
        }

        [Test]
        public void GameHandler_StateTransitions_FollowExpectedSequence()
        {
            Assert.That(_handler.State, Is.EqualTo(GameState.WaitingToStart));
            _handler.StartGame();
            Assert.That(_handler.State, Is.EqualTo(GameState.Playing));
            _handler.maxScore = 1;
            _handler.ScorePoint(true);
            Assert.That(_handler.State, Is.EqualTo(GameState.GameOver));
            _handler.Restart();
            Assert.That(_handler.State, Is.EqualTo(GameState.WaitingToStart));
        }
    }
}
