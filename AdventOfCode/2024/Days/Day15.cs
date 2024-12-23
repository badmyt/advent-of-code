using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day15 : DayBase
    {
        public Day15(int year) : base(year) { }

        public override void Run()
        {
            var linesPart1 = File.ReadAllLines(InputPath);
            var mapSizePart1 = linesPart1[0].Length;
            var mapPart1 = linesPart1.Take(mapSizePart1).Select(x => x.ToList()).ToList();
            var moves = linesPart1.Skip(mapSizePart1 + 1).ToArray().Aggregate("", (prev,cur) => $"{prev}{cur}");
            var robotPos = FindRobot(mapPart1);

            Console.WriteLine("Initial state:");
            for (int i = 0; i < moves.Length; i++)
            {
                var move = moves[i];
                robotPos = TryMoveRobot(mapPart1, robotPos, move);
            }

            var boxes = FindAllBoxes(mapPart1);
            var lanternfishSum = GetLanternfishSum(boxes);

            Console.WriteLine($"Sum of gps coordinates of all boxes for the lanternfish: {lanternfishSum}");

            // part2
            var map = GetExpandedMap(File.ReadAllLines(InputPath));
            robotPos = FindRobot(map);
            var mapWidth = map[0].Count;
            var mapHeight = map.Count;

            Console.WriteLine("Initial state:");
            PrintDebugMatrix(map);
            for (int i = 0; i < moves.Length; i++)
            {
                var move = moves[i];
                robotPos = TryMoveRobot(mapPart1, robotPos, move, part2: true);

                Console.WriteLine($"State after {i+1} moves: {move}");
                PrintDebugMatrix(map);
            }
        }

        private Point TryMoveRobot(List<List<char>> map, Point point, char directionSymbol, bool part2 = false)
        {
            var direction = Directions[directionSymbol];
            var nextPoint = point.Add(direction);
            var nextSymbol = map[nextPoint.Row][nextPoint.Col];
            if (nextSymbol == '#')
            {
                return point; // cannot move
            }

            if (nextSymbol == '.')
            {
                map[nextPoint.Row][nextPoint.Col] = '@';
                map[point.Row][point.Col] = '.';
                return nextPoint; // moved
            }

            if (part2 && TryMoveBox_part2(map, nextPoint, direction, '@') || TryMoveBox_part1(map, nextPoint, direction, '@'))
            {
                map[nextPoint.Row][nextPoint.Col] = '@';
                map[point.Row][point.Col] = '.';
                return nextPoint; // moved
            }

            return point; // cannot move 
        }

        private bool TryMoveBox_part2(List<List<char>> map, Point point, Point direction, char previousSymbol)
        {
            var symbol = map[point.Row][point.Col];
            if (symbol != '[' && symbol != ']')
            {
                throw new Exception($"Point {point} is not a box!");
            }

            var nextBox = point.Add(direction);
            var nextPoint = map[nextBox.Row][nextBox.Col];
            if (nextPoint == '#')
            {
                return false; // wall, cannot move
            }
            if (nextPoint == '.')
            {
                map[nextBox.Row][nextBox.Col] = symbol;
                map[point.Row][point.Col] = 'O';
                return true; //empty space, moved
            }

            var isNextMoved = TryMoveBox_part1(map, nextBox, direction, previousSymbol);
            if (isNextMoved)
            {
                map[nextBox.Row][nextBox.Col] = 'O';
                map[point.Row][point.Col] = previousSymbol;
                return true; //stack of boxes moved
            }
            return false; // stack of boxes is near the wall
        }

        private bool TryMoveBox_part1(List<List<char>> map, Point point, Point direction, char previousSymbol)
        {
            var symbol = map[point.Row][point.Col];
            if (symbol != 'O')
            {
                throw new Exception($"Point {point} is not a box!");
            }

            var nextBox = point.Add(direction);
            var nextPoint = map[nextBox.Row][nextBox.Col];
            if (nextPoint == '#')
            {
                return false; // wall, cannot move
            }
            if (nextPoint == '.')
            {
                map[nextBox.Row][nextBox.Col] = symbol;
                map[point.Row][point.Col] = 'O';
                return true; //empty space, moved
            }

            var isNextMoved = TryMoveBox_part1(map, nextBox, direction, previousSymbol);
            if (isNextMoved)
            {
                map[nextBox.Row][nextBox.Col] = 'O';
                map[point.Row][point.Col] = previousSymbol;
                return true; //stack of boxes moved
            }
            return false; // stack of boxes is near the wall
        }

        private Point FindRobot(List<List<char>> map)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == '@')
                        return new Point(i, j);
                }
            }

            return null;
        }

        private static List<List<char>> GetExpandedMap(string[] lines)
        {
            var result = new List<List<char>>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = new List<char>();
                for (int j = 0; j < lines[i].Length; j++)
                {
                    var symbol = lines[i][j];
                    if (symbol == '#')
                    {
                        line.Add('#');
                        line.Add('#');
                    }
                    else if (symbol == '.')
                    {
                        line.Add('.');
                        line.Add('.');
                    }
                    else if (symbol == 'O')
                    {
                        line.Add('[');
                        line.Add(']');
                    }
                    else if (symbol == '@')
                    {
                        line.Add('@');
                        line.Add('.');
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                result.Add(line);
            }

            return result;
        }

        private List<Point> FindAllBoxes(List<List<char>> map)
        {
            var result = new List<Point>();

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == 'O')
                    {
                        result.Add(new Point(i, j));
                    }
                }
            }

            return result;
        }

        private long GetLanternfishSum(List<Point> boxes)
        {
            long sum = 0;

            foreach (var box in boxes)
            {
                sum += 100 * box.Row + box.Col;
            }

            return sum;
        }

        private static void PrintDebugMatrix(List<List<char>> map)
        {
            var mapSize = map[0].Count;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Console.Write(map[i][j]);
                }
                Console.WriteLine();
            }
        }

        private class Point
        {
            public Point(int row, int col) { Row = row; Col = col; }
            public int Row { get; set; }
            public int Col { get; set; }
            public bool InBounds<T>(List<List<T>> area) => area.Count > 1 && Row >= 0 && Col >= 0 && Row < area.Count && Col < area[0].Count;
            public Point Add(Point vector, int times = 1) => new(Row + vector.Row * times, Col + vector.Col * times);
            public override string ToString() => $"{Row}:{Col}";
            public static Point Parse(string point) => new(int.Parse(point.Split(":")[0]), int.Parse(point.Split(":")[1]));
        }

        private static readonly List<Point> DirectionVectors = new()
        {
            new(0, -1), // <
            new(-1, 0), // ^
            new(0, 1),  // >
            new(1, 0),  // v
        };

        private static readonly Dictionary<char, Point> Directions = new()
        {
            { '<', DirectionVectors[0] },
            { '^', DirectionVectors[1] },
            { '>', DirectionVectors[2] },
            { 'v', DirectionVectors[3] },
        };
    }
}
