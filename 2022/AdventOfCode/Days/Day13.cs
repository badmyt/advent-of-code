using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventOfCode.Days
{
    public class Day13 : DayBase
    {
        public override void Run()
        {
            var packetPairs = File.ReadAllLines(InputPath)
                .Where(x => x.Length > 1)
                .Select(JsonConvert.DeserializeObject)
                .Select(Packet.FromObject)
                .ToArray()
                .SplitAsArrays(2)
                .Select(x => new PacketPair(x[0], x[1]))
                .ToArray();

            var correctlyOrderedIndicesSum = 0;
            for (int i = 0; i < packetPairs.Length; i++)
            {
                if (packetPairs[i].WasInCorrectOrder)
                    correctlyOrderedIndicesSum += i + 1;
            }

            Console.WriteLine($"Sum of indices of pairs which were in correct order - {correctlyOrderedIndicesSum}");

            var allPackets = packetPairs.SelectMany(x => new[] { x.Packet1, x.Packet2 }).ToList();

            var divider1 = Packet.FromObject(JsonConvert.DeserializeObject("[[2]]"));
            var divider2 = Packet.FromObject(JsonConvert.DeserializeObject("[[6]]"));

            allPackets.Add(divider1);
            allPackets.Add(divider2);

            var orderedPackets = allPackets.OrderBy(x => x).ToList();
            var index1 = orderedPackets.IndexOf(divider1) + 1;
            var index2 = orderedPackets.IndexOf(divider2) + 1;
            var decoderKey = index1 * index2;

            Console.WriteLine($"Decoder key for the distress signal - {decoderKey}");
        }

        private class PacketPair
        {
            public bool WasInCorrectOrder { get; set; }
            public Packet Packet1 { get; set; }
            public Packet Packet2 { get; set; }
            public PacketPair(Packet packet1, Packet packet2)
            {
                if (packet1 < packet2)
                {
                    Packet1 = packet1;
                    Packet2 = packet2;
                    WasInCorrectOrder = true;
                }
                else
                {
                    Packet1 = packet2;
                    Packet2 = packet1;
                    WasInCorrectOrder = false;
                }
            }
        }

        private class Packet : IComparable, IComparable<Packet>
        {
            public bool IsArray;
            public int? Value;

            public List<Packet> Items { get; set; } = new List<Packet>();

            public static bool operator <(Packet a, Packet b)
            {
                if (a == b)
                    return false;

                if (!a.IsArray && !b.IsArray)
                    return a.Value < b.Value;

                if (a.IsArray && b.IsArray)
                {
                    var minLength = Math.Min(a.Items.Count, b.Items.Count);
                    for (int i = 0; i < minLength; i++)
                    {
                        if (a.Items[i] == b.Items[i])
                            continue;

                        if (a.Items[i] < b.Items[i])
                            return true;

                        return false;
                    }

                    if (a.Items.Count < b.Items.Count)
                        return true;

                    return false;
                }

                if (!a.IsArray)
                {
                    var newA = new Packet { IsArray = true, Items = new List<Packet> { new Packet { Value = a.Value } } };
                    return newA < b;
                }

                if (!b.IsArray)
                {
                    var newB = new Packet { IsArray = true, Items = new List<Packet> { new Packet { Value = b.Value } } };
                    return a < newB;
                }

                return false;
            }

            public static bool operator >(Packet a, Packet b) => !(a == b) && !(a < b);
            public static bool operator ==(Packet a, Packet b) => a?.Equals(b) == true;
            public static bool operator !=(Packet a, Packet b) => a?.Equals(b) == false;
            public override bool Equals(object obj)
            {
                var packet = obj as Packet;
                if (packet != null && this.IsArray != packet.IsArray)
                {
                    if (this.IsArray && Items.Count == 1)
                        return Items[0].Equals(packet);

                    if (packet.IsArray && packet.Items.Count == 1)
                        return this.Equals(packet.Items[0]);
                }

                return $"{this}" == $"{obj}";
            }

            public override int GetHashCode() => $"{this}".GetHashCode();

            public static Packet FromObject(object packetObject)
            {
                var packet = new Packet();

                if (packetObject == null)
                    return null;

                if (packetObject.GetType() == typeof(JArray))
                {
                    packet.IsArray = true;
                    foreach (var item in packetObject as JArray)
                    {
                        packet.Items.Add(FromObject(item));
                    }
                }
                else
                {
                    packet.Value = (packetObject as JValue)?.ToObject<int>();
                }

                return packet;
            }

            public override string ToString() => Value.HasValue ? $"{Value}" : $"[{string.Join(",", Items)}]";

            public int CompareTo(object obj) => CompareTo(obj as Packet);

            public int CompareTo([AllowNull] Packet other) 
            {
                if (this < other)
                    return -1;

                if (this > other)
                    return 1;

                return 0;
            }
        }
    }
}
