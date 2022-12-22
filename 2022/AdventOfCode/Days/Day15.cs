using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Days
{
    public class Day15 : DayBase
    {
        private const string InputAlpha = "1234567890=,;-";
        private HashSet<(int, int)> _beacons = new HashSet<(int, int)>();
        private List<Sensor> _sensors = new List<Sensor>();

        public override void Run()
        {
            var sw = new Stopwatch();
            sw.Start();

            var delimiters = new[] { '=', ',', ':' };
            var pairs = File.ReadAllLines(InputPath)
                .Select(x => string.Concat(x.Where(c => InputAlpha.Contains(c))))
                .Select(x => x.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => new { Sensor = new Point(x[0],x[1]), Beacon = new Point(x[2], x[3])})
                .ToList();

            _beacons = pairs
                .Select(b => (b.Beacon.X, b.Beacon.Y))
                .ToHashSet();

            _sensors = pairs
                .Select(x => new Sensor { Location = x.Sensor, Radius = GetRadius(x.Sensor, x.Beacon) })
                .ToList();

            var set = new HashSet<int>();
            var controlY = 20000;

            foreach (var sensor in _sensors)
            {
                var y = controlY;
                var dy = Math.Abs(controlY - sensor.Location.Y);

                var startX = sensor.Location.X - sensor.Radius + dy;
                var endX = sensor.Location.X + sensor.Radius - dy;

                if (Math.Abs(dy) > sensor.Radius)
                    continue;

                for (int x = startX; x <= endX; x++)
                {
                    if (!_beacons.Contains((x, y)))
                        set.Add(x);
                }
            }

            Console.WriteLine($"There are {set.Count} positions that cannot contain beacon with y = {controlY}, elapsed = {sw.Elapsed}");

            Point beacon = null;
            foreach (var sensor in _sensors)
            {
                var y = sensor.Location.Y;
                for (var x = sensor.Location.X - sensor.Radius - 1; x <= sensor.Location.X; x++)
                {
                    if (!IsCoveredByAnySensor(x, y, sensor))
                        beacon = new Point(x, y);

                    y--;
                }
            }

            var tuningFrequency = (BigInteger)beacon.X * 4_000_000 + beacon.Y;

            Console.WriteLine($"Beacon is at ({beacon?.X}, {beacon?.Y}), tuning frequency = {tuningFrequency}, elapsed = {sw.Elapsed}");
        }

        private bool IsCoveredByAnySensor(int x, int y, Sensor caller)
        {
            if (x < 0 || y < 0)
                return true;

            if (_sensors.Except(new[] { caller }).Any(sensor => IsCoveredBySensor(new Point(x, y), sensor)))
                return true;

            return false;
        }

        private bool IsCoveredBySensor(Point point, Sensor sensor)
        {
            var dx = Math.Abs(point.X - sensor.Location.X);
            var dy = Math.Abs(point.Y - sensor.Location.Y);
            return dx + dy <= sensor.Radius;
        }

        private int GetRadius(Point a, Point b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

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
