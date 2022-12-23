namespace AdventOfCode2022Cs.Day23;

public enum Direction
{
    West = 1,
    East = 2,
    North = 3,
    South = 4,
    NorthWest = 5,
    NorthEast = 6,
    SouthEast = 7,
    SouthWest = 8
}

public static class CoordinateHelper
{
    public static (int x, int y) GetDirectionVector(int xFrom, int yFrom, int xTo, int yTo)
    {
        return (xTo - xFrom, yTo - yFrom);
    }

    public static Direction GetDirection(int xFrom, int yFrom, int xTo, int yTo)
    {
        var (x, y) = GetDirectionVector(xFrom, yFrom, xTo, yTo);

        if (x > 0)
        {
            if (y != 0)
            {
                return y > 0 ? Direction.SouthWest : Direction.NorthEast;
            }

            return Direction.East;
        }
        else if (x < 0)
        {
            if (y != 0)
            {
                return y > 0 ? Direction.SouthEast : Direction.NorthWest;
            }


            return Direction.West;
        }
        else if (y > 0)
        {
            return Direction.South;
        }
        else if (y < 0)
        {
            return Direction.North;
        }

        throw new ArgumentOutOfRangeException();
    }

    public static (int x, int y) GetMovement(Direction direction)
    {
        switch (direction)
        {
            case Direction.West:
                return (-1, 0);
            case Direction.East:
                return (1, 0);
            case Direction.North:
                return (0, -1);
            case Direction.South:
                return (0, 1);
            case Direction.SouthEast:
                return (1, 1);
            case Direction.SouthWest:
                return (-1, 1);
            case Direction.NorthWest:
                return (-1, -1);
            case Direction.NorthEast:
                return (1, -1);
        }

        throw new ArgumentOutOfRangeException(nameof(direction));
    }
}