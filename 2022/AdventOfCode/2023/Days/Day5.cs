using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day5 : DayBase
    {
        public Day5(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var seedsPart1 = lines[0].Split(": ").Skip(1).First().Split(" ").Select(long.Parse).ToList();
            var seedRanges = new List<(long, long)>();
            var maps = new List<Map>();

            for (int i = 0; i < seedsPart1.Count - 1; i += 2)
            {
                seedRanges.Add((seedsPart1[i], seedsPart1[i + 1]));
            }

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                if (line.Contains("map"))
                {
                    maps.Add(new Map());
                    continue;
                }

                if ("1234567890".Contains(line[0]))
                {
                    var lineInput = line.Split(" ").Select(long.Parse).ToArray();
                    var mapRow = new MapRow
                    {
                        Destination = lineInput[0],
                        Start = lineInput[1],
                        Range = lineInput[2]
                    };
                    maps.Last().Rows.Add(mapRow);
                    continue;
                }
            }

            var mappedSeedsPart1 = seedsPart1
                .Select(x => maps.Aggregate(x, (prev, cur) => cur.MapNumber(prev)))
                .ToList();
            
            var lowestMappedSeed1 = mappedSeedsPart1.Min();

            long lowestMappedSeed2 = long.MaxValue;
            foreach (var seedRange in seedRanges)
            {
                for (long i = seedRange.Item1; i <= seedRange.Item1 + seedRange.Item2; i++)
                {
                    var mapped = maps.Aggregate(i, (prev, cur) => cur.MapNumber(prev));
                    lowestMappedSeed2 = mapped < lowestMappedSeed2 ? mapped : lowestMappedSeed2;
                }
            }
            
            Console.WriteLine($"Lowest mapped seed part 1: {lowestMappedSeed1}");
            Console.WriteLine($"Lowest mapped seed part 2: {lowestMappedSeed2}");

        }

        private class Map
        {
            public List<MapRow> Rows { get; set; } = new List<MapRow>();

            public long MapNumber(long number)
            {
                foreach (var row in Rows)
                {
                    if (number >= row.Start && number <= row.Start + row.Range)
                    {
                        var delta = number - row.Start;
                        var mapped = row.Destination + delta;
                        return mapped;
                    }
                }

                return number;
            }
        }
        
        private class MapRow
        {
            public long Destination { get; set; }
            public long Start { get; set; }
            public long Range { get; set; }
        }
    }
}
