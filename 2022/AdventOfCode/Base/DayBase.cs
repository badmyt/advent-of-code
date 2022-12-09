using System;

namespace AdventOfCode.Days
{
    public abstract class DayBase : IDay
    {
        public abstract void Run();

        protected virtual string InputPath { get
            {
                return AppDomain.CurrentDomain.BaseDirectory + $"Input/{this.GetType().Name.ToLower()}.txt"; 
            }
        }
    }
}
