using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day11 : DayBase
    {
        public override void Run()
        {
            var text = File.ReadAllText(InputPath);

            var mb = new MonkeyBusiness(text, reliefOn: false);
            var rounds = 10000;
            mb.Run(rounds);

            Console.WriteLine($"Monkey business level after {rounds} rounds - {mb.Level}");
        }

        private class MonkeyBusiness
        {
            private int _currentRound;
            private List<Monkey> _monkeys;
            private bool _reliefOn;
            private int _commonMultiple;
            private MonkeyBusiness() { }

            public long Level
            {
                get
                {
                    var topTwo = _monkeys.Select(x => x.ItemsInspected).OrderByDescending(x => x).Take(2).ToList();
                    return topTwo[0] * topTwo[1];
                }
            }

            public MonkeyBusiness(string input, bool reliefOn)
            {
                _monkeys = input.Split("\n\n").Select(Monkey.Parse).ToList();
                _reliefOn = reliefOn;
                _commonMultiple = _monkeys.Select(x => x.Divisor).Aggregate(1, (prev, cur) => { return prev * cur; });
            }

            public void Run(int rounds) => Enumerable.Range(1, rounds).ToList().ForEach(x => RunRound());

            private void RunRound()
            {
                foreach (var monkey in _monkeys)
                {
                    for (int i = 0; i < monkey.Items.Count; i++)
                    {
                        int oldWorryLevel = monkey.Items[i];
                        int newWorryLevel = monkey.OperationValue == "old"
                            ? oldWorryLevel * oldWorryLevel
                            : monkey.OperationSign switch
                            {
                                '+' => oldWorryLevel + int.Parse(monkey.OperationValue),
                                _ => oldWorryLevel * int.Parse(monkey.OperationValue)
                            };

                        int reliefedWorryLevel = _reliefOn
                            ? newWorryLevel / 3
                            : newWorryLevel % _commonMultiple;

                        var isDivisible = (reliefedWorryLevel % monkey.Divisor) == 0;
                        var nextMonkeyId = isDivisible ? monkey.ThrowIfTrue : monkey.ThrowIfFalse;
                        var nextMonkey = _monkeys.First(x => x.Id == nextMonkeyId);

                        nextMonkey.Items.Add(reliefedWorryLevel);

                        monkey.ItemsInspected++;
                    }

                    monkey.Items.Clear();
                }

                _currentRound++;

                Console.WriteLine($"\nItems inspected after round {_currentRound}:");
                foreach (var monkey in _monkeys)
                {
                    Console.WriteLine($"Monkey {monkey.Id} - inspected {monkey.ItemsInspected} items");
                }
            }

            private class Monkey
            {
                private static string _numbers = "1234567890,";
                private static string _signs = "*/+-";

                public int Id;
                public List<int> Items;
                public char OperationSign;
                public string OperationValue;
                public int Divisor;
                public int ThrowIfTrue;
                public int ThrowIfFalse;

                public long ItemsInspected;

                public static Monkey Parse(string monkeyText)
                {
                    var lines = monkeyText.Split("\n");

                    var monkey = new Monkey
                    {
                        Id = int.Parse(GetNumbers(lines[0])),
                        Items = GetNumbers(lines[1]).Split(",").Select(x => int.Parse(x)).ToList(),
                        OperationSign = GetSign(lines[2]),
                        OperationValue = GetNumbers(lines[2]),
                        Divisor = int.Parse(GetNumbers(lines[3])),
                        ThrowIfTrue = int.Parse(GetNumbers(lines[4])),
                        ThrowIfFalse = int.Parse(GetNumbers(lines[5]))
                    };

                    if (monkey.OperationValue == string.Empty)
                    {
                        monkey.OperationValue = "old";
                    }

                    return monkey;
                }

                private static string GetNumbers(string line) => string.Concat(line.Where(x => _numbers.Contains(x)));

                private static char GetSign(string line) => line.FirstOrDefault(x => _signs.Contains(x));

                public override string ToString() => $"Monkey {Id}: {{{string.Join(", ", Items ?? new List<int>())}}}";
            }
        }
    }
}
