using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day14 : DayBase
    {
        private const int Height = 7;
        private const int Width = 11;

        //test example height = 7, width = 11
        //real example height = 103, width = 101

        public Day14(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var robots = ParseInput(lines);

            // first - col change per second
            // second - row change per second
            
            
            Console.WriteLine($"");
        }

        private List<Robot> ParseInput(string[] lines)
        {
            var result = new List<Robot>();

            foreach (var line in lines)
            {
                var pointPart = line.Split("p=")[1].Split(" v=")[0].Split(",");
                var position = new Point(int.Parse(pointPart[1]), int.Parse(pointPart[0]));

                var velocityPart = line.Split("p=")[1].Split(" v=")[1].Split(",");
                var velocity = new Point(int.Parse(velocityPart[1]), int.Parse(velocityPart[0]));

                var robot = new Robot { Position = position, Velocity = velocity };
                result.Add(robot);
            }

            return result;
        }

        private class Robot
        {
            public Point Position { get; set; }
            public Point Velocity { get; set; }
            public override string ToString() => $"Pos: {Position}, Velocity: {Velocity}";
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
