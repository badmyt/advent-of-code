using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day21 : DayBase
    {
        public Day21(int year) : base(year) { }

        static readonly string[,] originalKeypad = {
                { "7", "8", "9" },
                { "4", "5", "6" },
                { "1", "2", "3" },
                { null, "0", "A" }
            };

        static readonly string[,] newKeypad = {
                { null, "^", "A" },
                { "<", "v", ">" }
            };

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);

            //var test = "<<^";
            //var path = FindKeypadSequence(test, "A");
            //var path2 = FindKeypadSequence(path, "A");

            var result = new List<(string, string)>();

            foreach (var line in lines)
            {
                var doorSymbol = "A";
                var robot1Pos = "A";
                var robot2Pos = "A";
                var personPos = "A";

                var totalPath = "";
                foreach (var nextSymbol in line)
                {
                    var doorPath = KeypadPathfinder.ShortestPathOnDoor(doorSymbol, $"{nextSymbol}");
                    doorSymbol = $"{nextSymbol}";

                    var robot1Path = FindKeypadSequence(doorPath, robot1Pos);
                    robot1Pos = LastSymbol(robot1Path);

                    var robot2Path = FindKeypadSequence(robot1Path, robot2Pos);
                    robot2Pos = LastSymbol(robot2Path);

                    var personPath = FindKeypadSequence(robot2Path, personPos);
                    personPos = LastSymbol(personPath);

                    totalPath += personPath;
                }
                result.Add((line, totalPath));
            }

            foreach (var item in result)
            {
                Console.WriteLine($"{item.Item1}: {item.Item2}");
            }
        }

        private static string FindKeypadSequence(string doorSequence, string currentSymbol)
        {
            var totalPath = "";

            foreach (var symbol in doorSequence)
            {
                var pathToNextSymbol = KeypadPathfinder.ShortestPathOnKeypad(currentSymbol, $"{symbol}");
                totalPath += pathToNextSymbol + "A";
                currentSymbol = $"{symbol}";
            }

            return totalPath;
        }

        private static string LastSymbol(string text)
        {
            return $"{text[^1]}";
        }

        class KeypadPathfinder
        {
            static readonly string[,] originalKeypad = {
                { "7", "8", "9" },
                { "4", "5", "6" },
                { "1", "2", "3" },
                { null, "0", "A" }
            };

            static readonly string[,] newKeypad = {
                { null, "^", "A" },
                { "<", "v", ">" }
            };

            static readonly int[] dRow = { -1, 0, 1, 0 };
            static readonly int[] dCol = { 0, 1, 0, -1 };
            static readonly char[] directionSymbols = { '^', '>', 'v', '<' };

            class Node
            {
                public int Row, Col;
                public string Path;

                public Node(int row, int col, string path)
                {
                    Row = row;
                    Col = col;
                    Path = path;
                }
            }

            public static string ShortestPathOnDoor(string start, string target)
            {
                (int startRow, int startCol) = FindPosition(originalKeypad, start);
                (int targetRow, int targetCol) = FindPosition(originalKeypad, target);

                if (startRow == -1 || targetRow == -1)
                    return null;

                var visited = new bool[originalKeypad.GetLength(0), originalKeypad.GetLength(1)];
                var queue = new Queue<Node>();
                queue.Enqueue(new Node(startRow, startCol, ""));
                visited[startRow, startCol] = true;

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    if (current.Row == targetRow && current.Col == targetCol)
                        return current.Path;

                    for (int i = 0; i < 4; i++)
                    {
                        int newRow = current.Row + dRow[i];
                        int newCol = current.Col + dCol[i];

                        if (IsValid(originalKeypad, newRow, newCol) && !visited[newRow, newCol])
                        {
                            visited[newRow, newCol] = true;
                            string newPath = current.Path + directionSymbols[i];
                            queue.Enqueue(new Node(newRow, newCol, newPath));
                        }
                    }
                }

                return null;
            }

            public static string ShortestPathOnKeypad(string start, string target)
            {
                if (start == target)
                    return "";

                (int startRow, int startCol) = FindPosition(newKeypad, start);
                (int targetRow, int targetCol) = FindPosition(newKeypad, target);

                if (startRow == -1 || targetRow == -1)
                    return null;

                var visited = new bool[newKeypad.GetLength(0), newKeypad.GetLength(1)];
                var queue = new Queue<Node>();
                queue.Enqueue(new Node(startRow, startCol, ""));
                visited[startRow, startCol] = true;

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    if (current.Row == targetRow && current.Col == targetCol)
                        return current.Path;

                    for (int i = 0; i < 4; i++)
                    {
                        int newRow = current.Row + dRow[i];
                        int newCol = current.Col + dCol[i];

                        if (IsValid(newKeypad, newRow, newCol) && !visited[newRow, newCol])
                        {
                            visited[newRow, newCol] = true;
                            string newPath = current.Path + directionSymbols[i];
                            queue.Enqueue(new Node(newRow, newCol, newPath));
                        }
                    }
                }

                return null;
            }

            static (int, int) FindPosition(string[,] keypad, string key)
            {
                for (int row = 0; row < keypad.GetLength(0); row++)
                {
                    for (int col = 0; col < keypad.GetLength(1); col++)
                    {
                        if (keypad[row, col] == key)
                            return (row, col);
                    }
                }
                return (-1, -1);
            }

            static bool IsValid(string[,] keypad, int row, int col)
            {
                return row >= 0 && row < keypad.GetLength(0) &&
                       col >= 0 && col < keypad.GetLength(1) &&
                       keypad[row, col] != null;
            }
        }
    }
}
