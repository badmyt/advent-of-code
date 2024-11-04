using System;
using System.Diagnostics;
using AdventOfCode.Days;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of code console!");
            var sw = new Stopwatch();
            sw.Start();

            var day = new Day9(2023);
            day.Run();

            Console.WriteLine("Execution time - " + sw.Elapsed);
            Console.WriteLine("Press enter to exit..");
            Console.ReadLine();
        }
    }
}
