using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day15 : DayBase
    {
        #region run
        
        public Day15(int year) : base(year) { }

        public override void Run()
        {
            //RunPart1();
            RunPart2();
        }
        
        private void RunPart1()
        {
            var linesPart1 = File.ReadAllLines(InputPath);
            var mapSizePart1 = linesPart1[0].Length;
            var mapPart1 = linesPart1.Take(mapSizePart1).Select(x => x.ToList()).ToList();
            var moves = linesPart1.Skip(mapSizePart1 + 1).ToArray().Aggregate("", (prev,cur) => $"{prev}{cur}");
            var robotPos = FindRobot(mapPart1);

            Console.WriteLine("Initial state:");
            for (int i = 0; i < moves.Length; i++)
            {
                var move = moves[i];
                robotPos = TryMoveRobot_part1(mapPart1, robotPos, move);
            }

            var boxes = FindAllSymbols(mapPart1, '0');
            var lanternfishSum = GetLanternfishSum(boxes);

            Console.WriteLine($"Sum of gps coordinates of all boxes for the lanternfish: {lanternfishSum}");
        }

        private void RunPart2()
        {
            var lines = File.ReadAllLines(InputPath);
            var mapSize = lines[0].Length;
            var moves = lines.Skip(mapSize + 1).ToArray().Aggregate("", (prev,cur) => $"{prev}{cur}");

            var map = GetExpandedMap(File.ReadAllLines(InputPath));
            var robotPos = FindRobot(map);
            var obstacles = FindAllSymbols(map, '#').ToHashSet();
            var boxes = FindAllSymbols(map, '[');

            Console.WriteLine("Press Y/N to turn interactive mode ON/OFF:");
            var isInteractive = Console.ReadKey().Key == ConsoleKey.Y;
            Console.WriteLine("Initial state:");
            PrintDebugMatrix(map);

            for (int i = 0; i < moves.Length; i++)
            {
                var move = isInteractive ? Console.ReadKey().Key switch
                {
                    ConsoleKey.LeftArrow => '<',
                    ConsoleKey.UpArrow => '^',
                    ConsoleKey.RightArrow => '>',
                    ConsoleKey.DownArrow => 'v',
                    _ => moves[i]
                } : moves[i];

                robotPos = TryMoveRobot_part2(robotPos, move, boxes, obstacles);

                if (isInteractive)
                {
                    var nextMove = i < moves.Length - 1 ? moves[i + 1] : 'N';
                    RebuildMap(map, robotPos, boxes, obstacles);
                    PrintDebugMatrixInteractive(map, i + 1, move, nextMove);
                }
            }

            Console.WriteLine("End state:");
            RebuildMap(map, robotPos, boxes, obstacles);
            PrintDebugMatrix(map);

            var lanternfishSum = GetLanternfishSum(boxes);

            Console.WriteLine($"Sum of gps coordinates of all boxes for the lanternfish: {lanternfishSum}");
        }

        #endregion
        
        #region part1
        
        private Point TryMoveRobot_part1(List<List<char>> map, Point point, char directionSymbol)
        {
            var direction = Directions[directionSymbol];
            var nextPoint = point.Add(direction);
            var nextSymbol = map[nextPoint.Row][nextPoint.Col];
            if (nextSymbol == '#')
            {
                return point; // cannot move
            }

            if (nextSymbol == '.')
            {
                map[nextPoint.Row][nextPoint.Col] = '@';
                map[point.Row][point.Col] = '.';
                return nextPoint; // moved
            }

            if (TryMoveBox_part1(map, nextPoint, direction, '@'))
            {
                map[nextPoint.Row][nextPoint.Col] = '@';
                map[point.Row][point.Col] = '.';
                return nextPoint; // moved
            }

            return point; // cannot move 
        }

        private bool TryMoveBox_part1(List<List<char>> map, Point point, Point direction, char previousSymbol)
        {
            var symbol = map[point.Row][point.Col];
            if (symbol != 'O')
            {
                throw new Exception($"Point {point} is not a box!");
            }

            var nextBox = point.Add(direction);
            var nextPoint = map[nextBox.Row][nextBox.Col];
            if (nextPoint == '#')
            {
                return false; // wall, cannot move
            }
            if (nextPoint == '.')
            {
                map[nextBox.Row][nextBox.Col] = symbol;
                map[point.Row][point.Col] = 'O';
                return true; //empty space, moved
            }

            var isNextMoved = TryMoveBox_part1(map, nextBox, direction, previousSymbol);
            if (isNextMoved)
            {
                map[nextBox.Row][nextBox.Col] = 'O';
                map[point.Row][point.Col] = previousSymbol;
                return true; //stack of boxes moved
            }
            return false; // stack of boxes is near the wall
        }

        #endregion
        
        #region part2

        private Point TryMoveRobot_part2(Point point, char directionSymbol, List<Point> allBoxes, HashSet<Point> obstacles)
        {
            var direction = Directions[directionSymbol];
            var nextPoint = point.Add(direction);

            if (obstacles.Contains(new Point(nextPoint.Row, nextPoint.Col)))
            {
                return point; // cannot move
            }

            if (!allBoxes.Contains(new Point(nextPoint.Row, nextPoint.Col)) && !allBoxes.Contains(new Point(nextPoint.Row, nextPoint.Col - 1)))
            {
                return nextPoint; // moved point
            }

            var isHorizontal = directionSymbol is '<' or '>';
            var filteredBoxes = (isHorizontal
                    ? allBoxes.Where(x => x.Row == point.Row)
                    : directionSymbol == '^'
                        ? allBoxes.Where(x => x.Row <= point.Row)
                        : allBoxes.Where(x => x.Row >= point.Row))
                .ToHashSet();

            if (isHorizontal && TryMoveHorizontal(point, direction, filteredBoxes, obstacles))
            {
                return nextPoint; // moved point and boxes horizontally
            }
            if (!isHorizontal && TryMoveVertical(point, new HashSet<Point>(), direction, filteredBoxes, obstacles))
            {
                return nextPoint; // moved point and boxes vertically
            }
            
            return point; // cannot move
        }

        private bool TryMoveHorizontal(Point point, Point direction, HashSet<Point> boxesOnTheRow, HashSet<Point> obstacles)
        {
            var nextBoxes = direction.Col == 1
                ? boxesOnTheRow.Where(x => x.Col > point.Col)
                : boxesOnTheRow.Where(x => x.Col < point.Col);
            
            var nextBoxesOrdered = direction.Col == 1
                ? nextBoxes.OrderBy(x => x.Col).ToList()
                : nextBoxes.OrderByDescending(x => x.Col).ToList();

            var boxesToMove = 0;
            var currentCol = point.Col;
            foreach (var box in nextBoxesOrdered)
            {
                currentCol += 2 * direction.Col;
                if ((direction.Col == -1 && box.Col == currentCol) || (direction.Col == 1  && box.Col == currentCol - 1))
                {
                    boxesToMove++;
                }
                else
                {
                    currentCol -= 2 * direction.Col;
                    break;
                }
            }

            if (boxesToMove == 0 || obstacles.Contains(new Point(point.Row, currentCol + direction.Col)))
            {
                return false;
            }

            foreach (var boxToMove in nextBoxesOrdered.Take(boxesToMove))
            {
                boxToMove.Col += direction.Col;
            }

            return true;
        }
        
        private bool TryMoveVertical(Point initialPoint, HashSet<Point> boxesToMove, Point direction, HashSet<Point> allBoxes, HashSet<Point> obstacles)
        {
            var pushPoints = GetPushPoints(initialPoint, boxesToMove);
            var nextBoxesToMove = new HashSet<Point>();
            var nextRow = pushPoints[0].Row + direction.Row;
            foreach (var point in pushPoints)
            {
                if (obstacles.Contains(new Point(nextRow, point.Col)))
                {
                    return false;
                }

                var point1 = new Point(nextRow, point.Col);
                var point2 = new Point(nextRow, point.Col - 1);
                if (allBoxes.Contains(point1))
                {
                    nextBoxesToMove.Add(point1);
                }
                else if (allBoxes.Contains(point2))
                {
                    nextBoxesToMove.Add(point2);
                }
            }
            
            if (nextBoxesToMove.Count == 0)
            {
                return true;
            }
            
            var canMoveNextBoxes = TryMoveVertical(initialPoint, nextBoxesToMove, direction, allBoxes, obstacles);
            if (!canMoveNextBoxes)
            {
                return false;
            }
            
            foreach (var box in allBoxes)
            {
                if (nextBoxesToMove.Contains(box))
                {
                    box.Row += direction.Row;
                }
            }
            
            return true;
        }

        private List<Point> GetPushPoints(Point initialPoint, HashSet<Point> boxesToMove)
        {
            var pushPoints = new List<Point>();
            if (boxesToMove.Any())
            {
                foreach (var box in boxesToMove)
                {
                    pushPoints.Add(new Point(box.Row, box.Col));
                    pushPoints.Add(new Point(box.Row, box.Col+1));
                }
            }
            else
            {
                pushPoints.Add(initialPoint);
            }

            return pushPoints;
        }
        
        #endregion

        #region misc

        private void RebuildMap(List<List<char>> map, Point robotPos, List<Point> boxes, HashSet<Point> obstacles)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    if (robotPos.Row == i && robotPos.Col == j)
                    {
                        map[i][j] = '@';
                        continue;
                    }

                    if (obstacles.Contains(new Point(i, j)))
                    {
                        map[i][j] = '#';
                        continue;
                    }

                    if (boxes.Contains(new Point(i, j)))
                    {
                        map[i][j] = '[';
                        continue;
                    }
                    
                    if (boxes.Contains(new Point(i, j - 1)))
                    {
                        map[i][j] = ']';
                        continue;
                    }

                    map[i][j] = '.';
                }
            }
        }
        
        private Point FindRobot(List<List<char>> map)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == '@')
                        return new Point(i, j);
                }
            }

            return null;
        }
        
        private List<Point> FindAllSymbols(List<List<char>> map, char symbol)
        {
            var result = new List<Point>();
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == symbol)
                        result.Add(new Point(i, j));
                }
            }
            return result;
        }

        private static List<List<char>> GetExpandedMap(string[] lines)
        {
            var result = new List<List<char>>();

            var emptyLineIndex = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == string.Empty)
                {
                    emptyLineIndex = i;
                    break;
                }
            }

            for (int i = 0; i < emptyLineIndex; i++)
            {
                var line = new List<char>();
                for (int j = 0; j < lines[i].Length; j++)
                {
                    var symbol = lines[i][j];
                    if (symbol == '#')
                    {
                        line.Add('#');
                        line.Add('#');
                    }
                    else if (symbol == '.')
                    {
                        line.Add('.');
                        line.Add('.');
                    }
                    else if (symbol == 'O')
                    {
                        line.Add('[');
                        line.Add(']');
                    }
                    else if (symbol == '@')
                    {
                        line.Add('@');
                        line.Add('.');
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                result.Add(line);
            }

            return result;
        }

        private long GetLanternfishSum(List<Point> boxes)
        {
            long sum = 0;

            foreach (var box in boxes)
            {
                sum += 100 * box.Row + box.Col;
            }

            return sum;
        }

        private static void PrintDebugMatrix(List<List<char>> map)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    Console.Write(map[i][j]);
                }
                Console.WriteLine();
            }
        }

        private static void PrintDebugMatrixInteractive(List<List<char>> map, int iteration, char prev, char next)
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J");

            // Move the cursor up to overwrite the previously printed matrix
            if (map.Count > 0)
            {
                Console.SetCursorPosition(0, 0);
            }

            Console.WriteLine($"\nState after {iteration} moves. Previous: {prev}{prev}{prev}{prev}, Next: {next}{next}{next}{next}{next}");

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    var symbol = map[i][j];
                    if (symbol == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    if (symbol == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    if (symbol == '[' || symbol == ']')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }

                    Console.Write(symbol);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
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
            public override bool Equals(object obj) => obj is Point p && Row == p.Row && Col == p.Col;
            public override int GetHashCode() => HashCode.Combine(Row, Col);
        }

        private static readonly Dictionary<char, Point> Directions = new()
        {
            { '<', new Point(0, -1) },
            { '^', new Point(-1, 0) },
            { '>', new Point(0, 1)  },
            { 'v', new Point(1, 0)  },
        };

        #endregion
    }
}
