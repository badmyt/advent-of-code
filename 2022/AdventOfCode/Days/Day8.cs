using System;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AreaOfCode.Days
{
    public class Day8 : DayBase
    {
        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var matrix = GetMatrix(lines);

            var visibleTrees = GetVisibleTreesCount(matrix);
            var highestScenicScore = GetHighestScenicScore(matrix);

            Console.WriteLine($"Trees visible from outside of the grid - {visibleTrees}");
            Console.WriteLine($"Highest scenic score tree - {highestScenicScore}");
        }

        private int GetVisibleTreesCount(int[,] treeMatrix)
        {
            var length = treeMatrix.GetWidth();
            var treeVisibilityMatrix = new int[length, length];

            int row = 0;
            int col = 0;

            var loopActions = new (int StartRow, int StartCol, Action InnerLoopAction, Action OuterLoopAction)[]
            {
                (0,         1,           () => { row += 1; },    () => { col += 1; row = 0; }),          // down, right
                (1,         length-1,    () => { col -= 1; },    () => { row += 1; col = length - 1; }), // left, down
                (length-1,  length-2,    () => { row -= 1; },    () => { col -= 1; row = length - 1; }), // up, left
                (length-2,  0,           () => { col += 1; },    () => { row -= 1; col = 0; }),          // right, up
            };

            int iterations = length - 2;

            foreach (var loop in loopActions)
            {
                row = loop.StartRow;
                col = loop.StartCol;

                for (int i = 0; i < iterations; i++)
                {
                    int highestPreviousTree = treeMatrix[row, col];

                    for (int j = 0; j < iterations; j++)
                    {
                        loop.InnerLoopAction();

                        int currentTree = treeMatrix[row, col];
                        if (currentTree > highestPreviousTree)
                        {
                            treeVisibilityMatrix[row, col] = 1;
                            highestPreviousTree = currentTree;
                        }
                    }

                    loop.OuterLoopAction();
                }
            }

            int visibleTrees = treeVisibilityMatrix.Sum() + length * 4 - 4; ;
            return visibleTrees;
        }

        private int GetHighestScenicScore(int[,] treeMatrix)
        {
            var length = treeMatrix.GetWidth();
            var scenicScoreMatrix = new int[length, length];

            for (int i = 1; i < length - 1; i++)
            {
                for (int j = 1; j < length - 1; j++)
                {
                    scenicScoreMatrix[i, j] = GetScenicScoreForTree(treeMatrix, i, j);
                }
            }

            return scenicScoreMatrix.Max();
        }

        private int GetScenicScoreForTree(int[,] treeMatrix, int treeRow, int treeCol)
        {
            var length = treeMatrix.GetWidth();
            var mainTreeHeight = treeMatrix[treeRow, treeCol];

            int row = treeRow;
            int col = treeCol;

            var loops = new (Action Move, Func<bool> Condition)[]
            {
                (() => { row -= 1; }, () => { return row > 0; }), // up
                (() => { col += 1; }, () => { return col < length-1; }), // right
                (() => { row += 1; }, () => { return row < length-1; }), // down
                (() => { col -= 1; }, () => { return col > 0;  }), // left
            };

            int scenicScore = 1;

            foreach (var loop in loops)
            {
                row = treeRow;
                col = treeCol;

                int visibleTrees = 0;

                while (loop.Condition())
                {
                    loop.Move();

                    visibleTrees += 1;

                    var tree = treeMatrix[row,col];
                    if (tree >= mainTreeHeight)
                    {
                        break;
                    }
                }

                scenicScore *= visibleTrees;
            }

            return scenicScore;
        }

        private int[,] GetMatrix(string[] trees)
        {
            var matrix = new int[trees.Length, trees[0].Length];

            for (int row = 0; row < trees.Length; row++)
            {
                for (int col = 0; col < trees[0].Length; col++)
                {
                    matrix[row, col] = int.Parse($"{trees[row][col]}");
                }
            }

            return matrix;
        }
    }
}
