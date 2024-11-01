using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day19 : DayBase
    {
        static int SurfaceAreaOfShape(List<Tuple<int, int, int>> cubes)
        {
            int totalArea = 0;
            HashSet<Tuple<int, int, int>> cubeSet = new HashSet<Tuple<int, int, int>>(cubes);
            foreach (Tuple<int, int, int> cube in cubes)
            {
                int x = cube.Item1, y = cube.Item2, z = cube.Item3;
                if (!cubeSet.Contains(Tuple.Create(x + 1, y, z))) totalArea++;
                if (!cubeSet.Contains(Tuple.Create(x - 1, y, z))) totalArea++;
                if (!cubeSet.Contains(Tuple.Create(x, y + 1, z))) totalArea++;
                if (!cubeSet.Contains(Tuple.Create(x, y - 1, z))) totalArea++;
                if (!cubeSet.Contains(Tuple.Create(x, y, z + 1))) totalArea++;
                if (!cubeSet.Contains(Tuple.Create(x, y, z - 1))) totalArea++;
            }
            return totalArea;
        }

        static int ExteriorSurfaceArea(List<Tuple<int, int, int>> cubes)
        {
            int totalArea = SurfaceAreaOfShape(cubes);
            HashSet<Tuple<int, int, int>> cubeSet = new HashSet<Tuple<int, int, int>>(cubes);
            foreach (Tuple<int, int, int> cube in cubes)
            {
                int x = cube.Item1, y = cube.Item2, z = cube.Item3;
                if (!cubeSet.Contains(Tuple.Create(x + 1, y, z))) totalArea--;
                if (!cubeSet.Contains(Tuple.Create(x, y + 1, z))) totalArea--;
                if (!cubeSet.Contains(Tuple.Create(x, y, z + 1))) totalArea--;
            }
            return totalArea;
        }

        public override void Run()
        {
            var tuples = File.ReadAllLines(InputPath)
                .Select(x => x.Split(",").ToArray())
                .Select(x => new Tuple<int,int,int>(int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2])))
                .ToList();

            var exteriorSurface = ExteriorSurfaceArea(tuples);

            Console.WriteLine($"Exterior surface - {exteriorSurface}");
        }
    }
}
