using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day3 : DayBase
    {
        public Day3(int year) : base(year)
        {
        }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var width = lines[0].Length;
            var height = lines.Length;

            var numbers = new List<long>();
            var numberPositions = new List<(long Number, List<Point> Position)>();

            for (int row = 0; row < height; row++)
            {
                var counting = false;
                var stringNumber = "";
                var points = new List<Point>();

                for (int col = 0; col < width; col++)
                {
                    var character = lines[row][col];
                    if (IsDigit(character))
                    {
                        stringNumber += character;
                        points.Add(new Point(row,col));
                        counting = counting || IsNumberEligible(row, col, lines);
                    }
                    else
                    {
                        if (stringNumber?.Length >= 1 && counting)
                        {
                            var number = long.Parse(stringNumber);
                            numbers.Add(number);
                            numberPositions.Add((number,points));
                        }
                        counting = false;
                        stringNumber = "";
                        points = new List<Point>();
                    }
                }

                if (counting)
                {
                    var number = long.Parse(stringNumber);
                    numbers.Add(number);
                    numberPositions.Add((number,points));
                }
            }

            long gearRatioSum = 0;
            
            for (int row = 0; row < height; row++)
            for (int col = 0; col < width; col++)
            {
                if (lines[row][col] != '*')
                    continue;
                
                var surroundings = new List<Point>
                {
                    new Point(row-1, col-1),
                    new Point(row-1, col),
                    new Point(row-1, col+1),
                    
                    new Point(row, col-1),
                    new Point(row, col+1),
                    
                    new Point(row+1, col-1),
                    new Point(row+1, col),
                    new Point(row+1, col+1),
                }.FindAll(x => x.IsInBounds(lines));

                var adjacentNumbers = numberPositions
                    .Where(x => x.Position.Any(p => surroundings.Any(s => p == s)))
                    .ToList();

                if (adjacentNumbers.Count != 2)
                    continue;
                
                gearRatioSum += adjacentNumbers[0].Number * adjacentNumbers[1].Number;
            }
            
            Console.WriteLine("Sum of valid numbers: " + numbers.Sum());
            Console.WriteLine("Gear ratio sum: " + gearRatioSum);
        }

        private static bool IsNumberEligible(int row, int col, string[] lines)
        {
            return new Point[]
                {
                    new Point(row-1, col-1),
                    new Point(row-1, col),
                    new Point(row-1, col+1),

                    new Point(row, col-1),
                    new Point(row, col+1),

                    new Point(row+1, col-1),
                    new Point(row+1, col),
                    new Point(row+1, col+1),

                }
                .Where(x => x.IsInBounds(lines))
                .Any(x => IsSymbol(lines[x.Row][x.Col]));
        }
        
        private struct Point
        {
            public int Row;
            public int Col;

            public Point(int row, int col)
            {
                Row = row;
                Col = col;
            }
            
            public bool IsInBounds(string[] lines)
            {
                return Row >= 0 && Row < lines.Length && Col >= 0 && Col < lines[0].Length;
            }

            public bool HandleGear(string[] lines, List<Point> adjacentGears)
            {
                var symbol = lines[Row][Col];
                if (symbol == '*')
                {
                    adjacentGears.Add(this);
                }

                return true;
            }

            public string GetPositionString()
            {
                return $"{Row},{Col}";
            }

            public static bool operator ==(Point a, Point b)
            {
                return a.GetPositionString() == b.GetPositionString();
            }
            
            public static bool operator !=(Point a, Point b)
            {
                return a.GetPositionString() != b.GetPositionString();
            }
        }
        public static bool IsDigit(char x) => "1234567890".Contains(x);
        public static bool IsSymbol(char x) => !"1234567890.".Contains(x);
    }
}
