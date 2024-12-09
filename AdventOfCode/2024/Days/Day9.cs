using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    public class Day9 : DayBase
    {
        public Day9(int year) : base(year) { }

        public override void Run()
        {
            var map = System.IO.File.ReadAllLines(InputPath)[0];
            var expanded = ExpandMap(map);
            var reorganized_by_blocks = ReorganizeBlocks(expanded.Memory);
            var checksum1 = CheckSum(reorganized_by_blocks);

            Console.WriteLine($"Checksum of reorganized memory (by blocks): {checksum1}");

            var reorganized_by_files = ReorganizeFiles(expanded.Memory, expanded.Files);
            var checksum2 = CheckSum(reorganized_by_files);

            Console.WriteLine($"Checksum of reorganized memory (by files): {checksum2}");
        }

        private static List<string> ReorganizeFiles(List<string> memoryInput, List<File> files)
        {
            var memory = memoryInput.ToList();

            foreach (var file in files.OrderByDescending(x => x.Id))
            {
                var index = FindSpaceForFile(memory, file);
                if (index == -1)
                    continue;

                var fileBlockIndex = 0;
                var fileBlockLength = file.End - file.Start + 1;
                for (int i = index; i < index + fileBlockLength; i++)
                {
                    memory[i] = file.Id.ToString();
                    memory[file.Start + fileBlockIndex] = ".";
                    fileBlockIndex++;
                }
            }

            return memory;
        }

        private static int FindSpaceForFile(List<string> memory, File file)
        {
            var length = file.End - file.Start + 1;

            for (int i = 0; i < file.Start; i++)
            {
                bool isSpaceAvailable = true;

                for (int j = 0; j < length; j++)
                {
                    if ((i+j) >= file.Start || memory[i + j] != ".")
                    {
                        isSpaceAvailable = false;
                        break;
                    }
                }

                if (isSpaceAvailable)
                {
                    return i;
                }
            }

            return -1;
        }

        private static List<string> ReorganizeBlocks(List<string> memoryInput)
        {
            var memory = memoryInput.ToList();

            int leftIndex = 0, rightIndex = memory.Count - 1;
            while (leftIndex < rightIndex)
            {
                var leftElement = memory[leftIndex];
                while(leftElement != "." && leftIndex < rightIndex)
                {
                    leftIndex++;
                    leftElement = memory[leftIndex];
                }

                var rightElement = memory[rightIndex];
                while (rightElement == "." && rightIndex > leftIndex)
                {
                    rightIndex--;
                    rightElement = memory[rightIndex];
                }

                memory[leftIndex] = rightElement;
                memory[rightIndex] = leftElement;
            }

            return memory;
        }

        private static long CheckSum(List<string> memory)
        {
            long sum = 0;

            for (int i = 0; i < memory.Count; i++)
            {
                var id = memory[i];
                if (id != ".")
                    sum += i * int.Parse(memory[i]);
            }

            return sum;
        }

        private static (List<string> Memory, List<File> Files) ExpandMap(string denseMap)
        {
            var memory = new List<string>();
            var files = new List<File>();

            bool isFreeSpace = false;
            int fileId = 0;
            foreach (var block in denseMap)
            {
                var blockInt = int.Parse($"{block}");
                var id = isFreeSpace ? "." : fileId.ToString();

                var start = memory.Count;
                for (int i = 0; i < blockInt; i++)
                {
                    memory.Add(id);
                }
                var end = memory.Count - 1;

                if (!isFreeSpace)
                {
                    files.Add(new File
                    {
                        Id = fileId,
                        Start = start,
                        End = end
                    });
                    fileId++;
                }

                isFreeSpace = !isFreeSpace;
            }

            return (memory, files);
        }

        private class File
        {
            public int Id { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
        }
    }
}
