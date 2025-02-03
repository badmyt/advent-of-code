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
            RunPart1();
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
            var obstacles = FindAllSymbols(map, '#');
            var boxes = FindAllSymbols(map, '[');

            Console.WriteLine("Initial state:");
            PrintDebugMatrix(map);
            for (int i = 0; i < moves.Length; i++)
            {
                var move = moves[i];
                robotPos = TryMoveRobot_part2(robotPos, move, boxes, obstacles);

                Console.WriteLine($"\nState after {i+1} moves: {move}");
                RebuildMap(map, robotPos, boxes, obstacles);
                //PrintDebugMatrix(map);
            }
            
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

        private Point TryMoveRobot_part2(Point point, char directionSymbol,
            List<Point> allBoxes, List<Point> obstacles)
        {
            var direction = Directions[directionSymbol];
            var nextPoint = point.Add(direction);

            if (obstacles.Any(x => x.Row == nextPoint.Row && x.Col == nextPoint.Col))
            { // can optimize
                return point; // cannot move
            }
            
            if (allBoxes.All(x => x.Row != nextPoint.Row ||
                               (x.Col != nextPoint.Col && x.Col != nextPoint.Col - 1)))
            { // can optimize
                return nextPoint; // moved point
            }
            
            var isHorizontal = directionSymbol is '<' or '>';
            if (isHorizontal && TryMoveHorizontal(point, direction, allBoxes, obstacles))
            {
                return nextPoint; // moved point and boxes horizontally
            }
            if (!isHorizontal && TryMoveVertical(point, new List<Point>(), direction, allBoxes, obstacles))
            {
                return nextPoint; // moved point and boxes vertically
            }
            
            return point; // cannot move
        }

        private bool TryMoveHorizontal(Point point, Point direction, List<Point> boxes, List<Point> obstacles)
        {
            var nextBoxes = direction.Col == 1
                ? boxes.Where(x => x.Col > point.Col)
                : boxes.Where(x => x.Col < point.Col);
            
            var nextBoxesOrdered = direction.Col == 1
                ? nextBoxes.OrderBy(x => x.Col).ToList()
                : nextBoxes.OrderByDescending(x => x.Col).ToList();

            var boxesToMove = 0;
            var currentCol = point.Col;
            foreach (var box in nextBoxesOrdered)
            {
                currentCol += 2 * Math.Sign(direction.Col);
                if (box.Col == currentCol)
                {
                    boxesToMove++;
                }
                else
                {
                    break;
                }
            }

            if (boxesToMove > 0 && !obstacles.Any(x => x.Row == point.Row && x.Col == currentCol))
            {
                for (int i = 0; i < boxesToMove; i++)
                {
                    boxes[i].Col += direction.Col;
                }
                return true;
            }
            
            return false;
        }
        
        private bool TryMoveVertical(Point initialPoint, List<Point> boxesToMove, Point direction, List<Point> allBoxes, List<Point> obstacles)
        {
            var pushPoints = GetPushPoints(initialPoint, boxesToMove);
            var nextBoxesToMoveStrings = new HashSet<string>();
            var nextRow = pushPoints[0].Row + direction.Row;
            foreach (var point in pushPoints)
            {
                if (obstacles.Any(x => x.Row == nextRow && x.Col == point.Col))
                {
                    return false;
                }

                var nextBoxToMove = allBoxes.FirstOrDefault(x => x.Row == nextRow &&
                                                              (x.Col == point.Col || x.Col == point.Col - 1));
                if (nextBoxToMove != null)
                {
                    nextBoxesToMoveStrings.Add($"{nextBoxToMove}");
                }
            }
            
            var nextBoxesToMove = nextBoxesToMoveStrings.Distinct().Select(Point.Parse).ToList();
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
                if (nextBoxesToMoveStrings.Contains(box.ToString()))
                {
                    box.Row += direction.Row;
                }
            }
            
            return true;
        }

        private List<Point> GetPushPoints(Point initialPoint, List<Point> boxesToMove)
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

        private void RebuildMap(List<List<char>> map, Point robotPos, List<Point> boxes,
            List<Point> obstacles)
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

                    if (obstacles.Any(x => x.Row == i && x.Col == j))
                    {
                        map[i][j] = '#';
                        continue;
                    }

                    if (boxes.Any(x => x.Row == i && (x.Col == j)))
                    {
                        map[i][j] = '[';
                        continue;
                    }
                    
                    if (boxes.Any(x => x.Row == i && (x.Col == j - 1)))
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
