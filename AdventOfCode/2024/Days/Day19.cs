using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day19 : DayBase
    {
        public Day19(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var towelsList = lines[0].Split(", ").ToList();
            var designs = lines.Skip(2).ToList();
            var towels = towelsList.ToHashSet();
            var towelMaxLength = towelsList.Max(x => x.Length);

            var possibleDesignsCount = 0;

            foreach (var design in designs)
            {
                var isPossible = IsDesignPossible(design, towels, towelMaxLength, new HashSet<int>());
                if (isPossible)
                {
                    possibleDesignsCount++;
                }
            }
            
            Console.WriteLine($"{possibleDesignsCount} designs are possible");
        }

        private static bool IsDesignPossible(string design, HashSet<string> towels, int towelMaxLength, HashSet<int> calculatedOffsets, int currentOffset = 0)
        {
            var maxLength = design.Length < towelMaxLength ? design.Length : towelMaxLength;
            for (int i = maxLength; i > 0; i--)
            {
                var designPart = design[..i];
                if (towels.Contains(designPart))
                {
                    if (designPart == design)
                        return true;

                    var nextOffset = currentOffset + i;
                    if (calculatedOffsets.Contains(nextOffset))
                        continue; // stop branching as we already checked this case before

                    calculatedOffsets.Add(nextOffset);
                    var restOfDesign = design[i..];
                    var restIsPossible = IsDesignPossible(restOfDesign, towels, towelMaxLength, calculatedOffsets, nextOffset);
                    if (restIsPossible)
                        return true;
                }
            }

            return false;
        }
    }
}
