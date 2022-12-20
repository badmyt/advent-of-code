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

        public static IEnumerable<T[]> SplitAsArrays<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size).ToArray();
            }
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

        /* public static TResult Traverse<TResult, TItem>(this TItem[,] array)
        {

        }*/

        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            TV value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}
