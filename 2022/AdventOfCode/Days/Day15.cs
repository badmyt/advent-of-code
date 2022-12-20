using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day15 : DayBase
    {
        private const string InputAlpha = "1234567890=,;-";

        public override void Run()
        {
            var delimiters = new[] { '=', ',', ':' };
            var pairs = File.ReadAllLines(InputPath)
                .Select(x => string.Concat(x.Where(c => InputAlpha.Contains(c))))
                .Select(x => x.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new { Sensor = new Point(x[0],x[1]), Beacon = new Point(x[2], x[3])})
                .ToList();

            var beacons = pairs
                .Select(b => (b.Beacon.X, b.Beacon.Y))
                .ToHashSet();

            var sensors = pairs
                .Select(x => new Sensor { Location = x.Sensor, Radius = GetRadius(x.Sensor, x.Beacon) })
                .ToList();

            var set = new HashSet<int>();
            var controlY = 10;

            foreach (var sensor in sensors)
            {
                for (int dx = -sensor.Radius; dx <= sensor.Radius; dx++)
                {
                    var yLimit = sensor.Radius - Math.Abs(dx);
                    for (int dy = -yLimit; dy <= yLimit; dy++)
                    {
                        var x = sensor.Location.X + dx;
                        var y = sensor.Location.Y + dy;
                        if (y == controlY && !beacons.Contains((x,y)))
                            set.Add(x);
                    }
                }
            }

            //var testList = set.ToList().OrderBy(x => x).ToList();

            Console.WriteLine($"There are {set.Count} positions that cannot contain beacon with y={controlY}");
        }

        private int GetRadius(Point a, Point b)
        {
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            return dx + dy;
        }

        private class Sensor {
            public Point Location;
            public int Radius;
            public override string ToString() => $"{Location.X},{Location.Y} => {Radius}";
        }
        private class Point {
            public int X;
            public int Y;
            public Point(int x, int y){ X = x; Y = y; }
            public Point(string x, string y){ X = int.Parse(x); Y = int.Parse(y); }
        }
    }
}
