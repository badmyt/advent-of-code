using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Combinatorics.Collections;

namespace AdventOfCode.Days
{
    public class Day7 : DayBase
    {
        public Day7(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var equations = lines.Select(x => new Equation
            {
                Result = long.Parse(x.Split(":")[0]),
                Numbers = x.Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList()
            }).ToList();

            var possibleEquations = equations.FindAll(x => IsPossible(x)).ToList();
            var possibleEquationsSum = possibleEquations.Select(x => x.Result).Sum();

            Console.WriteLine($"Sum of possible equations results: {possibleEquationsSum}");

            var possibleEquationsPart2 = equations.FindAll(x => IsPossible(x, true)).ToList();
            var possibleEquationsSumPart2 = possibleEquationsPart2.Select(x => x.Result).Sum();

            Console.WriteLine($"Sum of possible equations results with concat: {possibleEquationsSumPart2}");
        }

        private static bool IsPossible(Equation equation, bool enableConcatenation = false)
        {
            var operations = new List<string> { "+", "*", "||" }.FindAll(x => enableConcatenation || x != "||");
            var variations = new Variations<string>(operations, equation.Numbers.Count - 1, GenerateOption.WithRepetition).ToList();
            foreach (var variation in variations)
            {
                var numbers = equation.Numbers.ToList();
                var accumulator = numbers[0];
                for (int i = 0; i < numbers.Count - 1; i++)
                {
                    var operation = variation[i];
                    var nextValue = numbers[i + 1];

                    if (operation == "+")
                        accumulator += nextValue;
                    else if (operation == "*")
                        accumulator *= nextValue;
                    else
                        accumulator = long.Parse($"{accumulator}{nextValue}");
                }

                if (accumulator == equation.Result)
                    return true;
            }

            return false;
        }

        private class Equation
        {
            public long Result { get; set; }
            public List<long> Numbers { get; set; }
        }
    }
}
