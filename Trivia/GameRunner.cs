using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UglyTrivia;

namespace Trivia
{
    public class GameRunner
    {
        private static bool notAWinner;

        public static void Main(String[] args)
        {
            Game aGame = new Game();

            aGame.AddPlayer("Chet");
            aGame.AddPlayer("Pat");
            aGame.AddPlayer("Sue");

            Random rand = new Random();

            do
            {
                aGame.UpdateLocationBasedOnPenaltyBoxStateAndAskQuestionWhenNotInPenaltyBox(rand.Next(5) + 1);

                if (rand.Next(9) == 7)
                {
                    notAWinner = aGame.HandleIncorrectAnswerFromPlayer();
                }
                else
                {
                    notAWinner = aGame.HandleCorrectAnswerFromPlayer();
                }

            } while (notAWinner);

        }
    }
}

