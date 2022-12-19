using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2022Cs.Day19
{
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
    public record Blueprint(int Id, Dictionary<Resource, Robot> Robots);

    public static class Solver
    {

        public static void Solve()
        {
            Console.Clear();

            Solve("Day19/test.txt");
        }

        private static void Solve(string filename)
        {
            var lines = File.ReadAllLines(filename);

            var blueprints = LoadBlueprints(lines);

            var ql = 0;

            foreach (var blueprint in blueprints)
            {
                ql += RunBlueprint(blueprint, true);
                break;
            }

            Console.WriteLine($"Final quality level was {ql}");
        }
        private static void Log(string msg, bool verbose)
        {
            if (verbose) Console.WriteLine(msg);
        }

        private static int RunBlueprint(Blueprint blueprint, bool verbose, int minutes = 24)
        {
            var resources = new Dictionary<Resource, int>() { { Resource.Ore, 0 }, { Resource.Clay, 0 }, { Resource.Geode, 0 }, { Resource.Obsidian, 0 } };
            var robots = new Dictionary<Resource, int>() { { Resource.Ore, 1 }, { Resource.Clay, 0 }, { Resource.Geode, 0 }, { Resource.Obsidian, 0 } };


            Log($"Running blueprint {blueprint.Id}", verbose);
            foreach (var blueprintRobot in blueprint.Robots)
            {
                Log(blueprintRobot.ToString(), verbose);
            }

            int minute = 0;

            while (minute++ < minutes)
            {
                Log($"== Minute {minute} ==", verbose);

                var newRobot = SpendResources(blueprint, resources, robots, verbose);

                GatherResources(resources, robots, verbose);

                if (newRobot.HasValue)
                {
                    robots[newRobot.Value]++;

                    Log($"The new {newRobot.Value} is ready, there are now {robots[newRobot.Value]}", verbose);
                }

                Log("", verbose);
            }

            var finalGeodes = resources[Resource.Geode];
            var quality = finalGeodes * blueprint.Id;

            Console.WriteLine($"Blueprint {blueprint.Id} has quality level {quality}");

            return quality;
        }

        private static void GatherResources(Dictionary<Resource, int> resources, Dictionary<Resource, int> robots, bool verbose)
        {
            foreach (var robot in robots)
            {
                if (robot.Value > 0)
                {
                    resources[robot.Key] += robot.Value;

                    Log($"{robot.Value} {robot.Key} resource collectors collect {robot.Value} {robot.Key}, there is now {resources[robot.Key]} {robot.Key}", verbose);
                }
            }
        }

        private static Resource[] OrderOfPreference = new Resource[]
        {
            Resource.Geode,
            Resource.Obsidian,
            Resource.Clay,
            Resource.Ore
        };

        private static Resource? SpendResources(Blueprint blueprint, Dictionary<Resource, int> resources, Dictionary<Resource, int> robots, bool verbose)
        {
            foreach (var resource in OrderOfPreference)
            {
                if (robots[resource] >= 4)
                {
                    continue;
                }

                var costs = blueprint.Robots[resource].Costs;
                var canAfford = true;

                foreach (var cost in costs)
                {
                    if (resources[cost.Key] < cost.Value)
                    {
                        canAfford = false;
                        break;
                    }
                }

                if (canAfford)
                {
                    foreach (var cost in costs)
                    {
                        resources[cost.Key] -= cost.Value;
                    }

                    var c = string.Join(", ", costs.Select(x => $"{x.Value} {x.Key}"));

                    Log($"Spend {c} to make a {resource}", verbose);

                    return resource;
                }
            }

            return null;
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
}
