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

        public Day16(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var map = lines.Select(x => x.ToList()).ToList();

            Console.Clear();

            var route = GetShortestRoute(map);

            Print(map);
            PrintInteractive(route, map);

            Console.WriteLine($"Lowest score (of the shortest path) from start to end is {route.Cost} points");
        }

        private static Route GetShortestRoute(List<List<char>> map)
        {
            var startPoint = FindStartPoint(map);
            var finalRoute = new Route();
            finalRoute.Points.Push(startPoint);
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
                        return nextRoute; // test

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
            var p = route.Points.Peek();
            var routeDirection = route.Direction;
            foreach (var nextDirection in _directionVectors)
            {
                var prevPoint = p.Add(routeDirection.Invert());
                var nextPoint = p.Add(nextDirection);
                if (($"{prevPoint}" == $"{nextPoint}") || route.Visited.Contains($"{nextPoint}")) // already visited
                    continue;

                if (!nextPoint.InBounds(map) || map[nextPoint.Row][nextPoint.Col] == '#')
                    continue; // out of bounds or met a wall

                var cost = ($"{nextDirection}" == $"{routeDirection}")
                    ? 1
                    : 1 + TurnCost;

                var newStack = new Stack<Point>(new Stack<Point>(route.Points));
                newStack.Push(nextPoint);

                var nextRoute = new Route
                {
                    Cost = route.Cost + cost,
                    Points = newStack,
                    Visited = newStack.Select(x => $"{x}").ToHashSet(),
                    Direction = nextDirection
                };

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
            var list = route.Points.Reverse().ToList();

            foreach (var point in list)
            {
                Console.SetCursorPosition(point.Col, point.Row); // Position cursor (2 for spacing)
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{map[point.Row][point.Col]}"); // Highlight current cell
                Console.ResetColor();
                Thread.Sleep(100); // 0.1-second delay
            }

            Console.SetCursorPosition(0, height + 1);
        }

        //private List<List<Point>> GetPointPool(List<List<char>> map)
        //{
        //    var lines = new List<List<Point>>();
        //    for (int i = 0; i < map.Count; i++)
        //    {
        //        var line = new List<Point>(map[i].Count);
        //        for (int j = 0; j < map[i].Count; j++)
        //        {
        //            line[j] = new Point(i, j);
        //        }
        //        lines.Add(line);
        //    }
        //    return lines;
        //}

        private class Route
        {
            public bool IsFinal { get; set; }
            public int Cost { get; set; }
            public Stack<Point> Points { get; set; } = new Stack<Point>();
            public HashSet<string> Visited { get; set; } = new HashSet<string>();
            public Point Direction { get; set; } = _initialDirection;
            public override string ToString() => string.Join(", ", Points);
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
