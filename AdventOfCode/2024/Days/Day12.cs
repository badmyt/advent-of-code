using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day12 : DayBase
    {
        public Day12(int year) : base(year) { }

        public override void Run()
        {
            var map = File.ReadAllLines(InputPath).Select(x => x.ToList()).ToList();

            var points = GetAllPoints(map);
            var regions = GetAllRegions(points, map);

            CalculateSidesForRegions(regions, map);

            var totalPriceByPerimeter = regions.Sum(x => x.GetPriceByPerimeter());
            var totalPriceBySides = regions.Sum(x => x.GetPriceBySides());

            Console.WriteLine($"Total price of fencing all regions: {totalPriceByPerimeter}");
            Console.WriteLine($"Total price of fencing all regions (with discount): {totalPriceBySides}");
        }

        private List<Region> GetAllRegions(List<Point> points, List<List<char>> map)
        {
            var result = new List<Region>();
            var visitedPoints = new HashSet<string>();

            var groupedPoints = points.GroupBy(x => map[x.Row][x.Col]);
            foreach (var group in groupedPoints)
            {
                var regions = new List<Region>();

                foreach (var point in group)
                {
                    if (visitedPoints.Contains($"{point}"))
                        continue;

                    var (Points, Perimeter) = GetRegionPoints(point, map, visitedPoints);
                    if (Points.Any())
                    {
                        regions.Add(new Region(Points, Perimeter, group.Key, map));
                    }
                }

                result.AddRange(regions);
            }


            return result;
        }

        private (List<Point> Points, int Perimeter) GetRegionPoints(Point currentPoint, List<List<char>> map, HashSet<string> visitedPoints)
        {
            if (visitedPoints.Contains($"{currentPoint}"))
                return (new List<Point>(), 0);

            var resultPoints = new List<Point> { currentPoint };
            var type = map[currentPoint.Row][currentPoint.Col];
            visitedPoints.Add($"{currentPoint}");

            var adjacentPointsSameType = _directionVectors
                .Select(x => currentPoint.Add(x))
                .Where(x => x.InBounds(map) && map[x.Row][x.Col] == type && !visitedPoints.Contains($"{x}"))
                .ToList();

            var adjacentPointsOtherType = _directionVectors
                .Select(x => currentPoint.Add(x))
                .Where(x => !x.InBounds(map) || map[x.Row][x.Col] != type)
                .ToList();

            var perimeter = adjacentPointsOtherType.Count;
            if (adjacentPointsSameType.Any())
            {
                var nextPoints = adjacentPointsSameType.Select(x => GetRegionPoints(x, map, visitedPoints)).ToList();
                foreach (var group in nextPoints)
                {
                    resultPoints.AddRange(group.Points);
                    perimeter += group.Perimeter;
                }
            }

            return (Points: resultPoints, Perimeter: perimeter);
        }

        private void CalculateSidesForRegions(List<Region> regions, List<List<char>> map)
        {
            foreach (var region in regions)
            {
                var regionPoints = new HashSet<string>(region.Points.Select(x => $"{x}"));

                var minRow = region.Points.Min(x => x.Row);
                var minCol = region.Points.Min(x => x.Col);
                var maxRow = region.Points.Max(x => x.Row);
                var maxCol = region.Points.Max(x => x.Col);

                var totalSides = 0;

                for (var row = minRow; row <= maxRow; row++)
                {
                    var connectedGroupsTopFence = GetConnectedPointGroupsOnSameRow(region, regionPoints, row, fenceOnTop: true);
                    var connectedGroupsBotFence = GetConnectedPointGroupsOnSameRow(region, regionPoints, row, fenceOnTop: false);
                    var horizontalSides = connectedGroupsTopFence.Count + connectedGroupsBotFence.Count;

                    totalSides += horizontalSides;
                }

                for (var col = minCol; col <= maxCol; col++)
                {
                    var connectedGroupsLeftFence = GetConnectedPointGroupsOnSameCol(region, regionPoints, col, fenceOnLeft: true);
                    var connectedGroupsRightFence = GetConnectedPointGroupsOnSameCol(region, regionPoints, col, fenceOnLeft: false);
                    var verticalSides = connectedGroupsLeftFence.Count + connectedGroupsRightFence.Count;

                    totalSides += verticalSides;
                }

                region.Sides = totalSides;
            }
        }

        private List<List<Point>> GetConnectedPointGroupsOnSameRow(Region region, HashSet<string> regionPoints, int row, bool fenceOnTop)
        {
            var groups = new List<List<Point>>();

            var fenceVector = fenceOnTop ? new Point(-1, 0) : new Point(1, 0);
            var points = region.Points
                .Where(x => x.Row == row)
                .Where(x => !regionPoints.Contains($"{x.Add(fenceVector)}"))
                .OrderBy(x => x.Col)
                .ToList();

            if (points.Count == 0)
            {
                return groups;
            }
            if (points.Count == 1)
            {
                groups.Add(region.Points.ToList());
                return groups;
            }

            var currentGroup = new List<Point> { points[0] };
            for (int i = 1; i < points.Count; i++)
            {
                var curPoint = points[i];
                var prevPoint = currentGroup[^1];

                if (curPoint.Col - prevPoint.Col != 1)
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<Point> { curPoint };
                }
                else
                {
                    currentGroup.Add(curPoint);
                }
            }
            groups.Add(currentGroup);

            return groups;
        }

        private List<List<Point>> GetConnectedPointGroupsOnSameCol(Region region, HashSet<string> regionPoints, int col, bool fenceOnLeft)
        {
            var groups = new List<List<Point>>();

            var fenceVector = fenceOnLeft ? new Point(0, -1) : new Point(0, 1);
            var points = region.Points
                .Where(x => x.Col == col)
                .Where(x => !regionPoints.Contains($"{x.Add(fenceVector)}"))
                .OrderBy(x => x.Row)
                .ToList();

            if (points.Count == 0)
            {
                return groups;
            }
            if (points.Count == 1)
            {
                groups.Add(region.Points.ToList());
                return groups;
            }

            var currentGroup = new List<Point> { points[0] };
            for (int i = 1; i < points.Count; i++)
            {
                var curPoint = points[i];
                var prevPoint = currentGroup[^1];

                if (curPoint.Row - prevPoint.Row != 1)
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<Point> { curPoint };
                }
                else
                {
                    currentGroup.Add(curPoint);
                }
            }
            groups.Add(currentGroup);

            return groups;
        }

        private List<Point> GetAllPoints(List<List<char>> map)
        {
            var result = new List<Point>();

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    result.Add(new Point(i, j));
                }
            }

            return result;
        }

        private class Region
        {
            public Region(List<Point> points, int perimeter, char pointType, List<List<char>> map)
            {
                Points = points;
                Perimeter = perimeter;
                PointType = pointType;
                if (!Points.All(x => map[x.Row][x.Col] == pointType))
                {
                    throw new Exception("Cannot create region with different type points");
                }
            }
            public int Sides { get; set; }
            public int Perimeter { get; }
            public char PointType { get; }
            public List<Point> Points { get; set; } = new();
            public int GetArea() => Points.Count;
            public int GetPriceByPerimeter() => Perimeter * GetArea();
            public int GetPriceBySides() => Sides * GetArea();
            public override string ToString() => $"Type: {PointType}, Area: {GetArea()}, Perimeter: {Perimeter}, Sides {Sides}, Price1: {GetPriceByPerimeter()}, Price2 {GetPriceBySides()}";
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

        private readonly List<Point> _directionVectors = new()
        {
            new(-1, 0),
            new(0, 1),
            new(1, 0),
            new(0, -1)
        };
    }
}
