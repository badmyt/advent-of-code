using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day11 : DayBase
    {
        public Day11(int year) : base(year) { }

        public override void Run()
        {
            var originalSpace = File.ReadAllLines(InputPath);
            var space = ExpandSpace(originalSpace);
            var galaxies = FindGalaxies(space);
            var galaxyPairs = GetGalaxyPairs(galaxies);
            var galaxiesWithPaths = CalculatePaths(galaxyPairs, space);

            var sum = galaxiesWithPaths.Select(x => x.PathLength).Sum();
            // 10494813
            
            Console.WriteLine($"Sum of shortest path between all galaxy pairs: {sum}");
        }

        private static List<GalaxyPair> CalculatePaths(List<GalaxyPair> galaxyPairs, List<string> space)
        {
            foreach (var pair in galaxyPairs)
            {
                pair.PathLength = pair.GetShortestPath(space);
            }

            return galaxyPairs;
        }
        
        private static List<GalaxyPair> GetGalaxyPairs(List<Galaxy> galaxies)
        {
            var result = new List<GalaxyPair>();
            for (var i = 0; i < galaxies.Count - 1; i++)
            {
                var galaxyOne = galaxies[i];
                for (int j = i + 1; j < galaxies.Count; j++)
                {
                    var galaxyTwo = galaxies[j];
                    var galaxyPair = new GalaxyPair(galaxyOne, galaxyTwo);
                    result.Add(galaxyPair);
                }
            }
            
            return result;
        }

        private static List<Galaxy> FindGalaxies(List<string> space)
        {
            var galaxies = new List<Galaxy>();
            var index = 1;
            var width = space[0].Length;
            
            for (var row = 0; row < space.Count; row++)
            for (var col = 0; col < width; col++)
                if (space[row][col] == '#')
                {
                    var newGalaxy = new Galaxy(index.ToString(), row, col);
                    galaxies.Add(newGalaxy);
                    index++;
                }
            
            return galaxies;
        }
        
        private static List<string> ExpandSpace(string[] originalSpace)
        {
            var expandedSpace = originalSpace.Select(x => x.ToString()).ToList();
            
            var lastRow = originalSpace.Length - 1;
            var lastCol = originalSpace[0].Length - 1;
            var rowsToExpand = new List<int>();
            for (var row = lastRow; row >= 0; row--)
            {
                var expandRow = originalSpace[row].All(x => x == '.');
                if (expandRow)
                {
                    rowsToExpand.Add(row);
                }
            }

            var colsToExpand = new List<int>();
            for (var col = lastCol; col >= 0; col--)
            {
                var expandCol = originalSpace.All(x => x[col] == '.');
                if (expandCol)
                {
                    colsToExpand.Add(col);
                }
            }

            foreach (var col in colsToExpand)
            {
                for (var row = 0; row < originalSpace.Length; row++)
                {
                    var originalRow = expandedSpace[row];
                    var newRow = originalRow[..col] + '.' + originalRow[col..];
                    expandedSpace[row] = newRow;
                }
            }

            var rowLength = expandedSpace[0].Length;
            foreach (var row in rowsToExpand)
            {
                var newRow = string.Join("", Enumerable.Range(0, rowLength).Select(x => '.'));
                expandedSpace.Insert(row, newRow);
            }

            return expandedSpace;
        }

        private class GalaxyPair
        {
            private List<Galaxy> _galaxies;
            public int? PathLength { get; set; }

            public GalaxyPair(Galaxy one, Galaxy two)
            {
                _galaxies = new List<Galaxy> { one, two }.OrderBy(x => x.Id).ToList();
            }

            public Galaxy One => _galaxies[0];
            public Galaxy Two => _galaxies[1];

            public int GetShortestPath(List<string> space)
            {
                var colDiff = Math.Abs(Two.Col - One.Col);
                var rowDiff = Math.Abs(Two.Row - One.Row);
                return colDiff + rowDiff;
            }
            
            public override string ToString()
            {
                var withPath = PathLength.HasValue;
                return withPath 
                    ? $"{_galaxies[0].Id}-{_galaxies[1].Id}  :  {PathLength}"
                    : $"{_galaxies[0].Id}-{_galaxies[1].Id}";
            }
        }

        private class Galaxy : Point
        {
            public string Id { get; set; }

            public Galaxy(string id, int row, int col)
            {
                Id = id;
                Row = row;
                Col = col;
            }
        }
        
        private class Point
        {
            public int Row { get; set; }
            public int Col { get; set; }

            public Point()
            {
            }
            
            public Point(int row, int col)
            {
                Row = row;
                Col = col;
            }
        }
    }
}