namespace AdventOfCode2022Cs.Day18;

public static class Solver
{

    public static void Solve()
    {
        Solve("Day18/input.txt");
    }

    private static void Solve(string filename)
    {
        var lines = File.ReadAllLines(filename);

        var (grid, cubes) = MakeCubes(lines);

        var sum = cubes.Sum(x => CountFaces(x, grid));

        Console.WriteLine($"There are {sum} faces exposed");


        var steam = ExploreSteam(grid);

        Console.WriteLine($"Steam faces: {steam.Count + 6}");
    }

    private static void Render(bool[,,] grid)
    {
        for (int z = 0; z < grid.GetLength(2); z++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    var g = grid[x, y, z];

                    Console.Write(g ? "#" : " ");
                }
                Console.WriteLine("   ");
            }

            Console.WriteLine($" ------------ {z + 1} ");

        }
    }

    private static (bool[,,] grid, (int x, int y, int z)[] cubes) MakeCubes(params string[] cubeInfo)
    {
        var cubes = cubeInfo
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(s =>
            {
                var split = s.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var x = Convert.ToInt32(split[0]);
                var y = Convert.ToInt32(split[1]);
                var z = Convert.ToInt32(split[2]);

                return (x, y, z);
            })
            .ToArray();

        // Make the grid 1 size larger so don't have to test bounds when querying
        var xMax = cubes.Max(c => c.x) + 2;
        var yMax = cubes.Max(c => c.y) + 2;
        var zMax = cubes.Max(c => c.z) + 2;

        var grid = new bool[xMax, yMax, zMax];

        foreach (var cube in cubes)
        {
            var (x, y, z) = cube;
            if (grid[x, y, z])
            {
                Console.WriteLine($"A cube already exists at {x} {y} {z}");
            }

            grid[x, y, z] = true;
        }

        return (grid, cubes);
    }

    private static int CountFaces((int x, int y, int z) cube, bool[,,] grid)
    {
        var faces = ExposedFaces(cube, grid).ToArray();

        return faces.Count();
    }

    private static (int x, int y, int z)[] _dirs = new (int x, int y, int z)[]
    {
        (1, 0, 0),
        (-1, 0, 0),
        (0, 1, 0),
        (0, -1, 0),
        (0, 0, 1),
        (0, 0, -1)
    };

    private static bool IsAirPocket(int x, int y, int z, bool[,,] grid)
    {
        if (IsExposed(x, y, z, grid))
        {
            var count = CountFaces((x, y, z), grid);

            return count == 0;
        }

        return false;
    }

    private static IEnumerable<(int, int, int)> ExposedFaces((int x, int y, int z) cube, bool[,,] grid)
    {
        foreach (var test in _dirs)
        {
            var tx = test.x + cube.x;
            var ty = test.y + cube.y;
            var tz = test.z + cube.z;

            if (IsExposed(tx, ty, tz, grid))
            {
                yield return (tx, ty, tz);
            }
        }
    }

    private static bool IsExposed(int x, int y, int z, bool[,,] grid)
    {
        if (x < 0 || y < 0 || z < 0)
        {
            return true;
        }

        if (x >= grid.GetLength(0) || y >= grid.GetLength(1) || z >= grid.GetLength(2))
        {
            return true;
        }

        return !grid[x, y, z];
    }

    private static List<(int x, int y, int z, int dX, int dY, int dZ)> ExploreSteam(bool[,,] grid)
    {
        var found = new HashSet<(int x, int y, int z, int dX, int dY, int dZ)>();
        var frontier = new Stack<(int x, int y, int z)>();
        var visited = new HashSet<(int, int, int)>();

        frontier.Push((0,0,0));

        while (frontier.Any())
        {
            var next = frontier.Pop();
            visited.Add(next);

            foreach (var direct in _dirs)
            {
                var x = next.x + direct.x;
                var y = next.y + direct.y;
                var z = next.z + direct.z;

                var n = (x, y, z);

                if (visited.Contains(n))
                {
                    continue;
                }

                if (x < 0 || y < 0 || z < 0)
                {
                    continue;
                }

                if (x >= grid.GetLength(0) || y >= grid.GetLength(1) || z >= grid.GetLength(2))
                {
                    continue;
                }

                var isExposed = IsExposed(x, y, z, grid);

                if (isExposed)
                {
                    frontier.Push(n);
                }
                else
                {
                    var f = (x, y, z, direct.x, direct.y, direct.z);

                    found.Add(f);
                }
            }
        }

        return found.ToList();
    }
}