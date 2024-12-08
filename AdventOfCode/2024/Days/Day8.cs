using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day8 : DayBase
    {
        public Day8(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var map = lines.Select(x => x.ToList()).ToList();

            var frequencies = GetFrequencies(map);
            var frequencyPairGroups = GetFrequencyPairGroups(frequencies);
            var antinodesPart1 = GetAntinodes(frequencyPairGroups, map, takeTwo: true);
            var antinodesPart2 = GetAntinodes(frequencyPairGroups, map, takeTwo: false);

            Console.WriteLine($"Part1: There are '{antinodesPart1.Count}' antinodes on the map");
            Console.WriteLine($"Part2: There are '{antinodesPart2.Count}' antinodes on the map");
        }

        private List<Point> GetAntinodes(List<FrequencyPairGroup> frequencyPairGroups, List<List<char>> map, bool takeTwo)
        {
            var antinodes = new List<Point>();

            foreach (var group in frequencyPairGroups)
            {
                var antinodeGroup = new List<Point>();
                foreach (var pair in group.Pairs)
                {
                    var antinodeLine = GetAntinodeLine(pair, map, takeTwo);
                    antinodeGroup.AddRange(antinodeLine);
                }

                antinodes.AddRange(antinodeGroup);
            }

            var uniqueAntinodes = antinodes.Select(x => x.ToString()).Distinct().Select(Point.Parse).ToList();
            return uniqueAntinodes;
        }

        private List<Point> GetAntinodeLine(FrequencyPair frequencyPair, List<List<char>> map, bool takeTwo)
        {
            var antinodes = new List<Point>();

            var p1 = frequencyPair.One.Point;
            var p2 = frequencyPair.Two.Point;

            bool InBounds(int row, int col)
            {
                return row >= 0 && col >= 0 && row < map.Count && col < map[0].Count;
            }

            if (!takeTwo)
            {
                antinodes.Add(p1);
                antinodes.Add(p2);
            }

            // add antinodes before p1
            var rowDiff = p2.Row - p1.Row;
            var colDiff = p2.Col - p1.Col;

            var row = p1.Row;
            var col = p1.Col;
            while (InBounds(row,col))
            {
                row -= rowDiff;
                col -= colDiff;

                if (InBounds(row,col))
                    antinodes.Add(new Point(row, col));

                if (takeTwo)
                    break;
            }

            // add antinodes after p2
            rowDiff = p2.Row - p1.Row;
            colDiff = p2.Col - p1.Col;

            row = p2.Row;
            col = p2.Col;
            while (InBounds(row, col))
            {
                row += rowDiff;
                col += colDiff;

                if (InBounds(row, col))
                    antinodes.Add(new Point(row, col));

                if (takeTwo)
                    break;
            }

            // validate just in case
            var antinodesInBounds = antinodes.FindAll(x => x.InBounds(map));
            return antinodesInBounds;
        }

        private List<FrequencyPairGroup> GetFrequencyPairGroups(List<Frequency> frequencies)
        {
            var result = new List<FrequencyPairGroup>();

            var grouped = frequencies.GroupBy(x => x.Symbol);
            foreach (var group in grouped)
            {
                var pairGroup = new List<FrequencyPair>();
                var groupFrequencies = group.ToList();

                for (int i = 0; i < groupFrequencies.Count; i++)
                {
                    var frequencyOne = groupFrequencies[i];
                    for (int j = i + 1; j < groupFrequencies.Count; j++)
                    {
                        var frequencyTwo = groupFrequencies[j];
                        pairGroup.Add(new FrequencyPair(frequencyOne, frequencyTwo));
                    }
                }

                result.Add(new FrequencyPairGroup(group.Key, pairGroup));
            }

            return result;
        }

        private List<Frequency> GetFrequencies(List<List<char>> map)
        {
            var result = new List<Frequency>();

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    var symbol = map[i][j];
                    if (symbol != '.')
                        result.Add(new Frequency(i, j, symbol));
                }
            }

            return result;
        }

        private class FrequencyPairGroup
        {
            public FrequencyPairGroup(char symbol, List<FrequencyPair> pairs)
            {
                Symbol = symbol;
                Pairs = pairs;
            }

            public char Symbol { get; set; }

            public List<FrequencyPair> Pairs { get; set; }
        }

        private class FrequencyPair
        {
            public FrequencyPair(Frequency one, Frequency two)
            {
                One = one; Two = two;
            }
            public Frequency One { get; set; }
            public Frequency Two { get; set; }
        }

        private class Frequency
        {
            public Frequency(int row, int col, char symbol)
            {
                Point = new Point(row, col);
                Symbol = symbol;
            }

            public char Symbol { get; set; }
            public Point Point { get; set; }
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

            public bool InBounds<T>(List<List<T>> area)
            {
                return area.Count > 1 && Row >= 0 && Col >= 0 && Row < area.Count && Col < area[0].Count;
            }

            public Point Add(Point vector, int times = 1) => new(Row + vector.Row * times, Col + vector.Col * times);

            public override string ToString()
            {
                return ($"{Row}:{Col}");
            }

            public static Point Parse(string point)
            {
                var row = int.Parse(point.Split(":")[0]);
                var col = int.Parse(point.Split(":")[1]);
                return new Point(row, col);
            }
        }
    }
}
