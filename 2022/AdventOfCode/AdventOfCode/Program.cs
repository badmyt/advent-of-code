using System;
using AreaOfCode.Days;

namespace AreaOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of code console!");

            var day = new Day6();
            day.Run();

            Console.WriteLine("Press enter to exit..");
            Console.ReadLine();
        }
    }
}
