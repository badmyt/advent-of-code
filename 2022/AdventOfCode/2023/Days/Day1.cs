using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        private const string Digits = "1234567890";
        private readonly List<string> _digits = new List<string>
        {
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
        };

        private readonly List<string> _digitWords = new List<string>
        {
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
        };

        public Day1(int year) : base(year)
        {
        }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);

            var digits = lines.Select(GetFirstLastDigits).ToList();
            var sum = digits.Sum();

            var digits2 = lines.Select(GetFirstLastDigitsWithStrings).ToList();
            var sum2 = digits2.Sum();

            foreach (var line in lines)
            {
                Console.WriteLine(line + " -> " + GetFirstLastDigitsWithStrings(line));
            }

            Console.WriteLine($"Sum of digits (part1) - {sum}");
            Console.WriteLine($"Sum of digits (part2) - {sum2}");
        }

        private int GetFirstLastDigitsWithStrings(string line)
        {
            var first = (int.MaxValue, 0);
            var last = (int.MinValue, 0);

            foreach (var digit in _digits)
            {
                var firstIndex = line.IndexOf(digit);
                if (firstIndex == -1)
                    continue;

                var lastIndex = line.LastIndexOf(digit);

                if (firstIndex < first.Item1)
                    first = (firstIndex, StringToDigit(digit));

                if (lastIndex > last.Item1)
                    last = (lastIndex, StringToDigit(digit));
            }

            if (last.Item1 == -1)
                last = first;

            return int.Parse($"{first.Item2}{last.Item2}");
        }

        private int StringToDigit(string text)
        {
            if (_digitWords.Contains(text))
                return _digitWords.IndexOf(text);
            else
                return int.Parse(text);
        }

        private static int GetFirstLastDigits(string line)
        {
            var digits = line.Where(x => Digits.Contains(x)).ToList();
            var first = digits.FirstOrDefault();
            var last = digits.LastOrDefault();
            var result = $"{first}{last}";

            var parsed = int.TryParse(result, out var intResult);

            return parsed ? intResult : 0;
        }
    }
}
