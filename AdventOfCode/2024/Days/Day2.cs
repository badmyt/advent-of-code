using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day2 : DayBase
    {
        public Day2(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var reports = lines.Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList()).ToList();

            var safeReports = new List<List<int>>();
            for (int reportIndex = 0;  reportIndex < reports.Count; reportIndex++)
            {
                var report = reports[reportIndex];
                var isSafe = true;
                var sign = Math.Sign(report[1] - report[0]);
                for (int i = 0; i < report.Count-1; i++)
                {
                    var diff = report[i + 1] - report[i];
                    var currentSign = Math.Sign(diff);
                    var absDiff = Math.Abs(diff);
                    if (currentSign != sign || absDiff < 1 || absDiff > 3)
                    {
                        isSafe = false;
                        break;
                    }
                }

                if (isSafe)
                {
                    safeReports.Add(reports[reportIndex]);
                }
            }

            var safeCount = safeReports.Count();

            Console.WriteLine($"Total safe reports: {safeCount}");

            /// part 2
            var unsafeReports = reports.Except(safeReports).ToList();
            var editedSafeReports = new List<List<int>>();

            for (int reportIndex = 0; reportIndex < unsafeReports.Count; reportIndex++)
            {
                var report = unsafeReports[reportIndex];
                var reportOptions = GetAllOptions(report);

                foreach (var reportOption in reportOptions)
                {
                    var isOptionSafe = true;
                    var sign = Math.Sign(reportOption[1] - reportOption[0]);
                    for (int i = 0; i < reportOption.Count - 1; i++)
                    {
                        var diff = reportOption[i + 1] - reportOption[i];
                        var currentSign = Math.Sign(diff);
                        var absDiff = Math.Abs(diff);
                        if (currentSign != sign || absDiff < 1 || absDiff > 3)
                        {
                            isOptionSafe = false;
                            break;
                        }
                    }

                    if (isOptionSafe)
                    {
                        editedSafeReports.Add(report);
                        break;
                    }
                }
            }

            var editedSafeReportsCount = editedSafeReports.Count();
            var totalSafeCount = safeCount + editedSafeReportsCount;

            Console.WriteLine($"Total safe reports together with edited: {totalSafeCount}");
        }

        private static List<List<int>> GetAllOptions(List<int> list)
        {
            return Enumerable.Range(0, list.Count).Select(x => list.SkipAt(x).ToList()).ToList();
        }
    }
}
