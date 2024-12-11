using AdventOfCode.Days;
using System.Diagnostics;
using System;
using System.Threading.Tasks;

public abstract class DayBaseAsync : IAsyncDay
{
    public readonly int Year;

    public abstract Task Run();
    public Stopwatch Stopwatch { get; set; } = new Stopwatch();

    public DayBaseAsync(int year)
    {
        Year = year;
    }

    protected virtual string InputPath
    {
        get
        {
            return AppDomain.CurrentDomain.BaseDirectory + $"{Year}/Input/{this.GetType().Name.ToLower()}.txt";
        }
    }
}