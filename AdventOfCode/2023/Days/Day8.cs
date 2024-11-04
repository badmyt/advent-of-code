using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day8 : DayBase
    {
        public Day8(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var moves = lines[0];

            var nodesList = lines.Skip(2).Select(x =>
            {
                var node = x.Substring(0, 3);
                var leftNode = x.Substring(7, 3);
                var rightNode = x.Substring(12, 3);
                return new Node(node, leftNode, rightNode);
            }).ToList();

            var nodes = nodesList.ToDictionary(x => x.Text, x => x);
            var target = "ZZZ";
            var currentNode = nodes["AAA"];

            // part1

            long steps = 0;
            for (int i = 0; i < moves.Length; i++)
            {
                steps++;

                var nextNodeText = moves[i] == 'L' ? currentNode.Left : currentNode.Right;
                currentNode = nodes[nextNodeText];

                if (currentNode.Text == target)
                    break;

                if (i == moves.Length - 1)
                {
                    i = -1;
                }
            }

            Console.WriteLine($"Steps required to reach ZZZ - {steps}");

            // part2

            var currentNodes = nodes.Where(x => x.Key[2] == 'A').Select(x => x.Value).ToList();
            var pathsToZ = new long[currentNodes.Count];

            steps = 0;
            for (int i = 0; i < moves.Length; i++)
            {
                steps++;

                for (var nodeIndex = 0; nodeIndex < currentNodes.Count; nodeIndex++)
                {
                    currentNode = currentNodes[nodeIndex];
                    var nextNodeText = moves[i] == 'L' ? currentNode.Left : currentNode.Right;
                    currentNodes[nodeIndex] = nodes[nextNodeText];

                    if (nextNodeText[2] == 'Z')
                    {
                        var currentPathToZ = pathsToZ[nodeIndex];
                        if (currentPathToZ <= 0)
                        {
                            pathsToZ[nodeIndex] = steps;
                        }
                    }
                }

                if (pathsToZ.All(x => x > 0))
                {
                    break;
                }

                if (i == moves.Length - 1)
                {
                    i = -1;
                }
            }

            var leastCommonMultiple = lcm_of_array_elements(pathsToZ);

            Console.WriteLine($"Steps for ghosts required to reach all points ending on 'Z' - {leastCommonMultiple}");
        }

        private class Node
        {
            public Node(string text, string left, string right)
            {
                Text = text;
                Left = left;
                Right = right;
            }
            public string Text { get; set; }
            public string Left { get; set; }
            public string Right { get; set; }

            public override string ToString()
            {
                return $"{Text} = ({Left}, {Right})";
            }

            public string GetNext(char direction)
            {
                if (direction == 'L')
                    return Left;

                if (direction == 'R')
                    return Right;

                throw new ArgumentException(nameof(direction));
            }
        }

        public static long lcm_of_array_elements(long[] element_array)
        {
            long lcm_of_array_elements = 1;
            long divisor = 2;

            while (true)
            {

                long counter = 0;
                bool divisible = false;
                for (long i = 0; i < element_array.Length; i++)
                {

                    // lcm_of_array_elements (n1, n2, ... 0) = 0.
                    // For negative number we convert into
                    // positive and calculate lcm_of_array_elements.
                    if (element_array[i] == 0)
                    {
                        return 0;
                    }
                    else if (element_array[i] < 0)
                    {
                        element_array[i] = element_array[i] * (-1);
                    }
                    if (element_array[i] == 1)
                    {
                        counter++;
                    }

                    // Divide element_array by devisor if complete
                    // division i.e. without remainder then replace
                    // number with quotient; used for find next factor
                    if (element_array[i] % divisor == 0)
                    {
                        divisible = true;
                        element_array[i] = element_array[i] / divisor;
                    }
                }

                // If divisor able to completely divide any number
                // from array multiply with lcm_of_array_elements
                // and store into lcm_of_array_elements and continue
                // to same divisor for next factor finding.
                // else increment divisor
                if (divisible)
                {
                    lcm_of_array_elements = lcm_of_array_elements * divisor;
                }
                else
                {
                    divisor++;
                }

                // Check if all element_array is 1 indicate 
                // we found all factors and terminate while loop.
                if (counter == element_array.Length)
                {
                    return lcm_of_array_elements;
                }
            }
        }
    }
}
