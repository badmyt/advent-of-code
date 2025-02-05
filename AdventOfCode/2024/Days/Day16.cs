using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day16 : DayBase
    {
        private const int TurnCost = 1000;
        private static readonly Point _initialDirection = new (0, 1); // east
        private static readonly Dictionary<Point, long> _pointCosts = new Dictionary<Point, long>();

        public Day16(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var map = lines.Select(x => x.ToList()).ToList();

            Console.Clear();

            var route = GetShortestRoute(map);

            Print(map);
            //PrintInteractive(route, map);

            Console.WriteLine($"Lowest score (of the shortest path) from start to end is {route.Cost} points");
        }

        private static Route GetShortestRoute(List<List<char>> map)
        {
            var startPoint = FindStartPoint(map);
            var finalRoute = new Route { Head = startPoint, Visited = new HashSet<Point> { startPoint } };
            var queue = new Queue<Route>();
            queue.Enqueue(finalRoute);

            while(queue.Any())
            {
                var route = queue.Dequeue();
                var nextRoutes = GetNextRoutes(route, map);
                if (!nextRoutes.Any())
                    continue; // dead end

                foreach (var nextRoute in nextRoutes)
                {
                    if (nextRoute.IsFinal)
                    {
                        //return nextRoute; // test

                        if (!finalRoute.IsFinal || nextRoute.Cost < finalRoute.Cost)
                        {
                            finalRoute = nextRoute;
                        }

                        continue;
                    }

                    queue.Enqueue(nextRoute);
                }
            }

            return finalRoute;
        }

        private static List<Route> GetNextRoutes(Route route, List<List<char>> map)
        {
            var nextRoutes = new List<Route>();
            var routeDirection = route.Direction;
            foreach (var nextDirection in _directionVectors)
            {
                var prevPoint = route.Head.Add(routeDirection.Invert());
                var nextPoint = route.Head.Add(nextDirection);

                if ((route.Visited.Count > 1) && (prevPoint == nextPoint) || route.Visited.Contains(nextPoint)) // already visited
                    continue;

                if (!nextPoint.InBounds(map) || map[nextPoint.Row][nextPoint.Col] == '#')
                    continue; // out of bounds or met a wall

                var cost = nextDirection == routeDirection
                    ? 1
                    : 1 + TurnCost;

                var newSet = route.Visited.ToHashSet();
                newSet.Add(nextPoint);

                var nextRouteCost = route.Cost + cost;
                var nextRoute = new Route
                {
                    Cost = nextRouteCost,
                    Head = nextPoint,
                    Visited = newSet,
                    Direction = nextDirection
                };

                if (_pointCosts.TryGetValue(nextPoint, out var existingCost) && existingCost < nextRouteCost)
                {
                    continue;
                }
                else
                {
                    _pointCosts[nextPoint] = nextRouteCost;
                }

                if (map[nextPoint.Row][nextPoint.Col] == 'E') // end reached
                {
                    nextRoute.IsFinal = true;
                    return new List<Route> { nextRoute };
                }

                nextRoutes.Add(nextRoute);
            }

            return nextRoutes;
        }

        private static Point FindStartPoint(List<List<char>> map)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == 'S')
                        return new Point(i, j);
                }
            }

            throw new Exception("Start point not found");
        }

        private static void PrintInteractive(Route route, List<List<char>> map)
        {
            var height = map.Count;
            var list = route.Visited.ToList();

            foreach (var point in list)
            {
                Console.SetCursorPosition(point.Col, point.Row);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{map[point.Row][point.Col]}");
                Console.ResetColor();
                Thread.Sleep(100);
            }

            Console.SetCursorPosition(0, height + 1);
        }

        private class Route
        {
            public bool IsFinal { get; set; }
            public int Cost { get; set; }
            public Point Head { get; set; }
            public HashSet<Point> Visited { get; set; } = new HashSet<Point>();
            public Point Direction { get; set; } = _initialDirection;
            public override string ToString() => string.Join(", ", Visited);
        }

        private class Point
        {
            public Point(int row, int col) { Row = row; Col = col; }
            public int Row { get; set; }
            public int Col { get; set; }
            public bool InBounds<T>(List<List<T>> area) => area.Count > 1 && Row >= 0 && Col >= 0 && Row < area.Count && Col < area[0].Count;
            public Point Add(Point vector, int times = 1) => new(Row + vector.Row * times, Col + vector.Col * times);
            public Point Invert() => new(Row * -1, Col * -1);
            public override string ToString() => $"{Row}:{Col}";
            public static Point Parse(string point) => new(int.Parse(point.Split(":")[0]), int.Parse(point.Split(":")[1]));
            public override int GetHashCode() => HashCode.Combine(Row, Col);
            public override bool Equals(object obj) => obj is Point p && Row == p.Row && Col == p.Col; 
        }

        private static readonly List<Point> _directionVectors = new()
        {
            new(-1, 0),
            new(0, 1),
            new(1, 0),
            new(0, -1)
        };
    }
}
