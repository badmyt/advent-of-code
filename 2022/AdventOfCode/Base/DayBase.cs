using System;
using System.Diagnostics;

namespace AdventOfCode.Days
{
    public abstract class DayBase : IDay
    {
        public readonly int Year;

        public abstract void Run();
        public Stopwatch Stopwatch { get; set; }

        public DayBase(int year)
        {
            Year = year;
        }

        protected virtual string InputPath { get
            {
                return AppDomain.CurrentDomain.BaseDirectory + $"{Year}/Input/{this.GetType().Name.ToLower()}.txt"; 
            }
        }
    }
}
