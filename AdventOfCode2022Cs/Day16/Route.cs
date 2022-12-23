using System.Diagnostics;

namespace AdventOfCode2022Cs.Day16;

public enum ActionType
{
    Move,
    Open,
    Wait
}

public class Action
{
    public Action(ActionType type, string targetValve)
    {
        Type = type;
        TargetValve = targetValve;
    }

    public ActionType Type { get; }
    public string TargetValve { get; }

    public override string ToString() => $"{Type}:{TargetValve}";
}

public class Actor
{
    public Actor(string position)
    {
        Position = position;
    }

    public Actor(Actor actor, params Action[] nextActions)
    {
        Position = actor.Position;

        Actions = new List<Action>(actor.Actions);
        ActionsToProcess = new Queue<Action>(nextActions);
    }

    public string Position { get; set; }
    public List<Action> Actions { get; private set; } = new();
    public Queue<Action> ActionsToProcess { get; } = new();
}

[DebuggerDisplay("{Id} {TotalPressure}")]
public class Route
{
    private static int _nextId;

    public bool IsFinished => Minute >= End || IsStuck;

    public bool IsStuck { get; private set; }

    public int Minute { get; private set; }

    public int End { get; }

    public int TotalPressure { get; private set; }

    public ValveGraph Valves { get; }

    public HashSet<string> OpenValves { get; }

    public Actor Self { get; }
    public Actor? Elephant { get; }


    public Route(int end, ValveGraph valves, string position, bool allowElephant = false)
    {
        Minute = 1;
        End = end;
        Valves = valves;

        Self = new Actor(position);
        if (allowElephant)
        {
            Elephant = new Actor(position);
        }


        OpenValves = new HashSet<string>();
        Id = _nextId++;
    }

    public int Id { get; }

    public Route(Route route, Actor self, Actor? elephant)
    {
        TotalPressure = route.TotalPressure;

        Minute = route.Minute;
        End = route.End;
        Valves = route.Valves;

        Self = self;
        Elephant = elephant;

        OpenValves = new HashSet<string>(route.OpenValves);
        Id = _nextId++;
    }


    private readonly List<Route> _additionalRoutes = new();
    public IReadOnlyCollection<Route> Step()
    {
        _additionalRoutes.Clear();


        _additionalRoutes.AddRange(UpdateActions(Self, false, Elephant));
        _additionalRoutes.AddRange(UpdateActions(Elephant, true, Self));

        ProcessNextAction(Self);
        ProcessNextAction(Elephant);

        ReleasePressure();
        Minute++;
            
        return _additionalRoutes.ToArray();
    }

    private IReadOnlyCollection<Route> UpdateActions(Actor? actor, bool isElephant, Actor? other)
    {
        if (actor == null || actor.ActionsToProcess.Any())
        {
            return Array.Empty<Route>();
        }
            
        var newActions = FindActions(actor.Position, other).ToList();

        if (other != null)
        {
            var otherTarget = other.ActionsToProcess.LastOrDefault();

            if (otherTarget != null)
            {
                for (int i = 0; i < newActions.Count; i++)
                {
                    var target = newActions[i].Last();

                    if (target.TargetValve == otherTarget.TargetValve && target.Type == otherTarget.Type)
                    {
                        newActions.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        if (newActions.Count == 0)
        {
            IsStuck = true;
        }
        else
        {
            var newRoutes = newActions.Skip(1)
                .Select(x => new Route(
                    this,
                    new Actor(this.Self, isElephant ? Array.Empty<Action>() : x),
                    this.Elephant != null ? new Actor(this.Elephant, isElephant ? x : Array.Empty<Action>()) : null
                ))
                .ToArray();

            foreach (var action in newActions[0])
            {
                actor.ActionsToProcess.Enqueue(action);
            }

            return newRoutes;
        }

        if (IsStuck)
        {
            actor.ActionsToProcess.Enqueue(new Action(ActionType.Wait, actor.Position));
            return Array.Empty<Route>();
        }

        return Array.Empty<Route>();
    }

    private void ProcessNextAction(Actor? actor)
    {
        if (actor == null)
        {
            return;
        }

        var lastAction = actor.ActionsToProcess.Dequeue();
        actor.Actions.Add(lastAction);

        if (lastAction.Type == ActionType.Move)
        {
            actor.Position = lastAction.TargetValve;
        }
        else if (lastAction.Type == ActionType.Open)
        {
            OpenValves.Add(lastAction.TargetValve);
        }
    }

    private void ReleasePressure()
    {
        foreach (var openValve in OpenValves)
        {
            var v = Valves[openValve];
            TotalPressure += v.FlowRate;
        }
    }

    private IEnumerable<Action[]> FindActions(string position, Actor? other)
    {
        var currentValve = Valves[position];

        if (!OpenValves.Contains(currentValve.Id) && currentValve.FlowRate > 0)
        {
            yield return new Action[] { new(ActionType.Open, currentValve.Id) };
        }

        var valvesHere = Valves
            .Where(x => x.Id != position && x.FlowRate > 0 && !OpenValves.Contains(x.Id))
            .ToArray();

        var open = string.Join(", ", OpenValves);
        var consider = string.Join(", ", valvesHere.Select(x => x.Id));

        //Console.WriteLine($"{Id} on m{Minute} has {valvesHere.Length} to consider: o={open}, c={consider}");

        foreach (var valve in valvesHere)
        {
            var sp = Valves.ShortestPathsTo[position][valve.Id];

            if (sp.Count > End - Minute)
            {
                //Console.WriteLine("Skipping - too far");
                continue;
            }

            var moves = sp.Select(x => new Action(ActionType.Move, x.Id)).ToList();

            // Always open the valve when we get there
            moves.Add(new Action(ActionType.Open, valve.Id));

            yield return moves.ToArray();
        }
    }
}