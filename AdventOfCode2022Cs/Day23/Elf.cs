namespace AdventOfCode2022Cs.Day23;

public class Elf
{
    public static bool Debug { get; set; }

    public (int x, int y) Position { get; private set; }

    public (int x, int y)? ProposedPosition { get; private set; }

    private Queue<DirectionConsideration> _directionsToConsider = new Queue<DirectionConsideration>(
        new DirectionConsideration[]
        {
        new(Direction.North, Direction.North, Direction.NorthEast, Direction.NorthWest),
        new(Direction.South, Direction.South, Direction.SouthEast, Direction.SouthWest),
        new(Direction.West, Direction.West, Direction.NorthWest, Direction.SouthWest),
        new(Direction.East, Direction.East, Direction.NorthEast, Direction.SouthEast),
    });

    public Elf(int x, int y)
    {
        Position = (x, y);
    }

    public void Consider(HashSet<(int, int)> elves)
    {
        ProposedPosition = null;

        if (CheckForElf(elves, Direction.North, Direction.NorthWest, Direction.NorthEast, Direction.East, Direction.South, Direction.SouthEast, Direction.SouthWest, Direction.West))
        {
            for (int i = 0; i < _directionsToConsider.Count; i++)
            {
                var d = _directionsToConsider.ElementAt(i);

                ProposedPosition = ExamineConsideration(elves, d);

                if (ProposedPosition.HasValue)
                {
                    Log($"{Position} will try {ProposedPosition} ({d.Direction})");

                    break;
                }
            }

            if (!ProposedPosition.HasValue)
            {
                Log($"{Position} no valid movements");
            }
        }
        else
        {
            Log($"{Position} happy where I am");
        }
    }

    private void Log(string p0)
    {
        if (Debug)
        {
            Console.WriteLine(p0);
        }
    }

    public void TryMove(Dictionary<(int, int), int> proposedPositions)
    {
        if (ProposedPosition.HasValue && proposedPositions[ProposedPosition.Value] == 1)
        {
            Log($"{Position} moved to {ProposedPosition}");
            Position = ProposedPosition.Value;
        }
        else
        {
            Log($"{Position} someone else wanted {ProposedPosition}");
        }

        var d = _directionsToConsider.Dequeue();
        _directionsToConsider.Enqueue(d);
    }

    private (int x, int y)? ExamineConsideration(HashSet<(int, int)> elves, DirectionConsideration directionConsideration)
    {
        if (CheckForElf(elves, directionConsideration.Tests))
        {
            return null;
        }

        var (dx, dy) = CoordinateHelper.GetMovement(directionConsideration.Direction);

        return (dx + Position.x, dy + Position.y);
    }

    private bool CheckForElf(HashSet<(int, int)> elves, params Direction[] tests)
    {
        foreach (var test in tests)
        {
            var d = CoordinateHelper.GetMovement(test);

            var c = (d.x + Position.x, d.y + Position.y);

            if (elves.Contains(c))
            {
                return true;
            }
        }

        return false;
    }
}

public record DirectionConsideration(Direction Direction, params Direction[] Tests);