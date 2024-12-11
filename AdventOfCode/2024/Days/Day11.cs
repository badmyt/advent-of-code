using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day11 : DayBase
    {
        private const int BlinksPartOne = 25;
        private const int BlinksPartTwo = 75;
        private const int StepMatrixBlinkCount = 38;

        public Day11(int year) : base(year) { }

        public override void Run()
        {
            var stones = File.ReadAllLines(InputPath)[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();

            // part 1
            var stonesPart1 = BlinkTimes(stones, BlinksPartOne);

            Console.WriteLine($"We have '{stonesPart1.Count}' stones after blinking {BlinksPartOne} times");

            // part 2
            Console.WriteLine($"Calculating stone counts... Elapsed: {Stopwatch.Elapsed}");

            var stonesPart2 = stones.Select(x => (Val: x, Final: false)).ToList();
            var stoneCounts = new List<long>[10];
            object lockObj = new();
            Parallel.ForEach("123456789", x =>
            {
                var parsedValue = int.Parse($"{x}");
                var result = GetStoneCounts(new List<long> { parsedValue }, StepMatrixBlinkCount);
                lock (lockObj)
                {
                    stoneCounts[parsedValue] = result;
                }
            });

            Console.WriteLine($"Calculating stone counts... FINISHED, Elapsed: {Stopwatch.Elapsed}");
            Console.WriteLine($"Calculating result...");

            var blinkIndex = 0;
            while (blinkIndex < BlinksPartTwo)
            {
                stonesPart2 = Blink(stonesPart2, blinkIndex, stoneCounts);
                blinkIndex++;
            }

            checked
            {
                var finalStonesPart2 = stonesPart2.FindAll(x => x.Final);
                var finalStonesSum = finalStonesPart2.Sum(x => x.Val);

                var midStonesPart2 = stonesPart2.FindAll(x => !x.Final);
                var midStonesSum = midStonesPart2.Count();

                long totalStonesPart2 = finalStonesSum + midStonesSum;

                Console.WriteLine($"We have '{totalStonesPart2}' stones after blinking {BlinksPartTwo} times");
            }
        }

        private static List<(long Val, bool Final)> Blink(List<(long Val, bool Final)> stones, int blinkIndex, List<long>[] stoneCountsPerValue)
        {
            var result = new List<(long, bool)>();

            for (int i = 0; i < stones.Count; i++)
            {
                if (stones[i].Final)
                {
                    result.Add(stones[i]);
                    continue;
                }

                var stone = stones[i].Val;
                var blinksLeft = BlinksPartTwo - blinkIndex;
                if (stone >= 1 && stone <= 9)
                {
                    var stoneCounts = stoneCountsPerValue[stone];
                    if (blinksLeft > 0 && blinksLeft < stoneCounts.Count)
                    {
                        var finalValue = stoneCounts[blinksLeft];
                        result.Add((finalValue, true));
                        continue;
                    }
                }

                if (stone == 0)
                {
                    result.Add((1, false));
                    continue;
                }

                var length = $"{stone}".Length;
                if (length % 2 == 0)
                {
                    var left = long.Parse($"{stone}"[..(length / 2)]);
                    var right = long.Parse($"{stone}"[(length / 2)..]);
                    result.Add((left, false));
                    result.Add((right, false));
                    continue;
                }

                var newStone = stone * 2024;
                result.Add((newStone, false));
            }

            return result;
        }

        private static List<long> GetStoneCounts(List<long> stones, int blinks)
        {
            var stoneCounts = new List<long> { stones.Count };
            var stoneList = stones.ToList();
            for (int i = 0; i < blinks; i++)
            {
                stoneList = BlinkPartOne(stoneList);
                stoneCounts.Add(stoneList.Count);
            }

            return stoneCounts;
        }

        private static List<long> BlinkTimes(List<long> stones, int times)
        {
            var result = stones.ToList();
            for (int i = 0; i < times; i++)
            {
                result = BlinkPartOne(result);
            }
            return result;
        }

        private static List<long> BlinkPartOne(List<long> stones)
        {
            var result = new List<long>();
            for (int i = 0; i < stones.Count; i++)
            {
                var stone = stones[i];
                if (stone == 0)
                {
                    result.Add(1);
                    continue;
                }

                var length = $"{stone}".Length;
                if (length % 2 == 0)
                {
                    var left = long.Parse($"{stone}"[..(length / 2)]);
                    var right = long.Parse($"{stone}"[(length / 2)..]);
                    result.Add(left);
                    result.Add(right);
                    continue;
                }

                var newStone = stone * 2024;
                result.Add(newStone);
            }
            return result;
        }
    }
}
