using System;
using System.Collections.Generic;

namespace AdventOfCode.Days
{
    public class DayX : IDay
    {
        public DayX()
        {
            
        }

        public void Run()
        {
            // part 1

            List<int> list1 = new List<int>() { 1, 2, 3, 1, 1, 2 };
            RemoveDuplicates(list1);


            Console.WriteLine();
            foreach (var item in list1)
            {
                Console.Write(item + ", ");
            }
            Console.WriteLine();

            // should be - 1, 2, 3

            // part 2
            List<int> list2 = new List<int>() { 7, 2, 4, -1, -2, -4, 0, 0, 9, +1, 7 };
            var result2 = MaxValueHavingNegativeBrother(list2);
            Console.WriteLine("part 2 result - " + result2);
            // should be 4
            var list200 = GenerateListOfInt(200_000);
            var result200 = MaxValueHavingNegativeBrother(list200);
            Console.WriteLine("part 2 result on 200k - " + result200);
        }

        public static void RemoveDuplicates(List<int> list)
        {
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    if (list[i] == list[j])
                    {
                        list.RemoveAt(j);
                        count--;
                        j--;
                    }
                }
            }
        }

        public static int MaxValueHavingNegativeBrother(List<int> list)
        {
            var set = new HashSet<int>(list);
            var max = 0;

            foreach (var number in list)
            {
                if (number > max && set.Contains(number * -1))
                {
                    max = number;
                }
            }

            return max;
        }

        static List<int> GenerateListOfInt(int size)
        {
            int halfSize = size / 2;
            List<int> lst1 = new List<int>();
            List<int> lst2 = new List<int>();
            for (int i = 0; i < halfSize; i++)
            {
                lst1.Add(-2 * (halfSize - i) + 1);
                lst2.Add((i == 0) ? 1 : 2 * i);
            }
            lst1.AddRange(lst2);
            if (lst1.Count < size)
                lst1.Add(size);
            return lst1;
        }
    }
}
