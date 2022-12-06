using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AreaOfCode.Days
{
    public class Day6 : DayBase
    {
        public override void Run()
        {
            var text = File.ReadAllText(InputPath);

            var packetQueue = new Queue<char>();
            var messageQueue = new Queue<char>();

            int packetSize = 4;
            int messageSize = 14;

            int? startOfPacket = null;
            int? startOfMessage = null;

            for (int i = 0; i < text.Length; i++)
            {
                if (packetQueue.Count >= packetSize)
                {
                    packetQueue.Dequeue();
                }

                if (messageQueue.Count >= messageSize)
                {
                    messageQueue.Dequeue();
                }

                packetQueue.Enqueue(text[i]);
                messageQueue.Enqueue(text[i]);

                if (packetQueue.Count == packetSize && packetQueue.ToList().Distinct().Count() == packetSize)
                {
                    startOfPacket ??= i + 1;
                }

                if (messageQueue.Count == messageSize && messageQueue.ToList().Distinct().Count() == messageSize)
                {
                    startOfMessage = i + 1;
                    break;
                }
            }

            Console.WriteLine($"Start-of-packet marker found after {startOfPacket} characters processed");
            Console.WriteLine($"Start-of-message marker found after {startOfMessage} characters processed");
        }
    }
}
