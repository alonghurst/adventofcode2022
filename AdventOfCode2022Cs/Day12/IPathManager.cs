namespace AdventOfCode2022Cs.Day12;

public interface IPathManager<T>
{
    // Get the list of outbound paths from this node
    IEnumerable<T> OutboundPaths(T node);

    // Get the heuristic cost to the goal
    float GetHeuristicCost(T node, T goal);

    // Check whether this node is the same as another node
    bool EqualsOtherNode(T a, T b);
}