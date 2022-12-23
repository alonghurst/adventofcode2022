namespace AdventOfCode2022Cs.Day12;

public class Cell
{
    public Cell(int x, int y, int height, char source)
    {
        X = x;
        Y = y;
        Height = height;
        Source = source;
    }

    public int X { get; }
    public int Y { get; }
    public int Height { get; }

    public char Source { get; }

    public override string ToString() => $"{X}/{Y} {Height}";
}

public class Grid : IPathManager<Cell>
{
    public Grid(string[] lines)
    {
        Height = lines.Length;

        Cells = new Cell[Height][];

        for (int y = 0; y < Height; y++)
        {
            var line = lines[y];

            Width = line.Length;

            Cells[y] = new Cell[Width];

            for (int x = 0; x < line.Length; x++)
            {
                var c = line[x];

                var h = (int)c;

                if (c == 'S')
                {
                    h = (int)'a';
                }
                else if (c == 'E')
                {
                    h = (int)'z';
                }

                var cell = new Cell(x, y, h, c);

                Cells[y][x] = cell;

                if (cell.Source == 'S')
                {
                    Start = cell;
                }
                else if (cell.Source == 'E')
                {
                    End = cell;
                }
            }
        }
    }

    public Cell Start { get; }

    public Cell End { get; }

    public Cell[][] Cells { get; }
    public int Height { get; }
    public int Width { get; }

    public bool IsWithinBounds(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return false;
        }

        return true;
    }

    public IEnumerable<Cell> OutboundPaths(Cell node)
    {
        IEnumerable<Cell> TryCells(params (int x, int y)[] coords)
        {
            foreach (var coord in coords)
            {
                if (IsWithinBounds(coord.x, coord.y))
                {
                    var cell = Cells[coord.y][coord.x];

                    if (cell.Height <= node.Height + 1)
                    {
                        yield return cell;
                    }
                }
            }
        }

        return TryCells(
            (node.X + 1, node.Y),
            (node.X - 1, node.Y),
            (node.X, node.Y + 1),
            (node.X, node.Y - 1)
        ).ToArray();
    }

    public float GetHeuristicCost(Cell node, Cell goal)
    {
        int dX = goal.X - node.X;
        int dY = goal.Y - node.Y;
         
        return (float)Math.Sqrt(dX * dX + dY * dY);
    }

    private static float ManhattanDistance(Cell node, Cell goal)
    {
        // use manhattan distance
        var x = Math.Abs(node.X - goal.X);
        var y = Math.Abs(node.Y - goal.Y);

        return x + y;
    }

    public bool EqualsOtherNode(Cell a, Cell b)
    {
        return a.X == b.X && a.Y == b.Y;
    }
}

public static class Solver
{
    public static void Solve()
    {
        SolvePart1();
        SolvePart2();
    }

    private static void SolvePart1()
    {
        var lines = File.ReadAllLines("Day12/input.txt").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        var grid = new Grid(lines);

        var aStar = new AStar<Cell>(grid);

        var path = aStar.GetPath(grid.Start, grid.End);

        Console.WriteLine($"Path has {path.Count} steps");
    }

    private static void SolvePart2()
    {
        var lines = File.ReadAllLines("Day12/input.txt").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        var grid = new Grid(lines);

        var aStar = new AStar<Cell>(grid);

        var possibleStarts =
            grid.Cells.SelectMany(c => c)
                .Where(c => c.Height == 'a')
                .ToArray();

        Console.WriteLine($"There are {possibleStarts.Length} possible starts");

        var shortestPath = possibleStarts.Select((x,i) =>
            {
                Console.WriteLine($"Processing {i}");
                return aStar.GetPath(x, grid.End);
            })
            .Where(x => x != null)
            .Select(x => x.Count - 1)
            .MinBy(x => x);
            
        Console.WriteLine($"Shortest path has {shortestPath} steps");
    }
}