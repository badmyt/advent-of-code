using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
            RunPart2BruteForce();

            Console.WriteLine("Write octal number to parse it, write 'exit' to exit");
            string line = Console.ReadLine();
            while (line != "exit")
            {
                RunForNumberOctal(line);
                line = Console.ReadLine();
            }
        }
        
        private void RunPart2BruteForce()
        {
            // var startNumberOctal = "5611134250025052";

            // 4 digits
            //var leftPart = "561113";
            //var rightPart = "025052";

            // 6 digits
            //var leftPart = "56111";
            //var rightPart = "25052";

            // 8 digits
            var leftPart = "";
            var rightPart = "62025052";

            for (int i = 0; i < 16777216; i++) // 16777216 is 8^8 (00000000 to 77777777 in octal)
            {
                string middlePart = Convert.ToString(i, 8).PadLeft(8, '0');
                string fullNumberOctal = leftPart + middlePart + rightPart;

                RunForNumberOctal(fullNumberOctal, false);
            }
        }

        private void RunPart1()
        {
            Console.WriteLine("----------------------------------------------" + "\n");

            //part 1
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
                    5 => @out,
                    6 => bdv,
                    7 => cdv,
                    _ => throw new InvalidOperationException($"Invalid opcode {opcode}")
                };
                instruction(operand);
                _pointer += 2;
            }
            Console.WriteLine("Part 1 Output: \n" + _output.ToString() + "\n");
            Console.WriteLine("----------------------------------------------" + "\n");
        }

        private void RunForNumber(long number, bool print = true)
        {
            _output = new StringBuilder();
            _skip = false;
            _pointer = 0;
            A = number;
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
                    5 => @out,
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
            
            var binary = Convert.ToString(number, 2);
            var octal = ConvertToOctal(number);

            if (print || _initialProgramString == _output.ToString())
            {
                Console.WriteLine($"{number} --->>> (oct){octal} --->>> {_output}");

                if (_initialProgramString == _output.ToString())
                {
                    Console.WriteLine("^^^^^^^^ ANSWER FOUND ^^^^^^");
                }
            }
        }

        static string ConvertToOctal(long number)
        {
            if (number == 0) return "0";
            number = Math.Abs(number); 
            string octal = "";
            while (number > 0)
            {
                octal = (number % 8) + octal;
                number /= 8;
            }
            return number < 0 ? "-" + octal : octal; 
        }

        private void RunForNumberOctal(string octalNumber, bool print = true)
        {
            var longNumber = Convert.ToInt64(octalNumber, 8);
            RunForNumber(longNumber, print);
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
