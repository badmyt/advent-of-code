using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    public class Day6 : DayBase
    {
        public Day6(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var times = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
            var distances = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

            var waysToWin = new int[times.Length];
            
            for (var raceIndex = 0; raceIndex < times.Length; raceIndex++)
            {
                var time = int.Parse(times[raceIndex]);
                var distance = int.Parse(distances[raceIndex]);

                for (var i = 1; i < time - 1; i++)
                {
                    var raceResult = (time - i) * i;
                    if (raceResult > distance)
                        waysToWin[raceIndex] += 1;
                }
            }
            
            var multiplication = waysToWin.Aggregate(1, (x, y) => x * y);
            
            Console.WriteLine($"Ways to win multiplicated: {multiplication}");
            // part 2

            var raceTime = long.Parse(string.Join("", lines[0].Where(x => "1234567890".Contains(x))));
            var raceRecord = long.Parse(string.Join("", lines[1].Where(x => "0123456789".Contains(x))));

            long waysToWin2 = 0;

            for (var i = 1; i < raceTime - 1; i++)
            {
                var raceResult = (raceTime - i) * i;
                if (raceResult > raceRecord)
                    waysToWin2 += 1;
            }
            
            Console.WriteLine($"Ways to win part 2: {waysToWin2}");
        }
    }
}
