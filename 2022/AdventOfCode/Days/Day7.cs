using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AreaOfCode.Days
{
    public class Day7 : DayBase
    {
        public override void Run()
        {
            var commands = File.ReadAllLines(InputPath);

            var maxSearchedDirectoriesSize = 100000;
            var maxUsableSpace = 40000000;

            var tree = BuildTreeFromCommands(commands);

            var resultDirectoriesSize = GetDirectoriesSizeSum(tree, maxSearchedDirectoriesSize);
            var directoryToDelete = GetDirectoryForDeletion(tree, tree.GetSize() - maxUsableSpace);

            Console.WriteLine($"Sum of the total sizes of directories - {resultDirectoriesSize}");
            Console.WriteLine($"Size of min directory to be deleted - {directoryToDelete?.Size}");
        }

        private FsNode BuildTreeFromCommands(string[] commands)
        {
            var root = new FsNode { Name = "/", IsDirectory = true };
            var currentNode = root;

            foreach (string command in commands)
            {
                if (command.StartsWith("$ cd"))
                {
                    currentNode = currentNode.Cd(command["$ cd ".Length..]);
                }
                else if (command.StartsWith("$ ls")) { }
                else if (command.StartsWith("dir"))
                {
                    currentNode.AddNode(command["dir ".Length..]);
                }
                else
                {
                    string[] arguments = command.Split(" ");
                    currentNode.AddNode(arguments[1], int.Parse(arguments[0]));
                }
            }

            return root;
        }

        private int GetDirectoriesSizeSum(FsNode root, int maxSize)
        {
            return root.Traverse().Where(x => x.IsDirectory && x.GetSize() <= maxSize).Sum(x => x.Size.Value);
        }

        private FsNode GetDirectoryForDeletion(FsNode root, int minSize)
        {
            return root.Traverse().Where(x => x.IsDirectory && x.Size >= minSize).OrderBy(x => x.Size).FirstOrDefault();
        }

        private class FsNode
        {
            public string Name { get; set; }
            public int? Size { get; set; }
            public bool IsDirectory { get; set; }
            public FsNode Parent { get; set; }
            public List<FsNode> Children { get; set; } = new List<FsNode>();

            public IEnumerable<FsNode> Traverse()
            {
                foreach (FsNode child in Children)
                    foreach (FsNode n in child.Traverse())
                        yield return n;

                yield return this;
            }

            public FsNode Cd(string destination)
            {
                if (destination == "..")
                    return Parent ?? this;

                var child = Children.FirstOrDefault(x => x.Name == destination);
                if (child != null)
                    return child;

                var newFolder = new FsNode { Name = destination, IsDirectory = true, Parent = this };
                Children.Add(newFolder);
                return newFolder;
            }

            public int GetSize()
            {
                Size ??= Children.Sum(x => x.GetSize());
                return Size.Value;
            }

            public void AddNode(string name, int? size = null)
            {
                Children.Add(new FsNode { Name = name, Size = size, IsDirectory = size == null, Parent = this });
            }
        }
    }
}
