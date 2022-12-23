namespace AdventOfCode2022Cs.Day19;

public class BlueprintRunner
{
    public bool IsFinished => Minute < 24;

    public int Minute { get; private set; }

    public int Quality => Blueprint.Id * Resources[Resource.Geode];

    public Dictionary<Resource, int> Robots { get; }
    public Dictionary<Resource, int> Resources { get; }
    public Blueprint Blueprint { get; }

    public Resource? NextAction { get; private set; }

    public List<string> Logs { get; }

    public BlueprintRunner(Blueprint blueprint)
    {
        Blueprint = blueprint;
        Resources = new Dictionary<Resource, int>() { { Resource.Ore, 0 }, { Resource.Clay, 0 }, { Resource.Geode, 0 }, { Resource.Obsidian, 0 } };
        Robots = new Dictionary<Resource, int>() { { Resource.Ore, 1 }, { Resource.Clay, 0 }, { Resource.Geode, 0 }, { Resource.Obsidian, 0 } };

        Logs = new List<string>();
    }

    private BlueprintRunner(BlueprintRunner parent, Resource? nextAction)
    {
        Blueprint = parent.Blueprint;
        Minute = parent.Minute;

        Robots = new Dictionary<Resource, int>(parent.Robots);
        Resources = new Dictionary<Resource, int>(parent.Resources);

        NextAction = nextAction;

        Logs = new List<string>(parent.Logs);
    }


    public BlueprintRunner[] Run()
    {
        Minute++;

        Log($"== Minute {Minute} ==");

        var newRobot = ProcessAction();

        GatherResources();

        if (newRobot.HasValue)
        {
            Robots[newRobot.Value]++;

            Log($"The new {newRobot.Value} is ready, there are now {Robots[newRobot.Value]}");
        }
        Log(string.Empty);

        var possibleActions = FindPossibleActions().ToArray();
            
        NextAction = null;

        if (possibleActions.Length > 0)
        {
            NextAction = possibleActions[0];

            possibleActions = possibleActions.Skip(1).ToArray();
        }
            
        var runners = possibleActions.Select(x => new BlueprintRunner(this, x)).ToArray();

        return runners;
    }

    private void GatherResources()
    {
        foreach (var robot in Robots)
        {
            if (robot.Value > 0)
            {
                Resources[robot.Key] += robot.Value;

                Log($"{robot.Value} {robot.Key} resource collectors collect {robot.Value} {robot.Key}, there is now {Resources[robot.Key]} {robot.Key}");
            }
        }
    }


    private Resource? ProcessAction()
    {
        if (NextAction.HasValue)
        {
            if (CanAfford(Blueprint.Robots[NextAction.Value]))
            {
                var costs = Blueprint.Robots[NextAction.Value].Costs;

                foreach (var cost in costs)
                {
                    Resources[cost.Key] -= cost.Value;
                }

                var c = string.Join(", ", costs.Select(x => $"{x.Value} {x.Key}"));

                Log($"Spend {c} to make a {NextAction.Value}");
            }
            else
            {
                Log($"I was told to make a {NextAction} but I can't afford it");
                return null;
            }
        }

        return NextAction;
    }

    private void Log(string s)
    {
        Logs.Add(s);
    }

    private IEnumerable<Resource?> FindPossibleActions()
    {
        var canAffordAll = true;

        if (CanAfford(Blueprint.Robots[Resource.Geode]))
        {
            yield return Resource.Geode;
            yield break;
        }

        if (CanAfford(Blueprint.Robots[Resource.Obsidian]))
        {
            yield return Resource.Obsidian;
            yield break;
        }

        foreach (var robot in Blueprint.Robots)
        {
            if (robot.Key == Resource.Obsidian || robot.Key == Resource.Geode)
            {
                continue;
            }

            if (robot.Key != Resource.Geode)
            {
                var current = Robots[robot.Key];
                if (current >= Blueprint.MostExpensive[robot.Key])
                {
                    continue;
                }
            }

            var canAfford = CanAfford(robot.Value);

            if (canAfford)
            {
                yield return robot.Key;
            }
            else
            {
                canAffordAll = false;
            }
        }

        if (!canAffordAll)
        {
            yield return null;
        }
    }

    private bool CanAfford(Robot robot)
    {
        var costs = robot.Costs;
        var canAfford = true;

        foreach (var cost in costs)
        {
            if (Resources[cost.Key] < cost.Value)
            {
                canAfford = false;
                break;
            }
        }

        return canAfford;
    }
}