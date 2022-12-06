using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AreaOfCode.Days
{
    public class Day5 : DayBase
    {
        private string _alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);

            // create stacks of boxes

            var crateBox = lines.Where(x => !x.StartsWith("move") && x.Length > 2 && !x.Contains('1')).ToArray();

            var stacksCount = lines
                .First(x => x.Contains("1"))
                .Split(" ", System.StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x))
                .Max();

            var stacksPartOne = BuildStacksFromText(crateBox, stacksCount);
            var stacksPartTwo = BuildStacksFromText(crateBox, stacksCount);

            // do the moves

            var moves = lines.Where(x => x.StartsWith("move")).ToArray();

            MoveCargoes(stacksPartOne, moves);
            MoveCargoesPreserveOrder(stacksPartTwo, moves);

            // results

            var crateMessageOne = string.Join("", stacksPartOne.Where(x => x.Any()).Select(x => x.Pop()));
            var crateMessageTwo = string.Join("", stacksPartTwo.Where(x => x.Any()).Select(x => x.Pop()));

            Console.WriteLine($"Crates message after rearrangement with CrateMover 9000 - {crateMessageOne}");
            Console.WriteLine($"Crates message after rearrangement with CrateMover 9001 - {crateMessageTwo}");
        }

        private void MoveCargoes(List<Stack<char>> stacks, string[] moves)
        {
            foreach (string move in moves)
            {
                var moveData = move.Split(" ").Where(x => int.TryParse(x, out int number)).ToArray();

                var amount = int.Parse(moveData[0]);
                var fromStack = int.Parse(moveData[1]) - 1;
                var toStack = int.Parse(moveData[2]) - 1;

                for (int i = 0; i < amount; i++)
                {
                    char crate = stacks[fromStack].Pop();
                    stacks[toStack].Push(crate);
                }
            }
        }

        private void MoveCargoesPreserveOrder(List<Stack<char>> stacks, string[] moves)
        {
            foreach (string move in moves)
            {
                var moveData = move.Split(" ").Where(x => int.TryParse(x, out int number)).ToArray();

                var amount = int.Parse(moveData[0]);
                var fromStack = int.Parse(moveData[1]) - 1;
                var toStack = int.Parse(moveData[2]) - 1;

                var tempStack = new Stack<char>();

                for (int i = 0; i < amount; i++)
                {
                    char crate = stacks[fromStack].Pop();
                    tempStack.Push(crate);
                }

                for (int i = 0; i < amount; i++)
                {
                    char crate = tempStack.Pop();
                    stacks[toStack].Push(crate);
                }
            }
        }

        private List<Stack<char>> BuildStacksFromText(string[] crateBox, int stacksCount)
        {
            var stacks = new List<Stack<char>>();
            for (int i = 0; i < stacksCount; i++)
            {
                stacks.Add(new Stack<char>());
            }

            for (int row = crateBox.Length - 1; row >= 0; row--)
            {
                var crateLine = crateBox[row];
                int stackIndex = 0;

                for (int col = 1; col < crateLine.Length; col += 4)
                {
                    char crate = crateLine[col];

                    if (_alpha.Contains(crate))
                    {
                        stacks[stackIndex].Push(crate);
                    }

                    stackIndex++;
                }
            }

            return stacks;
        }
    }
}
