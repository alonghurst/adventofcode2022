using AdventOfCode2022Cs.Extensions;

namespace AdventOfCode2022Cs.Day13;

public static class Solver
{
    public static void Solve()
    {
        SolvePart1("Day13/test.txt");
        SolvePart1("Day13/input.txt");
        SolvePart2("Day13/test.txt");
        SolvePart2("Day13/input.txt");
    }

    private static void SolvePart1(string path)
    {
        var lines = File.ReadAllLines(path).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        var packets = lines.Select(l => new Packet(l)).ToArray();

        var pairs = packets
            .Batch(2)
            .Select(x => x.ToArray())
            .ToArray();

        var resultingPairs = pairs.Select((x, i) => new
        {
            pair = x,
            index = i + 1,
            isCorrectOrder = IsCorrectOrder(x)
        }).ToArray();

        //foreach (var resultingPair in resultingPairs)
        //{
        //    Console.WriteLine($"{resultingPair.index}: {resultingPair.isCorrectOrder}");
        //    Console.WriteLine(resultingPair.pair[0]);
        //    Console.WriteLine(resultingPair.pair[1]);
        //}

        var s = resultingPairs
            .Where(x => x.isCorrectOrder.HasValue && x.isCorrectOrder.Value)
            .Sum(x => x.index);

        Console.WriteLine($"Sum of indexes is: {s}");
    }

    private static bool? IsCorrectOrder(Packet[] packets)
    {
        var left = packets[0];
        var right = packets[1];

        return IsCorrectOrder(left.Data, right.Data);
    }

    private static bool? IsCorrectOrder(List<object> left, List<object> right)
    {
        for (int i = 0; i < left.Count; i++)
        {
            // If left continues but right has run out
            if (i >= right.Count)
            {
                return false;
            }

            if (left[i] is int li && right[i] is int ri)
            {
                var ic = IsCorrectOrder(li, ri);

                if (ic.HasValue)
                {
                    return ic.Value;
                }
            }
            else if (left[i] is int li2 && right[i] is List<object> rl)
            {
                var rlc = IsCorrectOrder(new List<object>(new object[] { li2 }), rl);

                if (rlc.HasValue)
                {
                    return rlc.Value;
                }
            }
            else if (left[i] is List<object> ll && right[i] is int ri2)
            {
                var llc = IsCorrectOrder(ll, new List<object>(new object[] { ri2 }));

                if (llc.HasValue)
                {
                    return llc.Value;
                }
            }
            else if (left[i] is List<object> ll2 && right[i] is List<object> rl2)
            {
                var blc = IsCorrectOrder(ll2, rl2);

                if (blc.HasValue)
                {
                    return blc.Value;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        if (left.Count < right.Count)
        {
            return true;
        }

        return null;
    }

    private static bool? IsCorrectOrder(int left, int right)
    {
        if (left < right)
        {
            return true;
        }

        if (right < left)
        {
            return false;
        }

        return null;
    }

    private static void SolvePart2(string path)
    {
        var lines = File.ReadAllLines(path).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        var packets = lines
            .Select(l => new Packet(l))
            .Union(new DividerPacket[] { new DividerPacketA(), new DividerPacketB() })
            .OrderBy(_ => _, new PacketComparer())
            .Select((x, i) => new
            {
                packet = x,
                index = i + 1
            })
            .ToArray();

        //foreach (var packet in packets)
        //{
        //    Console.WriteLine($"{packet.index}: {packet.packet} {(packet.packet is DividerPacket ? " Div" : string.Empty)}");
        //}

        var decoders = packets.Where(x => x.packet is DividerPacket).ToArray();

        var decoderKey = decoders[0].index * decoders[1].index;

        Console.WriteLine($"Decoder key: {decoderKey}");
    }

    private class PacketComparer : IComparer<Packet>
    {
        public int Compare(Packet x, Packet y)
        {
            var correct = IsCorrectOrder(x.Data, y.Data);

            if (correct.HasValue)
            {
                return correct.Value ? -1 : 1;
            }

            return 0;
        }
    }
}