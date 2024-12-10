using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day10 : DayBase
    {
        public Day10(int year) : base(year) { }

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
            var routes = GetAllRoutes(map);
            var fullRoutes = routes.FindAll(x => x.Peak == 9);
            var trailheadRouteGroups = fullRoutes.GroupBy(x => x.Points[0].ToString()).ToList();

            var totalScore = trailheadRouteGroups.Select(group => group.Select(g => g.Last.ToString()).Distinct().Count()).Sum();
            var totalRating = trailheadRouteGroups.Sum(x => x.Count());

            Console.WriteLine($"Sum of scores of all routes: {totalScore}");
            Console.WriteLine($"Sum of ratings of all routes: {totalRating}");
        }

        private List<Route> GetAllRoutes(List<List<char>> map)
        {
            var result = new List<Route>();

            var startPoints = FindStartPoints(map);
            foreach (var startPoint in startPoints)
            {
                var startRoute = new Route(new List<Point> { startPoint }, map);
                var routes = GetRoutes(startPoint, map, startRoute);
                result.AddRange(routes);
            }

            return result;
        }

        private List<Route> GetRoutes(Point current, List<List<char>> map, Route currentRoute)
        {
            var routes = new List<Route>();

            var nextPoints = GetNextPoints(map, current);
            if (nextPoints.Any())
            {
                var nextRoutes = nextPoints.Select(x => GetRoutes(x, map, currentRoute.Add(x))).ToList();
                foreach (var group in nextRoutes)
                {
                    routes.AddRange(group);
                }
                return routes;
            }

            return new List<Route> { currentRoute };
        }

        private List<Point> GetNextPoints(List<List<char>> map, Point current)
        {
            var currentValue = int.Parse($"{map[current.Row][current.Col]}");
            var nextPoints = _directionVectors
                .Select(x => current.Add(x))
                .Where(x => x.InBounds(map))
                .Where(x => map[x.Row][x.Col] != '.' && int.Parse($"{map[x.Row][x.Col]}") - currentValue == 1)
                .ToList();

            return nextPoints;
        }

        private List<Point> FindStartPoints(List<List<char>> map)
        {
            var points = new List<Point>();

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == '0')
                        points.Add(new Point(i, j));
                }
            }

            return points;
        }

        private class Route
        {
            public Route(List<Point> points, List<List<char>> map)
            {
                Points = points;
                Map = map;
            }
            public Route(List<Point> points, List<List<char>> map, Route path)
            {
                Points = points;
                Map = map;
            }
            public int Peak => Points.Max(x => int.Parse($"{Map[x.Row][x.Col]}"));
            public Point Last => Points.Last();
            public List<Point> Points { get; set; } = new List<Point>();
            public List<List<char>> Map { get; set; } = new List<List<char>>();
            public Route Add(Point point)
            {
                var newRoute = new Route(Points.ToList(), Map);
                newRoute.Points.Add(point);
                return newRoute;
            }
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
