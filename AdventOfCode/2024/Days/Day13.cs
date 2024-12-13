using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day13 : DayBase
    {
        private const long MaxButtonPressCountPartOne = 100;
        private const long PrizePositionAddedValuePartTwo = 10_000_000_000_000;

        public Day13(int year) : base(year) { }

        public override void Run()
        {
            var text = File.ReadAllText(InputPath);

            var machinesPart1 = GetMachines(text);
            long totalCostPart1 = 0;
            foreach (var machine in machinesPart1)
            {
                var minPair = GetMinCostPair(machine);
                if (minPair.A_Count == 0 && minPair.B_Count == 0 || minPair.MachineMinCost == 0)
                    continue;

                totalCostPart1 += minPair.MachineMinCost;

                machine.Cost = minPair.MachineMinCost;
            }

            Console.WriteLine($"Part1: Fewest tokens to spend to win all possible prizes: {totalCostPart1}, Elapsed: {Stopwatch.Elapsed}");

            var machinesPart2 = GetMachines(text, true);
            long totalCostPart2 = 0;
            foreach (var machine in machinesPart2)
            {
                var a = machine.A;
                var b = machine.B;
                var p = machine.Prize;

                if ((p.X * b.DY - p.Y * b.DX) % (b.DY * a.DX - b.DX * a.DY) != 0
                    || (p.X * a.DY - p.Y * a.DX) % (b.DX * a.DY - b.DY * a.DX) != 0)
                {
                    // no solution
                    continue;
                }

                long aTimes = (p.X * b.DY - p.Y * b.DX) / (b.DY * a.DX - b.DX * a.DY);
                long bTimes = (p.X * a.DY - p.Y * a.DX) / (b.DX * a.DY - b.DY * a.DX);

                var costA = aTimes * a.Cost;
                var costB = bTimes * b.Cost;

                totalCostPart2 += costA + costB;

                machine.Cost = costA + costB;
            }

            Console.WriteLine($"Part2: Fewest tokens to spend to win all possible prizes: {totalCostPart2}, Elapsed: {Stopwatch.Elapsed}");
        }

        private (long A_Count, long B_Count, long MachineMinCost) GetMinCostPair(Machine machine, bool partTwo = false)
        {
            var buttonCountPairs = GetButtonCountPairs(machine, partTwo);
            if (!buttonCountPairs.Any())
            {
                return (0,0,0);
            }

            var minCost = long.MaxValue;
            var minPairIndex = -1;

            for (int i = 0; i < buttonCountPairs.Count; i++)
            {
                var pair = buttonCountPairs[i];
                var pairCost = machine.A.Cost * pair.A_Count + machine.B.Cost * pair.B_Count;
                if (pairCost < minCost)
                {
                    minCost = pairCost;
                    minPairIndex = i;
                }
            }
            var minPair = buttonCountPairs[minPairIndex];

            return (minPair.A_Count, minPair.B_Count, minCost);
        }

        private static List<(long A_Count, long B_Count)> GetButtonCountPairs(Machine machine, bool partTwo)
        {
            var result = new List<(long, long)>();

            var maxStepsX = machine.Prize.X / machine.A.DX;
            var maxStepsY = machine.Prize.Y / machine.A.DY;
            var maxStepsXY = maxStepsX < maxStepsY ? maxStepsX : maxStepsY;

            for (long aCount = 0; aCount <= maxStepsXY; aCount++)
            {
                if (!partTwo && aCount >= MaxButtonPressCountPartOne)
                {
                    break;
                }

                var aDX = aCount * machine.A.DX;
                var aDY = aCount * machine.A.DY;

                var dxLeft = machine.Prize.X - aDX;
                var dyLeft = machine.Prize.Y - aDY;

                if (dxLeft == 0 && dyLeft == 0)
                {
                    result.Add((aCount, 0));
                }

                if (dxLeft % machine.B.DX == 0 && dyLeft % machine.B.DY == 0)
                {
                    var bCount = dxLeft / machine.B.DX;
                    var bCount_test = dyLeft / machine.B.DY;
                    if (bCount == bCount_test && bCount <= MaxButtonPressCountPartOne)
                    {
                        result.Add((aCount, bCount));
                    }
                }
            }

            return result;
        }

        private List<Machine> GetMachines(string input, bool addValuePartTwo = false)
        {
            var groups = input.Split("\r\n\r\n");
            var machines = new List<Machine>();
            foreach (var group in groups)
            {
                var lines = group.Split("\r\n").ToList();
                var a = lines[0];
                var adx = a.Split("A: X+")[1].Split(", Y+")[0];
                var ady = a.Split("A: X+")[1].Split(", Y+")[1];

                var b = lines[1];
                var bdx = b.Split("B: X+")[1].Split(", Y+")[0];
                var bdy = b.Split("B: X+")[1].Split(", Y+")[1];

                var prize = lines[2];
                var prizeXText = prize.Split("Prize: X=")[1].Split(", Y=")[0];
                var prizeYText = prize.Split("Prize: X=")[1].Split(", Y=")[1];

                var prizeX = long.Parse(prizeXText);
                var prizeY = long.Parse(prizeYText);

                if (addValuePartTwo)
                {
                    prizeX += PrizePositionAddedValuePartTwo;
                    prizeY += PrizePositionAddedValuePartTwo;
                }

                var machine = new Machine
                {
                    A = new Button { Cost = 3, DX = long.Parse(adx), DY = long.Parse(ady) },
                    B = new Button { Cost = 1, DX = long.Parse(bdx), DY = long.Parse(bdy) },
                    Prize = new Prize { X = prizeX, Y = prizeY }
                };
                machines.Add(machine);
            }
            return machines;
        }

        private class Machine
        {
            public long Cost { get; set; }

            public Button A { get; set; }
            public Button B { get; set; }
            public Prize Prize { get; set; }
            public override string ToString() => $"Cost: {Cost}, A: {A}, B: {B}, Prize: {Prize}";
        }

        private class Button
        {
            public long DX { get; set; }
            public long DY { get; set; }
            public long Cost { get; set; }
            public override string ToString() => $"${Cost}, +{DX}, +{DY}";
        }

        private class Prize
        {
            public long X { get; set; }
            public long Y { get; set; }
            public override string ToString() => $"{X}, {Y}";
        }
    }
}
