using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day19 : DayBase
    {
        private HashSet<string> _towels;
        private int _towelMaxLength = 0;

        public Day19(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var towelsList = lines[0].Split(", ").ToList();
            var designs = lines.Skip(2).ToList();

            _towels = towelsList.ToHashSet();
            _towelMaxLength = towelsList.Max(x => x.Length);

            var possibleDesigns = new List<string>();
            foreach (var design in designs)
            {
                var isPossible = IsDesignPossible(design, new HashSet<int>());
                if (isPossible)
                {
                    possibleDesigns.Add(design);
                }
            }

            Console.WriteLine($"Part 1: {possibleDesigns.Count} designs are possible");

            long totalDesignVariations = 0;
            foreach (var design in possibleDesigns)
            {
                var designVariations = GetVariationsDynamic(design);
                totalDesignVariations += designVariations;

                Console.WriteLine($"Have {designVariations} variations for design '{design}' - dynamic");
            }

            Console.WriteLine($"Part 2: {totalDesignVariations} designs are possible including different variations");
        }

        private bool IsDesignPossible(string design, HashSet<int> calculatedOffsets, int currentOffset = 0)
        {
            var maxLength = design.Length < _towelMaxLength ? design.Length : _towelMaxLength;
            for (int i = maxLength; i > 0; i--)
            {
                var designPart = design[..i];
                if (_towels.Contains(designPart))
                {
                    if (designPart == design)
                        return true;

                    var nextOffset = currentOffset + i;
                    if (calculatedOffsets.Contains(nextOffset))
                        continue; // stop branching as we already checked this case before

                    calculatedOffsets.Add(nextOffset);
                    var restOfDesign = design[i..];
                    var restIsPossible = IsDesignPossible(restOfDesign, calculatedOffsets, nextOffset);
                    if (restIsPossible)
                        return true;
                }
            }

            return false;
        }

        private long GetVariationsDynamic(string design)
        {
            var posCounter = new long[design.Length + 1];
            posCounter[0] = 1;  // 1 variation initially

            for (int i = 1; i <= design.Length; i++)
            {
                for (int length = 1; length <= _towelMaxLength && i - length >= 0; length++)
                {
                    var part = design.Substring(i - length, length);
                    if (_towels.Contains(part)) 
                    {
                        posCounter[i] += posCounter[i - length]; 
                    }
                }
            }

            return posCounter[design.Length];
        }

        private long GetVariations(string design)
        {
            if (string.IsNullOrWhiteSpace(design))
            {
                return 0;
            }

            long nextVariations = 0;
            var maxLength = design.Length < _towelMaxLength ? design.Length : _towelMaxLength;
            for (int i = maxLength; i > 0; i--)
            {
                var designPart = design[..i];
                if (_towels.Contains(designPart))
                {

                    if (designPart == design)
                    {
                        nextVariations++;
                        continue;
                    }

                    var restOfDesign = design[i..];
                    var restVariations = GetVariations(restOfDesign);
                    nextVariations += restVariations;
                }
            }

            return nextVariations;
        }
    }
}
