using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day9 : DayBase
    {
        public override void Run()
        {
            var vectors = File.ReadAllLines(InputPath).Select(Vector.Parse).ToList();

            var knots = Enumerable.Range(1, 10).Select(x => new Point()).ToList();
            var head = knots[0];
            var knotSets = new Dictionary<Point, HashSet<string>>
            {
                { knots[1], new HashSet<string> { $"{knots[1]}" } },
                { knots.Last(), new HashSet<string> { $"{knots.Last()}" } }
            };

            foreach (var headVector in vectors)
            {
                (int dx, int dy) = (Math.Abs(headVector.X), Math.Abs(headVector.Y));
                var steps = dx > dy ? dx : dy;

                for (var i = 0; i < steps; i++)
                {
                    knots.Aggregate(head + headVector, (prev, cur) =>
                    {
                        var prevKnotVector = GetVector(cur, prev);
                        if (Math.Abs(prevKnotVector.X) > 1 || Math.Abs(prevKnotVector.Y) > 1 || cur == head)
                        {
                            MovePointToVector(cur, prevKnotVector);
                            knotSets.GetValue(cur)?.Add($"{cur}");
                        }
                        return cur;
                    });
                }
            }

            Console.WriteLine($"First knot unique visited positions - {knotSets.First().Value.Count}");
            Console.WriteLine($"Tail unique visited positions - {knotSets.Last().Value.Count}");
        }

        private Vector GetVector(Point from, Point to) => new Vector(to.X - from.X, to.Y - from.Y);

        private void MovePointToVector(Point point, Vector vector)
        {
            point.X += Math.Sign(vector.X);
            point.Y += Math.Sign(vector.Y);
        }

        private class Vector : Point
        {
            public Vector(int x, int y) : base(x,y) { }
            public static Vector Parse(string line)
            {
                var arguments = line.Split(' ');
                (char direction, int magnitude) = (arguments[0][0], int.Parse(arguments[1]));
                return direction switch
                {
                    'U' => new Vector(0, magnitude),
                    'R' => new Vector(magnitude, 0),
                    'D' => new Vector(0, -magnitude),
                    'L' => new Vector(-magnitude, 0),
                    _ => throw new IOException()
                };
            }
        }

        private class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Point() { }
            public Point(int x, int y) { X = x; Y = y; }
            public static Point operator +(Point a, Vector b) => new Point(a.X + b.X, a.Y + b.Y);
            public override string ToString() => $"{X},{Y}";
        }
    }
}
