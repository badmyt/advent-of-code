using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        public Day1(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var numberPairs = lines.Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray()).ToList();

            var leftList = new List<int>();
            var rightList = new List<int>();

            foreach (var pair in numberPairs)
            {
                leftList.Add(int.Parse(pair[0]));
                rightList.Add(int.Parse(pair[1]));
            }

            leftList = leftList.OrderBy(x => x).ToList();
            rightList = rightList.OrderBy(x => x).ToList();

            var totalDiff = 0;

            for (var i = 0; i < leftList.Count; i++)
            {
                totalDiff += Math.Abs(rightList[i] - leftList[i]);
            }

            Console.WriteLine($"Total diff sum: {totalDiff}");

            var rightNumberCounts = rightList.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            var similaritySumm = 0;
            foreach (var number in leftList)
            {
                if (!rightList.Contains(number))
                    continue;

                var count = rightNumberCounts[number];
                similaritySumm += number * count;
            }

            Console.WriteLine($"Total similarity sum: {similaritySumm}");
        }
    }
}
