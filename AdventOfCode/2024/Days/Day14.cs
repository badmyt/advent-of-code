using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day14 : DayBase
    {
        //private const int Height = 7;
        //private const int Width = 11;

        private const int Height = 103;
        private const int Width = 101;

        private const int Seconds = 100;

        public Day14(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);

            // part1
            var robotsPart1 = ParseInput(lines);
            Move(robotsPart1, Seconds);
            var safetyFactor = GetSafetyFactor(robotsPart1);
            Console.WriteLine($"Safety factor after {Seconds} seconds: {safetyFactor}");

            // part2
            var robotsPart2 = ParseInput(lines);
            var seconds = 0;
            bool stop = false;
            var lineLength = 10;
            while (!stop)
            {
                Move(robotsPart2, 1);
                seconds++;

                var maxRobotsLinedUp = GetMaxLinedUpRobotsCount(robotsPart2, lineLength);
                if (maxRobotsLinedUp > lineLength)
                    stop = true;
            }

            Console.WriteLine($"Found a christmass tree after {seconds} seconds!");
            Console.WriteLine("Debug matrix all robots:");
            PrintDebugMatrix(robotsPart2, false);
        }

        private static int GetMaxLinedUpRobotsCount(List<Robot> robots, int lineLength)
        {
            var groups = robots.GroupBy(x => x.Position.Row).Where(x => x.Count() >= lineLength);
            var linedRobotsFound = false;
            var linedRobots = new List<Robot>();

            foreach (var group in groups)
            {
                var ordered = group.OrderBy(x => x.Position.Col).ToList();

                var midRobotIndex = ordered.Count / 2 - 1;
                var midRobot = ordered.Skip(midRobotIndex).Take(1).ToList()[0];

                var prevRobot = midRobot;
                var leftMatch = false;
                for (int i = midRobotIndex - 1; i >= 0; i--)
                {
                    if (midRobotIndex - i > lineLength / 2)
                    {
                        leftMatch = true;
                        break;
                    }

                    var curRobot = ordered[i];
                    if (prevRobot.Position.Col - curRobot.Position.Col == 1)
                    {
                        prevRobot = curRobot;
                        continue;
                    }

                    break;
                }

                if (!leftMatch)
                    continue;

                prevRobot = midRobot;
                var rightMatch = false;
                for (int i = midRobotIndex + 1; i < ordered.Count; i++)
                {
                    if (i - midRobotIndex > lineLength / 2)
                    {
                        rightMatch = true;
                        break;
                    }

                    var curRobot = ordered[i];
                    if (curRobot.Position.Col - prevRobot.Position.Col == 1)
                    {
                        prevRobot = curRobot;
                        continue;
                    }

                    break;
                }

                if (rightMatch)
                {
                    linedRobotsFound = true;
                    linedRobots = ordered.ToList();
                    break;
                }
            }

            if (linedRobotsFound)
            {
                return linedRobots.Count;
            }

            return -1;
        }

        private void Move(List<Robot> robots)
        {
            for (int i = 0; i < robots.Count; i++)
            {
                var robot = robots[i];
                var next = GetNextRobotPosition(robot);
                robot.Position = next;
            }
        }

        private Point GetNextRobotPosition(Robot robot)
        {
            var cur = robot.Position;

            var nextRow = cur.Row + robot.Velocity.Row;
            if (nextRow < 0)
            {
                nextRow = (Height) - Math.Abs(nextRow);
            }
            else if (nextRow >= Height)
            {
                nextRow = Math.Abs(nextRow - 1) % (Height - 1);
            }

            var nextCol = cur.Col + robot.Velocity.Col;
            if (nextCol < 0)
            {
                nextCol = (Width) - Math.Abs(nextCol);
            }
            else if (nextCol >= Width)
            {
                nextCol = Math.Abs(nextCol - 1) % (Width - 1);
            }

            var next = new Point(nextRow, nextCol);
            return next;
        }

        private static void PrintDebugMatrix(List<Robot> robots, bool hideMid)
        {
            var midRow = Height / 2;
            var midCol = Width / 2; 

            Console.WriteLine("   12345678911");
            for (int i = 0; i < Height; i++)
            {
                Console.Write($"{i}: ");
                for (int j = 0; j < Width; j++)
                {
                    if (hideMid && (i == midRow || j == midCol))
                    {
                        Console.Write(" ");
                        continue;
                    }

                    var robotsOnPoint = robots.FindAll(x => x.Position.Row == i && x.Position.Col == j);
                    var robotCount = robotsOnPoint.Count;

                    if (robotCount > 0)
                    {
                        Console.Write($"{robotCount}");
                        continue;
                    }

                    Console.Write(".");
                }
                Console.WriteLine();
            }
        }

        private void Move(List<Robot> robots, int times)
        {
            //Console.WriteLine("Matrix initial state:");
            //PrintDebugMatrix(robots, false);
            for (int i = 0; i < times; i++)
            {
                Move(robots);

                if (Height < 10)
                {
                    Console.WriteLine($"Matrix after {i + 1} seconds:");
                    PrintDebugMatrix(robots, false);
                }
            }
        }

        private static int GetSafetyFactor(List<Robot> robots)
        {
            var midRow = Height / 2;
            var midCol = Width / 2;

            var q1 = robots.FindAll(x => x.Position.Row >= 0 && x.Position.Row < midRow && x.Position.Col >= 0 && x.Position.Col < midCol);
            var q2 = robots.FindAll(x => x.Position.Row >= 0 && x.Position.Row < midRow && x.Position.Col > midCol  && x.Position.Col < Width);
            var q3 = robots.FindAll(x => x.Position.Row > midRow && x.Position.Row < Height && x.Position.Col >= 0 && x.Position.Col < midCol);
            var q4 = robots.FindAll(x => x.Position.Row > midRow && x.Position.Row < Height && x.Position.Col > midCol && x.Position.Col < Width);
            var quadrantsWithRobots = new[] { q1, q2, q3, q4 }.Where(x => x.Count > 0).ToList();
            if (!quadrantsWithRobots.Any())
            {
                return 0;
            }

            var safetyFactor = 1;
            foreach (var q in quadrantsWithRobots)
            {
                safetyFactor *= q.Count;
            }

            return safetyFactor;
        }

        private static List<Robot> ParseInput(string[] lines)
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
