using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day4 : DayBase
    {
        public Day4(int year) : base(year) { }

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);

            var totalPoints = 0;
            var cardCounts = new int[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                cardCounts[i] = 1;
            }
            
            for (var i = 0; i < lines.Length; i++)
            {
                var allCards = lines[i].Split(": ").Skip(1).First().Split(" | ").ToArray();
                var winningCards = allCards[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                var myCards = allCards[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                var intersection = winningCards.Intersect(myCards).Count();
                if (intersection <= 0)
                    continue;
                
                totalPoints += Convert.ToInt32(Math.Pow(2, intersection-1));

                var currentCardCount = cardCounts[i];
                for (var j = i + 1; j < lines.Length && j <= i + intersection; j++)
                {
                    cardCounts[j] += currentCardCount;
                }
            }

            var cardsCollected = cardCounts.Sum();
            
            Console.WriteLine($"Total points in pile of cards: {totalPoints}");
            Console.WriteLine($"Cards collected: {cardsCollected}");
        }
    }
}