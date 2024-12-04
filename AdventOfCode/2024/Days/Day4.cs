using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day4 : DayBase
    {
        private const string Xmas = "XMAS";
        private static readonly List<Point> _directionVectors = new List<Point>
        {
            new(-1, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, -1),
            new(0, 1),
            new(1, 0),
            new(1, -1),
            new(1, 1)
        };

        private static readonly List<Point> _xMasVectors = new List<Point>
        {
            new(-1, -1),
            new(-1, 1),
            new(1, -1),
            new(1, 1)
        };

        private static readonly string[] _xMasPatterns = new string[]
        {
            "MMSS","MSMS","SMSM","SSMM"
        };

        public Day4(int year) : base(year) { }

        public override void Run()
        {
            var input = File.ReadAllLines(InputPath);
            var startPoints = GetStartPoints(input, Xmas[0]);

            var allWords = new List<List<Point>>();
            foreach (var point in startPoints)
            {
                var words = GetXmasWords(input, point, Xmas);
                allWords.AddRange(words);
            }

            var wordCount = allWords.Count;
            Console.WriteLine($"Total word count: {wordCount}");

            var part2StartPoints = GetStartPoints(input, 'A');
            var inBoundStartPoints = part2StartPoints.FindAll(x => x.InXmasBounds(input));
            var matchingPoints = inBoundStartPoints.FindAll(x => IsXmasPoint(input, x));
            var xMasWordsCount = matchingPoints.Count;

            Console.WriteLine($"Total x-mas words count: {xMasWordsCount}");
        }

        private static List<List<Point>> GetXmasWords(string[] input, Point point, string word)
        {
            if (!word.Any() || input[point.Row][point.Col] != word[0])
            {
                return new List<List<Point>>();
            }

            var words = new List<List<Point>>();

            foreach (var vector in _directionVectors)
            {
                var letterPoints = new List<Point> { point };

                for (int i = 1; i < word.Length; i++)
                {
                    var pointToCheck = point.Add(vector, i);
                    if (!pointToCheck.InBounds(input))
                        break;

                    var match = input[pointToCheck.Row][pointToCheck.Col] == word[i];
                    if (match)
                    {
                        letterPoints.Add(pointToCheck);
                    }
                }

                if (letterPoints.Count == word.Length)
                {
                    words.Add(letterPoints);
                }
            }

            return words;
        }

        private static List<Point> GetStartPoints(string[] input, char letter)
        {
            var startPoints = new List<Point>();

            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < input[0].Length; j++)
                {
                    if (input[i][j] == letter)
                    {
                        startPoints.Add(new Point(i,j));
                    }
                }
            }

            return startPoints;
        }

        private static bool IsXmasPoint(string[] input, Point point)
        {
            if (input[point.Row][point.Col] != 'A')
                return false;

            var pattern = _xMasVectors.Aggregate(string.Empty, (prev, cur) =>
            {
                var symbolPoint = point.Add(cur);
                var symbol = input[symbolPoint.Row][symbolPoint.Col];
                return prev + symbol;
            });

            var match = _xMasPatterns.Contains(pattern);
            return match;
        }

        private class Point
        {
            public Point(int row, int col)
            {
                Row = row;
                Col = col;
            }
            public int Row { get; set; }
            public int Col { get; set; }

            public bool InBounds(string[] area)
            {
                return area.Length > 1 && Row >= 0 && Col >= 0 && Row < area.Length && Col < area[0].Length;
            }

            public bool InXmasBounds(string[] area)
            {
                return InBounds(area) && _xMasVectors.All(x => Add(x).InBounds(area));
            }

            public Point Add(Point vector, int times = 1) => new(Row + vector.Row * times, Col + vector.Col * times);
        }
    }
}
