namespace AdventOfCode2022Cs.Day22;

public static class Solver
{

    public static void Solve()
    {

        Solve("Day22/input.txt", false);

        //Solve2("Day21/test.txt");
    }

    private static void Solve(string filename, bool render)
    {
        var (board, movements) = Load(filename);


        var allMovements = CreateMovements(board);
        var pos = CreateStart(board);

        allMovements[pos.x, pos.y] = pos.d;
        
        foreach (var movement in movements)
        {
            pos = ProcessMove(board, pos, movement, allMovements);
            allMovements[pos.x, pos.y] = pos.d;

            if (render)
            {
                Render(board, allMovements, movement);
            }
        }
        
        var r = (1000 * (pos.y + 1)) + (4 * (pos.x + 1)) + pos.d;
        Console.WriteLine($"{pos.x} {pos.y} {pos.d} {r}");
    }

    private static Direction?[,] CreateMovements(Cell[,] board)
    {
        return new Direction?[board.GetLength(0), board.GetLength(1)];
    }

    private static (int x, int y, Direction d) ProcessMove(Cell[,] board, (int, int, Direction) pos, Move move, Direction?[,] allMovements)
    {
        var (x, y, d) = pos;

        if (move.Type == MoveType.Forward)
        {
            (x, y) = DoMove(pos, move.Spaces, board, allMovements);

            return (x, y, d);
        }
        else if (move.Type == MoveType.Left)
        {
            return (x, y, Left(d));
        }
        else if (move.Type == MoveType.Right)
        {
            return (x, y, Right(d));

        }

        throw new NotImplementedException();
    }

    private static (int x, int y) DoMove((int, int, Direction) pos, int moveSpaces, Cell[,] board, Direction?[,] allMovements)
    {
        var (x, y, d) = pos;

        do
        {
            var newPos = NewPosition(x, y, d, board, allMovements);

            if (newPos.HasValue)
            {
                (x, y) = newPos.Value;
            }
            else
            {
                break;
            }

        } while (--moveSpaces > 0);


        return (x, y);
    }

    private static (int x, int y)? NewPosition(int x, int y, Direction d, Cell[,] board, Direction?[,] allMovements)
    {
        var dir = DirectionFrom(d);

        var (nx, ny) = (x + dir.x, y + dir.y);

        Wrap(board, ref nx, ref ny);

        while (board[nx, ny] == Cell.Nothing)
        {
            nx += dir.x;
            ny += dir.y;
            Wrap(board, ref nx, ref ny);
        }

        if (board[nx, ny] == Cell.Wall)
        {
            return null;
        }


        allMovements[nx, ny] = d;

        return (nx, ny);
    }

    private static void Wrap(Cell[,] board, ref int nx, ref int ny)
    {
        if (nx < 0)
        {
            nx = board.GetLength(0) - 1;
        }

        if (ny < 0)
        {
            ny = board.GetLength(1) - 1;
        }

        if (nx >= board.GetLength(0))
        {
            nx = 0;
        }

        if (ny >= board.GetLength(1))
        {
            ny = 0;
        }
    }

    private static (int x, int y) DirectionFrom(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return (-1, 0);
            case Direction.Right:
                return (1, 0);
            case Direction.Up:
                return (0, -1);
            case Direction.Down:
                return (0, 1);
        }
        throw new NotImplementedException();
    }

    private static Direction Left(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:
                return Direction.Right;
            case Direction.Right:
                return Direction.Up;
            case Direction.Up:
                return Direction.Left;
            case Direction.Left:
                return Direction.Down;
        }

        throw new NotImplementedException();
    }

    private static Direction Right(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down:
                return Direction.Left;
            case Direction.Left:
                return Direction.Up;
            case Direction.Up:
                return Direction.Right;
            case Direction.Right:
                return Direction.Down;
        }

        throw new NotImplementedException();
    }

    private static (int x, int y, Direction d) CreateStart(Cell[,] board)
    {
        for (int x = 0; x < board.Length; x++)
        {
            if (board[x, 0] == Cell.Floor)
            {
                return (x, 0, Direction.Right);
            }
        }

        throw new InvalidOperationException();
    }

    private static void Render(Cell[,] board, Direction?[,] allMovements, Move? move = null)
    {
        Console.WriteLine($"============{(move != null ? move : "")}===============");

        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                Console.Write(Write(board[x, y], allMovements[x, y]));
            }

            Console.WriteLine();
        }
    }

    private static string Write(Cell cell, Direction? allMovement)
    {
        if (allMovement.HasValue)
        {
            switch (allMovement.Value)
            {
                case Direction.Down:
                    return "V";
                case Direction.Left:
                    return "<";
                case Direction.Right:
                    return ">";
                case Direction.Up:
                    return "^";
            }
        }

        switch (cell)
        {
            case Cell.Floor:
                return ".";
            case Cell.Nothing:
                return " ";
            case Cell.Wall:
                return "#";
        }

        throw new NotImplementedException();
    }

    private static (Cell[,] board, Move[] moves) Load(string filename)
    {
        var lines = File.ReadAllLines(filename);

        var w = lines.Max(x => x.Length);
        var h = lines.Length - 2;

        var board = new Cell[w, h];

        for (int y = 0; y < lines.Length - 2; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                board[x, y] = Parse(line[x]);
            }
        }

        var moves = new List<Move>();

        var moveWords = lines[^1];

        while (moveWords.Length > 0)
        {
            if (moveWords[0] == 'R')
            {
                moves.Add(new Move(MoveType.Right, 0));
                moveWords = moveWords.Substring(1);
            }
            else if (moveWords[0] == 'L')
            {
                moves.Add(new Move(MoveType.Left, 0));
                moveWords = moveWords.Substring(1);
            }
            else
            {
                var nr = moveWords.IndexOf('R');
                var nl = moveWords.IndexOf('L');

                var n = nr >= 0 && nr < nl ? nr : nl >= 0 ? nl : -1;

                var i = n >= 0 ? n : moveWords.Length;

                var s = moveWords.Substring(0, i);

                var m = Convert.ToInt32(s);

                moves.Add(new Move(MoveType.Forward, m));

                moveWords = moveWords.Substring(i);
            }
        }

        return (board, moves.ToArray());
    }

    private static Cell Parse(char c)
    {

        if (c == '#')
        {
            return Cell.Wall;
        }

        if (c == '.')
        {
            return Cell.Floor;
        }

        return Cell.Nothing;
    }

    public record Move(MoveType Type, int Spaces)
    {
        public override string ToString()
        {
            if (Type != MoveType.Forward)
            {
                return Type.ToString();
            }

            return $"F {Spaces.ToString()}";
        }
    }

    public enum MoveType
    {
        Forward,
        Left,
        Right
    }

    public enum Cell
    {
        Nothing = 0,
        Floor = 1,
        Wall = 2
    }

    public enum Direction
    {
        Down = 1,
        Up = 3,
        Left = 2,
        Right = 0
    }
}