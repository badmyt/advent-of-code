using System;
using System.IO;
using System.Linq;

namespace AreaOfCode.Days
{
    public class Day4 : DayBase
    {
        public override void Run()
        {
            var pairSections = File.ReadAllLines(InputPath);
            var pairs = pairSections
                .Select(AssignmentPair.Parse)
                .ToList();

            var fullyOverlappingCount = pairs.Count(x => x.IsFullyOverlapping);
            var partiallyOverlappingCount = pairs.Count(x => x.IsPartiallyOverlapping);
            
            Console.WriteLine($"Fully overlapping periods count - {fullyOverlappingCount}");
            Console.WriteLine($"Partially overlapping periods count - {partiallyOverlappingCount}");
        }

        private class AssignmentPair
        {
            public int From1;
            public int To1;

            public int From2;
            public int To2;

            public override string ToString()
            {
                return $"{From1}-{To1},{From2}-{To2}";
            }

            public static AssignmentPair Parse(string line)
            {
                var splitLine = new { First = line.Split(',').First(), Last = line.Split(',').Last() };
                var pair = new AssignmentPair
                {
                    From1 = int.Parse(splitLine.First.Split('-').First()),
                    To1 = int.Parse(splitLine.First.Split('-').Last()),
                    From2 = int.Parse(splitLine.Last.Split('-').First()),
                    To2 = int.Parse(splitLine.Last.Split('-').Last())
                };

                return pair;
            }

            public bool IsFullyOverlapping => (From1 <= From2 && To1 >= To2) || (From2 <= From1 && To2 >= To1);

            public bool IsPartiallyOverlapping => From1 <= To2 && From2 <= To1;
        }
    }
}
