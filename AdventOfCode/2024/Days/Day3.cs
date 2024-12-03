using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    public class Day3 : DayBase
    {
        private const string Pattern = @"mul\((\d+),(\d+)\)";
        private const string DoPattern = @"do\(\)";
        private const string DontPattern = @"don't\(\)";

        private static readonly int DoLength = "do()".Length;
        private static readonly int DontLenght = "don't()".Length;

        public Day3(int year) : base(year) { }

        public override void Run()
        {
            var input = File.ReadAllText(InputPath);
            var correctedInput = CorrectInput(input);

            var sum = Regex.Matches(correctedInput, Pattern)
                .Where(x => x.Success)
                .Sum(x => int.Parse(x.Groups[1].Value) * int.Parse(x.Groups[2].Value));

            Console.WriteLine($"Sum of all correct multiplications: {sum}");
        }

        private static string CorrectInput(string input)
        {
            var doRanges = Regex.Matches(input, DoPattern)
                .Where(x => x.Success)
                .Select(x => new { From = x.Index, To = x.Index + DoLength })
                .ToList();

            var dontRanges = Regex.Matches(input, DontPattern)
                .Where(x => x.Success)
                .Select(x => new { From = x.Index, To = x.Index + DontLenght })
                .ToList();

            var sb = new StringBuilder();
            bool take = true;
            for (var i = 0; i < input.Length; i++)
            {
                if (doRanges.Any(x => i >= x.From && i <= x.To))
                    take = true;
                else if (dontRanges.Any(x => i >= x.From && i <= x.To))
                    take = false;

                if (take)
                    sb.Append(input[i]);
            }

            return sb.ToString();
        }
    }
}
