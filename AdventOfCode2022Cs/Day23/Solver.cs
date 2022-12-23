namespace AdventOfCode2022Cs.Day23;

public static class Solver
{

    public static void Solve()
    {

        Solve("Day23/input.txt", false);

        //Solve2("Day21/test.txt");
    }

    private static void Solve(string filename, bool render)
    {
        var elves = LoadElves(filename);

        var positions = elves.Select(x => x.Position).ToHashSet();

        if (render)
        {
            Render("Loaded", positions);
        }

        var tries = int.MaxValue;
        var cycles = 0;

        while (cycles++ < tries)
        {

            foreach (var elf in elves)
            {
                elf.Consider(positions);
            }

            var proposed = elves
                .Where(x => x.ProposedPosition.HasValue)
                .GroupBy(x => x.ProposedPosition!.Value)
                .ToDictionary(x => x.Key, x => x.Count());

            foreach (var elf in elves)
            {
                elf.TryMove(proposed);
            }

            positions = elves.Select(x => x.Position).ToHashSet();

            if (render)
            {
                Render($"After {cycles}", positions);
            }

            if (proposed.Count == 0)
            {
                Console.WriteLine($"Stopped moving after cycle {cycles}");
                break;
            }
            else if (cycles % 100 == 0)
            {
                Console.WriteLine($"Cycle {cycles}");
            }
        }

        var empty = CountEmpty(positions);
        Console.WriteLine($"There are {empty} positions");
    }

    private static int CountEmpty(HashSet<(int x, int y)> positions)
    {
        var xMin = positions.Min(x => x.x);
        var yMin = positions.Min(x => x.y);
        var xMax = positions.Max(x => x.x);
        var yMax = positions.Max(x => x.y);

        var w = (xMax - xMin) + 1;
        var h = (yMax - yMin) + 1;

        var s = w * h;

        return s - positions.Count;
    }

    private static void Render(string label, HashSet<(int x, int y)> positions)
    {
        Console.WriteLine($"============ {label} ==============");

        var xMin = positions.Min(x => x.x);
        var yMin = positions.Min(x => x.y);
        var xMax = Math.Max(positions.Max(x => x.x), 10);
        var yMax = Math.Max(positions.Max(x => x.y), 10);

        for (int y = yMin; y < yMax; y++)
        {
            for (int x = xMin; x < xMax; x++)
            {
                if (positions.Contains((x, y)))
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }
    }

    private static Elf[] LoadElves(string filename)
    {
        var elves = new List<Elf>();

        var lines = File.ReadAllLines(filename);

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    elves.Add(new Elf(x, y));
                }
            }
        }

        return elves.ToArray();
    }
}