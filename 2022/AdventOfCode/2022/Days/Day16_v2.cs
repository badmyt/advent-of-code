using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Combinatorics.Collections;

namespace AdventOfCode.Days
{
    public class Day16_v2 : DayBase
    {
        private const int MinutesLimitPartOne = 30;
        private const int MinutesLimitPartTwo = 26;
        private readonly string[] InputPattern = new[] { "Valve ", " has flow rate=", "; tunnels lead to valves ", "; tunnel leads to valve " };

        private Dictionary<string, Valve> _valves;
        private List<string> _valuableValves;
        private Dictionary<string, Dictionary<string, int>> _distanceMatrix;
        private Dictionary<string, int> _cache = new Dictionary<string, int>();

        public override void Run()
        {
            Init();

            Console.WriteLine($"Elapsed after init - {Stopwatch.Elapsed}, releasing pressure...");

            var entrance = "AA";
            var valvePermutations = new Permutations<string>(_valuableValves);
            var totalPermutations = valvePermutations.Count;

            int counter = 0;
            int maxPressureReleasedPartOne = 0;
            IReadOnlyList<string> optimalPath = new List<string>();
            foreach (var path in valvePermutations)
            {
                int pressureReleased = CalculatePath(MinutesLimitPartOne, entrance, path);

                if (maxPressureReleasedPartOne < pressureReleased)
                {
                    maxPressureReleasedPartOne = pressureReleased;
                    optimalPath = path;
                }
                counter++;
            }
            var optimalPathString = string.Join(", ", optimalPath);

            Console.WriteLine($"Part 1 pressure released: {maxPressureReleasedPartOne}, elapsed - {Stopwatch.Elapsed}");

            counter = 0;
            int valvesHalfPathLength = valvePermutations.First().Count / 2;
            int valvesHalfPathTake = valvePermutations.First().Skip(valvesHalfPathLength).Count();
            int maxPressureReleasedPartTwo = 0;

            var optimalPathTogether = new List<string>() as IReadOnlyList<string>;

            foreach (var path in valvePermutations)
            {
                var pressureReleasedByMe = CalculatePath(26, entrance, path, 0, 0, valvesHalfPathLength);
                var pressureReleasedByElephant = CalculatePath(26, entrance, path, 0, valvesHalfPathLength, valvesHalfPathTake);
                var pressureReleased = pressureReleasedByMe + pressureReleasedByElephant;

                if (maxPressureReleasedPartTwo < pressureReleased)
                {
                    maxPressureReleasedPartTwo = pressureReleased;
                    optimalPathTogether = path;
                }
                counter++;
            }

            var optimalPathMe = optimalPathTogether.Take(valvesHalfPathLength).ToList();
            var optimalPathElephant = optimalPathTogether.Skip(valvesHalfPathLength).Take(valvesHalfPathTake).ToList();
            var stringMe = string.Join(", ", optimalPathMe);
            var stringElephant = string.Join(", ", optimalPathElephant);

            Console.WriteLine($"Part 2 pressure released: {maxPressureReleasedPartTwo}, elapsed - {Stopwatch.Elapsed}");
            // not 1891, not 1901, not 1904
        }

        private int CalculatePath(int minutesLimit, string current, IReadOnlyList<string> path,
            int minute = 0,
            int? skip = null,
            int? take = null)
        {
            int pressureReleased = 0;

            IEnumerable<string> modifiedPath = path;
            if (skip.HasValue)
                modifiedPath = modifiedPath.Skip(skip.Value);
            if (take.HasValue)
                modifiedPath = modifiedPath.Take(take.Value);

            var nextPath = modifiedPath.ToList();
            if (nextPath.Count == 0)
                return pressureReleased;

            var key = $"{current}-{string.Join(string.Empty, nextPath)}-{minute}";
            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }

            var next = path[skip ?? 0];
            var distance = _distanceMatrix[current][next];
            minute += distance + 1;
            current = next;

            if (minute >= minutesLimit)
                return pressureReleased;

            pressureReleased += (minutesLimit - minute) * _valves[current].FlowRate;

            pressureReleased += CalculatePath(minutesLimit, current, nextPath, minute, skip: 1);

            _cache[key] = pressureReleased;

            return pressureReleased;
        }

        private void Init()
        {
            // valves
            _valves = File.ReadAllLines(InputPath)
                .Select(x => x.Split(InputPattern, StringSplitOptions.RemoveEmptyEntries).ToArray())
                .Select(x => Valve.Create(x[0], int.Parse(x[1]), x[2]))
                .ToDictionary(x => x.Name);

            _valuableValves = _valves
                .Where(x => x.Value.FlowRate >= 1)
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

            if (depth > MinutesLimitPartTwo)
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
            public static Valve Create(string name, int flowRate, string valvesCsv) => new Valve
            {
                Name = name,
                FlowRate = flowRate,
                NextValves = valvesCsv.Split(", ").ToList(),
                NextValvesSet = valvesCsv.Split(", ").ToHashSet()
            };
            public override string ToString() => $"{Name}:\t{FlowRate}\t->\t{string.Join(", ", NextValves)}";
        }
    }
}