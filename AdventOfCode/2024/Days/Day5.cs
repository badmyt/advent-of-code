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
            var prints = lines.Where(x => x.Contains(',')).Select(x => x.Split(",").Select(int.Parse).ToList()).ToList();
            var rules = lines
                .Where(x => x.Contains('|'))
                .Select(x =>
                    new Rule
                    {
                        Low = int.Parse(x.Split('|')[0]),
                        High = int.Parse(x.Split('|')[1])
                    })
                .GroupBy(x => x.Low, x => x.High)
                .Select(x => new RuleGroup(x.Key, x.ToList()))
                .ToDictionary(x => x.Low, x => x.Highs);

            var correctPrints = prints.FindAll(x => IsCorrect(x, rules));
            var middleNumbers = correctPrints.Select(x => x.ElementAt(x.Count / 2)).ToList();
            var middleNumbersSum = middleNumbers.Sum();

            var orderedPrints = correctPrints.Select(x => OrderPrint(x, rules)).ToList();
            var orderedMiddleNumbers = correctPrints.Select(x => x.ElementAt(x.Count / 2)).ToList();
            var orderedMiddleNumbersSum = orderedMiddleNumbers.Sum();

            Console.WriteLine($"Sum of middle numbers of correct prints: {middleNumbersSum}");
            Console.WriteLine($"Sum of middle numbers of Ordered correct prints: {middleNumbersSum}");

        }

        private List<int> OrderPrint(List<int> print, Dictionary<int, List<int>> rules)
        {
            var result = new List<int>();



            return result;
        }

        private bool IsCorrect(List<int> print, Dictionary<int, List<int>> rules)
        {
            for (int i = 1; i < print.Count; i++)
            {
                var number = print[i];
                if (!rules.ContainsKey(number))
                    continue;

                var highs = rules[number];
                foreach (var high in highs)
                {
                    for (int j = i-1; j >= 0; j--)
                    {
                        if (print[j] == high)
                            return false;
                    }
                }
            }

            return true;
        }

        private class Rule
        {
            public Rule() { }
            public Rule(int low, int high)
            {
                Low = low;
                High = high;
            }
            public int Low { get; set; }
            public int High { get; set; }
        }
        private class RuleGroup
        {
            public RuleGroup() { }
            public RuleGroup(int low, List<int> highs)
            {
                Low = low;
                Highs = highs;
            }
            public int Low { get; set; }
            public List<int> Highs { get; set; }
        }
    }
}
