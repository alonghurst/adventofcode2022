using System.Collections;
using System.Net.NetworkInformation;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2022Cs.Day20
{

    public static class Solver
    {

        public static void Solve()
        {

            //Solve("Day20/input.txt");

            Solve2("Day20/test.txt");
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
            while (i++ < 10)
            {
                newNumbers = MoveNumbers(originalNumbers, newNumbers, false);
                Console.WriteLine("Did " + i);
            }

            FindCoords(newNumbers, 1000, 2000, 3000);
        }

        private static void FindCoords(List<Number> newNumbers, int x, int y, int z)
        {
            var idx = newNumbers.FindIndex(x => x.Value == 0);

            long Do(int v)
            {
                var i = idx + v;
                while (i >= newNumbers.Count)
                {
                    i -= newNumbers.Count;
                }
                var n = newNumbers[i];
                Console.WriteLine($"V: {n.Value}");

                return n.Value;
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
            }

            public long Value { get; }

            public override string ToString() => Value.ToString();
        }
    }
}
