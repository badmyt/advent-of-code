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
                .ToList();

            var lowToHighs = rules
                .GroupBy(x => x.Low, x => x.High)
                .Select(x => new RuleGroupLowToHighs(x.Key, x.ToList()))
                .ToList()
                .ToDictionary(x => x.Low, x => x.Highs);

            var highToLows = rules
                .GroupBy(x => x.High, x => x.Low)
                .Select(x => new RuleGroupHighToLows(x.Key, x.ToList()))
                .ToList()
                .ToDictionary(x => x.High, x => x.Lows);

            var correctPrints = prints.FindAll(x => IsCorrect(x, lowToHighs));
            var middleNumbers = correctPrints.Select(x => x.ElementAt(x.Count / 2)).ToList();
            var middleNumbersSum = middleNumbers.Sum();

            Console.WriteLine($"Sum of middle numbers of correct prints: {middleNumbersSum}");

            var incorrectPrints = prints.Except(correctPrints).ToList();
            var orderedPrints = incorrectPrints.Select(x => OrderPrint(x, lowToHighs, highToLows)).ToList();
            var orderedMiddleNumbers = orderedPrints.Any() ? orderedPrints.Select(x => x.ElementAt(x.Count / 2)).ToList() : new List<int>();
            var orderedMiddleNumbersSum = orderedMiddleNumbers.Sum();

            Console.WriteLine($"Sum of middle numbers of Ordered correct prints: {orderedMiddleNumbersSum}");
        }

        private List<int> OrderPrint(List<int> printInput, Dictionary<int, List<int>> lowToHighs, Dictionary<int, List<int>> highToLows)
        {
            var print = printInput.ToList();

            for (int repeat = 0; repeat < printInput.Count; repeat++)
            {
                for (int printIndex = 0; printIndex < printInput.Count; printIndex++)
                {
                    var printValue = print[printIndex];
                    if (!highToLows.TryGetValue(printValue, out var lowerValues))
                        continue;

                    lowerValues = lowerValues.FindAll(x => print.Contains(x));
                    if (!lowerValues.Any())
                        continue;

                    var nextValues = print.Skip(printIndex + 1).ToList();
                    if (nextValues.Intersect(lowerValues).Any())
                    {
                        var highestValueIndex = FindExtremeIndex(lowerValues, print, left: false);
                        print = MoveValueRight(print, printIndex, highestValueIndex);
                    }
                }

                for (int printIndex = printInput.Count - 1; printIndex >= 0; printIndex--)
                {
                    var printValue = print[printIndex];
                    if (!lowToHighs.TryGetValue(printValue, out var higherValues))
                        continue;

                    higherValues = higherValues.FindAll(x => print.Contains(x));
                    if (!higherValues.Any())
                        continue;

                    var prevValues = print.Take(printIndex).ToList();
                    if (prevValues.Intersect(higherValues).Any())
                    {
                        var lowestValueIndex = FindExtremeIndex(higherValues, print, left: true);
                        print = MoveValueRight(print, lowestValueIndex, printIndex);
                    }
                }
            }

            return print;
        }

        private static List<int> MoveValueRight(List<int> values, int valueIndex, int afterIndex)
        {
            var result = new List<int>(values);
            var valueToMove = result[valueIndex];

            result.Insert(afterIndex + 1, valueToMove);
            result.RemoveAt(valueIndex);

            return result;
        }

        private static int FindExtremeIndex(List<int> values, List<int> print, bool left)
        {
            var indexes = new List<int>();

            for (int i = 0; i < print.Count; i++)
            {
                var val = print[i];
                if (values.Contains(val))
                {
                    indexes.Add(i);
                }
            }

            if (indexes.Count == 0)
            {
                return -1;
            }

            var cornerIndex = left
                ? indexes.OrderBy(x => x).First()
                : indexes.OrderByDescending(x => x).First();

            return cornerIndex;
        }

        private bool IsCorrect(List<int> print, Dictionary<int, List<int>> lowToHighRules)
        {
            for (int i = 1; i < print.Count; i++)
            {
                var number = print[i];
                if (!lowToHighRules.ContainsKey(number))
                    continue;

                var highs = lowToHighRules[number];
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
            public int Low { get; set; }
            public int High { get; set; }
            public override string ToString() => $"{Low}|{High}";
        }

        private class RuleGroupLowToHighs
        {
            public RuleGroupLowToHighs(int low, List<int> highs)
            {
                Low = low;
                Highs = highs;
            }
            public int Low { get; set; }
            public List<int> Highs { get; set; }
        }

        private class RuleGroupHighToLows
        {
            public RuleGroupHighToLows(int high, List<int> lows)
            {
                High = high;
                Lows = lows;
            }
            public int High { get; set; }
            public List<int> Lows { get; set; }
        }
    }
}
