using System.Collections;
using System.Diagnostics;
using AdventOfCode2022Cs.Day12;

namespace AdventOfCode2022Cs.Day16;

public class ValveGraph : IPathManager<Valve>, IEnumerable<Valve>
{
    public ValveGraph(Dictionary<string, Valve> valves)
    {
        Valves = valves;
        AStar = new AStar<Valve>(this);
        ShortestPathsTo = new Dictionary<string, Dictionary<string, IReadOnlyCollection<Valve>>>();

        foreach (var a in Valves.Values)
        {
            var paths = new Dictionary<string, IReadOnlyCollection<Valve>>();

            foreach (var b in Valves.Values)
            {
                if (a.Id == b.Id)
                {
                    continue;
                }

                var path = AStar.GetPath(a, b);

                if (path.First().Id == a.Id)
                {
                    path = path.Skip(1).ToArray();
                }

                paths.Add(b.Id, path);
            }

            ShortestPathsTo.Add(a.Id, paths);
        }
    }

    public Valve this[string id] => Valves[id];

    public Dictionary<string, Dictionary<string, IReadOnlyCollection<Valve>> >ShortestPathsTo { get; }

    public AStar<Valve> AStar { get; } 

    public Dictionary<string, Valve> Valves { get; }

    public IEnumerable<Valve> OutboundPaths(Valve node)
    {
        var valvesHere = Valves.Values
            .Where(x => node.LeadsTo.Contains(x.Id))
            .ToArray();

        return valvesHere;
    }

    public float GetHeuristicCost(Valve node, Valve goal)
    {
        if (node.Id == goal.Id)
        {
            return 1;
        }

        if (node.LeadsTo.Contains(goal.Id))
        {
            return 2;
        }

        return 3;
    }

    public bool EqualsOtherNode(Valve a, Valve b)
    {
        return a.Id == b.Id;
    }

    public IEnumerator<Valve> GetEnumerator() => Valves.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[DebuggerDisplay("{Id} {FlowRate} {IsOpen")]
public class Valve
{
    public string Id { get; }
    public int FlowRate { get; }

    public IReadOnlyCollection<string> LeadsTo { get; }
        

    public Valve(string line)
    {
        var splits = line.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);

        // 0     1  2   3   4           5      6  7   8     9+
        // Valve AA has flow rate=0; tunnels lead to valves DD, II, BB

        Id = splits[1];
        FlowRate = Convert.ToInt32(splits[4].Split('=')[1].Replace(";", string.Empty));
        LeadsTo = splits.Skip(9).Select(s => s.Replace(",", String.Empty)).ToArray();
    }

    //public int CalculateAdditionalFlow(int minute, int end)
    //{
    //    if (IsOpen)
    //    {
    //        return 0;
    //    }

    //    // Costs a minute to open it
    //    minute++;

    //    var remainder = end - minute;

    //    if (remainder > 0)
    //    {
    //        return remainder * FlowRate;
    //    }

    //    return 0;
    //}

    //public void Open()
    //{
    //    IsOpen = true;
    //}

    public override string ToString()
    {
        string v = string.Join(", ", LeadsTo);

        return $"Valve {Id} has flow rate={FlowRate}; tunnels lead to valves {v}";
    }
}