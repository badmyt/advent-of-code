using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day9 : DayBase
    {
        public Day9(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var historiesInput = lines.Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList()).ToList();

            long totalSum = 0;

            foreach (var historyInput in historiesInput.ToList())
            {
                var history = historyInput.ToList();
                bool diffTreeFinished = false;
                var diffRows = new List<List<long>>() { history };
                var currentRow = history;
                while (!diffTreeFinished)
                {
                    var nextRow = GetDifferences(currentRow);
                    diffRows.Add(nextRow);
                    currentRow = nextRow;
                    if (currentRow.All(x => x == 0))
                        break;
                }

                long? previousLastElement = null;
                for (int rowIndex = diffRows.Count - 1; rowIndex >= 0; rowIndex--)
                {
                    var row = diffRows[rowIndex];
                    var lastElement = row[row.Count - 1];
                    if (previousLastElement.HasValue)
                    {
                        previousLastElement = previousLastElement.Value + lastElement;
                        row.Add(previousLastElement.Value);
                    }
                    else
                    {
                        previousLastElement = 0;
                        row.Add(previousLastElement.Value);
                    }
                }

                var mainHistoryRow = diffRows[0];
                var addedHistoryRecord = mainHistoryRow[mainHistoryRow.Count - 1];
                totalSum += addedHistoryRecord;
            }
            
            Console.WriteLine($"Total sum of added numbers for all histories: {totalSum}");


            // part2
            totalSum = 0;
            foreach (var historyInput in historiesInput.ToList())
            {
                var history = historyInput.ToList();
                bool diffTreeFinished = false;
                var diffRows = new List<List<long>>() { history };
                var currentRow = history;
                while (!diffTreeFinished)
                {
                    var nextRow = GetDifferences(currentRow);
                    diffRows.Add(nextRow);
                    currentRow = nextRow;
                    if (currentRow.All(x => x == 0))
                        break;
                }

                long? previousFirstElement = null;
                for (int rowIndex = diffRows.Count - 1; rowIndex >= 0; rowIndex--)
                {
                    var row = diffRows[rowIndex];
                    var firstElement = row[0];
                    if (previousFirstElement.HasValue)
                    {
                        previousFirstElement = firstElement - previousFirstElement.Value;
                        diffRows[rowIndex] = row.Prepend(previousFirstElement.Value).ToList();
                    }
                    else
                    {
                        previousFirstElement = 0;
                        diffRows[rowIndex] = row.Prepend(previousFirstElement.Value).ToList();
                    }
                }

                var mainHistoryRow = diffRows[0];
                var addedHistoryRecord = mainHistoryRow[0];
                totalSum += addedHistoryRecord;
            }

            Console.WriteLine($"Total sum of prepended numbers for all histories: {totalSum}");
        }

        private List<long> GetDifferences(List<long> numbers)
        {
            if (numbers.Count == 1) return new List<long>();

            var result = new List<long>(numbers.Count - 1);

            for (int i = 1; i < numbers.Count; i++)
            {
                var diff = numbers[i] - numbers[i - 1];
                result.Add(diff);
            }

            return result;
        }
    }
}
