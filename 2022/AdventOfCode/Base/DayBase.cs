using System;
using System.Diagnostics;

namespace AdventOfCode.Days
{
    public abstract class DayBase : IDay
    {
        public abstract void Run();
        public Stopwatch Stopwatch { get; set; }

        protected virtual string InputPath { get
            {
                return AppDomain.CurrentDomain.BaseDirectory + $"Input/{this.GetType().Name.ToLower()}.txt"; 
            }
        }
    }
}
