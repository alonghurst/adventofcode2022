namespace AdventOfCode2022Cs.Day12;

public class Path<T>
{
    public List<T> CurrentPath { get; }

    public T NextNode => CurrentPath[^1];

    public Path(List<T> pathToHere, T newNode)
    {
        CurrentPath = new List<T>();

        if (pathToHere != null)
        {
            foreach (var t in pathToHere)
            {
                CurrentPath.Add(t);
            }
        }

        CurrentPath.Add(newNode);
    }
}

public class AStar<T>
{
    private readonly IPathManager<T> _pathManager;
    private readonly IndexedPriorityQueue<Path<T>> _ipq = new();
    private readonly List<T> _visited = new();

    public AStar(IPathManager<T> pathManager)
    {
        _pathManager = pathManager;
    }

    public IReadOnlyCollection<T> GetPath(T start, T goal)
    {
        var outboundFromStart = _pathManager.OutboundPaths(start);

        // If the goal node is directly accessible from the start then just return a path between them
        if (outboundFromStart.FirstOrDefault(n => _pathManager.EqualsOtherNode(n, goal)) != null)
        {
            return new[] { start, goal };
        }

        _ipq.Clear();
        _visited.Clear();

        // start a new search
        _ipq.Insert(new Path<T>(null, start), 0);
        _visited.Add(start);

        while (!_ipq.Empty())
        {
            // Get the next path under consideration
            var path = _ipq.Pop();

            // Get next node
            var node = path.NextNode;

            var outboundPaths = _pathManager.OutboundPaths(node);

            // Add each non-visited outbound to IPQ
            foreach (var nextNode in outboundPaths)
            {
                if (_visited.Contains(nextNode))
                {
                    continue;
                }

                _visited.Add(nextNode);

                var newPath = new Path<T>(path.CurrentPath, nextNode);

                if (_pathManager.EqualsOtherNode(nextNode, goal))
                {
                    return newPath.CurrentPath.ToArray();
                }

                // Calculate gCost and hCost
                float gCost = path.CurrentPath.Count + 1;
                float hCost = _pathManager.GetHeuristicCost(nextNode, goal);

                _ipq.Insert(newPath, gCost + hCost);
            }

            _ipq.Sort();
        }

        // Nothing was found so return null
        return null;
    }
}