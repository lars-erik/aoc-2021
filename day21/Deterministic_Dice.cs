using System;
using System.Collections.Generic;
using System.Linq;
using common;
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

        [Test]
        public void Solo_Player_Wins_In_A_Bunch_Of_Universes()
        {
            var startPos = 4;

            var wonUniverses = new List<UniversePosition>();

            var universes = new List<UniversePosition>
            {
                new(startPos, 0, 1)
            };

            int i = 0;
            while(universes.Any(u => u.Score < 21) && i < 30)
            {

                universes = universes
                    .SelectMany(u =>
                    {
                        return offsets.Select(o =>
                        {
                            var newPos = u.Position + o.offset;
                            newPos = newPos > 10 ? newPos - 10 : newPos;
                            return new UniversePosition(newPos, u.Score + newPos, u.Universes * o.count * (i > 0 ? 27 : 1));
                        });
                    })
                    .ToList();

                wonUniverses.AddRange(universes.Where(u => u.Score >= 21));

                universes = universes
                    .Where(u => u.Score < 21)
                    .ToList();

                Console.WriteLine($"{++i}:");
                DumpScores(universes);
                wonUniverses.Sum(u => u.Universes).Dump();
                Console.WriteLine();
            }

            var total = wonUniverses.Sum(x => x.Universes);
            Console.WriteLine(total);
            Assert.AreEqual(444356092776315, total);
        }

        private static void DumpScores(IEnumerable<UniversePosition> universes)
        {
            universes
                .GroupBy(u => u.Score)
                .Select(p => (score:p.Key, universes:p.Sum(u => u.Universes)))
                .OrderByDescending(x => x)
                .ToList()
                .ForEach(x => Console.WriteLine($"Score: {x.score} Universes: {x.universes}"));
        }

        private static void DumpUniverses(IEnumerable<UniversePosition> universes)
        {
            universes
                .GroupBy(x => x.Position)
                .SelectMany(pair => { return pair.Select(v => $"Pos {pair.Key}: Score {v.Score} UniCount {v.Universes}"); })
                .ToList()
                .ForEach(Console.WriteLine);
        }

        private List<(int offset, int count)> offsets = new()
        {
            (3, 1),
            (4, 3),
            (5, 6),
            (6, 7),
            (7, 6),
            (8, 3),
            (9, 1)
        };
    }

    public record UniversePosition(int Position, int Score, long Universes);

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