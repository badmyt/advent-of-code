using System;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day3 : DayBase
    {
        private string _alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public override void Run()
        {
            var rucksacks = File.ReadAllLines(InputPath);

            var prioritySum = rucksacks.Select(x => GetLetterPriority(GetDuplicatingLetter(x))).Sum();
            var groupBadgePrioritiesSum = rucksacks
                .SplitAsArrays(3)
                .Select(x => GetLetterPriority(GetDuplicatingLetter(x[0], x[1], x[2])))
                .Sum();

            Console.WriteLine($"Rucksack duplicate items priorities sum - {prioritySum}");
            Console.WriteLine($"Group of elves badge priorities sum - {groupBadgePrioritiesSum}");
        }

        private static char GetDuplicatingLetter(string text)
        {
            int halfLength = text.Length / 2;
            var first = text.Substring(0, halfLength);
            var last = text.Substring(halfLength, halfLength);

            return first.Intersect(last).FirstOrDefault();
        }

        private static char GetDuplicatingLetter(string line1, string line2, string line3)
        {
            return line1.Intersect(line2).Intersect(line3).FirstOrDefault();
        }

        private int GetLetterPriority(char letter) => _alpha.IndexOf(letter) + 1;
    }
}
