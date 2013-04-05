using System.Collections.Generic;
using NUnit.Framework;
using UglyTrivia;

namespace Trivia
{
    [TestFixture]
    public class GameTest
    {
        private const string FIRST_PLAYER_NAME = "FirstPlayerName";

        [Test]
        public void OnAddingPlayerShowPlayerAddedAndNumberOfPlayers()
        {
            var displayedMessages = new List<string>();
            var game = new Game(displayedMessages.Add);
            
            game.AddPlayer(FIRST_PLAYER_NAME);
            
            var expectedMessages = new[]
                                       {
                                           FIRST_PLAYER_NAME + " was added", 
                                           "They are player number 1"
                                       };

            CollectionAssert.AreEqual(expectedMessages, displayedMessages);
        }
    }
}
