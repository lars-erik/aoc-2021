using System;
using System.Linq;
using NUnit.Framework;

namespace day21
{
    [TestFixture]
    public class Deterministic_Dice
    {
        [Test]
        [TestCase(4, 8, 739785, TestName = "Wins in sample")]
        [TestCase(8, 1, 518418, TestName = "Wins in input")]
        public void Wins(int startPos1, int startPos2, int expectedResult)
        {
            var players = new[]
            {
                new Player(startPos1),
                new Player(startPos2),
            };
            var playerIndex = 1;
            var round = 0;
            var nextThrow = 1;


            while (players.All(p => p.Score < 1000))
            {
                round++;
                
                playerIndex = (playerIndex + 1) % 2;
                var player = players[playerIndex];
                var sum = 0;
                for (var roll = 0; roll < 3; roll++)
                {
                    sum += nextThrow;
                    nextThrow = nextThrow + 1;
                    if (nextThrow == 101) nextThrow = 1;
                }

                player.Position += sum;
                while (player.Position > 10)
                    player.Position -= 10;

                player.Score += player.Position;
            }

            Console.WriteLine(round);
            Console.WriteLine($"Player 1 at {players[0].Position} with score {players[0].Score}");
            Console.WriteLine($"Player 2 at {players[1].Position} with score {players[1].Score}");

            var minScore = players.Min(x => x.Score);
            var result = minScore * round * 3;

            Assert.AreEqual(expectedResult, result);
        }
    }

    public class Player
    {
        public int Score;
        public int Position;

        public Player(int startPosition)
        {
            Position = startPosition;
        }
    }
}