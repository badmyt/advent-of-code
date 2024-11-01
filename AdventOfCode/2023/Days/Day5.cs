using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                        End = lineInput[1] + lineInput[2]-1
                    };
                    maps.Last().Rows.Add(mapRow);
                    continue;
                }
            }

            maps.ForEach(map => map.OrderRows());

            var mappedSeedsPart1 = seedsPart1
                .Select(x => maps.Aggregate(x, (prev, cur) => cur.MapNumber(prev)))
                .ToList();

            var lowestMappedSeed1 = mappedSeedsPart1.Min();
            long lowestMappedSeed2 = long.MaxValue;

            // this should work with ranges instead of numbers, but cba changing as it already gave the result

            Parallel.ForEach(seedRanges, seedRange =>
            {
                long localLowestMapped = long.MaxValue;

                for (long i = seedRange.Item1; i <= seedRange.Item1 + seedRange.Item2; i++)
                {
                    long mapped = i;

                    foreach (var map in maps)
                    {
                        mapped = map.MapNumber(mapped);
                    }

                    localLowestMapped = Math.Min(localLowestMapped, mapped);
                }

                lock (this) 
                {
                    lowestMappedSeed2 = Math.Min(lowestMappedSeed2, localLowestMapped);
                }
            });

            Console.WriteLine($"Lowest mapped seed part 1: {lowestMappedSeed1}");
            Console.WriteLine($"Lowest mapped seed part 2: {lowestMappedSeed2}");
        }

        private class Map
        {
            public List<MapRow> Rows { get; set; } = new List<MapRow>();

            public void OrderRows()
            {
                Rows = Rows.OrderBy(x => x.Start).ToList();
            }

            public long MapNumber(long number)
            {
                int index = BinarySearchRow(number);
                if (index != -1)
                {
                    var row = Rows[index];
                    return row.Destination + (number - row.Start);
                }

                return number;
            }

            private int BinarySearchRow(long number)
            {
                int left = 0;
                int right = Rows.Count - 1;

                while (left <= right)
                {
                    int mid = left + (right - left) / 2;
                    var row = Rows[mid];

                    if (number >= row.Start && number <= row.End)
                    {
                        return mid;
                    }
                    else if (number < row.Start)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }

                return -1;
            }
        }
        
        private class MapRow
        {
            public long Destination { get; set; }
            public long Start { get; set; }
            public long End { get; set; }
        }
    }
}
