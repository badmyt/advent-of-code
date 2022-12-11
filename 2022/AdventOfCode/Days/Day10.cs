using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode.Days
{
    public class Day10 : DayBase
    {
        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);

            new CRTDevice().Run(lines);
        }

        private class CRTDevice
        {
            private int _register = 1;
            private int _signalStrength = 0;
            private int[] _controlCycles = new[] { 20, 60, 100, 140, 180, 220 };
            private int _cycle = 0;
            private StringBuilder _outputBuilder = new StringBuilder();

            public void Run(string[] instructions)
            {
                foreach (var line in instructions)
                {
                    switch (line)
                    {
                        case "noop": RunCycle(); break;
                        default: Addx(int.Parse(line.Split(" ")[1])); break;
                    }
                }

                Console.WriteLine($"Signal strength sum - {_signalStrength}");
                Console.WriteLine($"\nCRT Screen output: \n\n{_outputBuilder}");
            }

            private void Addx(int x)
            {
                RunCycle();
                RunCycle();
                _register += x;
            }

            private void RunCycle()
            {
                _cycle += 1;

                var pos = (_cycle - 1) % 40;
                _outputBuilder.Append(Enumerable.Range(_register - 1, 3).Contains(pos) ? '#' : '.');

                if (_cycle % 40 == 0)
                    _outputBuilder.Append('\n');

                if (_controlCycles.Contains(_cycle))
                    _signalStrength += _cycle * _register;
            }
        }
    }
}
