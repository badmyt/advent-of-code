﻿using System;
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
                    Console.Write(array[i, j]);
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
    }
}
