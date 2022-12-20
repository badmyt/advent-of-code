using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day15 : DayBase
    {
        private const string InputAlpha = "1234567890=,;-";
        private readonly HashSet<(int, int)> _coveredPoints = new HashSet<(int, int)>();
        private readonly HashSet<(int, int)> _coveredPointsMultipleTimes = new HashSet<(int, int)>();

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

            var beacons = pairs
                .Select(b => (b.Beacon.X, b.Beacon.Y))
                .ToHashSet();

            _sensors = pairs
                .Select(x => new Sensor { Location = x.Sensor, Radius = GetRadius(x.Sensor, x.Beacon) })
                .ToList();

            var set = new HashSet<int>();
            var controlY = 10;

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
                    if (!beacons.Contains((x, y)))
                        set.Add(x);
                }
            }

            Console.WriteLine($"There are {set.Count} positions that cannot contain beacon with y = {controlY}, elapsed = {sw.Elapsed}");

            Point beacon = null;

            foreach (var sensor in _sensors)
            {
                if (sensor.Location.X == 8)
                {
                    //debug
                }

                var startX = sensor.Location.X - sensor.Radius;
                var endX = sensor.Location.X + sensor.Radius;
                var leftX = startX - 1;
                var rightX = endX + 1;

                if (leftX == -6 || rightX == -6)
                {
                    //debug
                }

                beacon ??= CheckForBeacon(leftX, sensor.Location.Y, sensor);
                beacon ??= CheckForBeacon(rightX, sensor.Location.Y, sensor);
                if (beacon != null)
                    break;

                for (var x = startX; x <= endX; x++)
                {
                    if (x == -6)
                    {
                        //debug
                    }

                    var dx = Math.Abs(x - sensor.Location.X);
                    var dy = Math.Abs(sensor.Radius - dx + 1);

                    var yUpper = sensor.Location.Y - dy;
                    var yLower = sensor.Location.Y + dy;
                    var yZero = sensor.Location.Y;

                    beacon ??= CheckForBeacon(x, yUpper, sensor);
                    beacon ??= CheckForBeacon(x, yLower, sensor);
                    if (beacon != null)
                        break;
                }
            }

            //var xor1 = _coveredPoints.Aggregate((0 ,0), (prev, cur) => { return (prev.Item1 ^ cur.Item1, prev.Item2 ^ cur.Item2); });
            //var xor2 = _coveredPointsMultipleTimes.Aggregate(xor1, (prev, cur) => { return (prev.Item1 ^ cur.Item1, prev.Item2 ^ cur.Item2); });

            //var tuningFrequency = xor2.Item1 * 4000000 + xor2.Item2;
            var tuningFrequency = beacon == null ? 0 : beacon.X * 4_000_000 + beacon.Y;

            Console.WriteLine($"Beacon is at ({beacon?.X}, {beacon?.Y}), tuning frequency = {tuningFrequency}, elapsed = {sw.Elapsed}");
        }

        private Point CheckForBeacon(int x, int y, Sensor caller)
        {
            var maxValue = 20;
            if (x > maxValue || y > maxValue || x < 0 || y < 0)
                return null;

            if (_sensors.Except(new[] { caller }).Any(sensor => IsCoveredBySensor(new Point(x, y), sensor)))
                return null;

            return new Point(x,y);
        }

        private bool IsCoveredBySensor(Point point, Sensor sensor)
        {
            var dx = Math.Abs(point.X - sensor.Location.X);
            var dy = Math.Abs(point.Y - sensor.Location.Y);

            if (dx > sensor.Radius || dy > sensor.Radius)
                return false;

            if (dx + dy <= sensor.Radius)
                return true;

            return false;
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
