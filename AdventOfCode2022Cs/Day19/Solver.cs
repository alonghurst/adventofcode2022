namespace AdventOfCode2022Cs.Day19;

public enum Resource
{
    Ore = 1,
    Clay = 2,
    Obsidian = 3,
    Geode = 4
}

public record Robot(Resource Collects, Dictionary<Resource, int> Costs)
{
    public override string ToString()
    {
        var c = string.Join(", ", Costs.Select(x => $"{x.Value} {x.Key}"));

        return $"{Collects}: {c}";
    }
}

public class Blueprint
{
    public Dictionary<Resource, int> MostExpensive { get; }

    public Blueprint(int id, Dictionary<Resource, Robot> robots)
    {
        Id = id;
        Robots = robots;

        MostExpensive = robots.SelectMany(x => x.Value.Costs)
            .GroupBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Max(y => y.Value));
    }

    public int Id { get; }

    public Dictionary<Resource, Robot> Robots { get; }
}

public static class Solver
{

    public static void Solve()
    {
        Console.Clear();

        //Solve("Day19/input.txt");
        Solve2("Day19/input.txt");
    }

    private static void Solve(string filename)
    {
        var lines = File.ReadAllLines(filename);

        var blueprints = LoadBlueprints(lines);

        var d = new Dictionary<int, int>();

        foreach (var blueprint in blueprints)
        {
            var q = RunBlueprint(blueprint, true, 24);

            d.Add(blueprint.Id, q.Quality);
        }

        foreach (var kvp in d)
        {
            Console.WriteLine($"Blueprint: {kvp.Key} was {kvp.Value}");
        }

        var ql = d.Sum(x => x.Value);

        Console.WriteLine($"Final quality level was {ql}");
    }

    private static void Solve2(string filename)
    {
        var lines = File.ReadAllLines(filename).Take(3).ToArray();

        var blueprints = LoadBlueprints(lines);

        var d = new Dictionary<int, int>();

        foreach (var blueprint in blueprints)
        {
            var q = RunBlueprint(blueprint, true, 32);

            d.Add(blueprint.Id, q.Resources[Resource.Geode]);
        }

        foreach (var kvp in d)
        {
            Console.WriteLine($"Blueprint: {kvp.Key} was {kvp.Value}");
        }

        var ql = d.Aggregate(0, (i, x) => i == 0 ? x.Value : i * x.Value);

        Console.WriteLine($"Final sum was {ql}");
    }

    private static void Log(string msg, bool verbose)
    {
        if (verbose) Console.WriteLine(msg);
    }

    private static BlueprintRunner RunBlueprint(Blueprint blueprint, bool verbose, int end = 24)
    {
        Log($"Running blueprint {blueprint.Id}", verbose);
        foreach (var blueprintRobot in blueprint.Robots)
        {
            Log(blueprintRobot.ToString(), verbose);
        }

        int minute = 0;
        var runners = new List<BlueprintRunner>()
        {
            new(blueprint)
        };
        var toAdd = new List<BlueprintRunner>();

        var neededObsidian = blueprint.Robots[Resource.Geode].Costs[Resource.Obsidian];
        var clayCost = blueprint.Robots[Resource.Clay].Costs[Resource.Ore];

        var bestQuality = 0;
        var bestObsidianRobots = 0;
        var bestObsidianCollection = 0;

        while (minute++ < end)
        {
            var r1 = 0;
            var r2 = 0;
            var r3 = 0;
            var r4 = 0;
            var r5 = 0;
            toAdd.Clear();

            for (int i = 0; i < runners.Count; i++)
            {
                var runner = runners[i];

                var shouldRemove = false;

                //if (minute > 16 && runner.Robots[Resource.Obsidian] <= 1)
                //{
                //    r1++;
                //    shouldRemove = true;
                //}
                //else if (minute > 13 && runner.Robots[Resource.Obsidian] <= 0)
                //{
                //    r1++;
                //    shouldRemove = true;
                //}
                //else if (minute > 18 && runner.Robots[Resource.Geode] <= 0)
                //{
                //    r1++;
                //    shouldRemove = true;
                //}

                if (minute > 19 && bestObsidianCollection > 0)
                {
                    var obs = runner.Robots[Resource.Obsidian] + runner.Resources[Resource.Geode];

                    if (obs < bestObsidianCollection && (runner.Robots[Resource.Geode] <= 0 && obs < neededObsidian))
                    {
                        r5++;
                        shouldRemove = true;
                    }
                }
                if (minute > clayCost + 1 && runner.Robots[Resource.Clay] <= 0)
                {
                    r4++;
                    shouldRemove = true;
                }
                if (bestQuality > 0 && runner.Quality < bestQuality - 1)
                {
                    r2++;
                    shouldRemove = true;
                }
                if (bestObsidianRobots > 0 && runner.Robots[Resource.Obsidian] < bestObsidianRobots - 1)
                {
                    r3++;
                    shouldRemove = true;
                }

                if (shouldRemove && false)
                {
                    runners.RemoveAt(i);
                    i--;
                }
                else
                {
                    var a = runner.Run();
                    toAdd.AddRange(a);
                }
            }

            runners.AddRange(toAdd);
            Console.WriteLine($"Did {minute}, there are {runners.Count} runners (trimmed {r1 + r2 + r3 + r4 + r5}: {r1}, {r2}, {r3}, {r4}, {r5})");

            bestQuality = runners.Max(x => x.Quality);
            bestObsidianRobots = runners.Max(x => x.Robots[Resource.Obsidian]);
            bestObsidianCollection = runners.Max(x => x.Robots[Resource.Obsidian] + x.Resources[Resource.Geode]);
        }
        var best = runners.MaxBy(x => x.Quality);

        if (verbose)
        {
            foreach (var bestLog in best.Logs)
            {
                Console.WriteLine(bestLog);
            }
        }

        var finalGeodes = best.Resources[Resource.Geode];
        var quality = finalGeodes * blueprint.Id;

        Console.WriteLine($"Blueprint {blueprint.Id} has quality level {quality}");

        return best;
    }

    private static Blueprint[] LoadBlueprints(string[] lines) => lines.Select(LoadBlueprint).ToArray();

    private static Blueprint LoadBlueprint(string line)
    {
        var splits = line.Split(':', StringSplitOptions.RemoveEmptyEntries);

        var id = Convert.ToInt32(splits[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);

        var robots = new List<Robot>();

        foreach (var robotLine in splits[1].Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            var robotParts = robotLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Resource? collects = null;
            var costs = new Dictionary<Resource, int>();
            var lastVal = 0;

            foreach (var robotPart in robotParts)
            {
                if (int.TryParse(robotPart, out int r))
                {
                    lastVal = r;
                }
                else if (Enum.TryParse(typeof(Resource), robotPart, true, out var o) && o is Resource res)
                {
                    if (!collects.HasValue)
                    {
                        collects = res;
                    }
                    else if (lastVal > 0)
                    {
                        costs.Add(res, lastVal);
                        lastVal = 0;
                    }
                    else
                    {
                        Console.WriteLine($"Found {res} but didn't know what to do with it");
                    }
                }
            }

            if (collects.HasValue)
            {
                if (costs.Count == 0)
                {
                    Console.WriteLine($"Didn't parse any costs: {robotLine}");
                }

                robots.Add(new Robot(collects.Value, costs));
            }
            else
            {
                Console.WriteLine($"Didn't parse a robot: {robotLine}");
            }
        }

        return new Blueprint(id, robots.ToDictionary(x => x.Collects, x => x));
    }
}