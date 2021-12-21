using System;
using System.Collections.Generic;
using System.Linq;
using common;
using NUnit.Framework;
using NUnit.Framework.Internal;

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
        [TestCase(4, 8, 444356092776315, 341960390180808, 20, TestName="Sample game is won in a bunch of universes", Category="Slow")]
        [TestCase(8, 1, 113467910521040, 116741133558209, 20, TestName="Input game is won in a bunch of universes", Category = "Slow")]
        public void Game_Is_Won_In_A_Bunch_Of_Universes(int startPosA, int startPosB, long expectedA, long expectedB, int maxTurns)
        {
            var wonUniverses = new List<Game>();

            var games = new List<Game>
            {
                new(
                    0,
                    new(1, startPosA, 0, 1),
                    new(2, startPosB, 0, 1)
                )
            };

            int turn = 0;
            while(games.Any(g => g.Player1.Score < 21 && g.Player2.Score < 21 && turn < maxTurns))
            {
                var playerIndex = turn % 2;

                games = games
                    .SelectMany(g =>
                    {
                        var player = playerIndex == 0 ? g.Player1 : g.Player2;
                        var other = playerIndex == 0 ? g.Player2 : g.Player1;
                        return offsets.Select(o =>
                        {
                            var newPos = player.Position + o.offset;
                            newPos = newPos > 10 ? newPos - 10 : newPos;
                            var newPlayer = player with {Position =  newPos, Score = player.Score + newPos, Universes = player.Universes * o.count };
                            var newOther = other with {Universes = other.Universes * o.count};
                            if (playerIndex == 0)
                                return new Game(turn + 1, newPlayer, newOther);
                            return new Game(turn + 1, newOther, newPlayer);
                        });
                    })
                    .ToList();

                wonUniverses.AddRange(games.Where(g => g.Player1.Score >= 21 || g.Player2.Score >= 21));

                games = games
                    .Where(g => g.Player1.Score < 21 && g.Player2.Score < 21)
                    .ToList();

                turn++;

                var totalUniverses = games.Sum(x => x.Player1.Universes) + wonUniverses.Sum(x => x.Player1.Universes);
                Console.WriteLine($"Player {playerIndex + 1}, turn {turn}");
                Console.WriteLine("Total universes: " + totalUniverses);
                Console.WriteLine($"Player 1: {wonUniverses.Where(g => g.Player1.Score >= 21).Sum(g => g.Player1.Universes)} / {wonUniverses.Sum(g => g.Player1.Universes)}");
                Console.WriteLine($"Player 2: {wonUniverses.Where(g => g.Player2.Score >= 21).Sum(g => g.Player2.Universes)} / {wonUniverses.Sum(g => g.Player2.Universes)}");
                Console.WriteLine();
            }

            var totalA = wonUniverses.Where(x => x.Player1.Score > x.Player2.Score).Sum(x => x.Player1.Universes);
            var totalB = wonUniverses.Where(x => x.Player2.Score > x.Player1.Score).Sum(x => x.Player2.Universes);
            Console.WriteLine(totalA);
            Console.WriteLine(totalB);
            Assert.AreEqual(expectedA, totalA);
            Assert.AreEqual(expectedB, totalB);
        }

        private void DumpGames(List<Game> games)
        {
            games.ForEach(g => Console.WriteLine($"{g.Round}: P1: {g.Player1.Position} {g.Player1.Score} / {g.Player1.Universes} P2: {g.Player2.Position} {g.Player2.Score} / {g.Player2.Universes}"));
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

        private static void DumpLeadingScores(IEnumerable<Game> games)
        {
            games
                .Select(g => g.Player1.Score > g.Player2.Score ? g.Player1 : g.Player2)
                .GroupBy(u => (u.Player, u.Score))
                .Select(p => (score:p.Key.Score, player:p.Key.Player, universes:p.Sum(u => u.Universes)))
                .OrderByDescending(x => x)
                .ToList()
                .ForEach(x => Console.WriteLine($"Player: {x.player} Score: {x.score} Universes: {x.universes}"));
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

    public record Game(
        int Round,
        UniversePosition Player1,
        UniversePosition Player2
    );

    public record UniversePosition(int Player, int Position, int Score, long Universes);

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