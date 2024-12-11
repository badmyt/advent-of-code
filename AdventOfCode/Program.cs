using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AdventOfCode.Days;

namespace AdventOfCode
{
    class Program
    {
        private const int Year = 2024;
        private const bool RunSpecificDay = false;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Advent of code console!");

            TimeSpan elapsed = TimeSpan.Zero;

            if (RunSpecificDay)
            {
                var day = new Day0(Year);
                day.Run();
            }
            else
            {
                // Get all types in the current assembly that match the pattern DayX and inherit from DayBase
                var dayType = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass && t.Name.StartsWith("Day") && (t.BaseType?.Name == nameof(DayBase) || t.BaseType?.Name == nameof(DayBaseAsync)))
                    .OrderByDescending(t => int.Parse(t.Name.Substring(3)))  // Extract number and sort descending
                    .FirstOrDefault();  // Get the class with the highest number

                if (dayType != null)
                {
                    // Create an instance of the highest DayX class and Run
                    var isAsync = dayType.BaseType.Name.ToLower().Contains("async");
                    if (isAsync)
                    {
                        var dayInstanceAsync = (DayBaseAsync)Activator.CreateInstance(dayType, Year);
                        var dayClassNameAsync = dayInstanceAsync.GetType().Name;
                        var dayNumberAsync = dayClassNameAsync.Substring(3);
                        Console.WriteLine($"Running day {dayNumberAsync}...");

                        dayInstanceAsync.Stopwatch.Start();
                        await dayInstanceAsync.Run();
                        elapsed = dayInstanceAsync.Stopwatch.Elapsed;
                    }
                    else
                    {
                        var dayInstance = (DayBase)Activator.CreateInstance(dayType, Year);
                        var dayClassName = dayInstance.GetType().Name;
                        var dayNumber = dayClassName.Substring(3);
                        Console.WriteLine($"Running day {dayNumber}...");

                        dayInstance.Stopwatch.Start();
                        dayInstance.Run();
                        elapsed = dayInstance.Stopwatch.Elapsed;
                    }
                }
                else
                {
                    Console.WriteLine("No DayX classes found.");
                }
            }

            Console.WriteLine("Execution time - " + elapsed);
            Console.WriteLine("Press enter to exit..");
            Console.ReadLine();
        }
    }
}
