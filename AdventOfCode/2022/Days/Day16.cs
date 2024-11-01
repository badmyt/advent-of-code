using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Combinatorics.Collections;

namespace AdventOfCode.Days
{
    public class Day16 : DayBase
    {
        private const int MinutesLimit = 30;
        private readonly string[] InputPattern = new[] { "Valve ", " has flow rate=", "; tunnels lead to valves ", "; tunnel leads to valve "};

        private Dictionary<string, Valve> _valves;
        private List<string> _valuableValves;
        private Dictionary<string, Dictionary<string, int>> _distanceMatrix;

        public override void Run()
        {
            Init();

            Console.WriteLine($"Elapsed after init - {Stopwatch.Elapsed}, releasing pressure...");

            var entrance = _valves["AA"];

            var valvePermutations = new Permutations<string>(_valuableValves);
            var boolPermutations = Extensions.GetBooleanPermutations(_valuableValves.Count).ToList();
            var valuableBoolPermutations = boolPermutations.Where(x => x.Count(y => y) >= _valuableValves.Count / 3).ToList();

            int maxPressureReleased = 0;
            IReadOnlyList<string> optimalPath = new List<string>();
            IReadOnlyList<bool> optimalValveOpenings = new List<bool>();
            foreach (var path in valvePermutations)
            {
                int maxPressureReleasedPerPath = 0;
                IReadOnlyList<bool> preferredValveOpeningsPerPath = new List<bool>();
                foreach (var valveOpenings in valuableBoolPermutations)
                {
                    int pressureReleased = CalculatePath(entrance, path, valveOpenings);
                    if (maxPressureReleasedPerPath < pressureReleased)
                    {
                        maxPressureReleasedPerPath = pressureReleased;
                        preferredValveOpeningsPerPath = valveOpenings;
                    }
                }
                if (maxPressureReleased < maxPressureReleasedPerPath)
                {
                    maxPressureReleased = maxPressureReleasedPerPath;
                    optimalPath = path;
                    optimalValveOpenings = preferredValveOpeningsPerPath;
                }
            }

            Console.WriteLine($"Pressure released: {maxPressureReleased}, elapsed - {Stopwatch.Elapsed}");
        }

        private int CalculatePath(Valve entrance, IReadOnlyList<string> path, IReadOnlyList<bool> valveOpenings)
        {
            int minute = 0;
            int pressureReleased = 0;

            var current = entrance.Name;
            for (int i = 0; i < path.Count; i++)
            {
                var distance = _distanceMatrix[current][path[i]];
                minute += distance;
                current = path[i];

                if (minute >= MinutesLimit)
                    break;

                if (valveOpenings[i])
                {
                    minute++;
                    pressureReleased += (MinutesLimit - minute) * _valves[current].FlowRate;
                }
            }

            return pressureReleased;
        }

        private void Init()
        {
            // valves
            _valves = File.ReadAllLines(InputPath)
                .Select(x => x.Split(InputPattern, StringSplitOptions.RemoveEmptyEntries).ToArray())
                .Select(x => Valve.Create(x[0], int.Parse(x[1]), x[2]))
                .ToDictionary(x => x.Name);

            var maxFlowRate = _valves.Values.Max(x => x.FlowRate);
            var valuableFlowRate = maxFlowRate / 2;
            //int valuableFlowRate = 1;

            _valuableValves = _valves
                .Where(x => x.Value.FlowRate >= valuableFlowRate)
                .Select(x => x.Key)
                .Except(new[] { "AA" })
                .ToList();

            // distances
            _distanceMatrix = new Dictionary<string, Dictionary<string, int>>();
            foreach (var valve in _valves)
            {
                var distances = new Dictionary<string, int>();
                foreach (var nextValve in _valves)
                {
                    if (nextValve.Key == valve.Key)
                        continue;

                    var distance = FindClosestPaths(nextValve.Key, valve.Value).First().Count - 1;
                    distances[nextValve.Key] = distance;
                }
                _distanceMatrix[valve.Key] = distances;
            }
        }

        private List<List<string>> FindClosestPaths(string targetValve, Valve current, List<string> currentPath = null, int depth = 0)
        {
            depth++;

            if (depth > MinutesLimit)
                return null;

            var path = currentPath == null ? new List<string>() : new List<string>(currentPath);
            path.Add(current.Name);

            if (current.NextValvesSet.Where(x => !path.Contains(x)).Contains(targetValve))
            {
                path.Add(targetValve);
                return new List<List<string>> { path };
            }

            List<List<string>> resultingPaths = new List<List<string>>();
            foreach (var nextValveCode in current.NextValves.Where(x => !path.Contains(x)))
            {
                var nextValve = _valves[nextValveCode];
                var nextValvePathsToTarget = FindClosestPaths(targetValve, nextValve, path, depth);
                if (nextValvePathsToTarget?.Any() == true)
                    resultingPaths.AddRange(nextValvePathsToTarget);
            }

            var closestPaths = resultingPaths
                .Where(x => x != null)
                .GroupBy(x => x.Where(y => y != null).Count())
                .OrderBy(x => x.Key)
                ?.FirstOrDefault()
                ?.ToList();

            return closestPaths;
        }

        private class Valve
        {
            public string Name;
            public int FlowRate;
            public List<string> NextValves = new List<string>();
            public HashSet<string> NextValvesSet = new HashSet<string>();
            public static Valve Create(string name, int flowRate, string valvesCsv) => new Valve{
                Name = name,
                FlowRate = flowRate,
                NextValves = valvesCsv.Split(", ").ToList(),
                NextValvesSet = valvesCsv.Split(", ").ToHashSet()
            };
            public override string ToString() => $"{Name}:\t{FlowRate}\t->\t{string.Join(", ", NextValves)}";
        }
    }
}