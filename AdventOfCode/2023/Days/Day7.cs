using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using static AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day7 : DayBase
    {
        public Day7(int year) : base(year) { }

        private const string StrengthOrder = "AKQJT98765432";
        private const string StrengthOrderPartTwo = "AKQT98765432J";

        private const bool EnableJoker = true;

        public override void Run()
        {
            var lines = File.ReadAllLines(InputPath);
            var hands = lines
                .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray())
                .Select(x => new Hand(x[0], long.Parse(x[1])))
                .OrderBy(x => x)
                .ToList();

            long totalWinnings = 0;
            for (int i = 0; i < hands.Count; i++)
            {
                totalWinnings += hands[i].Bid * (i + 1);
            }

            Console.WriteLine($"Total winnings with joker-enabled-{EnableJoker}: {totalWinnings}");

            // 251891094 - too high
            // 251515496 - correct
            // 251335624 - too low

            var text = hands.Select(x => x.Value + "\t" + x.HandType.ToString() + "\t" + x.Bid).ToList();
            File.WriteAllLines("debug_my.txt", text);

            //debug
            var oneCard = hands.FindAll(x => x.HandType == HandType.HighCard);
            var onePair = hands.FindAll(x => x.HandType == HandType.OnePair);
            var twoPairs = hands.FindAll(x => x.HandType == HandType.TwoPair);
            var fullHouse = hands.FindAll(x => x.HandType == HandType.FullHouse);
            var fourCards = hands.FindAll(x => x.HandType == HandType.FourCards);
            var fiveCards = hands.FindAll(x => x.HandType == HandType.FiveCards);

            var handsWithJoker = hands.FindAll(x => x.Value.Contains('J'));
            var joker_oneCard = handsWithJoker.FindAll(x => x.HandType == HandType.HighCard);
            var joker_onePair = handsWithJoker.FindAll(x => x.HandType == HandType.OnePair);
            var joker_twoPairs = handsWithJoker.FindAll(x => x.HandType == HandType.TwoPair);
            var joker_fullHouse = handsWithJoker.FindAll(x => x.HandType == HandType.FullHouse);
            var joker_fourCards = handsWithJoker.FindAll(x => x.HandType == HandType.FourCards);
            var joker_fiveCards = handsWithJoker.FindAll(x => x.HandType == HandType.FiveCards);
        }

        private enum HandType
        {
            HighCard = 0,
            OnePair = 1,
            TwoPair = 2,
            ThreeCards = 3,
            FullHouse = 4,
            FourCards = 5,
            FiveCards = 6
        }

        private class Hand : IComparable<Hand>
        {
            public override string ToString()
            {
                return $"{Value}\t {HandType}\t {Bid}";
            }

            public string ToStringShort()
            {
                return $"{Value}\t {HandType}";
            }

            public string Value { get; set; }
            public HandType HandType { get; set; }
            public long Bid { get; set; }

            private Hand InitNormal()
            {
                var grouped = Value
                    .GroupBy(x => x)
                    .ToDictionary(g => g.Key, g => (Card: g.Key, Count: g.Count()))
                    .OrderByDescending(g => g.Value.Count);

                foreach (var grouping in grouped)
                {
                    if (grouping.Value.Count == 5)
                    {
                        HandType = HandType.FiveCards;
                        return this;
                    }

                    if (grouping.Value.Count == 4)
                    {
                        HandType = HandType.FourCards;
                        return this;
                    }

                    if (grouping.Value.Count == 3)
                    {
                        HandType = HandType.ThreeCards;
                        break;
                    }
                }

                // at this point it either 3cards or not set
                // check for full house
                if (grouped.Any(x => x.Value.Count == 3) && grouped.Any(x => x.Value.Count == 2))
                {
                    HandType = HandType.FullHouse;
                    return this;
                }

                // return if 3cards
                if (HandType == HandType.ThreeCards)
                    return this;

                // now it can be only 1-2 pairs or highcard
                var pairCount = grouped.Count(x => x.Value.Count == 2);
                if (pairCount == 2)
                {
                    HandType = HandType.TwoPair;
                    return this;
                }
                else if (pairCount == 1)
                {
                    HandType = HandType.OnePair;
                    return this;
                }

                HandType = HandType.HighCard;
                return this;
            }

            private Hand InitWithJoker()
            {
                var grouped = Value
                    .GroupBy(x => x)
                    .ToDictionary(g => g.Key, g => (Card: g.Key, Count: g.Count()))
                    .OrderByDescending(g => g.Value.Count);

                if (Value == "J3Q3K")
                {
                    //debug
                }
                
                bool checkedFor4Jokers = false,
                     checkedFor3Jokers = false;

                var haveJokers = grouped.Any(x => x.Value.Card == 'J');
                var jokersCount = haveJokers ? grouped.First(x => x.Value.Card == 'J').Value.Count : 0;
                if (grouped.Any(x => x.Value.Card != 'J' && x.Value.Count + jokersCount == 5))
                {
                    HandType = HandType.FiveCards;
                    return this;
                }
                if (grouped.Any(x => x.Value.Card != 'J' && x.Value.Count + jokersCount == 4))
                {
                    HandType = HandType.FourCards;
                    return this;
                }

                foreach (var grouping in grouped)
                {
                    if (grouping.Value.Count == 5)
                    {
                        HandType = HandType.FiveCards;
                        return this;
                    }

                    if (!checkedFor4Jokers && grouped.Any(x => x.Value.Card == 'J' && x.Value.Count >=4))
                    {
                        HandType = HandType.FiveCards;
                        return this;
                    }
                    else { checkedFor4Jokers = true; }

                    if (grouping.Value.Count == 4)
                    {
                        if (grouping.Value.Card != 'J' && grouped.Any(x => x.Value.Card == 'J'))
                        {
                            HandType = HandType.FiveCards;
                            return this;
                        }

                        HandType = HandType.FourCards;
                        return this;
                    }

                    if (!checkedFor3Jokers && grouped.Any(x => x.Value.Card == 'J' && x.Value.Count >= 3))
                    {
                        HandType = HandType.FourCards;
                        return this;
                    }
                    else { checkedFor3Jokers = true; }

                    if (grouping.Value.Count == 3)
                    {
                        if (grouped.Any(x => x.Value.Card == 'J' && x.Value.Count == 2))
                        {
                            HandType = HandType.FiveCards;
                            return this;
                        }

                        if (grouping.Value.Card != 'J' && grouped.Any(x => x.Value.Card == 'J'))
                        {
                            HandType = HandType.FourCards;
                            return this;
                        }

                        HandType = HandType.ThreeCards;
                        break;
                    }
                }

                //check for 4cards with jokers
                if (grouped.Any(x => x.Value.Card != 'J' && x.Value.Count == 2) && grouped.Any(x => x.Value.Card == 'J' && x.Value.Count == 2))
                {
                    HandType = HandType.FourCards;
                    return this;
                }

                // at this point it either 3cards or not set
                // check for full house
                if (grouped.Any(x => x.Value.Count == 3) && grouped.Any(x => x.Value.Count == 2))
                {
                    HandType = HandType.FullHouse;
                    return this;
                }

                //check for full house with jokers
                // 3 + 1 + J
                if (grouped.Any(x => x.Value.Count == 3 && x.Value.Card != 'J') && grouped.Any(x => x.Value.Card == 'J'))
                {
                    HandType = HandType.FullHouse;
                    return this;
                }

                // 2 + 2 + j
                var twoCardGroups = grouped.Where(x => x.Value.Count == 2).ToList();
                if (twoCardGroups.Count == 2 && twoCardGroups.All(x => x.Value.Card != 'J') && grouped.Any(x => x.Value.Card == 'J'))
                {
                    HandType = HandType.FullHouse;
                    return this;
                }

                // return if 3cards
                if (HandType == HandType.ThreeCards)
                    return this;

                // check for 3 cards with joker
                foreach (var twoCardGroup in twoCardGroups)
                {
                    if (twoCardGroup.Value.Card != 'J' && grouped.Any(x => x.Value.Card == 'J'))
                    {
                        HandType = HandType.ThreeCards; 
                        return this;
                    }
                }

                // check for 2 jokers with any card
                if (grouped.Any(x => x.Value.Card == 'J' && x.Value.Count >= 2))
                {
                    HandType = HandType.ThreeCards;
                    return this;
                }

                // now it can be only 1-2 pairs or highcard
                var pairs = grouped.Where(x => x.Value.Count == 2).ToList();
                if (pairs.Count == 2)
                {
                    HandType = HandType.TwoPair;
                    return this;
                }
                else if (pairs.Count == 1)
                {
                    if (pairs[0].Value.Card != 'J' && grouped.Any(x => x.Value.Card == 'J'))
                    {
                        HandType = HandType.TwoPair;
                        return this;
                    }

                    HandType = HandType.OnePair;
                    return this;
                }

                if (grouped.Any(x => x.Value.Card == 'J'))
                {
                    HandType = HandType.OnePair;
                    return this;
                }

                HandType = HandType.HighCard;
                return this;
            }
            
            public Hand(string val, long bid)
            {
                Value = val;
                Bid = bid;

                if (EnableJoker)
                    InitWithJoker();
                else
                    InitNormal();
            }

            public bool IsHigherThan(Hand otherHand)
            {
                if (HandType > otherHand.HandType)
                    return true;

                if (HandType < otherHand.HandType)
                    return false;

                for (int i = 0; i < Value.Length; i++)
                {
                    var kicker = Value[i];
                    var otherKicker = otherHand.Value[i];

                    var order = EnableJoker ? StrengthOrderPartTwo : StrengthOrder;
                    var kickerPos = order.IndexOf(char.ToUpper(kicker));
                    var otherKickerPos = order.IndexOf(char.ToUpper(otherKicker));

                    if (kickerPos < otherKickerPos)
                        return true;

                    if (kickerPos > otherKickerPos)
                        return false;
                }

                return false;
            }

            public bool IsEqualTo(Hand otherHand)
            {
                if (HandType != otherHand.HandType)
                    return false;

                for (int i = 0; i < Value.Length; i++)
                {
                    var kicker = Value[i];
                    var otherKicker = otherHand.Value[i];

                    var order = EnableJoker ? StrengthOrderPartTwo : StrengthOrder;
                    var kickerPos = order.IndexOf(char.ToUpper(kicker));
                    var otherKickerPos = order.IndexOf(char.ToUpper(otherKicker));

                    if (kickerPos != otherKickerPos)
                        return false;
                }

                return true;
            }

            public int CompareTo([AllowNull] Hand other)
            {
                if (this.IsHigherThan(other)) return 1;
                if (this.IsEqualTo(other)) return 0;
                return -1;
            }
        }
    }
}
