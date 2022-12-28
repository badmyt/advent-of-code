using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day17 : DayBase
    {
        private long _preservedTowerHeight = 0;
        private const int TowerWidth = 7;
        private char[] EmptyLine => ".......".ToArray();
        private string _moves;
        private Rock[] _rocks => new[]
        {
            Rock.FromPicture(
                "..####."
            ),
            Rock.FromPicture(
                "...#...",
                "..###..",
                "...#..."
            ),
            Rock.FromPicture(
                "....#..",
                "....#..",
                "..###.."
            ),
            Rock.FromPicture(
                "..#....",
                "..#....",
                "..#....",
                "..#...."
            ),
            Rock.FromPicture(
                "..##...",
                "..##..."
            )
        };

        public override void Run()
        {
            _moves = File.ReadAllText(InputPath);

            SimulateRockFall(2022);

            SimulateRockFall(1_000_000_000_000);
        }

        private long SimulateRockFall(long rocksCount)
        {
            Console.WriteLine($"Simulating sandfall for {rocksCount} rocks falling");

            long? firstFullCycleUnits = null;
            long? firstFullCycleRocks = null;
            long? cycleDiffUnits = null;
            long? cycleDiffRocks = null;

            _preservedTowerHeight = 0;

            List<char[]> tower = new List<char[]>();
            int gasPushIndex = 0;

            long rockIndex = 0;
            var rockTypesCount = _rocks.Length;
            while (rockIndex < rocksCount)
            {
                var rock = _rocks[rockIndex % rockTypesCount];
                var rockPointsWithinTower = BuildRock(ref tower, rock);

                bool falling = true;
                bool gasPush = true;
                bool fullCycle = false;
                while (falling)
                {
                    if (gasPush)
                    {
                        var direction = _moves[gasPushIndex % _moves.Length];
                        rockPointsWithinTower = MoveRockToDirection(tower, rockPointsWithinTower, direction);

                        if (rockIndex > 0 && gasPushIndex % _moves.Length == 0)
                        {
                            fullCycle = true;
                        }

                        gasPushIndex++;
                    }
                    else
                    {
                        rockPointsWithinTower = TryMoveRockDown(tower, rockPointsWithinTower, out var rockMoved);
                        if (!rockMoved)
                            falling = false;
                    }

                    gasPush = !gasPush;
                }

                if (fullCycle && (firstFullCycleUnits == null || cycleDiffUnits == null))
                {
                    var towerHeightAfterCycle = GetCurrentTowerHeight(tower);

                    if (firstFullCycleUnits == null)
                    {
                        firstFullCycleUnits = towerHeightAfterCycle;
                        firstFullCycleRocks = rockIndex;
                    }
                    else
                    {
                        cycleDiffUnits = towerHeightAfterCycle - firstFullCycleUnits;
                        cycleDiffRocks = rockIndex - firstFullCycleRocks;

                        var rocksAtm = firstFullCycleRocks + cycleDiffRocks;
                        var unitsAtm = GetFullTowerHeight(tower);
                        var middleUnits = (rocksCount - rocksAtm) / cycleDiffRocks * cycleDiffUnits;
                        var rocksLeft = (rocksCount - rocksAtm) % cycleDiffRocks;

                        var currentTowerHeight = GetCurrentTowerHeight(tower);

                        _preservedTowerHeight = unitsAtm + middleUnits.Value - currentTowerHeight;
                        rockIndex = rocksCount - rocksLeft.Value;
                    }
                }

                rockIndex++;
            }

            var towerHeight = GetFullTowerHeight(tower);
            Console.WriteLine($"Tower height after {rocksCount} rocks stopped falling - {towerHeight}");

            return towerHeight;
        }

        

        private List<Point> TryMoveRockDown(List<char[]> tower, List<Point> rock, out bool rockMoved)
        {
            rockMoved = false;

            var bottomRockPoints = rock
                .GroupBy(x => x.Col)
                .Select(x => new { Col = x.Key, Row = x.Min(point => point.Row) })
                .ToList();

            foreach (var point in bottomRockPoints)
            {
                if (point.Row == 0 || tower[point.Row - 1][point.Col] == '#')
                    return rock;
            }

            var points = rock.OrderBy(x => x.Row).ToList();
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];

                tower[point.Row][point.Col] = '.';

                point.Row -= 1;
                tower[point.Row][point.Col] = '#';
            }

            rockMoved = true;

            return points;
        }

        private List<Point> MoveRockToDirection(List<char[]> tower, List<Point> rock, char direction)
        {
            if (!"<>".Contains(direction))
                throw new ArgumentException(nameof(direction));

            var dcol = direction == '<' ? -1 : +1;
            var cornerPoints = rock
                .GroupBy(x => x.Row)
                .Select(x => new
                {
                    Row = x.Key,
                    Col = direction == '<' ? x.Min(point => point.Col) : x.Max(point => point.Col)
                })
                .ToList();

            foreach (var point in cornerPoints)
            {
                var nextCol = point.Col + dcol;
                if (nextCol < 0 || nextCol >= TowerWidth)
                    return rock;

                if (tower[point.Row][nextCol] == '#')
                    return rock;
            }

            var points = (direction == '<') ? rock.OrderBy(x => x.Col).ToList() : rock.OrderByDescending(x => x.Col).ToList();
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];

                tower[point.Row][point.Col] = '.';

                point.Col += dcol;
                tower[point.Row][point.Col] = '#';
            }

            return points;
        }


        private List<Point> BuildRock(ref List<char[]> tower, Rock rock)
        {
            var existingWhiteLines = GetExistingWhiteLinesCount(tower);
            if (existingWhiteLines > 3)
            {
                var excessLines = existingWhiteLines - 3;
                tower = tower.GetRange(0, tower.Count - excessLines);
            }
            else if (existingWhiteLines < 3)
            {
                tower.AddRange(GetWhiteSpace(3 - existingWhiteLines));
            }

            var height = tower.Count;

            tower.AddRange(rock.ReversedPicture);

            var trackingPoints = rock.ReversedPoints.Select(point => new Point(point.Row + height, point.Col)).ToList();
            return trackingPoints;
        }

        private int GetExistingWhiteLinesCount(IEnumerable<char[]> tower)
        {
            int count = 0;
            foreach (var row in tower.Reverse())
            {
                if (row.Any(x => x != '.'))
                    return count;

                count++;
            }

            return count;
        }

        private class Rock {
            public char[][] Picture { get; }
            public char[][] ReversedPicture { get; }
            public List<Point> ReversedPoints { get; }
            public Rock(char[][] picture)
            {
                var points = new List<Point>();
                var reversed = picture.Reverse().ToArray();
                for (int row = 0; row < reversed.Length; row++)
                {
                    for (int col = 0; col < reversed[row].Length; col++)
                    {
                        if (reversed[row][col] == '#')
                            points.Add(new Point(row, col));
                    }
                }

                Picture = picture;
                ReversedPicture = reversed;
                ReversedPoints = points;
            }
            public static Rock FromPicture(params string[] picture) => new Rock(picture.Select(x => x.ToArray()).ToArray());
            public void Print() => Picture.ToList().ForEach(x => Console.WriteLine(string.Concat(x)));
        }

        private void PrintTower(List<char[]> tower)
        {
            Console.WriteLine("");
            foreach (var line in (tower as IEnumerable<char[]>).Reverse().Take(20))
            {
                Console.Write('|');
                Console.Write(line);
                Console.WriteLine('|');
            }
            Console.WriteLine("---------");
        }

        private List<char[]> GetWhiteSpace(int lines) => Enumerable.Range(0, lines).Select(x => EmptyLine).ToList();
        private long GetCurrentTowerHeight(List<char[]> tower) => tower.Count - GetExistingWhiteLinesCount(tower);
        private long GetFullTowerHeight(List<char[]> tower) => GetCurrentTowerHeight(tower) + _preservedTowerHeight;

        private class Point {
            public int Row;
            public int Col;
            public Point(int row, int col) { Row = row; Col = col; }
            public override string ToString() => $"{Row},{Col}";
        }
    }
}
