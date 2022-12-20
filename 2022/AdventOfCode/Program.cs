using System;
using AdventOfCode.Days;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of code console!");

            var day = new Day15();
            day.Run();

            Console.WriteLine("Press enter to exit..");
            Console.ReadLine();
        }
    }
}
