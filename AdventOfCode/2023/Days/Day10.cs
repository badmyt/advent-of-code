using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day10 : DayBase
    {
        private const bool Debug = true;
        private HashSet<(int x, int y)> _wayPoints = new HashSet<(int x, int y)>();
        private HashSet<(int x, int y)> _visitedPoints = new HashSet<(int x, int y)>();
        private (int x, int y) _startPoint = new(0, 0);
        private const char VisitedPointSymbol = '/';
        private const char WayPointSymbol = '0';

        public Day10(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var height = lines.Length;
            var width = lines[0].Length;
            var matrix = new char[height,width];

            Point start = null;

            for (int i = 0; i < lines.Length;i++)
            for (int j = 0; j < lines[0].Length; j++)
            {
                var symbol = lines[i][j];
                if (symbol == 'S')
                {
                    start = new Point(y: i, x: j);
                    _startPoint = (start.X, start.Y);
                    _wayPoints.Add((start.X, start.Y));
                }
                matrix[i, j] = symbol;
            }

            var wayPointsToCheckAllDirections = new List<Point>
            {
                new(start.X - 1, start.Y),
                new(start.X + 1, start.Y),
                new(start.X, start.Y + 1),
                new(start.X, start.Y + 1)
            };

            // my input has only 2 pipes connected to starting position so we can skip check for all possible direction combinations
            // so this is always 2 for me
            var wayPointsToCheck = wayPointsToCheckAllDirections
                .Where(p => GetNextPoint(matrix[p.Y, p.X], start, p) != null)
                .ToArray();

            var prevA = start;
            var prevB = start;
            var curA = wayPointsToCheck[0];
            var curB = wayPointsToCheck[1];
            var distance = 2;
            bool searching = true;
            PrintDebugMatrix(matrix, curA, curB, 1);
            while (searching)
            {
                var nextA = GetNextPoint(matrix[curA.Y, curA.X], prevA, curA);
                var nextB = GetNextPoint(matrix[curB.Y, curB.X], prevB, curB);

                prevA = curA;
                prevB = curB;

                curA = nextA;
                curB = nextB;

                foreach (var wayPoint in new[] { curA, curB })
                {
                    if (!_wayPoints.Contains((wayPoint.X,wayPoint.Y)))
                    {
                        _wayPoints.Add((wayPoint.X, wayPoint.Y));
                    }
                }

                PrintDebugMatrix(matrix, curA, curB, distance);

                if (nextA.X == nextB.X && nextA.Y == nextB.Y)
                    break;

                distance++;
            }

            //6931 - my answer
            Console.WriteLine($"Distance to farthest point from the start of pipe labyrinth: {distance}");

            // part 2
            var startPoints = new HashSet<Point>();
            for (int i = 0; i < width; i++)  // from top left -> to the right
                startPoints.Add(new Point(i, 0));
            for (int i = 0; i < height; i++) // from top left -> to the bottom
                startPoints.Add(new Point(0, i));
            for (int i = 0; i < width; i++)  // from bot left -> to the right
                startPoints.Add(new Point(i, height - 1));
            for (int i = 0; i < height; i++) // from bot right -> to the bottom
                startPoints.Add(new Point(width - 1, i));

            foreach (var point in startPoints)
            {
                TraverseMatrix(point, matrix);
            }

            CopyFinalDebugMatrix(matrix);
        }

        private void TraverseMatrix(Point point, char[,] matrix)
        {
            _visitedPoints.Add((point.X, point.Y));

            int height = matrix.GetLength(0);
            int width = matrix.GetLength(1);

            var surroundings = GetSurroundingPoints(point, matrix);
            var notVisited = surroundings.FindAll(x => !IsVisitedPoint(x));
            var notWayPoints = surroundings.FindAll(x => !IsWayPoint(x));

            var nextPoints = notWayPoints;
            foreach (var nextPoint in nextPoints)
            {
                TraverseMatrix(nextPoint, matrix);
            }
        }

        private bool IsVisitedPoint(Point point)
        {
            return _visitedPoints.Contains((point.X, point.Y));
        }

        private bool IsWayPoint(Point point)
        {
            return _wayPoints.Contains((point.X, point.Y));
        }

        private List<Point> GetSurroundingPoints(Point point, char[,] matrix)
        {
            var surroundings = new List<Point>
            {
                new(point.X - 1, point.Y),
                new(point.X + 1, point.Y),
                new(point.X, point.Y - 1),
                new(point.X, point.Y + 1),
            }.FindAll(x => IsInBounds(x, matrix));

            return surroundings;
        }

        private static bool IsInBounds(Point point, char[,] matrix)
        {
            int height = matrix.GetLength(0);
            int width = matrix.GetLength(1);

            return point.X >= 0 && point.X < width && point.Y >= 0 && point.Y < height;
        }

        private Point GetNextPoint(char pipeSymbol, Point prev, Point pipe) => pipeSymbol switch
        {
            '|' => pipe.IsTopFrom(prev)   ? pipe.MoveTop()   : pipe.IsBotFrom(prev)   ? pipe.MoveBot()   : null,
            '-' => pipe.IsLeftFrom(prev)  ? pipe.MoveLeft()  : pipe.IsRightFrom(prev) ? pipe.MoveRight() : null,
            'L' => pipe.IsLeftFrom(prev)  ? pipe.MoveTop()   : pipe.IsBotFrom(prev)   ? pipe.MoveRight() : null,
            'J' => pipe.IsRightFrom(prev) ? pipe.MoveTop()   : pipe.IsBotFrom(prev)   ? pipe.MoveLeft()  : null,
            '7' => pipe.IsTopFrom(prev)   ? pipe.MoveLeft()  : pipe.IsRightFrom(prev) ? pipe.MoveBot()   : null,
            'F' => pipe.IsTopFrom(prev)   ? pipe.MoveRight() : pipe.IsLeftFrom(prev)  ? pipe.MoveBot()   : null,
            '.' => null,
            'S' => throw new InvalidOperationException("We reached the start, something is wrong"),
            _ => throw new Exception($"Unknown symbol '{pipe}'")
        };

        private void CopyFinalDebugMatrix(char[,] matrix)
        {
            int matrixHeight = matrix.GetLength(0);
            int matrixWidth = matrix.GetLength(1);

            var text = "";

            for (int y = 0; y < matrixHeight; y++)
            {
                for (int x = 0; x < matrixWidth; x++)
                {
                    var symbol = matrix[x, y];
                    if (_wayPoints.Contains((x, y)))
                    {
                        symbol = WayPointSymbol;
                    }
                    else if (_visitedPoints.Contains((x, y)))
                    {
                        symbol = VisitedPointSymbol;
                    }
                    text += symbol;
                }
                text += "\r\n";
            }
        }

        private void PrintDebugMatrix(char[,] matrix, Point p1, Point p2, int distance)
        {
            if (!Debug || distance > 10)
                return;

            var localPrintedPoints = new HashSet<(int,int)>();

            _wayPoints.Add((p1.X, p1.Y));
            _wayPoints.Add((p2.X, p2.Y));

            int matrixHeight = matrix.GetLength(0);
            int matrixWidth = matrix.GetLength(1);

            // Calculate the bounds for the 10x8 area around p1
            int startY1 = Math.Max(0, p1.Y - 4);
            int endY1 = Math.Min(matrixHeight - 1, p1.Y + 3);
            int startX1 = Math.Max(0, p1.X - 5);
            int endX1 = Math.Min(matrixWidth - 1, p1.X + 4);

            // Clone the matrix and mark p1
            char[,] clonedArray1 = (char[,])matrix.Clone();
            clonedArray1[p1.Y, p1.X] = $"{distance}"[0];

            // Calculate the bounds for the 10x8 area around p2
            int startY2 = Math.Max(0, p2.Y - 4);
            int endY2 = Math.Min(matrixHeight - 1, p2.Y + 3);
            int startX2 = Math.Max(0, p2.X - 5);
            int endX2 = Math.Min(matrixWidth - 1, p2.X + 4);

            // Clone the matrix and mark p2
            char[,] clonedArray2 = (char[,])matrix.Clone();
            clonedArray2[p2.Y, p2.X] = $"{distance}"[0];

            Console.WriteLine($"---------------- distance {distance} -----------------");

            var endReached = p1.X == p2.X && p1.Y == p2.Y;

            // Loop through the rows of the 10x8 sections and print them side by side with left padding
            for (int i = 0; i < 8; i++)
            {
                // Print left padding of 5 spaces
                Console.Write(new string(' ', 5));

                // Print row from the first 10x8 section around p1
                for (int x = startX1; x <= endX1; x++)
                {
                    if (startY1 + i <= endY1)
                        WriteColored(clonedArray1[startY1 + i, x], p1.X, p1.Y, startY1+i, x, endReached, localPrintedPoints);
                    else
                        Console.Write(" "); // Fill empty rows if out of bounds
                }

                // Print 10 spaces between the sections
                Console.Write(new string(' ', 10));

                // Print row from the second 10x8 section around p2
                for (int x = startX2; x <= endX2; x++)
                {
                    if (startY2 + i <= endY2)
                        WriteColored(clonedArray2[startY2 + i, x], p2.X, p2.Y, startY2+i, x, endReached, localPrintedPoints);
                    else
                        Console.Write(" "); // Fill empty rows if out of bounds
                }

                Console.WriteLine(); // Move to the next row
            }

            Console.WriteLine(); // Extra line at the end
        }

        private void WriteColored(char symbol, int pipeX, int pipeY, int localPrintX, int localPrintY, bool endReached, HashSet<(int,int)> localPrintedPoints)
        {
            var isCenterPoint = localPrintX == pipeX && localPrintY == pipeY;
            if (endReached && isCenterPoint)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(symbol);
                Console.ForegroundColor = originalColor;
            }
            else if (isCenterPoint)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(symbol);
                Console.ForegroundColor = originalColor;

                if (!_wayPoints.Contains((pipeX, pipeY)))
                {
                    _wayPoints.Add((pipeX, pipeY));
                }
            }
            else if (_wayPoints.Any(point => point.x == pipeX && point.y == pipeY))
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(symbol);
                Console.ForegroundColor = originalColor;
            }
            else
            {
                Console.Write(symbol);
            }

            if (!localPrintedPoints.Contains((localPrintX, localPrintY)))
            {
                localPrintedPoints.Add((localPrintX, localPrintY));
            }
        }


        private class Point
        {
            public override string ToString() => $"X: {X},\tY: {Y}";

            public int X;
            public int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public bool IsLeftFrom(Point point) => X == point.X - 1 && Y == point.Y;
            public bool IsTopFrom(Point point) => X == point.X && Y == point.Y - 1;
            public bool IsRightFrom(Point point) => X == point.X + 1 && Y == point.Y;
            public bool IsBotFrom(Point point) => X == point.X && Y == point.Y + 1;

            public Point MoveLeft() => new(X - 1, Y);
            public Point MoveRight() => new(X + 1, Y);
            public Point MoveTop() => new(X, Y - 1);
            public Point MoveBot() => new(X, Y + 1);
        }
    }
}
