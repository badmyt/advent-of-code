using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day14 : DayBase
    {
        public override void Run()
        {
            var rocks = File.ReadAllLines(InputPath)
                .Select(l => l.Split(" -> ").Aggregate(new List<Point>(), (list, item) =>
                    {
                        list.Add(new Point {
                            X = int.Parse(item.Split(",").First()),
                            Y = int.Parse(item.Split(",").Last())
                        });
                        return list;
                    }))
                .Select(l => new Rock { Points = l })
                .ToList();

            //part 1
            SimulateSandfall(rocks, addGround: false, print: false);

            //part 2
            SimulateSandfall(rocks, addGround: true, print: false);
        }

        private void SimulateSandfall(List<Rock> rocks, bool addGround = false, bool print = false)
        {
            var map = new char[1500, 1500].PopulateWith('.');

            int? abyssRow = rocks.SelectMany(x => x.Points).Select(x => x.Y).Max();
            if (addGround)
            {
                var groundRow = abyssRow.Value + 2;
                var pointA = new Point { X = 0, Y = groundRow };
                var pointB = new Point { X = map.GetWidth()-1, Y = groundRow };
                var rock = new Rock { Points = new List<Point> { pointA, pointB } };
                rocks.Add(rock);
                abyssRow = null;
            }

            foreach (var rock in rocks)
            {
                DrawRock(map, rock, print);
            }

            var units = DropSand(map, abyssRow, print);

            Console.WriteLine($"Units of sand came before falling into abyss or source blocked - {units}");
        }

        private int DropSand(char[,] map, int? abyssRowIndex, bool print)
        {
            int units = 0;

            while (units < 100000)
            {
                if (DropUnitOfSand(map, abyssRowIndex, print))
                    break;

                units++;
            }

            return units;
        }

        private bool DropUnitOfSand(char[,] map, int? abyssRowIndex = null, bool print = false)
        {
            int row = 0;
            int col = 500;

            if (map[row, col] != '.')
                return true;

            bool falling = true;
            while (falling)
            {
                falling = false;

                if (abyssRowIndex.HasValue && row >= abyssRowIndex)
                    return true;

                foreach (var (nextRow,nextCol) in new[] { (row + 1, col), (row + 1, col - 1), (row + 1, col + 1) })
                {
                    if (map[nextRow, nextCol] == '.')
                    {
                        row = nextRow;
                        col = nextCol;
                        falling = true;
                        break;
                    }
                }
            }

            map[row, col] = 'o';

            //debug
            if (print)
            {
                var take = 20;
                var minX = 0;
                var minY = col - take/2;
                minY = minY < 0 ? 0 : minY;
                map.SubArray(minX, take, minY, take).Print();
            }
            //debug

            return false;
        }

        private void DrawRock(char[,] map, Rock rock, bool print)
        {
            rock.Points.Aggregate((prev, cur) =>
            {
                DrawLine(map, prev, cur, '#');
                return cur;
            });

            //debug
            if (print)
            {
                var take = 20;
                var minX = rock.Points.Select(x => x.X).Min() - take/2;
                var minY = rock.Points.Select(y => y.Y).Min() - take/2;
                minX = minX < 0 ? 0 : minX;
                minY = minY < 0 ? 0 : minY;

                map.SubArray(minY, take, minX, take*2).Print();
            }
            //debug
        }

        private void DrawLine<T>(T[,] map, Point a, Point b, T filler)
        {
            //x -> +column
            //y -> +row

            var dx = Math.Abs(b.X - a.X);
            var dy = Math.Abs(b.Y - a.Y);

            var lowerX = Math.Min(a.X, b.X);
            var lowerY = Math.Min(a.Y, b.Y);

            for (int i = 0; i <= dx; i++)
                map[a.Y, lowerX + i] = filler;

            for (int i = 0; i <= dy; i++)
                map[lowerY + i, a.X] = filler;
        }

        private class Point {
            public int X;
            public int Y;
            public override string ToString() => $"{X},{Y}";
        }
        private class Rock {
            public List<Point> Points = new List<Point>();
            public override string ToString() => string.Join(" -> ", Points);
        }
    }
}
