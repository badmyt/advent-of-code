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
            (char[,] map, int sr, int sc) = ParseMap(mapLines);

            var stepCountPartOne = new PathFinder_BFS(map).FindClosestPath_BFS(sr, sc);

            Console.WriteLine($"Optimal path from S to E is {stepCountPartOne} steps");

            var startPositions = FindAllPositions(map, 'a');
            var stepCountPartTwo = startPositions
                .Select(x => new PathFinder_BFS(map).FindClosestPath_BFS(x.Item1, x.Item2))
                .Where(x => x > 0)
                .OrderBy(x => x)
                .First();

            Console.WriteLine($"Optimal path from S to E from best 'a' location is {stepCountPartTwo} steps");
        }

        private class PathFinder_BFS
        {
            private bool[,] _visited;
            private char[,] _map;
            private int _height;
            private int _width;
            private int[] _dr = new[] { -1, +1, 0, 0 };
            private int[] _dc = new[] { 0, 0, +1, -1 };
            private int _nodesLeftInLayer = 1;
            private int _nodesInNextLayer = 0;
            private int _moveCount = 0;
            private bool _endReached;

            private Queue<int> _rowQueue = new Queue<int>();
            private Queue<int> _colQueue = new Queue<int>();

            public PathFinder_BFS(char[,] map)
            {
                _map = map;
                _height = map.GetHeight();
                _width = map.GetWidth();
                _visited = new bool[_height, _width];
            }

            public int FindClosestPath_BFS(int sRow, int sCol)
            {
                _rowQueue.Enqueue(sRow);
                _colQueue.Enqueue(sCol);
                _visited[sRow, sCol] = true;

                while (_rowQueue.Any())
                {
                    var row = _rowQueue.Dequeue();
                    var col = _colQueue.Dequeue();

                    if (_map[row, col] == 'E')
                    {
                        _endReached = true;
                        break;
                    }

                    if (_map[row, col] == 'S')
                        _map[row, col] = 'a';

                    ExploreAdjacent(row, col);

                    _nodesLeftInLayer--;
                    if (_nodesLeftInLayer == 0)
                    {
                        _nodesLeftInLayer = _nodesInNextLayer;
                        _nodesInNextLayer = 0;
                        _moveCount++;
                    }
                }

                return _endReached ? _moveCount : -1;
            }

            private void ExploreAdjacent(int row, int col)
            {
                for (int i = 0; i < 4; i++)
                {
                    var nextRow = row + _dr[i];
                    var nextCol = col + _dc[i];

                    if (nextRow < 0 || nextCol < 0 || nextRow >= _height || nextCol >= _width)
                        continue;

                    if (_visited[nextRow, nextCol])
                        continue;

                    var current = _map[row, col];

                    if (_map[nextRow, nextCol] == 'S')
                        _map[nextRow, nextCol] = 'a';

                    var next = _map[nextRow, nextCol];
                    if (next - current > 1)
                        continue;

                    if (next == 'E' && current != 'z' && current != 'x')
                        continue;

                    _rowQueue.Enqueue(nextRow);
                    _colQueue.Enqueue(nextCol);

                    _visited[nextRow, nextCol] = true;

                    _nodesInNextLayer++;
                }
            }
        }

        public static (char[,], int, int) ParseMap(string[] lines)
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
            return (map, row, col);
        }

        public List<(int, int)> FindAllPositions(char[,] map, char val)
        {
            var result = new List<(int, int)>();
            var height = map.GetHeight();
            var width = map.GetWidth();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (map[i, j] == val || map[i, j] == 'S')
                        result.Add((i, j));
                }
            }
            return result;
        }
    }
}
