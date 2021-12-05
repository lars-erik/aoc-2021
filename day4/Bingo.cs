using System;
using System.Collections.Generic;
using System.Linq;
using common;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using static System.Convert;

namespace day4
{
    public class Bingo
    {
        [TestCase("day4.sample.txt", 4512)]
        [TestCase("day4.input.txt", 50008)]
        public void Is_Guaranteed_Victory_By_Figuring_Which_Board_Wins_First(string input, int expected)
        {
            var lines = Resources.GetResourceLines(GetType(), input);

            var numbers = ParseNumbers(lines);
            var boards = ParseBoards(lines);

            var winState = FindWinState(numbers, boards);
            var score = CalculateScore(winState);

            Assert.AreEqual(expected, score);
        }

        [TestCase("day4.sample.txt", 1924)]
        //[TestCase("day4.input.txt", 0)]
        public void Lets_Squid_Win_By_Figuring_Which_Board_Wins_Last(string input, int expected)
        {
            var lines = Resources.GetResourceLines(GetType(), input);

            var numbers = ParseNumbers(lines);
            var boards = ParseBoards(lines);

            var currentNumber = 0;
            var wins = new List<WinState>();
            while(currentNumber < numbers.Length)
            { 
                var winState = FindWinState(numbers.Skip(currentNumber).ToArray(), boards);
                wins.Add(winState);
                currentNumber += winState.index + 1;
            }

            var score = 0;

            Assert.AreEqual(expected, score);
        }

        private static int[] ParseNumbers(string[] lines)
        {
            return lines[0].Split(',').Select(x => ToInt32(x)).ToArray();
        }

        private static List<Board> ParseBoards(string[] lines)
        {
            var boards = new List<Board>();
            for (var i = 2; i < lines.Length; i++)
            {
                var cells = new Cell[5][];
                for (var j = 0; j < 5; j++)
                {
                    var boardLine = lines[i + j].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => new Cell(ToInt32(x), false)).ToArray();
                    cells[j] = boardLine;
                }

                i += 5;
                boards.Add(new Board(cells));
            }

            return boards;
        }

        private static int CalculateScore(WinState winState)
        {
            var unmarkedSum = 0;
            for (var y = 0; y < 5; y++)
            {
                for (var x = 0; x < 5; x++)
                {
                    if (!winState.board.Cells[y][x].s)
                    {
                        unmarkedSum += winState.board.Cells[y][x].n;
                    }
                }
            }

            var score = unmarkedSum * winState.lastNumber;
            return score;
        }

        private static WinState FindWinState(int[] numbers, List<Board> boards)
        {
            for (int i = 0, n = numbers[i]; i < numbers.Length; i++, n = numbers[i])
            {
                foreach (var board in boards)
                {
                    var cells = board.Cells;
                    for (var y = 0; y < 5; y++)
                    {
                        for (var x = 0; x < 5; x++)
                        {
                            if (cells[y][x].n == n)
                            {
                                cells[y][x] = cells[y][x] with {s=true};
                            }

                            var inCol = 0;
                            for (var y2 = 0; y2 < 5; y2++)
                            {
                                if (cells[y2][x].s)
                                {
                                    inCol++;
                                }
                            }

                            if (inCol == 5)
                            {
                                // We won on column
                                board.Cols[x].HasWon = true;
                                return new (board, n, i);
                            }
                        }

                        var inRow = 0;
                        for (var x2 = 0; x2 < 5; x2++)
                        {
                            if (cells[y][x2].s)
                            {
                                inRow++;
                            }
                        }

                        if (inRow == 5)
                        {
                            // We won on row
                            board.Rows[y].HasWon = true;
                            return new(board, n, i);
                        }
                    }
                }
            }

            throw new Exception("Couldn't find win state");
        }

        private class Board
        {
            public Cell[][] Cells;
            public Set[] Rows;
            public Set[] Cols;

            public Board(Cell[][] cells)
            {
                Cells = cells;
                Rows = new Set[5];
                Cols = new Set[5];
                for (var y = 0; y < 5; y++)
                {
                    Rows[y] = new Set(cells[y]);
                }

                for (var x = 0; x < 5; x++)
                {
                    Cols[x] = new Set(Enumerable.Range(0, 5).Select(y => cells[y][x]).ToArray());
                }
            }
        }

        private class Set
        {
            public Cell[] Cells;
            public bool HasWon;

            public Set(Cell[] cells)
            {
                Cells = cells;
            }
        }

        private record WinState(Board board, int lastNumber, int index)
        {
        }

        private record Cell(int n, bool s)
        {
            public (int n, bool s) Deconstruct()
            {
                return (n, s);
            }
        }
    }
}