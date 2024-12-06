using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day6 : DayBase
    {
        public Day6(int year) : base(year) { }

        private const int StepTreshold = 10_000;
        private const string DirectionMarkers = "^>v<";
        private readonly List<Point> _directionVectors = new()
        {
            new(-1, 0),
            new(0, 1),
            new(1, 0),
            new(0, -1)
        };

        public override void Run()
        {
            var map = File.ReadAllLines(InputPath).Select(x => x.ToList()).ToList();
            var start = GetGuardLocation(map);
            var direction = map[start.Row][start.Col];

            var distinctPositions = GetDistinctPositions(map, start);

            Console.WriteLine($"Guard visited '{distinctPositions}' distinct positions");

            int loopingObstaclesCount = 0;

            // We love brute force <3
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] != '.' && map[i][j] != 'X')
                        continue;

                    map[i][j] = 'O';

                    var positions = GetDistinctPositions(map, start);
                    if (positions == -1)
                        loopingObstaclesCount++;

                    map[i][j] = '.';
                }
            }

            Console.WriteLine($"There are '{loopingObstaclesCount}' possible obstacles that would cause guard to loop");
        }

        private int GetDistinctPositions(List<List<char>> map, Point start)
        {
            int directionStep = 0;
            bool moving = true;
            var point = start;
            int step = 0;
            while (moving)
            {
                step++;
                if (step > StepTreshold) 
                {
                    // loop
                    return -1;
                }

                map[point.Row][point.Col] = 'X';
                var directionVector = GetDirectionVector(directionStep);
                var next = point.Add(directionVector);
                if (!next.InBounds(map))
                {
                    moving = false;
                    break;
                }

                if (map[next.Row][next.Col] == '#' || map[next.Row][next.Col] == 'O')
                {
                    directionStep++;
                    continue;
                }

                point = next;
            }

            var visitedCount = map.Select(x => x.Count(y => y == 'X')).Sum();
            return visitedCount;
        }

        private Point GetDirectionVector(int step)
        {
            return _directionVectors[step % _directionVectors.Count];
        }

        private Point GetGuardLocation(List<List<char>> map)
        {
            for (int i = 0; i < map.Count; i++)
                for (int j = 0; j < map[0].Count; j++)
                    if (DirectionMarkers.Contains(map[i][j]))
                        return new Point(i, j);

            throw new Exception("Cannot find guard on the map");
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

            public bool InBounds(List<List<char>> area)
            {
                return area.Count > 1 && Row >= 0 && Col >= 0 && Row < area.Count && Col < area[0].Count;
            }

            public Point Add(Point vector, int times = 1) => new(Row + vector.Row * times, Col + vector.Col * times);
        }
    }
}
