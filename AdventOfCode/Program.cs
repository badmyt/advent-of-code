using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AdventOfCode.Days;

namespace AdventOfCode
{
    class Program
    {
        private const int Year = 2023;

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of code console!");
            var sw = new Stopwatch();
            sw.Start();

            // Get all types in the current assembly that match the pattern DayX and inherit from DayBase
            var dayType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && t.Name.StartsWith("Day") && t.BaseType?.Name == nameof(DayBase))
                .OrderByDescending(t => int.Parse(t.Name.Substring(3)))  // Extract number and sort descending
                .FirstOrDefault();  // Get the class with the highest number

            if (dayType != null)
            {
                // Create an instance of the highest DayX class and call Run
                var dayInstance = (DayBase)Activator.CreateInstance(dayType, Year); // Assuming constructor takes year

                var dayClassName = dayInstance.GetType().Name;
                var dayNumber = dayClassName.Substring(3);
                Console.WriteLine($"Running day {dayNumber}...");
                dayInstance.Run();
            }
            else
            {
                Console.WriteLine("No DayX classes found.");
            }

            Console.WriteLine("Execution time - " + sw.Elapsed);
            Console.WriteLine("Press enter to exit..");
            Console.ReadLine();
        }
    }
}
