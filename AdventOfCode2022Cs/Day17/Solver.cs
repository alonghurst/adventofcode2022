namespace AdventOfCode2022Cs.Day17;

public static class Solver
{
    public static bool[][,] _rocks = MakeRocks("####", ".#.;###;.#.;", "..#;..#;###", "#;#;#;#;", "##;##").ToArray();

    private static IEnumerable<bool[,]> MakeRocks(params string[] rocks)
    {
        foreach (var rock in rocks)
        {
            var rows = rock.Split(";", StringSplitOptions.RemoveEmptyEntries);

            var w = rows[0].Length;
            var h = rows.Length;

            var r = new bool[w, h];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    var ym = (h - y) - 1;

                    r[x, ym] = rows[y][x] == '#';
                }
            }

            yield return r;
        }
    }

    public static void Solve()
    {
        Solve("Day17/input.txt", 2022);
        Solve("Day17/input.txt", 1000000000000);
    }

    private static void Solve(string filename, long rocksToDo)
    {
        var rows = new List<bool[]>(1000000);

        var width = 7;

        var directions = ReadDirections(filename);

        var terse = true;
        var verbose = false;

        var dir = 0;
        var rockIndex = 0;

        while (rocksToDo-- > 0)
        {
            if (rocksToDo % 10000000000 == 0)
            {
                Console.WriteLine($"Did a chunk - {rocksToDo} remain");
            }

            var rock = _rocks[rockIndex++];

            if (rockIndex >= _rocks.Length)
            {
                rockIndex = 0;
            }

            var (rX, rY) = GetStartingPosition(width, rock, rows);
            var atRest = false;

            if (verbose)
            {
                Render(rock, width, rX, rY, $"Start rock ", rows);
            }

            while (!atRest)
            {
                var direction = directions[dir++];
                if (dir >= directions.Length)
                {
                    dir = 0;
                }

                MoveAndTestAtRest(width, rock, ref rX, ref rY, direction, 0, rows);

                if (verbose)
                {
                    Render(rock, width, rX, rY, $"Gas {(direction > 0 ? ">" : "<")} ", rows);
                }


                atRest = MoveAndTestAtRest(width, rock, ref rX, ref rY, 0, -1, rows);

                if (verbose)
                {
                    Render(rock, width, rX, rY, "Gravity", rows);
                }

                if (atRest)
                {
                    AddRock(rock, rX, rY, rows);
                        
                    if (!terse)
                    {
                        Console.Clear();
                        Render(null, width, 0, 0, "New State", rows);
                    }
                    break;
                }
            }
        }

        var height = FindHighestRock(rows) + 1;

        Console.WriteLine($"Highest rock: {height}");

    }

    private static void AddRock(bool[,] rock, int rX, int rY, List<bool[]> rows)
    {
        for (int y = 0; y < rock.GetLength(1); y++)
        {
            for (int x = 0; x < rock.GetLength(0); x++)
            {
                var pX = rX + x;
                var pY = rY + y;

                if (rock[x, y])
                {
                    rows[pY][pX] = true;
                }
            }
        }
    }

    private static bool MoveAndTestAtRest(int width, bool[,] rock, ref int rX, ref int rY, int dX, int dY, List<bool[]> rows)
    {
        var nX = rX + dX;
        var nY = rY + dY;

        var adjust = true;
        var atRest = false;

        for (int y = 0; y < rock.GetLength(1); y++)
        {
            for (int x = 0; x < rock.GetLength(0); x++)
            {
                var pX = nX + x;
                var pY = nY + y;

                var isRock = rock[x, y];

                if (pX < 0)
                {
                    adjust = false;
                }
                else if (isRock)
                {
                    if (pX >= width)
                    {
                        adjust = false;
                    }
                    else
                    {
                        var isWall = rows[pY][pX];

                        if (isWall)
                        {
                            if (dY != 0)
                            {
                                atRest = true;
                                adjust = false;
                            }
                            else
                            {
                                adjust = false;
                            }
                        }
                        else
                        {
                            if (pY < 0)
                            {
                                adjust = false;
                            }

                            if (pY == 0)
                            {
                                atRest = true;
                            }
                        }
                    }
                }

                if (!adjust)
                {
                    break;
                }
            }
            if (!adjust)
            {
                break;
            }
        }

        if (adjust)
        {
            rX = nX;
            rY = nY;
        }

        return atRest;
    }

    private static int[] ReadDirections(string filename)
    {
        var s = File.ReadAllText(filename).Trim();

        return s.Select(x => x == '<' ? -1 : 1).ToArray();
    }

    private static (int, int) GetStartingPosition(int width, bool[,] rock, List<bool[]> rows)
    {
        var rockH = rock.GetLength(1);

        var highestRock = FindHighestRock(rows);

        var sX = 2;
        var sY = highestRock + 4;


        while (rows.Count <= sY + rockH)
        {
            rows.Add(new bool[width]);
        }

        return (sX, sY);
    }

    private static int FindHighestRock(List<bool[]> rows)
    {
        for (int y = rows.Count - 1; y >= 0; y--)
        {
            if (rows[y].Any(x => x))
            {
                return y;
            }
        }

        return 0;
    }

    private static void Render(bool[,]? rock, int width, int sX, int sY, string desc, List<bool[]> rows)
    {
        Console.WriteLine($"======={desc} {sX} / {sY}");

        for (int y = rows.Count - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                var ym = y;

                var isCurrentRock = rock != null && IsRock(rock, sX, sY, x, ym);
                var isStaticRock = rows[y][x];

                var c = isCurrentRock ? "@" : isStaticRock ? "#" : ".";

                Console.Write(c);
            }
            Console.WriteLine();
        }
    }

    private static bool IsRock(bool[,] rock, int sX, int sY, int x, int y)
    {
        x -= sX;
        y -= sY;

        if (y < rock.GetLength(1) && x < rock.GetLength(0) && x >= 0 && y >= 0)
        {
            return rock[x, y];
        }

        return false;
    }

    private static void SolvePart2(string filename)
    {

    }
}