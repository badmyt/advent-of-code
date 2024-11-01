using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day2 : DayBase
    {
        private const int MaxGreen = 13;
        private const int MaxRed = 12;
        private const int MaxBlue = 14;

        public Day2(int year) : base(year)
        {
        }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var sum = 0;
            var powerSum = 0;

            for (int i=0; i<lines.Length; i++)
            {
                var gameNumber = i + 1;
                var (isPossible, power) = IsGamePossible(lines[i]);
                if (isPossible)
                {
                    sum += gameNumber;
                }

                powerSum += power;
            }

            Console.WriteLine($"Sum of possible game ids = {sum}");
            Console.WriteLine($"Sum of ALL game powers = {powerSum}");
        }

        private (bool, int) IsGamePossible(string game)
        {
            var possible = true;

            int maxGreen = 0;
            int maxRed = 0;
            int maxBlue = 0;

            var shows = game.Split(": ").Last().Split("; ");
            foreach (var show in shows)
            {
                var items = show.Split(", ");
                var green = Convert.ToInt32(items.FirstOrDefault(x => x.Contains("green"))?.Split(" ")?.FirstOrDefault() ?? "0");
                var red = Convert.ToInt32(items.FirstOrDefault(x => x.Contains("red"))?.Split(" ")?.FirstOrDefault() ?? "0");
                var blue = Convert.ToInt32(items.FirstOrDefault(x => x.Contains("blue"))?.Split(" ")?.FirstOrDefault() ?? "0");

                if (green > MaxGreen || red > MaxRed || blue > MaxBlue)
                {
                    possible = false;
                }

                if (green > maxGreen)
                    maxGreen = green;

                if (red > maxRed)
                    maxRed = red;

                if (blue > maxBlue)
                    maxBlue = blue;
            }

            var power = new[] { maxGreen, maxRed, maxBlue }.Where(x => x > 0).Aggregate(1, (prev,cur) => prev * cur);

            return (possible, power);
        }
    }
}
