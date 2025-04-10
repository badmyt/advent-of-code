using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day20 : DayBase
    {
        private const int CheatPicosecondsThreshold = 100;
        private const int CheatPicosecondsThresholdPartTwo = 50;
        private const int CheatPicosecondsLengthPartTwo = 6;

        public Day20(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var map = lines.Select(x => x.ToList()).ToList();

            Console.Clear();

            Reverse(map);
            var reversedResult = GetShortestRoutes(map);
            var pointCostsFromEnd = reversedResult.PointCosts;
            Reverse(map);

            var legalShortestTime = reversedResult.Final.Cost;
            var cheats = GetCheats(map, pointCostsFromEnd, legalShortestTime);

            Print(map);
            PrintInteractive(reversedResult.Final, map, reversedResult.Final.Visited, cheats);

            var cheatsPartTwo = GetCheatsPartTwo(map, pointCostsFromEnd, legalShortestTime);
            var cheatsPartTwoGrouped = cheatsPartTwo.GroupBy(x => x.PicosecondsSaved).Select(x => (x.Key, x.Count())).OrderByDescending(x => x.Key).ToList();

            Console.WriteLine($"Shortest time from start to end is {reversedResult.Final.Cost} picoseconds");
            Console.WriteLine($"There are '{cheats.Count}' cheats which save {CheatPicosecondsThreshold} picoseconds or more");

            Console.WriteLine($"Part 2: There are '{cheatsPartTwo.Count}' cheats which save {CheatPicosecondsThresholdPartTwo} picoseconds or more");

        }

        private static List<(Point PointA, Point PointB, int PicosecondsSaved)> GetCheatsPartTwo(List<List<char>> map, Dictionary<Point, int> pointCostsFromEnd, int legalShortestTime)
        {
            var startPoint = FindStartPoint(map);
            var pointCosts = new Dictionary<Point, int> { { startPoint, 0 } };
            var finalRoute = new Route { Head = startPoint, Visited = new HashSet<Point> { startPoint } };
            var queue = new Queue<Route>();
            var cheatQueue = new Queue<(Point Point, int Cost, Point CheatPoint)>();
            queue.Enqueue(finalRoute);

            var finalRoutes = new List<Route>();

            while (queue.Any())
            {
                var route = queue.Dequeue();
                var cheatPoints = GetCheatPointsPartTwo(route.Head, route, map);
                foreach (var cheatPoint in cheatPoints)
                    cheatQueue.Enqueue((route.Head, route.Cost, cheatPoint));

                var nextRoutes = GetNextRoutes(route, map, pointCosts);
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

            var validCheats = new List<(Point Point, Point CheatPoint, int SavedTime)>();

            while (cheatQueue.Any())
            {
                var item = cheatQueue.Dequeue();
                if (!pointCostsFromEnd.ContainsKey(item.CheatPoint))
                    continue;

                var cheatPointCost = pointCostsFromEnd[item.CheatPoint];
                var routePointCost = item.Cost;
                var distance = Math.Abs(item.CheatPoint.Row - item.Point.Row) + Math.Abs(item.CheatPoint.Col - item.Point.Col);
                var totalCost = routePointCost + cheatPointCost + distance;
                var savedCost = legalShortestTime - totalCost;

                if (savedCost >= CheatPicosecondsThresholdPartTwo)
                {
                    validCheats.Add((item.Point, item.CheatPoint, savedCost));
                }
            }

            return validCheats;
        }

        private static List<Point> GetCheatPointsPartTwo(Point current, Route route, List<List<char>> map)
        {
            var result = new List<Point>();

            var minCol = current.Col - CheatPicosecondsLengthPartTwo <= 0 ? 0 : current.Col - CheatPicosecondsLengthPartTwo;
            var minRow = current.Row - CheatPicosecondsLengthPartTwo <= 0 ? 0 : current.Row - CheatPicosecondsLengthPartTwo;

            var maxCol = current.Col + CheatPicosecondsLengthPartTwo >= map[0].Count ? map[0].Count - 1 : current.Col + CheatPicosecondsLengthPartTwo;
            var maxRow = current.Row + CheatPicosecondsLengthPartTwo >= map.Count ? map.Count - 1 : current.Row + CheatPicosecondsLengthPartTwo;

            for (var col = minCol; col <= maxCol; col++)
            {
                for (var row = minRow; row <= maxRow; row++)
                {
                    var symbol = map[row][col];
                    if (symbol != 'E' && symbol != '.')
                        continue;

                    var point = new Point(row, col);
                    result.Add(point);
                }
            }

            return result;
        }


        private static List<(Point Point, int PicosecondsSaved)> GetCheats(List<List<char>> map, Dictionary<Point,int> pointCostsFromEnd, int legalShortestTime)
        {
            var startPoint = FindStartPoint(map);
            var pointCosts = new Dictionary<Point, int> { { startPoint, 0 } };
            var finalRoute = new Route { Head = startPoint, Visited = new HashSet<Point> { startPoint } };
            var queue = new Queue<Route>();
            var cheatQueue = new Queue<(Point Point, int Cost, Point WallPoint, Point CheatPoint)>();
            queue.Enqueue(finalRoute);

            var finalRoutes = new List<Route>();

            while (queue.Any())
            {
                var route = queue.Dequeue();
                var cheatPoints = GetCheatPoints(route.Head, route.Direction, map);
                foreach (var cheatPoint in cheatPoints)
                    cheatQueue.Enqueue((route.Head, route.Cost, cheatPoint.WallPoint, cheatPoint.CheatPoint));

                var nextRoutes = GetNextRoutes(route, map, pointCosts);
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

            var validCheats = new List<(Point Point, int SavedTime)>();

            while (cheatQueue.Any())
            {
                var item = cheatQueue.Dequeue();
                if (!pointCostsFromEnd.ContainsKey(item.CheatPoint))
                    continue;

                var cheatPointCost = pointCostsFromEnd[item.CheatPoint];
                var routePointCost = item.Cost;
                var totalCost = routePointCost + cheatPointCost + 2;

                var savedCost = legalShortestTime - totalCost;

                if (savedCost >= CheatPicosecondsThreshold)
                {
                    validCheats.Add((item.WallPoint, savedCost));
                }
            }

            return validCheats;
        }

        private static List<(Point WallPoint, Point CheatPoint)> GetCheatPoints(Point current, Point direction, List<List<char>> map)
        {
            var result = new List<(Point WallPoint, Point CheatPoint)>();
            foreach (var d in Directions)
            {
                if (direction != null && d.Invert() == direction)
                    continue;

                var wallPoint = current.Add(d);
                if (wallPoint.InBounds(map))
                {
                    var isWall = map[wallPoint.Row][wallPoint.Col] == '#';
                    if (!isWall)
                        continue;

                    var nextPoint = wallPoint.Add(d);
                    if (!nextPoint.InBounds(map))
                        continue;

                    var nextSymbol = map[nextPoint.Row][nextPoint.Col];
                    if (nextSymbol != '#' && nextSymbol != 'S')
                        result.Add((wallPoint, nextPoint));
                }
            }
            return result;
        }

        private static (Route Final, List<Route> All, Dictionary<Point,int> PointCosts) GetShortestRoutes(List<List<char>> map)
        {
            var startPoint = FindStartPoint(map);
            var pointCosts = new Dictionary<Point, int> { { startPoint, 0 } };
            var finalRoute = new Route { Head = startPoint, Visited = new HashSet<Point> { startPoint } };
            var queue = new Queue<Route>();
            queue.Enqueue(finalRoute);

            var finalRoutes = new List<Route>();

            while (queue.Any())
            {
                var route = queue.Dequeue();
                var nextRoutes = GetNextRoutes(route, map, pointCosts);
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

            return (finalRoute, finalRoutes, pointCosts);
        }

        private static List<Route> GetNextRoutes(Route route, List<List<char>> map, Dictionary<Point, int> pointCosts)
        {
            var nextRoutes = new List<Route>();
            var routeDirection = route.Direction;
            var prevPoint = route.Direction == null ? null : route.Head.Add(routeDirection.Invert());

            foreach (var nextDirection in Directions)
            {
                var nextPoint = route.Head.Add(nextDirection);

                if ((route.Visited.Count > 1) && (prevPoint == nextPoint) || route.Visited.Contains(nextPoint)) // already visited
                    continue;

                if (!nextPoint.InBounds(map) || map[nextPoint.Row][nextPoint.Col] == '#')
                    continue; // out of bounds or met a wall

                var newSet = route.Visited.ToHashSet();
                newSet.Add(nextPoint);

                var nextRouteCost = route.Cost + 1;
                var nextRoute = new Route
                {
                    Cost = nextRouteCost,
                    Head = nextPoint,
                    Visited = newSet,
                    Direction = nextDirection
                };

                if (pointCosts.TryGetValue(nextPoint, out var existingCost) && existingCost < nextRouteCost)
                {
                    continue;
                }
                else
                {
                    pointCosts[nextPoint] = nextRouteCost;
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

        #region misc

        private static void Reverse(List<List<char>> map)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == 'S')
                    {
                        map[i][j] = 'E';
                    }
                    else if (map[i][j] == 'E')
                    {
                        map[i][j] = 'S';
                    }
                }
            }
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

        private static void PrintInteractive(Route route, List<List<char>> map, HashSet<Point> watchPoints, List<(Point Point, int Time)> cheats)
        {
            var height = map.Count;
            if (height > 50)
            {
                return;
            }

            foreach (var point in watchPoints)
            {
                Console.SetCursorPosition(point.Col, point.Row);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"O");
                Console.ResetColor();
                Thread.Sleep(20);
            }

            foreach (var cheat in cheats)
            {
                Console.SetCursorPosition(cheat.Point.Col, cheat.Point.Row);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"$");
                Console.ResetColor();
                Thread.Sleep(50);
            }

            Console.SetCursorPosition(0, height + 1);
        }

        private class Route
        {
            public bool IsFinal { get; set; }
            public int Cost { get; set; }
            public Point Head { get; set; }
            public HashSet<Point> Visited { get; set; } = new HashSet<Point>();
            public Point Direction { get; set; }
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

        private static readonly List<Point> Directions = new()
        {
            new Point(0, -1),
            new Point(-1, 0),
            new Point(0, 1),
            new Point(1, 0)
        };

        #endregion
    }
}
