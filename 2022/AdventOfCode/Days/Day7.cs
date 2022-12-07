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

            int maxSearchedDirectoriesSize = 100000;
            int totalDiskSpace = 70000000;
            int requiredFreeSpace = 30000000;
            int maxUsableSpace = totalDiskSpace - requiredFreeSpace;

            var tree = BuildTreeFromCommands(commands);
            var resultDirectories = GetDirectoriesOfMaxSize(tree, maxSearchedDirectoriesSize);
            var sum = resultDirectories.Sum(x => x.GetSize());

            var usedSpace = tree.GetSize();
            var spaceToDelete = usedSpace - maxUsableSpace;

            var directoryToDelete = GetDirectoryForDeletion(tree, spaceToDelete);

            Console.WriteLine($"Sum of the total sizes of directories - {sum}");
            Console.WriteLine($"Size of min directory to be deleted - {directoryToDelete?.Size ?? 0}");
        }

        private FsNode BuildTreeFromCommands(string[] commands)
        {
            var root = FsNode.Folder("/", null);
            var currentNode = root;

            for (int i = 1; i < commands.Length; i++)
            {
                var command = commands[i];

                if (command.StartsWith("$ cd"))
                {
                    // changing directory
                    string argument = command.Substring("$ cd ".Length);
                    currentNode = currentNode.Cd(argument);
                }
                else if (command.StartsWith("$ ls"))
                {
                    // nothing to do
                }
                else if (command.StartsWith("dir"))
                {
                    // folder listed
                    string argument = command.Substring("dir ".Length);
                    currentNode.AddFolder(argument);
                }
                else
                {
                    // file listed
                    string[] arguments = command.Split(" ");
                    int fileSize = int.Parse(arguments[0]);
                    string fileName = arguments[1];

                    currentNode.AddFile(fileName, fileSize);
                }
            }

            return root;
        }

        private List<FsNode> GetDirectoriesOfMaxSize(FsNode root, int maxSize)
        {
            var resultDirectories = new List<FsNode>();

            root.Traverse(x =>
            {
                if (x.IsDirectory && x.GetSize() <= maxSize)
                {
                    resultDirectories.Add(x);
                }
            });

            return resultDirectories;
        }

        private FsNode GetDirectoryForDeletion(FsNode root, int minSize)
        {
            var directories = new List<FsNode>();

            root.Traverse(x =>
            {
                if (x.IsDirectory)
                {
                    directories.Add(x);
                }
            });

            var directoryToDelete = directories
                .Where(x => x.Size >= minSize)
                .OrderBy(x => x.Size)
                .FirstOrDefault();

            return directoryToDelete;
        }

        private class FsNode
        {
            public string Name { get; set; }

            public int? Size { get; set; }

            public bool IsDirectory { get; set; }

            public List<FsNode> Children { get; set; }

            public FsNode Parent { get; set; }

            public int GetSize()
            {
                Size ??= Children.Sum(x => x.GetSize());

                return Size.Value;
            }

            public void Traverse(Action<FsNode> action)
            {
                action(this);

                Children?.ForEach(x => x.Traverse(action));
            }

            public FsNode Cd(string destination)
            {
                if (destination == "..")
                {
                    if (Parent == null)
                    {
                        throw new Exception("Cannot go higher in hierarchy");
                    }

                    return Parent;
                }

                var child = Children.FirstOrDefault(x => x.Name?.ToLower() == destination.ToLower());
                if (child != null)
                {
                    return child;
                }

                var newFolder = Folder(destination, this);
                Children.Add(newFolder);
                return newFolder;
            }

            public FsNode AddFolder(string folderName)
            {
                if (!Children.Any(x => x.Name.ToLower() == folderName.ToLower()))
                {
                    Children.Add(Folder(folderName, this));
                }

                return this;
            }

            public FsNode AddFile(string name, int size)
            {
                if (!Children.Any(x => x.Name.ToLower() == name.ToLower()))
                {
                    Children.Add(File(name, size, this));
                }

                return this;
            }

            public static FsNode Folder(string name, FsNode baseNode)
            {
                return new FsNode { Name = name, IsDirectory = true, Children = new List<FsNode>(), Parent = baseNode };
            }

            public static FsNode File(string name, int size, FsNode baseNode)
            {
                return new FsNode { Name = name, Size = size, Parent = baseNode };
            }

            public override string ToString()
            {
                return string.Join(" - ", new[] { Name, Size?.ToString() }.Where(x => x != null));
            }
        }
    }
}
