namespace AdventOfCode2022Cs.Day16;

public static class Solver
{
    public static void Solve()
    {
        //SolvePart1("Day16/input.txt");
        SolvePart2("Day16/test.txt"); 
    }

    private static void SolvePart1(string filename)
    {
        var graph = CreateGraph(filename);

        var end = 30;

        var start = new Route(end, graph, "AA");

        ProcessAndFindBest(start, graph);
    }

    private static void SolvePart2(string filename)
    {
        var graph = CreateGraph(filename);

        var end = 26;

        var start = new Route(end, graph, "AA", true);

        ProcessAndFindBest(start, graph);
    }

    private static void ProcessAndFindBest(Route start, ValveGraph graph)
    {
        var routes = ProcessRoutes(start);

        var best = routes.OrderByDescending(x => x.TotalPressure).First();

        RenderRoute(best, graph);
    }

    private static void RenderRoute(Route best, ValveGraph graph)
    {
        int minute = 0;
        var openValves = new HashSet<string>();
        var totalPressure = 0;

        while (minute < best.End)
        {

            Console.WriteLine($"== Minute {minute + 1} ==");

            if (openValves.Any())
            {
                var extraFlow = 0;
                foreach (var openValve in openValves)
                {
                    var va = graph[openValve];
                    extraFlow += va.FlowRate;
                }

                totalPressure += extraFlow;
                var v = string.Join(", ", openValves.Select(x => x));
                Console.WriteLine($"Valves {v} are open, releasing {extraFlow} pressure ({totalPressure} total)");
            }
            else
            {
                Console.WriteLine("No valves are open");
            }

            DoAction(best.Self, minute, openValves, false);
            DoAction(best.Elephant, minute, openValves, true);

            minute++;
        }

        Console.WriteLine("=====================");
        Console.WriteLine($"Total pressure: {best.TotalPressure}");
    }

    private static void DoAction(Actor? actor, int minute, HashSet<string> openValves, bool isElephant)
    {
        if (actor == null)
        {
            return;
        }

        var action = actor.Actions.Count > minute ? actor.Actions[minute] : null;

        if (action != null)
        {
            if (action.Type == ActionType.Move)
            {
                Console.WriteLine($"{(isElephant ? "Elephant " : "")}Moving to {action.TargetValve}");
            }
            else if (action.Type == ActionType.Open)
            {
                Console.WriteLine($"{(isElephant ? "Elephant " : "")}Opening {action.TargetValve}");
                openValves.Add(action.TargetValve);
            }
        }
    }

    private static List<Route> ProcessRoutes(Route start)
    {
        var routes = new List<Route>() { start };

        while (routes.Any(x => !x.IsFinished))
        {
            Console.WriteLine($"Minute: {routes.First().Minute}");

            var extra = new List<Route>();

            foreach (var route in routes)
            {
                if (route.IsFinished)
                {
                    continue;
                }

                var a = route.Step();

                extra.AddRange(a);
            }

            routes.AddRange(extra);
        }

        return routes;
    }

    private static ValveGraph CreateGraph(string filename)
    {
        var lines = File.ReadAllLines(filename).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        var valves = lines.Select(s => new Valve(s)).ToDictionary(v => v.Id, v => v);

        foreach (var valvesValue in valves.Values)
        {
            Console.WriteLine(valvesValue.ToString());
        }


        Console.WriteLine("Calculating shortest paths");
        var graph = new ValveGraph(valves);
        return graph;
    }
}