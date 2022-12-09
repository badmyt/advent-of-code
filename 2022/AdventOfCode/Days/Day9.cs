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
            
            var tail = new Point(0, 0);
            var tailSet = new HashSet<string> { $"{tail}" };
            var tailVector = new Vector();
            
            foreach (var headVector in vectors)
            {
                tailVector += headVector;
                while (Math.Abs(tailVector.X) > 1 || Math.Abs(tailVector.Y) > 1)
                {
                    MovePointToVector(tail, tailVector);
                    tailSet.Add($"{tail}");
                }
            }

            Console.WriteLine($"Tail unique visited positions - {tailSet.Count}");
        }

        private void MovePointToVector(Point point, Vector vector)
        {
            if (vector.X >= 1)
            {
                point.X += 1;
                vector.X -= 1;
            }
            if (vector.X <= -1)
            {
                point.X -= 1;
                vector.X += 1;
            }
            if (vector.Y >= 1)
            {
                point.Y += 1;
                vector.Y -= 1;
            }
            if (vector.Y <= -1)
            {
                point.Y -= 1;
                vector.Y += 1;
            }
        }

        private class Vector : Point
        {
            public Vector() { }
            public Vector(int x, int y) : base(x,y) { }
            public static Vector Parse(string line)
            {
                var arguments = line.Split(' ');
                var direction = arguments[0][0];
                var magnitude = int.Parse(arguments[1]);
                switch (direction)
                {
                    case 'U': return new Vector(0,magnitude);
                    case 'R': return new Vector(magnitude,0);
                    case 'D': return new Vector(0,-magnitude);
                    case 'L': return new Vector(-magnitude,0);
                    default: throw new IOException();
                }
            }
            public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
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
