using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day18 : DayBase
    {
        private readonly Point3D[] _adjacentVectors = new[]
        {
            (  0,  0, +1 ), // up
            (  0,  0, -1 ), // down
            (  0, +1,  0 ), // forward
            (  0, -1,  0 ), // back
            ( -1,  0,  0 ), // left
            ( +1,  0,  0 )  // right
        }
        .Select(Point3D.FromTuple).ToArray();

        public override void Run()
        {
            var points = File.ReadAllLines(InputPath)
                .Select(x => x.Split(",").ToArray())
                .Select(x => new Point3D(x[0], x[1], x[2]))
                .ToList();

            var bounds = (X: points.Max(x => x.X) + 2, Y: points.Max(x => x.Y) + 2, Z: points.Max(x => x.Z) + 2);

            var area = new int[bounds.X, bounds.Y, bounds.Z];
            points.ForEach(point => { area[point.X, point.Y, point.Z] = 1; });

            var totalExposedSides = 0;
            foreach (var point in points)
            {
                var pointExposedSides = 6;

                foreach (var adjacentVector in _adjacentVectors)
                {
                    var adjacentPoint = point.AddVector(adjacentVector);

                    if (new[] { adjacentPoint.X, adjacentPoint.Y, adjacentPoint.Z }.Any(x => x < 0))
                        continue;

                    if (area[adjacentPoint.X, adjacentPoint.Y, adjacentPoint.Z] == 1)
                        pointExposedSides--;
                }

                totalExposedSides += pointExposedSides;
            }

            Console.WriteLine($"Surface area of scanned lava droplet - {totalExposedSides}");

            // find area of all air captured inside of droplet, then distract it

            Console.WriteLine($"Exterior surface area of scanned lava droplet - ");
            // not 3178 (too high)
        }

        private class Point3D {
            public int X;
            public int Y;
            public int Z;
            public Point3D() { }
            public Point3D(int x, int y, int z) { X = x; Y = y; Z = z; }
            public Point3D(string x, string y, string z) { X = int.Parse(x); Y = int.Parse(y); Z = int.Parse(z); }
            public Point3D AddVector(Point3D vector) { return new Point3D(X + vector.X, Y + vector.Y, Z + vector.Z); }
            public static Point3D FromTuple((int X, int Y, int Z) tuple) => new Point3D(tuple.X, tuple.Y, tuple.Z);
            public override string ToString() => $"{X},{Y},{Z}";
        }
    }
}
