using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day18 : DayBase
    {
        private const bool IsExample = false;
        private readonly int Width, Height, Take;
        private Dictionary<Point,int> _pointCosts = new();

        public Day18(int year) : base(year)
        {
            if (IsExample)
            {
                Width = 7;
                Height = 7;
                Take = 12;
            }
            else
            {
                Width = 71;
                Height = 71;
                Take = 1024;
            }
        }

        public override void Run()
        {
            var allBytes = File.ReadAllLines(InputPath).Select(x => x.Split(",").Select(int.Parse).ToArray()).Select(x => new Point(x[1], x[0])).ToList();
            var bytes = allBytes.Take(Take).ToHashSet();
            var skippedBytes = allBytes.Skip(Take).ToList();
            var map = BuildMap(bytes);
            var routes = GetShortestRoutes(bytes, map);

            Console.WriteLine("--------------- Part 1 ----------------");
            PrintDebugMatrix(map, routes[0].Visited, bytes);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"Shortest route length: {routes[0].Visited.Count - 1}");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("--------------- Part 2 ----------------");
            var byteIndex = Take;
            foreach (var bytePoint in skippedBytes)
            {
                bytes.Add(bytePoint);
                routes = GetShortestRoutes(bytes, map);
                if (routes.Count == 0)
                {
                    Console.WriteLine($"Byte # {byteIndex} is the first one to block the exit! Coordinates(Col,Row): {bytePoint.Col},{bytePoint.Row}");
                    break;
                }
                byteIndex++;
            }
            Console.WriteLine("---------------------------------------");
        }

        private List<Route> GetShortestRoutes(HashSet<Point> bytes, List<List<char>> map)
        {
            _pointCosts = new Dictionary<Point, int>();
            var startPoint = new Point(0, 0);
            var finalRoute = new Route {Head = startPoint, Visited = new HashSet<Point> { startPoint } };

            var queue = new Queue<Route>();
            queue.Enqueue(finalRoute);

            var finalRoutes = new List<Route>();
            while (queue.Any())
            {
                var route = queue.Dequeue();
                var nextRoutes = GetNextRoutes(route, bytes, map);
                if (!nextRoutes.Any())
                    continue; // dead end

                foreach (var nextRoute in nextRoutes)
                {
                    if (nextRoute.IsFinal)
                    {
                        finalRoutes.Add(nextRoute);

                        if (!finalRoute.IsFinal || nextRoute.Cost < finalRoute.Cost)
                        {
                            finalRoute = nextRoute;
                        }

                        continue;
                    }

                    queue.Enqueue(nextRoute);
                }

            }

            return finalRoutes;
        }

        private List<Route> GetNextRoutes(Route route, HashSet<Point> bytes, List<List<char>> map)
        {
            var nextRoutes = new List<Route>();
            foreach (var direction in Directions)
            {
                var nextPoint = route.Head.Add(direction);

                if (route.Visited.Contains(nextPoint)) // already visited
                    continue;

                if (!nextPoint.InBounds(map) || bytes.Contains(nextPoint))
                    continue; // out of bounds or met a wall (byte)

                var newSet = route.Visited.ToHashSet();
                newSet.Add(nextPoint);

                var nextRouteCost = route.Cost + 1;
                var nextRoute = new Route
                {
                    Cost = nextRouteCost,
                    Head = nextPoint,
                    Visited = newSet,
                };

                if (_pointCosts.TryGetValue((nextPoint), out var existingCost) && existingCost <= nextRouteCost)
                {
                    continue;
                }
                else
                {
                    _pointCosts[nextPoint] = nextRouteCost;
                }

                if (nextPoint.Row == Height-1 && nextPoint.Col == Width-1) // end reached
                {
                    nextRoute.IsFinal = true;
                    return new List<Route> { nextRoute };
                }

                nextRoutes.Add(nextRoute);
            }

            return nextRoutes;
        }


        #region misc

        private List<List<char>> BuildMap(HashSet<Point> bytes)
        {
            var usedBytes = bytes.Take(Take).ToList();

            var map = new List<List<char>>();
            for (int i = 0; i < Height; i++)
            {
                map.Add(new List<char>());
                for (int j = 0; j < Width; j++)
                {
                    var symbol = usedBytes.Any(b => b.Row == i && b.Col == j) ? '#' : '.';
                    map[i].Add(symbol);
                }
            }

            return map;
        }

        private static void PrintDebugMatrix(List<List<char>> map, HashSet<Point> route, HashSet<Point> bytes)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    if (route.Contains(new Point(i, j)))
                    {
                        Console.Write('O');
                    }
                    else if (bytes.Contains(new Point(i, j)))
                    {
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }

        private class Route
        {
            public bool IsFinal { get; set; }
            public int Cost { get; set; }
            public Point Head { get; set; }
            public HashSet<Point> Visited { get; set; } = new HashSet<Point>();
            public override string ToString() => string.Join(", ", Visited);
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
            public override bool Equals(object obj) => obj is Point p && Row == p.Row && Col == p.Col;
            public override int GetHashCode() => HashCode.Combine(Row, Col);
        }

        private static readonly List<Point> Directions = new()
        {
            { new Point(0, -1) },
            { new Point(-1, 0) },
            { new Point(0, 1)  },
            { new Point(1, 0)  },
        };

        #endregion
    }
}
