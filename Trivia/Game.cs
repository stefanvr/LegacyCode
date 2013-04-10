using System;
using System.Collections.Generic;
using System.Linq;

namespace UglyTrivia
{
    public class Game
    {
        private readonly Action<string> DisplayMessage = message => Console.WriteLine(message);

        public Game(Action<string> displayMessage)
            : this()
        {
            DisplayMessage = displayMessage;
        }

        private const int MAX_NUMBER_OF_FIELDS = 12;
        private const int INDEX_LAST_LOCATION = 11;
        private const int MAX_QUESTIONS_PER_CATEGORY = 50;
        private const int FIRST_PLAYER_INDEX = 0;
        public const int WINNING_NUMBER_OF_GOLDEN_COINS = 6;

        List<string> Players = new List<string>();

        int[] Locations = new int[6];
        int[] PlayersNumberOfGoldenCoinsWon = new int[6];

        bool[] PlayerInPenaltyBoxState = new bool[6];

        LinkedList<string> popQuestions = new LinkedList<string>();
        LinkedList<string> scienceQuestions = new LinkedList<string>();
        LinkedList<string> sportsQuestions = new LinkedList<string>();
        LinkedList<string> rockQuestions = new LinkedList<string>();

        int CurrentPlayerIndex = 0;
        bool IsGettingOutOfPenaltyBox;

        public Game()
        {
            for (int questionNumber = 0; questionNumber < MAX_QUESTIONS_PER_CATEGORY; questionNumber++)
            {
                popQuestions.AddLast("Pop Question " + questionNumber);
                scienceQuestions.AddLast(("Science Question " + questionNumber));
                sportsQuestions.AddLast(("Sports Question " + questionNumber));
                rockQuestions.AddLast(CreateRockQuestion(questionNumber));
            }
        }

        public String CreateRockQuestion(int index)
        {
            return "Rock Question " + index;
        }

        // not used
        public bool IsPlayable()
        {
            return (NumberOfPlayerInTheGame() >= 2);
        }

        public bool AddPlayer(String playerName)
        {
            Players.Add(playerName);
            Locations[NumberOfPlayerInTheGame()] = 0;
            PlayersNumberOfGoldenCoinsWon[NumberOfPlayerInTheGame()] = 0;
            PlayerInPenaltyBoxState[NumberOfPlayerInTheGame()] = false;

            DisplayMessage(playerName + " was added");
            DisplayMessage("They are player number " + Players.Count);
            return true;
        }

        public int NumberOfPlayerInTheGame()
        {
            return Players.Count;
        }

        public void UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(int diceValue)
        {
            DisplayMessage(Players[CurrentPlayerIndex] + " is the current player");
            DisplayMessage("They have rolled a " + diceValue);

            if (PlayerInPenaltyBoxState[CurrentPlayerIndex])
            {
                if (diceValue % 2 == 0)
                {
                    DisplayMessage(Players[CurrentPlayerIndex] + " is not getting out of the penalty box");
                    IsGettingOutOfPenaltyBox = false;
                    return;
                }

                IsGettingOutOfPenaltyBox = true;
                DisplayMessage(Players[CurrentPlayerIndex] + " is getting out of the penalty box");
            }

            Locations[CurrentPlayerIndex] = Locations[CurrentPlayerIndex] + diceValue;
            if (Locations[CurrentPlayerIndex] > 11) Locations[CurrentPlayerIndex] = Locations[CurrentPlayerIndex] - 12;

            DisplayMessage(Players[CurrentPlayerIndex]
                    + "'s new location is "
                    + Locations[CurrentPlayerIndex]);
            DisplayMessage("The category is " + GetCurrentCategory());
            PrintQuestion();
        }

        private void PrintQuestion()
        {
            var currentCategory = GetCurrentCategory();

            if (currentCategory == "Pop")
            {
                DisplayMessage(popQuestions.First());
                popQuestions.RemoveFirst();
            }
            if (currentCategory == "Science")
            {
                DisplayMessage(scienceQuestions.First());
                scienceQuestions.RemoveFirst();
            }
            if (currentCategory == "Sports")
            {
                DisplayMessage(sportsQuestions.First());
                sportsQuestions.RemoveFirst();
            }
            if (currentCategory == "Rock")
            {
                DisplayMessage(rockQuestions.First());
                rockQuestions.RemoveFirst();
            }
        }

        private String GetCurrentCategory()
        {
            if (Locations[CurrentPlayerIndex] == 0) return "Pop";
            if (Locations[CurrentPlayerIndex] == 4) return "Pop";
            if (Locations[CurrentPlayerIndex] == 8) return "Pop";
            if (Locations[CurrentPlayerIndex] == 1) return "Science";
            if (Locations[CurrentPlayerIndex] == 5) return "Science";
            if (Locations[CurrentPlayerIndex] == 9) return "Science";
            if (Locations[CurrentPlayerIndex] == 2) return "Sports";
            if (Locations[CurrentPlayerIndex] == 6) return "Sports";
            if (Locations[CurrentPlayerIndex] == 10) return "Sports";
            return "Rock";
        }

        public bool HandleCorrectAnswerFromPlayer()
        {
            if (PlayerInPenaltyBoxState[CurrentPlayerIndex])
            {
                if (!IsGettingOutOfPenaltyBox)
                {
                    CurrentPlayerIndex++;
                    if (CurrentPlayerIndex == Players.Count) CurrentPlayerIndex = FIRST_PLAYER_INDEX;
                    return true;
                }

                DisplayMessage("Answer was correct!!!!");
                PlayersNumberOfGoldenCoinsWon[CurrentPlayerIndex]++;
                DisplayMessage(Players[CurrentPlayerIndex]
                               + " now has "
                               + PlayersNumberOfGoldenCoinsWon[CurrentPlayerIndex]
                               + " Gold Coins.");

                bool notWon = IsPlayerNotYetAWinner();
                CurrentPlayerIndex++;
                if (CurrentPlayerIndex == Players.Count) CurrentPlayerIndex = FIRST_PLAYER_INDEX;

                return notWon;

            }
            else
            {
                DisplayMessage("Answer was correct!!!!");
                PlayersNumberOfGoldenCoinsWon[CurrentPlayerIndex]++;
                DisplayMessage(Players[CurrentPlayerIndex]
                        + " now has "
                        + PlayersNumberOfGoldenCoinsWon[CurrentPlayerIndex]
                        + " Gold Coins.");

                bool notWon = IsPlayerNotYetAWinner();
                CurrentPlayerIndex++;
                if (CurrentPlayerIndex == Players.Count) CurrentPlayerIndex = FIRST_PLAYER_INDEX;

                return notWon;
            }
        }

        public bool HandleIncorrectAnswerFromPlayer()
        {
            DisplayMessage("Question was incorrectly answered");
            DisplayMessage(Players[CurrentPlayerIndex] + " was sent to the penalty box");
            PlayerInPenaltyBoxState[CurrentPlayerIndex] = true;

            CurrentPlayerIndex++;
            if (CurrentPlayerIndex == Players.Count) CurrentPlayerIndex = FIRST_PLAYER_INDEX;
            return true;
        }

        private bool IsPlayerNotYetAWinner()
        {
            return !(PlayersNumberOfGoldenCoinsWon[CurrentPlayerIndex] == WINNING_NUMBER_OF_GOLDEN_COINS);
        }
    }
}
