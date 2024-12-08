using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int length, int chunkSize)
        {
            for (var i = 0; i < (float)length / chunkSize; i++)
            {
                yield return array.Skip(i * chunkSize).Take(chunkSize);
            }
        }

        public static IEnumerable<T[]> SplitAsArrays<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size).ToArray();
            }
        }

        public static IEnumerable<T> SkipAt<T>(this IEnumerable<T> collection, int elementIndex)
        {
            return collection.Where((value, index) => index != elementIndex);
        }

        public static int GetHeight<T>(this T[,] array)
        {
            return array.GetLength(0);
        }

        public static int GetWidth<T>(this T[,] array)
        {
            return array.GetLength(1);
        }

        public static void Print<T>(this T[,] array)
        {
            int height = array.GetHeight();
            int width = array.GetWidth();

            for (int i = 0; i < height; i++)
            {
                Console.WriteLine();

                for (int j = 0; j < width; j++)
                {
                    var item = $"{array[i, j]}";
                    if (item == "True")
                        item = "1";
                    if (item == "False")
                        item = "0";

                    Console.Write(item);
                }
            }

            Console.WriteLine("\n");
        }

        public static void Print<T>(List<List<T>> squareArray)
        {
            int height = squareArray.Count;
            int width = squareArray[0].Count;

            for (int i = 0; i < height; i++)
            {
                Console.WriteLine();

                for (int j = 0; j < width; j++)
                {
                    var item = $"{squareArray[i][j]}";
                    if (item == "True")
                        item = "1";
                    if (item == "False")
                        item = "0";

                    Console.Write(item);
                }
            }

            Console.WriteLine("\n");
        }

        public static void Print<T>(T[,] array1, T[,] array2)
        {
            int height1 = array1.GetLength(0);
            int width1 = array1.GetLength(1);
            int height2 = array2.GetLength(0);
            int width2 = array2.GetLength(1);

            // Determine the max height to print all rows
            int maxHeight = Math.Max(height1, height2);

            for (int i = 0; i < maxHeight; i++)
            {
                // Print row from the first array
                for (int j = 0; j < width1; j++)
                {
                    if (i < height1)  // Check if we're within bounds of array1
                    {
                        var item = $"{array1[i, j]}";
                        if (item == "True") item = "1";
                        if (item == "False") item = "0";
                        Console.Write(item);
                    }
                    else
                    {
                        Console.Write(" ");  // Print space if outside bounds of array1
                    }
                }

                // Add whitespace between the arrays
                Console.Write(new string(' ', 10));

                // Print row from the second array
                for (int j = 0; j < width2; j++)
                {
                    if (i < height2)  // Check if we're within bounds of array2
                    {
                        var item = $"{array2[i, j]}";
                        if (item == "True") item = "1";
                        if (item == "False") item = "0";
                        Console.Write(item);
                    }
                    else
                    {
                        Console.Write(" ");  // Print space if outside bounds of array2
                    }
                }

                Console.WriteLine();  // Move to the next row
            }

            Console.WriteLine();  // Extra line at the end
        }

        public static T[,] PopulateWith<T>(this T[,] array, T val)
        {
            int height = array.GetHeight();
            int width = array.GetWidth();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    array[i,j] = val;
                }
            }

            return array;
        }

        public static T[,] SubArray<T>(this T[,] array, int rowStart, int rowTake, int colStart, int colTake)
        {
            var result = new T[rowTake, colTake];

            for (int i = 0; i < rowTake; i++)
            {
                for (int j = 0; j < colTake; j++)
                {
                    result[i, j] = array[rowStart + i, colStart + j];
                }
            }

            return result;
        }

        public static void PrintVisitMap(this char[,] array)
        {
            int height = array.GetHeight();
            int width = array.GetWidth();

            for (int i = 0; i < height; i++)
            {
                Console.WriteLine();

                for (int j = 0; j < width; j++)
                {
                    var item = array[i, j];
                    if (item == default(char))
                        item = '.';

                    Console.Write(item);
                }
            }

            Console.WriteLine("\n");
        }

        public static int Sum(this int[,] array)
        {
            int height = array.GetHeight();
            int width = array.GetWidth();

            int sum = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sum += array[i, j];
                }
            }

            return sum;
        }

        public static int Max(this int[,] array)
        {
            int height = array.GetHeight();
            int width = array.GetWidth();

            int max = int.MinValue;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (array[i,j] > max)
                    {
                        max = array[i, j];
                    }
                }
            }

            return max;
        }

        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            TV value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static bool[][] GetBooleanPermutations(int n)
        {
            List<bool[]> matrix = new List<bool[]>();
            double count = Math.Pow(2, n);
            for (int i = 0; i < count; i++)
            {
                string str = Convert.ToString(i, 2).PadLeft(n, '0');
                bool[] boolArr = str.Select((x) => x == '1').ToArray();
                matrix.Add(boolArr);
            }

            bool[][] arr = matrix.ToArray();
            return arr;
        }
    }
}
