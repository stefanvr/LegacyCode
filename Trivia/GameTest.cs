using System.Collections.Generic;
using System.Linq;
using UglyTrivia;

namespace Trivia
{
    public class GameTest
    {
        private const string FIRST_PLAYER_NAME = "FirstPlayerName";
        private const string SECOND_PLAYER_NAME = "SecondPlayerName";

        private const int EVEN_DICE_VALUE = 2;
        private const int ODD_DICE_VALUE = 1;

        public class GameUsage
        {
            protected List<string> DisplayedMessages;
            protected Game Game;

            protected void CreateGame()
            {
                DisplayedMessages = new List<string>();
                Game = new Game(DisplayedMessages.Add);
            }

            protected void AddPlayerOneToGame()
            {
                Game.AddPlayer(FIRST_PLAYER_NAME);
                DisplayedMessages.Clear();
            }

            protected void AddPlayerTwoToGame()
            {
                Game.AddPlayer(SECOND_PLAYER_NAME);
                DisplayedMessages.Clear();
            }
        }

        [TestFixture]
        public class OnGameSetup : GameUsage
        {
            [SetUp]
            public void SetUp()
            {
                CreateGame();
            }

            [Test]
            public void AddingPlayerShows_PlayerAddedAndNumberOfPlayersBeeingOne()
            {
                var expectedMessages = new []  {
                                               string.Format(GameMessages.PLAYER_ADDED, FIRST_PLAYER_NAME),
                                               string.Format(GameMessages.NUMBER_OF_PLAYERS, 1)
                                           };

                Game.AddPlayer(FIRST_PLAYER_NAME);
            
                CollectionAssert.AreEqual(expectedMessages, DisplayedMessages);
            }

            [Test]
            public void AddingSecondPlayerShows_PlayerAddedAndNumberOfPlayersBeeingTwo()
            {
                AddPlayerOneToGame();

                var expectedMessages = new[]  {
                                               string.Format(GameMessages.PLAYER_ADDED, SECOND_PLAYER_NAME),
                                               string.Format(GameMessages.NUMBER_OF_PLAYERS, 2)
                                           };

                Game.AddPlayer(SECOND_PLAYER_NAME);

                CollectionAssert.AreEqual(expectedMessages, DisplayedMessages);
            }
        }

        [TestFixture]
        public class AnswerQuestion : GameUsage
        {
            [SetUp]
            public void SetUp()
            {
                CreateGame();
                AddPlayerOneToGame();
            }

            private void AssertCorrectAnswerFor(string playerName, int playerScore)
            {
                var expectedMessages = new[]
                                           {
                                               GameMessages.CORRECT_ANSWER,
                                               string.Format(GameMessages.GOLDEN_COINS, playerName, playerScore)
                                           };

                CollectionAssert.AreEqual(expectedMessages, DisplayedMessages);
            }

            [Test]
            public void OneTimeCorrect_DoesMakePlayerWinner()
            {
                Assert.True(Game.HandleCorrectAnswerFromPlayer());
            }

            [Test]
            public void SixCTimesCorrect_WinsTheGame()
            {
                for (int i = 0; i < Game.WINNING_NUMBER_OF_GOLDEN_COINS; i++)
                    Game.HandleCorrectAnswerFromPlayer();

                Assert.True(Game.HandleCorrectAnswerFromPlayer());
            }

            [Test]
            public void Correct_AddsGoldenCoin()
            {
                Game.HandleCorrectAnswerFromPlayer();
                AssertCorrectAnswerFor(FIRST_PLAYER_NAME, 1);
            }

            [Test]
            public void Correct_AddsGoldenCoin_ForEachAnswer()
            {
                for (int correctAnswers = 1; correctAnswers <= Game.WINNING_NUMBER_OF_GOLDEN_COINS; correctAnswers++)
                {
                    DisplayedMessages.Clear();
                    Game.HandleCorrectAnswerFromPlayer();
                    AssertCorrectAnswerFor(FIRST_PLAYER_NAME, correctAnswers);
                }
            }

            [Test]
            public void Correct_GettingOutOfPenaltyBox_AddsGoldenCoin()
            {
                Game.HandleIncorrectAnswerFromPlayer();
                Game.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(ODD_DICE_VALUE);
                DisplayedMessages.Clear();

                Game.HandleCorrectAnswerFromPlayer();

                AssertCorrectAnswerFor(FIRST_PLAYER_NAME, 1);
            }

            [Test]
            public void Correct_WhenInPenaltyBox_DoesNotAddGoldenCoin()
            {
                Game.HandleIncorrectAnswerFromPlayer();
                Game.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(EVEN_DICE_VALUE);
                DisplayedMessages.Clear();

                Game.HandleCorrectAnswerFromPlayer();

                Assert.AreEqual(0, DisplayedMessages.Count);
            }

            [Test]
            public void InCorrect_SendsPlayerToPenaltyBox_And_NoGoldenCoin()
            {
                Game.HandleIncorrectAnswerFromPlayer();

                var expectedMessages = new[] {
                                               GameMessages.INCORRECT_ANSWER,
                                               string.Format(GameMessages.SEND_TO_PENALTY_BOX, FIRST_PLAYER_NAME)
                                           };

                CollectionAssert.AreEqual(expectedMessages, DisplayedMessages);
            }
        }

        [TestFixture]
        public class TurnOf : GameUsage
        {
            [SetUp]
            public void SetUp()
            {
                CreateGame();
                AddPlayerOneToGame();
                AddPlayerTwoToGame();
            }

            private void AssertCurrentPlayer(string playerName)
            {
                DisplayedMessages.Clear();
                Game.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(1);
                var msg = string.Format(GameMessages.CURRENT_PLAYER, playerName);
                Assert.AreEqual(msg, DisplayedMessages.First());
            }

            private void SetTurnPlayerTwo()
            {
                Game.HandleIncorrectAnswerFromPlayer();
            }

            private void PutPlayersInPenaltyBox()
            {
                Game.HandleIncorrectAnswerFromPlayer();
                Game.HandleIncorrectAnswerFromPlayer();
            }

            [Test]
            public void PlayerTwo_AfterCorrectAnswerPlayerOne()
            {
                Game.HandleCorrectAnswerFromPlayer();
                AssertCurrentPlayer(SECOND_PLAYER_NAME);
            }

            [Test]
            public void PlayerOne_AfterCorrectAnswerPlayerTwo()
            {
                SetTurnPlayerTwo();
                Game.HandleCorrectAnswerFromPlayer();
                AssertCurrentPlayer(FIRST_PLAYER_NAME);
            }

            [Test]
            public void PlayerTwo_AfterIncorrectAnswerPlayerOne()
            {
                Game.HandleIncorrectAnswerFromPlayer();
                AssertCurrentPlayer(SECOND_PLAYER_NAME);
            }

            [Test]
            public void PlayerOne_AfterIncorrectAnswerPlayerTwo()
            {
                SetTurnPlayerTwo();
                Game.HandleIncorrectAnswerFromPlayer();
                AssertCurrentPlayer(FIRST_PLAYER_NAME);
            }

            [Test]
            public void PlayerTwo_AfterIncorrectAnswerPlayerOne_MovingOutPenaltyBox()
            {
                PutPlayersInPenaltyBox();
                Game.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(EVEN_DICE_VALUE);
                Game.HandleIncorrectAnswerFromPlayer();
                AssertCurrentPlayer(SECOND_PLAYER_NAME);
            }

            [Test]
            public void PlayerOne_AfterIncorrectAnswerPlayerTwo_MovingOutPenaltyBox()
            {
                PutPlayersInPenaltyBox();
                SetTurnPlayerTwo();
                Game.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(EVEN_DICE_VALUE);
                Game.HandleIncorrectAnswerFromPlayer();
                AssertCurrentPlayer(FIRST_PLAYER_NAME);
            }
        }

        [TestFixture]
        public class ThrowingDice : GameUsage
        {
            [SetUp]
            public void SetUp()
            {
                CreateGame();
                AddPlayerOneToGame();
            }

            [Test]
            public void WithValue1_MovesPlayerToFirstLocation()
            {
                const int diceValue = 1;
                Game.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(diceValue);

                var expectedMessages = new[] {
                                               string.Format(GameMessages.CURRENT_PLAYER,FIRST_PLAYER_NAME),
                                               string.Format(GameMessages.DICE_VALUE, diceValue),
                                               string.Format(GameMessages.NEW_LOCATION, FIRST_PLAYER_NAME, diceValue),
                                               string.Format(GameMessages.QUESTION_CATEGORY, "Science"),
                                               string.Format(GameMessages.QUESTION, "Science", 0)
                                           };

                CollectionAssert.AreEqual(expectedMessages, DisplayedMessages);
            }

            [Test]
            public void WhenPlayerInPenaltyBox_WithOddValue_MovesPlayerToFirstLocation_OutOfPenaltyBox()
            {
                Game.HandleIncorrectAnswerFromPlayer();
                DisplayedMessages.Clear();
                Game.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(ODD_DICE_VALUE);

                var expectedMessages = new[] {
                                               string.Format(GameMessages.CURRENT_PLAYER,FIRST_PLAYER_NAME),
                                               string.Format(GameMessages.DICE_VALUE, ODD_DICE_VALUE),
                                               string.Format(GameMessages.GET_OUT_OF_PENALTY_BOX, FIRST_PLAYER_NAME),
                                               string.Format(GameMessages.NEW_LOCATION, FIRST_PLAYER_NAME, ODD_DICE_VALUE),
                                               string.Format(GameMessages.QUESTION_CATEGORY, "Science"),
                                               string.Format(GameMessages.QUESTION, "Science", 0)
                                           };

                CollectionAssert.AreEqual(expectedMessages, DisplayedMessages);
            }

            [Test]
            public void WhenPlayerInPenaltyBox_WithEvenValue_KeepsPlayerInPenaltyBox()
            {
                Game.HandleIncorrectAnswerFromPlayer();
                DisplayedMessages.Clear();
                Game.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(EVEN_DICE_VALUE);

                var expectedMessages = new[] {
                                               string.Format(GameMessages.CURRENT_PLAYER,FIRST_PLAYER_NAME),
                                               string.Format(GameMessages.DICE_VALUE, EVEN_DICE_VALUE),
                                               string.Format(GameMessages.STAY_IN_PENALTY_BOX, FIRST_PLAYER_NAME)
                                           };

                CollectionAssert.AreEqual(expectedMessages, DisplayedMessages);
            }
        }
    }
}
