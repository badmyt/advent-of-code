using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day12 : DayBase
    {
        public override void Run()
        {
            var mapLines = File.ReadAllLines(InputPath);
            var node = QuadNode.ParseFromText(mapLines);

            var closestPath = node.GetClosestPath();

            Console.WriteLine($"Closest path is {closestPath?.Depth} steps");
        }

        private class QuadNode
        {
            private char[,] _map;
            private char[,] _visitMap;
            private int _height;
            private int _width;
            private char _symbol;

            public int Row { get; set; }
            public int Col { get; set; }

            public QuadNode Up { get; set; }
            public QuadNode Right { get; set; }
            public QuadNode Down { get; set; }
            public QuadNode Left { get; set; }

            public QuadNode GetClosestPath()
            {
                if (Value == 'E')
                {
                    _visitMap[Row, Col] = 'E';

                    /*
                    Console.WriteLine("Path found, visitmap:");
                    _visitMap.PrintVisitMap();
                    */

                    return this;
                }

                Up ??= GetNext(-1, 0, '^');
                Right ??= GetNext(0, 1, '>');
                Down ??= GetNext(1, 0, 'v');
                Left ??= GetNext(0, -1, '<');

                var directions = new[] { Right, Up, Down, Left }.Where(x => x != null).ToList(); ;
                if (!directions.Any())
                {
                    return null;
                }

                if (Value == 'h')
                {
                    //debugger
                }

                var paths = new List<QuadNode>();

                for (int i = 0; i < directions.Count; i++)
                {
                    var path = directions[i].GetClosestPath();
                    if (path != null)
                        paths.Add(path);
                }

                var closestPath = paths.OrderBy(x => x.Depth).FirstOrDefault();

                return closestPath;
            }

            public bool IsVisited => new[] { 'v', '^', '>', '<', 'S' }.Contains(_symbol);
            public char Value => _map[Row, Col];

            public int Depth { get; }

            public QuadNode(char[,] map, int row, int col, int depth = 0, char[,] visitMap = null, char? symbol = null)
            {
                Row = row;
                Col = col;
                Depth = depth;

                if (symbol != null)
                    _symbol = symbol.Value;
                else
                    _symbol = 'S';

                _map = map;
                _height = _map.GetHeight();
                _width = _map.GetWidth();

                _visitMap = new char[_height, _width];
                if (visitMap != null)
                {
                    Array.Copy(visitMap, _visitMap, visitMap.Length);
                }

                _visitMap[row, col] = _symbol;
            }

            private QuadNode GetNext(int drow, int dcol, char symbol)
            {
                int row = Row + drow;
                int col = Col + dcol;

                if (row < 0 || row >= _height || col < 0 || col >= _width)
                    return null;

                if (_visitMap[row, col] != default(char))
                    return null;

                char next = _map[row, col];

                if (next == 'E' && Value != 'z')
                    return null;

                if ((next - Value <= 1) || (Value == 'S' && next == 'a') || (Value == 'z' && next == 'E'))
                    return new QuadNode(_map, row, col, Depth + 1, _visitMap, symbol);

                return null;
            }

            public static QuadNode ParseFromText(string[] lines)
            {
                var map = new char[lines.Length, lines[0].Length];
                int row = 0, col = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    for (int j = 0; j < lines[i].Length; j++)
                    {
                        map[i, j] = lines[i][j];
                        if (map[i, j] == 'S')
                        {
                            row = i;
                            col = j;
                        }
                    }
                }
                return new QuadNode(map, row, col);
            }

            public override string ToString() => $"{Value} - ({Row},{Col})";
        }
    }
}
