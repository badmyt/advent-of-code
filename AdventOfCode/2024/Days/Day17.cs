using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day17 : DayBase
    {
        public Day17(int year) : base(year)
        {
            _instructions = new List<Action<long>> { adv, bxl, bst, jnz, bxc, @out, bdv, cdv };
        }

        private long A, B, C;
        private int _pointer;
        private bool _skip = false;
        private List<long> _program = new();
        private List<long> _initialProgram = new();
        private string _initialProgramString = "";
        private StringBuilder _output = new StringBuilder();
        private List<Action<long>> _instructions = new();

        public override void Run()
        {
            Init(File.ReadAllLines(InputPath));
            Console.WriteLine("----------------------------------------------" + "\n");
            Console.WriteLine("Input:\n" + File.ReadAllText(InputPath) + "\n");

            RunPart1();
            RunPart2();
        }

        private void RunPart1()
        {
            Console.WriteLine("----------------------------------------------" + "\n");

            //part 1
            var length = _program.Count;
            while (_pointer < length)
            {
                // remove
                //A = 1;

                var opcode = (int)_program[_pointer];
                var operand = _program[_pointer + 1];
                _instructions[opcode](operand);
                _pointer += 2;
            }
            Console.WriteLine("Part 1 Output: \n" + _output.ToString() + "\n");
            Console.WriteLine("----------------------------------------------" + "\n");
        }

        private void RunPart2()
        {
            long A_register = 0;

            for (long i = 0; i < 10; i++)
            {
                _output = new StringBuilder();
                _skip = false;
                _pointer = 0;
                A = i;
                B = 0;
                C = 0;

                var length = _program.Count;
                while (_pointer < length)
                {
                    var opcode = (int)_program[_pointer];
                    var operand = _program[_pointer + 1];
                    Action<long> instruction = opcode switch
                    {
                        0 => adv,
                        1 => bxl,
                        2 => bst,
                        3 => jnz,
                        4 => bxc,
                        5 => @out_part2,
                        6 => bdv,
                        7 => cdv,
                        _ => throw new InvalidOperationException($"Invalid opcode {opcode}")
                    };
                    instruction(operand);
                    if (_skip)
                    {
                        break;
                    }

                    _pointer += 2;
                }

                if (!_skip && _output.ToString() == _initialProgramString)
                {
                    A_register = i;
                    break;
                }
            }

            Console.WriteLine("Part 2 Output: \n" + _output.ToString());
            Console.WriteLine($"\nA register: {A_register}\n");
            Console.WriteLine("----------------------------------------------" + "\n");
        }

        private void adv(long operand)
        {
            var combo = OperandVal(operand);
            var result = A / Math.Pow(2, combo);
            A = (long)Math.Truncate(result);
        }

        private void bxl(long operand)
        {
            B ^= operand;
        }

        private void bst(long operand)
        {
            var combo = OperandVal(operand);
            B = combo % 8;
        }

        private void jnz(long operand)
        {
            if (A == 0) return;
            _pointer = (int)operand - 2;
        }

        private void bxc(long operand)
        {
            B ^= C;
        }

        private void @out(long operand)
        {
            var combo = OperandVal(operand);
            var result = combo % 8;

            if (_output.Length == 0)
            {
                _output.Append(result);
            }
            else
            {
                _output.Append(",");
                _output.Append(result);
            }
        }

        private void @out_part2(long operand)
        {
            var combo = OperandVal(operand);
            var result = combo % 8;

            if (_output.Length == 0)
            {
                if (!_initialProgramString.StartsWith(result.ToString()))
                {
                    _skip = true;
                }

                _output.Append(result);
            }
            else
            {
                _output.Append(",");
                _output.Append(result);

                if (!_initialProgramString.StartsWith(_output.ToString()))
                {
                    _skip = true;
                }
            }
        }

        private void bdv(long operand)
        {
            var combo = OperandVal(operand);
            var result = A / Math.Pow(2, combo);
            B = (long)Math.Truncate(result);
        }

        private void cdv(long operand)
        {
            var combo = OperandVal(operand);
            var result = A / Math.Pow(2, combo);
            C = (long)Math.Truncate(result);
        }

        private long OperandVal(long operand)
        {
            return operand switch
            {
                >= 0 and <= 3 => operand,
                4 => A,
                5 => B,
                6 => C,
                _ => throw new InvalidOperationException($"invalid operand {operand}")
            };
        }

        private void Init(string[] lines)
        {
            A = long.Parse(lines[0].Split(": ")[1]);
            B = long.Parse(lines[1].Split(": ")[1]);
            C = long.Parse(lines[2].Split(": ")[1]);
            _program = lines[4].Split(": ")[1].Split(",").Select(long.Parse).ToList();
            _initialProgram = lines[4].Split(": ")[1].Split(",").Select(long.Parse).ToList();
            _initialProgramString = string.Join(",", _initialProgram);
        }
    }
}
