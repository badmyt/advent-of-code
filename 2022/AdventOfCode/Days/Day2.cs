using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day2 : DayBase
    {
        public override void Run()
        {
            string text = File.ReadAllText(InputPath);
            var codes = text.Replace(" ", string.Empty).Split("\n");

            var treePartOne = new Dictionary<string, int>
            {
                { "AX", 4 }, { "AY", 8}, { "AZ", 3 },
                { "BX", 1 }, { "BY", 5}, { "BZ", 9 },
                { "CX", 7 }, { "CY", 2}, { "CZ", 6 },
            };
            var outcomeOne = codes.Select(x => treePartOne[x]).Sum();

            var treePartTwo = new Dictionary<string, int>
            {
                { "AX", 3 }, { "AY", 4}, { "AZ", 8 },
                { "BX", 1 }, { "BY", 5}, { "BZ", 9 },
                { "CX", 2 }, { "CY", 6}, { "CZ", 7 },
            };
            var outcomeTwo = codes.Select(x => treePartTwo[x]).Sum();

            Console.WriteLine($"Rock/paper/scissors outcome part 1 - {outcomeOne}");
            Console.WriteLine($"Rock/paper/scissors outcome part 2 - {outcomeTwo}");
        }
    }
}
