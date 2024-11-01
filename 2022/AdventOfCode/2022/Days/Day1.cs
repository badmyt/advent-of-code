using System;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day1 : DayBase
    {
        public override void Run()
        {
            string text = File.ReadAllText(InputPath);

            var elves = text.Split("\n\n").Select(x => x.Split("\n")).ToList();
            var elvenValues = elves.Select(e => e.Sum(v => int.Parse(v))).ToList().OrderByDescending(ev => ev);
            var biggestValue = elvenValues.FirstOrDefault();
            var topThreeValue = elvenValues.Take(3).Sum();

            Console.WriteLine($"Biggest carried calories - {biggestValue}");
            Console.WriteLine($"Top three biggest carried calories - {topThreeValue}");
        } 
    }
}
