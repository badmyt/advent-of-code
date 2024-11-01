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

        private (int X, int Y, int Z) _boundaries;

        private List<(int X, int Y, int Z)> _visitedAirList = new List<(int X, int Y, int Z)>();
        private HashSet<(int X, int Y, int Z)> _visitedAirSet = new HashSet<(int X, int Y, int Z)>();

        private List<Point3D> _lavaList;
        private HashSet<(int X,int Y,int Z)> _lavaSet;

        public override void Run()
        {
            var tuples = File.ReadAllLines(InputPath)
                .Select(x => x.Split(",").ToArray())
                .Select(x => new Tuple<int, int, int>(int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2])))
                .ToList();



            _lavaList = File.ReadAllLines(InputPath)
                .Select(x => x.Split(",").ToArray())
                .Select(x => new Point3D(x[0], x[1], x[2]))
                .ToList();

            _boundaries = (X: _lavaList.Max(x => x.X) + 2, Y: _lavaList.Max(x => x.Y) + 2, Z: _lavaList.Max(x => x.Z) + 2);
            _lavaSet = _lavaList.Select(x => (x.X, x.Y, x.Z)).ToHashSet();

            var exposedSides = TraverseAir(new Point3D(0, 0, 0));

            var traverseIndex = 0;
            bool finish = false;
            while (!finish)
            {
                var visitedAir = _visitedAirList[traverseIndex];
                foreach (var vector in _adjacentVectors)
                {
                    var nearbyAir = new Point3D(visitedAir.X, visitedAir.Y, visitedAir.Z).AddVector(vector);
                    exposedSides += TraverseAir(nearbyAir);
                }

                traverseIndex++;

                if (traverseIndex > 0 && traverseIndex >= _visitedAirList.Count)
                    finish = true;
            }


            Console.WriteLine($"Surface area of scanned lava droplet - {exposedSides}");
            Console.WriteLine($"Exterior surface area of scanned lava droplet - {exposedSides}");
            // not 1977, not 1978 (too low)
            // not 3178 (too high)
        }

        private int TraverseAir(Point3D point)
        {
            if (!IsWithinBoundaries(point)
                || _lavaSet.Contains((point.X, point.Y, point.Z))
                || _visitedAirSet.Contains((point.X, point.Y, point.Z)))
                return 0;

            _visitedAirSet.Add((point.X, point.Y, point.Z));
            _visitedAirList.Add((point.X, point.Y, point.Z));

            var exposedLavaSides = 0;

            foreach (var vector in _adjacentVectors)
            {
                var adj = point.AddVector(vector);

                if (!IsWithinBoundaries(adj) || _visitedAirSet.Contains((adj.X, adj.Y, adj.Z)))
                    continue;

                if (_lavaSet.Contains((adj.X, adj.Y, adj.Z)))
                {
                    exposedLavaSides += 1;
                }
                else
                {
                    exposedLavaSides += TraverseAir(adj);
                }
            }

            return exposedLavaSides;
        }

        private bool IsWithinBoundaries(Point3D point)
        {
            return point.X >= 0 && point.X < _boundaries.X
                && point.Y >= 0 && point.Y < _boundaries.Y
                && point.Z >= 0 && point.Z < _boundaries.Z;
        }

        private class Point3D {
            public int X;
            public int Y;
            public int Z;
            public Point3D() { }
            public Point3D(int x, int y, int z) { X = x; Y = y; Z = z; }
            public Point3D(string x, string y, string z) { X = int.Parse(x); Y = int.Parse(y); Z = int.Parse(z); }
            public Point3D AddVector(Point3D vector) { return new Point3D(X + vector.X, Y + vector.Y, Z + vector.Z); }
            public Point3D Copy() { return new Point3D(X, Y, Z); }
            public static Point3D FromTuple((int X, int Y, int Z) tuple) => new Point3D(tuple.X, tuple.Y, tuple.Z);
            public override string ToString() => $"{X},{Y},{Z}";
        }
    }
}
