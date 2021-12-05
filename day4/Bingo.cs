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

            var winStates = FindWinStates(numbers, boards);
            var score = CalculateScore(winStates.First());

            Assert.AreEqual(expected, score);
        }

        [TestCase("day4.sample.txt", 1924)]
        [TestCase("day4.input.txt", 17408)]
        public void Lets_Squid_Win_By_Figuring_Which_Board_Wins_Last(string input, int expected)
        {
            var lines = Resources.GetResourceLines(GetType(), input);

            var numbers = ParseNumbers(lines);
            var boards = ParseBoards(lines).ToList();

            var currentNumber = 0;
            var wins = new List<WinState>();
            while(currentNumber < numbers.Length && boards.Any())
            { 
                var winStates = FindWinStates(numbers.Skip(currentNumber).ToArray(), boards);
                wins.AddRange(winStates);
                foreach (var win in winStates)
                {
                    boards.Remove(win.board);
                };

                currentNumber += winStates.First().index + 1;
            }

            var lastWinIndex = wins.Count - 1;
            var lastWin = wins[lastWinIndex];
            var lastWinBoard = lastWin.board;
            var score = CalculateScore(lastWin);

            Assert.AreEqual(expected, score);
        }

        private static int[] ParseNumbers(string[] lines)
        {
            return lines[0].Split(',').Select(x => ToInt32(x)).ToArray();
        }

        private static List<Board> ParseBoards(string[] lines)
        {
            var boards = new List<Board>();
            var boardNo = 1;
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
                boards.Add(new Board(boardNo++, cells));
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
                    if (!winState.board.Cells[y][x].S)
                    {
                        unmarkedSum += winState.board.Cells[y][x].n;
                    }
                }
            }

            var score = unmarkedSum * winState.lastNumber;
            return score;
        }

        private static List<WinState> FindWinStates(int[] numbers, List<Board> boards)
        {
            List<WinState> wins = new List<WinState>();
            for (int i = 0, n = numbers[i]; i < numbers.Length; i++, n = numbers[i])
            {
                foreach (var board in boards)
                {
                    var cells = board.Cells;
                    for (var y = 0; y < 5; y++)
                    {
                        if (board.Rows[y].HasWon)
                        {
                            continue;
                        }

                        for (var x = 0; x < 5; x++)
                        {
                            if (board.Cols[x].HasWon)
                            {
                                continue;
                            }

                            if (cells[y][x].n == n)
                            {
                                cells[y][x].S = true;
                            }

                            var inCol = 0;
                            for (var y2 = 0; y2 < 5; y2++)
                            {
                                if (cells[y2][x].S)
                                {
                                    inCol++;
                                }
                            }

                            if (inCol == 5)
                            {
                                // We won on column
                                board.Cols[x].HasWon = true;
                                wins.Add(new (board, n, i));
                            }
                        }

                        var inRow = 0;
                        for (var x2 = 0; x2 < 5; x2++)
                        {
                            if (cells[y][x2].S)
                            {
                                inRow++;
                            }
                        }

                        if (inRow == 5)
                        {
                            // We won on row
                            board.Rows[y].HasWon = true;
                            wins.Add(new(board, n, i));
                        }
                    }
                }

                if (wins.Any())
                {
                    return wins;
                }
            }

            throw new Exception("Couldn't find win state");
        }

        private class Board
        {
            public Cell[][] Cells;
            public Set[] Rows;
            public Set[] Cols;
            public readonly int BoardNumber;

            public Board(int boardNumber, Cell[][] cells)
            {
                BoardNumber = boardNumber;
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

            public override string ToString()
            {
                return $"Board #{BoardNumber}";
            }

            public bool IsComplete()
            {
                return Cols.All(x => x.HasWon) && Rows.All(x => x.HasWon);
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

            public override string ToString()
            {
                return $"HasWon: {HasWon}";
            }
        }

        private class WinState
        {
            public Board board;
            public int lastNumber;
            public int index;

            public WinState(Board board, int lastNumber, int index)
            {
                this.board = board;
                this.lastNumber = lastNumber;
                this.index = index;
            }
        }

        private class Cell
        {
            public int n;
            private bool s;

            public Cell(int n, bool s)
            {
                this.n = n;
                this.s = s;
            }

            public bool S
            {
                get => s;
                set => s = value;
            }

            public (int n, bool s) Deconstruct()
            {
                return (n, s);
            }

            public override string ToString()
            {
                return $"N: {n}, Marked: {s}";
            }
        }
    }
}