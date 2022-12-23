using System.Diagnostics;

namespace AdventOfCode2022Cs.Day20;

public static class Solver
{

    public static void Solve()
    {

        //Solve("Day20/input.txt");

        Solve2("Day20/input.txt");
    }

    private static void Solve(string filename)
    {
        var lines = File.ReadAllLines(filename);

        var originalNumbers = LoadNumbers(lines);

        var newNumbers = MoveNumbers(originalNumbers, originalNumbers.ToList(), false);

        FindCoords(newNumbers, 1000, 2000, 3000);
    }

    private static void Solve2(string filename)
    {
        var lines = File.ReadAllLines(filename);

        var originalNumbers = LoadNumbers(lines, 811589153);

        List<Number> newNumbers = originalNumbers.ToList();

        var i = 0;
        var tries = 10;
        while (i++ < tries)
        {
            var sw = new Stopwatch();
            sw.Start();

            newNumbers = MoveNumbers(originalNumbers, newNumbers, false);
            Console.WriteLine("Did " + i);

            sw.Stop();
            Console.WriteLine($"Took {sw.Elapsed.TotalSeconds} sec");

            var rem = TimeSpan.FromSeconds(sw.Elapsed.TotalSeconds * (tries - i));

            Console.WriteLine($"Estimated remaining {rem.TotalMinutes} min");

        }

        FindCoords(newNumbers, 1000, 2000, 3000);
    }

    private static void FindCoords(List<Number> newNumbers, int x, int y, int z)
    {
        var idx = newNumbers.FindIndex(x => x.StartingValue == 0);

        long Do(int v)
        {
            var i = idx + v;
            while (i >= newNumbers.Count)
            {
                i -= newNumbers.Count;
            }
            var n = newNumbers[i];
            Console.WriteLine($"V: {n.StartingValue}");

            return n.StartingValue;
        }

        var total = Do(x) + Do(y) + Do(z);

        Console.WriteLine($"Total: {total}");
    }

    private static List<Number> MoveNumbers(Number[] originalNumbers, List<Number> newNumbers, bool verbose)
    {
        void Log()
        {
            if (!verbose)
            {
                return;
            }

            var s = string.Join(", ", newNumbers);

            Console.WriteLine(s);
        }

        Log();

        foreach (var originalNumber in originalNumbers)
        {
            if (originalNumber.Value == 0)
            {
                continue;
            }

            var index = newNumbers.IndexOf(originalNumber);
            var newPos = index + originalNumber.Value;

            newNumbers.RemoveAt(index);

            if (originalNumber.Value > 0)
            {
                while (newPos >= newNumbers.Count)
                {
                    newPos -= newNumbers.Count;
                }
            }
            else
            {
                while (newPos < 0)
                {
                    newPos += newNumbers.Count;
                }
            }

            newNumbers.Insert((int)newPos, originalNumber);

            Log();
        }

        return newNumbers;
    }

    private static Number[] LoadNumbers(string[] lines, long decryptionKey = 1)
    {
        var numbers = lines.Where(x => !string.IsNullOrWhiteSpace(x))
            .Select((x, i) => new Number(Convert.ToInt64(x) * decryptionKey))
            .ToArray();

        return numbers;
    }

    public class Number
    {
        public Number(long value)
        {
            Value = value;
            StartingValue = value;
        }

        public long StartingValue { get; }

        public long Value { get; set; }

        public override string ToString() => Value.ToString();
    }

    static long SolvePartTwo(IEnumerable<Number> initialList)
    {
        List<Node> nodes = new();
        foreach (var n in initialList)
        {
            nodes.Add(new Node(n.Value * 811_589_153));
        }

        foreach (var (l, r) in nodes.Zip(nodes.Skip(1)))
        {
            l.Next = r;
            r.Prev = l;
        }

        nodes[0].Prev = nodes[^1];
        nodes[^1].Next = nodes[0];

        for (int i = 0; i < 10; i++)
        {
            foreach (var n in nodes)
            {
                //Remove our node from the loop
                n.Prev.Next = n.Next;
                n.Next.Prev = n.Prev;

                //Allows us to walk along the nodes.
                Node l = n.Prev, r = n.Next;

                foreach (var _ in Enumerable.Range(0, (int)(Math.Abs(n.Val) % (nodes.Count - 1))))
                {
                    if (n.Val < 0)
                    {
                        l = l.Prev;
                        r = r.Prev;
                    }
                    else
                    {
                        l = l.Next;
                        r = r.Next;
                    }
                }
                l.Next = n;
                n.Prev = l;
                r.Prev = n;
                n.Next = r;

            }
        }

        long res = 0;

        Node start = nodes.First(a => a.Val == 0);

        foreach (var _ in Enumerable.Range(0, 3))
        {
            foreach (var _2 in Enumerable.Range(0, 1000))
            {
                start = start.Next;
            }
            res += start.Val;
        }

        return res;
    }

    class Node
    {
        public long Val { get; set; }
        public Node Next { get; set; }
        public Node Prev { get; set; }

        public Node(int Value)
        {
            this.Val = Value;
        }

        public Node(long Value)
        {
            this.Val = Value;
        }

        public override string ToString()
        {
            return $"V:{Val}, L:{Prev.Val}, R:{Next.Val}";
        }
    }
}